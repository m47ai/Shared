namespace M47.Shared.Domain.Api;

using System.Collections.Generic;

public class ValidationFailureResponse
{
    public IEnumerable<ValidationResponse> Errors { get; init; } = Array.Empty<ValidationResponse>();
}

public class ValidationResponse
{
    public string PropertyName { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}