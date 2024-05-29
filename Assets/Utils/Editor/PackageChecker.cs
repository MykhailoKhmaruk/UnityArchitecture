using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;

[InitializeOnLoad]
public class PackageChecker
{
    private const string packageName = "com.unity.editorcoroutines";
    private static ListRequest listRequest;
    private static AddRequest addRequest;

    static PackageChecker()
    {
        // Run the check when the editor loads
        EditorApplication.update += CheckForPackage;
    }

    private static void CheckForPackage()
    {
        listRequest = Client.List(); // List installed packages
        EditorApplication.update += ListRequestProgress;
    }

    private static void ListRequestProgress()
    {
        if (listRequest.IsCompleted)
        {
            if (listRequest.Status == StatusCode.Success)
            {
                bool isInstalled = false;
                foreach (var package in listRequest.Result)
                {
                    if (package.name == packageName)
                    {
                        isInstalled = true;
                        break;
                    }
                }

                if (!isInstalled)
                {
                    Debug.Log($"{packageName} is not installed. Installing now...");
                    addRequest = Client.Add(packageName); // Install package
                    EditorApplication.update += AddRequestProgress;
                }
                else
                {
                    Debug.Log($"{packageName} is already installed.");
                }
            }
            else if (listRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError(listRequest.Error.message);
            }

            EditorApplication.update -= ListRequestProgress;
        }
    }

    private static void AddRequestProgress()
    {
        if (addRequest.IsCompleted)
        {
            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log($"Successfully installed {packageName}.");
            }
            else if (addRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError($"Failed to install {packageName}: {addRequest.Error.message}");
            }

            EditorApplication.update -= AddRequestProgress;
        }
    }
}
