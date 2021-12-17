using System;
using System.Diagnostics;

namespace FinnFemFel
{
    internal class Program
    {
        static void Main()
        {
            var expectedResult = "FOXXJCF";

            var datetime = DateTime.Now;

            int inputInt = 1024431;
            var (result, stored) = Calculator<int>.Calculate(inputInt, Unit.Normal, datetime);
            EvaluateResult("Initial", expectedResult, false, result, stored);

            decimal inputDecimal = 1024.4305M;
            (result, stored) = Calculator<decimal>.Calculate(inputDecimal, Unit.Milli, datetime);
            EvaluateResult("Decimal", expectedResult, true, result, stored);

            float inputFloat = 1024.4305F;
            (result, stored) = Calculator<float>.Calculate(inputFloat, Unit.Milli, datetime);
            EvaluateResult("Float", expectedResult, true, result, stored);

            double inputDouble = 1024.4305D;
            (result, stored) = Calculator<double>.Calculate(inputDouble, Unit.Milli, datetime);
            EvaluateResult("Double", expectedResult, true, result, stored);

            var laterDatetime = datetime.AddHours(1);
            (result, stored) = Calculator<int>.Calculate(inputInt, Unit.Normal, laterDatetime);
            EvaluateResult("Later", expectedResult, false, result, stored);

            Console.WriteLine("Press Any Key To Exit...");
            Console.Read();
        }

        private static void EvaluateResult(string name, string expectedResult, bool expectedStored, string actualResult, bool actualStored)
        {
            if (expectedResult != actualResult)
            {
                Console.WriteLine($"{name,-20}WRONG RESULT! - {actualResult}"); // 🤬
            }
            else if (expectedStored && !actualStored)
            {
                Console.WriteLine($"{name,-20}SLOW, DIDN'T USE THE STORED RESULT!"); // 🥱
            }
            else
            {
                Console.WriteLine($"{name,-20}CORRECT!"); // 😎
            }
        }
    }
}
