using Ghost.Presentation.Common;
using NUnit.Framework;
using UnityEngine;

namespace Ghost.Tests.EditMode
{
    public sealed class FloatingWindowDragHandleTests
    {
        private static readonly Vector2 ParentSize = new Vector2(1920f, 1080f);
        private static readonly Vector2 ParentPivot = new Vector2(0.5f, 0.5f);
        private static readonly Vector2 WindowSize = new Vector2(400f, 300f);
        private static readonly Vector2 WindowPivot = new Vector2(0.5f, 0.5f);

        private static Vector2 Clamp(Vector2 proposed, Vector2 anchorMin, Vector2 anchorMax)
        {
            return FloatingWindowDragHandle.Clamp(
                proposed, ParentSize, ParentPivot, WindowSize, WindowPivot, anchorMin, anchorMax);
        }

        /// <summary>Where the window's top edge ends up, in parent-local space.</summary>
        private static float TopEdge(Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            var anchorCentre = ((anchorMin.y + anchorMax.y) * 0.5f - ParentPivot.y) * ParentSize.y;
            return anchorCentre + anchoredPosition.y + WindowSize.y * (1f - WindowPivot.y);
        }

        // The bug this covers: the clamp ignored the window's anchors, so it was only correct for a
        // centre-anchored window. Chapters whose Lily window used another anchor stopped short of the
        // top of the screen.
        [TestCase(0.5f, 1f, 0.5f, 1f, TestName = "Clamp_CentreAnchor_ReachesTop")]
        [TestCase(0f, 1f, 0f, 1f, TestName = "Clamp_TopLeftAnchor_ReachesTop")]
        [TestCase(0f, 1f, 1f, 1f, TestName = "Clamp_TopStretchAnchor_ReachesTop")]
        [TestCase(0f, 0f, 1f, 1f, TestName = "Clamp_FullStretchAnchor_ReachesTop")]
        [TestCase(1f, 0f, 1f, 0f, TestName = "Clamp_BottomRightAnchor_ReachesTop")]
        public void DraggingPastTheTopStopsExactlyAtTheParentTop(
            float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY)
        {
            var anchorMin = new Vector2(anchorMinX, anchorMinY);
            var anchorMax = new Vector2(anchorMaxX, anchorMaxY);

            var clamped = Clamp(new Vector2(0f, 99999f), anchorMin, anchorMax);

            var parentTop = ParentSize.y * (1f - ParentPivot.y);
            Assert.That(TopEdge(clamped, anchorMin, anchorMax), Is.EqualTo(parentTop).Within(0.01f));
        }

        [Test]
        public void APositionAlreadyInsideIsLeftAlone()
        {
            var anchor = new Vector2(0.5f, 0.5f);

            var clamped = Clamp(new Vector2(120f, -80f), anchor, anchor);

            Assert.That(clamped, Is.EqualTo(new Vector2(120f, -80f)));
        }

        [Test]
        public void AWindowLargerThanItsParentIsCentredRatherThanFlungAway()
        {
            var anchor = new Vector2(0.5f, 0.5f);

            var clamped = FloatingWindowDragHandle.Clamp(
                new Vector2(5000f, 5000f),
                new Vector2(200f, 200f),
                ParentPivot,
                new Vector2(600f, 600f),
                WindowPivot,
                anchor,
                anchor);

            Assert.That(clamped, Is.EqualTo(Vector2.zero));
        }
    }
}
