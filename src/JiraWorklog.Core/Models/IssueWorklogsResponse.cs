using System.Collections.Generic;
using Newtonsoft.Json;

namespace JiraWorklog.Core.Models
{
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
}