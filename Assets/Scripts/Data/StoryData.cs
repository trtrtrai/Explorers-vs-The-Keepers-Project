using System;
using System.Collections.Generic;

namespace Data
{
    public class StoryData : GameData
    {
        public List<CutScene> CutScenes;
    }

    [Serializable]
    public struct CutScene
    {
        public List<Speech> speeches;
    }

    [Serializable]
    public struct Speech
    {
        public CharacterSpeech talker;
        public string talkerSpriteName;
        public List<string> speechTexts;
    }

    public enum CharacterSpeech
    {
        None,
        Supporter,
        Headquarter,
        Commander,
        SuperMiner,
    }
}