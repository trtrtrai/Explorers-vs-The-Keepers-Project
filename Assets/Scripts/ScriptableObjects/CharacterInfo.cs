using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Character Info", menuName = "Customs/New Character Info")]
    public class CharacterInfo : ScriptableObject
    {
        public int TotalStats; // for editor check only
        public string Name;
        public Status Status;
        public List<CharacterTag> CharacterTags;
    }

    [Serializable]
    public class Status
    {
        public int Health;
        public int Attack;
        public int Defense;
        public int Critical;
        public int Speed;
        public int Step;
        public int Agility;
        public int Aim;
    }

    public enum CharacterTag
    {
        Explorer,
        Forest,
        Rock,
        Sea,
        Volcano,
        Regular,
        Group2,
        Group3,
        Group4,
        Defender,
        SkyForce,
        HqPrior,
        SkyAttack,
        Destroyer,
        Reinforced,
        General,
        Headquarter,
        Squad, //General
        Purified, //General
        SDefender, //General
        Reconstruct, //General
        // Some specific tag of General
    }
}