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
        readonly CancellationTokenSource _cts = new();
        private RedLockFactory _distributedLockFactory;
        private const string _resource = "the-thing-we-are-locking-on";
        private const int _expirySecondsCount = 10;
        private readonly TimeSpan _expiry = TimeSpan.FromSeconds(_expirySecondsCount);
        private const int _retrySecondsCount = 3;
        private readonly TimeSpan _retry = TimeSpan.FromSeconds(_retrySecondsCount);

        public async Task Start()
        {
            var endpoint = new RedLockEndPoint(new DnsEndPoint("localhost", 49153));
            endpoint.Password = "redispw";
            var endPoints = new List<RedLockEndPoint>
            {
                endpoint
                //todo add more redis
            };
            _distributedLockFactory = RedLockFactory.Create(endPoints);
            try
            {
                await TryAcquireLock(_cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _distributedLockFactory.Dispose();
                throw;
            }
        }

        private async Task TryAcquireLock(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            try
            {
                var distributedLock = await _distributedLockFactory.CreateLockAsync(
                    _resource,
                    _expiry,
                    TimeSpan.MaxValue,
                    _retry,
                    token);
                if (distributedLock.IsAcquired)
                {
                    DoLeaderJob();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void DoLeaderJob()
        {
            Console.WriteLine("ya cherv lider luchshe menya net");
        }
    }
}