using System;

namespace JiraWorklog.Core.DataTransferObjects
{
	public class WorklogDto
	{
		public string Person { get; set; }
		public string Task { get; set; }
		public string TaskLink { get; set; }
		public int Hours { get; set; }
		public DateTime DateTime { get; set; }
	}
}
