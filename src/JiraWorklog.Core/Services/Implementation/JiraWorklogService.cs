using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;
using Newtonsoft.Json;

namespace JiraWorklog.Core.Services.Implementation
{
	public class JiraWorklogService : IJiraWorklogService
	{
		private const string AtlassianAccount = "";
		private const string ProjectCode = "";
		private const string AuthToken = "";

		public async Task<IEnumerable<WorklogDto>> GetWorklogItems()
		{
			var jiraItems = await FetchWorklogItems(new DateTime(2018, 5, 1), new DateTime(2018, 5, 31));
			var workItems = new List<WorklogDto>();

						foreach (var jiraItem in jiraItems)
						{
							workItems.Add(new WorklogDto
							{
								DateTime = jiraItem.Date,
								Hours = jiraItem.TimeSpentSeconds / 3600,
								Person = jiraItem.Author.DisplayName,
								Task = jiraItem.Task,
								TaskLink = $"https://{AtlassianAccount}.atlassian.net/browse/{jiraItem.Task}"
							});
						}

			return workItems;
		}

		public async Task<IEnumerable<IssueWorklog>> FetchWorklogItems(DateTime dateFrom, DateTime dateTo)
		{
			var list = new List<IssueWorklog>();
			var dateFromStr = dateFrom.ToString("yyyy-MM-dd");
			var dateToStr = dateTo.ToString("yyyy-MM-dd");

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri($"https://{AtlassianAccount}.atlassian.net");
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Basic", AuthToken);

				var seachQuery = new SearchQuery();
				seachQuery.Jql = $"project = {ProjectCode} AND updated >= {dateFromStr} AND updated <= {dateToStr} ORDER BY created DESC";
				seachQuery.StartAt = 0;
				seachQuery.MaxResults = 100;
				seachQuery.Fields = new[] { "id" };

				var searchQueryContent = ConvertToJsonContent(seachQuery);

				var searchResponse = await client.PostAsync("/rest/api/2/search", searchQueryContent);

				var searchResultString = await searchResponse.Content.ReadAsStringAsync();
				var searchResult = JsonConvert.DeserializeObject<SearchResponse>(searchResultString);

				Debug.WriteLine("Total issues to read: " + searchResult.Total);
				foreach (var issue in searchResult.Issues)
				{
					Debug.WriteLine("Read issue: " + issue.Key);
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
			}

			return list;
		}

		protected StringContent ConvertToJsonContent(object obj)
		{
			return new StringContent(JsonConvert.SerializeObject(obj),
				Encoding.UTF8, "application/json");
		}
	}

	public class SearchQuery
	{
		[JsonProperty("jql")]
		public string Jql { get; set; }

		[JsonProperty("startAt")]
		public int StartAt { get; set; }

		[JsonProperty("maxResults")]
		public int MaxResults { get; set; }

		[JsonProperty("fields")]
		public IEnumerable<string> Fields { get; set; }
	}

	public class SearchResponse
	{
		[JsonProperty("expand")]
		public string Expand { get; set; }

		[JsonProperty("startAt")]
		public int StartAt { get; set; }

		[JsonProperty("maxResults")]
		public int MaxResults { get; set; }

		[JsonProperty("total")]
		public int Total { get; set; }

		[JsonProperty("issues")]
		public IEnumerable<SearchIssue> Issues { get; set; }
	}

	public class SearchIssue
	{
		[JsonProperty("expand")]
		public string Expand { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("self")]
		public string Self { get; set; }

		[JsonProperty("key")]
		public string Key { get; set; }
	}

	public class IssueWorklogsResponse
	{
		[JsonProperty("startAt")]
		public int StartAt { get; set; }

		[JsonProperty("maxResults")]
		public int MaxResults { get; set; }

		[JsonProperty("total")]
		public int Total { get; set; }

		[JsonProperty("worklogs")]
		public IEnumerable<IssueWorklog> Worklogs { get; set; }
	}

	public class IssueWorklog
	{
		[JsonProperty("timeSpentSeconds")]
		public int TimeSpentSeconds { get; set; }

		[JsonProperty("author")]
		public IssueWorklogAuthor Author { get; set; }

		[JsonProperty("started")]
		public DateTime Date { get; set; }

		[JsonIgnore]
		public string Task { get; set; }
	}

	public class IssueWorklogAuthor
	{
		[JsonProperty("displayName")]
		public string DisplayName { get; set; }
	}
}
