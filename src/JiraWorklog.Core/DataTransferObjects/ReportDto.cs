using System.Collections.Generic;

namespace JiraWorklog.Core.DataTransferObjects
{
	public class ReportDto
	{
		public int Month { get; set; }
		public int Year { get; set; }
		public IList<WorklogDto> Logs { get; set; }
	}
}
