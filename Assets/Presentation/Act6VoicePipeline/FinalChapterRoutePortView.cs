using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act6VoicePipeline
{
    /// <summary>
    /// One dot on a route card: the one underneath it that a wire leaves from, or the one on top that
    /// a wire arrives at. Chapter 3 splits these into two components; here one component carries a
    /// flag, because the Final Chapter's cards are laid out by a layout group rather than dragged
    /// around a canvas, and the two dots differ only in which way they face.
    ///
    /// A wire can be drawn from either end - starting at the card you want to arrive at is a natural
    /// thing to try, and refusing it would read as a bug rather than a rule.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class FinalChapterRoutePortView : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IPointerClickHandler
    {
        private IFinalChapterWireHost host;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private string stepId;
        private bool isOutput;

        public string StepId => stepId;

        public bool IsOutput => isOutput;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        public void Initialize(IFinalChapterWireHost wireHost, string routeStepId, bool output)
        {
            host = wireHost;
            stepId = routeStepId ?? string.Empty;
            isOutput = output;
            rectTransform = GetComponent<RectTransform>();
            EnsureCanvasGroup();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnsureCanvasGroup();

            // Otherwise this dot swallows the drop meant for whatever is underneath the pointer.
            canvasGroup.blocksRaycasts = false;
            host?.BeginWireDrag(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            host?.UpdateWireDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }

            host?.EndWireDrag();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null || eventData.pointerDrag == null)
            {
                return;
            }

            var other = eventData.pointerDrag.GetComponent<FinalChapterRoutePortView>();
            if (other == null || other.IsOutput == isOutput)
            {
                return;
            }

            // Normalise the pair so the host only ever sees source-then-destination.
            host?.CompleteWireDrop(other.IsOutput ? other : this, other.IsOutput ? this : other);
            eventData.Use();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // A drag ends with a click too; only treat this as "take the wire down" if nothing moved.
            if (eventData != null && eventData.dragging)
            {
                return;
            }

            if (isOutput)
            {
                host?.ClearWireFrom(this);
            }
        }

        private void OnDisable()
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }
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
