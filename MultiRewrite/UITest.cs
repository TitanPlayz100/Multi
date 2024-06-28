using System;
using System.Collections.Generic;

public class UITest
{
    public static void Main()
    {
        UI ui = new UI("test", 3);

        // main page
        List<Option> options = new List<Option> {
            new Option("Exit", () => { Console.Clear(); Environment.Exit(0); }),
            new Option("Operations", () => { ui.NavigatePage("operationtest"); }),
            new Option("Algorithms", () => { ui.NavigatePage("algorithmtest"); })
        };

        ui.CreatePage("testPage", options, false);

        // operation test page
        List<Option> operationOptions = new List<Option> {
            new Option("Add", () => { new Writer(ui).Out("Result: "+AddTest(1, 2)); })
        };

        ui.CreatePage("operationtest", operationOptions);

        // algorithm test page
        List<Option> algorithmOptions = new List<Option> {
            new Option("Linear Search", () => { new Writer(ui).Out("Result: "+LinearSearchTest(10)); }),  
        };

        ui.CreatePage("algorithmtest", algorithmOptions);

        ui.NavigatePage("testPage");
    }

    public static int AddTest(int a, int b)
    {
        return a + b;
    }

    public static int LinearSearchTest(int a)
    {
        return a;
    }
}