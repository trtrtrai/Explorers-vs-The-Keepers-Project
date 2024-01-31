using System.Collections;
using Models;
using ScriptableObjects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class CardInfoUI : MonoBehaviour
    {
        [SerializeField] private Card self;

        [Header("Card Forward")]
        [SerializeField] private TMP_Text cardType;
        [SerializeField] private TMP_Text activeType; //icon after
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

                cardName.text = self.Name;
                cost.text = self.Cost + "";
                cardIcon.sprite = self.CardIcon;
                description.text = self.Description;
                cardType.text = self.CardType.ToString();
                activeType.text = self.ActiveType.ToString();

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

                    hp.text = "<sprite=\"StatusIcon\" index=0>" + characterInfo.Status.Health;
                    atk.text = "<sprite=\"StatusIcon\" index=1>" + characterInfo.Status.Attack;
                    def.text = "<sprite=\"StatusIcon\" index=2>" + characterInfo.Status.Defense;
                    crit.text = "<sprite=\"StatusIcon\" index=3>" + characterInfo.Status.Critical;
                    spd.text = "<sprite=\"StatusIcon\" index=4>" + characterInfo.Status.Speed;
                    step.text = "<sprite=\"StatusIcon\" index=5>" + characterInfo.Status.Step;
                    agi.text = "<sprite=\"StatusIcon\" index=6>" + characterInfo.Status.Agility;
                    aim.text = "<sprite=\"StatusIcon\" index=7>" + characterInfo.Status.Aim;
                }
            }
            else
            {
                Destroy(this);
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                transform.GetChild(1).SetAsFirstSibling();
            }
        }
    }
}