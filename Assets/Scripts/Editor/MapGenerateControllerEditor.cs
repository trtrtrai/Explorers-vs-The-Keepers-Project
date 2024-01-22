using Controllers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MapGenerateController))]
    public class MapGenerateControllerEditor : UnityEditor.Editor
    {
        private MapGenerateController _controller;

        private void OnEnable()
        {
            _controller = (MapGenerateController)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Update Tile Style"))
            {
                _controller.UpdateAllTileData();
            }
            GUILayout.Space(20);
            
            DrawDefaultInspector();
            
            GUILayout.Space(20);
            if (GUILayout.Button("Generate"))
            {
                _controller.Generate();
            }
            
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}