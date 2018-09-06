using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraWorklog.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace JiraWorklog.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IJiraWorklogService _jiraWorklogService;

		public List<PersonModel> PersonModels { get; set; } = new List<PersonModel>();

		[BindProperty(SupportsGet = true)]
		public int? Month { get; set; }

		[BindProperty(SupportsGet = true)]
		public int? Year { get; set; }

		public IndexModel(IJiraWorklogService jiraWorklogService)
		{
			_jiraWorklogService = jiraWorklogService;
		}

		public async Task OnGetAsync()
		{
			try
			{
				var items = (await _jiraWorklogService.GetWorklogItems(Month, Year)).ToList();

				var sortedByDate = items.Select(e => e.DateTime).ToList();
				sortedByDate.Sort((a, b) => a.CompareTo(b));
				var from = sortedByDate.First();
				var to = sortedByDate.Last();

				var persons = items.Select(e => e.Person).Distinct().ToList();

				foreach(var person in persons)
				{
					PersonModels.Add(new PersonModel
					{
						Person = person,
						From = from,
						To = to,
						JiraWorklogItems = items.Where(e => e.Person == person).ToList(),
					});
				}
			}
			catch (Exception e)
			{
				// Ignore
			}
		}
	}
}
