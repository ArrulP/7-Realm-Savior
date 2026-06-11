using UnityEngine;
using UnityEditor;

public class CopyHierarchyPath
{
    [MenuItem("GameObject/Copy Relative Path", false, 0)]
    private static void CopyPath()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null) return;

        string path = selected.name;
        Transform current = selected.transform;

        // Loop upwards until we hit the top root parent
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }

        EditorGUIUtility.systemCopyBuffer = path;
        Debug.Log($"Full Hierarchy Path Copied: {path}");
    }
}
