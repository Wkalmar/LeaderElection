using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace LeaderElection
{
    internal class LeaderElectionService
    {
        private Timer _acquireLockTimer;
        readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private RedLockFactory _distributedLockFactory;
        private const string _resource = "the-thing-we-are-locking-on";
        private const int _expirySecondsCount = 3;
        private readonly TimeSpan _expiry = TimeSpan.FromSeconds(_expirySecondsCount);

        public void Start()
        {
            var endPoints = new List<RedLockEndPoint>
            {
                new DnsEndPoint("localhost", 6379)
                //todo add more redis
            };
            _distributedLockFactory = RedLockFactory.Create(endPoints);
            _acquireLockTimer = new Timer(async state => await TryAcquireLock((CancellationToken)state), _cts.Token, 0, _expirySecondsCount * 1000);
        }

        private async Task TryAcquireLock(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var distributedLock = await _distributedLockFactory.CreateLockAsync(_resource, _expiry);
            if (distributedLock.IsAcquired)
            {
                DoLeaderJob();
                _acquireLockTimer.Dispose(); //no need to renew lock because of autoextend
            }
            else
            {
                DoSlaveJob();
            }
        }

        private static void DoSlaveJob()
        {
            Console.WriteLine("ya cherv pidor huzhe menya net");
        }

        private static void DoLeaderJob()
        {
            Console.WriteLine("ya cherv lider luchshe menya net");
        }
    }
}