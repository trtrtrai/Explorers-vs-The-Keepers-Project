using System;
using Controllers;
using UnityEngine;
using Random = System.Random;

namespace GUI
{
    public class RandomCardMainMenu : MonoBehaviour
    {
        private void Start()
        {
            var children = GetComponentsInChildren<CardRewardUI>();
            var length = Enum.GetNames(typeof(CardName)).Length;
            foreach (var cardRewardUI in children)
            {
                cardRewardUI.Setup(new Random().Next(length));
            }
        }
    }
}