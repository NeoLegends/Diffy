using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diffy
{
    // We use a custom requires class instead of Code Contracts to avoid
    // the build speed penalty and verbosity.

    internal static class Requires
    {
        [Conditional("REQUIRES_FULL")]
        [Conditional("REQUIRES_NULL_CHECK")]
        [Conditional("REQUIRES_QUANTIFIERS")]
#if NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void NotNull<T>(T value, string paramName)
            where T : class
        {
            if (value == null) {
                throw new ArgumentNullException(paramName);
            }
        }

        [Conditional("REQUIRES_FULL")]
        [Conditional("REQUIRES_RANGE")]
#if NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Range(bool rangeCondition, string paramName, string message = null)
        {
            if (!rangeCondition) {
                throw new ArgumentOutOfRangeException(!string.IsNullOrEmpty(message) ? message : "A precondition was not met.", paramName);
            }
        }

    }
}
