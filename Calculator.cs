using System;
using System.Collections.Generic;
using System.Linq;

namespace FinnFemFel
{
    public static class Calculator<T>
    {
        private static readonly Dictionary<int, (DateTime Expires, List<List<int>> Result)> StoredResults =
                            new Dictionary<int, (DateTime Expires, List<List<int>> Result)>();

        /// <summary>
        /// Applies XXXXX algoritm on <paramref name="value"/>.
        /// Since the algortim is slow, previous values are stored to speed up calculations
        /// </summary>
        /// <param name="value">
        /// Value to calculate on<br/>
        /// When using values with a decimal point, to preserve as much pression as possible, prefer to supply them in milli-units (1000 times smaller) and set <paramref name="Unit"/> to <c>Milli</c>
        /// </param>
        /// <param name="unit">Tells if <paramref name="value"/> is in units 1000 times smaller</param>
        /// <param name="dateTime">The time when the calculation is made (most often it should be the current time)</param>
        /// <returns></returns>
        public static (string Result, bool UsedStoredResult) Calculate(T value, Unit unit, DateTime dateTime)
        {
            var integer = RoundValueToClosestInteger(value, unit);

            var calculatedResult = GetStoredResult(integer, dateTime);

            if (calculatedResult != null)
            {
                return (FormatResult(calculatedResult), true);
            }

            // Value isnt stored or it has expired, use the slow algoritm to calculate it
            calculatedResult = ApplySlowAlgorithm(integer);
            SaveResult(integer, calculatedResult);
            return (FormatResult(calculatedResult), false);
        }

        private static int RoundValueToClosestInteger(T value, Unit unit)
        {
            var multiplier = unit.HasFlag(Unit.Milli) ? 1000 : 1;
            var integer = value switch
            {
                int i => i * multiplier,
                decimal d => (int)Math.Round(d * multiplier),
                float f => (int)Math.Round(f * multiplier),
                double d => (int)Math.Round(d * multiplier),
                _ => throw new Exception($"Value is of an unsupported type: {typeof(T).Name}")
            };
            return integer;
        }

        private static List<List<int>> ApplySlowAlgorithm(int integer)
        {
            var result = new List<List<int>>();

            var numerator = integer;
            while (numerator > 0)
            {
                var remainder = numerator % 10;
                List<int> list = null;
                if (remainder <= 3)
                {
                    list = Enumerable.Range(2, remainder + 1).ToList();
                }
                result.Add(list);
                numerator /= 10;
            }

            return result;
        }

        private static string FormatResult(List<List<int>> calculatedResult)
        {
            // Examples:
            // null    => X
            // <empty> => X
            // 2,3     => F
            // 2,3,4   => J
            static char ConvertToLetter(List<int> list)
            {
                if (list?.Any() != false)
                {
                    var sum = list?.Sum() ?? 0;
                    var letter = (char)(sum + 'A');
                    return letter;
                }
                return 'X';
            }

            var formatedResult = "";

            foreach (var list in calculatedResult)
            {
                var letter = ConvertToLetter(list);
                formatedResult += letter;
            }

            return formatedResult;
        }

        private static List<List<int>> GetStoredResult(int integer, DateTime dateTime)
        {
            if (StoredResults.TryGetValue(integer, out var stored))
            {
                if (stored.Expires > dateTime)
                {
                    // Result is still actual
                    return stored.Result;
                }
                else
                {
                    // Result has expired, remove it
                    StoredResults.Remove(integer);
                }
            }
            return null;
        }

        private static void SaveResult(int integer, List<List<int>> result)
        {
            const int TtlInMinutes = 10;
            var expires = DateTime.UtcNow.AddMinutes(TtlInMinutes);
            StoredResults.Add(integer, (expires, result));
        }
    }
}
