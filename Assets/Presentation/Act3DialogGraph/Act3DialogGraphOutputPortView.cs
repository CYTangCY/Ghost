using Ghost.Puzzles.DialogGraph;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Act3DialogGraph
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class Act3DialogGraphOutputPortView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Act3DialogGraphStaticPresenter presenter;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private string nodeId;
        private DialogTransitionCondition condition;

        public string NodeId => nodeId;

        public DialogTransitionCondition Condition => condition;

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

        public void Initialize(
            Act3DialogGraphStaticPresenter presenter,
            string nodeId,
            DialogTransitionCondition condition)
        {
            this.presenter = presenter;
            this.nodeId = nodeId ?? string.Empty;
            this.condition = condition;
            rectTransform = GetComponent<RectTransform>();
            EnsureCanvasGroup();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnsureCanvasGroup();
            canvasGroup.blocksRaycasts = false;
            presenter?.BeginWireDrag(this, eventData);
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

            presenter?.EndWireDrag(this);
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
