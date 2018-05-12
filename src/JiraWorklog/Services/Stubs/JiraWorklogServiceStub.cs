using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Infrastructure;

namespace JiraWorklog.Services.Stubs
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
			TwoPlusTwo,
			Five,
		}

		public Task<IEnumerable<JiraWorklogItem>> GetWorklogItems()
		{
			var jiraWorklogItems = new List<JiraWorklogItem>();

			foreach (var person in Persons)
			{
				for (int day = 0; day < Days; day++)
				{
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
						case WorkHours.TwoPlusTwo:
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 2, day));
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 2, day));
							break;
						case WorkHours.Five:
							jiraWorklogItems.Add(GetJiraWorklogItem(person, 5, day));
							break;
					}
				}
			}

			return Task.FromResult((IEnumerable<JiraWorklogItem>)jiraWorklogItems);
		}

		private JiraWorklogItem GetJiraWorklogItem(string person, int hours, int day)
		{
			var random = new Random((int)DateTime.Now.Ticks);
			return new JiraWorklogItem
			{
				Person = person,
				Task = $"PNP-{random.Next() % 10000}",
				Hours = hours,
				DateTime = DateTime.Now - TimeSpan.FromDays(Days - day)
			};
		}
	}
}
