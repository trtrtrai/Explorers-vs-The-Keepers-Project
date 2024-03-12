using System;
using TMPro;
using UnityEngine;

namespace GUI
{
    public class StoryWriter : MonoBehaviour
    {
        [SerializeField] private TMP_Text speechTxt;
        private string speechContent;
        [SerializeField] private float timePerChar;
        [SerializeField] private float timer;
        [SerializeField] private int charIndex;
        [SerializeField] private int maxChar;

        public bool IsCompleted = true;

        public void SetupWriter(string content)
        {
            speechContent = content;
            maxChar = speechContent.Length;
            charIndex = 0;
            timer = 0f;
            
            IsCompleted = false;
        }

        private void Update()
        {
            if (charIndex < maxChar)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    timer = timePerChar;
                    charIndex++;
                    speechTxt.text = speechContent[..charIndex];
                }
            }

            if (charIndex != maxChar) return;
            maxChar = 0;
            IsCompleted = true;
        }

        public void SkipWriter()
        {
            charIndex = maxChar;

            speechTxt.text = speechContent;
            IsCompleted = true;
        }
    }
}