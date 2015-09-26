using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffy
{
    /// <summary>
    /// Represents a longest common subsequence.
    /// </summary>
    public struct LongestCommonSubsequenceResult : IEquatable<LongestCommonSubsequenceResult>
    {
        /// <summary>
        /// Indicates whether a longest common subsequence could be found.
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// The length of the subsequence.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The starting position in the source collection.
        /// </summary>
        public int PositionInFirstCollection { get; private set; }

        /// <summary>
        /// The starting position in the destination collection.
        /// </summary>
        public int PositionInSecondCollection { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="LongestCommonSubsequenceResult"/> representing a found sequence.
        /// </summary>
        /// <param name="positionInFirstCollection">The starting position in the source collection.</param>
        /// <param name="positionInSecondCollection">The starting position in the destination collection.</param>
        /// <param name="length">The length of the subsequence.</param>
        public LongestCommonSubsequenceResult(int positionInFirstCollection, int positionInSecondCollection, int length)
            : this(true, positionInFirstCollection, positionInSecondCollection, length)
        { }

        /// <summary>
        /// Initializes a new <see cref="LongestCommonSubsequenceResult"/> representing a found sequence.
        /// </summary>
        /// <param name="isSuccess">Indicates whether a longest common subsequence could be found.</param>
        /// <param name="positionInFirstCollection">The starting position in the source collection.</param>
        /// <param name="positionInSecondCollection">The starting position in the destination collection.</param>
        /// <param name="length">The length of the subsequence.</param>
        public LongestCommonSubsequenceResult(bool isSuccess, int positionInFirstCollection, int positionInSecondCollection, int length)
            : this()
        {
            this.IsSuccess = isSuccess;
            this.PositionInFirstCollection = positionInFirstCollection;
            this.PositionInSecondCollection = positionInSecondCollection;
            this.Length = length;
        }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone() => new LongestCommonSubsequenceResult(this.IsSuccess, this.PositionInFirstCollection, this.PositionInSecondCollection, this.Length);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns><c>true</c> if both objects are equal, otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return (obj is LongestCommonSubsequenceResult) ? this.Equals((LongestCommonSubsequenceResult)obj) : false;
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><c>true</c> if both objects are equal, otherwise <c>false</c>.</returns>
        public bool Equals(LongestCommonSubsequenceResult other)
        {
            return (this.IsSuccess == other.IsSuccess) && (this.PositionInFirstCollection == other.PositionInFirstCollection) &&
                   (this.PositionInSecondCollection == other.PositionInSecondCollection) && (this.Length == other.Length);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked {
                int hash = 29;
                hash = hash * 486187739 + this.IsSuccess.GetHashCode();
                hash = hash * 486187739 + this.PositionInFirstCollection.GetHashCode();
                hash = hash * 486187739 + this.PositionInSecondCollection.GetHashCode();
                hash = hash * 486187739 + this.Length.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Converts the <see cref="LongestCommonSubsequenceResult"/> into a string.
        /// </summary>
        /// <returns>The conversion result.</returns>
        public override string ToString() => this.IsSuccess ?
            $"LCS ({this.PositionInFirstCollection}, {this.PositionInSecondCollection}, x{this.Length}" :
            "LCS -";

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if both objects are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(LongestCommonSubsequenceResult left, LongestCommonSubsequenceResult right) => left.Equals(right);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if both objects are inequal, otherwise <c>false</c>.</returns>
        public static bool operator !=(LongestCommonSubsequenceResult left, LongestCommonSubsequenceResult right) => !(left == right);
    }
}
