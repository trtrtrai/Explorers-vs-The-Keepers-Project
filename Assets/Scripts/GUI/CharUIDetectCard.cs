using Controllers;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    public class CharUIDetectCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private CharacterTab _characterTab;
        private Character selfComp;

        public void Setup()
        {
            _characterTab = GetComponentInParent<CharacterTab>();
            
            if (_characterTab is not null && TryGetComponent(out CharInfoUI infoUI))
            {
                selfComp = _characterTab.GetCharacterFrom(infoUI);
            }

            if (selfComp is null) Destroy(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.dragging && eventData.pointerDrag.TryGetComponent(out Draggable drag))
            {
                //Debug.Log("Pointer enter");
                WorldManager.Instance.CardEnterCharacterInfoUI(selfComp);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.dragging && eventData.pointerDrag.TryGetComponent(out Draggable drag))
            {
                //Debug.Log("Pointer exit");
                WorldManager.Instance.CardExitCharacterInfoUI(selfComp);
            }
        }
    }
}