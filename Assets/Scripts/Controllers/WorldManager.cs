using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventArgs;
using Extensions;
using GUI;
using Models;
using Models.Spells;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;

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

        [SerializeField] private GameObject energyManagerPref;
        [SerializeField] private GameObject cardControllerPref;

        [SerializeField] private EnergyManager playerEnergy;
        [SerializeField] private bool team1Player;
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

        [SerializeField] private int characterTagAmount;
        private List<Character> worldCharacters;

        public List<CharacterTagPriority> Priorities;
        private BitArray prioritiesBitArr;

        private List<List<TileData>> team1Paths;
        private List<List<TileData>> team2Paths; // reverse from team 1?

        private int Counting; //test

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

                Instantiate(cardControllerPref, transform);
                
                var obj = Instantiate(energyManagerPref, transform); // setup layer after
                playerEnergy = obj.GetComponent<EnergyManager>();
                playerEnergy.SetupAndStart();
            }
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
                        
                        if (TryConsumeCard(card, playerEnergy))
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

                    if (TryConsumeCard(card, playerEnergy))
                    {
                        CreateCharacter(prefab, dropPosition.RoadIndex, team1Player ? 0 : 1);
                        
                        //test enemy spawn
                        if (Counting % 2 == 0)
                        {
                            CreateCharacter(prefab, dropPosition.RoadIndex,
                                team1Player ? 1 : 0);
                        }

                        Counting++;
                    }
                    break;
                }
                case CardType.Spells when card.ActiveType is CardActiveType.Single or CardActiveType.SingleEnemy:
                    if (characterTarget is null) break;
                    
                    if (TryConsumeCard(card, playerEnergy))
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
                            
                            if (TryConsumeCard(card, playerEnergy))
                            {
                                environmentSpells.ListSettingUp = listActive.ConvertAll(d => d.GetComponent<TileData>());
                                SpellsExecute.Activate(null, card.SpellsEffect);
                            }
                            break;
                        case EnergyBoostSpells energyBoostSpells:
                            if (TryConsumeCard(card, playerEnergy))
                            {
                                energyBoostSpells.Target = playerEnergy; // because this method only use for player, not bot
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
                            
                            if (TryConsumeCard(card, playerEnergy))
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
                        
                        if (TryConsumeCard(card, playerEnergy))
                        {
                            foreach (var character in characters)
                            {
                                SpellsExecute.Activate(character, card.SpellsEffect);
                            }
                        }
                    }
                    else // another effect except character
                    {
                        if (TryConsumeCard(card, playerEnergy)) SpellsExecute.Activate(null, card.SpellsEffect);
                    }
                    break;
                }
                case CardType.Spells when card.ActiveType == CardActiveType.Road:
                {
                    var spellsEff = card.SpellsEffect;

                    if (spellsEff is SummonSpells summonSpells)
                    {
                        if (dropPosition is null) break;
                        
                        if (TryConsumeCard(card, playerEnergy))
                        {
                            summonSpells.RoadIndex = dropPosition.RoadIndex;
                            SpellsExecute.Activate(null, spellsEff);
                        }
                    }
                    break;
                }
                case CardType.Spells when card.ActiveType == CardActiveType.World:
                {
                    // Do not check character count, always use if it can (Mechanics)
                    //if (worldCharacters.Count == 0) break;
                    
                    if (TryConsumeCard(card, playerEnergy))
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
        public static bool TryConsumeCard(Card card, EnergyManager energyManager) =>
            CardController.Instance.CardConsuming(card, energyManager);
        
        private void CreateCharacter(GameObject prefab, int roadIndex, int team) // After, it will also be assigned in event of AI script
        {
            var characterObj = Instantiate(prefab);
            var character = characterObj.GetComponent<Character>();
            var startPoint = team == 0 ? team1StartPoint : team2StartPoint;
            character.Position = startPoint;
            characterObj.transform.localPosition = startPoint.transform.localPosition;
            character.Setup(roadIndex, team);
            
            worldCharacters.Add(character);
            character.OnCharacterDeath += OnCharacterDeath;
            CharacterSpawn?.Invoke(this, new CharacterSpawnEventArgs(character, team));
        }

        public void CreateCharacter(GameObject prefab, int roadIndex, int team, SpellsEffect a) // After, it will also be assigned in event of AI script
        {
            CreateCharacter(prefab, roadIndex, team);
        }

        private void OnCharacterDeath(object sender, CharacterDeathEventArgs args)
        {
            if (sender is Character character && worldCharacters.Contains(character))
            {
                character.OnCharacterDeath -= OnCharacterDeath;
                worldCharacters.Remove(character);
            }
        }
    }
}