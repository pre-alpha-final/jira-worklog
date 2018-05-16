using System;
using JiraWorklog.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Services.Implementation;

namespace JiraWorklog.Services
{
	public interface IJiraWorklogService
	{
		Task<IEnumerable<JiraWorklogItem>> GetWorklogItems();
		Task<IEnumerable<IssueWorklog>> FetchWorklogItems(DateTime dateFrom, DateTime dateTo);
	}
}
