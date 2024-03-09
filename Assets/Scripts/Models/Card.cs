using ScriptableObjects;
using UnityEngine;

namespace Models
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardInfo cardInfo;
        
        /*#region test

        private void Awake()
        {
            SetupCard(cardInfo);
            //Debug.Log(cardInfo.Radius);
        }

        #endregion*/

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

        public string ObjectName => cardInfo.name;
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
        
        public int HandIndex { get; private set; }

        public void SetupCard(CardInfo card, int handIndex)
        {
            if (isSetup) return;
            isSetup = true;

            cardInfo = card;
            HandIndex = handIndex;
            canvasGroup = GetComponent<CanvasGroup>();
            
            cardName = card.Name;
            gameObject.name = Name;
            cardType = card.CardType;
            activeType = card.ActiveType;
            radius = card.Radius;
            cost = card.Cost;
            cardIcon = card.CardIcon;
            description = card.Description;
            
            spellsEffect = card.SpellsEffect;
            character = card.Character;
            
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