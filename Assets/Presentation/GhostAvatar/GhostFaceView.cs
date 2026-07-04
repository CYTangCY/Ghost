using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.GhostAvatar
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class GhostFaceView : MonoBehaviour
    {
        private Image bodyImage;
        private Image leftEyeImage;
        private Image rightEyeImage;
        private Text mouthText;
        private Text moodMarkText;
        private RectTransform leftEyeRect;
        private RectTransform rightEyeRect;
        private static Texture2D generatedSpriteTexture;
        private static Sprite generatedSprite;

        private void Awake()
        {
            EnsureView();
            SetMood(GhostMood.Neutral);
        }

        public void SetMood(GhostMood mood)
        {
            EnsureView();

            switch (mood)
            {
                case GhostMood.Happy:
                    ApplyMood(
                        new Color(0.96f, 1f, 0.92f),
                        new Vector2(-24f, 18f),
                        new Vector2(24f, 18f),
                        new Vector2(15f, 18f),
                        "u",
                        string.Empty);
                    break;
                case GhostMood.Confused:
                    ApplyMood(
                        new Color(0.93f, 0.97f, 1f),
                        new Vector2(-22f, 21f),
                        new Vector2(25f, 15f),
                        new Vector2(13f, 18f),
                        "o",
                        "?");
                    break;
                case GhostMood.Sad:
                    ApplyMood(
                        new Color(0.94f, 0.94f, 1f),
                        new Vector2(-24f, 18f),
                        new Vector2(24f, 18f),
                        new Vector2(14f, 16f),
                        "n",
                        string.Empty);
                    break;
                default:
                    ApplyMood(
                        new Color(0.98f, 0.98f, 1f),
                        new Vector2(-24f, 18f),
                        new Vector2(24f, 18f),
                        new Vector2(14f, 17f),
                        "-",
                        string.Empty);
                    break;
            }
        }

        private void EnsureView()
        {
            var root = GetComponent<RectTransform>();
            root.sizeDelta = root.sizeDelta == Vector2.zero ? new Vector2(150f, 150f) : root.sizeDelta;

            bodyImage = EnsureImage("Ghost Body", transform, out var bodyRect);
            bodyRect.anchorMin = new Vector2(0.5f, 0.5f);
            bodyRect.anchorMax = new Vector2(0.5f, 0.5f);
            bodyRect.pivot = new Vector2(0.5f, 0.5f);
            bodyRect.anchoredPosition = Vector2.zero;
            bodyRect.sizeDelta = new Vector2(136f, 128f);
            bodyImage.sprite = GetBuiltinSprite();
            bodyImage.raycastTarget = false;

            leftEyeImage = EnsureImage("Left Eye", bodyRect, out leftEyeRect);
            rightEyeImage = EnsureImage("Right Eye", bodyRect, out rightEyeRect);
            ConfigureEye(leftEyeImage);
            ConfigureEye(rightEyeImage);

            mouthText = EnsureFillText("Mouth", bodyRect, 34, FontStyle.Bold, TextAnchor.MiddleCenter);
            mouthText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            mouthText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            mouthText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            mouthText.rectTransform.anchoredPosition = new Vector2(0f, -24f);
            mouthText.rectTransform.sizeDelta = new Vector2(56f, 36f);

            moodMarkText = EnsureFillText("Mood Mark", bodyRect, 28, FontStyle.Bold, TextAnchor.MiddleCenter);
            moodMarkText.rectTransform.anchorMin = new Vector2(1f, 1f);
            moodMarkText.rectTransform.anchorMax = new Vector2(1f, 1f);
            moodMarkText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            moodMarkText.rectTransform.anchoredPosition = new Vector2(-16f, -18f);
            moodMarkText.rectTransform.sizeDelta = new Vector2(32f, 32f);
        }

        private void ApplyMood(
            Color bodyColor,
            Vector2 leftEyePosition,
            Vector2 rightEyePosition,
            Vector2 eyeSize,
            string mouth,
            string moodMark)
        {
            bodyImage.color = bodyColor;
            SetEye(leftEyeRect, leftEyePosition, eyeSize);
            SetEye(rightEyeRect, rightEyePosition, eyeSize);
            mouthText.text = mouth;
            mouthText.color = new Color(0.16f, 0.12f, 0.22f);
            moodMarkText.text = moodMark;
            moodMarkText.color = new Color(0.22f, 0.18f, 0.34f);
        }

        private static void ConfigureEye(Image eye)
        {
            eye.sprite = GetBuiltinSprite();
            eye.color = new Color(0.12f, 0.10f, 0.16f);
            eye.raycastTarget = false;
        }

        private static void SetEye(RectTransform eyeRect, Vector2 position, Vector2 size)
        {
            eyeRect.anchorMin = new Vector2(0.5f, 0.5f);
            eyeRect.anchorMax = new Vector2(0.5f, 0.5f);
            eyeRect.pivot = new Vector2(0.5f, 0.5f);
            eyeRect.anchoredPosition = position;
            eyeRect.sizeDelta = size;
        }

        private static Image EnsureImage(string name, Transform parent, out RectTransform rect)
        {
            var existing = parent.Find(name) as RectTransform;
            if (existing == null)
            {
                existing = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
                existing.SetParent(parent, false);
            }

            rect = existing;
            var image = existing.GetComponent<Image>();
            if (image == null)
            {
                image = existing.gameObject.AddComponent<Image>();
            }

            return image;
        }

        private static Text EnsureFillText(
            string name,
            Transform parent,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment)
        {
            var existing = parent.Find(name) as RectTransform;
            if (existing == null)
            {
                existing = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
                existing.SetParent(parent, false);
            }

            var text = existing.GetComponent<Text>();
            if (text == null)
            {
                text = existing.gameObject.AddComponent<Text>();
            }

            text.font = GetBuiltinFont();
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static Sprite GetBuiltinSprite()
        {
            if (generatedSprite != null)
            {
                return generatedSprite;
            }

            generatedSpriteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            generatedSpriteTexture.SetPixel(0, 0, Color.white);
            generatedSpriteTexture.Apply(false, false);

            generatedSprite = Sprite.Create(
                generatedSpriteTexture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0.5f),
                1f);
            generatedSprite.hideFlags = HideFlags.HideAndDontSave;
            return generatedSprite;
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
