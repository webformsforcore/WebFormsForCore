using System;
using System.ComponentModel;
using Microsoft.ReportingServices.Rendering.HtmlRenderer;

namespace Microsoft.Reporting.WebForms;

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

	public ReportActionHandler(Report report, object eventSender, int currentPage, PageCountMode pageCountMode, PageNavigationEventHandler pageNav, CancelEventHandler toggle, BookmarkNavigationEventHandler bookmarkNavigation, DocumentMapNavigationEventHandler documentMapNavigation, DrillthroughEventHandler drillthrough, SortEventHandler sort, SearchEventHandler search, CancelEventHandler refresh)
	{
		m_report = report;
		m_pageCountMode = pageCountMode;
		m_currentPage = currentPage;
		m_eventSender = eventSender;
		m_pageNav = pageNav;
		m_toggle = toggle;
		m_bookmarkNavigation = bookmarkNavigation;
		m_documentMapNavigation = documentMapNavigation;
		m_drillthrough = drillthrough;
		m_sort = sort;
		m_search = search;
		m_refresh = refresh;
	}

	public bool HandleToggle(string toggleID, out ScrollTarget scrollTarget)
	{
		CancelEventArgs e = new CancelEventArgs();
		if (m_toggle != null)
		{
			m_toggle(m_eventSender, e);
		}
		if (!e.Cancel)
		{
			m_report.PerformToggle(toggleID);
			scrollTarget = new ScrollTarget(toggleID, ActionScrollStyle.MaintainPosition);
			return true;
		}
		scrollTarget = null;
		return false;
	}

	public bool HandleBookmarkNavigation(string bookmarkId, out int newPage, out ScrollTarget scrollTarget)
	{
		newPage = 0;
		scrollTarget = null;
		BookmarkNavigationEventArgs e = new BookmarkNavigationEventArgs(bookmarkId);
		if (m_bookmarkNavigation != null)
		{
			m_bookmarkNavigation(m_eventSender, e);
		}
		if (!e.Cancel)
		{
			newPage = m_report.PerformBookmarkNavigation(bookmarkId, out var uniqueName);
			if (newPage > 0 && FirePageNavigationEvent(newPage))
			{
				scrollTarget = new ScrollTarget(uniqueName, ActionScrollStyle.AlignedToTopLeft);
				return true;
			}
		}
		return false;
	}

	public bool HandleDocMapNavigation(string docMapID, out int newPage, out ScrollTarget scrollTarget)
	{
		newPage = 0;
		scrollTarget = null;
		DocumentMapNavigationEventArgs e = new DocumentMapNavigationEventArgs(docMapID);
		if (m_documentMapNavigation != null)
		{
			m_documentMapNavigation(m_eventSender, e);
		}
		if (!e.Cancel)
		{
			newPage = m_report.PerformDocumentMapNavigation(docMapID);
			if (newPage > 0 && FirePageNavigationEvent(newPage))
			{
				scrollTarget = new ScrollTarget(docMapID, ActionScrollStyle.AlignedToTopLeft);
				return true;
			}
		}
		return false;
	}

	public Report HandleDrillthrough(string drillthroughID)
	{
		string reportPath = null;
		Report report = m_report.PerformDrillthrough(drillthroughID, out reportPath);
		DrillthroughEventArgs e = new DrillthroughEventArgs(reportPath, report);
		if (m_drillthrough != null)
		{
			m_drillthrough(m_eventSender, e);
		}
		if (e.Cancel)
		{
			return null;
		}
		return report;
	}

	public bool HandleSort(string sortActionFromClient, out int pageNumber, out ScrollTarget scrollTarget)
	{
		pageNumber = 0;
		scrollTarget = null;
		ActionParamToSortParams(sortActionFromClient, out var sortID, out var sortDirection, out var clearSort);
		SortEventArgs e = new SortEventArgs(sortID, sortDirection, clearSort);
		if (m_sort != null)
		{
			m_sort(m_eventSender, e);
		}
		if (!e.Cancel)
		{
			string uniqueName;
			int num = m_report.PerformSort(sortID, sortDirection, clearSort, m_pageCountMode, out uniqueName);
			if (num > 0)
			{
				if (!FirePageNavigationEvent(num))
				{
					int totalPages = m_report.GetTotalPages();
					if (m_currentPage > totalPages)
					{
						pageNumber = totalPages;
					}
				}
				else
				{
					scrollTarget = new ScrollTarget(uniqueName, ActionScrollStyle.MaintainPosition);
					pageNumber = num;
				}
				return true;
			}
		}
		return false;
	}

	public SearchResult HandleSearch(SearchState searchState, out int newPage, out ScrollTarget scrollTarget)
	{
		newPage = 0;
		scrollTarget = null;
		SearchEventArgs e = new SearchEventArgs(searchState.Text, searchState.StartPage, isFindNext: false);
		if (m_search != null)
		{
			m_search(m_eventSender, e);
			if (e.Cancel)
			{
				return SearchResult.Cancelled;
			}
		}
		int searchEndPage = GetSearchEndPage(e.StartPage);
		newPage = m_report.PerformSearch(e.SearchString, e.StartPage, searchEndPage);
		if (newPage > 0)
		{
			if (!FirePageNavigationEvent(newPage))
			{
				return SearchResult.Cancelled;
			}
			scrollTarget = GetScrollTargetForSearchResult(SearchResult.FoundMoreHits, newPage != searchState.StartPage);
			return SearchResult.FoundMoreHits;
		}
		newPage = searchState.StartPage;
		scrollTarget = GetScrollTargetForSearchResult(SearchResult.NoMoreHits, pageChanged: false);
		return SearchResult.NoMoreHits;
	}

	public SearchResult HandleSearchNext(SearchState searchState, out int newPage, out ScrollTarget scrollTarget)
	{
		newPage = 0;
		scrollTarget = null;
		SearchEventArgs e = new SearchEventArgs(searchState.Text, m_currentPage, isFindNext: true);
		if (m_search != null)
		{
			m_search(m_eventSender, e);
			if (e.Cancel)
			{
				return SearchResult.Cancelled;
			}
		}
		int searchEndPage = GetSearchEndPage(searchState.StartPage);
		if (m_currentPage != searchEndPage)
		{
			PageCountMode pageCountMode;
			int totalPages = m_report.GetTotalPages(out pageCountMode);
			int startPage = ((m_currentPage == totalPages && pageCountMode == PageCountMode.Actual) ? 1 : (m_currentPage + 1));
			newPage = m_report.PerformSearch(e.SearchString, startPage, searchEndPage);
			if (newPage != 0)
			{
				if (!FirePageNavigationEvent(newPage))
				{
					return SearchResult.Cancelled;
				}
				scrollTarget = GetScrollTargetForSearchResult(SearchResult.FoundMoreHits, newPage != searchState.StartPage);
				return SearchResult.FoundMoreHits;
			}
		}
		scrollTarget = GetScrollTargetForSearchResult(SearchResult.NoMoreHits, pageChanged: false);
		return SearchResult.NoMoreHits;
	}

	private ScrollTarget GetScrollTargetForSearchResult(SearchResult result, bool pageChanged)
	{
		switch (result)
		{
		case SearchResult.NoMoreHits:
			return new ScrollTarget(null, ActionScrollStyle.AvoidScrolling);
		case SearchResult.FoundMoreHits:
		{
			string navigationId = HTML4Renderer.m_searchHitIdPrefix + "0";
			if (pageChanged)
			{
				return new ScrollTarget(navigationId, ActionScrollStyle.AvoidScrollingFromOrigin);
			}
			return new ScrollTarget(navigationId, ActionScrollStyle.AvoidScrolling);
		}
		default:
			return null;
		}
	}

	public bool HandleRefresh()
	{
		CancelEventArgs e = new CancelEventArgs();
		if (m_refresh != null)
		{
			m_refresh(m_eventSender, e);
		}
		if (!e.Cancel)
		{
			m_report.Refresh();
			return true;
		}
		return false;
	}

	public bool HandlePageNavigation(int targetPage)
	{
		if (targetPage <= 0)
		{
			throw new ArgumentOutOfRangeException("targetPage");
		}
		PageCountMode pageCountMode;
		int totalPages = m_report.GetTotalPages(out pageCountMode);
		if (totalPages == 0)
		{
			pageCountMode = PageCountMode.Estimate;
		}
		if (targetPage == int.MaxValue && pageCountMode != PageCountMode.Estimate)
		{
			targetPage = totalPages;
		}
		if (targetPage > totalPages && pageCountMode != PageCountMode.Estimate)
		{
			throw new InvalidOperationException(Errors.InvalidPageNav);
		}
		return FirePageNavigationEvent(targetPage);
	}

	private bool FirePageNavigationEvent(int targetPage)
	{
		if (m_pageNav != null && targetPage != m_currentPage)
		{
			PageNavigationEventArgs e = new PageNavigationEventArgs(targetPage);
			m_pageNav(m_eventSender, e);
			return !e.Cancel;
		}
		return true;
	}

	private void ActionParamToSortParams(string actionParam, out string sortID, out SortOrder sortDirection, out bool clearSort)
	{
		string[] array = actionParam.Split('_');
		if (array.Length < 3)
		{
			throw new ArgumentOutOfRangeException("actionParam");
		}
		sortID = array[0];
		if (string.Compare(array[1], "A", StringComparison.Ordinal) == 0)
		{
			sortDirection = SortOrder.Ascending;
		}
		else
		{
			sortDirection = SortOrder.Descending;
		}
		clearSort = string.Compare(array[2], "T", StringComparison.Ordinal) != 0;
	}

	private int GetSearchEndPage(int startPage)
	{
		if (startPage == 1)
		{
			PageCountMode pageCountMode;
			int totalPages = m_report.GetTotalPages(out pageCountMode);
			if (pageCountMode != PageCountMode.Actual)
			{
				return int.MaxValue;
			}
			return totalPages;
		}
		return startPage - 1;
	}
}
