using System;

namespace Data
{
    [Serializable]
    public class Reward
    {
        public RewardItem rewardItem;
        public int value;
    }

    public enum RewardItem
    {
        Card,
        Coin,
        Cash
    }
}