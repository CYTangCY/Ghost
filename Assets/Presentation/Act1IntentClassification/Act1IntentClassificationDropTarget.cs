using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentClassificationDropTarget : MonoBehaviour, IDropHandler
    {
        private string intentId;
        private Action<string, string> cardDroppedOnIntent;
        private Action<string> cardDroppedOnUnassigned;

        public void InitializeIntentGroup(string intentId, Action<string, string> cardDroppedOnIntent)
        {
            this.intentId = intentId;
            this.cardDroppedOnIntent = cardDroppedOnIntent;
            cardDroppedOnUnassigned = null;
        }

        public void InitializeUnassigned(Action<string> cardDroppedOnUnassigned)
        {
            intentId = null;
            cardDroppedOnIntent = null;
            this.cardDroppedOnUnassigned = cardDroppedOnUnassigned;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            var draggedObject = eventData.pointerDrag;
            if (draggedObject == null)
            {
                return;
            }

            var draggedCard = draggedObject.GetComponent<Act1IntentClassificationDraggableCard>();
            if (draggedCard == null || string.IsNullOrEmpty(draggedCard.CardId))
            {
                return;
            }

            var cardId = draggedCard.CardId;
            draggedCard.CompleteDragVisuals();

            if (!string.IsNullOrEmpty(intentId))
            {
                cardDroppedOnIntent?.Invoke(cardId, intentId);
            }
            else
            {
                cardDroppedOnUnassigned?.Invoke(cardId);
            }

            eventData.Use();
        }
    }
}
