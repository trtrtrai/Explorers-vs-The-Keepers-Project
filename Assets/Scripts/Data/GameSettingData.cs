using System;

namespace Data
{
    public class GameSettingData : GameData
    {
        public int PlanetAmount;
        public bool IsFirstPlay;
    }

    [Serializable]
    public class StoryTrigger
    {
        public bool triggerActive;
        public int storyIndex;
    }
}