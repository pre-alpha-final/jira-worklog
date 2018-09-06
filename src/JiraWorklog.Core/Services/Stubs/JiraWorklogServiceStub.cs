using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;

namespace JiraWorklog.Core.Services.Stubs
{
	public class JiraWorklogServiceStub : IJiraWorklogService
	{
		private const int Days = 40;

		private List<string> Persons = new List<string>
		{
			"John Doe",
			"Jane Doe",
		};

		enum WorkHours
		{
			Six,
			ThreePlusThree,
			TwoPlusTwoPlusTwo,
			FivePlusOne,
			Five,
		}

		public Task<IEnumerable<WorklogDto>> GetWorklogItems(int? month, int? year)
		{
			var jiraWorklogItems = new List<WorklogDto>();

			foreach (var person in Persons)
			{
				for (int day = 1; day <= Days; day++)
				{
					var dayOfWeek = (DateTime.Now - TimeSpan.FromDays(Days - day)).DayOfWeek;
					if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
					{
						continue;
					}

					var random = new Random((int)DateTime.Now.Ticks);
					var workHours = (WorkHours)(random.Next() % 5);
					switch (workHours)
					{
						case WorkHours.Six:
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 6, day));
							break;
						case WorkHours.ThreePlusThree:
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 3, day));
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 3, day));
							break;
						case WorkHours.TwoPlusTwoPlusTwo:
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 2, day));
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 2, day));
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 2, day));
							break;
						case WorkHours.FivePlusOne:
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 5, day));
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 1, day));
							break;
						case WorkHours.Five:
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 5, day));
							break;
					}
				}
			}

			return Task.FromResult((IEnumerable<WorklogDto>)jiraWorklogItems);
		}

		private WorklogDto GetJiraWorklogItem(string person, int hours, int day)
		{
			var random = new Random((int)DateTime.Now.Ticks);
			var task = $"PNP-{random.Next() % 10000}";
			return new WorklogDto
			{
				Person = person,
				Task = task,
				TaskLink = $"https://www.google.com/search?q={task}",
				Hours = hours,
				DateTime = DateTime.Now - TimeSpan.FromDays(Days - day)
			};
		}
	}
}
