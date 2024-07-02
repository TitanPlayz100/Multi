using System;
using System.Linq;

public class UnitTests
{
    public static void Main()
    {
        int[] test1 = OperationTest();
        OutputResult("Operations test", test1[0], test1[1]);

        int[] test2 = AlgorithmsTest();
        OutputResult("Algorithms test", test2[0], test2[1]);

        int[] test3 = CalculationsTest();
        OutputResult("Calculators test", test3[0], test3[1]);
    }

    private static void OutputResult(string text, int passed, int total)
    {
        Console.ForegroundColor = (passed == 0) ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(text + ": " + passed + "/" + total + " passed");
        Console.ResetColor();
    }

    public static int[] OperationTest()
    {
        bool[] tests = new bool[] {
            Operations.AddFunc(1, 2) == 3,
            Operations.SubtractFunc(4, 2) == 2,
            Operations.MultiplyFunc(1, 2) == 2,
            Operations.DivideFunc(4, 2) == 2,
            Operations.PythagorasFunc(1, 2, 3) == 3,
        };

        // returns count of passed tests, and total tests in an array
        return new int[] { tests.Count(c => c), tests.Length };
    }

    public static int[] AlgorithmsTest()
    {
        bool[] tests = new bool[] {
            Algorithms.LinearSearch(1) == 1,
            Algorithms.SortingAlgorithmFast(new int[] { 3, 2, 1 }) == new int[] { 1, 2, 3 },
            Algorithms.LinearSort(new int[] { 1, 2, 3 }) == new int[] { 1, 2, 3 },
            Algorithms.MaxFunc(new int[] { 1, 2, 3 }) == 3,
            Algorithms.MinFunc(new int[] { 1, 2, 3 }) == 3
        };

        // returns count of passed tests, and total tests in an array
        return new int[] { tests.Count(c => c), tests.Length };
    }

    public static int[] CalculationsTest()
    {
        bool[] tests = new bool[] {
            Calculators.BMIcalculator(1, 1) == "Skinny",
        };

        // returns count of passed tests, and total tests in an array
        return new int[] { tests.Count(c => c), tests.Length };
    }
}