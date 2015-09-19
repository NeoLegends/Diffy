using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffy
{
    /// <summary>
    /// The type of <see cref="DiffSection"/>.
    /// </summary>
    public enum DiffSectionType
    {
        /// <summary>
        /// The elements must be copied.
        /// </summary>
        Copy,

        /// <summary>
        /// The elements must be inserted.
        /// </summary>
        Insert,

        /// <summary>
        /// The elements must be removed.
        /// </summary>
        Delete
    }

    /// <summary>
    /// Represents a part in a list diff.
    /// </summary>
    public struct DiffSection : IEquatable<DiffSection>
    {
        /// <summary>
        /// The length of the <see cref="DiffSection"/>.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The <see cref="DiffSectionType"/>.
        /// </summary>
        public DiffSectionType Type { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="DiffSectionType"/>.
        /// </summary>
        /// <param name="type">The <see cref="DiffSectionType"/>.</param>
        /// <param name="length">The length of the <see cref="DiffSection"/>.</param>
        public DiffSection(DiffSectionType type, int length)
            : this()
        {
            this.Length = length;
            this.Type = type;
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone() => new DiffSection(this.Type, this.Length);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns><c>true</c> if both objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is DiffSection) ? this.Equals((DiffSection)obj) : false;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><c>true</c> if both objects are equal, otherwise <c>false</c>.</returns>
        public bool Equals(DiffSection other) => (this.Length == other.Length) && (this.Type == other.Type);

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked {
                int hash = 29;
                hash = hash * 486187739 + this.Length.GetHashCode();
                hash = hash * 486187739 + this.Type.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Converts the <see cref="DiffSection"/> into a string.
        /// </summary>
        /// <returns>The conversion result.</returns>
        public override string ToString() => $"{this.Type} {this.Length}";

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if both objects are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(DiffSection left, DiffSection right) => left.Equals(right);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if both objects are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(DiffSection left, DiffSection right) => !(left == right);
    }
}
