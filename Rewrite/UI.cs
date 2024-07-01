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
    public Dictionary<string, UIPage> Pages { get; set; }
    public UIPage currentPage { get; set; }

    /// <summary>
    /// An list of all the pages opened in order of opening 
    /// </summary>
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

    /// <summary>
    /// Create a page by providing a dictionary of selections that each page has, numbered in order of the dictionary.
    /// <br/>
    /// Can provide additional options for page like title colour and if it has a back button.
    /// </summary>
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

    /// <summary>
    /// Navigate to a specific page using its name.
    /// <br/>
    /// If page is not open it will be opened on the next column, otherwise it will be navigated to.
    /// <br/>
    /// If page is not found it will throw an error.
    /// </summary>
    public void NavigatePage(string Page)
    {
        if (ended) return;

        if (!Pages.ContainsKey(Page))
        {
            throw new Exception("Page does not exist");
        }

        currentPage = Pages[Page];
        UIWriter line;

        if (OpenPages.Contains(Page)) // going back page(s)
        {
            int Last = OpenPages.IndexOf(Page);
            for (int i = OpenPages.Count - 1; i > Last; i--)
            {
                ClosePage(i);
            }
            // title
            line = new UIWriter(this);
            line.Out("<- " + Pages[Page].name + " ->", Pages[Page].options.titleColour);
        }
        else // open new page
        {
            OpenPages.Remove("function");
            OpenPages.Add(Page);
            ClearPage(OpenPages.Count);

            // title
            line = new UIWriter(this);
            line.Out("<- " + Pages[Page].name + " ->", Pages[Page].options.titleColour);

            // list out options
            int index = 0;
            foreach (UISelection selection in Pages[Page].selections)
            {
                line.Out("[" + index + "] " + selection.name);
                index++;
            }
        }

        ChangeOtherCols(Page);
        line.Select();

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

    public void ClearPage(int col)
    {
        UIWriter clearer = new UIWriter(this, 0, col);
        int gap = Console.WindowWidth / MaxColumns;
        string EmptySpace = new string(' ', gap);
        for (int row = 0; row < Console.WindowHeight - 1; row++)
        {
            clearer.Out(EmptySpace);
        }
    }

    private void ChangeOtherCols(string curPage)
    {
        int gap = Console.WindowWidth / MaxColumns;
        for (int i = 1; i < OpenPages.IndexOf(curPage) + 1; i++)
        {
            UIWriter titleChanger = new UIWriter(this, 0, i);
            titleChanger.Out(new string(' ', gap));
            titleChanger.Row--;
            titleChanger.Out(OpenPages[i - 1], Pages[OpenPages[i - 1]].options.titleColour);
        }
    }

    private void ClosePage(int index)
    {
        UIWriter clearer = new UIWriter(this);
        int gap = Console.WindowWidth / MaxColumns;
        string EmptySpace = new string(' ', gap);
        for (int row = 0; row < Console.WindowHeight - 1; row++)
        {
            clearer.Out(EmptySpace);
        }
        OpenPages.RemoveAt(index);
    }

    /// <summary>
    /// Clears the console and any further actions
    /// </summary>
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

    public UIPage(UI ui, string name, Options options)
    {
        UIWriter lineWriter = new UIWriter(ui);
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


/// <summary>
/// Provides a way to write text to the console in a specific column, and other QOL methods</br>
/// Can use check method to check for bad input, and getlist to get a list of numbers
/// </summary>
public class UIWriter
{
    public int Row { get; set; }
    public int ColumnCoord { get; set; }
    private UI Ui;
    private bool HasBadInput = false;

    public UIWriter(UI ui, int StartRow = 0, int column = -1)
    {
        Row = StartRow;
        Ui = ui;
        ColumnCoord = ConvertColumn((column == -1 ? ui.OpenPages.Count : column) - 1);
    }

    /// <summary>
    /// Starts a new line and writes the text out, in the specified column of the instance
    /// </summary>
    public void Out(string line, ConsoleColor colour = ConsoleColor.Gray)
    {
        Console.SetCursorPosition(ColumnCoord, Row);
        Console.ForegroundColor = colour;
        Console.WriteLine(line);
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
    public void Check(string message)
    {
        Out(HasBadInput ? "Invalid input." : message);
    }

    private int GetIntInput(int top)
    {
        Console.SetCursorPosition(ColumnCoord, top);
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

    /// <summary>
    /// Waits for a user input, and runs the action associated with the integer pressed on the current page
    /// </summary>
    public void Select()
    {
        Console.SetCursorPosition(ColumnCoord, 0);
        var input = Console.ReadKey(true);
        string key = input.KeyChar.ToString();
        int selection = int.MinValue;

        try { selection = int.Parse(key); }
        catch
        {
            HandleError("Error occured.");
        }

        if (selection < 0 || selection >= Ui.currentPage.selections.Count)
        {
            HandleError("Invalid input.");
            return;
        }

        if (Ui.OpenPages.Count > Ui.MaxColumns)
        {
            HandleError("Too many pages, use Ctrl+C to end");
            return;
        }


        if (!Ui.OpenPages.Contains("function"))
        {
            Ui.OpenPages.Add("function");
        }
        else
        {
            Ui.ClearPage(Ui.OpenPages.Count);
        }
        Ui.currentPage.selections[selection].function();
        if (!Ui.ended) Select();
    }

    private void HandleError(string message)
    {
        Out(message);
        Row--;
        Select();
    }

    private int ConvertColumn(int col)
    {
        int gap = Console.WindowWidth / Ui.MaxColumns;
        return col * gap;
    }
}