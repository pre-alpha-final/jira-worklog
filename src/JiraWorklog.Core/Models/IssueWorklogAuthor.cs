using Newtonsoft.Json;

namespace JiraWorklog.Core.Models
{
	public class IssueWorklogAuthor
	{
		[JsonProperty("displayName")]
		public string DisplayName { get; set; }
	}
}