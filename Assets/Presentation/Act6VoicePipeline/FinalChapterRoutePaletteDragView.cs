using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act6VoicePipeline
{
    /// <summary>
    /// A step sitting in the palette, waiting to be dragged onto the board. Chapter 3's palette works
    /// the same way: the card only becomes part of the conversation once the player puts it somewhere.
    ///
    /// It fades while held so the board underneath stays readable, and hands the drop to the presenter
    /// on release - if the pointer never reached the board, nothing is placed and the card is simply
    /// still in the palette.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class FinalChapterRoutePaletteDragView : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private IFinalChapterWireHost host;
        private CanvasGroup canvasGroup;
        private string stepId;

        public string StepId => stepId;

        public void Initialize(IFinalChapterWireHost wireHost, string routeStepId)
        {
            host = wireHost;
            stepId = routeStepId ?? string.Empty;
            EnsureCanvasGroup();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnsureCanvasGroup();
            canvasGroup.alpha = 0.65f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Nothing to move - the palette card stays put and the drop is what counts. The handler
            // still has to exist, or Unity never raises OnEndDrag.
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Restore();
            host?.TryPlaceStepAtPointer(stepId, eventData);
        }

        private void OnDisable()
        {
            Restore();
        }

        private void Restore()
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        private void EnsureCanvasGroup()
        {
            if (canvasGroup != null)
            {
                return;
            }

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
}
