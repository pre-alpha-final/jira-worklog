using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;
using JiraWorklog.Core.Stores;

namespace JiraWorklog.Core.Services.Implementation
{
	public class JiraWorklogService : IJiraWorklogService
	{
		private readonly IReportStore _reportStore;

		public JiraWorklogService(IReportStore reportStore)
		{
			_reportStore = reportStore;
		}

		public async Task<IEnumerable<WorklogDto>> GetWorklogItems(int? month, int? year)
		{
			if (month == null)
			{
				month = DateTime.Now.Month;
			}
			if (year == null)
			{
				year = DateTime.Now.Year;
			}

			var report = await _reportStore.FindByDate((int)month, (int)year);

			return report.Logs;
		}
	}
}
