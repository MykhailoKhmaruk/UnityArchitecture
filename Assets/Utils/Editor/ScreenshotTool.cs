using UnityEngine;
using UnityEditor;

public class ScreenshotTool : EditorWindow
{
    private int width = 1920;
    private int height = 1080;
    private string screenshotName = "screenshot";
    private string folderPath = "Screenshots";

    [MenuItem("Tools/Screenshot Tool")]
    public static void ShowWindow()
    {
        GetWindow<ScreenshotTool>("Screenshot Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Screenshot Settings", EditorStyles.boldLabel);

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        screenshotName = EditorGUILayout.TextField("Screenshot Name", screenshotName);
        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        if (GUILayout.Button("Take Screenshot"))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        // Create the folder if it doesn't exist
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        // Set the resolution
        Screen.SetResolution(width, height, false);

        // Capture the screenshot
        string filePath = System.IO.Path.Combine(folderPath, $"{screenshotName}_{width}x{height}.png");
        ScreenCapture.CaptureScreenshot(filePath);
        
        Debug.Log($"Screenshot taken and saved to: {filePath}");
    }
}