using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/// <summary>
/// This provides additional comments and intellisense to each method for a better understanding. <br/>
/// This class is used to create a console UI with multiple columns (pages) and write easily to each page
/// </summary>
public class UI
{
    /// <summary>
    /// A dictionary of all the pages in the UI, the key is the name of the page
    /// </summary>
    public Dictionary<string, UIPage> Pages;
    public UIPage currentPage;
    public List<string> OpenPages;
    public int MaxColumns;
    public bool Ended;
    public IDisplay Style;

    public UI(string WindowTitle, int MaxColumns, IDisplay style = null)
    {
        Console.Clear();
        Console.Title = WindowTitle;
        this.MaxColumns = MaxColumns;
        Pages = new Dictionary<string, UIPage>();
        OpenPages = new List<string>();
        Ended = false;
        Style = style ?? new UIDisplayDefault();
    }

    /// <summary>
    /// Create a page by providing a dictionary of selections that each page has, numbered in order of the dictionary.
    /// <br/>
    /// Can provide additional options for page like title colour and if it has a back button.
    /// </summary>
    public void CreatePage(string name, Dictionary<string, Action> selections, Options options = null)
    {
        options = options ?? new Options();
        UIPage page = new UIPage(this, name, options);

        foreach (var selection in selections)
        {
            page.AddSelection(selection.Key, selection.Value);
        }

        if (!options.HasBackButton) page.RemoveSelection(0);
        Pages.Add(name, page);
    }

    /// <summary>
    /// Navigate to a specific page using its name.
    /// <br/>
    /// If page is not open it will be opened on the next column, otherwise it will be navigated to.
    /// <br/>
    /// If page is not found it will throw an error.
    /// </summary>
    public void NavigatePage(string Page)
    {
        if (Ended) return;

        if (!Pages.ContainsKey(Page))
        {
            throw new Exception("Page "+ Page +" does not exist");
        }

        currentPage = Pages[Page];

        if (OpenPages.Contains(Page))
        {
            Style.NavigateBackPage(this, Page);
        }
        else
        {
            OpenPages.Remove("function");
            OpenPages.Add(Page);
            Style.NavigateForwardPage(this, Page);
        }
    }

    /// <summary>
    /// Navigate back to the previous page, unless it is the first page
    /// </summary>
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

    /// <summary>
    /// Clears the console and any further actions
    /// </summary>
    public void ClearConsole()
    {
        Ended = true;
        Console.Clear();
    }
}

public class UIPage
{
    // name cannot be "function" (special word representing something else)
    public List<UISelection> selections = new List<UISelection>();
    public Options options;
    public string name;

    public UIPage(UI ui, string name, Options options)
    {
        AddSelection("Back", () => { ui.NavigateBack(); });
        this.options = options;
        this.name = name;
    }

    public void AddSelection(string name, Action function)
    {
        UISelection selection = new UISelection { name = name, function = function };
        selections.Add(selection);
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
    public ConsoleColor titleColour;
    public bool HasBackButton;

    public Options()
    {
        titleColour = ConsoleColor.Gray;
        HasBackButton = true;
    }
}

public class UISelection
{
    // name cannot be "back"
    public string name;
    public Action function;
}


/// <summary>
/// Provides a way to write text to the console in a specific column, and other QOL methods</br>
/// Can use check method to check for bad input, and getlist to get a list of numbers
/// </summary>
public class UIWriter
{
    public int Row;
    public int ColumnCoord;
    private UI ui;
    private bool HasBadInput = false;

    public UIWriter(UI ui, int StartRow = 0, int? column = null)
    {
        Row = StartRow;
        this.ui = ui;
        if (ui.Style is UIDisplayExplorer) column = 1;
        ColumnCoord = ConvertColumn((column ?? ui.OpenPages.Count) - 1);
    }

    /// <summary>
    /// Starts a new line and writes the text out, in the specified column of the instance
    /// </summary>
    public void Out(string text, ConsoleColor colour = ConsoleColor.Gray)
    {
        Console.SetCursorPosition(ColumnCoord, Row);
        Console.ForegroundColor = colour;
        Console.WriteLine(text); // change to Write
        Console.ResetColor();
        Row++;
        if (Row > Console.WindowHeight) Row = 1;
    }

    /// <summary>
    /// Starts a new line and returns the users integer input after they press enter
    /// </summary>
    public int Get(bool ignoreErr = false)
    {
        Row++;
        if (Row > Console.WindowHeight) Row = 1;
        int input = GetIntInput(Row - 1);
        if (input == int.MinValue && !ignoreErr) HasBadInput = true;
        return input;
    }

    /// <summary>
    /// Starts a new line for each user input, until they input a non integer, and returns the array of integers
    /// </summary>
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

    /// <summary>
    /// Will use Out method if there was no bad input, otherwise will output error message
    /// </summary>
    public void Check(string text)
    {
        Out(HasBadInput ? "Invalid input." : text);
    }

    private int GetIntInput(int top)
    {
        Console.SetCursorPosition(ColumnCoord, top);
        var input = Console.ReadLine();
        if (input == null || input == string.Empty) return int.MinValue;
        try
        {
            return int.Parse(input);
        }
        catch
        {
            return int.MinValue;
        }
    }

    /// <summary>
    /// Waits for a user input, and runs the action associated with the integer pressed on the current page
    /// </summary>
    public void Select()
    {
        ui.Style.StartSelect(ui, this);
        var input = Console.ReadKey(true);
        string key = input.KeyChar.ToString();
        int selection = int.MinValue;

        try { selection = int.Parse(key); }
        catch
        {
            HandleError("Invalid input.");
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


        ui.Style.ClearPage(ui, ui.OpenPages.Count);

        ui.currentPage.selections[selection].function();
        if (ui.OpenPages.Contains("function"))
        {
            UIWriter line = new UIWriter(ui, Console.WindowHeight - 3);
            line.Out("Press any key...", ConsoleColor.Red);
            line.Get();
        }

        if (!ui.Ended) ui.Style.NavigateBackPage(ui, ui.currentPage.name);
    }

    private void HandleError(string message)
    {
        Out(message);
        Row--;
        Select();
    }

    private int ConvertColumn(int col)
    {
        int gap = Console.WindowWidth / ui.MaxColumns;
        return col * gap;
    }
}