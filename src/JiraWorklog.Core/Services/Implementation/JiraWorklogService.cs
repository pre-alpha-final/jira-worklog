using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;

namespace JiraWorklog.Core.Services.Implementation
{
	public class JiraWorklogService : IJiraWorklogService
	{
		public async Task<IEnumerable<WorklogDto>> GetWorklogItems()
		{
			//TODO Use report store and pass month and year
			return new List<WorklogDto>();
		}
	}
}
