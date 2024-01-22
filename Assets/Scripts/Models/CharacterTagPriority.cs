using System;
using System.Collections.Generic;
using ScriptableObjects;

namespace Models
{
    [Serializable]
    public class CharacterTagPriority
    {
        public CharacterTag Subject;
        public List<CharacterTagPriorityInfo> ListObjects;
    }
}