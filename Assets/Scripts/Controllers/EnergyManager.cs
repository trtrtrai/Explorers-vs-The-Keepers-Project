using System.Collections;
using Models;
using ScriptableObjects;
using UnityEngine;

namespace Controllers
{
    public class EnergyManager : MonoBehaviour
    {
        // I don't want to manage both 2 player in same script anymore...
        //public static EnergyManager Instance { get; private set; }

        public delegate int CardUsing(Card card);
        public event CardUsing CardUsingEvent;
        
        [SerializeField] private int energy;
        [SerializeField] private int maximumEnergy;
        [SerializeField] private float timePerEnergy;
        [SerializeField] private float timePerEnergyInRealTime;
        [SerializeField] private float timer;
        [SerializeField] private float timerScale;
        
        [SerializeField] private bool isSetup;

        public int Energy => energy;
        public int MaximumEnergy => maximumEnergy;
        public float TimePerEnergy => timePerEnergy;
        public float TimePerEnergyInRealTime => timePerEnergyInRealTime;
        public float Timer => timer;

        public float TimerScale => timerScale;

        public bool IsSetup => isSetup;

        public void SetupAndStart()
        {
            if (isSetup) return;
            
            isSetup = true;

            WorldManager.Instance.OnGameReset += OnGameReset;
            timePerEnergyInRealTime = timePerEnergy;
            timerScale = 1f;
            StartCoroutine(EnergyCooldown());
        }
        
        private void OnGameReset(object sender, System.EventArgs args)
        {
            if (sender is WorldManager)
            {
                StopAllCoroutines();
                
                energy = maximumEnergy / 2;
                timePerEnergyInRealTime = timePerEnergy;
                timerScale = 1f;
                StartCoroutine(EnergyCooldown());
            }
        }

        private IEnumerator EnergyCooldown()
        {
            timer = timePerEnergyInRealTime;

            while (timer > 0f)
            {
                timer -= Time.deltaTime * timerScale;

                yield return null;
            }

            energy++;
            if (energy < maximumEnergy)
            {
                StartCoroutine(EnergyCooldown());
            }
        }

        public bool UseCard(Card target)
        {
            var realCost = CardUsingEvent?.Invoke(target);

            if (realCost is null || realCost == -1)
            {
                realCost = target.Cost;
            }

            if (energy >= realCost)
            {
                if (energy == MaximumEnergy) StartCoroutine(EnergyCooldown());
                energy -= (int)realCost;
                
                return true;
            }

            return false;
        }
        
        public bool CardCanBeUsed(CardInfo target)
        {
            //var realCost = CardUsingEvent?.Invoke(target);
            var realCost = target.Cost;

            return energy >= realCost;
        }

        public bool UseCard(CardInfo target)
        {
            //var realCost = CardUsingEvent?.Invoke(target);
            var realCost = target.Cost;

            if (energy >= realCost)
            {
                if (energy == MaximumEnergy) StartCoroutine(EnergyCooldown());
                energy -= realCost;
                
                return true;
            }

            return false;
        }

        /*public void AddingTimerScale(float addingScale)
        {
            timerScale += addingScale;
        }
        
        public void RemoveTimerScale(float addingScale)
        {
            timerScale -= addingScale;
        }*/
    }
}