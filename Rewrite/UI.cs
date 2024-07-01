using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class UI : ConsoleUI
{
    public Dictionary<string, UIPage> Pages { get; set; }
    public UIPage currentPage { get; set; }
    public List<string> OpenPages { get; set; }
    public int MaxColumns { get; set; }
    public bool ended { get; set; }

    public UI(string WindowTitle, int MaxColumns)
    {
        Console.Clear();
        Console.Title = WindowTitle;
        this.MaxColumns = MaxColumns;
        Pages = new Dictionary<string, UIPage>();
        OpenPages = new List<string>();
        ended = false;
    }

    public void CreatePage(string name, Dictionary<string, Action> selections, Options options = null)
    {
        if (options == null) options = new Options();
        List<UISelection> selectList = new List<UISelection>();
        foreach (var selection in selections)
        {
            selectList.Add(new UISelection { name = selection.Key, function = selection.Value });
        }
        UIPage page = new UIPage(this, name, options);
        if (!options.HasBackButton) page.RemoveSelection(0);
        page.SetSelections(selectList);
        Pages.Add(name, page);
    }

    public void NavigatePage(string Page)
    {
        if (ended) return;
        if (OpenPages.Contains(Page))
        {
            int Last = OpenPages.IndexOf(Page);
            for (int i = OpenPages.Count - 1; i > Last; i--)
            {
                ClosePage(i);
            }
            OpenPages.Remove(Page);
        }

        OpenPage(Page);
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

    // add function to clear current column

    public void ClearPage(int col)
    {
        Writer clearer = new UIWriter(this, 0, col);
        int gap = Console.WindowWidth / MaxColumns;
        string EmptySpace = new string(' ', gap);
        for (int row = 0; row < Console.WindowHeight - 1; row++)
        {
            clearer.Out(EmptySpace);
        }
    }

    private void OpenPage(string Page)
    {
        if (!Pages.ContainsKey(Page))
        {
            throw new Exception("Page does not exist");
        }
        OpenPages.Remove("function");
        OpenPages.Add(Page);
        currentPage = Pages[Page];

        ClearPage(OpenPages.Count);

        // title
        Writer line = new UIWriter(this);
        line.Out("<- " + Pages[Page].name + " ->", Pages[Page].options.titleColour);

        // list out options
        int index = 0;
        foreach (UISelection selection in Pages[Page].selections)
        {
            line.Out("[" + index + "] " + selection.name);
            index++;
        }

        // change how other columns look
        int gap = Console.WindowWidth / MaxColumns;
        for (int i = 1; i < OpenPages.IndexOf(Page) + 1; i++)
        {
            Writer titleChanger = new UIWriter(this, 0, i);
            titleChanger.Out(new string(' ', gap));
            titleChanger.row--;
            titleChanger.Out(OpenPages[i - 1], Pages[OpenPages[i - 1]].options.titleColour);
        }

        line.Select();
    }

    private void ClosePage(int index)
    {
        Writer clearer = new UIWriter(this);
        int gap = Console.WindowWidth / MaxColumns;
        string EmptySpace = new string(' ', gap);
        for (int row = 0; row < Console.WindowHeight - 1; row++)
        {
            clearer.Out(EmptySpace);
        }
        OpenPages.RemoveAt(index);
    }

    public void ClearConsole()
    {
        ended = true;
        Console.Clear();
    }
}

public class UIPage
{
    // name cannot be "function" (special word representing something else)
    public List<UISelection> selections = new List<UISelection>();

    public Options options;

    public string name;

    public UIPage(ConsoleUI ui, string name, Options options)
    {
        Writer lineWriter = new UIWriter(ui);
        AddSelection("Back", () => { ui.NavigateBack(); });
        this.options = options;
        this.name = name;
    }

    public int AddSelection(string name, Action function)
    {
        UISelection selection = new UISelection { name = name, function = function };
        selections.Add(selection);
        return selections.Count - 1;
    }

    public void RemoveSelection(int index)
    {
        selections.RemoveAt(index);
    }

    public void SetSelections(List<UISelection> options)
    {
        selections = selections.Concat(options).ToList();
    }
}

public class Options
{
    public ConsoleColor titleColour { get; set; }
    public bool HasBackButton { get; set; }

    public Options()
    {
        titleColour = ConsoleColor.Gray;
        HasBackButton = true;
    }
}

// name cannot be "back"
public class UISelection
{
    public string name { get; set; }
    public Action function { get; set; }
}

public class UIWriter : Writer
{
    public int row { get; set; }
    public int columnCoord { get; set; }
    private ConsoleUI ui;
    private bool HasBadInput = false;

    public UIWriter(ConsoleUI ui, int StartRow = 0, int column = -1)
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

    public void Check(string message)
    {
        Out(HasBadInput ? "Invalid input." : message);
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
        int selection = int.MinValue;

        try { selection = int.Parse(key); }
        catch
        {
            HandleError("Error occured.");
        }

        if (selection < 0 || selection >= ui.currentPage.selections.Count)
        {
            HandleError("Invalid input.");
            return;
        }

        if (ui.OpenPages.Count > ui.MaxColumns)
        {
            HandleError("Too many pages, use Ctrl+C to end");
            return;
        }


        if (!ui.OpenPages.Contains("function"))
        {
            ui.OpenPages.Add("function");
        }
        else
        {
            ui.ClearPage(ui.OpenPages.Count);
        }
        ui.currentPage.selections[selection].function();
        if (!ui.ended) Select();
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