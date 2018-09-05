using Newtonsoft.Json;

namespace JiraWorklog.Core.Models
{
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
}