using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The interface provides additional comments to each method for a better understanding. <br/>
/// This class is used to create a console UI with multiple columns (pages) and write easily to each page
/// </summary>
public interface ConsoleUI
{
    /// <summary>
    /// A dictionary of all the pages in the UI, the key is the name of the page
    /// </summary>
    Dictionary<string, UIPage> Pages { get; set; }

    bool ended { get; set; }

    UIPage currentPage { get; set; }

    /// <summary>
    /// An list of all the pages opened in order of opening 
    /// </summary>
    List<string> OpenPages { get; set; }

    int MaxColumns { get; set; }

    /// <summary>
    /// Create a page by providing a dictionary of selections that each page has, numbered in order of the dictionary.
    /// <br/>
    /// Can provide additional options for page like title colour and if it has a back button.
    /// </summary>
    void CreatePage(string name, Dictionary<string, Action> selections, Options options = null);

    /// <summary>
    /// Navigate to a specific page using its name.
    /// <br/>
    /// If page is not open it will be opened on the next column, otherwise it will be navigated to.
    /// <br/>
    /// If page is not found it will throw an error.
    /// </summary>
    void NavigatePage(string Page);

    /// <summary>
    /// Navigate back to the previous page, unless it is the first page
    /// </summary>
    void NavigateBack();

    /// <summary>
    /// Clears the console and any further actions
    /// </summary>
    void ClearConsole();

    void ClearPage(int col);
}

/// <summary>
/// Provides a way to write text to the console in a specific column, and other QOL methods</br>
/// Can use check method to check for bad input, and getlist to get a list of numbers
/// </summary>
public interface Writer
{
    /// <summary>
    /// the y coord that the cursor is currently at, changing with each new line
    /// </summary>
    int row { get; set; }

    /// <summary>
    /// the x coord that the cursor starts at the column specified
    /// </summary>
    int columnCoord { get; set; }

    /// <summary>
    /// Starts a new line and writes the text out, in the specified column of the instance
    /// </summary>
    void Out(string line, ConsoleColor colour = ConsoleColor.Gray);

    /// <summary>
    /// Starts a new line and returns the users integer input after they press enter
    /// </summary>
    int Get(bool ignoreErr = false);

    /// <summary>
    /// Will use Out method if there was no bad input, otherwise will output error message
    /// </summary>
    void Check(string passedMessage);

    /// <summary>
    /// Starts a new line for each user input, until they input a non integer, and returns the array of integers
    /// </summary>
    /// <returns></returns>
    int[] GetList();

    /// <summary>
    /// Waits for a user input, and runs the action associated with the integer pressed on the current page
    /// </summary>
    void Select();
}