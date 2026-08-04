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
            return Clamp(
                proposedPosition,
                parentRect.rect.size,
                parentRect.pivot,
                targetWindow.rect.size,
                targetWindow.pivot,
                targetWindow.anchorMin,
                targetWindow.anchorMax);
        }

        /// <summary>
        /// Keeps a window inside its parent. Pulled out as a pure function so the anchor maths can be
        /// tested without building a Canvas.
        /// </summary>
        /// <remarks>
        /// anchoredPosition is measured from the centre of the window's own anchor rect, not from the
        /// parent's origin. The original version left that term out, so the bounds were only correct
        /// for a centre-anchored window - which is why "Ask Lily" refused to travel to the top of the
        /// screen in some chapters and behaved fine in others.
        /// </remarks>
        public static Vector2 Clamp(
            Vector2 proposedPosition,
            Vector2 parentSize,
            Vector2 parentPivot,
            Vector2 windowSize,
            Vector2 windowPivot,
            Vector2 anchorMin,
            Vector2 anchorMax)
        {
            var anchorCentre = new Vector2(
                ((anchorMin.x + anchorMax.x) * 0.5f - parentPivot.x) * parentSize.x,
                ((anchorMin.y + anchorMax.y) * 0.5f - parentPivot.y) * parentSize.y);

            var minX = -parentSize.x * parentPivot.x + windowSize.x * windowPivot.x - anchorCentre.x;
            var maxX = parentSize.x * (1f - parentPivot.x) - windowSize.x * (1f - windowPivot.x) - anchorCentre.x;
            var minY = -parentSize.y * parentPivot.y + windowSize.y * windowPivot.y - anchorCentre.y;
            var maxY = parentSize.y * (1f - parentPivot.y) - windowSize.y * (1f - windowPivot.y) - anchorCentre.y;

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
