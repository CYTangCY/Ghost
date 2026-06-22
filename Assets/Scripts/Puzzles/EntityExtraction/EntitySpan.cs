using System;

namespace Ghost.Puzzles.EntityExtraction
{
    [Serializable]
    public sealed class EntitySpan : IEquatable<EntitySpan>
    {
        public EntitySpan(int start, int length, EntityType type)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), "Entity span start cannot be negative.");
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Entity span length must be greater than zero.");
            }

            Type = type ?? throw new ArgumentNullException(nameof(type));
            Start = start;
            Length = length;
        }

        public int Start { get; }

        public int Length { get; }

        public EntityType Type { get; }

        public string GetText(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (Start > message.Length || Length > message.Length - Start)
            {
                throw new ArgumentException("Entity span falls outside the supplied message.", nameof(message));
            }

            return message.Substring(Start, Length);
        }

        public bool Equals(EntitySpan other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Start == other.Start
                && Length == other.Length
                && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntitySpan);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start;
                hashCode = (hashCode * 397) ^ Length;
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Start {Start}, Length {Length}, Type {Type}";
        }

        public static bool operator ==(EntitySpan left, EntitySpan right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(EntitySpan left, EntitySpan right)
        {
            return !(left == right);
        }
    }
}
