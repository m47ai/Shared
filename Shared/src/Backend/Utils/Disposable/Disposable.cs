namespace M47.Shared.Utils.Disposable;

using System;

public abstract class Disposable : IDisposable
{
    private bool _disposed = false;

    ~Disposable() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            DisposeManagedResources();
        }

        _disposed = true;
    }

    public abstract void DisposeManagedResources();
}