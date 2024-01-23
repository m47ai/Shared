namespace M47.Shared.Application.MediatR.Behaviors;

using System;

public interface ICacheRequest
{
    string CacheKey { get; }
    DateTime? AbsoluteExpirationRelativeToNow { get; }
}