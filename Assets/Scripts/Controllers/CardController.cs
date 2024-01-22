using System;
using System.Collections.Generic;
using Models;
using Models.Generals;
using UnityEngine;

namespace Controllers
{
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
        
        private const string Assembly = "Models.Generals.";
        
        //[SerializeField] private GameObject generalCheckSummonPrefab;
        [SerializeField] private List<GeneralCheckCanSummon> team1Required;
        [SerializeField] private List<GeneralCheckCanSummon> team2Required;

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

        private void Start()
        {
            team1Required = new();
            team2Required = new();
            
            CreateChecker("Commander");
            CreateChecker("TheForest");
            CreateChecker("TheRock");
            CreateChecker("SuperMiner");
        }

        private void CreateChecker(string generalName)
        {
            var obj = new GameObject();
            obj.transform.SetParent(transform);
            var generalType = Type.GetType(Assembly + generalName);
            var script = obj.AddComponent(generalType) as GeneralCheckCanSummon;
            
            // temporary - test
            script.Team = WorldManager.Instance.GetAllyTeam();
            team2Required.Add(script);
            
            // obj.name = general name + team index
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
            // List card of both 2 player must be required manage by this after
            

            if (target.UseCard(card))
            {
                //Debug.Log(card.name + " activated.");
                Destroy(card.gameObject); // remove in list after
                return true;
            }

            return false;
        }
    }
}