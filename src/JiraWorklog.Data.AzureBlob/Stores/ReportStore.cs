using System.Collections.Generic;
using System.Threading.Tasks;
using JiraWorklog.Core.DataTransferObjects;
using JiraWorklog.Core.Stores;
using JiraWorklog.Data.AzureBlob.Wrappers;
using Newtonsoft.Json;

namespace JiraWorklog.Data.AzureBlob.Stores
{
	public class ReportStore : IReportStore
	{
		private const string ConnectionString = "";
		private const string BlobContainerName = "";

		private readonly AzureBlobWrapper _azureBlobWrapper;

		public ReportStore()
		{
			_azureBlobWrapper = new AzureBlobWrapper(
				ConnectionString,
				BlobContainerName);
		}

		public Task Store(ReportDto dto)
		{
			return _azureBlobWrapper.Upload(
				$"{dto.Month}-{dto.Year}",
				JsonConvert.SerializeObject(dto.Logs));
		}

		public async Task<ReportDto> FindByDate(int month, int year)
		{
			var content = await _azureBlobWrapper.Download(
				$"{month}-{year}");

			var logs = JsonConvert.DeserializeObject<IList<WorklogDto>>(content);

			return new ReportDto
			{
				Month = month,
				Year = year,
				Logs = logs
			};
		}
	}
}
