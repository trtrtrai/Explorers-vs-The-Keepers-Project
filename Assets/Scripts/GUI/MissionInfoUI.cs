using Data;
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
        [SerializeField] private Transform comingSoonLabel;

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
                    comingSoonLabel.gameObject.SetActive(false);
                    break;
                case MissionStatus.ComingSoon:
                    missionBtn.interactable = false;
                    comingSoonLabel.gameObject.SetActive(true);
                    break;
            }
        }
    }
}