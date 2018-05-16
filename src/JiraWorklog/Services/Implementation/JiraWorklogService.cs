using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JiraWorklog.Infrastructure;
using Newtonsoft.Json;

namespace JiraWorklog.Services.Implementation
{
	public class JiraWorklogService : IJiraWorklogService
	{
		private const string AtlassianAccount = "";
		private const string ProjectCode = "";
		private const string AuthToken = "";

		public async Task<IEnumerable<JiraWorklogItem>> GetWorklogItems()
		{
			var jiraItems = await FetchWorklogItems(new DateTime(2018, 5, 1), new DateTime(2018, 5, 31));
			var workItems = new List<JiraWorklogItem>();

			foreach (var jiraItem in jiraItems)
			{
				workItems.Add(new JiraWorklogItem
				{
					DateTime = jiraItem.Date,
					Hours = jiraItem.TimeSpentSeconds / 3600,
					Person = jiraItem.Author.DisplayName,
					Task = jiraItem.Task,
					TaskLink = string.Format("https://{0}.atlassian.net/browse/{1}", AtlassianAccount, jiraItem.Task)
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
				client.BaseAddress = new Uri(string.Format("https://{0}.atlassian.net", AtlassianAccount));
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Basic", AuthToken);

				var seachQuery = new SearchQuery();
				seachQuery.Jql = string.Format("project = {0} AND updated >= {1} AND updated <= {2} ORDER BY created DESC", ProjectCode, dateFromStr, dateToStr);
				seachQuery.StartAt = 0;
				seachQuery.MaxResults = 100;
				seachQuery.Fields = new [] {"id"};

				var searchQueryContent = ConvertToJsonContent(seachQuery);

				var searchResponse = await client.PostAsync("/rest/api/2/search", searchQueryContent);

				var searchResultString = await searchResponse.Content.ReadAsStringAsync();
				var searchResult = JsonConvert.DeserializeObject<SearchResponse>(searchResultString);

				Debug.WriteLine("Total issues to read: " + searchResult.Total);
				foreach (var issue in searchResult.Issues)
				{
					Debug.WriteLine("Read issue: " + issue.Key);
					var issueWorklogResponse = await client.GetAsync(string.Format("rest/api/2/issue/{0}/worklog", issue.Key));
					var issueWorklogString = await issueWorklogResponse.Content.ReadAsStringAsync();
					var issueWorklog = JsonConvert.DeserializeObject<IssueWorklogsResponse>(issueWorklogString);
					foreach (var log in issueWorklog.Worklogs)
					{
						log.Task = issue.Key;
					}
					list.AddRange(issueWorklog.Worklogs);
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
