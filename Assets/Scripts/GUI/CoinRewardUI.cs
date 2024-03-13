using TMPro;
using UnityEngine;

namespace GUI
{
    public class CoinRewardUI : RewardItemUI
    {
        [SerializeField] private TMP_Text valueTxt;

        public override void Setup(int value)
        {
            base.Setup(value);

            valueTxt.text = "" + itemValue;
        }
    }
}