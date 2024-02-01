using Models;
using static Extensions.GUIExtension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = ScriptableObjects.CharacterInfo;

namespace GUI
{
    public class CharInfoUI : MonoBehaviour
    {
        public TMP_Text characterName;
        
        public TMP_Text hp;
        public TMP_Text atk;
        public TMP_Text def;
        public TMP_Text crit;
        public TMP_Text spd;
        public TMP_Text step;
        public TMP_Text agi;
        public TMP_Text aim;
        
        public int groupImmutable;

        public Image healthBar;

        public void Setup(CharacterInfo charInfo)
        {
            characterName.text = charInfo.Name;

            foreach (var cTag in charInfo.CharacterTags)
            {
                if (cTag.ToString().Contains("Group"))
                {
                    var strNum = cTag.ToString().Substring(5);
                    groupImmutable = int.TryParse(strNum, out groupImmutable) ? groupImmutable : 1;

                    break;
                }
            }

            if (groupImmutable == 0) groupImmutable = 1;
            
            hp.text = GetSpriteStatusIcon(StatsType.Health) + $"{charInfo.Status.Health}/{charInfo.Status.Health}";
            atk.text = GetSpriteStatusIcon(StatsType.Attack) + charInfo.Status.Attack;
            def.text = GetSpriteStatusIcon(StatsType.Defense) + charInfo.Status.Defense;
            crit.text = GetSpriteStatusIcon(StatsType.Critical) + charInfo.Status.Critical;
            spd.text = GetSpriteStatusIcon(StatsType.Speed) + charInfo.Status.Speed;
            step.text = GetSpriteStatusIcon(StatsType.Step) + charInfo.Status.Step;
            agi.text = GetSpriteStatusIcon(StatsType.Agility) + charInfo.Status.Agility;
            aim.text = GetSpriteStatusIcon(StatsType.Aim) + charInfo.Status.Aim;

            healthBar.fillAmount = 1;
            
            GetComponent<CharUIDetectCard>().Setup();
        }
    }
}