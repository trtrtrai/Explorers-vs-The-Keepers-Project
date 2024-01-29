using System;
using ScriptableObjects;

namespace Models
{
    [Serializable]
    public class CharacterCardCopyLimit
    {
        public CharacterTag CharacterTag;
        public int Limit;
    }
}