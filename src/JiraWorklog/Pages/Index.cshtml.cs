using JiraWorklog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace JiraWorklog.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IJiraWorklogService _jiraWorklogService;

		public IndexModel(IJiraWorklogService jiraWorklogService)
		{
			_jiraWorklogService = jiraWorklogService;
		}

		public async Task OnGetAsync()
		{
			var items = await _jiraWorklogService.GetWorklogItems();
		}
	}
}
