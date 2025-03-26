using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Entry;
using organization_back_end.Services;
using Xunit;

namespace organization_back_end.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _mockBlobService = new Mock<IBlobService>();
            _fileService = new FileService(_mockBlobService.Object);
        }

        [Fact]
        public async Task UploadFileAsync_ValidFile_ReturnsFileName()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = new MemoryStream();
            var writer = new StreamWriter(content);
            writer.Write("Test file content");
            writer.Flush();
            content.Position = 0;

            fileMock.Setup(_ => _.OpenReadStream()).Returns(content);
            fileMock.Setup(_ => _.Length).Returns(content.Length);

            var request = new FileRequest { Name = "testFile", Extension = ".pdf" };
            var fileId = Guid.NewGuid();
            var expectedFileName = $"{fileId}-testFile.pdf";

            _mockBlobService.Setup(b => b.UploadFileAsync(It.IsAny<IFormFile>(), expectedFileName))
                .ReturnsAsync(expectedFileName);

            // Act
            var result = await _fileService.UploadFileAsync(fileMock.Object, request, fileId);

            // Assert
            Assert.Equal(expectedFileName, result);
        }

        [Fact]
        public async Task UploadFileAsync_InvalidExtension_ThrowsException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.Length).Returns(100);
            var request = new FileRequest { Name = "testFile", Extension = ".exe" };
            var fileId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _fileService.UploadFileAsync(fileMock.Object, request, fileId));
        }

        [Fact]
        public async Task UploadFileAsync_NullFile_ReturnsEmptyString()
        {
            // Arrange
            var request = new FileRequest { Name = "testFile", Extension = ".pdf" };
            var fileId = Guid.NewGuid();

            // Act
            var result = await _fileService.UploadFileAsync(null, request, fileId);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task UploadFileAsync_FileZeroLength_ReturnsEmptyString()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var request = new FileRequest { Name = "testFile", Extension = ".pdf" };
            var fileId = Guid.NewGuid();

            // Act
            var result = await _fileService.UploadFileAsync(fileMock.Object, request, fileId);

            // Assert
            Assert.Equal(string.Empty, result);
        }



        [Fact]
        public async Task DownloadFileAsync_ValidFile_ReturnsFileStreamResult()
        {
            // Arrange
            var fileName = "testFile.pdf";
            var fileStreamResult = new FileStreamResult(new MemoryStream(), "application/pdf");

            _mockBlobService.Setup(b => b.DownloadFileAsync(fileName)).ReturnsAsync(fileStreamResult);

            // Act
            var result = await _fileService.DownloadFileAsync(fileName);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<FileStreamResult>(result);
        }

        [Fact]
        public async Task DeleteFileAsync_ValidFile_CallsBlobService()
        {
            // Arrange
            var fileName = "testFile.pdf";

            // Act
            await _fileService.DeleteFileAsync(fileName);

            // Assert
            _mockBlobService.Verify(b => b.DeleteFileAsync(fileName), Times.Once);
        }
    }
}
