using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntityTokenDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private CanvasGroup canvasGroup;
        private RectTransform dragPreview;
        private string tokenText;
        private static readonly List<RectTransform> ActivePreviews = new List<RectTransform>();

        public string ChipKey { get; private set; }

        public void Configure(string chipKey, string text)
        {
            ChipKey = chipKey ?? string.Empty;
            tokenText = text ?? string.Empty;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ClearActivePreviews();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.blocksRaycasts = false;
            dragPreview = CreateDragPreview();
            if (dragPreview != null)
            {
                ActivePreviews.Add(dragPreview);
            }

            MovePreview(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MovePreview(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            RestoreRaycasts();
            ClearOwnPreview();
        }

        private void OnDisable()
        {
            RestoreRaycasts();
            ClearOwnPreview();
        }

        private void OnDestroy()
        {
            ClearOwnPreview();
        }

        public static void ClearActivePreviews()
        {
            for (var index = ActivePreviews.Count - 1; index >= 0; index--)
            {
                var preview = ActivePreviews[index];
                if (preview == null)
                {
                    ActivePreviews.RemoveAt(index);
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(preview.gameObject);
                }
                else
                {
                    DestroyImmediate(preview.gameObject);
                }

                ActivePreviews.RemoveAt(index);
            }
        }

        private void RestoreRaycasts()
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void ClearOwnPreview()
        {
            if (dragPreview == null)
            {
                return;
            }

            ActivePreviews.Remove(dragPreview);
            if (Application.isPlaying)
            {
                Destroy(dragPreview.gameObject);
            }
            else
            {
                DestroyImmediate(dragPreview.gameObject);
            }

            dragPreview = null;
        }

        private RectTransform CreateDragPreview()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                return null;
            }

            var preview = new GameObject("Act 2 Token Drag Preview", typeof(RectTransform)).GetComponent<RectTransform>();
            preview.SetParent(canvas.transform, false);
            preview.sizeDelta = new Vector2(150f, 54f);

            var image = preview.gameObject.AddComponent<Image>();
            image.color = new Color(1f, 0.93f, 0.68f, 0.92f);
            image.raycastTarget = false;

            var outline = preview.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.95f, 0.55f, 0.18f, 0.95f);
            outline.effectDistance = new Vector2(2f, -2f);

            var label = new GameObject("Text", typeof(RectTransform)).AddComponent<Text>();
            label.transform.SetParent(preview, false);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            label.text = tokenText;
            label.font = GetBuiltinFont();
            label.fontSize = 20;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.13f, 0.10f, 0.20f);
            label.raycastTarget = false;

            return preview;
        }

        private void MovePreview(PointerEventData eventData)
        {
            if (dragPreview == null || eventData == null)
            {
                return;
            }

            dragPreview.position = eventData.position;
        }

        private static Font GetBuiltinFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font != null)
            {
                return font;
            }

            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
