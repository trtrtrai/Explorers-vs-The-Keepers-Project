using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using EventArgs;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject cardInfoUIPrefab;
        public GameObject cardTagContentPrefab;
        public GameObject cardTagPrefab;

        [SerializeField] private CardTagTooltip tagTooltip;
        
        [SerializeField] private List<TagExplain> listTagExplain;

        private void Start()
        {
            StartCoroutine(WaitToSetup());
        }

        private IEnumerator WaitToSetup()
        {
            while (WorldManager.Instance is null || CardController.Instance is null)
            {
                yield return null;
            }

            WorldManager.Instance.OnGameReset += OnGameReset;

            if (WorldManager.Instance.Team1Player)
            {
                CardController.Instance.OnTeam1DrawCard += OnPlayerDrawNewCard;
            }
            else
            {
                CardController.Instance.OnTeam2DrawCard += OnPlayerDrawNewCard;
            }
        }

        private void OnGameReset(object sender, System.EventArgs args)
        {
            if (sender is WorldManager)
            {
                foreach (Transform t in transform)
                {
                    Destroy(t.gameObject);
                }
            }
        }

        private void OnPlayerDrawNewCard(object sender, DrawNewCardEventArgs args)
        {
            if (sender is CardController)
            {
                var cardObj = Instantiate(cardInfoUIPrefab, transform);
                var script = cardObj.GetComponent<Card>();
                script.SetupCard(args.Card, args.HandIndex);
            }
        }

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