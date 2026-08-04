using System.Reflection;
using Ghost.Presentation.Act5TestingDebugging;
using NUnit.Framework;
using UnityEngine;

namespace Ghost.Tests.EditMode
{
    public sealed class Act5TestingStaticPresenterTests
    {
        [Test]
        public void DrawLine_UsesCenteredWireLayerCoordinatesWithoutBoardOffset()
        {
            var lineObject = new GameObject("Wire", typeof(RectTransform));

            try
            {
                var line = lineObject.GetComponent<RectTransform>();
                var drawLine = typeof(Act5TestingStaticPresenter).GetMethod(
                    "DrawLine",
                    BindingFlags.NonPublic | BindingFlags.Static);

                Assert.That(drawLine, Is.Not.Null);

                var start = new Vector2(-320f, 85f);
                var end = new Vector2(180f, -115f);
                drawLine.Invoke(null, new object[] { line, start, end });

                Assert.That(line.anchorMin, Is.EqualTo(new Vector2(0.5f, 0.5f)));
                Assert.That(line.anchorMax, Is.EqualTo(new Vector2(0.5f, 0.5f)));
                Assert.That(line.pivot, Is.EqualTo(new Vector2(0f, 0.5f)));
                Assert.That(line.anchoredPosition, Is.EqualTo(start));
                Assert.That(line.sizeDelta.x, Is.EqualTo(Vector2.Distance(start, end)).Within(0.001f));
                Assert.That(line.sizeDelta.y, Is.EqualTo(5f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(lineObject);
            }
        }
    }
}
