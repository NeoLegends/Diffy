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
        [Conditional("REQUIRES_CONDITION")]
#if NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Condition(bool value, string paramName, string message = null)
        {
            if (!value) {
                throw new ArgumentException(!string.IsNullOrEmpty(message) ? message : "A precondition was not met.", paramName);
            }
        }

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
    }
}
