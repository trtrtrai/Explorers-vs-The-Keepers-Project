using System;
using System.Collections.Generic;
using System.Linq;
using EventArgs;
using Models;
using Models.Generals;
using ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Controllers
{
    public enum CardName
    {
        Platoon,
        Wolf,
        ArmorSoldier,
        Spybot,
        Treant,
        Pangolin,
        Commander,
        TheForest,
        MovingBomb,
        MinefieldWorker,
        Vulture,
        Golem,
        SuperMiner,
        TheRock,
        GunReload,
        TargetLauncher,
        AngrySpirit,
        Protect,
        StunBomb,
        SaleOff,
        GoHome,
        MindConnect,
        SmashRock,
        SupportPack,
        RollingRock,
        Teleport,
        DestructiveRay,
        ThermonuclearBomb,
        PoisonSwamp,
        AxitRain,
    }
    
    /// <summary>
    /// Shuffle, draw new card, check consume card, check generals required.
    /// </summary>
    public class CardController : MonoBehaviour
    {
        public static CardController Instance
        {
            get;
            private set;
        }
        //todo: limit 1 General card on hand + War Field, only while Generals on War Field death => have ratio draw General + reset required
        public event EventHandler<DrawNewCardEventArgs> OnTeam1DrawCard;
        public event EventHandler<DrawNewCardEventArgs> OnTeam2DrawCard;
        
        private const string Assembly = "Models.Generals.";

        [SerializeField] private List<CharacterCardCopyLimit> limits;

        [SerializeField] private List<CardInfo> team1Deck;
        [SerializeField] private List<CardInfo> team2Deck;

        [SerializeField] private List<int> team1CardOffHand;
        [SerializeField] private List<int> team2CardOffHand;
        
        [SerializeField] private List<GeneralCheckCanSummon> team1Required;
        [SerializeField] private List<GeneralCheckCanSummon> team2Required;

        [SerializeField] private List<int> team1Hand;
        [SerializeField] private List<int> team2Hand;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void Setup(List<CardName> team1, List<CardName> team2)
        {
            team1Deck.Clear();
            team1Deck = new();
            team2Deck.Clear();
            team2Deck = new();
            
            team1CardOffHand.Clear();
            team1CardOffHand = new();
            team2CardOffHand.Clear();
            team2CardOffHand = new();
            
            team1Required.ForEach(checker => Destroy(checker.gameObject));
            team1Required.Clear();
            team1Required = new();
            team2Required.ForEach(checker => Destroy(checker.gameObject));
            team2Required.Clear();
            team2Required = new();

            team1Hand.Clear();
            team1Hand = new();
            team2Hand.Clear();
            team2Hand = new();

            for (int i = 0; i < 10; i++)
            {
                if (i < team1.Count)
                {
                    AddCard(team1[i].ToString(), 0);
                }
                
                if (i < team2.Count)
                {
                    AddCard(team2[i].ToString(), 1);
                }
            }
        }

        public void StartGame()
        {
            for (int i = 0; i < 5; i++)
            {
                RandomCard(0, i);
                RandomCard(1, i);
            }
        }

        private int GetMaxCharacterCardOnHand(List<CharacterTag> tags)
        {
            /*
             * Others 3 copy
             * HqPrior 2 copy
             * Destroyer 1 copy
             * Reinforce 1 copy
             */
            foreach (var limitTag in from cTag in tags from limitTag in limits where cTag == limitTag.CharacterTag select limitTag) // ????
            {
                return limitTag.Limit;
            }

            return 3;
        }

        private void RandomCard(int team, int handIndex)
        {
            var listRand = new List<int>();
            if (team == 0)
            {
                for (int i = 0; i < team1Deck.Count; i++)
                {
                    var j = i;
                    var minus = 0;
                    
                    var cardInHand = team1Hand.FindAll(index => index == j);
                    minus = cardInHand.Count;
                    
                    var count = team1CardOffHand[i];

                    for (int k = 0; k < count - minus; k++)
                    {
                        listRand.Add(j);
                    }
                }
            }
            else
            {
                for (int i = 0; i < team2Deck.Count; i++)
                {
                    var j = i;
                    var minus = 0;
                    var cardInHand = team2Hand.FindAll(index => index == j);
                    minus = cardInHand.Count;
                    var count = team2CardOffHand[i];

                    for (int k = 0; k < count - minus; k++)
                    {
                        listRand.Add(j);
                    }
                }
            }
            
            var rand = new Random().Next(listRand.Count);
            //Debug.Log($"{rand} {listRand.Count}");
            DrawCard(listRand[rand], team, handIndex);
        }

        private void DrawCard(int deckIndex, int team, int handIndex)
        {
            if (team == 0)
            {
                team1Hand.Insert(handIndex, deckIndex);
                OnTeam1DrawCard?.Invoke(this, new DrawNewCardEventArgs(team1Deck[deckIndex], handIndex));
            }
            else
            {
                team2Hand.Insert(handIndex, deckIndex);;
                OnTeam2DrawCard?.Invoke(this, new DrawNewCardEventArgs(team2Deck[deckIndex], handIndex));
            }
        }

        private void AddCard(string cardName, int team)
        {
            var cardInfo = Resources.Load<CardInfo>($"ScriptableObjects/Cards/{cardName}Card");
            if (team == 0)
            {
                team1Deck.Add(cardInfo);

                switch (cardInfo.CardType)
                {
                    case CardType.Minions:
                        var tags = cardInfo.Character.GetComponent<Character>().CharacterInfo.CharacterTags;
                        team1CardOffHand.Add(GetMaxCharacterCardOnHand(tags));
                        break;
                    case CardType.Generals:
                        CreateChecker(cardName, team);
                        team1CardOffHand.Add(1);
                        break;
                    case CardType.Spells:
                        team1CardOffHand.Add(1);
                        break;
                }
            }
            else
            {
                team2Deck.Add(cardInfo);
                
                switch (cardInfo.CardType)
                {
                    case CardType.Minions:
                        var tags = cardInfo.Character.GetComponent<Character>().CharacterInfo.CharacterTags;
                        team2CardOffHand.Add(GetMaxCharacterCardOnHand(tags));
                        break;
                    case CardType.Generals:
                        CreateChecker(cardName, team);
                        team2CardOffHand.Add(1);
                        break;
                    case CardType.Spells:
                        team2CardOffHand.Add(1);
                        break;
                }
            }
        }

        private void CreateChecker(string generalName, int team)
        {
            var obj = new GameObject();
            obj.transform.SetParent(transform);
            var generalType = Type.GetType(Assembly + generalName);
            var script = obj.AddComponent(generalType) as GeneralCheckCanSummon;
            
            script.Team = team;
            if (team == 0) team1Required.Add(script);
            else team2Required.Add(script);

            obj.name = $"{generalName} {team}";
        }

        public bool CheckCanSummon(string generalName, int team)
        {
            var listCheck = team == 0 ? team1Required : team2Required;
            var generalType = Type.GetType(Assembly + generalName);
            foreach (var generalCheck in listCheck)
            {
                if (generalCheck.Team == team && generalType == generalCheck.GetType())
                {
                    return generalCheck.GeneralCanSummon();
                }
            }

            return false;
        }

        /// <summary>
        /// Card can active with energy manager.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CardConsuming(Card card, EnergyManager target)
        {
            if (target.UseCard(card))
            {
                //Debug.Log(card.name + " activated.");
                var team = LayerMask.LayerToName(target.gameObject.layer).Equals("Team1") ? 0 : 1;

                if (team == 0)
                {
                    team1Hand.RemoveAt(card.HandIndex);
                    Destroy(card.gameObject);
                    if (team1Hand.Count < 5) RandomCard(team, card.HandIndex); // == 4 ? if [1:3] ?
                }
                else
                {
                    team2Hand.RemoveAt(card.HandIndex);
                    Destroy(card.gameObject);
                    if (team2Hand.Count < 5) RandomCard(team, card.HandIndex);
                }
                
                return true;
            }

            return false;
        }

        #region AI Training Using

        public bool CardCanBeUsed(int team, CardInfo card)
        {
            if (team == 0)
            {
                if (WorldManager.Instance.PlayerEnergy.CardCanBeUsed(card))
                {
                    return true;
                }
            }
            else
            {
                if (WorldManager.Instance.EnemyEnergy.CardCanBeUsed(card))
                {
                    return true;
                }
            }

            return false;
        }
        
        public List<CardName> GetCardCanBeUsed(int team)
        {
            var list = new List<CardName>();
            
            if (team == 0)
            {
                foreach (var deckIndex in team1Hand)
                {
                    var card = team1Deck[deckIndex];
                    if (WorldManager.Instance.PlayerEnergy.CardCanBeUsed(card))
                    {
                        list.Add((CardName)Enum.Parse(typeof(CardName), card.name.Substring(0, card.name.Length - 4)));
                    }
                }
            }
            else
            {
                foreach (var deckIndex in team2Hand)
                {
                    var card = team2Deck[deckIndex];
                    if (WorldManager.Instance.EnemyEnergy.CardCanBeUsed(card))
                    {
                        list.Add((CardName)Enum.Parse(typeof(CardName), card.name.Substring(0, card.name.Length - 4)));
                    }
                }
            }

            return list;
        }

        public bool IsHandContains(int team, int cardNameIndex, out CardInfo card)
        {
            card = null;
            var cardEnum = (CardName)cardNameIndex;
            var signal = false;
            
            if (team == 0)
            {
                foreach (var i in team1Hand)
                {
                    if (TeamDeckCardName(team1Deck[i]).Equals(cardEnum.ToString()))
                    {
                        card = team1Deck[i];
                        signal = true;
                        break;
                    }
                }
            }
            else
            {
                foreach (var i in team2Hand)
                {
                    if (TeamDeckCardName(team2Deck[i]).Equals(cardEnum.ToString()))
                    {
                        card = team2Deck[i];
                        signal = true;
                        break;
                    }
                }
            }
            
            return signal;
        }

        public void CardConsuming(int team, CardInfo card)
        {
            var target = team == 0 ? WorldManager.Instance.PlayerEnergy : WorldManager.Instance.EnemyEnergy;
            if (target.UseCard(card))
            {
                //Debug.Log(card.name + " activated.");
                CardRedraw(team, card);
            }
        }

        public void CardRedraw(int team, CardInfo card)
        {
            if (team == 0)
            {
                var cardDeck = team1Deck.FirstOrDefault(c => c.name.Equals(card.name));
                var cardDeckIndex = team1Deck.IndexOf(cardDeck);
                team1Hand.Remove(cardDeckIndex);
                if (team1Hand.Count < 5) RandomCard(team); // == 4 ? if [1:3] ?
            }
            else
            {
                var cardDeck = team2Deck.FirstOrDefault(c => c.name.Equals(card.name));
                var cardDeckIndex = team2Deck.IndexOf(cardDeck);
                team2Hand.Remove(cardDeckIndex);
                if (team2Hand.Count < 5) RandomCard(team);
            }
        }
        
        private void RandomCard(int team)
        {
            var listRand = new List<int>();
            if (team == 0)
            {
                for (int i = 0; i < team1Deck.Count; i++)
                {
                    var j = i;
                    var minus = 0;
                    
                    var cardInHand = team1Hand.FindAll(index => index == j);
                    minus = cardInHand.Count;
                    
                    var count = team1CardOffHand[i];

                    for (int k = 0; k < count - minus; k++)
                    {
                        listRand.Add(j);
                    }
                }
            }
            else
            {
                for (int i = 0; i < team2Deck.Count; i++)
                {
                    var j = i;
                    var minus = 0;
                    var cardInHand = team2Hand.FindAll(index => index == j);
                    minus = cardInHand.Count;
                    var count = team2CardOffHand[i];

                    for (int k = 0; k < count - minus; k++)
                    {
                        listRand.Add(j);
                    }
                }
            }
            
            var rand = new Random().Next(listRand.Count);
            //Debug.Log($"{rand} {listRand.Count}");
            DrawCard(listRand[rand], team);
        }
        
        private void DrawCard(int deckIndex, int team)
        {
            if (team == 0)
            {
                team1Hand.Add(deckIndex);
                OnTeam1DrawCard?.Invoke(this, new DrawNewCardEventArgs(team1Deck[deckIndex], team1Hand.Count - 1));
            }
            else
            {
                team2Hand.Add(deckIndex);
                OnTeam2DrawCard?.Invoke(this, new DrawNewCardEventArgs(team1Deck[deckIndex], team1Hand.Count - 1));
            }
        }

        private string TeamDeckCardName(CardInfo card) => card.name.Substring(0, card.name.Length - 4);

        public int GetCardType(CardName cardName, int team)
        {
            var card = team == 0 ? team1Deck.FirstOrDefault(c => TeamDeckCardName(c).Equals(cardName.ToString())) : team2Deck.FirstOrDefault(c => TeamDeckCardName(c).Equals(cardName.ToString()));
            
            if (card is null) return -1;
            return (int)card.CardType;
        }

        #endregion
    }
}