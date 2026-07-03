using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ghost.Presentation.Act1IntentClassification
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class Act1IntentClassificationLabelDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const float DraggedSourceAlpha = 0.35f;
        private const float DragPreviewAlpha = 1f;

        private string intentId;
        private Canvas rootCanvas;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private RectTransform dragPreview;

        public string IntentId => intentId;

        public void Initialize(string intentId, Canvas rootCanvas)
        {
            this.intentId = intentId;
            this.rootCanvas = rootCanvas;
            EnsureComponents();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(intentId))
            {
                return;
            }

            EnsureComponents();
            if (rootCanvas == null)
            {
                rootCanvas = GetComponentInParent<Canvas>();
            }

            if (rootCanvas == null)
            {
                return;
            }

            CreateDragPreview();
            canvasGroup.alpha = DraggedSourceAlpha;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragPreview == null)
            {
                return;
            }

            var scaleFactor = rootCanvas == null || rootCanvas.scaleFactor <= 0f
                ? 1f
                : rootCanvas.scaleFactor;
            dragPreview.anchoredPosition += eventData.delta / scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            CompleteDragVisuals();
        }

        private void OnDisable()
        {
            CompleteDragVisuals();
        }

        public void CompleteDragVisuals()
        {
            if (dragPreview != null)
            {
                var previewObject = dragPreview.gameObject;
                dragPreview = null;
                previewObject.SetActive(false);

                if (Application.isPlaying)
                {
                    Destroy(previewObject);
                }
                else
                {
                    DestroyImmediate(previewObject);
                }
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
        }

        private void EnsureComponents()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        private void CreateDragPreview()
        {
            CompleteDragVisuals();

            var previewObject = Instantiate(gameObject, rootCanvas.transform);
            previewObject.name = gameObject.name + " Drag Preview";
            previewObject.transform.SetAsLastSibling();

            foreach (var dragView in previewObject.GetComponentsInChildren<Act1IntentClassificationLabelDragView>())
            {
                dragView.enabled = false;
            }

            foreach (var button in previewObject.GetComponentsInChildren<Button>())
            {
                button.enabled = false;
            }

            var previewGroup = previewObject.GetComponent<CanvasGroup>();
            if (previewGroup == null)
            {
                previewGroup = previewObject.AddComponent<CanvasGroup>();
            }

            previewGroup.alpha = DragPreviewAlpha;
            previewGroup.blocksRaycasts = false;
            previewGroup.interactable = false;

            dragPreview = previewObject.GetComponent<RectTransform>();
            dragPreview.anchorMin = new Vector2(0.5f, 0.5f);
            dragPreview.anchorMax = new Vector2(0.5f, 0.5f);
            dragPreview.pivot = rectTransform.pivot;
            dragPreview.sizeDelta = rectTransform.rect.size;
            dragPreview.position = rectTransform.position;
        }
    }
}
