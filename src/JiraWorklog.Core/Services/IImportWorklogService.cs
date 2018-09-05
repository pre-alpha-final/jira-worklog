using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core.Models;

namespace JiraWorklog.Core.Services
{
	public interface IImportWorklogService
	{
		Task<IList<IssueWorklog>> Import(DateTime dateFrom, DateTime dateTo);
	}
}
