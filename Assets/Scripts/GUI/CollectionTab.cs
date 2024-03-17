using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class CollectionTab : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private GameObject cashPrefab;

        public void ShowReward(List<Reward> rewards)
        {
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

        public void OnPointerClick(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}