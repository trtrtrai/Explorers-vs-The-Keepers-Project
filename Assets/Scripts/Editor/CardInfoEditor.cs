using System.Linq;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using CharacterInfo = ScriptableObjects.CharacterInfo;

namespace Editor
{
    [CustomEditor(typeof(CardInfo))]
    public class CardInfoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var cardInfo = (CardInfo)target;
            
            cardInfo.Name = EditorGUILayout.TextField(new GUIContent("Skill Name"), cardInfo.Name);
            
            cardInfo.Cost = EditorGUILayout.IntField(new GUIContent("Cost"), cardInfo.Cost);
            
            cardInfo.CardIcon = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Card Icon"), cardInfo.CardIcon , typeof(Sprite), false);
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Description", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                cardInfo.Description = EditorGUILayout.TextArea(cardInfo.Description, EditorStyles.textArea, GUILayout.Height(100));
            }
            EditorGUILayout.EndHorizontal();
            
            cardInfo.CardType = (CardType)EditorGUILayout.EnumPopup(new GUIContent("Card Type"), cardInfo.CardType);
            
            cardInfo.ActiveType = (CardActiveType)EditorGUILayout.EnumPopup(new GUIContent("Active Type"), cardInfo.ActiveType);

            switch (cardInfo.CardType)
            {
                case CardType.Spells:
                {
                    if (cardInfo.ActiveType == CardActiveType.Area || cardInfo.ActiveType == CardActiveType.AreaEnemy)
                    {
                        RangeAttribute range = (RangeAttribute)serializedObject.FindProperty(nameof(CardInfo.Radius)).GetUnderlyingField().GetCustomAttributes(false).FirstOrDefault();
                        if (range is null) break;
                        cardInfo.Radius = EditorGUILayout.IntSlider(new GUIContent("Radius"), cardInfo.Radius, (int)range.min, (int)range.max);
                    }

                    break;
                }
            }

            if (cardInfo.CardType == CardType.Spells)
            {
                cardInfo.SpellsEffect =
                (SpellsEffect)EditorGUILayout.ObjectField(new GUIContent("Spells Effect"), cardInfo.SpellsEffect, typeof(SpellsEffect), false);
            }
            else
            {
                cardInfo.Character = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Character"),
                    cardInfo.Character, typeof(GameObject), false);
            }

            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}