using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act3DialogGraph
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class Act3DialogGraphNodeDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Act3DialogGraphStaticPresenter presenter;
        private RectTransform rectTransform;
        private string nodeId;

        public void Initialize(Act3DialogGraphStaticPresenter presenter, string nodeId)
        {
            this.presenter = presenter;
            this.nodeId = nodeId ?? string.Empty;
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            presenter?.MoveNodeToPointer(nodeId, RectTransform, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            presenter?.MoveNodeToPointer(nodeId, RectTransform, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            presenter?.CompleteNodeDrag(nodeId, RectTransform, eventData);
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