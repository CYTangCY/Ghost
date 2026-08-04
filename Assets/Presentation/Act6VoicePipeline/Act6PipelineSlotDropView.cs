using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act6VoicePipeline
{
    public sealed class Act6PipelineSlotDropView : MonoBehaviour,
        IDropHandler,
        IPointerClickHandler
    {
        private IAct6PipelineInteractionHost host;
        private int mainSlotIndex = -1;
        private bool isBackendSlot;

        public void ConfigureMain(
            IAct6PipelineInteractionHost interactionHost,
            int slotIndex)
        {
            host = interactionHost;
            mainSlotIndex = slotIndex;
            isBackendSlot = false;
        }

        public void ConfigureBackend(IAct6PipelineInteractionHost interactionHost)
        {
            host = interactionHost;
            mainSlotIndex = -1;
            isBackendSlot = true;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null || eventData.pointerDrag == null)
            {
                return;
            }

            var part = eventData.pointerDrag.GetComponentInParent<Act6PipelinePartDragView>();
            if (part == null)
            {
                return;
            }

            Act6PipelinePartDragView.ClearActivePreviews();
            if (isBackendSlot)
            {
                host?.DropComponentOnBackendSlot(part.ComponentId);
                return;
            }

            host?.DropComponentOnMainSlot(part.ComponentId, mainSlotIndex);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isBackendSlot)
            {
                host?.PlaceSelectedOnBackendSlot();
                return;
            }

            host?.PlaceSelectedOnMainSlot(mainSlotIndex);
        }
    }
}
