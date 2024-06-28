using System;
using System.Collections.Generic;
using System.Linq;

public class UI
{
    public Dictionary<string, UIPage> Pages = new Dictionary<string, UIPage>();
    public UIPage currentPage;
    public List<string> OpenPages = new List<string>();
    public int MaxColumns;

    public UI(string WindowTitle, int MaxColumns)
    {
        Console.Clear();
        Console.Title = WindowTitle;
        this.MaxColumns = MaxColumns;
    }

    public void CreatePage(string name, List<Option> options, bool BackButton = true)
    {
        UIPage page = new UIPage(this, name);
        if (!BackButton) page.removeOption(0);
        page.SetOptionsList(options);
        Pages.Add(name, page);
    }

    public void CreatePage(string name, Dictionary<string, Action> options, bool BackButton = true)
    {
        List<Option> optionList = new List<Option>();
        foreach (var option in options)
        {
            optionList.Add(new Option(option.Key, option.Value));
        }
        CreatePage(name, optionList, BackButton);
    }

    public void NavigatePage(string Page)
    {
        if (OpenPages.Contains(Page))
        {
            int Last = OpenPages.IndexOf(Page);
            for (int i = OpenPages.Count - 1; i > Last; i--)
            {
                ClosePage(i);
            }
            OpenPages.Remove(Page);
            OpenPage(Page);
        }
        else
        {
            OpenPage(Page);
        }
    }

    public void NavigateBack()
    {
        if (OpenPages.Count == 1)
        {
            NavigatePage(OpenPages[0]);
            return;
        }
        string Page = OpenPages.Contains("function")
            ? OpenPages[OpenPages.Count - 3]
            : OpenPages[OpenPages.Count - 2];
        NavigatePage(Page);
    }

    private void OpenPage(string Page)
    {
        OpenPages.Remove("function");

        OpenPages.Add(Page);
        currentPage = Pages[Page];



        Writer line = new Writer(this);
        line.Out("<- " + Pages[Page].name + " ->", ConsoleColor.Cyan);

        int index = 0;
        foreach (Option option in Pages[Page].options)
        {
            line.Out("[" + index + "] " + option.name);
            index++;
        }

        int gap = Console.WindowWidth / MaxColumns;
        for (int i = 1; i < OpenPages.IndexOf(Page) + 1; i++)
        {
            Writer titleChanger = new Writer(this, 0, i);
            titleChanger.Out(new string(' ', gap));
            titleChanger.row--;
            titleChanger.Out(OpenPages[i-1], ConsoleColor.Cyan);
        }

        line.Select();
    }

    private void ClosePage(int index)
    {
        Writer clearer = new Writer(this);
        int gap = Console.WindowWidth / MaxColumns;
        string EmptySpace = new string(' ', gap);
        for (int row = 0; row < Console.WindowHeight - 1; row++)
        {
            clearer.Out(EmptySpace);
        }
        OpenPages.RemoveAt(index);
    }
}

public class UIPage
{
    // name cannot be "function" (special word representing something else)
    public string name;
    public List<Option> options = new List<Option>();
    public Writer line;

    public UIPage(UI ui, string name)
    {
        this.name = name;
        Writer lineWriter = new Writer(ui);
        AddOption("Back", () => { ui.NavigateBack(); });
    }

    public int AddOption(string name, Action function)
    {
        Option option = new Option(name, function);
        options.Add(option);
        return options.Count - 1;
    }

    public void removeOption(int index)
    {
        options.RemoveAt(index);
    }

    public void SetOptionsList(List<Option> options)
    {
        this.options = this.options.Concat(options).ToList();
    }
}

public class Option
{
    // name cannot be "back"
    public string name;
    public Action function;

    public Option(string name, Action function)
    {
        this.name = name;
        this.function = function;
    }
}

public class Writer
{
    public int row;
    public int columnCoord;
    private UI ui;
    private bool HasBadInput = false;

    public Writer(UI ui, int StartRow = 0, int column = -1)
    {
        row = StartRow;
        this.ui = ui;
        columnCoord = ConvertColumn((column == -1 ? ui.OpenPages.Count : column) - 1);
    }

    public void Out(string line, ConsoleColor colour = ConsoleColor.Gray)
    {
        Console.SetCursorPosition(columnCoord, row);
        Console.ForegroundColor = colour;
        Console.WriteLine(line);
        Console.ResetColor();
        row++;
    }

    public int Get(bool ignoreErr = false)
    {
        row++;
        int input = GetIntInput(row - 1);
        if (input == int.MinValue && !ignoreErr) HasBadInput = true;
        return input;
    }

    public int[] GetList()
    {
        List<int> nums = new List<int>();
        int term = Get(true);
        while (term != int.MinValue)
        {
            nums.Add(term);
            term = Get(true);
        }
        return nums.ToArray();
    }

    public void Err(Action passedFunc)
    {
        if (HasBadInput)
        {
            Out("Invalid input.");
            return;
        }
        passedFunc();
    }

    private int GetIntInput(int top)
    {
        Console.SetCursorPosition(columnCoord, top);
        var input = Console.ReadLine();
        if (input == null) return int.MinValue;
        if (input == string.Empty) return int.MinValue;

        try
        {
            return int.Parse(input);
        }
        catch
        {
            return int.MinValue;
        }
    }

    public void Select()
    {
        Console.SetCursorPosition(columnCoord, 0);
        var input = Console.ReadKey(true);
        string key = input.KeyChar.ToString();
        int option = int.MinValue;

        try { option = int.Parse(key); }
        catch
        {
            HandleError("Error occured.");
        }

        if (option < 0 || option >= ui.currentPage.options.Count)
        {
            HandleError("Invalid input.");
            return;
        }

        if (ui.OpenPages.Count - 1 > ui.MaxColumns)
        {
            HandleError("Too many pages!");
            return;
        }

        if (!ui.OpenPages.Contains("function")) ui.OpenPages.Add("function");
        ui.currentPage.options[option].function();
        Select();
    }

    private void HandleError(string message)
    {
        Out(message);
        row--;
        Select();
    }

    private int ConvertColumn(int col)
    {
        int gap = Console.WindowWidth / ui.MaxColumns;
        return col * gap;
    }
}