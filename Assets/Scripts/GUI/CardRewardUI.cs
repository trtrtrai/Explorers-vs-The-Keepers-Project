using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Data;

namespace GUI
{
    public class CardRewardUI : RewardItemUI
    {
        [SerializeField] private TMP_Text cardTypeTxt;
        [SerializeField] private TMP_Text cardNameTxt;
        [SerializeField] private Image cardIconImg;

        public override void Setup(int value)
        {
            base.Setup(value);

            var cardInfo = DataManager.GetCardInfo(Enum.GetNames(typeof(CardName))[itemValue]);

            cardNameTxt.text = cardInfo.Name;
            cardTypeTxt.text = cardInfo.CardType.ToString();
            cardIconImg.sprite = cardInfo.CardIcon;
        }
    }
}