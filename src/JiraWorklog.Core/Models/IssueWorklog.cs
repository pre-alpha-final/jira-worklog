using System;
using Newtonsoft.Json;

namespace JiraWorklog.Core.Models
{
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
}