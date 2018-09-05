using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;

namespace JiraWorklog.Core.Stores
{
	public interface IReportStore
	{
		Task Store(ReportDto dto);
		Task<ReportDto> FindByDate(int month, int year);
	}
}
