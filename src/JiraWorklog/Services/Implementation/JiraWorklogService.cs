using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Infrastructure;

namespace JiraWorklog.Services.Implementation
{
	public class JiraWorklogService : IJiraWorklogService
	{
		public Task<IEnumerable<JiraWorklogItem>> GetWorklogItems()
		{
			throw new System.NotImplementedException();
		}
	}
}
