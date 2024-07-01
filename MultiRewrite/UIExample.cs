using System;
using System.Collections.Generic;
using System.Linq;

public class UITest
{
    public static ConsoleUI ui;
    public static void Main()
    {
        ui = new UI("test", 8);

        // page options
        Options mainPageOptions = new Options { titleColour = ConsoleColor.Red, HasBackButton = false };
        Options pageOptions = new Options { titleColour = ConsoleColor.Green };

        // main page
        Dictionary<string, Action> selections = new Dictionary<string, Action> {
            {"Exit", () => { Console.Clear(); Environment.Exit(0); }},
            {"Operations", () => { ui.NavigatePage("operationtest"); }},
            {"Algorithms", () => { ui.NavigatePage("algorithmtest"); }},
            {"pages in pages", () => { ui.NavigatePage("page2"); }}
        };

        ui.CreatePage("testPage", selections, mainPageOptions);

        // operation test page
        Dictionary<string, Action> operationSelections = new Dictionary<string, Action> {
            {"Another Page", () => { ui.NavigatePage("additionalPage"); }},
            {"Add", () => { Add(); }},
        };

        ui.CreatePage("operationtest", operationSelections, pageOptions);

        // additional page
        Dictionary<string, Action> additionalSelections = new Dictionary<string, Action> {
            {"Minus", () => { new UIWriter(ui).Out("Result: " + MinusTest(2, 1)); }}, // can inline functions as well
        };

        ui.CreatePage("additionalPage", additionalSelections); // options are optional

        // algorithm test page
        Dictionary<string, Action> algorithmSelections = new Dictionary<string, Action> {
            {"Linear Search", () => { LinearSearch(); }},
            {"new section", () => { ui.ClearConsole(); }}
        };

        ui.CreatePage("algorithmtest", algorithmSelections, pageOptions);

        // create loop of new pages
        for (int i = 2; i < 10; i++)
        {
            int next = i+1;
            Dictionary<string, Action> recursiveSelections = new Dictionary<string, Action> {
                {"Next Page", () => { ui.NavigatePage("page"+next); }},
            };

            ui.CreatePage("page"+i, recursiveSelections);
        }

        // open first page
        ui.NavigatePage("testPage");



        // further code after ClearConsole() is called
        Console.WriteLine("test");
        Console.Read();
    }

    // uses display writer
    public static void Add()
    {
        Writer writer = new UIWriter(ui);
        writer.Out("Input two numbers: ");
        int a = writer.Get();
        int b = writer.Get();
        writer.Check("Result: " + AddTest(a, b));
    }

    public static void LinearSearch()
    {
        new UIWriter(ui).Out("Result: " + LinearSearchTest(10));
    }

    // function implementations
    public static int AddTest(int a, int b)
    {
        return a + b;
    }

    public static int MinusTest(int a, int b)
    {
        return a - b;
    }

    public static int LinearSearchTest(int a)
    {
        return a;
    }
}