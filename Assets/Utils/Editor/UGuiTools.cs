using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public static class UGuiTools
    {
        [MenuItem("Tools/RT_UGUI/Anchors to Corners &[")]
        private static void AnchorsToCorners()
        {
            foreach (var transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "AnchorsToCorners");
                var rectTransform = transform as RectTransform;
                
                if (rectTransform == null) 
                    continue;

                var parentTransform = rectTransform.parent as RectTransform;
                
                if (parentTransform == null) 
                    continue;
                
                var parentRect = parentTransform.rect;

                var newAnchorsMin = new Vector2(
                    rectTransform.anchorMin.x + rectTransform.offsetMin.x / parentRect.width,
                    rectTransform.anchorMin.y + rectTransform.offsetMin.y / parentRect.height);

                var newAnchorsMax = new Vector2(
                    rectTransform.anchorMax.x + rectTransform.offsetMax.x / parentRect.width,
                    rectTransform.anchorMax.y + rectTransform.offsetMax.y / parentRect.height);

                rectTransform.anchorMin = newAnchorsMin;
                rectTransform.anchorMax = newAnchorsMax;

                rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;

                EditorUtility.SetDirty(transform);
            }
        }

        [MenuItem("Tools/RT_UGUI/Corners to Anchors &]")]
        private static void CornersToAnchors()
        {
            foreach (var transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "CornersToAnchors");
                var rectTransform = transform as RectTransform;
                if (rectTransform == null)
                    continue;

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                EditorUtility.SetDirty(transform);
            }
        }

        [MenuItem("Tools/RT_UGUI/Mirror Horizontally Around Anchors &;")]
        private static void MirrorHorizontallyAroundAnchors() =>
            MirrorHorizontally(false);

        [MenuItem("Tools/RT_UGUI/Mirror Horizontally Around Parent Center &:")]
        private static void MirrorHorizontallyAroundParent() =>
            MirrorHorizontally(true);

        private static void MirrorHorizontally(bool mirrorAnchors)
        {
            foreach (var transform in Selection.transforms)
            {
                Undo.RecordObject(transform, $"MirrorHorizontally {mirrorAnchors}");

                var rectTransform = transform as RectTransform;
                if (rectTransform == null)
                    continue;

                var parentTransform = rectTransform.parent as RectTransform;
                if (parentTransform == null)
                    continue;
                // Mirror the anchors if needed
                if (mirrorAnchors)
                {
                    var oldAnchorMin = rectTransform.anchorMin;
                    rectTransform.anchorMin = new Vector2(1 - rectTransform.anchorMax.x, rectTransform.anchorMin.y);
                    rectTransform.anchorMax = new Vector2(1 - oldAnchorMin.x, rectTransform.anchorMax.y);
                }
                // Mirror the offsets
                var oldOffsetMin = rectTransform.offsetMin;
                rectTransform.offsetMin = new Vector2(-rectTransform.offsetMax.x, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(-oldOffsetMin.x, rectTransform.offsetMax.y);

                // Mirror the local scale
                var localScale = rectTransform.localScale;
                rectTransform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);

                EditorUtility.SetDirty(transform);
            }
        }

        [MenuItem("Tools/RT_UGUI/Mirror Vertically Around Anchors &'")]
        private static void MirrorVerticallyAroundAnchors() =>
            MirrorVertically(false);

        [MenuItem("Tools/RT_UGUI/Mirror Vertically Around Parent Center &\"")]
        private static void MirrorVerticallyAroundParent() =>
            MirrorVertically(true);

        private static void MirrorVertically(bool mirrorAnchors)
        {
            foreach (var transform in Selection.transforms)
            {
                Undo.RecordObject(transform, $"MirrorVertically {mirrorAnchors}");
                var rectTransform = transform as RectTransform;
                if (rectTransform == null)
                    continue;

                var parentTransform = rectTransform.parent as RectTransform;
                if (parentTransform == null)
                    continue;

                if (mirrorAnchors)
                {
                    var oldAnchorMin = rectTransform.anchorMin;
                    rectTransform.anchorMin = new Vector2(oldAnchorMin.x, 1 - rectTransform.anchorMax.y);
                    rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, 1 - oldAnchorMin.y);
                }

                var oldOffsetMin = rectTransform.offsetMin;
                rectTransform.offsetMin = new Vector2(oldOffsetMin.x, -rectTransform.offsetMax.y);
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -oldOffsetMin.y);

                var localScale = rectTransform.localScale;
                rectTransform.localScale = new Vector3(localScale.x, -localScale.y, localScale.z);

                EditorUtility.SetDirty(transform);
            }
        }
    }
}
