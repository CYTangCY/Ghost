using UnityEngine;

namespace Ghost.Presentation.Act2EntityExtraction
{
    public sealed class Act2EntityChipView : MonoBehaviour
    {
        public int Start;
        public int Length;
        public string Text;

        public void Configure(int start, int length, string text)
        {
            Start = start;
            Length = length;
            Text = text ?? string.Empty;
        }
    }
}
