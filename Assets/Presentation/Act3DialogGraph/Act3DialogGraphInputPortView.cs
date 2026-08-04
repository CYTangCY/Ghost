using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act3DialogGraph
{
    /// <summary>
    /// The top dot on a card. It receives wires dropped from an output port, and since wiring a graph
    /// backwards is a natural thing to try, it can also start a wire of its own and be dropped onto an
    /// output port.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class Act3DialogGraphInputPortView : MonoBehaviour,
        IDropHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private IDialogGraphWireInteractionHost presenter;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private string nodeId;

        public string NodeId => nodeId;

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

        public void Initialize(IDialogGraphWireInteractionHost presenter, string nodeId)
        {
            this.presenter = presenter;
            this.nodeId = nodeId ?? string.Empty;
            rectTransform = GetComponent<RectTransform>();
            EnsureCanvasGroup();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData == null || eventData.pointerDrag == null)
            {
                return;
            }

            var outputPort = eventData.pointerDrag.GetComponent<Act3DialogGraphOutputPortView>();
            if (outputPort == null)
            {
                return;
            }

            presenter?.CompleteWireDrop(outputPort, this);
            eventData.Use();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnsureCanvasGroup();

            // Let the pointer see the ports underneath, otherwise this dot swallows its own drop.
            canvasGroup.blocksRaycasts = false;
            presenter?.BeginReverseWireDrag(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            presenter?.UpdateWireDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }

            presenter?.EndReverseWireDrag(this);
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
