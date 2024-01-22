using ScriptableObjects;
using UnityEngine;

namespace Models
{
    public class Card : MonoBehaviour
    {
        #region test

        [SerializeField] private CardInfo cardInfo;

        private void Awake()
        {
            SetupCard(cardInfo);
            //Debug.Log(cardInfo.Radius);
        }

        #endregion

        [SerializeField] private string cardName;
        [SerializeField] private CardType cardType;
        [SerializeField] private CardActiveType activeType;
        [SerializeField] private int radius;
        [SerializeField] private int cost;
        [SerializeField] private Sprite cardIcon;
        [SerializeField] private string description;
        
        [SerializeField] private SpellsEffect spellsEffect;
        [SerializeField] private GameObject character;

        [SerializeField] private bool isSetup;

        [SerializeField] private CanvasGroup canvasGroup;
        public string Name => cardName;
        public CardType CardType => cardType;
        public CardActiveType ActiveType => activeType;
        public int Radius => radius;
        public int Cost => cost;
        public Sprite CardIcon => cardIcon;
        public string Description => description;

        public SpellsEffect SpellsEffect => spellsEffect;
        public GameObject Character => character;

        public bool IsSetup => isSetup;

        public void SetupCard(CardInfo cardInfo)
        {
            if (isSetup) return;
            isSetup = true;
            
            canvasGroup = GetComponent<CanvasGroup>();
            
            cardName = cardInfo.Name;
            gameObject.name = Name;
            cardType = cardInfo.CardType;
            activeType = cardInfo.ActiveType;
            radius = cardInfo.Radius;
            cost = cardInfo.Cost;
            cardIcon = cardInfo.CardIcon;
            description = cardInfo.Description;
            
            spellsEffect = cardInfo.SpellsEffect;
            character = cardInfo.Character;
            
            UnHideCard();
        }

        public void HideCard()
        {
            canvasGroup.alpha = 0.2f;
        }

        public void UnHideCard()
        {
            canvasGroup.alpha = 1f;
        }
    }
}