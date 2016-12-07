using System;
using System.Threading;

namespace MediumProxy
{
    internal class RwLockScope : IDisposable
    {
        private static readonly object Sync = new object();
        private static volatile ReaderWriterLockSlim _rwLockSlim;

        static RwLockScope()
        {
        }

        public RwLockScope(RwLockScopes rwLockScopes = RwLockScopes.ReadOnly, int lockWaitTimeoutSecond = 10)
        {
            if (rwLockScopes == RwLockScopes.Undefined)
                throw new ArgumentOutOfRangeException(nameof(rwLockScopes), "'RwLockScopes.Undefined' is unacceptable value.");

            if (_rwLockSlim == null)
                lock (Sync)
                {
                    if (_rwLockSlim == null)
                    {
                        _rwLockSlim = new ReaderWriterLockSlim();
                    }
                }

            switch (rwLockScopes)
            {
                case RwLockScopes.ReadOnly:
                    _rwLockSlim.TryEnterReadLock(lockWaitTimeoutSecond);
                    break;
                case RwLockScopes.Upgradeable:
                    _rwLockSlim.TryEnterUpgradeableReadLock(lockWaitTimeoutSecond);
                    break;
                case RwLockScopes.Write:
                    _rwLockSlim.TryEnterWriteLock(lockWaitTimeoutSecond);
                    break;
                case RwLockScopes.WriteWithUpgradeable:
                    _rwLockSlim.TryEnterUpgradeableReadLock(lockWaitTimeoutSecond);
                    try
                    {
                        _rwLockSlim.TryEnterWriteLock(lockWaitTimeoutSecond);
                    }
                    catch (Exception)
                    {
                        _rwLockSlim.ExitUpgradeableReadLock();
                        throw;
                    }
                    break;
            }
        }


        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_rwLockSlim.IsWriteLockHeld)
                    _rwLockSlim.ExitWriteLock();

                if (_rwLockSlim.IsUpgradeableReadLockHeld)
                    _rwLockSlim.ExitUpgradeableReadLock();

                if (_rwLockSlim.IsReadLockHeld)
                    _rwLockSlim.ExitReadLock();;
            }

            _disposed = true;
        }
    }

    internal enum RwLockScopes
    {
        Undefined,
        ReadOnly,
        Upgradeable,
        Write,
        WriteWithUpgradeable
    }
}
