using System.Collections;
using Controllers;
using UnityEngine;

namespace GUI
{
    public class GeneralRequirementTab : MonoBehaviour
    {
        [SerializeField] private GameObject generalRequirementUIPrefab;
        [SerializeField] private CardTagTooltip tagTooltip;

        private void Start()
        {
            StartCoroutine(WaitingCardController());
        }

        private IEnumerator WaitingCardController()
        {
            while (CardController.Instance is null)
            {
                yield return null;
            }

            while (!CardController.Instance.IsSetup)
            {
                yield return null;
            }

            var listRequirement =
                CardController.Instance.GetGeneralRequirement(WorldManager.Instance.Team1Player ? 0 : 1);

            foreach (var requirement in listRequirement)
            {
                var obj = Instantiate(generalRequirementUIPrefab, transform);
                var script = obj.GetComponent<GeneralRequirementUI>();
                script.Setup(requirement);
            }
        }

        public void ActiveTooltip(string description)
        {
            tagTooltip.Setup(description);
        }
    }
}