using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI
{
    public class ScrollViewPlanetMap : MonoBehaviour
    {
        [SerializeField] private TMP_Text missionLvlLabel;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private int levelAmount;
        [SerializeField] private int curLevel;
        [SerializeField] private int newLevel;
        [SerializeField] private float ratioPerLevel;
        [SerializeField] private float timer;
        [SerializeField] private bool stopDetect;
        [SerializeField] private float contentOffset;

        private void Awake()
        {
            stopDetect = false;
            curLevel = 0;
            newLevel = curLevel;
            missionLvlLabel.text = $"mission {curLevel + 1}";
            levelAmount = scroll.content.childCount;
            ratioPerLevel = 1f / levelAmount;
            contentOffset = 0.00758f;
            //contentOffset = /*((scroll.transform as RectTransform).rect.width - ratioPerLevel * scroll.content.sizeDelta.x)*/66.67f / scroll.content.sizeDelta.x;
        }

        public void Change(Vector2 v)
        {
            //Debug.Log(v);

            if (stopDetect)
            {
                stopDetect = false;
                return;
            }
            
            StopAllCoroutines();
            
            newLevel = Mathf.Clamp(Mathf.RoundToInt(v.x / ratioPerLevel), 0, levelAmount - 1);
            StartCoroutine(Sleep());
        }

        private IEnumerator Sleep()
        {
            timer = .5f;

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            stopDetect = true;
            //Debug.Log("Change transform " + content.sizeDelta.x * curLevel * ratioPerLevel);
            scroll.horizontalNormalizedPosition = (ratioPerLevel + contentOffset) * newLevel;
            curLevel = newLevel;
            missionLvlLabel.text = $"mission {curLevel + 1}";
        }
    }
}