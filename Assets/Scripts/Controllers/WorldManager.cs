using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agents;
using EventArgs;
using Extensions;
using GUI;
using Models;
using Models.SpecialCharacter;
using Models.Spells;
using ScriptableObjects;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace Controllers
{
    /// <summary>
    /// Execute task for both 2 team.
    /// </summary>
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance
        {
            get;
            private set;
        }

        public event EventHandler<CharacterSpawnEventArgs> CharacterSpawn;
        public event EventHandler OnGameEnded;
        public event EventHandler OnGameReset;

        [SerializeField] private Transform aiBot;
        [SerializeField] private Transform aiBotTeam0;

        [SerializeField] private GameObject energyManagerPref;
        [SerializeField] private GameObject cardControllerPref;
        [SerializeField] private GameObject characterContainer;
        [SerializeField] private GameObject objectContainer;
        [SerializeField] private GameObject headquarter;

        [SerializeField] private EnergyManager playerEnergy;
        [SerializeField] private EnergyManager enemyEnergy;

        [SerializeField] private Headquarter team1Hq;
        [SerializeField] private Headquarter team2Hq;
        
        [SerializeField] private bool team1Player;
        [SerializeField] private bool playerExplorer;
        [SerializeField] private Vector3 team1Rotation;
        [SerializeField] private Vector3 team2Rotation;
        [SerializeField] private Card cardWaitingActive = null;
        [SerializeField] private bool cardCanActive;
        [SerializeField] private Droppable dropPosition = null;
        [SerializeField] private Character characterTarget;
        [SerializeField] private List<Droppable> listActive;
        
        [SerializeField] private TileData team1StartPoint;
        [SerializeField] private TileData team2StartPoint;

        [SerializeField] private List<RoadPath> roads;

        [SerializeField] private int roadAmount;
        public int RoadAmount => roadAmount;
        public TileData Team1StartPoint => team1StartPoint;
        public TileData Team2StartPoint => team2StartPoint;

        public EnergyManager PlayerEnergy => playerEnergy;
        public EnergyManager EnemyEnergy => enemyEnergy;

        public bool Team1Player => team1Player;

        public Transform ObjectContainer => objectContainer.transform;

        [SerializeField] private int characterTagAmount;
        [SerializeField] private List<Character> worldCharacters;

        public List<CharacterTagPriority> Priorities;
        private BitArray prioritiesBitArr;

        private List<List<TileData>> team1Paths;
        private List<List<TileData>> team2Paths;

        [SerializeField] private List<CardName> team1CardList;
        [SerializeField] private List<CardName> team2CardList;

        [SerializeField] private bool initialize;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;

                worldCharacters = new();
                roadAmount = roads.Count;
                
                team1Paths = new List<List<TileData>>();
                team2Paths = new List<List<TileData>>();
                for (int i = 0; i < roadAmount; i++)
                {
                    team1Paths.Add(GetRoad(0, i));
                    team2Paths.Add(GetRoad(1, i));
                }
                
                for (var i = 0; i < team1Paths.Count; i++)
                {
                    var road = team1Paths[i];
                    for (var j = 1; j < road.Count - 1; j++) // index first and end is start point
                    {
                        var tile = road[j];
                        var droppable = tile.AddComponent<Droppable>();
                        droppable.RoadIndex = i;
                        droppable.Team1PositionIndex = j;
                        droppable.Team2PositionIndex = road.Count - 1 - j;

                        var meshCollider = tile.AddComponent<MeshCollider>();
                        meshCollider.convex = true;
                    }
                }
                
                characterTagAmount = Enum.GetValues(typeof(CharacterTag)).Length;
                var characterTag = Priorities.Select(p => p.Subject).ToList();
                prioritiesBitArr = CreateCharacterTag(characterTag);
                
                var pMng = Instantiate(energyManagerPref, transform);
                pMng.layer = GetAllyLayerOfPlayer();
                playerEnergy = pMng.GetComponent<EnergyManager>();
                
                var eMng = Instantiate(energyManagerPref, transform);
                eMng.layer = GetEnemyLayerOfPlayer();
                enemyEnergy = eMng.GetComponent<EnergyManager>();

                characterContainer = new GameObject
                {
                    name = "Character Container"
                };

                objectContainer = new GameObject
                {
                    name = "Object Container"
                };

                var team1Headquarter = Instantiate(headquarter, characterContainer.transform);
                team1Hq = team1Headquarter.GetComponent<Headquarter>();
                team1Hq.Position = team1StartPoint;
                team1Hq.SetupHeadquarter(playerExplorer, team1Rotation);
                team1Headquarter.transform.localPosition = team1StartPoint.transform.localPosition;
                team1Hq.Setup(0, 0);
                team1Hq.OnCharacterDeath += OnEndGame;
                
                var team2Headquarter = Instantiate(headquarter, characterContainer.transform);
                team2Hq = team2Headquarter.GetComponent<Headquarter>();
                team2Hq.Position = team2StartPoint;
                team2Hq.SetupHeadquarter(!playerExplorer, team2Rotation);
                team2Headquarter.transform.localPosition = team2StartPoint.transform.localPosition;
                team2Hq.Setup(0, 1);
                team2Hq.OnCharacterDeath += OnEndGame;
            }
        }

        private void Start()
        {
            Instantiate(cardControllerPref, transform);
            StartCoroutine(WaitToSettingCardController());
        }

        public void SoftReset()
        {
            initialize = !initialize;
            if (initialize) return;
            
            OnGameEnded -= aiBot.GetComponent<Mission1Bot>().OnHeadquarterDestroy;
            //CharacterSpawn -= aiBot.GetComponent<Mission5State2Bot>().OnCharacterSpawn;
            aiBot.GetComponent<Agent>().enabled = false;
            
            /*OnGameEnded -= aiBotTeam0.GetComponent<Mission5State2Bot>().OnHeadquarterDestroy;
            CharacterSpawn -= aiBotTeam0.GetComponent<Mission5State2Bot>().OnCharacterSpawn;
            aiBotTeam0.GetComponent<Agent>().enabled = false;*/
            
            OnGameReset?.Invoke(this, System.EventArgs.Empty);
            worldCharacters.ForEach(c => Destroy(c.gameObject));
            worldCharacters.Clear();

            foreach (Transform t in objectContainer.transform)
            {
                Destroy(t.gameObject);
            }
            
            team1Hq.OnCharacterDeath -= OnEndGame;
            team2Hq.OnCharacterDeath -= OnEndGame;
            Destroy(team1Hq.gameObject);
            Destroy(team2Hq.gameObject);
            
            var team1Headquarter = Instantiate(headquarter, characterContainer.transform);
            team1Hq = team1Headquarter.GetComponent<Headquarter>();
            team1Hq.Position = team1StartPoint;
            team1Headquarter.transform.localPosition = team1StartPoint.transform.localPosition;
            team1Hq.Setup(0, 0);
            team1Hq.OnCharacterDeath += OnEndGame;
                
            var team2Headquarter = Instantiate(headquarter, characterContainer.transform);
            team2Hq = team2Headquarter.GetComponent<Headquarter>();
            team2Hq.Position = team2StartPoint;
            team2Headquarter.transform.localPosition = team2StartPoint.transform.localPosition;
            team2Hq.Setup(0, 1);
            team2Hq.OnCharacterDeath += OnEndGame;
            
            StartCoroutine(WaitToSettingCardController());
        }

        private IEnumerator WaitToSettingCardController()
        {
            while (CardController.Instance is null)
            {
                yield return null;
            }
            
            CardController.Instance.Setup(team1CardList, team2CardList);

            yield return new WaitForSeconds(0.1f);
            GameStart();
        }

        private void GameStart()
        {
            playerEnergy.SetupAndStart();
            enemyEnergy.SetupAndStart();
            
            CardController.Instance.StartGame();
            
            aiBot.GetComponent<Agent>().enabled = true;
            OnGameEnded += aiBot.GetComponent<Mission1Bot>().OnHeadquarterDestroy;
            //CharacterSpawn += aiBot.GetComponent<Mission5State2Bot>().OnCharacterSpawn;
            
            /*aiBotTeam0.GetComponent<Agent>().enabled = true;
            OnGameEnded += aiBotTeam0.GetComponent<Mission5State2Bot>().OnHeadquarterDestroy;
            CharacterSpawn += aiBotTeam0.GetComponent<Mission5State2Bot>().OnCharacterSpawn;*/
        }

        private void OnEndGame(object sender, CharacterDeathEventArgs args)
        {
            if (sender is not Headquarter hq) return;

            var won = LayerMask.LayerToName(hq.gameObject.layer).Equals("Team1") ? 1 : 0;
            Debug.Log("Game ended " + won + " won!");
            OnGameEnded?.Invoke(won, System.EventArgs.Empty);
        }
        
        #region Card drag system control all behaviour of card dragging.
        
        /// <summary>
        /// Card dragged into Character Information rect. Use for Single active type tags only.
        /// </summary>
        /// <param name="target"></param>
        public void CardEnterCharacterInfoUI(Character target)
        {
            if (target is null || cardWaitingActive is null) return;
            if (cardWaitingActive.ActiveType != CardActiveType.Single &&
                cardWaitingActive.ActiveType != CardActiveType.SingleEnemy) return;
            if (!worldCharacters.Contains(target)) return;
                            
            var layer = cardWaitingActive.ActiveType == CardActiveType.Single ? GetAllyLayerOfPlayer() : GetEnemyLayerOfPlayer(); // single or single enemy

            if (target.gameObject.layer == layer)
            {
                characterTarget = target;
                cardCanActive = true;
                cardWaitingActive.HideCard();
            }
        }
        
        /// <summary>
        /// Card dragged out of Character Information rect. Use for Single active type tags only.
        /// </summary>
        /// <param name="target"></param>
        public void CardExitCharacterInfoUI(Character target)
        {
            if (target is null || cardWaitingActive is null) return;
            if (cardWaitingActive.ActiveType != CardActiveType.Single &&
                cardWaitingActive.ActiveType != CardActiveType.SingleEnemy) return;
            if (!worldCharacters.Contains(target)) return;
            if (!cardCanActive) return;
            
            cardCanActive = false;
            cardWaitingActive.UnHideCard();
        }

        /// <summary>
        /// Card entered tile of any road in the map.
        /// </summary>
        /// <param name="droppable"></param>
        public void CardEnterDroppable(Droppable droppable)
        {
            if (characterTarget is not null) return; //detect single active type card
            if (cardWaitingActive is null) return;
            
            dropPosition = droppable;
            
            GUICardActive();
        }
        
        /// <summary>
        /// Card exit the tile previous.
        /// </summary>
        /// <param name="droppable"></param>
        public void CardExitDroppable(Droppable droppable)
        {
            if (characterTarget is not null) return;
            if (cardWaitingActive is null) return;
            
            // Detect some active type can active in any position
            switch (cardWaitingActive.ActiveType)
            {
                case CardActiveType.WarField:
                case CardActiveType.WarFieldEnemy:
                case CardActiveType.World:
                    return;
                default:
                    cardCanActive = false;
                    GUICardDisable(droppable);
                    break;
            }
        }

        /// <summary>
        /// Find area of card effect. Use for dropping card that required drop into the map.
        /// </summary>
        /// <param name="origin"></param>
        private void ActiveAreaList(TileData origin)
        {
            listActive = new() { dropPosition };
            var radius = cardWaitingActive.Radius;
            if (radius == 0)
            {
                return;
            }
            
            var listTiles = new List<TileData>();
            var listTilePos = new TilePosition[6];
            
            for (int i = 0; i < 6; i++)
            {
                var neighbor = origin.GetNeighbor(i);

                if (neighbor is null)
                {
                    var signal = origin.TilePosition.GetPositionFromDirection(i, out float x, out float z);
                        
                    if (signal)
                    {
                        listTilePos[i] = new ()
                        {
                            X = x,
                            Z = z
                        };
                    }
                }
                else
                {
                    listTiles.Add(neighbor);
                    listTilePos[i] = neighbor.TilePosition;
                }
            }

            foreach (var tileData in listTiles)
            {
                if (tileData.TryGetComponent(out Droppable droppable))
                {
                    listActive.Add(droppable);
                }
            }

            if (radius == 1)
            {
                return;
            }
            var currentRadius = 2;

            while (currentRadius <= radius)
            {
                var newTilePos = new TilePosition[6];

                for (int i = 0; i < 6; i++)
                {
                    var signal = listTilePos[i].GetPositionFromDirection(i, out float x, out float z);

                    if (signal)
                    {
                        newTilePos[i] = new()
                        {
                            X = x,
                            Z = z
                        };
                        
                        foreach (var path in team1Paths)
                        {
                            var result = path.GetEqualsTilePositionFrom(newTilePos[i], out TileData tileData);

                            if (result && !tileData.Equals(team1StartPoint) && !tileData.Equals(team2StartPoint))
                            {
                                listActive.Add(tileData.GetComponent<Droppable>());
                            }
                        }
                    }
                }

                for (int j = 0; j < 6; j++)
                {
                    var curTilePos = newTilePos[j];
                    var direct = curTilePos.GetDirectionFromPosition(newTilePos[(j + 1) % 6]);
                    if (direct == -1) continue;
            
                    for (int i = 0; i < currentRadius - 1; i++)
                    {
                        var signal = curTilePos.GetPositionFromDirection(direct, out float x, out float z);

                        if (signal)
                        {
                            var pos = new TilePosition
                            {
                                X = x,
                                Z = z
                            };

                            foreach (var path in team1Paths)
                            {
                                var result = path.GetEqualsTilePositionFrom(pos, out TileData tileData);

                                if (result && !tileData.Equals(team1StartPoint) && !tileData.Equals(team2StartPoint))
                                {
                                    listActive.Add(tileData.GetComponent<Droppable>());
                                }
                            }
                            
                            curTilePos = pos;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                currentRadius++;
                listTilePos = newTilePos;
            }
        }

        /// <summary>
        /// Appear future effect of card. Change parts of the road in the map to active mode.
        /// </summary>
        private void GUICardActive()
        {
            switch (cardWaitingActive.ActiveType)
            {
                case CardActiveType.Road:
                    // All tiles of road change color
                    var roadActive = team1Paths[dropPosition.RoadIndex]; // Same with team2Paths, it is not important in this case

                    foreach (var tile in roadActive)
                    {
                        if (tile.TryGetComponent(out Droppable droppable))
                        {
                            droppable.ActiveChoosing();
                        }
                    }

                    cardCanActive = true;
                    cardWaitingActive.HideCard();
                    break;
                case CardActiveType.Area:
                    // Many tiles around this position convert into active mode
                    if (dropPosition.TryGetComponent(out TileData tileData))
                    {
                        //Debug.Log(tileData.TilePosition);
                        ActiveAreaList(tileData);
                        listActive.ForEach(d => d.ActiveChoosing());

                        cardCanActive = true;
                        cardWaitingActive.HideCard();
                    }
                    break;
                case CardActiveType.AreaEnemy:
                    // Many tiles around this position convert into active mode
                    if (dropPosition.TryGetComponent(out TileData tileD))
                    {
                        //Debug.Log(tileData.TilePosition);
                        ActiveAreaList(tileD);
                        listActive.ForEach(d => d.ActiveChoosing());

                        cardCanActive = true;
                        cardWaitingActive.HideCard();
                    }
                    break;  
            }
        }

        /// <summary>
        /// Disappear future effect of card. Change parts of the road in the map to inactive mode.
        /// </summary>
        /// <param name="realDroppable"></param>
        private void GUICardDisable(Droppable realDroppable)
        {
            var cardActiveType = cardWaitingActive.ActiveType;
            cardWaitingActive.UnHideCard();
            
            // reset droppable
            switch (cardActiveType)
            {
                case CardActiveType.Road:
                    var roadActive = team1Paths[realDroppable.RoadIndex];

                    foreach (var tile in roadActive)
                    {
                        if (tile.TryGetComponent(out Droppable droppable))
                        {
                            droppable.DisableChoosing();
                        }
                    }
                    break;
                case CardActiveType.Single:
                    realDroppable.DisableChoosing();
                    break;
                case CardActiveType.Area:
                    if (dropPosition.TryGetComponent(out TileData tileData))
                    {
                        //Debug.Log(tileData.TilePosition);
                        listActive.ForEach(t => t.DisableChoosing());
                    }
                    break;
                case CardActiveType.SingleEnemy:
                    realDroppable.DisableChoosing();
                    break;
                case CardActiveType.AreaEnemy:
                    if (dropPosition.TryGetComponent(out TileData tileD))
                    {
                        //Debug.Log(tileData.TilePosition);
                        listActive.ForEach(t => t.DisableChoosing());
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Card beginning drag detect.
        /// </summary>
        /// <param name="card"></param>
        public void CardBeginDrag(Card card)
        {
            cardWaitingActive = card;

            // Detect some active type do not required to active
            cardCanActive = card.ActiveType == CardActiveType.WarField 
                            || cardWaitingActive.ActiveType == CardActiveType.WarFieldEnemy 
                            || cardWaitingActive.ActiveType == CardActiveType.World;
        }
        
        /// <summary>
        /// Invoke after release dragged card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isOutOfHand"></param>
        public void CardEndDrag(Card card, bool isOutOfHand)
        {
            if (dropPosition is not null) GUICardDisable(dropPosition);
            
            if (!isOutOfHand || !cardWaitingActive.Equals(card) || !cardCanActive)
            {
                ResetCardDrag();
                return;
            }
            
            switch (card.CardType)
            {
                case CardType.Generals:
                {
                    if (CardController.Instance.CheckCanSummon(card.Name.Replace(" ", ""), GetAllyTeam()))
                    {
                        var prefab = card.Character;
                        if (prefab is null || dropPosition is null) break;
                        
                        if (TryConsumeCard(card))
                        {
                            CreateCharacter(prefab, dropPosition.RoadIndex, team1Player ? 0 : 1);
                        }
                    }
                    break;
                }
                case CardType.Minions:
                {
                    var prefab = card.Character;
                    if (prefab is null || dropPosition is null) break;

                    if (TryConsumeCard(card))
                    {
                        CreateCharacter(prefab, dropPosition.RoadIndex, team1Player ? 0 : 1);
                    }
                    break;
                }
                case CardType.Spells when card.ActiveType is CardActiveType.Single or CardActiveType.SingleEnemy:
                    if (characterTarget is null) break;
                    
                    if (TryConsumeCard(card))
                    {
                        SpellsExecute.Activate(characterTarget, card.SpellsEffect);
                    }
                    break;
                case CardType.Spells when card.ActiveType is CardActiveType.Area or CardActiveType.AreaEnemy:
                {
                    switch (card.SpellsEffect)
                    {
                        case EnvironmentSpells environmentSpells:
                            if (listActive.Count == 0) break;
                            
                            if (TryConsumeCard(card))
                            {
                                environmentSpells.ListSettingUp = listActive.ConvertAll(d => d.GetComponent<TileData>());
                                SpellsExecute.Activate(null, card.SpellsEffect);
                            }
                            break;
                        default:
                        {
                            if (listActive.Count == 0 || worldCharacters.Count == 0) break;
                            
                            var layer = card.ActiveType == CardActiveType.Area ? GetAllyLayerOfPlayer() : GetEnemyLayerOfPlayer();
                            var characters = worldCharacters.FindAll(c => c.gameObject.layer == layer);
                            var charactersOnArea = characters.FindAll(c =>
                                listActive.Contains(c.Position.GetComponent<Droppable>()));

                            if (charactersOnArea.Count == 0) break;
                            
                            if (TryConsumeCard(card))
                            {
                                foreach (var character in charactersOnArea)
                                {
                                    SpellsExecute.Activate(character, card.SpellsEffect);
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
                case CardType.Spells when card.ActiveType is CardActiveType.WarField or CardActiveType.WarFieldEnemy:
                {
                    if (card.SpellsEffect.SpellsTags.Any(spellsTag => spellsTag == SpellsTag.Multiple)) // character effect
                    {
                        if (listActive.Count == 0 || worldCharacters.Count == 0) break;
                        
                        var layer = card.ActiveType == CardActiveType.WarField ? GetAllyLayerOfPlayer() : GetEnemyLayerOfPlayer();
                        var characters = worldCharacters.FindAll(c => c.gameObject.layer == layer);

                        if (characters.Count == 0) break;
                        
                        if (TryConsumeCard(card))
                        {
                            foreach (var character in characters)
                            {
                                SpellsExecute.Activate(character, card.SpellsEffect);
                            }
                        }
                    }
                    else if (card.SpellsEffect is EnergyBoostSpells energyBoostSpells)
                    {
                        if (TryConsumeCard(card))
                        {
                            energyBoostSpells.Target = playerEnergy; // because this method only use for player, not bot
                            SpellsExecute.Activate(null, card.SpellsEffect);
                        }
                    }
                    else // another effect except character
                    {
                        if (TryConsumeCard(card)) SpellsExecute.Activate(null, card.SpellsEffect);
                    }
                    break;
                }
                case CardType.Spells when card.ActiveType == CardActiveType.Road:
                {
                    var spellsEff = card.SpellsEffect;

                    if (spellsEff is SummonSpells summonSpells)
                    {
                        if (dropPosition is null) break;
                        
                        if (TryConsumeCard(card))
                        {
                            summonSpells.RoadIndex = dropPosition.RoadIndex;
                            summonSpells.Team = GetAllyTeam();
                            SpellsExecute.Activate(null, spellsEff);
                        }
                    }
                    break;
                }
                case CardType.Spells when card.ActiveType == CardActiveType.World:
                {
                    // Do not check character count, always use if it can (Mechanics)
                    //if (worldCharacters.Count == 0) break;
                    
                    if (TryConsumeCard(card))
                    {
                        foreach (var character in worldCharacters.ToList())
                        {
                            SpellsExecute.Activate(character, card.SpellsEffect);
                        }
                    }
                    break;
                }
            }
            
            ResetCardDrag();
        }
        
        /// <summary>
        /// Use for reset card drag system after player release dragged card.
        /// </summary>
        private void ResetCardDrag()
        {
            cardWaitingActive = null;
            dropPosition = null;
            cardCanActive = false;
            characterTarget = null;
        }
        
        #endregion

        #region Character get moving and attacking range
        
        /// <summary>
        /// Special method for reinforce character
        /// </summary>
        /// <param name="team"></param>
        /// <param name="roadIndex"></param>
        /// <param name="currentIndex"></param>
        /// <returns></returns>
        public List<TileData> GetPathBackToHeadquarter(int team, int roadIndex, int currentIndex)
        {
            var listTiles = new List<TileData>();
            
            List<TileData> path = team == 0 ? team1Paths[roadIndex] : team2Paths[roadIndex];

            currentIndex--; // except self
            for (int i = currentIndex; i >= 0; i--)
            {
                listTiles.Add(path[i]);
            }

            return listTiles;
        }

        /// <summary>
        /// The path that character can move once before rest.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="roadIndex"></param>
        /// <param name="currentIndex"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public List<TileData> GetPath(int team, int roadIndex, int currentIndex, int step)
        {
            var listTiles = new List<TileData>();
            
            List<TileData> path = team == 0 ? team1Paths[roadIndex] : team2Paths[roadIndex];
            var nextTile = currentIndex + 1;
            
            for (int i = 0; i < step; i++)
            {
                if (nextTile < path.Count)
                {
                    listTiles.Add(path[nextTile]);
                }
                else
                {
                    break;
                }
                    
                nextTile++;
            }

            return listTiles;
        }

        /// <summary>
        /// The path that character can attack to.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="roadIndex"></param>
        /// <param name="currentIndex"></param>
        /// <param name="aim"></param>
        /// <returns></returns>
        public List<TileData> GetAimPath(int team, int roadIndex, int currentIndex, int aim)
        {
            var listTiles = new List<TileData>();
            
            List<TileData> path = team == 0 ? team1Paths[roadIndex] : team2Paths[roadIndex];
            
            for (int i = -aim; i <= aim; i++)
            {
                var tileIndex = currentIndex + i;
                
                if (tileIndex < 0 || tileIndex >= path.Count) continue;
                
                listTiles.Add(path[tileIndex]);
            }

            return listTiles;
        }

        private List<TileData> GetRoad(int team, int roadIndex)
        {
            if (team != 0 && team != 1 || roadIndex < 0 || roadIndex > roadAmount - 1) return null;

            return GetPath(team, roadIndex);
        }
        
        /// <summary>
        /// Error check not yet. Only use surely setup. Get all path of a road.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="roadIndex"></param>
        /// <returns></returns>
        private List<TileData> GetPath(int team, int roadIndex)
        {
            var path = new List<TileData>();
            var origin = team == 0 ? team1StartPoint : team2StartPoint;
            var endPoint = team == 0 ? team2StartPoint : team1StartPoint;
            
            path.Add(origin);
            var listPoints = roads[roadIndex].RoadPoints;
            var fromPoint = origin;
            
            for (int i = 0; i < listPoints.Count; i++)
            {
                var index = team == 0 ? i : listPoints.Count - 1 - i;
                var direct = fromPoint.TilePosition.GetDirectionFromPosition(listPoints[index].TilePosition);
                //Debug.Log($"{index}: direct {direct}");
                var neighbor = fromPoint.GetNeighbor(direct);
                //Debug.Log($"{fromPoint.TilePosition} neighbor in direct {direct} is {neighbor.TilePosition}");
                path.Add(neighbor);
                fromPoint = neighbor;
                
                while (!neighbor.Equals(listPoints[index]))
                {
                    neighbor = fromPoint.GetNeighbor(direct);
                    path.Add(neighbor);
                    //Debug.Log($"Loop: {fromPoint.TilePosition} neighbor in direct {direct} is {neighbor.TilePosition}");
                    fromPoint = neighbor;
                }
                
                /*path.Add(listPoints[i]);
                fromPoint = listPoints[i];*/
            }
            
            var dir = fromPoint.TilePosition.GetDirectionFromPosition(endPoint.TilePosition);
            //Debug.Log($"endpoint direct {dir}");
            var nextTile = fromPoint.GetNeighbor(dir);
            path.Add(nextTile);
            fromPoint = nextTile;
                
            while (!nextTile.Equals(endPoint))
            {
                //Debug.Log(nextTile.TilePosition);
                nextTile = fromPoint.GetNeighbor(dir);
                path.Add(nextTile);
                fromPoint = nextTile;
            }

            return path;
        }
        
        #endregion

        #region Character tags setup and calculate
        
        public BitArray CreateCharacterTag(List<CharacterTag> characterTags)
        {
            var bitArr = new BitArray(characterTagAmount);

            foreach (var tags in characterTags)
            {
                bitArr[(int)tags] = true;
            }

            return bitArr;
        }
        
        public CharacterTagReferenceAttack GetAllPrioritiesOf(BitArray characterTag)
        {
            var listCharTagRefAtk = new CharacterTagReferenceAttack
            {
                CharacterTags = new BitArray(characterTagAmount),
                PrioritiesInfo = new List<CharacterTagPriorityInfo>()
            };

            var andBitArr = characterTag.Clone() as BitArray;
            
            if (andBitArr is null) return null;
            
            andBitArr.And(prioritiesBitArr);
            for (int i = 0; i < andBitArr.Length; i++)
            {
                if (andBitArr[i])
                {
                    var characterTagPriority = Priorities.FirstOrDefault(p => (int)p.Subject == i);
                    
                    if (characterTagPriority is null) continue;

                    foreach (var prior in characterTagPriority.ListObjects)
                    {
                        var tagIndex = (int)prior.Object;
                        if (!listCharTagRefAtk.CharacterTags[tagIndex])
                        {
                            listCharTagRefAtk.CharacterTags[tagIndex] = true;
                            listCharTagRefAtk.PrioritiesInfo.Add(new CharacterTagPriorityInfo
                            {
                                Object = prior.Object,
                                Priority = prior.Priority
                            });
                        }
                        else
                        {
                            var listIndex = listCharTagRefAtk.PrioritiesInfo.FindIndex(p => (int)p.Object == tagIndex);
                            listCharTagRefAtk.PrioritiesInfo[listIndex].Priority += prior.Priority;
                        }
                        
                    }
                }
            }

            // these tags ignore all other character tags
            if (characterTag[(int)CharacterTag.HqPrior] || characterTag[(int)CharacterTag.Destroyer])
            {
                for (int i = 0; i < listCharTagRefAtk.CharacterTags.Length; i++)
                {
                    if (!listCharTagRefAtk.CharacterTags[i])
                    {
                        listCharTagRefAtk.PrioritiesInfo.Add(new CharacterTagPriorityInfo
                        {
                            Priority = -1,
                            Object = (CharacterTag)i
                        });
                    }
                }
                
                listCharTagRefAtk.CharacterTags.SetAll(true);
            }

            return listCharTagRefAtk;
        }

        public int GetPriorityOf(BitArray target, CharacterTagReferenceAttack listCharRefAtk)
        {
            var priority = 0;

            // Enemy is SDefender and attacker do not is HqPrior or Destroyer
            if (target[(int)CharacterTag.SDefender])
            {
                if (!listCharRefAtk.CharacterTags[(int)CharacterTag.SDefender])
                {
                    return 101; // higher than headquarter
                }
            }
            
            // Enemy Sky Force check
            if (target[(int)CharacterTag.SkyForce])
            {
                if (!listCharRefAtk.CharacterTags[(int)CharacterTag.SkyForce])
                {
                    return -1;
                }
            }
            
            var andBitArr = target.Clone() as BitArray;

            if (andBitArr is null) return -1;
            
            andBitArr.And(listCharRefAtk.CharacterTags);
            for (int i = 0; i < andBitArr.Length; i++)
            {
                if (andBitArr[i])
                {
                    if (priority == -1) priority = 0;
                    
                    var index = i;
                    var priorityInfo = listCharRefAtk.PrioritiesInfo.FirstOrDefault(p => (int)p.Object == index);

                    if (priorityInfo is not null)
                    {
                        priority += priorityInfo.Priority;
                    }
                }
            }
            //Debug.Log("Priority " + priority);
            return priority;
        }
        
        public void LogBitArray(BitArray arr)
        {
            var str = "";
            foreach (bool bit in arr)
            {
                if (bit)
                {
                    str += "1";
                }
                else
                {
                    str += "0";
                }
            }
            Debug.Log(str);
        }
        
        #endregion

        public static GameObject InstantiatePrefab(GameObject prefab, Transform parent = null)
        {
            if (parent)
            {
                return Instantiate(prefab, parent);
            }

            return Instantiate(prefab);
        }
        
        public int GetAllyLayerOfPlayer()
        {
            var allyTeam = team1Player ? "Team1" : "Team2";

            return LayerMask.NameToLayer(allyTeam);
        }

        public int GetEnemyLayerOfPlayer()
        {
            var enemyTeam = team1Player ? "Team2" : "Team1";
            
            return LayerMask.NameToLayer(enemyTeam);
        }
        
        public int GetAllyTeam() => team1Player ? 0 : 1;

        public static int GetEnemyLayer(string myTeam) => LayerMask.NameToLayer(myTeam.Equals("Team1") ? "Team2" : "Team1");
        
        /// <summary>
        /// Check if enough energy to the card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="energyManager"></param>
        /// <returns></returns>
        private bool TryConsumeCard(Card card) =>
            CardController.Instance.CardConsuming(card, team1Player ? PlayerEnergy : EnemyEnergy);
        
        private void CreateCharacter(GameObject prefab, int roadIndex, int team) // After, it will also be assigned in event of AI script
        {
            var characterObj = Instantiate(prefab, characterContainer.transform);
            var character = characterObj.GetComponent<Character>();
            var startPoint = team == 0 ? team1StartPoint : team2StartPoint;
            character.Position = startPoint;
            characterObj.transform.localPosition = startPoint.transform.localPosition;
            character.Setup(roadIndex, team);
            
            worldCharacters.Add(character);
            character.OnCharacterDeath += OnCharacterDeath;
            CharacterSpawn?.Invoke(this, new CharacterSpawnEventArgs(character, team, roadIndex));
        }

        public void CreateCharacter(GameObject prefab, int roadIndex, int team, SpellsEffect a) // After, it will also be assigned in event of AI script
        {
            CreateCharacter(prefab, roadIndex, team);
        }

        public Character GetRandomAlly(int team)
        {
            var allies = worldCharacters.FindAll(c => c.gameObject.layer != GetEnemyLayer(team == 0 ? "Team1" : "Team2"));

            return allies.Count == 0 ? null : allies[new Random().Next(allies.Count)];
        }
        
        public Character GetRandomEnemy(int team)
        {
            var enemies = worldCharacters.FindAll(c => c.gameObject.layer == GetEnemyLayer(team == 0 ? "Team1" : "Team2"));

            return enemies.Count == 0 ? null : enemies[new Random().Next(enemies.Count)];
        }

        private void OnCharacterDeath(object sender, CharacterDeathEventArgs args)
        {
            if (sender is Character character && worldCharacters.Contains(character))
            {
                character.OnCharacterDeath -= OnCharacterDeath;
                worldCharacters.Remove(character);
            }
        }

        public bool IsGeneralOnWarField(int layer, string generalName)
        {
            return worldCharacters.Any(c => c.gameObject.layer == layer && c.CharacterInfo.name.Equals(generalName));
        }
        
        #region AI training

        private List<Character> GetCharacters(List<Droppable> listArea, int layer)
        {
            var characters = worldCharacters.FindAll(c => c.gameObject.layer == layer);
            var charactersOnArea = characters.FindAll(c =>
                listArea.Contains(c.Position.GetComponent<Droppable>()));

            return charactersOnArea;
        }
        
        public List<Character> GetCharactersInArea(TileData origin, int radius, int layerFilter, out List<Droppable> listArea)
        {
            listArea = new List<Droppable>();
            if (origin.TryGetComponent(out Droppable droppable))
            {
                listArea.Add(droppable);
            }

            if (radius == 0)
            {
                return GetCharacters(listArea, layerFilter);
            }
            
            var listTiles = new List<TileData>();
            var listTilePos = new TilePosition[6];
            
            for (int i = 0; i < 6; i++)
            {
                var neighbor = origin.GetNeighbor(i);

                if (neighbor is null)
                {
                    var signal = origin.TilePosition.GetPositionFromDirection(i, out float x, out float z);
                        
                    if (signal)
                    {
                        listTilePos[i] = new ()
                        {
                            X = x,
                            Z = z
                        };
                    }
                }
                else
                {
                    listTiles.Add(neighbor);
                    listTilePos[i] = neighbor.TilePosition;
                }
            }

            foreach (var tileData in listTiles)
            {
                if (tileData.TryGetComponent(out droppable))
                {
                    listArea.Add(droppable);
                }
            }

            if (radius == 1)
            {
                return GetCharacters(listArea, layerFilter);
            }
            var currentRadius = 2;

            while (currentRadius <= radius)
            {
                var newTilePos = new TilePosition[6];

                for (int i = 0; i < 6; i++)
                {
                    var signal = listTilePos[i].GetPositionFromDirection(i, out float x, out float z);

                    if (signal)
                    {
                        newTilePos[i] = new()
                        {
                            X = x,
                            Z = z
                        };
                        
                        foreach (var path in team1Paths)
                        {
                            var result = path.GetEqualsTilePositionFrom(newTilePos[i], out TileData tileData);

                            if (result && !tileData.Equals(team1StartPoint) && !tileData.Equals(team2StartPoint))
                            {
                                listArea.Add(tileData.GetComponent<Droppable>());
                            }
                        }
                    }
                }

                for (int j = 0; j < 6; j++)
                {
                    var curTilePos = newTilePos[j];
                    var direct = curTilePos.GetDirectionFromPosition(newTilePos[(j + 1) % 6]);
                    if (direct == -1) continue;
            
                    for (int i = 0; i < currentRadius - 1; i++)
                    {
                        var signal = curTilePos.GetPositionFromDirection(direct, out float x, out float z);

                        if (signal)
                        {
                            var pos = new TilePosition
                            {
                                X = x,
                                Z = z
                            };

                            foreach (var path in team1Paths)
                            {
                                var result = path.GetEqualsTilePositionFrom(pos, out TileData tileData);

                                if (result && !tileData.Equals(team1StartPoint) && !tileData.Equals(team2StartPoint))
                                {
                                    listArea.Add(tileData.GetComponent<Droppable>());
                                }
                            }
                            
                            curTilePos = pos;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                currentRadius++;
                listTilePos = newTilePos;
            }

            return GetCharacters(listArea, layerFilter);
        }

        /// <summary>
        /// ???? The logic of function was wrong! But AI still archive what I want then it will be keep.
        /// </summary>
        /// <param name="roadIndex"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        public bool IsGoodWhenPlaceCharacterOn(int roadIndex, int team)
        {
            var teamStr = team == 0 ? "Team1" : "Team2";
            var allyLayer = GetEnemyLayer(teamStr == "Team1" ? "Team2" : "Team1");
            var enemyLayer = GetEnemyLayer(teamStr);
            var difference = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < roadAmount; i++)
            {
                if (roadIndex == roadAmount) continue;

                var allies = worldCharacters.FindAll(c => c.gameObject.layer == allyLayer); // allies on road?
                var enemies = worldCharacters.FindAll(c => c.gameObject.layer == enemyLayer); // enemies on road?

                if (enemies.Count - allies.Count >= difference)
                {
                    return false;
                }
            }

            return true;
        }
        
        #endregion
    }
}