using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act1IntentClassification
{
    public sealed class Act1IntentTeachingDropTarget : MonoBehaviour, IDropHandler
    {
        private string pileId;
        private Action<string> cardDroppedOnNewPile;
        private Action<string> cardDroppedOnUnassigned;
        private Action<string, string> cardDroppedOnPile;
        private Action<string, string> labelDroppedOnPile;

        public void InitializeNewPile(Action<string> cardDroppedOnNewPile)
        {
            pileId = null;
            this.cardDroppedOnNewPile = cardDroppedOnNewPile;
            cardDroppedOnUnassigned = null;
            cardDroppedOnPile = null;
            labelDroppedOnPile = null;
        }

        public void InitializeUnassigned(Action<string> cardDroppedOnUnassigned)
        {
            pileId = null;
            cardDroppedOnNewPile = null;
            this.cardDroppedOnUnassigned = cardDroppedOnUnassigned;
            cardDroppedOnPile = null;
            labelDroppedOnPile = null;
        }

        public void InitializePile(
            string pileId,
            Action<string, string> cardDroppedOnPile,
            Action<string, string> labelDroppedOnPile)
        {
            this.pileId = pileId;
            cardDroppedOnNewPile = null;
            cardDroppedOnUnassigned = null;
            this.cardDroppedOnPile = cardDroppedOnPile;
            this.labelDroppedOnPile = labelDroppedOnPile;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null || eventData.pointerDrag == null)
            {
                return;
            }

            var card = eventData.pointerDrag.GetComponent<Act1IntentClassificationDraggableCard>();
            if (card != null && !string.IsNullOrEmpty(card.CardId))
            {
                var cardId = card.CardId;
                card.CompleteDragVisuals();
                DropCard(cardId);
                eventData.Use();
                return;
            }

            var label = eventData.pointerDrag.GetComponent<Act1IntentClassificationLabelDragView>();
            if (label != null && !string.IsNullOrEmpty(label.IntentId))
            {
                var intentId = label.IntentId;
                label.CompleteDragVisuals();
                DropLabel(intentId);
                eventData.Use();
            }
        }

        private void DropCard(string cardId)
        {
            if (!string.IsNullOrEmpty(pileId))
            {
                cardDroppedOnPile?.Invoke(cardId, pileId);
                return;
            }

            if (cardDroppedOnNewPile != null)
            {
                cardDroppedOnNewPile.Invoke(cardId);
                return;
            }

            cardDroppedOnUnassigned?.Invoke(cardId);
        }

        private void DropLabel(string intentId)
        {
            if (!string.IsNullOrEmpty(pileId))
            {
                labelDroppedOnPile?.Invoke(intentId, pileId);
            }
        }
    }
}
