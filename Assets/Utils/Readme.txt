UGUI Anchors Setters
UGUI Anchors Setters is a Unity Editor utility designed to simplify the manipulation of RectTransform anchors and offsets in Unity's UI system. This tool provides several useful commands for adjusting anchors and offsets, as well as mirroring UI elements horizontally and vertically.

Features
Anchors to Corners: Adjusts the anchors of the selected RectTransforms to match their corners.
Corners to Anchors: Adjusts the corners of the selected RectTransforms to match their anchors.
Mirror Horizontally Around Anchors: Mirrors the selected RectTransforms horizontally around their anchors.
Mirror Horizontally Around Parent Center: Mirrors the selected RectTransforms horizontally around the center of their parent.
Mirror Vertically Around Anchors: Mirrors the selected RectTransforms vertically around their anchors.
Mirror Vertically Around Parent Center: Mirrors the selected RectTransforms vertically around the center of their parent.
Installation
Download the UGuiTools.cs script and place it in your project's Editor folder.
Usage
The utility adds new menu items under the Tools/RT_UGUI menu in the Unity Editor:

Anchors to Corners: &[
Corners to Anchors: &]
Mirror Horizontally Around Anchors: &;
Mirror Horizontally Around Parent Center: &.
Mirror Vertically Around Anchors: &'
Mirror Vertically Around Parent Center: &/
To use these tools, select one or more UI elements in the hierarchy and choose the desired command from the Tools/RT_UGUI menu.

Detailed Functionality
Anchors to Corners
Adjusts the anchors of the selected RectTransforms to match their current corner positions.

C#
[MenuItem("Tools/RT_UGUI/Anchors to Corners &[")]
private static void AnchorsToCorners()
{
    // Implementation
}
Corners to Anchors
Resets the offsets of the selected RectTransforms to zero, effectively making the corners match the anchors.

C#
[MenuItem("Tools/RT_UGUI/Corners to Anchors &]")]
private static void CornersToAnchors()
{
    // Implementation
}
Mirror Horizontally Around Anchors
Mirrors the selected RectTransforms horizontally around their anchors.

C#
[MenuItem("Tools/RT_UGUI/Mirror Horizontally Around Anchors &;")]
private static void MirrorHorizontallyAroundAnchors()
{
    MirrorHorizontally(false);
}
Mirror Horizontally Around Parent Center
Mirrors the selected RectTransforms horizontally around the center of their parent RectTransform.

C#
[MenuItem("Tools/RT_UGUI/Mirror Horizontally Around Parent Center &.")]
private static void MirrorHorizontallyAroundParent()
{
    MirrorHorizontally(true);
}
Mirror Vertically Around Anchors
Mirrors the selected RectTransforms vertically around their anchors.

C#
[MenuItem("Tools/RT_UGUI/Mirror Vertically Around Anchors &'")]
private static void MirrorVerticallyAroundAnchors()
{
    MirrorVertically(false);
}
Mirror Vertically Around Parent Center
Mirrors the selected RectTransforms vertically around the center of their parent RectTransform.

C#
[MenuItem("Tools/RT_UGUI/Mirror Vertically Around Parent Center &/")]
private static void MirrorVerticallyAroundParent()
{
    MirrorVertically(true);
}
License
This utility is licensed under the Unity Companion License. For more information, see the Unity Companion License.

Feel free to modify the content as needed to better suit your project's requirements.