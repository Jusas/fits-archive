using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FitsArchiveUI.Utils
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
        {
            var types = assembly.GetTypes();
            var validTypes = new List<Type>();
            foreach (var t in types)
            {
                var baseType = t.BaseType;
                var interfaces = t.GetInterfaces();
                bool isMatch =
                    interfaces.Any(i => i.IsGenericType && openGenericType.IsAssignableFrom(i.GetGenericTypeDefinition()));
                if (!isMatch)
                    isMatch = baseType != null && baseType.IsGenericType &&
                              openGenericType.IsAssignableFrom(baseType.GetGenericTypeDefinition());
                if (isMatch && !t.IsAbstract)
                    validTypes.Add(t);

            }

            return validTypes;
        }
    }
}