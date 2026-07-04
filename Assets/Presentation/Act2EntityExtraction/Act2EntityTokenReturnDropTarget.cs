using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntityTokenReturnDropTarget : MonoBehaviour, IDropHandler
    {
        private Action<string> tokenReturned;

        public void Configure(Action<string> onTokenReturned)
        {
            tokenReturned = onTokenReturned;
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
            tokenReturned?.Invoke(token.ChipKey);
        }
    }
}
