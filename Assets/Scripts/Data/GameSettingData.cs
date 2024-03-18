using System;

namespace Data
{
    public class GameSettingData : GameData
    {
        public int PlanetAmount;
        public bool IsFirstPlay;
        public bool BgmNSfx;
        public float MusicVolume;
        public float SoundVolume;
    }

    [Serializable]
    public class StoryTrigger
    {
        public bool triggerActive;
        public int storyIndex;
    }
}