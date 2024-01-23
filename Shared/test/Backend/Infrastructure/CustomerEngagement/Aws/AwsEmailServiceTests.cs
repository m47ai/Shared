namespace M47.Shared.Tests.Infrastructure.CustomerEngagement.Aws;

using Amazon.SimpleEmail;
using M47.Shared.Domain.CustomerEngagement;
using M47.Shared.Infrastructure.CustomerEngagement;
using M47.Shared.Tests.Factory;
using System.Net;
using System.Threading.Tasks;

[Collection(nameof(SharedFactoryShared))]
public sealed class AwsEmailServiceTests
{
    private readonly AwsEmailService _emailService;
    private readonly IAmazonSimpleEmailService _client;

    public AwsEmailServiceTests(SharedFactory factory)
    {
        _client = factory.Localstack.AwsHelper.CreateSesClient();
        _emailService = new AwsEmailService(_client);
    }

    [Fact]
    public async Task Should_SendEmail_When_EmailParamsAreValid()
    {
        // Arrange
        var fromEmail = "no-reply@m47labs.com";
        var attachedFile = new AttachedFile[] { AttachedFile.Create("name of attached file", @"https://www.google.com") };
        var request = EmailRequest.Create(fromAddress: fromEmail,
                                          toAddress: new[] { "some@mail.com" },
                                          subject: "This is the subject",
                                          body: "This is the body",
                                          attachedFiles: attachedFile);

        // Act
        var status = await _emailService.SendEmailAsync(request);

        // Assert
        Assert.True(status == HttpStatusCode.OK);
    }
}