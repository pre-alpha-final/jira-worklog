using JiraWorklog.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace JiraWorklog.Pages
{
	public class PersonModel : PageModel
	{
		public string Person { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public IList<JiraWorklogItem> JiraWorklogItems { get; set; }
	}
}
