using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ghost.Presentation.Act1IntentClassification
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class Act1IntentClassificationDraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const float DraggedSourceAlpha = 0.3f;
        private const float DragPreviewAlpha = 1f;

        private static Act1IntentClassificationDraggableCard activeDragSource;
        private static RectTransform activeDragPreview;

        private string cardId;
        private Canvas rootCanvas;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private RectTransform dragPreview;
        private bool isDragging;

        public string CardId => cardId;

        public void Initialize(string cardId, Canvas rootCanvas)
        {
            this.cardId = cardId;
            this.rootCanvas = rootCanvas;
            EnsureComponents();

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(cardId))
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

            CancelActiveDrag();
            isDragging = true;
            activeDragSource = this;
            CreateDragPreview();
            canvasGroup.alpha = DraggedSourceAlpha;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || activeDragSource != this || dragPreview == null)
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
            if (activeDragSource == this)
            {
                CompleteDragVisuals();
            }
        }

        public void CompleteDragVisuals()
        {
            var ownsActiveDrag = activeDragSource == this;
            isDragging = false;

            if (dragPreview != null)
            {
                DestroyDragPreview();
            }
            else if (ownsActiveDrag)
            {
                DestroyActivePreview();
            }

            if (ownsActiveDrag)
            {
                activeDragSource = null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
        }

        private static void CancelActiveDrag()
        {
            if (activeDragSource != null)
            {
                activeDragSource.CompleteDragVisuals();
                return;
            }

            DestroyActivePreview();
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
            DestroyDragPreview();

            var previewObject = Instantiate(gameObject, rootCanvas.transform);
            previewObject.name = gameObject.name + " Drag Preview";
            previewObject.transform.SetAsLastSibling();

            foreach (var draggable in previewObject.GetComponentsInChildren<Act1IntentClassificationDraggableCard>())
            {
                draggable.enabled = false;
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
            activeDragPreview = dragPreview;
            dragPreview.anchorMin = new Vector2(0.5f, 0.5f);
            dragPreview.anchorMax = new Vector2(0.5f, 0.5f);
            dragPreview.pivot = rectTransform.pivot;
            dragPreview.sizeDelta = rectTransform.rect.size;
            dragPreview.position = rectTransform.position;
        }

        private void DestroyDragPreview()
        {
            if (dragPreview != null && activeDragPreview == dragPreview)
            {
                activeDragPreview = null;
            }

            if (dragPreview == null)
            {
                return;
            }

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

        private static void DestroyActivePreview()
        {
            if (activeDragPreview == null)
            {
                return;
            }

            var previewObject = activeDragPreview.gameObject;
            activeDragPreview = null;
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
    }
}
