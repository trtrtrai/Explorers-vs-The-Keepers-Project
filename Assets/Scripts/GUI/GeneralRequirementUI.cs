using EventArgs;
using Models.Generals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class GeneralRequirementUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image fillProgress;
        [SerializeField] private TMP_Text progressTxt;
        [SerializeField] private string description;
        [SerializeField] private GeneralCheckCanSummon self;

        public void Setup(GeneralCheckCanSummon requirement)
        {
            self = requirement;
            
            label.text = self.GetType().Name + " Requirement";
            fillProgress.fillAmount = 0f;
            progressTxt.text = "0%";
            description = self.GetDescription();

            self.OnRequireTrigger += OnRequireTrigger;
        }

        private void OnRequireTrigger(object sender, GeneralRequireTriggerEventArgs args)
        {
            if (ReferenceEquals(sender, self))
            {
                var curProgress = 1f - args.NewProgress;
                fillProgress.fillAmount = curProgress;
                progressTxt.text = Mathf.RoundToInt(curProgress * 100f) + "%";
            }
        }
        
        public void ActiveTooltip()
        {
            GetComponentInParent<GeneralRequirementTab>().ActiveTooltip(description);
        }

        private void OnDestroy()
        {
            self.OnRequireTrigger -= OnRequireTrigger;
        }
    }
}