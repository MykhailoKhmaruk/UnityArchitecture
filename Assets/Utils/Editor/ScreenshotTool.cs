/////////////////////////////////////////////////////////////////////////////////
//
// ScreenshotsTool by Mykhailo Khmaruk 
// https://www.linkedin.com/in/mykhailo-khmaruk-5b9522140/
//
//
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor; //  EditorCoroutines

namespace EditorUtils
{
    public class ScreenshotsTool : EditorWindow
    {
        private int _width = 1920;
        private int _height = 1080;
        private string _screenshotName = "screenshot";
        private string _folderPath = "Screenshots";
        private bool _useNumber = false;
        private int _numberOfScreenShot = 0;
        private static bool _isLandScape = false;

        private float _delaySeconds = 2f;

        private List<string> _listOfNames = new List<string>();
        private string _newName = string.Empty;

        static bool useRezolution = true;

        static object s_ScreenshotsTool_instance;
        static Type s_GameViewType;
        static MethodInfo s_GameView_SizeSelectionCallback;
        static Type s_GameViewSizesType;
        static MethodInfo s_GameViewSizes_GetGroup;

        private void OnEnable()
        {
            s_GameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            s_GameView_SizeSelectionCallback = s_GameViewType.GetMethod("SizeSelectionCallback",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            s_GameViewSizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            s_GameViewSizes_GetGroup = s_GameViewSizesType.GetMethod("GetGroup");

            var gameViewSizesType = typeof(Editor).Assembly.GetType("UnityEditor.ScriptableSingleton`1").MakeGenericType(s_GameViewSizesType);
            var instanceProperty = gameViewSizesType.GetProperty("instance");
            s_ScreenshotsTool_instance = instanceProperty.GetValue(null);
        }

        [MenuItem("Tools/Screenshot Tool")]
        public static void ShowWindow()
        {
            GetWindow<ScreenshotsTool>("ScreenshotsTool");
        }

        private void OnGUI()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height),
                new Color(5 / 255f, 10 / 255f, 10 / 255f));

            #region ToggleGroup

