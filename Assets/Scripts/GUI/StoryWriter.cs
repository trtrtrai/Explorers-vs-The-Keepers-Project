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
            IsCompleted = false;
            
            speechContent = content;
            maxChar = speechContent.Length;
            charIndex = 0;
            timer = 0f;
        }

        private void Update()
        {
            if (IsCompleted) return;
            
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
            else
            {
                IsCompleted = true;
            }
        }

        public void SkipWriter()
        {
            IsCompleted = true;
            charIndex = maxChar;
            speechTxt.text = speechContent;
        }
    }
}