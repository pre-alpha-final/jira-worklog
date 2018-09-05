using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;
using JiraWorklog.Core.Services.Implementation;

namespace JiraWorklog.Core.Services
{
	public interface IJiraWorklogService
	{
		Task<IEnumerable<WorklogDto>> GetWorklogItems();
		Task<IEnumerable<IssueWorklog>> FetchWorklogItems(DateTime dateFrom, DateTime dateTo);

	}
}
