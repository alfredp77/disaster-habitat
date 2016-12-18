using System;
using MvvmCross.Plugins.Messenger;

namespace Kastil.Core.Tests
{
    public class MessengerTestHelper<T> : IDisposable where T : MvxMessage
    {
        private readonly IMvxMessenger _messenger;
        private MvxSubscriptionToken _token;

        public MessengerTestHelper(IMvxMessenger messenger)
        {
            _messenger = messenger;
            _token = _messenger.Subscribe<T>(t => Message = t);
        }

        public T Message { get; private set; }

        public void Dispose()
        {
            _token.Dispose();
        }
    }
}