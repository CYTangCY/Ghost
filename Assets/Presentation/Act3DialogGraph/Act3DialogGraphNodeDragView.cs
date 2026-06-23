using Ghost.Puzzles.DialogGraph;
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

    [RequireComponent(typeof(RectTransform))]
    public sealed class Act3DialogGraphPaletteItemDragView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        private Act3DialogGraphStaticPresenter presenter;
        private CanvasGroup canvasGroup;
        private DialogNodeType type;
        private string intentId;
        private string requiredEntityType;
        private string responseId;

        public void Initialize(
            Act3DialogGraphStaticPresenter presenter,
            DialogNodeType type,
            string intentId,
            string requiredEntityType,
            string responseId)
        {
            this.presenter = presenter;
            this.type = type;
            this.intentId = intentId;
            this.requiredEntityType = requiredEntityType;
            this.responseId = responseId;
            EnsureCanvasGroup();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnsureCanvasGroup();
            canvasGroup.alpha = 0.65f;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            presenter?.TryPlacePaletteNodeAtPointer(type, intentId, requiredEntityType, responseId, eventData);
        }

        private void OnDisable()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
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
