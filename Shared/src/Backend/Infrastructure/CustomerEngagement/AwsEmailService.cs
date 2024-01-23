namespace M47.Shared.Infrastructure.CustomerEngagement;

using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using M47.Shared.Domain.CustomerEngagement;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class AwsEmailService : IEmailService
{
    private readonly IAmazonSimpleEmailService _client;

    public AwsEmailService(IAmazonSimpleEmailService client)
    {
        _client = client;
    }

    public async Task<HttpStatusCode> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default)
    {
        var emailRequest = GetEmailRequest(request);
        var response = await _client.SendEmailAsync(emailRequest, cancellationToken);

        return response.HttpStatusCode;
    }

    private static SendEmailRequest GetEmailRequest(EmailRequest request)
    {
        var destination = new Destination(request.ToAddresses.ToList())
        {
            BccAddresses = request.BccAddresses?.ToList()
        };

        var contentSubject = new Content(request.Subject);
        var bodyContent = new Content(request.Body + AddAttachedFiles(request));
        var contentBody = new Body();

        if (request.IsHTML)
        {
            contentBody.Html = bodyContent;
        }
        else
        {
            contentBody.Text = bodyContent;
        }

        var message = new Message(contentSubject, contentBody);

        return new SendEmailRequest(request.FromAddress, destination, message);
    }

    private static string? AddAttachedFiles(EmailRequest request)
    {
        if (!request.IsHTML || request.AttachedFiles is null || !request.AttachedFiles.Any())
        {
            return default;
        }

        var builder = new StringBuilder();

        foreach (var file in request.AttachedFiles)
        {
            builder.Append("<table><tr><td><a href=\"")
                   .Append(file.UrlToDownload)
                   .Append("\"></a></td></tr><tr><td><a href=\"")
                   .Append(file.UrlToDownload)
                   .Append("\">")
                   .Append(file.Name)
                   .Append("</a></td></tr></table><br><br>");
        }

        return builder.ToString();
    }
}