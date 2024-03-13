using Controllers;
using Data;
using UnityEngine;

namespace GUI
{
    public class MissionReward : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private GameObject cashPrefab;
        
        public void ShowReward(bool result)
        {
            if (!result) return;

            var missionData = WorldManager.Instance.MissionData;
            var rewards = DataManager.GetPlanetData()[missionData.PlanetMap].Missions[missionData.MissionIndex].rewards;

            foreach (var reward in rewards)
            {
                switch (reward.rewardItem)
                {
                    case RewardItem.Card:
                        var card = Instantiate(cardPrefab, content);
                        card.GetComponent<RewardItemUI>().Setup(reward.value);
                        break;
                    case RewardItem.Coin:
                        var coin = Instantiate(coinPrefab, content);
                        coin.GetComponent<RewardItemUI>().Setup(reward.value);
                        break;
                    case RewardItem.Cash:
                        var cash = Instantiate(cashPrefab, content);
                        cash.GetComponent<RewardItemUI>().Setup(reward.value);
                        break;
                }
            }
        }
    }
}