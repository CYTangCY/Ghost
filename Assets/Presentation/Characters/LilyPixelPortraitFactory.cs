using UnityEngine;

namespace Ghost.Presentation.Characters
{
    public static class LilyPixelPortraitFactory
    {
        private const int Size = 48;

        private static Texture2D portraitTexture;
        private static Sprite portraitSprite;

        // 48x48 pixel map, drawn top-down. Legend: . transparent, o outline, H hair, h hair highlight,
        // d hair shadow, S skin, s skin shadow, b blush, G black glasses frame, L lens tint, e eye,
        // w sparkle/button, m mouth, J blue suit jacket, j jacket shadow/lapel, W white shirt,
        // P black pants, p pants highlight, K black heels.
        private static readonly string[] PortraitRows =
        {
            "................................................",
            "................................................",
            "....................oooooooo....................",
            "..................oohhhhhHHHoo..................",
            ".................ohhhhhHHHHHHHo.................",
            "................ohhhhHHHHHHHHHHo................",
            "...............ohhhHHHHHHHHHHHHo................",
            "...............ohhHHHHHHHHHHHHHdo...............",
            "..............ohhHHHHHHHHHHHHHHdo...............",
            "..............ohHHHHHHHHHHHHHHHddo..............",
            "..............ohHHHHHHHHHHHHHHHddo..............",
            "..............ohHHHHHHHHHHHHHHHddo..............",
            "..............ohHHHSSHHHSSHHSSHddo..............",
            "..............ohHSGGGGSSSSGGGGSddo..............",
            "..............ohGGLwLLGGGGLwLLGGdo..............",
            "..............ohHGLeeLGSSGLeeLGddo..............",
            "..............ohHGLeeLGSSGLeeLGddo..............",
            "..............ohHSGGGGSSSSGGGGSddo..............",
            "..............ohHbbSSSSSsSSSSbbddo..............",
            ".............oohHSSSSSSmmSSSSSHddoo.............",
            ".............ohhdSSSSSSSSSSSSSddddo.............",
            "..............oodSSSSSSSSSSSSdoo................",
            "................odsSSSSSSSSsdo..................",
            ".................oosSSSSSSoo....................",
            "...................oSSSSSo......................",
            "..................oojWWWWjoo....................",
            "................oJJjWWWWWWjJJo..................",
            "...............oJJJjWWWWWWjJJJo.................",
            "..............oJJJJjjWWWWjjJJJJo................",
            "..............oJjJJJjWWWWjJJJjJo................",
            "..............oJjJJJjjWWjjJJJjJo................",
            "...............ojJJJJjWWjJJJJjo.................",
            "...............ojJJJJJwJJJJJjJo.................",
            "...............ojJJJJJJJJJJJjJo.................",
            "...............ooJJJJJwJJJJJoo..................",
            "...............oSSoJJJJJJJJoSSo.................",
            "...............oSSooPPPPPPooSSo.................",
            "................oo.PPPppPPPP.oo.................",
            "...................PPPp.PPPP....................",
            "...................PPPp.PPPP....................",
            "...................PPPp.PPPP....................",
            "...................PPPp.PPPP....................",
            "...................PPPp.PPPP....................",
            "...................PPPp.PPPP....................",
            "...................PPPp.PPPP....................",
            "..................KKKKK.KKKKK...................",
            "..................KKKKK.KKKKK...................",
            "...................K.......K...................."
        };

        public static Sprite GetPortrait()
        {
            if (portraitSprite != null)
            {
                return portraitSprite;
            }

            portraitTexture = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color32[Size * Size];
            var clear = new Color32(0, 0, 0, 0);
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = clear;
            }

            for (var rowIndex = 0; rowIndex < PortraitRows.Length && rowIndex < Size; rowIndex++)
            {
                var row = PortraitRows[rowIndex];
                var textureY = Size - 1 - rowIndex;
                for (var x = 0; x < row.Length && x < Size; x++)
                {
                    pixels[textureY * Size + x] = ColorForCharacter(row[x]);
                }
            }

            portraitTexture.SetPixels32(pixels);
            portraitTexture.Apply(false, false);

            portraitSprite = Sprite.Create(
                portraitTexture,
                new Rect(0f, 0f, Size, Size),
                new Vector2(0.5f, 0.5f),
                Size);
            portraitSprite.hideFlags = HideFlags.HideAndDontSave;
            return portraitSprite;
        }

        private static Color32 ColorForCharacter(char pixelCharacter)
        {
            switch (pixelCharacter)
            {
                case 'o':
                    return new Color32(52, 40, 58, 255);
                case 'H':
                    return new Color32(236, 190, 82, 255);
                case 'h':
                    return new Color32(252, 226, 130, 255);
                case 'd':
                    return new Color32(184, 136, 56, 255);
                case 'S':
                    return new Color32(252, 218, 196, 255);
                case 's':
                    return new Color32(230, 176, 152, 255);
                case 'b':
                    return new Color32(247, 158, 164, 255);
                case 'G':
                    return new Color32(28, 26, 32, 255);
                case 'L':
                    return new Color32(226, 238, 248, 255);
                case 'e':
                    return new Color32(70, 50, 44, 255);
                case 'w':
                    return new Color32(255, 255, 255, 255);
                case 'm':
                    return new Color32(204, 94, 104, 255);
                case 'J':
                    return new Color32(62, 96, 166, 255);
                case 'j':
                    return new Color32(42, 66, 118, 255);
                case 'W':
                    return new Color32(252, 252, 248, 255);
                case 'P':
                    return new Color32(30, 29, 36, 255);
                case 'p':
                    return new Color32(52, 51, 62, 255);
                case 'K':
                    return new Color32(14, 13, 18, 255);
                default:
                    return new Color32(0, 0, 0, 0);
            }
        }
    }
}
