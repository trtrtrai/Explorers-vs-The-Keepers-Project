using System;
using ScriptableObjects;
using UnityEngine;

namespace GUI
{
    [Serializable]
    public class TagExplain
    {
        public bool IsCharacterTag;
        
        public CharacterTag CharacterTag;
        public SpellsTag SpellsTag;

        [TextArea(5, 7)]
        public string Explain;

        public Color BackgroundColor;
    }
}