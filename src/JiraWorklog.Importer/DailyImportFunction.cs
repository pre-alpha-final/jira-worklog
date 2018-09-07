using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core;
using JiraWorklog.Core.DataTransferObjects;
using JiraWorklog.Core.Models;
using JiraWorklog.Core.Services.Implementation;
using JiraWorklog.Data.AzureBlob.Stores;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace JiraWorklog.Importer
{
	public static class DailyImportFunction
	{
		private static AppSettings _appSettings;

		[FunctionName("DailyImportFunction")]
		public static async Task Run([TimerTrigger("0 0 21 * * 1-5")]TimerInfo myTimer, ILogger log)
		{
			log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

			_appSettings = new AppSettings
			{
				AtlassianAccount = GetEnvironmentVariable("AppSettings_AtlassianAccount"),
				AuthToken = GetEnvironmentVariable("AppSettings_AuthToken"),
				ProjectCode = GetEnvironmentVariable("AppSettings_ProjectCode")
			};

			log.LogInformation("Config read");

			var currentDate = DateTime.UtcNow;

			var importedLogs = await ImportWorklogs(currentDate.Month, currentDate.Year, log);
			var storedLogCount = await StoreWorklogs(currentDate.Month, currentDate.Year, importedLogs);
			log.LogInformation($"Stored {storedLogCount} worklogs");
		}

		private static async Task<IEnumerable<IssueWorklog>> ImportWorklogs(int month, int year, ILogger log)
		{
			var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
			var endDate = startDate.AddMonths(1).AddSeconds(-1);

			log.LogInformation($"Import logs between {startDate.ToShortDateString()} " +
					 $"and {endDate.ToShortDateString()}");

			var service = new ImportWorklogService(_appSettings);
			var results = await service.Import(startDate, endDate);
			var count = results.Count;
			log.LogInformation($"Found {count} worklogs");
			return results;
		}

		private static async Task<int> StoreWorklogs(int month, int year, IEnumerable<IssueWorklog> worklogs)
		{
			var list = new List<WorklogDto>();

			foreach (var worklog in worklogs)
			{
				list.Add(new WorklogDto
				{
					DateTime = worklog.Date,
					Hours = worklog.TimeSpentSeconds / 3600,
					Person = worklog.Author.DisplayName,
					Task = worklog.Task,
					TaskLink = $"https://{_appSettings.AtlassianAccount}.atlassian.net/browse/{worklog.Task}"
				});
			}

			var store = new ReportStore(GetEnvironmentVariable("WEBSITE_CONTENTAZUREFILECONNECTIONSTRING"));
			await store.Store(new ReportDto
			{
				Month = month,
				Year = year,
				Logs = list
			});

			return list.Count;
		}

		public static string GetEnvironmentVariable(string name)
		{
			return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
		}
	}
}
