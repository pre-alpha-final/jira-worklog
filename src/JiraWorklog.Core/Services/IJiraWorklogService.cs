using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;

namespace JiraWorklog.Core.Services
{
	public interface IJiraWorklogService
	{
		Task<IEnumerable<WorklogDto>> GetWorklogItems();
	}
}
