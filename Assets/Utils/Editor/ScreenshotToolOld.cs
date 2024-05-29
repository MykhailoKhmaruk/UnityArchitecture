// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using UnityEditor;
// using UnityEngine;
//
// public class ScreenshotTool : EditorWindow
// {
//     // private int width = 1920;
//     // private int height = 1080;
//     // private string screenshotName = "screenshot";
//     // private string folderPath = "Screenshots";
//     private List<ResolutionsStruct> resolutions = new List<ResolutionsStruct>();
//     private SerializedObject serializedObject;
//     private SerializedProperty resolutionsProperty;
//
//     private string screenshotName = "screenshot";
//     private string folderPath = "Screenshots";
//
//     [Serializable]
//     public struct ResolutionsStruct
//     {
//         public int width;
//         public int height;
//     }
//
//     [MenuItem("Tools/ScreenshotTool")]
//     public static void ShowWindow()
//     {
//         GetWindow<ScreenshotTool>("ScreenshotTool");
//     }
//
//     private void OnEnable()
//     {
//         serializedObject = new SerializedObject(this);
//         resolutionsProperty = serializedObject.FindProperty("resolutions");
//     }
//
//     private void OnGUI()
//     {
//         serializedObject.Update();
//
//         GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);
//
//         // Display the list of resolutions
//         for (int i = 0; i < resolutions.Count; i++)
//         {
//             EditorGUILayout.BeginHorizontal();
//             var resolutionsStruct = resolutions[i];
//             GUILayout.Label("Width", GUILayout.Width(50));
//             resolutionsStruct.width = EditorGUILayout.IntField(resolutionsStruct.width, GUILayout.Width(50));
//             // EditorGUILayout.Space(5);
//             GUILayout.Label("Height", GUILayout.Width(50));
//             resolutionsStruct.height = EditorGUILayout.IntField(resolutionsStruct.height, GUILayout.Width(50));
//             // EditorGUILayout.Space(5);
//             if (GUILayout.Button("Remove"))
//             {
//                 resolutions.RemoveAt(i);
//             }
//
//             EditorGUILayout.EndHorizontal();
//         }
//
//         if (GUILayout.Button("Add Resolution"))
//         {
//             resolutions.Add(new ResolutionsStruct() { width = 1920, height = 1080 });
//         }
//
//         if (GUILayout.Button("Take Screenshots"))
//         {
//             TakeScreenshots();
//         }
//
//         screenshotName = EditorGUILayout.TextField("Screenshot Name", screenshotName);
//         folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
//
//         serializedObject.ApplyModifiedProperties();
//     }
//
//     private void TakeScreenshots()
//     {
//         // Create the folder if it doesn't exist
//         if (!System.IO.Directory.Exists(folderPath))
//         {
//             System.IO.Directory.CreateDirectory(folderPath);
//         }
//
//         foreach (var resolution in resolutions)
//         {
//             SetGameViewSize(resolution.width, resolution.height);
//
//             // Capture the screenshot
//             string filePath = System.IO.Path.Combine(folderPath, $"{screenshotName}_{resolution.width}x{resolution.height}.png");
//             ScreenCapture.CaptureScreenshot(filePath);
//             Debug.Log($"Screenshot taken and saved to: {filePath}");
//         }
//     }
//
//     private void SetGameViewSize(int width, int height)
//     {
//         System.Type gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
//         var gameViewWindow = EditorWindow.GetWindow(gameViewType);
//
//         MethodInfo setSize = gameViewType.GetMethod("SetMainPlayModeViewSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//         if (setSize != null)
//         {
//             setSize.Invoke(gameViewWindow, new object[] { 300, 200 });
//         }
//     }
//     
//     // private List<ResolutionsStruct> resolutions = new List<ResolutionsStruct>();
//     // // private SerializedObject serializedObject;
//     // // private SerializedProperty resolutionsProperty;
//     //
//     //
//     // [MenuItem("Tools/Screenshot Tool")]
//     // public static void ShowWindow()
//     // {
//     //     GetWindow<ScreenshotTool>("Screenshot Tool");
//     // }
//
//     // private void OnEnable()
//     // {
//     //     serializedObject = new SerializedObject(this);
//     //     resolutionsProperty = serializedObject.FindProperty("resolutions");
//     // }
//     
//     // void OnGUI()
//     // {
//     //
//     //     
//     //     // GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);
//     //     //
//     //     // width = EditorGUILayout.IntField("Width", width);
//     //     // height = EditorGUILayout.IntField("Height", height);
//     //     // screenshotName = EditorGUILayout.TextField("Screenshot Name", screenshotName);
//     //     // folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
//     //     // resolutions = EditorGUILayout.IntField(int,int)
//     //     // if (GUILayout.Button("Take Screenshot"))
//     //     // {
//     //     //     TakeScreenshot();
//     //     // }
//     // }
//
//     // void TakeScreenshot()
//     // {
//     //     SetGameViewSize(width, height);
//     //     // Create the folder if it doesn't exist
//     //     if (!System.IO.Directory.Exists(folderPath))
//     //     {
//     //         System.IO.Directory.CreateDirectory(folderPath);
//     //     }
//     //
//     //     // Set the resolution
//     //     Screen.SetResolution(width, height, false);
//     //
//     //     // Capture the screenshot
//     //     string filePath = System.IO.Path.Combine(folderPath, $"{screenshotName}_{width}x{height}.png");
//     //     ScreenCapture.CaptureScreenshot(filePath);
//     //     
//     //     Debug.Log($"Screenshot taken and saved to: {filePath}");
//     // }
//     //
//     // void SetGameViewSize(int width, int height)
//     // {
//     //     // Отримайте тип GameView
//     //     System.Type gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.Game");
//     //     var gameViewWindow = EditorWindow.GetWindow(gameViewType);
//     //
//     //     // Використовуйте рефлексію для виклику методу SetMainPlayModeViewSize
//     //     MethodInfo setSize = gameViewType.GetMethod("SetMainPlayModeViewSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//     //     if (setSize != null)
//     //     {
//     //         setSize.Invoke(gameViewWindow, new object[] { width, height });
//     //     }
//     // }
//  
//     // [Serializable]
//     // public struct ResolutionsStruct
//     // {
//     //     public int width;
//     //     public int height;
//     // }
//     
//     // public static void AddTestSize()
//     // {
//     //     AddCustomSize(GameViewSizeType.AspectRatio, GameViewSizeGroupType.Standalone, 123, 456, "Test size");
//     // }
//     //
//     // public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
//     // {
//     //     // goal:
//     //     // var group = ScriptableSingleton<GameViewSizes>.instance.GetGroup(sizeGroupType);
//     //     // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);
//     //
//     //     var asm = typeof(Editor).Assembly;
//     //     var sizesType = asm.GetType("UnityEditor.GameViewSizes");
//     //     var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
//     //     var instanceProp = singleType.GetProperty("instance");
//     //     var getGroup = sizesType.GetMethod("GetGroup");
//     //     var instance = instanceProp.GetValue(null, null);
//     //     var group = getGroup.Invoke(instance, new object[] { (int)sizeGroupType });
//     //     var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
//     //     var gvsType = asm.GetType("UnityEditor.GameViewSize");
//     //     var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
//     //     var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
//     //     addCustomSize.Invoke(group, new object[] { newSize });
//     // }
// }