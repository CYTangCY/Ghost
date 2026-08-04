using Ghost.Presentation.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ghost.Presentation.Act6BackendResponse
{
    public sealed class Act6BackendCardDragView : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler
    {
        private IAct6BackendInteractionHost host;
        private RectTransform rootCanvas;
        private CanvasGroup canvasGroup;
        private RectTransform preview;
        private string label;
        private bool clickSelectionEnabled;

        public string CardId { get; private set; }

        public void Configure(
            string cardId,
            string cardLabel,
            RectTransform canvasRect,
            IAct6BackendInteractionHost interactionHost,
            bool enableClickSelection)
        {
            CardId = cardId ?? string.Empty;
            label = cardLabel ?? string.Empty;
            rootCanvas = canvasRect;
            host = interactionHost;
            clickSelectionEnabled = enableClickSelection;
            EnsureCanvasGroup();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!clickSelectionEnabled || eventData == null || eventData.used)
            {
                return;
            }

            host?.SelectCard(CardId);
            eventData.Use();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            EnsureCanvasGroup();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.58f;
            preview = CreatePreview();
            MovePreview(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MovePreview(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Restore();
            ClearPreview();
        }

        private void OnDisable()
        {
            Restore();
            ClearPreview();
        }

        private void OnDestroy()
        {
            ClearPreview();
        }

        private void EnsureCanvasGroup()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void Restore()
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        private RectTransform CreatePreview()
        {
            if (rootCanvas == null)
            {
                return null;
            }

            var root = new GameObject(
                "Chapter 6 Backend Card Preview",
                typeof(RectTransform)).GetComponent<RectTransform>();
            root.SetParent(rootCanvas, false);
            root.anchorMin = new Vector2(0.5f, 0.5f);
            root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = new Vector2(310f, 70f);
            root.SetAsLastSibling();

            var image = GhostUITheme.Card(root.gameObject);
            image.color = new Color(1f, 0.97f, 0.78f, 0.96f);
            image.raycastTarget = false;

            var outline = root.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.55f, 0.43f, 0.18f, 0.9f);
            outline.effectDistance = new Vector2(2f, -2f);

            var textObject = new GameObject("Preview Label", typeof(RectTransform));
            textObject.transform.SetParent(root, false);
            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12f, 8f);
            textRect.offsetMax = new Vector2(-12f, -8f);

            var text = textObject.AddComponent<Text>();
            text.text = label;
            text.font = GhostUITheme.Font;
            text.fontSize = GhostUITheme.BodySize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = GhostUITheme.Ink;
            text.raycastTarget = false;
            return root;
        }

        private void MovePreview(PointerEventData eventData)
        {
            if (preview == null || rootCanvas == null || eventData == null)
            {
                return;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rootCanvas,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPoint))
            {
                preview.anchoredPosition = localPoint;
            }
        }

        private void ClearPreview()
        {
            if (preview == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(preview.gameObject);
            }
            else
            {
                DestroyImmediate(preview.gameObject);
            }

            preview = null;
        }

    }
}
