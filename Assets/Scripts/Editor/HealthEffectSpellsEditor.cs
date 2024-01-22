using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(HealthEffectSpells))]
    public class HealthEffectSpellsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var healthEfSpells = (HealthEffectSpells)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpellsTags"), true);
            serializedObject.ApplyModifiedProperties();
            healthEfSpells.IsDamage = EditorGUILayout.Toggle(new GUIContent("Is Damage"), healthEfSpells.IsDamage);
            healthEfSpells.Quantity = EditorGUILayout.FloatField(new GUIContent("Quantity"), healthEfSpells.Quantity);
            healthEfSpells.IsScale = EditorGUILayout.Toggle(new GUIContent("Is Scale"), healthEfSpells.IsScale);
            
            if (healthEfSpells.IsScale)
            {
                healthEfSpells.IsScaleWithMaxHealth =
                    EditorGUILayout.Toggle(new GUIContent("Is Scale With Max Health"),
                        healthEfSpells.IsScaleWithMaxHealth);
            }
            
            if (healthEfSpells.IsDamage)
            {
                healthEfSpells.IsPassDefense = EditorGUILayout.Toggle(new GUIContent("Is Pass Defense"), healthEfSpells.IsPassDefense);
            }
        }
    }
}