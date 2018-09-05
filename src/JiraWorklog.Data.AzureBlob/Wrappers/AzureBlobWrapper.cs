using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace JiraWorklog.Data.AzureBlob.Wrappers
{
	public class AzureBlobWrapper
	{
		private static readonly SemaphoreSlim InitializeSemaphore = new SemaphoreSlim(1);
		private readonly CloudBlobContainer _cloudBlobContainer;
		private bool _initialized;

		public AzureBlobWrapper(string connectionString, string blobContainerName)
		{
			var storageAccount = CloudStorageAccount.Parse(connectionString);
			var cloudBlobClient = storageAccount.CreateCloudBlobClient();
			_cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
		}

		public async Task Upload(string blobName, string text)
		{
			await Initialize();

			var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(blobName);
			await cloudBlockBlob.UploadTextAsync(text);
		}

		public async Task<string> Download(string blobName)
		{
			await Initialize();

			var cloudBlockBlob = _cloudBlobContainer.GetBlockBlobReference(blobName);
			return await cloudBlockBlob.DownloadTextAsync();
		}

		private async Task Initialize()
		{
			await InitializeSemaphore.WaitAsync();
			try
			{
				if (_initialized)
					return;
				_initialized = true;
			}
			finally
			{
				InitializeSemaphore.Release();
			}

			await _cloudBlobContainer.CreateIfNotExistsAsync();

			var permissions = new BlobContainerPermissions
			{
				PublicAccess = BlobContainerPublicAccessType.Blob
			};
			await _cloudBlobContainer.SetPermissionsAsync(permissions);
		}
	}
}
