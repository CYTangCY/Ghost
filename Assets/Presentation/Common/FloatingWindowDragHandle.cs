using UnityEngine;
using UnityEngine.EventSystems;

namespace Ghost.Presentation.Common
{
    public sealed class FloatingWindowDragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform targetWindow;

        private RectTransform parentRect;
        private Vector2 pointerStartLocalPosition;
        private Vector2 windowStartAnchoredPosition;

        public void Configure(RectTransform window)
        {
            targetWindow = window;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ResolveTargetWindow();
            if (targetWindow == null)
            {
                return;
            }

            parentRect = targetWindow.parent as RectTransform;
            if (parentRect == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out pointerStartLocalPosition);
            windowStartAnchoredPosition = targetWindow.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (targetWindow == null || parentRect == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out var currentPointerLocalPosition);

            var proposedPosition = windowStartAnchoredPosition + currentPointerLocalPosition - pointerStartLocalPosition;
            targetWindow.anchoredPosition = ClampToParent(proposedPosition);
        }

        private void ResolveTargetWindow()
        {
            if (targetWindow != null)
            {
                return;
            }

            targetWindow = transform.parent as RectTransform;
            if (targetWindow == null)
            {
                targetWindow = GetComponentInParent<RectTransform>();
            }
        }

        private Vector2 ClampToParent(Vector2 proposedPosition)
        {
            var parentSize = parentRect.rect.size;
            var windowSize = targetWindow.rect.size;

            var minX = -parentSize.x * parentRect.pivot.x + windowSize.x * targetWindow.pivot.x;
            var maxX = parentSize.x * (1f - parentRect.pivot.x) - windowSize.x * (1f - targetWindow.pivot.x);
            var minY = -parentSize.y * parentRect.pivot.y + windowSize.y * targetWindow.pivot.y;
            var maxY = parentSize.y * (1f - parentRect.pivot.y) - windowSize.y * (1f - targetWindow.pivot.y);

            return new Vector2(
                ClampAxis(proposedPosition.x, minX, maxX),
                ClampAxis(proposedPosition.y, minY, maxY));
        }

        private static float ClampAxis(float value, float min, float max)
        {
            return min <= max
                ? Mathf.Clamp(value, min, max)
                : (min + max) * 0.5f;
        }
    }
}
