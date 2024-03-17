using System;
using System.Collections.Generic;

namespace Data
{
    public enum MissionStatus
    {
        Playable, // can play
        Locked, // cannot play
        ComingSoon, // does not created yet
    }

    [Serializable]
    public class MissionData
    {
        public int missionLevel;
        public MissionStatus status;
        public bool firstPlay; // for story process
        public List<Reward> rewards;
    }
}