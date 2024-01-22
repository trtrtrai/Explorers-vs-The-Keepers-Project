using Models;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TileData)), CanEditMultipleObjects]
    public class TileEditor : UnityEditor.Editor
    {
        private TileData _tileData;

        private void OnEnable()
        {
            _tileData = (TileData)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            /*_tileData.TileTag = (TileTag)EditorGUILayout.EnumPopup("Tile Tag:", _tileData.TileTag);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CurMaterial"), new GUIContent("Material:"));*/
            
            GUILayout.Space(20);
            if (GUILayout.Button("Update"))
            {
                _tileData.CurMaterial = Resources.Load<TileProperty>("ScriptableObjects/" + _tileData.TileTag).TileMaterial;
                _tileData.RendererUpdateMesh();
            }

            /*switch (tile.TileTag)
            {
                case TileTag.None:
                    tile.CurMaterial = Resources.Load<TileProperty>("ScriptableObjects/Origin").TileMaterial;
                    break;
                case TileTag.Forest:
                    tile.CurMaterial = MapController.Instance.TileMaterials[1];
                    break;
                case TileTag.Rock:
                    tile.CurMaterial = MapController.Instance.TileMaterials[2];
                    break;
                case TileTag.Water:
                    tile.CurMaterial = MapController.Instance.TileMaterials[3];
                    break;
                case TileTag.Snowing:
                    tile.CurMaterial = MapController.Instance.TileMaterials[4];
                    break;
                case TileTag.Magma:
                    tile.CurMaterial = MapController.Instance.TileMaterials[5];
                    break;
                default:
                    break;
            }*/
            
            EditorUtility.SetDirty(target);
            //EditorSceneManager.MarkAllScenesDirty();
        }
    }
}