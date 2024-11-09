// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportActionHandler
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Rendering.HtmlRenderer;
using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ReportActionHandler
  {
    private Report m_report;
    private PageCountMode m_pageCountMode;
    private int m_currentPage;
    private object m_eventSender;
    private PageNavigationEventHandler m_pageNav;
    private CancelEventHandler m_toggle;
    private BookmarkNavigationEventHandler m_bookmarkNavigation;
    private DocumentMapNavigationEventHandler m_documentMapNavigation;
    private DrillthroughEventHandler m_drillthrough;
    private SortEventHandler m_sort;
    private SearchEventHandler m_search;
    private CancelEventHandler m_refresh;

    public ReportActionHandler(
      Report report,
      object eventSender,
      int currentPage,
      PageCountMode pageCountMode,
      PageNavigationEventHandler pageNav,
      CancelEventHandler toggle,
      BookmarkNavigationEventHandler bookmarkNavigation,
      DocumentMapNavigationEventHandler documentMapNavigation,
      DrillthroughEventHandler drillthrough,
      SortEventHandler sort,
      SearchEventHandler search,
      CancelEventHandler refresh)
    {
      this.m_report = report;
      this.m_pageCountMode = pageCountMode;
      this.m_currentPage = currentPage;
      this.m_eventSender = eventSender;
      this.m_pageNav = pageNav;
      this.m_toggle = toggle;
      this.m_bookmarkNavigation = bookmarkNavigation;
      this.m_documentMapNavigation = documentMapNavigation;
      this.m_drillthrough = drillthrough;
      this.m_sort = sort;
      this.m_search = search;
      this.m_refresh = refresh;
    }

    public bool HandleToggle(string toggleID, out ScrollTarget scrollTarget)
    {
      CancelEventArgs e = new CancelEventArgs();
      if (this.m_toggle != null)
        this.m_toggle(this.m_eventSender, e);
      if (!e.Cancel)
      {
        this.m_report.PerformToggle(toggleID);
        scrollTarget = new ScrollTarget(toggleID, ActionScrollStyle.MaintainPosition);
        return true;
      }
      scrollTarget = (ScrollTarget) null;
      return false;
    }

    public bool HandleBookmarkNavigation(
      string bookmarkId,
      out int newPage,
      out ScrollTarget scrollTarget)
    {
      newPage = 0;
      scrollTarget = (ScrollTarget) null;
      BookmarkNavigationEventArgs e = new BookmarkNavigationEventArgs(bookmarkId);
      if (this.m_bookmarkNavigation != null)
        this.m_bookmarkNavigation(this.m_eventSender, e);
      if (!e.Cancel)
      {
        string uniqueName;
        newPage = this.m_report.PerformBookmarkNavigation(bookmarkId, out uniqueName);
        if (newPage > 0 && this.FirePageNavigationEvent(newPage))
        {
          scrollTarget = new ScrollTarget(uniqueName, ActionScrollStyle.AlignedToTopLeft);
          return true;
        }
      }
      return false;
    }

    public bool HandleDocMapNavigation(
      string docMapID,
      out int newPage,
      out ScrollTarget scrollTarget)
    {
      newPage = 0;
      scrollTarget = (ScrollTarget) null;
      DocumentMapNavigationEventArgs e = new DocumentMapNavigationEventArgs(docMapID);
      if (this.m_documentMapNavigation != null)
        this.m_documentMapNavigation(this.m_eventSender, e);
      if (!e.Cancel)
      {
        newPage = this.m_report.PerformDocumentMapNavigation(docMapID);
        if (newPage > 0 && this.FirePageNavigationEvent(newPage))
        {
          scrollTarget = new ScrollTarget(docMapID, ActionScrollStyle.AlignedToTopLeft);
          return true;
        }
      }
      return false;
    }

    public Report HandleDrillthrough(string drillthroughID)
    {
      string reportPath = (string) null;
      Report targetReport = this.m_report.PerformDrillthrough(drillthroughID, out reportPath);
      DrillthroughEventArgs e = new DrillthroughEventArgs(reportPath, targetReport);
      if (this.m_drillthrough != null)
        this.m_drillthrough(this.m_eventSender, e);
      return e.Cancel ? (Report) null : targetReport;
    }

    public bool HandleSort(
      string sortActionFromClient,
      out int pageNumber,
      out ScrollTarget scrollTarget)
    {
      pageNumber = 0;
      scrollTarget = (ScrollTarget) null;
      string sortID;
      SortOrder sortDirection;
      bool clearSort;
      this.ActionParamToSortParams(sortActionFromClient, out sortID, out sortDirection, out clearSort);
      SortEventArgs e = new SortEventArgs(sortID, sortDirection, clearSort);
      if (this.m_sort != null)
        this.m_sort(this.m_eventSender, e);
      if (!e.Cancel)
      {
        string uniqueName;
        int targetPage = this.m_report.PerformSort(sortID, sortDirection, clearSort, this.m_pageCountMode, out uniqueName);
        if (targetPage > 0)
        {
          if (!this.FirePageNavigationEvent(targetPage))
          {
            int totalPages = this.m_report.GetTotalPages();
            if (this.m_currentPage > totalPages)
              pageNumber = totalPages;
          }
          else
          {
            scrollTarget = new ScrollTarget(uniqueName, ActionScrollStyle.MaintainPosition);
            pageNumber = targetPage;
          }
          return true;
        }
      }
      return false;
    }

    public SearchResult HandleSearch(
      SearchState searchState,
      out int newPage,
      out ScrollTarget scrollTarget)
    {
      newPage = 0;
      scrollTarget = (ScrollTarget) null;
      SearchEventArgs e = new SearchEventArgs(searchState.Text, searchState.StartPage, false);
      if (this.m_search != null)
      {
        this.m_search(this.m_eventSender, e);
        if (e.Cancel)
          return SearchResult.Cancelled;
      }
      int searchEndPage = this.GetSearchEndPage(e.StartPage);
      newPage = this.m_report.PerformSearch(e.SearchString, e.StartPage, searchEndPage);
      if (newPage > 0)
      {
        if (!this.FirePageNavigationEvent(newPage))
          return SearchResult.Cancelled;
        scrollTarget = this.GetScrollTargetForSearchResult(SearchResult.FoundMoreHits, newPage != searchState.StartPage);
        return SearchResult.FoundMoreHits;
      }
      newPage = searchState.StartPage;
      scrollTarget = this.GetScrollTargetForSearchResult(SearchResult.NoMoreHits, false);
      return SearchResult.NoMoreHits;
    }

    public SearchResult HandleSearchNext(
      SearchState searchState,
      out int newPage,
      out ScrollTarget scrollTarget)
    {
      newPage = 0;
      scrollTarget = (ScrollTarget) null;
      SearchEventArgs e = new SearchEventArgs(searchState.Text, this.m_currentPage, true);
      if (this.m_search != null)
      {
        this.m_search(this.m_eventSender, e);
        if (e.Cancel)
          return SearchResult.Cancelled;
      }
      int searchEndPage = this.GetSearchEndPage(searchState.StartPage);
      if (this.m_currentPage != searchEndPage)
      {
        PageCountMode pageCountMode;
        int startPage = this.m_currentPage != this.m_report.GetTotalPages(out pageCountMode) || pageCountMode != PageCountMode.Actual ? this.m_currentPage + 1 : 1;
        newPage = this.m_report.PerformSearch(e.SearchString, startPage, searchEndPage);
        if (newPage != 0)
        {
          if (!this.FirePageNavigationEvent(newPage))
            return SearchResult.Cancelled;
          scrollTarget = this.GetScrollTargetForSearchResult(SearchResult.FoundMoreHits, newPage != searchState.StartPage);
          return SearchResult.FoundMoreHits;
        }
      }
      scrollTarget = this.GetScrollTargetForSearchResult(SearchResult.NoMoreHits, false);
      return SearchResult.NoMoreHits;
    }

    private ScrollTarget GetScrollTargetForSearchResult(SearchResult result, bool pageChanged)
    {
      switch (result)
      {
        case SearchResult.FoundMoreHits:
          string navigationId = HTML4Renderer.m_searchHitIdPrefix + "0";
          return pageChanged ? new ScrollTarget(navigationId, ActionScrollStyle.AvoidScrollingFromOrigin) : new ScrollTarget(navigationId, ActionScrollStyle.AvoidScrolling);
        case SearchResult.NoMoreHits:
          return new ScrollTarget((string) null, ActionScrollStyle.AvoidScrolling);
        default:
          return (ScrollTarget) null;
      }
    }

    public bool HandleRefresh()
    {
      CancelEventArgs e = new CancelEventArgs();
      if (this.m_refresh != null)
        this.m_refresh(this.m_eventSender, e);
      if (e.Cancel)
        return false;
      this.m_report.Refresh();
      return true;
    }

    public bool HandlePageNavigation(int targetPage)
    {
      if (targetPage <= 0)
        throw new ArgumentOutOfRangeException(nameof (targetPage));
      PageCountMode pageCountMode;
      int totalPages = this.m_report.GetTotalPages(out pageCountMode);
      if (totalPages == 0)
        pageCountMode = PageCountMode.Estimate;
      if (targetPage == int.MaxValue && pageCountMode != PageCountMode.Estimate)
        targetPage = totalPages;
      if (targetPage > totalPages && pageCountMode != PageCountMode.Estimate)
        throw new InvalidOperationException(Errors.InvalidPageNav);
      return this.FirePageNavigationEvent(targetPage);
    }

    private bool FirePageNavigationEvent(int targetPage)
    {
      if (this.m_pageNav == null || targetPage == this.m_currentPage)
        return true;
      PageNavigationEventArgs e = new PageNavigationEventArgs(targetPage);
      this.m_pageNav(this.m_eventSender, e);
      return !e.Cancel;
    }

    private void ActionParamToSortParams(
      string actionParam,
      out string sortID,
      out SortOrder sortDirection,
      out bool clearSort)
    {
      string[] strArray = actionParam.Split('_');
      sortID = strArray.Length >= 3 ? strArray[0] : throw new ArgumentOutOfRangeException(nameof (actionParam));
      sortDirection = string.Compare(strArray[1], "A", StringComparison.Ordinal) != 0 ? SortOrder.Descending : SortOrder.Ascending;
      clearSort = string.Compare(strArray[2], "T", StringComparison.Ordinal) != 0;
    }

    private int GetSearchEndPage(int startPage)
    {
      if (startPage != 1)
        return startPage - 1;
      PageCountMode pageCountMode;
      int totalPages = this.m_report.GetTotalPages(out pageCountMode);
      return pageCountMode != PageCountMode.Actual ? int.MaxValue : totalPages;
    }
  }
}
