# ScreenshotsTool

This Unity Editor tool allows you to take screenshots of your game view with customizable settings. 
It supports capturing screenshots at specified resolutions, adding prefixes to filenames, and taking multiple screenshots with delays.

## Features

- Set custom resolution for screenshots
- Specify custom filename and folder path
- Option to add prefix numbers to filenames
- Toggle between landscape and portrait orientation (for Android)
- Batch screenshot capturing with delays
- Add and manage custom resolutions

## Installation

1. Copy the `ScreenshotsTool` script into your Unity project (preferably in an `Editor` folder).
2. Ensure you have the **Unity Editor Coroutines** package installed. You can install it via the Package Manager:
   - Open the Package Manager (`Window > Package Manager`).
   - Click the "+" button and select "Add package from git URL...".
   - Enter `com.unity.editorcoroutines` and click "Add".

3. Open Unity and go to the `Tools` menu. Select `ScreenshotsTool` to open the tool window.

## Usage

1. Open the tool via `Tools > ScreenshotsTool` in the Unity Editor menu.
2. Configure the desired settings in the tool window:
   - **Use resolution**: Toggle this to specify width and height manually.
   - **Screenshot Name**: Set the base name for the screenshots.
   - **Folder Path**: Specify the folder where screenshots will be saved.
   - **USE PREFIX NUMBER?**: Toggle this to add a numerical prefix to the filenames.
   - **IsLandScape (Android)**: Toggle this to switch between landscape and portrait modes.
3. Click **Take Screenshot** to capture a screenshot with the current settings.
4. Alternatively, switch to using a list of custom resolution names:
   - Add new resolution names to the list.
   - Click **TAKE SCREENSHOTS FOR ALL RESOLUTION IN THIS LIST** to capture screenshots for each resolution with a specified delay between captures.

## Example

### Taking a Single Screenshot

1. Set `Width` to `1920` and `Height` to `1080`.
2. Set `Screenshot Name` to `screenshot`.
3. Set `Folder Path` to `Screenshots`.
4. Toggle `USE PREFIX NUMBER?` if you want to prefix filenames with numbers.
5. Click **Take Screenshot**.

### Taking Multiple Screenshots with Different Resolutions

1. Uncheck `Use resolution`.
2. Add custom resolution names (e.g., "1920x1080", "1280x720") to the list.
3. Set the delay between screenshots using the `Delay` field.
4. Click **TAKE SCREENSHOTS FOR ALL RESOLUTION IN THIS LIST**.

## Script Overview

### Fields

- `_width` and `_height`: Resolution dimensions for the screenshot.
- `_screenshotName`: Base name for the screenshot file.
- `_folderPath`: Directory where screenshots are saved.
- `_useNumber`: Flag to add a numerical prefix to the filename.
- `_numberOfScreenShot`: Current number for the prefix.
- `_isLandScape`: Flag for landscape orientation (Android).
- `_delaySeconds`: Delay between taking screenshots in batch mode.
- `_listOfNames`: List of custom resolution names.
- `_newName`: New resolution name to be added to the list.

### Methods

- `OnEnable`: Initializes necessary reflection types and methods for the game view.
- `ShowWindow`: Opens the tool window in the Unity Editor.
- `OnGUI`: Renders the tool's UI and handles user input.
- `TakeScreenshot`: Captures a screenshot with the specified settings.
- `TakeScreenshotsWithDelay`: Coroutine to capture multiple screenshots with a delay.
- `SetGameViewSize`: Sets the game view size based on width and height.
- `SwitchToResolution`: Switches the game view to a specified resolution.
- `TrySetSize`: Tries to set the game view size to a specified resolution.
- `SetSizeIndex`: Sets the game view size by index.
- `FindSize`: Finds the index of a specified resolution in the game view settings.
- `GetGroup`: Gets the game view size group for the current platform.
- `GetCurrentGroupType`: Gets the current game view size group type based on the platform.

## Notes

- Ensure the specified folder path exists or will be created by the script.
- The script uses reflection to access Unity Editor internals, which may break with future Unity updates.
- The tool supports Android, iOS, and Standalone platforms by default.

## License

This tool is provided as-is with no warranties. You are free to modify and use it in your projects.