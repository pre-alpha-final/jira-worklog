using System.Collections.Generic;
using Newtonsoft.Json;

namespace JiraWorklog.Core.Models
{
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
}