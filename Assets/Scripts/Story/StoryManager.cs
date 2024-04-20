using System;
using System.Collections.Generic;
using Data;
using Extensions;
using GUI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Story
{
    public class StoryManager : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image talker;
        [SerializeField] private TMP_Text labelName;
        [SerializeField] private StoryWriter speechText;

        [SerializeField] private List<Speech> current;
        [SerializeField] private int speechIndex;
        [SerializeField] private int speechCurrentIndex;
        [SerializeField] private int speechTextIndex;
        [SerializeField] private int speechCurrentTextIndex;

        private Action callbackAction;
        private Action updateProcessCallbackAction;

        public void PlayCutScene(int index, Action callback, Action updateStoryProcessCallback)
        {
            Time.timeScale = 0;
            callbackAction = callback;
            updateProcessCallbackAction = updateStoryProcessCallback;
            
            current = GetCutScene(index).speeches;
            speechIndex = current.Count - 1;
            speechCurrentIndex = 0;
            speechTextIndex = current[0].speechTexts.Count - 1;
            speechCurrentTextIndex = 0;

            StartSpeech();
        }

        private void StartSpeech()
        {
            var speech = current[speechCurrentIndex];
            if (speechCurrentTextIndex == 0)
            {
                talker.sprite = Resources.Load<Sprite>(speech.talkerSpriteName);
                labelName.text = GUIExtension.SpaceBetweenWord(speech.talker.ToString());
            }
            speechText.SetupWriter(speech.speechTexts[speechCurrentTextIndex]);
        }
        
        private CutScene GetCutScene(int index)
        {
            return DataManager.Story.CutScenes[index];
        }

        /*private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (speechCurrentTextIndex == speechTextIndex && speechCurrentIndex < speechIndex)
                {
                    speechCurrentIndex++;
                    speechTextIndex = current[speechCurrentIndex].speechTexts.Count - 1;
                    speechCurrentTextIndex = 0;
                
                    if (speechText.IsCompleted) StartSpeech();
                    else speechText.SkipWriter();
                }
                else if (speechCurrentTextIndex == speechTextIndex && speechCurrentIndex == speechIndex)
                {
                    Time.timeScale = 1;
                    updateProcessCallbackAction?.Invoke();
                    callbackAction?.Invoke();
                    Destroy(gameObject);
                }
                else if (speechCurrentTextIndex < speechTextIndex)
                {
                    speechCurrentTextIndex++;
                
                    if (speechText.IsCompleted) StartSpeech();
                    else speechText.SkipWriter();
                }
            }
        }*/

        public void SkipCutScene()
        {
            Time.timeScale = 1;
            updateProcessCallbackAction?.Invoke();
            callbackAction?.Invoke();
            Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
            {
                if (speechCurrentTextIndex == speechTextIndex && speechCurrentIndex < speechIndex)
                {
                    if (speechText.IsCompleted)
                    {
                        speechCurrentIndex++;
                        speechTextIndex = current[speechCurrentIndex].speechTexts.Count - 1;
                        speechCurrentTextIndex = 0;
                        
                        StartSpeech();
                    }
                    else speechText.SkipWriter();
                }
                else if (speechCurrentTextIndex == speechTextIndex && speechCurrentIndex == speechIndex)
                {
                    if (speechText.IsCompleted)
                    {
                        Time.timeScale = 1;
                        updateProcessCallbackAction?.Invoke();
                        callbackAction?.Invoke();
                        Destroy(gameObject);
                    }
                    else speechText.SkipWriter();
                }
                else if (speechCurrentTextIndex < speechTextIndex)
                {
                    if (speechText.IsCompleted)
                    {
                        speechCurrentTextIndex++;
                        
                        StartSpeech();
                    }
                    else speechText.SkipWriter();
                }
            }
        }
    }
}