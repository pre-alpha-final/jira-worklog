using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;
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
		private const string AtlassianAccount = "";

		[FunctionName("Import")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, TraceWriter log)
		{
			log.Info("Import started");
			var parsedMonth = int.TryParse(req.Query["month"], out var month);
			var parsedYear = int.TryParse(req.Query["year"], out var year);

			if (parsedMonth == false || parsedYear == false)
				return new BadRequestObjectResult("Missing month or year parameter");

			if (month < 1 || month > 12)
				return new BadRequestObjectResult("Invalid month");

			if (year < 2018)
				return new BadRequestObjectResult("Invalid year");

			var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
			var endDate = startDate.AddMonths(1).AddSeconds(-1);
			log.Info($"Import logs between {startDate.ToShortDateString()} " +
			         $"and {endDate.ToShortDateString()}");

			var service = new JiraWorklogService();
			var results = await service.FetchWorklogItems(startDate, endDate);
			var count = results.Count();
			log.Info($"Found {count} worklogs");

			var store = new ReportStore();
			var list = new List<WorklogDto>();

			foreach (var workLog in results)
			{
				list.Add(new WorklogDto
				{
					DateTime = workLog.Date,
					Hours = workLog.TimeSpentSeconds / 3600,
					Person = workLog.Author.DisplayName,
					Task = workLog.Task,
					TaskLink = $"https://{AtlassianAccount}.atlassian.net/browse/{workLog.Task}"
				});
			}

			await store.Store(new ReportDto
			{
				Month = month,
				Year = year,
				Logs = list
			});

			return new OkObjectResult(count);
		}
	}
}
