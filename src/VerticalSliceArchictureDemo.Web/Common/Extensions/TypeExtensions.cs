using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VerticalSliceArchictureDemo.Web.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsOpenGeneric(this Type type)
            => type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;

        public static bool IsClosedTypeOf(this Type type, Type openGeneric)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (openGeneric == null)
            {
                throw new ArgumentNullException(nameof(openGeneric));
            }

            if (!openGeneric.IsOpenGeneric())
            {
                throw new ArgumentException("The specified openGeneric type is not a valid open generic.");
            }

            return type.GetTypesThatClose(openGeneric).Any();
        }

        public static IEnumerable<Type> GetTypesThatClose(this Type type, Type openGeneric)
            => FindAssignableTypesThatClose(type, openGeneric);

        private static IEnumerable<Type> FindAssignableTypesThatClose(Type candidateType, Type openGenericServiceType)
            => TypesAssignableFrom(candidateType).Where(t => t.IsClosedTypeOfImpl(openGenericServiceType));

        private static bool IsClosedTypeOfImpl(this Type type, Type openGeneric)
            => TypesAssignableFrom(type).Any(t => t.GetTypeInfo().IsGenericType && !type.GetTypeInfo().ContainsGenericParameters && t.GetGenericTypeDefinition() == openGeneric);

        private static IEnumerable<Type> TypesAssignableFrom(Type candidateType)
            => candidateType.GetTypeInfo().ImplementedInterfaces.Concat(Traverse.Across(candidateType, t => t.GetTypeInfo().BaseType));

        internal static class Traverse
        {
            public static IEnumerable<T> Across<T>(T first, Func<T, T> next) where T : class
            {
                var item = first;
                while (item != null)
                {
                    yield return item;
                    item = next(item);
                }
            }
        }
    }
}
