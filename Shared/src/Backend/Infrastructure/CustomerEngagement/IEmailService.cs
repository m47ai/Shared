namespace M47.Shared.Infrastructure.CustomerEngagement;

using M47.Shared.Domain.CustomerEngagement;
using System.Net;
using System.Threading.Tasks;

public interface IEmailService
{
    Task<HttpStatusCode> SendEmailAsync(EmailRequest request, CancellationToken cancellationToken = default);
}