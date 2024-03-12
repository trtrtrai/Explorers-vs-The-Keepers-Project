using System;
using ScriptableObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EnergyBoostSpells))]
    public class EnergyBoostSpellsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var energyBoostSpells = (EnergyBoostSpells)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpellsTags"), true);
            serializedObject.ApplyModifiedProperties();
            energyBoostSpells.BoostType = (EnergyBoostType)EditorGUILayout.EnumPopup(new GUIContent("Boost Type"), energyBoostSpells.BoostType);

            switch (energyBoostSpells.BoostType)
            {
                case EnergyBoostType.BoostRegenerate:
                    energyBoostSpells.AddingScale = EditorGUILayout.FloatField(new GUIContent("Adding Scale"),
                        energyBoostSpells.AddingScale);
                    energyBoostSpells.EffectTimer = EditorGUILayout.FloatField(new GUIContent("Effect Timer"),
                        energyBoostSpells.EffectTimer);
                    break;
                case EnergyBoostType.ReduceConsume:
                    energyBoostSpells.EnergyReducePerConsume = EditorGUILayout.IntField(new GUIContent("Reduce Amount"),
                        energyBoostSpells.EnergyReducePerConsume);
                    energyBoostSpells.Loop = EditorGUILayout.IntField(new GUIContent("Loop"),
                        energyBoostSpells.Loop);
                    break;
            }
            
            EditorUtility.SetDirty(target);
        }
    }
}