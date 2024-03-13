using System;
using System.Collections;
using Controllers;
using GUI.SelectCardDeck;
using static Extensions.GUIExtension;
using Models;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class CardInfoUI : MonoBehaviour
    {
        [SerializeField] private Card self;

        [Header("Card Forward")]
        [SerializeField] private TMP_Text cardType;
        [SerializeField] private Image activeTypeIcon;
        [SerializeField] private TMP_Text activeType;
        [SerializeField] private TMP_Text cardName;
        [SerializeField] private TMP_Text cost;
        [SerializeField] private Image cardIcon;
        [SerializeField] private TMP_Text description;

        [Header("Card Backward")] 
        [SerializeField] private Transform verticalTagContent;

        [SerializeField] private GameObject characterInfoLabel;
        [SerializeField] private GameObject characterInfoContent;
        [SerializeField] private TMP_Text hp;
        [SerializeField] private TMP_Text atk;
        [SerializeField] private TMP_Text def;
        [SerializeField] private TMP_Text crit;
        [SerializeField] private TMP_Text spd;
        [SerializeField] private TMP_Text step; //need icon
        [SerializeField] private TMP_Text agi;
        [SerializeField] private TMP_Text aim; //need icon
        
        private void Start()
        {
            StartCoroutine(WaitToCardSetup());
        }

        private IEnumerator WaitToCardSetup()
        {
            if (TryGetComponent(out self))
            {
                while (!self.IsSetup)
                {
                    yield return null;
                }

                var parent = transform.parent.GetComponent<Hand>();
                if (parent is null)
                {
                    SetupSelectMode();
                }
                else
                {
                    SetupPlayMode();
                }
            }
            else
            {
                Destroy(this);
            }
        }

        private void SetupPlayMode()
        {
            cardName.text = self.Name;
                cost.text = self.Cost + "";
                cardIcon.sprite = self.CardIcon;
                description.text = self.Description;
                cardType.text = self.CardType.ToString();
                activeTypeIcon.sprite = GetActiveTypeIcon(self.ActiveType);
                activeType.text = SpaceBetweenWord(self.ActiveType.ToString().Replace("Enemy", ""));

                var hand = transform.parent.GetComponent<Hand>();
                var cardTagContent = hand.cardTagContentPrefab;
                var cardTag = hand.cardTagPrefab;

                if (self.CardType == CardType.Spells)
                {
                    characterInfoLabel.SetActive(false);
                    characterInfoContent.SetActive(false);
                    
                    var spellsTag = self.SpellsEffect.SpellsTags;

                    var tagContent = Instantiate(cardTagContent, verticalTagContent);
                    int loop = 0;
                    foreach (var tags in spellsTag)
                    {
                        if (loop < 2)
                        {
                            var tagExplain = hand.GetTagExplain(false, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            
                            loop++;
                        }
                        else
                        {
                            tagContent = Instantiate(cardTagContent, verticalTagContent);
                            
                            var tagExplain = hand.GetTagExplain(false, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            
                            loop = 1;
                        }
                    }
                }
                else //General + Minion
                {
                    var characterInfo = self.Character.GetComponent<Character>().CharacterInfo;
                    var charTag = characterInfo.CharacterTags;

                    var tagContent = Instantiate(cardTagContent, verticalTagContent);
                    int loop = 0;
                    foreach (var tags in charTag)
                    {
                        if (loop < 2)
                        {
                            var tagExplain = hand.GetTagExplain(true, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            loop++;
                        }
                        else
                        {
                            tagContent = Instantiate(cardTagContent, verticalTagContent);
                            
                            var tagExplain = hand.GetTagExplain(true, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            
                            loop = 1;
                        }
                    }

                    hp.text = GetSpriteStatusIcon(StatsType.Health) + characterInfo.Status.Health;
                    atk.text = GetSpriteStatusIcon(StatsType.Attack) + characterInfo.Status.Attack;
                    def.text = GetSpriteStatusIcon(StatsType.Defense) + characterInfo.Status.Defense;
                    crit.text = GetSpriteStatusIcon(StatsType.Critical) + characterInfo.Status.Critical;
                    spd.text = GetSpriteStatusIcon(StatsType.Speed) + characterInfo.Status.Speed;
                    step.text = GetSpriteStatusIcon(StatsType.Step) + characterInfo.Status.Step;
                    agi.text = GetSpriteStatusIcon(StatsType.Agility) + characterInfo.Status.Agility;
                    aim.text = GetSpriteStatusIcon(StatsType.Aim) + characterInfo.Status.Aim;
                }
        }

        private void SetupSelectMode()
        {
            cardName.text = self.Name;
                cost.text = self.Cost + "";
                cardIcon.sprite = self.CardIcon;
                description.text = self.Description;
                cardType.text = self.CardType.ToString();
                activeTypeIcon.sprite = GetActiveTypeIcon(self.ActiveType);
                activeType.text = SpaceBetweenWord(self.ActiveType.ToString().Replace("Enemy", ""));
                
                var cardTagContent = PlanetManager.Instance.CardInventory.cardTagContentPrefab;
                var cardTag = PlanetManager.Instance.CardInventory.cardTagPrefab;

                if (self.CardType == CardType.Spells)
                {
                    characterInfoLabel.SetActive(false);
                    characterInfoContent.SetActive(false);
                    
                    var spellsTag = self.SpellsEffect.SpellsTags;

                    var tagContent = Instantiate(cardTagContent, verticalTagContent);
                    int loop = 0;
                    foreach (var tags in spellsTag)
                    {
                        if (loop < 2)
                        {
                            var tagExplain = PlanetManager.Instance.CardInventory.GetTagExplain(false, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            
                            loop++;
                        }
                        else
                        {
                            tagContent = Instantiate(cardTagContent, verticalTagContent);
                            
                            var tagExplain = PlanetManager.Instance.CardInventory.GetTagExplain(false, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            
                            loop = 1;
                        }
                    }
                }
                else //General + Minion
                {
                    var characterInfo = self.Character.GetComponent<Character>().CharacterInfo;
                    var charTag = characterInfo.CharacterTags;

                    var tagContent = Instantiate(cardTagContent, verticalTagContent);
                    int loop = 0;
                    foreach (var tags in charTag)
                    {
                        if (loop < 2)
                        {
                            var tagExplain = PlanetManager.Instance.CardInventory.GetTagExplain(true, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            loop++;
                        }
                        else
                        {
                            tagContent = Instantiate(cardTagContent, verticalTagContent);
                            
                            var tagExplain = PlanetManager.Instance.CardInventory.GetTagExplain(true, tags.ToString());
                            if (tagExplain is null) InstantiateTag(cardTag, tagContent.transform, tags.ToString());
                            else InstantiateTag(cardTag, tagContent.transform, tags.ToString(), tagExplain.BackgroundColor);
                            
                            loop = 1;
                        }
                    }

                    hp.text = GetSpriteStatusIcon(StatsType.Health) + characterInfo.Status.Health;
                    atk.text = GetSpriteStatusIcon(StatsType.Attack) + characterInfo.Status.Attack;
                    def.text = GetSpriteStatusIcon(StatsType.Defense) + characterInfo.Status.Defense;
                    crit.text = GetSpriteStatusIcon(StatsType.Critical) + characterInfo.Status.Critical;
                    spd.text = GetSpriteStatusIcon(StatsType.Speed) + characterInfo.Status.Speed;
                    step.text = GetSpriteStatusIcon(StatsType.Step) + characterInfo.Status.Step;
                    agi.text = GetSpriteStatusIcon(StatsType.Agility) + characterInfo.Status.Agility;
                    aim.text = GetSpriteStatusIcon(StatsType.Aim) + characterInfo.Status.Aim;
                }
        }

        private void InstantiateTag(GameObject tagPref, Transform tagContent, string tagName, Color color = new Color())
        {
            var tagLabel = Instantiate(tagPref, tagContent);
            var script = tagLabel.GetComponent<TooltipActive>();
            script.TagName = tagName;
            script.CardType = self.CardType;
            
            tagLabel.GetComponentInChildren<Image>().color = color;
            tagLabel.GetComponentInChildren<TMP_Text>().color = TextMatchBackground(color);
            tagLabel.GetComponentInChildren<TMP_Text>().text = tagName;
        }

        private Color TextMatchBackground(Color bgColor)
        {
            //return new Color(1 - bgColor.r, 1 - bgColor.g, 1 - bgColor.b, bgColor.a);
            return bgColor.r * 0.2126f + bgColor.g * 0.7152f + bgColor.b * 0.0722f > 186 / 255f
                ? Color.black
                : Color.white;
        }

        private Sprite GetActiveTypeIcon(CardActiveType type)
        {
            Sprite sprite = null;

            switch (type)
            {
                case CardActiveType.WarField:
                case CardActiveType.WarFieldEnemy:
                    sprite = Resources.Load<Sprite>("warField");
                    break;
                case CardActiveType.Road:
                    sprite = Resources.Load<Sprite>("road");
                    break;
                case CardActiveType.Single:
                case CardActiveType.SingleEnemy:
                    sprite = Resources.Load<Sprite>("single");
                    break;
                case CardActiveType.Area:
                case CardActiveType.AreaEnemy:
                    sprite = Resources.Load<Sprite>("area");
                    break;
                case CardActiveType.World:
                    sprite = Resources.Load<Sprite>("world");
                    break;
            }

            return sprite;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var show = transform.GetChild(0).gameObject;
                var hide = transform.GetChild(1).gameObject;
                
                show.SetActive(!show.activeSelf);
                hide.SetActive(!hide.activeSelf);
            }
        }
    }
}