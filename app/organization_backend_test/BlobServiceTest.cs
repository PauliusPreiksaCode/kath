using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Azure;
using organization_back_end.Services;

namespace organization_back_end.Tests.Services
{
    public class BlobServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<BlobServiceClient> _mockBlobServiceClient;
        private Mock<BlobContainerClient> _mockContainerClient;
        private Mock<BlobClient> _mockBlobClient;
        private Mock<IFormFile> _mockFormFile;

        public BlobServiceTests()
        {
            // Setup mock configuration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["Blob:Key"]).Returns("key");
            _mockConfiguration.Setup(c => c["Blob:ContainerName"]).Returns("files");

            // Setup mock blob service and container clients
            _mockBlobServiceClient = new Mock<BlobServiceClient>();
            _mockContainerClient = new Mock<BlobContainerClient>();
            _mockBlobClient = new Mock<BlobClient>();

            // Setup mock form file
            _mockFormFile = new Mock<IFormFile>();
        }

        private BlobService CreateBlobService()
        {
            // Setup BlobServiceClient to return mock container client
            _mockBlobServiceClient
                .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_mockContainerClient.Object);

            // Setup container client to return mock blob client
            _mockContainerClient
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(_mockBlobClient.Object);

            // Create the BlobService with the mocked configuration
            var blobService = new BlobService(_mockConfiguration.Object);

            // Use reflection to set the _containerClient field
            var containerClientField = typeof(BlobService).GetField("_containerClient",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            containerClientField?.SetValue(blobService, _mockContainerClient.Object);

            // Setup the ExistsAsync method for the mock blob client
            _mockBlobClient
                .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

            return blobService;
        }

        [Fact]
        public async Task UploadFileAsync_SuccessfulUpload_ReturnsUri()
        {
            // Arrange
            var service = CreateBlobService();
            var fullname = "test-file.txt";
            var uri = new Uri("https://kath.blob.core.windows.net/files/test-file.txt");

            // Setup mock form file
            var mockStream = new MemoryStream(new byte[] { 1, 2, 3 });
            _mockFormFile
                .Setup(f => f.OpenReadStream())
                .Returns(mockStream);

            // Setup blob client upload
            _mockBlobClient
                .Setup(x => x.Uri)
                .Returns(uri);
            _mockBlobClient
                .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<bool>(), CancellationToken.None))
                .Returns(Task.FromResult(Mock.Of<Response<BlobContentInfo>>()));

            // Act
            var result = await service.UploadFileAsync(_mockFormFile.Object, fullname);

            // Assert
            Assert.Equal(uri.ToString(), result);
        }

        [Fact]
        public async Task DownloadFileAsync_FileExists_ReturnsFileStreamResult()
        {
            // Arrange
            var service = CreateBlobService();
            var fullname = "test-file.txt";

            // Setup exists check
            _mockBlobClient
                .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

            // Setup download method using Mock.Of to create a proper Response
            _mockBlobClient
                .Setup(x => x.DownloadToAsync(It.IsAny<Stream>()))
                .Returns(Task.FromResult(Mock.Of<Response>()));

            // Act
            var result = await service.DownloadFileAsync(fullname);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-file.txt", result.FileDownloadName);
            Assert.Equal("application/octet-stream", result.ContentType);

            // Verify method calls
            _mockBlobClient.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockBlobClient.Verify(x => x.DownloadToAsync(It.IsAny<Stream>()), Times.Once);
        }

        [Fact]
        public async Task DownloadFileAsync_FileNotExists_ThrowsException()
        {
            // Arrange
            var service = CreateBlobService();
            var fullname = "non-existent-file.txt";

            // Setup blob exists check using Mock.Of to create a Response<bool>
            var existsResponse = Mock.Of<Response<bool>>(r => r.Value == false);
            _mockBlobClient
                .Setup(x => x.ExistsAsync(CancellationToken.None))
                .ReturnsAsync(existsResponse);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DownloadFileAsync(fullname));
        }

        [Fact]
        public async Task DeleteFileAsync_FileExists_DeletesCalled()
        {
            // Arrange
            var service = CreateBlobService();
            var fullname = "test-file.txt";
            BlobRequestConditions conditions = new BlobRequestConditions();


            // Setup blob exists check using Mock.Of to create a Response<bool>
            var existsResponse = Mock.Of<Response<bool>>(r => r.Value == true);
            _mockBlobClient
                .Setup(x => x.ExistsAsync(CancellationToken.None))
                .ReturnsAsync(existsResponse);

            // Setup delete
            _mockBlobClient
                .Setup(x => x.DeleteAsync(DeleteSnapshotsOption.None, conditions, CancellationToken.None))
                .Returns(Task.FromResult(Mock.Of<Response>()));

            // Act
            await service.DeleteFileAsync(fullname);

            // Assert
            _mockBlobClient.Verify(x => x.ExistsAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteFileAsync_FileNotExists_DoesNotThrow()
        {
            // Arrange
            var service = CreateBlobService();
            var fullname = "non-existent-file.txt";

            // Setup blob exists check WITHOUT specifying a cancellation token
            //_mockBlobClient
            //    .Setup(x => x.ExistsAsync(default))
            //    .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

            // Act
            await service.DeleteFileAsync(fullname);

            // Assert
            _mockBlobClient.Verify(x => x.ExistsAsync(default), Times.Once);
        }
    }
}