using Ghost.Presentation.Common;
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

            var pixelSprite = GhostPixelSpriteFactory.GetSprite(mood);
            if (pixelSprite != null)
            {
                ApplyPixelMood(pixelSprite);
                return;
            }

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
            bodyImage.preserveAspect = true;
            bodyImage.raycastTarget = false;

            leftEyeImage = EnsureImage("Left Eye", bodyRect, out leftEyeRect);
            rightEyeImage = EnsureImage("Right Eye", bodyRect, out rightEyeRect);
            ConfigureEye(leftEyeImage);
            ConfigureEye(rightEyeImage);

            mouthText = EnsureFillText("Mouth", bodyRect, GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleCenter);
            mouthText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            mouthText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            mouthText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            mouthText.rectTransform.anchoredPosition = new Vector2(0f, -24f);
            mouthText.rectTransform.sizeDelta = new Vector2(56f, 36f);

            moodMarkText = EnsureFillText("Mood Mark", bodyRect, GhostUITheme.TitleSize, FontStyle.Bold, TextAnchor.MiddleCenter);
            moodMarkText.rectTransform.anchorMin = new Vector2(1f, 1f);
            moodMarkText.rectTransform.anchorMax = new Vector2(1f, 1f);
            moodMarkText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            moodMarkText.rectTransform.anchoredPosition = new Vector2(-16f, -18f);
            moodMarkText.rectTransform.sizeDelta = new Vector2(32f, 32f);
        }

        private void ApplyPixelMood(Sprite sprite)
        {
            bodyImage.sprite = sprite;
            bodyImage.color = Color.white;
            bodyImage.preserveAspect = true;
            leftEyeImage.enabled = false;
            rightEyeImage.enabled = false;
            mouthText.enabled = false;
            moodMarkText.enabled = false;
        }

        private void ApplyMood(
            Color bodyColor,
            Vector2 leftEyePosition,
            Vector2 rightEyePosition,
            Vector2 eyeSize,
            string mouth,
            string moodMark)
        {
            leftEyeImage.enabled = true;
            rightEyeImage.enabled = true;
            mouthText.enabled = true;
            moodMarkText.enabled = true;
            bodyImage.color = bodyColor;
            SetEye(leftEyeRect, leftEyePosition, eyeSize);
            SetEye(rightEyeRect, rightEyePosition, eyeSize);
            mouthText.text = mouth;
            mouthText.color = GhostUITheme.Ink;
            moodMarkText.text = moodMark;
            moodMarkText.color = GhostUITheme.Ink;
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

            GhostUITheme.Label(text, string.Empty, fontSize, fontStyle, alignment, GhostUITheme.Ink);
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

    }
    public static class GhostPixelSpriteFactory
    {
        private const string NeutralResourcePath = "Characters/GhostPixelNeutral";
        private const string HappyResourcePath = "Characters/GhostPixelHappy";
        private const string ConfusedResourcePath = "Characters/GhostPixelConfused";
        private const string SadResourcePath = "Characters/GhostPixelSad";

        private static Sprite neutralSprite;
        private static Sprite happySprite;
        private static Sprite confusedSprite;
        private static Sprite sadSprite;

        public static Sprite GetSprite(GhostMood mood)
        {
            switch (mood)
            {
                case GhostMood.Happy:
                    return happySprite != null
                        ? happySprite
                        : happySprite = LoadSprite(HappyResourcePath);
                case GhostMood.Confused:
                    return confusedSprite != null
                        ? confusedSprite
                        : confusedSprite = LoadSprite(ConfusedResourcePath);
                case GhostMood.Sad:
                    return sadSprite != null
                        ? sadSprite
                        : sadSprite = LoadSprite(SadResourcePath);
                default:
                    return neutralSprite != null
                        ? neutralSprite
                        : neutralSprite = LoadSprite(NeutralResourcePath);
            }
        }

        private static Sprite LoadSprite(string resourcePath)
        {
            var texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
            {
                return null;
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            var sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                Mathf.Max(texture.width, texture.height));
            sprite.name = texture.name + " Runtime Sprite";
            return sprite;
        }
    }

}
