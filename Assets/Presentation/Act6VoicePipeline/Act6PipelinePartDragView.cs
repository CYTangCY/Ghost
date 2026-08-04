using Ghost.Presentation.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ghost.Presentation.Act6VoicePipeline
{
    public sealed class Act6PipelinePartDragView : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler
    {
        private static readonly List<RectTransform> ActivePreviews = new List<RectTransform>();

        private IAct6PipelineInteractionHost host;
        private CanvasGroup canvasGroup;
        private RectTransform dragPreview;
        private string label;

        public string ComponentId { get; private set; }

        public void Configure(
            IAct6PipelineInteractionHost interactionHost,
            string componentId,
            string componentLabel)
        {
            host = interactionHost;
            ComponentId = componentId ?? string.Empty;
            label = componentLabel ?? string.Empty;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            host?.SelectComponent(ComponentId);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ClearActivePreviews();
            EnsureCanvasGroup();
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
                ActivePreviews.RemoveAt(index);
                if (preview == null)
                {
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
            }
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

            var preview = new GameObject(
                "Final Chapter Component Drag Preview",
                typeof(RectTransform)).GetComponent<RectTransform>();
            preview.SetParent(canvas.transform, false);
            preview.sizeDelta = new Vector2(220f, 64f);

            var image = GhostUITheme.Chip(preview.gameObject);
            image.color = new Color(0.98f, 0.94f, 0.74f, 0.96f);
            image.raycastTarget = false;

            var outline = preview.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.32f, 0.48f, 0.66f, 0.95f);
            outline.effectDistance = new Vector2(2f, -2f);

            var text = new GameObject("Label", typeof(RectTransform)).AddComponent<Text>();
            text.transform.SetParent(preview, false);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(8f, 4f);
            text.rectTransform.offsetMax = new Vector2(-8f, -4f);
            text.text = label;
            text.font = GhostUITheme.Font;
            text.fontSize = GhostUITheme.HeadingSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = GhostUITheme.Ink;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = GhostUITheme.TinySize;
            text.resizeTextMaxSize = GhostUITheme.HeadingSize;
            text.raycastTarget = false;
            return preview;
        }

        private void MovePreview(PointerEventData eventData)
        {
            if (dragPreview != null && eventData != null)
            {
                dragPreview.position = eventData.position;
            }
        }

    }
}
