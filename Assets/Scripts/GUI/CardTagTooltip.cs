using TMPro;
using UnityEngine;

namespace GUI
{
    public class CardTagTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform canvas;
        [SerializeField] private RectTransform bg;
        [SerializeField] private RectTransform self;
        [SerializeField] private TextMeshProUGUI tooltipTxt;

        [SerializeField] private CanvasGroup selfGroup;

        [SerializeField] private bool isActive;

        public bool IsActive => isActive;
        
        public void Setup(string tooltip)
        {
            tooltipTxt.SetText(tooltip);
            tooltipTxt.ForceMeshUpdate();

            var textSize = tooltipTxt.GetRenderedValues(false);
            var paddingSize = new Vector2(8, 8);

            bg.sizeDelta = textSize + paddingSize;
            selfGroup.alpha = 1f;
            self.anchoredPosition = Input.mousePosition / canvas.localScale.x;

            isActive = true;
        }

        public void Reset()
        {
            if (!isActive) return;
            
            selfGroup.alpha = 0f;
            self.anchoredPosition = Vector2.zero;

            isActive = false;
        }
    }
}