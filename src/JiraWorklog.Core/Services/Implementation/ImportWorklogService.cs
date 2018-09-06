using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JiraWorklog.Core.Models;
using Newtonsoft.Json;

namespace JiraWorklog.Core.Services.Implementation
{
	public class ImportWorklogService : IImportWorklogService
	{
		private readonly AppSettings _appSettings;

		public ImportWorklogService(AppSettings appSettings)
		{
			_appSettings = appSettings;
		}

		public async Task<IList<IssueWorklog>> Import(DateTime dateFrom, DateTime dateTo)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri($"https://{_appSettings.AtlassianAccount}.atlassian.net");
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Basic", _appSettings.AuthToken);

				var searchResult = await PerformSearch(client, dateFrom, dateTo);
				return await ReadIssues(client, searchResult, dateFrom, dateTo);
			}
		}

		private async Task<SearchResponse> PerformSearch(HttpClient client, DateTime dateFrom, DateTime dateTo)
		{
			var dateFromStr = dateFrom.ToString("yyyy-MM-dd");
			var dateToStr = dateTo.ToString("yyyy-MM-dd");

			var seachQuery = new SearchQuery();
			seachQuery.Jql = $"project = {_appSettings.ProjectCode} AND updated >= {dateFromStr} AND updated <= {dateToStr} ORDER BY created DESC";
			seachQuery.StartAt = 0;
			seachQuery.MaxResults = 100;
			seachQuery.Fields = new[] { "id" };
			var searchQueryContent = ConvertToJsonContent(seachQuery);

			var searchResponse = await client.PostAsync("/rest/api/2/search", searchQueryContent);
			var searchResultString = await searchResponse.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<SearchResponse>(searchResultString);
		}

		protected StringContent ConvertToJsonContent(object obj)
		{
			return new StringContent(JsonConvert.SerializeObject(obj),
				Encoding.UTF8, "application/json");
		}

		private async Task<IList<IssueWorklog>> ReadIssues(HttpClient client, SearchResponse searchResult, DateTime dateFrom, DateTime dateTo)
		{
			var list = new List<IssueWorklog>();

			foreach (var issue in searchResult.Issues)
			{
				var issueWorklogResponse = await client.GetAsync($"rest/api/2/issue/{issue.Key}/worklog");
				var issueWorklogString = await issueWorklogResponse.Content.ReadAsStringAsync();
				var issueWorklog = JsonConvert.DeserializeObject<IssueWorklogsResponse>(issueWorklogString);
				foreach (var log in issueWorklog.Worklogs)
				{
					if (log.Date < dateFrom || log.Date > dateTo)
						continue;
					log.Task = issue.Key;
					list.Add(log);
				}
			}

			return list;
		}
	}
}
