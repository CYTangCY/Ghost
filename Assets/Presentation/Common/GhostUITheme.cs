using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ghost.Presentation.Common
{
    /// <summary>
    /// One place for every font size, colour and corner radius in the game. Before this existed each
    /// presenter carried its own copy of the same text/panel helpers, which is why body copy drifted
    /// down to 13px and the greys ended up unreadable.
    /// </summary>
    public static class GhostUITheme
    {
        // Second pass, 2026-08-03. The first scale (30/24/19/17/15) was too aggressive: it scaled the
        // title by +50% and the body by +35% while every column width, card size and row height in the
        // game was still drawn for 13-14px text, so headings wrapped into three lines inside 125px
        // columns. These values keep the readability win (body is still ~25% up on the old 13-14) at a
        // uniform ratio, so chapters stop looking mismatched.
        public const int TitleSize = 26;
        public const int HeadingSize = 21;
        public const int BodySize = 17;
        public const int SmallSize = 15;

        // Hard floor. Nothing in the game may render smaller than this.
        public const int TinySize = 14;

        public const int PanelRadius = 18;
        public const int CardRadius = 14;
        public const int ButtonRadius = 16;

        // Chips run about 36px tall, so this reads as a full pill.
        public const int ChipRadius = 18;

        public static readonly Color Ink = Hex(0x24, 0x1C, 0x2E);
        public static readonly Color InkSoft = Hex(0x4A, 0x41, 0x59);
        public static readonly Color InkOnDark = Hex(0xF4, 0xF0, 0xFA);
        public static readonly Color Accent = Hex(0x6D, 0x5B, 0xD0);
        public static readonly Color Good = Hex(0x1E, 0x7F, 0x63);
        public static readonly Color Bad = Hex(0xB3, 0x40, 0x2F);

        public static readonly Color PanelFill = Hex(0xFD, 0xFC, 0xFF);
        public static readonly Color CardFill = Hex(0xF5, 0xF2, 0xFC);
        public static readonly Color DropFill = Hex(0xEE, 0xEA, 0xF9);

        private static readonly Dictionary<int, Sprite> RoundedCache = new Dictionary<int, Sprite>();
        private static Font cachedFont;

        public static Font Font
        {
            get
            {
                if (cachedFont == null)
                {
                    cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                        ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
                }

                return cachedFont;
            }
        }

        /// <summary>
        /// Builds a 9-sliced rounded rectangle so panels stop looking like stacked boxes. Generated at
        /// runtime because the project ships no external art.
        /// </summary>
        public static Sprite RoundedSprite(int radius)
        {
            radius = Mathf.Clamp(radius, 2, 48);
            if (RoundedCache.TryGetValue(radius, out var cached) && cached != null)
            {
                return cached;
            }

            // One stretchable pixel in the middle is all the 9-slice needs.
            var size = radius * 2 + 2;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color32[size * size];
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    pixels[y * size + x] = new Color32(255, 255, 255, CornerAlpha(x, y, size, radius));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();

            var sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));
            sprite.hideFlags = HideFlags.HideAndDontSave;

            RoundedCache[radius] = sprite;
            return sprite;
        }

        public static Image Panel(string name, Transform parent, Color? fill = null)
        {
            var image = Surface(name, parent, fill ?? PanelFill, PanelRadius);
            if (image.color.a > 0.01f)
            {
                AddOutline(image.gameObject, new Color(Accent.r, Accent.g, Accent.b, 0.25f), 2f);
            }

            return image;
        }

        public static Image Panel(GameObject target, Color? fill = null)
        {
            var image = Surface(target, fill ?? PanelFill, PanelRadius);
            if (image.color.a > 0.01f)
            {
                AddOutline(image.gameObject, new Color(Accent.r, Accent.g, Accent.b, 0.25f), 2f);
            }

            return image;
        }

        public static RectTransform Panel(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax,
            Color? fill = null)
        {
            var image = Panel(name, parent, fill);
            var rect = image.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        public static Image Card(string name, Transform parent, Color? fill = null)
        {
            var image = Surface(name, parent, fill ?? CardFill, CardRadius);
            if (image.color.a > 0.01f)
            {
                AddOutline(image.gameObject, new Color(1f, 1f, 1f, 0.55f), 1f);
            }

            return image;
        }

        public static Image Card(GameObject target, Color? fill = null)
        {
            var image = Surface(target, fill ?? CardFill, CardRadius);
            if (image.color.a > 0.01f)
            {
                AddOutline(image.gameObject, new Color(1f, 1f, 1f, 0.55f), 1f);
            }

            return image;
        }

        public static RectTransform Card(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax,
            Color? fill = null)
        {
            var image = Card(name, parent, fill);
            var rect = image.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        public static Image Chip(string name, Transform parent, Color? fill = null)
        {
            return Surface(name, parent, fill ?? CardFill, ChipRadius);
        }

        public static Image Chip(GameObject target, Color? fill = null)
        {
            return Surface(target, fill ?? CardFill, ChipRadius);
        }

        public static Image DropZone(string name, Transform parent)
        {
            var image = Surface(name, parent, DropFill, PanelRadius);
            AddOutline(image.gameObject, new Color(Accent.r, Accent.g, Accent.b, 0.45f), 2f);
            return image;
        }

        public static Image DropZone(GameObject target)
        {
            var image = Surface(target, DropFill, PanelRadius);
            AddOutline(image.gameObject, new Color(Accent.r, Accent.g, Accent.b, 0.45f), 2f);
            return image;
        }

        public static Image DropZone(GameObject target, Color fill)
        {
            var image = Surface(target, fill, PanelRadius);
            AddOutline(image.gameObject, new Color(Accent.r, Accent.g, Accent.b, 0.45f), 2f);
            return image;
        }

        public static Image DropZone(string name, Transform parent, Color fill)
        {
            var image = Surface(name, parent, fill, PanelRadius);
            AddOutline(image.gameObject, new Color(Accent.r, Accent.g, Accent.b, 0.45f), 2f);
            return image;
        }

        public static Button PushButton(string name, Transform parent, string caption)
        {
            var image = Surface(name, parent, Accent, ButtonRadius);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var label = Label("Caption", image.transform, caption, BodySize, FontStyle.Bold, TextAnchor.MiddleCenter, InkOnDark);
            Stretch(label.rectTransform, 10f, 4f);
            return button;
        }

        public static Button PushButton(
            Transform parent,
            string caption,
            Color fill,
            float width)
        {
            var button = PushButton(caption + " Button", parent, caption);
            button.targetGraphic.color = fill;
            Label(
                button.GetComponentInChildren<Text>(),
                caption,
                BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Ink);

            var layout = button.gameObject.AddComponent<LayoutElement>();
            layout.minWidth = width;
            layout.preferredWidth = width;
            layout.minHeight = 42f;
            return button;
        }

        public static Button PushButton(
            GameObject target,
            string caption,
            Color? fill = null,
            Color? textColour = null)
        {
            var image = Surface(target, fill ?? Accent, ButtonRadius);
            var button = target.GetComponent<Button>() ?? target.AddComponent<Button>();
            button.targetGraphic = image;

            var label = target.GetComponentInChildren<Text>();
            if (label == null)
            {
                label = Label("Caption", target.transform, caption, BodySize, FontStyle.Bold, TextAnchor.MiddleCenter, textColour ?? InkOnDark);
            }
            else
            {
                Label(label, caption, BodySize, FontStyle.Bold, TextAnchor.MiddleCenter, textColour ?? InkOnDark);
            }

            Stretch(label.rectTransform, 10f, 4f);
            return button;
        }

        public static Button PushButton(
            string name,
            Transform parent,
            string caption,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Color fill,
            Color textColour)
        {
            var button = PushButton(name, parent, caption);
            var rect = (RectTransform)button.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            button.targetGraphic.color = fill;
            Label(
                button.GetComponentInChildren<Text>(),
                caption,
                BodySize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                textColour);
            return button;
        }

        public static Text Heading(string name, Transform parent, string text, Color? colour = null)
        {
            return Label(name, parent, text, HeadingSize, FontStyle.Bold, TextAnchor.MiddleLeft, colour ?? Ink);
        }

        public static Text Label(
            string name,
            Transform parent,
            string text,
            int size = BodySize,
            FontStyle style = FontStyle.Normal,
            TextAnchor align = TextAnchor.MiddleLeft,
            Color? colour = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var label = go.AddComponent<Text>();
            label.font = Font;
            label.text = text;
            label.fontSize = Mathf.Max(size, TinySize);
            label.fontStyle = style;
            label.alignment = align;
            label.color = colour ?? Ink;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;

            // Truncate, never Overflow. Overflow lets wrapped text paint outside its own rect and
            // straight over whatever sits below it - that is what printed the Act 3 palette titles on
            // top of their own descriptions. A clipped label is a bug you can see; an overflowing one
            // silently corrupts its neighbours.
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.supportRichText = true;
            return label;
        }

        public static Text Label(
            Transform parent,
            string text,
            int size,
            FontStyle style,
            TextAnchor align)
        {
            var label = Label(parent, "Text", text, size, style, align, Ink, 0f);
            Stretch(label.rectTransform, 6f, 2f);
            return label;
        }

        public static Text Label(
            Transform root,
            string childName,
            string text,
            int size,
            FontStyle style,
            TextAnchor align,
            Color? colour,
            float preferredHeight)
        {
            var child = root.Find(childName) as RectTransform;
            if (child == null)
            {
                child = new GameObject(childName, typeof(RectTransform)).GetComponent<RectTransform>();
                child.SetParent(root, false);
            }

            var label = child.GetComponent<Text>() ?? child.gameObject.AddComponent<Text>();
            Label(label, text, size, style, align, colour);
            if (preferredHeight > 0f)
            {
                preferredHeight = AtLeastOneLine(preferredHeight, size);
                var layout = child.GetComponent<LayoutElement>() ?? child.gameObject.AddComponent<LayoutElement>();
                layout.minHeight = preferredHeight;
                layout.preferredHeight = preferredHeight;
            }

            return label;
        }

        public static Text Label(
            Text label,
            string text,
            int size = BodySize,
            FontStyle style = FontStyle.Normal,
            TextAnchor align = TextAnchor.MiddleLeft,
            Color? colour = null)
        {
            if (label == null)
            {
                return null;
            }

            label.font = Font;
            label.text = text ?? string.Empty;
            label.fontSize = Mathf.Max(size, TinySize);
            label.fontStyle = style;
            label.alignment = align;
            label.color = colour ?? Ink;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;

            // Truncate, never Overflow. Overflow lets wrapped text paint outside its own rect and
            // straight over whatever sits below it - that is what printed the Act 3 palette titles on
            // top of their own descriptions. A clipped label is a bug you can see; an overflowing one
            // silently corrupts its neighbours.
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.supportRichText = true;
            return label;
        }

        public static Text Label(
            RectTransform target,
            string text,
            int size,
            FontStyle style,
            TextAnchor align,
            Color? colour,
            float preferredHeight)
        {
            if (target == null)
            {
                return null;
            }

            var label = target.GetComponent<Text>() ?? target.gameObject.AddComponent<Text>();
            Label(label, text, size, style, align, colour);

            if (preferredHeight > 0f)
            {
                preferredHeight = AtLeastOneLine(preferredHeight, size);
                var layout = target.GetComponent<LayoutElement>() ?? target.gameObject.AddComponent<LayoutElement>();
                layout.minHeight = preferredHeight;
                layout.preferredHeight = preferredHeight;
            }

            return label;
        }

        public static Text Label(
            string name,
            Transform parent,
            string text,
            int size,
            FontStyle style,
            TextAnchor align,
            Color? colour,
            Vector2 padding)
        {
            var label = Label(name, parent, text, size, style, align, colour);
            Stretch(label.rectTransform, padding.x, padding.y);
            return label;
        }

        public static Text Label(
            string name,
            Transform parent,
            string text,
            int size,
            FontStyle style,
            TextAnchor align,
            Color? colour,
            float preferredHeight)
        {
            var label = Label(name, parent, text, size, style, align, colour);
            var layout = label.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = preferredHeight;
            layout.preferredHeight = preferredHeight;
            return label;
        }

        public static Text Label(
            string name,
            Transform parent,
            string text,
            int size,
            FontStyle style,
            TextAnchor align,
            Color? colour,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            var label = Label(name, parent, text, size, style, align, colour);
            label.rectTransform.anchorMin = anchorMin;
            label.rectTransform.anchorMax = anchorMax;
            label.rectTransform.offsetMin = offsetMin;
            label.rectTransform.offsetMax = offsetMax;
            return label;
        }

        /// <summary>
        /// A box exactly as tall as the font clips its own text, because a rendered line is taller than
        /// its point size. Callers wrote heights like 26f for 26px titles back when overflow was allowed
        /// to spill; now that labels truncate, those titles would vanish entirely.
        /// </summary>
        private static float AtLeastOneLine(float requestedHeight, int fontSize)
        {
            return Mathf.Max(requestedHeight, Mathf.Ceil(Mathf.Max(fontSize, TinySize) * 1.35f));
        }

        /// <summary>
        /// Swaps real artwork into an Image built by one of the surface factories. Those set a 9-sliced
        /// rounded sprite, and a sliced Image ignores preserveAspect - which is what flattened Lily in
        /// the Chapter 0 portrait. Always route portrait/artwork assignment through here.
        /// </summary>
        public static void Picture(Image target, Sprite sprite, bool preserveAspect = true)
        {
            if (target == null)
            {
                return;
            }

            target.sprite = sprite;
            target.type = Image.Type.Simple;
            target.preserveAspect = preserveAspect;
            target.color = Color.white;
        }

        public static void Stretch(RectTransform rect, float horizontalPadding = 0f, float verticalPadding = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontalPadding, verticalPadding);
            rect.offsetMax = new Vector2(-horizontalPadding, -verticalPadding);
        }

        private static Image Surface(string name, Transform parent, Color fill, int radius)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return Surface(go, fill, radius);
        }

        private static Image Surface(GameObject go, Color fill, int radius)
        {
            var image = go.GetComponent<Image>() ?? go.AddComponent<Image>();
            image.sprite = RoundedSprite(radius);
            image.type = Image.Type.Sliced;
            image.color = fill;
            return image;
        }

        private static void AddOutline(GameObject target, Color colour, float thickness)
        {
            var outline = target.GetComponent<Outline>() ?? target.AddComponent<Outline>();
            outline.effectColor = colour;
            outline.effectDistance = new Vector2(thickness, -thickness);
            outline.useGraphicAlpha = false;
        }

        private static byte CornerAlpha(int x, int y, int size, int radius)
        {
            var px = x + 0.5f;
            var py = y + 0.5f;

            // How far the pixel sits inside one of the four corner squares; zero along the straight edges.
            var qx = Mathf.Max(radius - px, px - (size - radius), 0f);
            var qy = Mathf.Max(radius - py, py - (size - radius), 0f);

            var distance = Mathf.Sqrt(qx * qx + qy * qy);
            var coverage = Mathf.Clamp01(radius - distance + 0.5f);
            return (byte)Mathf.RoundToInt(coverage * 255f);
        }

        private static Color Hex(int r, int g, int b)
        {
            return new Color(r / 255f, g / 255f, b / 255f, 1f);
        }
    }
}
