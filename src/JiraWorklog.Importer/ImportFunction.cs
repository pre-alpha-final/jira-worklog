using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraWorklog.Core;
using JiraWorklog.Core.DataTransferObjects;
using JiraWorklog.Core.Models;
using JiraWorklog.Core.Services.Implementation;
using JiraWorklog.Data.AzureBlob.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

namespace JiraWorklog.Importer
{
	public static class ImportFunction
	{
		private static AppSettings _appSettings;

		[FunctionName("Import")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, TraceWriter log)
		{
			log.Info("Import started");
			_appSettings = new AppSettings
			{
				AtlassianAccount = GetEnvironmentVariable("AppSettings_AtlassianAccount"),
				AuthToken = GetEnvironmentVariable("AppSettings_AuthToken"),
				ProjectCode = GetEnvironmentVariable("AppSettings_ProjectCode")
			};

			log.Info("Config read");
			var parsedMonth = int.TryParse(req.Query["month"], out var month);
			var parsedYear = int.TryParse(req.Query["year"], out var year);

			if (parsedMonth == false || parsedYear == false)
				return new BadRequestObjectResult("Missing month or year parameter");
			if (month < 1 || month > 12)
				return new BadRequestObjectResult("Invalid month");
			if (year < 2018)
				return new BadRequestObjectResult("Invalid year");

			var importedLogs = await ImportWorklogs(month, year, log);
			var storedLogCount = await StoreWorklogs(month, year, importedLogs);
			log.Info($"Stored {storedLogCount} worklogs");

			return new OkObjectResult(storedLogCount);
		}

		private static async Task<IEnumerable<IssueWorklog>> ImportWorklogs(int month, int year, TraceWriter log)
		{
			var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
			var endDate = startDate.AddMonths(1).AddSeconds(-1);
			log.Info($"Import logs between {startDate.ToShortDateString()} " +
				$"and {endDate.ToShortDateString()}");

			var service = new ImportWorklogService(_appSettings);
			var results = await service.Import(startDate, endDate);
			var count = results.Count();
			log.Info($"Found {count} worklogs");
			return results;
		}

		private static async Task<int> StoreWorklogs(int month, int year, IEnumerable<IssueWorklog>worklogs)
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
