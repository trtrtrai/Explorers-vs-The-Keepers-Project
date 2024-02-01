using System.Collections;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class EnergyTab : MonoBehaviour
    {
        [Header("The Keeper Setting up")]
        [SerializeField] private Sprite TheKeeperEnergy;
        [SerializeField] private Color TheKeeperTxtColor;
        
        [Header("The Explorer Setting up")]
        [SerializeField] private Sprite TheExplorerEnergy;
        [SerializeField] private Color TheExplorerTxtColor;
        
        [Header("Same setting up")]
        [SerializeField] private Image perEnergyFill;
        [SerializeField] private Image energyFill;
        [SerializeField] private TMP_Text energyAmountTxt;
        [SerializeField] private Image energySymbol;

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
            
            while (WorldManager.Instance.EnemyEnergy is null || !WorldManager.Instance.EnemyEnergy.IsSetup)
            {
                yield return null;
            }

            var team1Player = WorldManager.Instance.Team1Player;
            energySymbol.sprite = team1Player ? TheExplorerEnergy : TheKeeperEnergy;
            var color = team1Player ? TheExplorerTxtColor : TheKeeperTxtColor;
            playerEnergy = team1Player ? WorldManager.Instance.PlayerEnergy : WorldManager.Instance.EnemyEnergy;
            perEnergyFill.fillAmount = 0f;
            perEnergyFill.color = new Color(color.r, color.g, color.b, 200f / 255f);
            energyFill.fillAmount = 1f * (playerEnergy.MaximumEnergy - playerEnergy.Energy) / playerEnergy.MaximumEnergy;
            energyAmountTxt.text = "" + playerEnergy.Energy;
            energyAmountTxt.color = color;
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

                energyFill.fillAmount = 1f * (maxE - curE) / maxE;
                perEnergyFill.fillAmount = (curTPEIRL - curTimer) / curTPEIRL;
                energyAmountTxt.text = "" + curE;
            }
        }
    }
}