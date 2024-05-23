using PhotoshopFile;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PSDLayersExporter : EditorWindow
{
    private Texture2D _psdFile;
    private Vector2 _scrollPos;
    private PsdFile psd;
    private int _atlasSize = 4096;
    private float _pixelsToUnitSize = 100.0f;
    private bool _importIntoSelected = false;
    private string _fileName;

    private string _gameObjectName = "";
    private string _atlasName = "";

    private Transform _selectedTransform;

    [MenuItem("Tools/Import layers from PSD")]
    public static void ShowWindow()
    {
        var window = GetWindow<PSDLayersExporter>();
        window.title = "PSD Import";
        window.Show();
    }

    public void OnGUI()
    {
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height),
            new Color(10 / 255f, 10 / 255f, 10 / 255f));
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        // GUILayout.FlexibleSpace();
        _psdFile = (Texture2D)EditorGUILayout.ObjectField("*.PSD File", _psdFile, typeof(Texture2D), true);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        bool changed = EditorGUI.EndChangeCheck();

        if (_psdFile != null)
        {
            if (changed)
            {
                var path = AssetDatabase.GetAssetPath(_psdFile);

                if (path.ToUpper().EndsWith(".PSD"))
                {
                    psd = new PsdFile(path, Encoding.Default);
                    _fileName = Path.GetFileNameWithoutExtension(path);
                    _gameObjectName = _fileName;
                }
                else
                {
                    psd = null;
                }
            }

            if (psd != null)
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                foreach (var layer in psd.Layers.Where(layer => layer.Name != "</Layer set>" && layer.Name != "</Layer group>"))
                {
                    layer.Visible = EditorGUILayout.ToggleLeft(layer.Name, layer.Visible);
                }

                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("EXPORT VISIBLE LAYERS", GUILayout.Height(25)))
                {
                    ExportLayers();
                }

                EditorGUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("IF YOU WONT EXPORT IN ATLAS", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                
                _atlasSize = EditorGUILayout.IntField("Max. atlas size", _atlasSize);
                if (!((_atlasSize != 0) && ((_atlasSize & (_atlasSize - 1)) == 0)))
                {
                    EditorGUILayout.HelpBox("Atlas size should be a power of 2", MessageType.Warning);
                }
                _atlasName = EditorGUILayout.TextField("Name for ATLAS", _atlasName);
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Create atlas",GUILayout.Height(25)))
                {
                    CreateAtlas();
                }

                EditorGUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("         IF YOU WONT EXPORT IN SCENE\nAND CREATE SPRITE RENDERER OBJECTS", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                
                _gameObjectName = EditorGUILayout.TextField("Name for root gameObject", _gameObjectName);
                
                _pixelsToUnitSize = EditorGUILayout.FloatField("Pixels To Unit Size", _pixelsToUnitSize);
                if (_pixelsToUnitSize <= 0)
                {
                    EditorGUILayout.HelpBox("Pixels To Unit Size should be greater than 0.", MessageType.Warning);
                }

                _importIntoSelected = EditorGUILayout.Toggle("Import to selected object", _importIntoSelected);

                if (GUILayout.Button("EXPORT VISIBLE LAYERS\n  AND CREATE SPRITES"))
                {
                    CreateSprites();
                }
                EditorGUILayout.Space(20);
            }
            else
            {
                EditorGUILayout.HelpBox("This texture is not a PSD file.", MessageType.Error);
            }
        }
    }

    private Texture2D CreateTexture(Layer layer)
    {
        if ((int)layer.Rect.width == 0 || (int)layer.Rect.height == 0)
            return null;

        Texture2D tex = new Texture2D((int)layer.Rect.width, (int)layer.Rect.height, TextureFormat.RGBA32, true);
        Color32[] pixels = new Color32[tex.width * tex.height];

        Channel red = (from l in layer.Channels where l.ID == 0 select l).First();
        Channel green = (from l in layer.Channels where l.ID == 1 select l).First();
        Channel blue = (from l in layer.Channels where l.ID == 2 select l).First();
        Channel alpha = layer.AlphaChannel;

        for (int i = 0; i < pixels.Length; i++)
        {
            byte r = red.ImageData[i];
            byte g = green.ImageData[i];
            byte b = blue.ImageData[i];
            byte a = 255;

            if (alpha != null)
                a = alpha.ImageData[i];

            int mod = i % tex.width;
            int n = ((tex.width - mod - 1) + i) - mod;
            pixels[pixels.Length - n - 1] = new Color32(r, g, b, a);
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }

    private void ExportLayers()
    {
        foreach (Layer layer in psd.Layers)
        {
            if (layer.Visible && !layer.Name.StartsWith("mask_"))
            {
                Texture2D tex = CreateTexture(layer);
                if (tex == null) continue;
                SaveAsset(tex, "_" + layer.Name);
                DestroyImmediate(tex);
            }
        }
    }

    private void CreateAtlas()
    {
        // Texture2D[] textures = (from layer in psd.Layers where layer.Visible select CreateTexture(layer) into tex where tex != null select tex).ToArray();

        List<Texture2D> textures = new List<Texture2D>();

        // Track the spriteRenderers created via a List
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        int zOrder = 0;
        GameObject root = new GameObject(_gameObjectName);
        foreach (var layer in psd.Layers)
        {
            if (layer.Visible && layer.Rect.width > 0 && layer.Rect.height > 0 && !layer.Name.StartsWith("mask_"))
            {
                Texture2D tex = CreateTexture(layer);
                // Add the texture to the Texture Array
                textures.Add(tex);

                GameObject go = new GameObject(layer.Name);
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                go.transform.position = new Vector3((layer.Rect.width / 2 + layer.Rect.x) / _pixelsToUnitSize,
                    (-layer.Rect.height / 2 - layer.Rect.y) / _pixelsToUnitSize, 0);
                // Add the sprite renderer to the SpriteRenderer Array
                spriteRenderers.Add(sr);
                sr.sortingOrder = zOrder++;
                go.transform.parent = root.transform;
            }
        }

        // The output of PackTextures returns a Rect array from which we can create our sprites
        Rect[] rects;
        Texture2D atlas = new Texture2D(_atlasSize, _atlasSize);
        Texture2D[] textureArray = textures.ToArray();
        rects = atlas.PackTextures(textureArray, 2, _atlasSize);
        List<SpriteMetaData> Sprites = new List<SpriteMetaData>();

        // For each rect in the Rect Array create the sprite and assign to the SpriteMetaData
        for (int i = 0; i < rects.Length; i++)
        {
            // add the name and rectangle to the dictionary
            SpriteMetaData smd = new SpriteMetaData();
            smd.name = spriteRenderers[i].name;
            smd.rect = new Rect(rects[i].xMin * atlas.width, rects[i].yMin * atlas.height, rects[i].width * atlas.width,
                rects[i].height * atlas.height);
            smd.pivot = new Vector2(0.5f, 0.5f); // Center is default otherwise layers will be misaligned
            smd.alignment = (int)SpriteAlignment.Center;
            Sprites.Add(smd);
        }

        // Need to load the image first
        string assetPath = AssetDatabase.GetAssetPath(_psdFile);
        string path = Path.Combine(Path.GetDirectoryName(assetPath),
            Path.GetFileNameWithoutExtension(assetPath) + "_atlas" + ".png");

        byte[] buf = atlas.EncodeToPNG();
        File.WriteAllBytes(path, buf);
        AssetDatabase.Refresh();

        // Get our texture that we loaded
        atlas = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        // Make sure the size is the same as our atlas then create the spritesheet
        textureImporter.maxTextureSize = _atlasSize;
        textureImporter.spritesheet = Sprites.ToArray();
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
        textureImporter.spritePixelsPerUnit = _pixelsToUnitSize;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        // For each rect in the Rect Array create the sprite and assign to the SpriteRenderer
        for (int j = 0; j < textureImporter.spritesheet.Length; j++)
        {
            // Debug.Log(textureImporter.spritesheet[j].rect);
            Sprite spr = Sprite.Create(atlas, textureImporter.spritesheet[j].rect, textureImporter.spritesheet[j].pivot,
                _pixelsToUnitSize); // The 100.0f is for the pixels to unit, maybe make that a public variable for the user to change before hand?

            // Add the sprite to the sprite renderer
            spriteRenderers[j].sprite = spr;
        }

        foreach (Texture2D tex in textureArray)
        {
            DestroyImmediate(tex);
        }
    }

    private void CreateSprites()
    {
        if (_importIntoSelected)
        {
            _selectedTransform = Selection.activeTransform;
        }

        int zOrder = 0;
        GameObject root = new GameObject(_gameObjectName);
        if (_importIntoSelected && _selectedTransform != null)
        {
            root.transform.parent = _selectedTransform;
        }

        foreach (var layer in psd.Layers)
        {
            if (layer.Visible && layer.Rect.width > 0 && layer.Rect.height > 0)
            {
                GameObject go = new GameObject(layer.Name);
                go.transform.position = new Vector3((layer.Rect.width / 2 + layer.Rect.x) / _pixelsToUnitSize,
                    (-layer.Rect.height / 2 - layer.Rect.y) / _pixelsToUnitSize, 0);
                go.transform.parent = root.transform;

                if (go.name.StartsWith("mask_"))
                {
                    var bc = go.AddComponent<BoxCollider2D>();
                    bc.size = new Vector2(layer.Rect.width / _pixelsToUnitSize, layer.Rect.height / _pixelsToUnitSize);
                }
                else
                {
                    Texture2D tex = CreateTexture(layer);
                    Sprite spr = SaveAsset(tex, "_" + layer.Name);
                    DestroyImmediate(tex);
                    SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = spr;
                    sr.sortingOrder = zOrder++;
                }
            }
        }
    }

    private void CreateImages()
    {
        if (_importIntoSelected)
        {
            _selectedTransform = Selection.activeTransform;
        }

        int zOrder = 0;
        GameObject root = new GameObject(_gameObjectName);
        root.transform.localPosition = new Vector3(0, 0, 0);
        var rtransf = root.AddComponent<RectTransform>();
        if (_importIntoSelected && _selectedTransform != null)
            root.transform.parent = _selectedTransform;
        rtransf.anchorMin = new Vector2(0f, 0f);
        rtransf.anchorMax = new Vector2(1f, 1f);
        rtransf.pivot = new Vector2(0.5f, 0.5f);
        rtransf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        rtransf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        rtransf.sizeDelta = Vector2.zero;
        rtransf.localPosition = Vector3.zero;

        foreach (var layer in psd.Layers)
        {
            if (layer.Visible && layer.Rect.width > 0 && layer.Rect.height > 0)
            {
                var targetOrder = zOrder++;
                Texture2D tex = CreateTexture(layer);
                Sprite spr = SaveAsset(tex, "_" + layer.Name);
                DestroyImmediate(tex);

                GameObject go = new GameObject(layer.Name);
                go.transform.parent = root.transform;
                go.transform.SetSiblingIndex(targetOrder);
                UnityEngine.UI.Image image = go.AddComponent<UnityEngine.UI.Image>();
                image.sprite = spr;
                image.rectTransform.localPosition = new Vector3((layer.Rect.x), (layer.Rect.y * -1) - layer.Rect.height,
                    targetOrder * 5);
                image.rectTransform.anchorMax = new Vector2(0, 1);
                image.rectTransform.anchorMin = new Vector2(0, 1);
                image.rectTransform.pivot = new Vector2(0f, 0f);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, layer.Rect.width);
                image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, layer.Rect.height);
                //print(sr.gameObject.name);
            }
        }
    }

    private Sprite SaveAsset(Texture2D tex, string suffix)
    {
        string assetPath = AssetDatabase.GetAssetPath(_psdFile);
        string path = Path.Combine(Path.GetDirectoryName(assetPath),
            Path.GetFileNameWithoutExtension(assetPath) + suffix + ".png");

        byte[] buf = tex.EncodeToPNG();
        File.WriteAllBytes(path, buf);
        AssetDatabase.Refresh();
        // Load the texture so we can change the type
        AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
        textureImporter.spritePixelsPerUnit = _pixelsToUnitSize;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        return (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
    }
}