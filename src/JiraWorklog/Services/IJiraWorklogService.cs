using JiraWorklog.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraWorklog.Services
{
	public interface IJiraWorklogService
	{
		Task<IEnumerable<JiraWorklogItem>> GetWorklogItems();
	}
}
