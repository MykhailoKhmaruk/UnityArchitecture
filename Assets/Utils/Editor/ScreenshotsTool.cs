using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MykhailoUtils
{
    public class ScreenshotsTool : EditorWindow
    {
        private int _width = 1920;
        private int _height = 1080;
        private string _screenshotName = "screenshot";
        private string _folderPath = "Screenshots";
        private int _numberOfScreenShot = 0;
        private static bool _isLandScape = false;

        private string _listOfNames = "";
        private bool _useListOfNames = false;
        
        static object s_ScreenshotsTool_instance;

        static Type s_GameViewType;
        static MethodInfo s_GameView_SizeSelectionCallback;

        static Type s_GameViewSizesType;
        static MethodInfo s_GameViewSizes_GetGroup;

        static Type s_GameViewSizeSingleType;

        static ScreenshotsTool( )
        {
            s_GameViewType = typeof( UnityEditor.Editor ).Assembly.GetType( "UnityEditor.GameView" );
            s_GameView_SizeSelectionCallback = s_GameViewType.GetMethod( "SizeSelectionCallback", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
            s_GameViewSizesType = typeof( UnityEditor.Editor ).Assembly.GetType( "UnityEditor.GameViewSizes" );
            s_GameViewSizeSingleType = typeof( ScriptableSingleton<> ).MakeGenericType( s_GameViewSizesType );
            s_GameViewSizes_GetGroup = s_GameViewSizesType.GetMethod( "GetGroup" );

            var instanceProp = s_GameViewSizeSingleType.GetProperty("instance");
            s_ScreenshotsTool_instance = instanceProp.GetValue( null, null );
        }
        


        [MenuItem("Tools/ScreenshotsTool")]
        public static void ShowWindow()
        {
            GetWindow<ScreenshotsTool>("ScreenshotsTool");
        }

        void OnGUI()
        {
            GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            _width = EditorGUILayout.IntField("Width", _width);
            _height = EditorGUILayout.IntField("Height", _height);
            _isLandScape = EditorGUILayout.Toggle("IsLandScape", _isLandScape);
            
            
            EditorGUILayout.Space(10);
            _screenshotName = EditorGUILayout.TextField("Screenshot Name", _screenshotName);
            EditorGUILayout.Space(5);
            _folderPath = EditorGUILayout.TextField("Folder Path", _folderPath);
            EditorGUILayout.Space(5);
            _numberOfScreenShot = EditorGUILayout.IntField("Number Of ScreenShot", _numberOfScreenShot);
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Take Screenshot"))
            {
                TakeScreenshot();
            }
            
            EditorGUILayout.Space(10);
            
            _isLandScape = EditorGUILayout.Toggle("Use List of Names", _useListOfNames);
            EditorGUILayout.Space(5);
            _listOfNames = EditorGUILayout.TextField("List of Names", _listOfNames);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Make Screenshots for all resolution in this List"))
            {
                TakeScreenshot();
            }
        }

        void TakeScreenshot()
        {
            SetGameViewSize(_width, _height);
            if (!System.IO.Directory.Exists(_folderPath))
            {
                System.IO.Directory.CreateDirectory(_folderPath);
            }

            Screen.SetResolution(_width, _height, false);

            //TODO Додати довання цифр до скріншотів
            string filePath = System.IO.Path.Combine(_folderPath, $"{_numberOfScreenShot}_{_screenshotName}_{_width}x{_height}.png");
            _numberOfScreenShot += 1;
            ScreenCapture.CaptureScreenshot(filePath);

            Debug.Log($"Screenshot taken and saved to: {filePath}");
        }

        void SetGameViewSize(int width, int height)
        {
            SwitchToResolution(width, height);
        }

        static void SwitchToResolution(int width, int height)
        {
            string value = width + "x" +height;
            switch( ScreenshotsTool.GetCurrentGroupType( ) )
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
        
        public static bool TrySetSize( string sizeText )
        {
            GameViewSizeGroupType currentGroup = GetCurrentGroupType( );
            int foundIndex = FindSize( currentGroup, sizeText );
            if( foundIndex < 0 )
            {
                UnityEngine.Debug.LogError( $"Size {sizeText} was not found in game view settings" );
                return false;
            }

            SetSizeIndex(foundIndex);
            return true;
        }
        
        public static void SetSizeIndex( int index )
        {
            EditorWindow currentWindow = EditorWindow.focusedWindow;
            SceneView lastSceneView = SceneView.lastActiveSceneView;

            EditorWindow gv = EditorWindow.GetWindow( s_GameViewType );
            s_GameView_SizeSelectionCallback.Invoke( gv, new object[] { index, null } );

            // if( lastSceneView != null )
            //     lastSceneView.Focus( );
            if( currentWindow != null )
                currentWindow.Focus( );
        }
        
        private static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
        {
            var group = GetGroup(sizeGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            for (int i = 0; i < displayTexts.Length; i++)
            {
                string display = displayTexts[i];
                // int pren = display.IndexOf('(');
                // if (pren != -1)
                //     display = display.Substring(0,
                //         pren - 1); 
                // -1 to remove the space that’s before the prens. This is very implementation-depdenent
                // if (display == text)
                if (display.Contains(text))
                {
                    if (_isLandScape)
                    {
                        if ((i + 1) < displayTexts.Length)
                        {
                            return ++i;
                        }
                    }

                    return i;
                }
            }

            return -1;
        }

        static object GetGroup( GameViewSizeGroupType type )
        {
            return s_GameViewSizes_GetGroup.Invoke( s_ScreenshotsTool_instance, new object[] { (int)type } );
        }

        public static GameViewSizeGroupType GetCurrentGroupType()
        {
#if UNITY_STANDALONE
            return GameViewSizeGroupType.Standalone;
#elif UNITY_ANDROID
            return GameViewSizeGroupType.Android;
#elif UNITY_IOS
            return GameViewSizeGroupType.iOS;
#endif
            //YOU CAN ADD 
        }
    }

    [Serializable]
    public struct ResolutionsStruct
    {
        public int width;
        public int height;
    }
}