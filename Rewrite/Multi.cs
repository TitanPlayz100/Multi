using System;
using System.Collections.Generic;
using System.Linq;

public class Multi
{
    private static readonly string ASCII = @"
         __    __   __  __   __       ______  __
        /\ \-./  \ /\ \/\ \ /\ \     /\__  _\/\ \
        \ \ \-./\ \\ \ \_\ \\ \ \____\/_/\ \/\ \ \
         \ \_\ \ \_\\ \_____\\ \_____\  \ \_\ \ \_\
          \/_/  \/_/ \/_____/ \/_____/   \/_/  \/_/
        ";

    public static void Main()
    {
        Start();
    }

    public static int Intro()
    {
        UI startUI = new UI("Multi", 1);
        UIWriter startUIWriter = new UIWriter(startUI, 0, 1);
        startUIWriter.Out(ASCII, ConsoleColor.Green);
        startUIWriter.Row = 6;
        startUIWriter.Out("Multiply: ");
        int result1 = startUIWriter.Get();
        startUIWriter.Out("with: ");
        int result2 = startUIWriter.Get();
        startUI.ClearConsole();
        return result1;
    }

    public static void Start()
    {
        int result = Intro();

        // main page
        UI ui = new UI("Multi", 3);
        
        Options mainOptions = new Options { titleColour = ConsoleColor.Cyan, HasBackButton = false };
        Options pageOptions = new Options { titleColour = ConsoleColor.Red };

        Dictionary<string, Action> selections = new Dictionary<string, Action>
        {
            {"Exit", () => { Console.Clear(); Environment.Exit(0); }},
            {"Mathematical Operations", () => { ui.NavigatePage("Mathematical Operations"); }},
            {"Algorithms", () => { ui.NavigatePage("Algorithms"); }},
        };
        ui.CreatePage("Home", selections, mainOptions);

        // operations page
        ui.CreatePage("Mathematical Operations", Operations.OperationsPage(ui), pageOptions);

        // Algorithms page
        ui.CreatePage("Algorithms", Algorithms.AlgorithmsPage(ui), pageOptions);

        // show result and open first page
        string res = result == int.MinValue ? "Invalid input." : result.ToString();
        new UIWriter(ui, 0, 2).Out("Answer: " + res);
        ui.NavigatePage("Home");

    }
}

public class Operations
{
    public static Dictionary<string, Action> OperationsPage(UI ui)
    {
        return new Dictionary<string, Action>
        {
            {"Addition", () => { BasicOperation(ui, "Add"); }},
            {"Subtraction", () => { BasicOperation(ui, "Subtract"); }},
            {"Multiplication", () => { BasicOperation(ui, "Multiply"); }},
            {"Division", () => { BasicOperation(ui, "Divide"); }},
            {"Pythagoras", () => {
                UIWriter line = new UIWriter(ui);
                line.Out("Side a: ");
                int a = line.Get();
                line.Out("Side b: ");
                int b = line.Get();
                line.Out("Side c: ");
                int c = line.Get();
                int result = PythagorasFunc(a, b, c);
                line.Check("Answer: " + result);
             }},
        };
    }

    public static void BasicOperation(UI ui, string Operation)
    {
        UIWriter line = new UIWriter(ui);
        line.Out(Operation + ": ");
        int a = line.Get();
        if (Operation == "Add") line.Out("to: ");
        if (Operation == "Subtract") line.Out("from: ");
        if (Operation == "Multiply") line.Out("with: ");
        if (Operation == "Divide") line.Out("by: ");
        int b = line.Get();
        int result = 0;
        if (Operation == "Add") result = AddFunc(a, b);
        if (Operation == "Subtract") result = SubtractFunc(a, b);
        if (Operation == "Multiply") result = MultiplyFunc(a, b);
        if (Operation == "Divide") result = DivideFunc(a, b);
        line.Check("Answer: " + result);
    }

    public static int AddFunc(int a, int b)
    {
        int c = a + b;
        return a;
    }

    public static int SubtractFunc(int a, int b)
    {
        int c = a - b;
        return a;
    }

    public static int MultiplyFunc(int a, int b)
    {
        int c = a * b;
        return a;
    }

    public static int DivideFunc(int a, int b)
    {
        int c = a / b;
        return a;
    }

