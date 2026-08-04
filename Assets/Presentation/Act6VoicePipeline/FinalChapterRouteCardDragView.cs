using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act6VoicePipeline
{
    /// <summary>
    /// A card already on the board. Moving it is how the player untangles a route they can no longer
    /// read, so it follows the pointer directly rather than waiting for a re-render.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class FinalChapterRouteCardDragView : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private IFinalChapterWireHost host;
        private RectTransform rectTransform;
        private string stepId;

        public string StepId => stepId;

        public void Initialize(IFinalChapterWireHost wireHost, string routeStepId)
        {
            host = wireHost;
            stepId = routeStepId ?? string.Empty;
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            host?.MoveStepToPointer(stepId, RectTransform, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            host?.MoveStepToPointer(stepId, RectTransform, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            host?.CompleteStepDrag(stepId, RectTransform, eventData);
        }

        private RectTransform RectTransform
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
    }
}
