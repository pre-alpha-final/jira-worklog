using System.Collections.Generic;
using Newtonsoft.Json;

namespace JiraWorklog.Core.Models
{
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
}