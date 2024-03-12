using ScriptableObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TeleportSpells))]
    public class TeleportSpellsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var teleSpellsEff = (TeleportSpells)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpellsTags"), true);
            serializedObject.ApplyModifiedProperties();
            teleSpellsEff.TeleportType = (TeleportType)EditorGUILayout.EnumPopup(new GUIContent("Teleport Type"), teleSpellsEff.TeleportType);

            if (teleSpellsEff.TeleportType != TeleportType.BackToHeadquarter)
            {
                teleSpellsEff.Step = EditorGUILayout.IntField(new GUIContent("Step"), teleSpellsEff.Step);
            }
            
            EditorUtility.SetDirty(target);
        }
    }
}