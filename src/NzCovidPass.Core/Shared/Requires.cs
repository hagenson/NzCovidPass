using System;
using System.Diagnostics;

namespace NzCovidPass.Core.Shared
{
    internal static class Requires
    {
        [DebuggerStepThrough]
        public static T NotNull<T>(T value, string paramName = null)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);

            return value;
        }
    }
}
