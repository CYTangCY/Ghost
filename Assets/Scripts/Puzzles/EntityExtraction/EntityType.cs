using System;

namespace Ghost.Puzzles.EntityExtraction
{
    public enum EntityCategory
    {
        System,
        Custom
    }

    [Serializable]
    public sealed class EntityType : IEquatable<EntityType>
    {
        public EntityType(string id, EntityCategory category)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Entity type id cannot be empty.", nameof(id));
            }

            Id = id;
            Category = category;
        }

        public string Id { get; }

        public EntityCategory Category { get; }

        public bool Equals(EntityType other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                && Category == other.Category;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.Ordinal.GetHashCode(Id) * 397) ^ (int)Category;
            }
        }

        public override string ToString()
        {
            return $"{Id} ({Category})";
        }

        public static bool operator ==(EntityType left, EntityType right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(EntityType left, EntityType right)
        {
            return !(left == right);
        }
    }
}
