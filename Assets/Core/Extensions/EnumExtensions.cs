using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Возвращает все значения enum в виде List<T>.
        /// </summary>
        public static List<T> GetAllEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// Возвращает случайное значение из любого enum, с опцией исключить конкретные значения.
        /// </summary>
        public static T GetRandomEnum<T>(params T[] excluded) where T : Enum
        {
            var values = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(v => excluded == null || !excluded.Contains(v))
                .ToArray();

            if (values.Length == 0)
            {
                throw new InvalidOperationException($"Нет допустимых значений для {typeof(T).Name} после исключения.");
            }

            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        /// <summary>
        /// Возвращает все значения enum в виде List<T>, исключая указанные значения.
        /// </summary>
        public static List<T> GetAllEnumValuesExcept<T>(params T[] excluded) where T : Enum
        {
            var excludedSet = new HashSet<T>(excluded);
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(value => !excludedSet.Contains(value))
                .ToList();
        }

        /// <summary>
        /// Возвращает случайно перемешанные значения enum'а, исключая указанные.
        /// </summary>
        public static List<T> GetRandomizedEnumValuesExcept<T>(params T[] excluded) where T : Enum
        {
            var excludedSet = new HashSet<T>(excluded);
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(value => !excludedSet.Contains(value))
                .OrderBy(_ => UnityEngine.Random.value)
                .ToList();
        }
    }
}