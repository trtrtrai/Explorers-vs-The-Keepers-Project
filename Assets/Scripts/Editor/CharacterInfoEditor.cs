using ScriptableObjects;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    [CustomEditor(typeof(CharacterInfo))]
    public class CharacterInfoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var charInfo = (CharacterInfo)target;
            var status = charInfo.Status;

            var totalStats = status.Health + status.Attack + status.Defense + status.Critical + status.Speed +
                             status.Agility;

            charInfo.TotalStats = totalStats;

            DrawDefaultInspector();
            
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}