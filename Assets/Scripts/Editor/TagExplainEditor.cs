using GUI;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TagExplain))]
    public class TagExplainEditor : PropertyDrawer
    {
        private int line;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * line;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var isCharacterTag = property.FindPropertyRelative("IsCharacterTag");
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.size.x, EditorGUI.GetPropertyHeight(isCharacterTag)), isCharacterTag);
            line = 1;

            var isCharacterTagValue = isCharacterTag.boolValue;
            if (isCharacterTagValue)
            {
                var characterTag = property.FindPropertyRelative("CharacterTag");
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(characterTag)), characterTag, new GUIContent("Tag"));
                line++;
            }
            else
            {
                var spellsTag = property.FindPropertyRelative("SpellsTag");
                EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(spellsTag)), spellsTag, new GUIContent("Tag"));
                line++;
            }

            var lastLine = line++;
            line += 5;
            var explain = property.FindPropertyRelative("Explain");
            // not render?????????
            EditorGUI.LabelField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * (lastLine), position.size.x, EditorGUI.GetPropertyHeight(explain)), new GUIContent("Explain"));
            explain.stringValue = EditorGUI.TextArea(
                new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * (lastLine + 1), position.size.x, EditorGUI.GetPropertyHeight(explain)), explain.stringValue, EditorStyles.textArea);
            line++;
            
            var bgColor = property.FindPropertyRelative("BackgroundColor");
            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * line, position.size.x, EditorGUI.GetPropertyHeight(bgColor)), bgColor, new GUIContent("Background Color"));
            line++;
            
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}