namespace API.Tests;

public class FileTests(AppFactory appFactory) : IntegrationTest(appFactory)
{
  [Fact]
  public Task JsonFile_WhenSentFileAsBase64EncodedString_ItShouldPersistTheFile()
  {
    throw new NotImplementedException();
  }

  [Fact]
  public Task JsonFiles_WhenSentFilesAsBase64EncodedStringPropertyValue_ItShouldPersistTheFiles()
  {
    throw new NotImplementedException();
  }
}