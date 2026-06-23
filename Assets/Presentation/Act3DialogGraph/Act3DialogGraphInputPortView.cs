using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act3DialogGraph
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class Act3DialogGraphInputPortView : MonoBehaviour, IDropHandler
    {
        private Act3DialogGraphStaticPresenter presenter;
        private RectTransform rectTransform;
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

        public void Initialize(Act3DialogGraphStaticPresenter presenter, string nodeId)
        {
            this.presenter = presenter;
            this.nodeId = nodeId ?? string.Empty;
            rectTransform = GetComponent<RectTransform>();
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
    }
}
