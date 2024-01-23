namespace M47.Shared.Domain.CustomerEngagement;

using System.Collections.Generic;
using System.Threading;

public class EmailRequest
{
    public static EmailRequest Create(string fromAddress, string[] toAddress, string subject, string body,
                                      IEnumerable<AttachedFile> attachedFiles)
    {
        return new EmailRequest(fromAddress, toAddress, subject, body, attachedFiles);
    }

    public string FromAddress { get; }

    public string[] ToAddresses { get; }

    public string[]? BccAddresses { get; }

    public string Subject { get; }

    public string Body { get; }

    public bool IsHTML { get; }

    public IEnumerable<AttachedFile> AttachedFiles { get; }

    public CancellationToken CancellationToken { get; }

    public EmailRequest(string fromAddress, string[] toAddress, string subject, string body,
                        IEnumerable<AttachedFile> attachedFiles)
    {
        FromAddress = fromAddress;
        BccAddresses = null;
        ToAddresses = toAddress;
        Subject = subject;
        Body = body;
        IsHTML = true;
        AttachedFiles = attachedFiles;
        CancellationToken = default;
    }
}