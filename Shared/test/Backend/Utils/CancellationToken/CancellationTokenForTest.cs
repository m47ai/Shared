namespace M47.Shared.Tests.Utils.CancellationToken;

using System.Threading;

public static class CancellationTokenForTest
{
    public static CancellationToken Canceled
    {
        get
        {
            var token = new CancellationTokenSource();
            token.Cancel();

            return token.Token;
        }
    }

    public static CancellationToken CreateDefault => CancelAfter(seconds: 60);

    public static CancellationToken CancelAfter(int seconds)
        => new CancellationTokenSource(seconds * 1000).Token;
}