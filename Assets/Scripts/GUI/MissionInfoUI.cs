using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class MissionInfoUI : MonoBehaviour
    {
        [SerializeField] private int missionLevel;
        [SerializeField] private MissionStatus status;
        [SerializeField] private bool firstPlay;

        [SerializeField] private Button missionBtn;
        [SerializeField] private TMP_Text comingSoonLabel;

        public void Setup(MissionData data)
        {
            missionLevel = data.missionLevel;
            status = data.status;
            firstPlay = data.firstPlay;

            switch (status)
            {
                case MissionStatus.Playable:
                    missionBtn.interactable = true;
                    comingSoonLabel.gameObject.SetActive(false);
                    break;
                case MissionStatus.Locked:
                    missionBtn.interactable = false;
                    comingSoonLabel.text = "locked";
                    comingSoonLabel.gameObject.SetActive(true);
                    break;
                case MissionStatus.ComingSoon:
                    missionBtn.interactable = false;
                    missionBtn.image.color = Color.black;
                    comingSoonLabel.text = "coming soon!!!";
                    comingSoonLabel.gameObject.SetActive(true);
                    break;
            }
        }
    }
}