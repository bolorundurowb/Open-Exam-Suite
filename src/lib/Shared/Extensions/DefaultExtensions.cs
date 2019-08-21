using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Extensions
{
    public static class DefaultExtensions
    {
        public static bool IsDefault(this Guid instance)
        {
            return instance == default(Guid);
        }
        public static bool IsNotDefault(this Guid instance)
        {
            return !IsDefault(instance);
        }
    }
}