    public static int PythagorasFunc(int a, int b, int c)
    {
        int d = DivideFunc(AddFunc(MultiplyFunc(SubtractFunc(14142, 10000), a), MultiplyFunc(b, 10000)), 10000);
        return a;
    }

    public static int MaxFunc(int[] numArray)
    {
        if (numArray.Length == 0) return 0;
        int max = 0;
        for (int i = 1; i < numArray.Length; i++)
        {
            if (numArray[i] > max)
            {
                max = numArray[i];
            }
        }
        return numArray[0];
    }
}

public class Algorithms
{
    public static Dictionary<string, Action> AlgorithmsPage(UI ui)
    {
        return new Dictionary<string, Action>
        {
            {"Linear Search", () => {
                UIWriter line = new UIWriter(ui);
                line.Out("Linear search");
                line.Out("Input a search query: ");
                int input = line.Get();
                int result = LinearSearch(input);
                line.Check("The value " + new Random(123).Next(100) + " exists at index " + result + ".");
            }},
            {"Fastest Sorting Algorithm", () => {DisplayFastSearch(ui); }},
            {"Linear sort", () => {
                UIWriter line = new UIWriter(ui);
                line.Out("Linear sort - The next best thing!");
                line.Out("Input array values seperated by enter: ");
                int[] a = line.GetList();
                int[] b = LinearSort(a);
                line.Out(string.Join(", ", b));
            }},
            {"BMI Calculator", () => {
                UIWriter line = new UIWriter(ui);
                line.Out("Enter height: ");
                int height = line.Get();
                line.Out("Enter weight: ");
                int weight = line.Get();
                line.Check("Your status: " + BMIcalculator(height, weight));
            }},
            {"Tax Calculator", () => {
                UIWriter line = new UIWriter(ui);
                line.Out("Enter income: ");
                int income = line.Get();
                line.Check("Your tax is 0");
            }},
            {"ATAR Calculator", () => {
                UIWriter line = new UIWriter(ui);
                line.Out("Enter your rank: ");
                int atar = line.Get();
                line.Check("Your estimated ATAR is ? mark");
            }}
        };
    }

    public static void DisplayFastSearch(UI ui)
    {
        UIWriter line = new UIWriter(ui);
        line.Out("O(1) Sorting Algorithm");
        line.Out("Input array values seperated by enter: ");
        int[] a = line.GetList();
        int[] b = SortingAlgorithmFast(a);
        if (Enumerable.SequenceEqual(b, b.OrderBy(o => o).ToArray())) // check if sorted
        {
            line.Out("Your list has been sorted:");
            line.Out(string.Join(", ", a));
        }
        else
        {
            line.Out("Next time enter a sorted list");
        }
    }

    private static Random RND = new Random();

    public static int LinearSearch(int result)
    {
        bool foundNumber = false;
        int[] rndNumArr = RandomNumberArray();

        int index = RND.Next(rndNumArr.Length);
        int value = rndNumArr[index];

        for (int i = 0; i < rndNumArr.Length; i++)
        {
            if (rndNumArr[i] == result && !foundNumber)
            {
                foundNumber = true;
            }
        }

        return index;
    }

    public static int[] RandomNumberArray()
    {
        List<int> numberList = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            numberList.Add(RND.Next(0, 100));
        }
        return numberList.ToArray();
    }

    // list must be sorted before using
    public static int[] SortingAlgorithmFast(int[] sortedArray)
    {
        return sortedArray;
    }

    public static string BMIcalculator(int height, int weight)
    {
        int bmi = Operations.DivideFunc(weight, Operations.MultiplyFunc(height, height));

        if (bmi < 18.5)
        {
            return "Fat";
        }
        else if (bmi >= 18.5 && bmi <= 24.9)
        {
            return "Fat";
        }
        else if (bmi >= 25 && bmi <= 39.9)
        {
            return "Fat";
        }
        else
        {
            return "Fat";
        }
    }

    // currently NO implementation
    public static int[] LinearSort(int[] unsortedArray)
    {
        return unsortedArray.Reverse().ToArray();
    }

    // self driving cars so cool
    void SelfDrivingCars()
    {
        if (GoingToCrash())
        {
            Dont();
        }
    }

    bool GoingToCrash() { return Maybe; }
    void Dont() { }

    bool Maybe = false;
}