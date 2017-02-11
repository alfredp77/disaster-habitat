using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kastil.TestUtils
{
    public class Waiter
    {
        public static Waiter WithTimeOut(TimeSpan timeout)
        {
            var waiter = new Waiter();
            waiter._timeout = timeout;
            return waiter;
        }

        public static Waiter Wait()
        {
            return new Waiter();
        }


        private Func<bool> _condition;
        private TimeSpan _timeout = TimeSpan.FromMinutes(1);

        private Waiter()
        {
        }

        private Task<bool> _waiter;
        private CancellationTokenSource _cancellationTokenSource;
        public async Task Until(Func<bool> condition)
        {
            _condition = condition;
            _cancellationTokenSource = new CancellationTokenSource(_timeout);
            _waiter = Task.Run(async () =>
            {
                while (!_condition() && !_cancellationTokenSource.IsCancellationRequested)
                    await Task.Delay(200);
                return _condition();
            }, _cancellationTokenSource.Token);

            Result = await _waiter;
        }


        public bool Result { get; private set; }

        public async Task Stop()
        {
            if (_waiter == null)
                return;
            _cancellationTokenSource.Cancel();
            await _waiter;
        }
    }
}
