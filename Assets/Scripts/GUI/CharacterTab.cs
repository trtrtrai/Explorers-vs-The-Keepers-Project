using System.Collections.Generic;
using System.Linq;
using Controllers;
using EventArgs;
using JetBrains.Annotations;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Extensions.GUIExtension;

namespace GUI
{
    public class CharacterTab : MonoBehaviour
    {
        [SerializeField] private CharacterTarget characterTarget;
        
        [SerializeField] private Transform allyContent;
        [SerializeField] private Transform enemyContent;
        private Dictionary<Character, CharInfoUI> _characters;
        
        [SerializeField] private GameObject charInfoPrefab;

        [SerializeField] private Color normalColor;
        [SerializeField] private Color boostStatsColor;
        [SerializeField] private Color badEffectStatsColor;

        [SerializeField] private bool tabShow;
        [SerializeField] private Transform showHideBtn;
        [SerializeField] private Transform showHideArrowImg;

        [SerializeField] private Image characterInfoTargeted;
        
        private void Start()
        {
            WorldManager.Instance.CharacterSpawn += CharacterSpawnDetect;
            _characters = new();
            tabShow = transform.GetComponent<RectTransform>().anchoredPosition.x == 0;
            
            if (tabShow) ToggleTab(); // always hide the tab at begin
        }

        private void CharacterSpawnDetect(object sender, CharacterSpawnEventArgs args)
        {
            var character = args.Character;
            var charInfoUI = Instantiate(charInfoPrefab, args.Team == WorldManager.Instance.GetAllyTeam() ? allyContent.transform : enemyContent.transform);

            var charInfoUIScript = charInfoUI.GetComponent<CharInfoUI>();
            _characters.Add(character, charInfoUIScript);
            charInfoUIScript.Setup(character.CharacterInfo);

            character.OnCharacterStatsChange += CharacterStatsChangeUI;
            character.OnCharacterDeath += CharacterDeathDetect;
        }

        private void CharacterDeathDetect(object sender, CharacterDeathEventArgs args)
        {
            var character = sender as Character;
            if (character is null || !_characters.Keys.ToList().Contains(character)) return;

            character.OnCharacterDeath -= CharacterDeathDetect;
            character.OnCharacterStatsChange -= CharacterStatsChangeUI;
            
            var charUI = _characters[character];
            
            if (ReferenceEquals(charUI.GetComponent<Image>(), characterInfoTargeted))
            {
                DeselectCharacter();
            }
            
            charUI.gameObject.SetActive(false);
            _characters.Remove(character);
            //Debug.Log(charUI.characterName.text + " death! Remove UI card.");
            Destroy(charUI.gameObject);
        }

        private void CharacterStatsChangeUI(object sender, CharacterStatsChangeEventArgs args)
        {
            var character = sender as Character;
            if (character is null || !_characters.Keys.ToList().Contains(character)) return;
            
            //Debug.Log(character.name + " " + args.StatsType + " changed " + " from " + args.OldValue + " to " + args.NewValue);
            
            var charUI = _characters[character];
            var statsStr = "" + args.NewValue;
            var spriteTxt = GetSpriteStatusIcon(args.StatsType);
            switch (args.StatsType)
            {
                case StatsType.Health:
                    charUI.healthBar.fillAmount = 1f * args.NewValue / args.Immutable;
                    charUI.hp.text = spriteTxt + $"{args.NewValue}/{args.Immutable}";
                    break;
                case StatsType.Attack:
                    charUI.atk.text = spriteTxt + args.NewValue * args.GroupNumber;
                    TextColorChange(charUI.atk, args.NewValue, args.Immutable);
                    break;
                case StatsType.Defense:
                    charUI.def.text = spriteTxt + args.NewValue * args.GroupNumber;
                    TextColorChange(charUI.def, args.NewValue, args.Immutable);
                    break;
                case StatsType.Critical:
                    charUI.crit.text = spriteTxt + statsStr;
                    TextColorChange(charUI.crit, args.NewValue, args.Immutable);
                    break;
                case StatsType.Speed:
                    charUI.spd.text = spriteTxt + statsStr;
                    TextColorChange(charUI.spd, args.NewValue, args.Immutable);
                    break;
                case StatsType.Step:
                    /*charUI.step.text = statsStr;
                    TextColorChange(charUI.atk, args.NewValue, charUI.Immutable.Attack);*/
                    break;
                case StatsType.Agility:
                    charUI.agi.text = spriteTxt + statsStr;
                    TextColorChange(charUI.agi, args.NewValue, args.Immutable);
                    break;
                case StatsType.Aim:
                    /*charUI.aim.text = statsStr;
                    TextColorChange(charUI.atk, args.NewValue, charUI.Immutable.Attack);*/
                    break;
                case StatsType.Shield:
                    // ???
                    break;
            }
        }

        private void TextColorChange(TMP_Text text, int newValue, int immutable)
        {
            if (newValue > immutable)
            {
                text.color = boostStatsColor;
            }
            else if (newValue < immutable)
            {
                text.color = badEffectStatsColor;
            }
            else
            {
                text.color = normalColor;
            }
        }
        
        [CanBeNull]
        public Character GetCharacterFrom(CharInfoUI charInfoUI) => _characters.FirstOrDefault(c => c.Value.Equals(charInfoUI)).Key;
        
        public void ToggleTab()
        {
            var rect = transform.GetComponent<RectTransform>();
            var rectPos = rect.anchoredPosition;
            var btnRect = showHideBtn.GetComponent<RectTransform>();
            var btnRectPos = btnRect.anchoredPosition;
            var arrowRotation = showHideArrowImg.rotation;
            //Debug.Log(rectPos);
            
            rectPos.x = tabShow ? rect.sizeDelta.x : 0;
            btnRectPos.x = tabShow ? -btnRect.sizeDelta.x : 0;
            
            rect.anchoredPosition = rectPos;
            btnRect.anchoredPosition = btnRectPos;
            arrowRotation.z *= -1f;
            showHideArrowImg.rotation = arrowRotation;
            tabShow = !tabShow;
        }
        
        public void BringToFront(GameObject tab)
        {
            tab.transform.SetAsFirstSibling();
            
            if (!tabShow) ToggleTab();
        }

        public void SelectCharacter(Image current, GameObject target)
        {
            if (characterInfoTargeted is not null)
            {
                characterInfoTargeted.enabled = false;
            }

            characterInfoTargeted = current;
            characterInfoTargeted.enabled = true;
            characterTarget.SetupTarget(target);
        }

        public void DeselectCharacter()
        {
            characterInfoTargeted.enabled = false;
            characterInfoTargeted = null;
            
            characterTarget.DeselectTarget();
        }
    }
}