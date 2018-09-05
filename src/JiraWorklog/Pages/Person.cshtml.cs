using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using JiraWorklog.Core.DataTransferObjects;

namespace JiraWorklog.Pages
{
	public class PersonModel : PageModel
	{
		public string Person { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public IList<WorklogDto> JiraWorklogItems { get; set; }

		public string Hours6 => JiraWorklogItems.Select(e => e.Hours).Sum().ToString();
		public string Hours8 => (JiraWorklogItems.Select(e => e.Hours).Sum() * 8 / 6).ToString();

		public IList<DateTime> DateTimes
		{
			get
			{
				var dateTimes = new List<DateTime>();
				var dateTime = From;
				do
				{
					dateTimes.Add(dateTime);
					dateTime += TimeSpan.FromDays(1);
				}
				while (dateTime.Date <= To.Date);
				return dateTimes;
			}
		}

		public string GetClass(DateTime dateTime)
		{
			if (DateTime.Now.Date == dateTime.Date)
			{
				return "now";
			}

			if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
			{
				return "na";
			}

			var workHours = JiraWorklogItems
				.Where(e => e.DateTime.Date == dateTime.Date)
				.Select(e => e.Hours)
				.ToList();
			if (workHours.Sum() != 6)
			{
				return "notok";
			}

			return "ok";
		}

		public string GetWorklog(DateTime dateTime)
		{
			var worklogs = JiraWorklogItems
				.Where(e => e.DateTime.Date == dateTime.Date)
				.ToList();
			if (worklogs.Count == 0)
			{
				return string.Empty;
			}

			var result = string.Empty;
			foreach (var worklog in worklogs)
			{
				result += $@"<br />{worklog.Hours}h/<a href=""{worklog.TaskLink}"" target=""_blank"">{worklog.Task.Replace(" ", " &nbsp;").Replace("-", "&#8209;")}</a>";
			}
			result = result.Remove(0, 6);

			return result;
		}
	}
}
