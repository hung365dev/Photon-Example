#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapInspector : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Map t = (Map)target;

        if (GUILayout.Button("Load Scene"))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(t.Scene));
        }
    }
}
#endif
