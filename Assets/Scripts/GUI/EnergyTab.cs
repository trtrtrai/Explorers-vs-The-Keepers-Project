using System;
using System.Collections;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class EnergyTab : MonoBehaviour
    {
        [SerializeField] private Image energyFill;
        [SerializeField] private TMP_Text energyAmountTxt;

        [SerializeField] private bool calcUI;
        [SerializeField] private EnergyManager playerEnergy;

        private void Start()
        {
            StartCoroutine(WaitToSetup());
        }

        private IEnumerator WaitToSetup()
        {
            while (WorldManager.Instance is null)
            {
                yield return null;
            }
            
            while (WorldManager.Instance.PlayerEnergy is null || !WorldManager.Instance.PlayerEnergy.IsSetup)
            {
                yield return null;
            }

            playerEnergy = WorldManager.Instance.PlayerEnergy;
            energyFill.fillAmount = 1f * playerEnergy.Energy / playerEnergy.MaximumEnergy;
            energyAmountTxt.text = playerEnergy.Energy + "";
            calcUI = true;
        }

        private void Update()
        {
            // change to event listener after but curTimer and curTPEIRL must be update!
            if (calcUI)
            {
                var curE = playerEnergy.Energy;
                var maxE = playerEnergy.MaximumEnergy;
                var curTPEIRL = playerEnergy.TimePerEnergyInRealTime;
                var curTimer = playerEnergy.Timer;

                energyFill.fillAmount = 1f * curE / maxE + 0.1f * (curTPEIRL - curTimer) / curTPEIRL;
                energyAmountTxt.text = curE + "";
            }
        }
    }
}