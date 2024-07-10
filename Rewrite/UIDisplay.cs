using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class is used to control the display of the the UI
/// </summary>
public interface IDisplay
{
    void NavigateForwardPage(UI ui, string Page);
    void NavigateBackPage(UI ui, string Page);
    void ClearPage(UI ui, int? index = null);
    void StartSelect(UI ui, UIWriter writer);
}

public class UIDisplayDefault : IDisplay
{
    public void NavigateForwardPage(UI ui, string Page)
    {
        ClearPage(ui, ui.OpenPages.Count);
        UIWriter line = new UIWriter(ui);
        line.Out("<- " + ui.Pages[Page].name + " ->", ui.Pages[Page].options.titleColour);

        // list out options
        int index = 0;
        foreach (UISelection selection in ui.Pages[Page].selections)
        {
            line.Out("[" + index + "] " + selection.name);
            index++;
        }
        ChangeOtherCols(ui, Page);
    }

    public void NavigateBackPage(UI ui, string Page)
    {
        int Last = ui.OpenPages.IndexOf(Page);
        for (int i = ui.OpenPages.Count - 1; i > Last; i--)
        {
            ClearPage(ui);
            ui.OpenPages.RemoveAt(i);
        }

        UIWriter line = new UIWriter(ui);
        line.Out("<- " + ui.Pages[Page].name + " ->", ui.Pages[Page].options.titleColour);
        ChangeOtherCols(ui, Page);
    }

    public void ClearPage(UI ui, int? col = null)
    {
        UIWriter clearer = new UIWriter(ui, 0, col);
        int gap = Console.WindowWidth / ui.MaxColumns;
        string EmptySpace = new string(' ', gap);
        for (int row = 0; row < Console.WindowHeight - 1; row++)
        {
            clearer.Out(EmptySpace);
        }
    }

    public void StartSelect(UI ui, UIWriter writer)
    {
        Console.SetCursorPosition(writer.ColumnCoord, 0);
    }

    private void ChangeOtherCols(UI ui, string curPage)
    {
        int gap = Console.WindowWidth / ui.MaxColumns;
        for (int i = 1; i < ui.OpenPages.IndexOf(curPage) + 1; i++)
        {
            UIWriter titleChanger = new UIWriter(ui, 0, i);
            titleChanger.Out(new string(' ', gap));
            titleChanger.Row--;
            titleChanger.Out(ui.OpenPages[i - 1], ui.Pages[ui.OpenPages[i - 1]].options.titleColour);
        }
    }
}

public class UIDisplayExplorer : IDisplay
{
    private string WriteTitle(UI ui)
    {
        string title = ui.OpenPages[0];
        if (ui.OpenPages.Count < 2) return title;

        for (int i = 1; i < ui.OpenPages.Count; i++)
        {
            title += " > " + ui.OpenPages[i];
        }
        return title;
    }

    public void NavigateForwardPage(UI ui, string Page)
    {
        ClearPage(ui, 1);
        UIWriter line = new UIWriter(ui);
        line.Out(WriteTitle(ui), ui.Pages[Page].options.titleColour);

        // list out options
        int index = 0;
        foreach (UISelection selection in ui.Pages[Page].selections)
        {
            line.Out("[" + index + "] " + selection.name);
            index++;
        }
        line.Select();
    }

    public void NavigateBackPage(UI ui, string Page)
    {
        int pageIndex = ui.OpenPages.IndexOf(Page);
        if (pageIndex != ui.OpenPages.Count - 1)
        {
            ui.OpenPages.RemoveRange(pageIndex + 1, ui.OpenPages.Count - pageIndex - 1);
        }
        
        ClearPage(ui, 1);

        UIWriter line = new UIWriter(ui);
        line.Out(WriteTitle(ui), ui.Pages[Page].options.titleColour);
        int index = 0;
        foreach (UISelection selection in ui.Pages[Page].selections)
        {
            line.Out("[" + index + "] " + selection.name);
            index++;
        }
        line.Select();
    }

    public void ClearPage(UI ui, int? col = null)
    {
        UIWriter clearer = new UIWriter(ui);
        string EmptySpace = new string(' ', Console.WindowWidth);
        for (int row = 0; row < Console.WindowHeight - 1; row++)
        {
            clearer.Out(EmptySpace);
        }
    }

    public void StartSelect(UI ui, UIWriter writer) { }
}

