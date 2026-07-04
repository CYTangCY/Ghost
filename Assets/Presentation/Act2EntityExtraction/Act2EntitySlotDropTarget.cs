using System;
using Ghost.Puzzles.EntityExtraction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntitySlotDropTarget : MonoBehaviour, IDropHandler
    {
        private Act2ErrandSlotId slotId;
        private Action<Act2ErrandSlotId, string> tokenDropped;

        public void Configure(Act2ErrandSlotId configuredSlotId, Action<Act2ErrandSlotId, string> onTokenDropped)
        {
            slotId = configuredSlotId;
            tokenDropped = onTokenDropped;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null || eventData.pointerDrag == null)
            {
                return;
            }

            var token = eventData.pointerDrag.GetComponent<Act2EntityTokenDragView>();
            if (token == null)
            {
                return;
            }

            Act2EntityTokenDragView.ClearActivePreviews();
            tokenDropped?.Invoke(slotId, token.ChipKey);
        }
    }
}
