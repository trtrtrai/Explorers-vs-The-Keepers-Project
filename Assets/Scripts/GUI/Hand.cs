using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject cardTagContentPrefab;
        public GameObject cardTagPrefab;

        [SerializeField] private CardTagTooltip tagTooltip;
        
        [SerializeField] private List<TagExplain> listTagExplain;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag is null) return;
            
            if (eventData.pointerDrag.TryGetComponent(out Draggable draggable) && draggable.Placeholder is not null)
            {
                draggable.DraggableObjOutOfHand = false;
                draggable.Placeholder.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag is null) return;
            
            if (eventData.pointerDrag.TryGetComponent(out Draggable draggable))
            {
                if (draggable.PlaceHolderDestroy) return;

                draggable.DraggableObjOutOfHand = true;
                draggable.Placeholder.SetActive(false);
            }
        }

        public void ActiveTooltip(bool isCharacterTag, string tags, int clickCount)
        {
            if ((!tagTooltip.IsActive && clickCount == 2) || (tagTooltip.IsActive && clickCount == 1))
            {
                var tagExplain = listTagExplain.FirstOrDefault(e =>
                    e.IsCharacterTag == isCharacterTag && GetEnumString(e, e.IsCharacterTag).Equals(tags));

                if (tagExplain is null) return;
                tagTooltip.Setup(tagExplain.Explain);
            }
        }

        private string GetEnumString(TagExplain target, bool isCharacterTag)
        {
            return isCharacterTag ? target.CharacterTag.ToString() : target.SpellsTag.ToString();
        }

        public TagExplain GetTagExplain(bool isCharacterTag, string tags)
        {
            return listTagExplain.FirstOrDefault(e =>
                e.IsCharacterTag == isCharacterTag && GetEnumString(e, e.IsCharacterTag).Equals(tags));
        }

        public void DisableTooltip()
        {
            tagTooltip.Reset();
        }
    }
}