            EditorGUILayout.Space(10);
            useRezolution = EditorGUILayout.BeginToggleGroup("Use resolution", useRezolution);
            EditorGUILayout.Space(10);
            GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            _width = EditorGUILayout.IntField("Width", _width);
            _height = EditorGUILayout.IntField("Height", _height);
            EditorGUILayout.Space(5);

#if UNITY_ANDROID
            _isLandScape = EditorGUILayout.Toggle("IsLandScape", _isLandScape);
#endif            
            EditorGUILayout.Space(10);
            _screenshotName = EditorGUILayout.TextField("Screenshot Name", _screenshotName);
            EditorGUILayout.Space(5);
            _folderPath = EditorGUILayout.TextField("Folder Path", _folderPath);
            EditorGUILayout.Space(5);
            _useNumber = EditorGUILayout.BeginToggleGroup("USE PREFIX NUMBER?", _useNumber);
            _numberOfScreenShot = EditorGUILayout.IntField("PREFIX Number", _numberOfScreenShot);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Take Screenshot", GUILayout.Width(200), GUILayout.Height(30)))
            {
                TakeScreenshot();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();

            #endregion

            EditorGUILayout.Space(10);

            #region ToggleGroup2

            useRezolution = !EditorGUILayout.BeginToggleGroup("Use names of resolution", !useRezolution);
            EditorGUILayout.Space(5);
            
            EditorGUILayout.Space(5);
            _useNumber = EditorGUILayout.BeginToggleGroup("USE PREFIX NUMBER?", _useNumber);
            _numberOfScreenShot = EditorGUILayout.IntField("PREFIX Number", _numberOfScreenShot);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("List of Names", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < _listOfNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _listOfNames[i] = EditorGUILayout.TextField(_listOfNames[i]);
                if (GUILayout.Button("Remove"))
                {
                    _listOfNames.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Add New Name of your resolution Here", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            _newName = EditorGUILayout.TextField("New Name", _newName);

            if (GUILayout.Button("Add"))
            {
                if (!string.IsNullOrEmpty(_newName))
                {
                    if (!_listOfNames.Contains(_newName))
                    {
                        _listOfNames.Add(_newName);
                        _newName = string.Empty;
                    }
                    else
                    {
                        Debug.LogWarning("The same name already present");
                    }
                }
            }

            EditorGUILayout.Space(5);
            _delaySeconds = EditorGUILayout.FloatField("Delay", _delaySeconds);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("TAKE SCREENSHOTS FOR ALL RESOLUTION IN THIS LIST", 
                    GUILayout.Width(400), GUILayout.Height(30)))
            {
                EditorCoroutineUtility.StartCoroutine(TakeScreenshotsWithDelay(), this);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();

            #endregion
        }

        private void TakeScreenshot()
        {
            SetGameViewSize(_width, _height);
            if (!System.IO.Directory.Exists(_folderPath))
            {
                System.IO.Directory.CreateDirectory(_folderPath);
            }

            var prefNum = _useNumber ? _numberOfScreenShot.ToString() + "_" : "";
            
            string filePath = System.IO.Path.Combine(_folderPath,
                $"{prefNum}{_screenshotName}_{_width}x{_height}.png");
            _numberOfScreenShot += 1;
            ScreenCapture.CaptureScreenshot(filePath);

            Debug.Log($"Screenshot taken and saved to: {filePath}");
        }

        private IEnumerator TakeScreenshotsWithDelay()
        {
            var namesCopy = new List<string>(_listOfNames);
            foreach (var name in namesCopy)
            {
                TakeScreenshot(name);
                yield return new EditorWaitForSeconds(_delaySeconds);
            }
        }

        private void TakeScreenshot(string nameOfView)
        {
            SwitchToResolution(nameOfView);
            if (!System.IO.Directory.Exists(_folderPath))
            {
                System.IO.Directory.CreateDirectory(_folderPath);
            }

            string filePath = System.IO.Path.Combine(_folderPath,
                $"{_numberOfScreenShot}_{_screenshotName}_{nameOfView}.png");
            _numberOfScreenShot += 1;
            ScreenCapture.CaptureScreenshot(filePath);

            Debug.Log($"Screenshot taken and saved to: {filePath}");
        }

        static void SetGameViewSize(int width, int height)
        {
            SwitchToResolution(width, height);
        }

        static void SwitchToResolution(string nameOfResolution)
        {
            switch (ScreenshotsTool.GetCurrentGroupType())
            {
                default:
                    ScreenshotsTool.TrySetSize(nameOfResolution);
                    break;
                case GameViewSizeGroupType.Android:
                    ScreenshotsTool.TrySetSize(nameOfResolution);
                    break;
                case GameViewSizeGroupType.iOS:
                    ScreenshotsTool.TrySetSize(nameOfResolution);
                    break;
            }
        }

        static void SwitchToResolution(int width, int height)
        {
            string value = width + "x" + height;
            switch (ScreenshotsTool.GetCurrentGroupType())
            {
                default:
                    ScreenshotsTool.TrySetSize(value);
                    break;
                case GameViewSizeGroupType.Android:
                    ScreenshotsTool.TrySetSize(value);
                    break;
                case GameViewSizeGroupType.iOS:
                    ScreenshotsTool.TrySetSize(value);
                    break;
            }
        }

        private static bool TrySetSize(string sizeText)
        {
            GameViewSizeGroupType currentGroup = GetCurrentGroupType();
            var foundIndex = FindSize(currentGroup, sizeText);
            if (foundIndex < 0)
            {
                Debug.LogError($"Size {sizeText} was not found in game view settings");
                return false;
            }

            SetSizeIndex(foundIndex);
            return true;
        }

        private static void SetSizeIndex(int index)
        {
            EditorWindow currentWindow = focusedWindow;

            EditorWindow gv = EditorWindow.GetWindow(s_GameViewType);
            s_GameView_SizeSelectionCallback.Invoke(gv, new object[] { index, null });

            if (currentWindow != null)
                currentWindow.Focus();
        }

        private static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
        {
            var group = GetGroup(sizeGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            if (getDisplayTexts == null) return -1;
            if (getDisplayTexts.Invoke(group, null) is not string[] displayTexts) return -1;
            for (int i = 0; i < displayTexts.Length; i++)
            {
                string display = displayTexts[i];

                if (!display.Contains(text)) continue;
                if (!_isLandScape) return i;
                if ((i + 1) < displayTexts.Length)
                {
                    return ++i;
                }

                return i;
            }

            return -1;
        }

        static object GetGroup(GameViewSizeGroupType type)
        {
            return s_GameViewSizes_GetGroup.Invoke(s_ScreenshotsTool_instance, new object[] { (int)type });
        }

        private static GameViewSizeGroupType GetCurrentGroupType()
        {
#if UNITY_STANDALONE
            return GameViewSizeGroupType.Standalone;
#elif UNITY_ANDROID
            return GameViewSizeGroupType.Android;
#elif UNITY_IOS
            return GameViewSizeGroupType.iOS;
#endif
            //YOU CAN ADD MORE PLATFORMS HERE
        }
    }
}
