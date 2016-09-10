// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the BaseTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Core.Messages;
using MvvmCross.Core.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.Platform;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Test.Core;
using NUnit.Framework;

namespace Kastil.Core.Tests
{
    /// <summary>
    /// Defines the BaseTest type.
    /// </summary>
    [TestFixture]
    public abstract class BaseTest : MvxIoCSupportingTest
    {
        /// <summary>
        /// The mock dispatcher.
        /// </summary>
        protected MockDispatcher _mockDispatcher;

        protected MvxMessengerHub _messengerHub;

        [SetUp]
        public virtual void SetUp()
        {
            this.ClearAll();

            this._mockDispatcher = new MockDispatcher();

            Ioc.RegisterSingleton<IMvxViewDispatcher>(this._mockDispatcher);
            Ioc.RegisterSingleton<IMvxMainThreadDispatcher>(this._mockDispatcher);
            Ioc.RegisterSingleton<IMvxTrace>(new TestTrace());
            Ioc.RegisterSingleton<IMvxSettings>(new MvxSettings());
            Ioc.RegisterSingleton<IMvxStringToTypeParser>(new MvxStringToTypeParser());

            _messengerHub = new MvxMessengerHub();
            Ioc.RegisterSingleton<IMvxMessenger>(_messengerHub);
            this.Initialize();
            this.CreateTestableObject();
        }
        
        [TearDown]
        public virtual void TearDown()
        {
            this.Terminate();
            _receivedMessages.Clear();
            foreach (var mvxSubscriptionToken in _subscriptionTokens)
            {
                mvxSubscriptionToken.Dispose();
            }
            _subscriptionTokens.Clear();
        }

        public virtual void CreateTestableObject()
        {
        }
       
        protected virtual void Initialize()
        {
        }
        
        protected virtual void Terminate()
        {
        }

        protected void AssertDispatcherCalled<T>(int sequence = 0, Dictionary<string, string> values=null)
        {
            var request = _mockDispatcher.Requests[sequence];
            Assert.That(request.ViewModelType, Is.EqualTo(typeof(T)));

            if (values != null)
            {
                foreach (var kvp in values)
                {
                    Assert.That(request.ParameterValues.ContainsKey(kvp.Key));
                    Assert.That(request.ParameterValues[kvp.Key], Is.EqualTo(kvp.Value));
                }
            }
        }

        protected void AssertDispatcherNotCalled<T>()
        {
            foreach (var request in _mockDispatcher.Requests)
            {
                if (request.ViewModelType == typeof(T))
                    Assert.Fail("ViewModel {0} should not be shown!", typeof(T).Name);
            }
        }

        private T FindHint<T>() where T : MvxPresentationHint
        {
            return _mockDispatcher.Hints.OfType<T>().SingleOrDefault();
        }

        protected void AssertViewModelClosed(IMvxViewModel vm)
        {
            var closeHint = FindHint<MvxClosePresentationHint>();
            Assert.That(closeHint, Is.Not.Null, "ViewModel " + vm.GetType().Name + " is not closed");
            Assert.That(closeHint.ViewModelToClose, Is.EqualTo(vm));
        }

        protected void AssertViewModelNotClosed(IMvxViewModel vm)
        {
            var closeHint = FindHint<MvxClosePresentationHint>();
            if (closeHint != null)
                Assert.That(closeHint.ViewModelToClose, Is.Not.EqualTo(vm));
        }

        protected void SendEvent<T>(T evt) where T : MvxMessage
        {
            _messengerHub.Publish(evt);
        }

        private readonly List<MvxMessage> _receivedMessages = new List<MvxMessage>();
        private readonly List<MvxSubscriptionToken> _subscriptionTokens = new List<MvxSubscriptionToken>();
        protected void Subscribe<T>() where T : MvxMessage
        {
            var token = _messengerHub.Subscribe<T>(OnMessageReceived);
            _subscriptionTokens.Add(token);
        }

        private void OnMessageReceived<T>(T t) where T : MvxMessage
        {
            _receivedMessages.Add(t);
        }


        protected async Task AssertMessageReceived<T>(Func<T, bool> matcher = null) where T : MvxMessage
        {
            await Waiter.WithTimeOut(TimeSpan.FromSeconds(3))
                .Until(MessageReceived<T>);

            foreach (var receivedMessage in _receivedMessages.OfType<T>())
            {
                if (matcher == null || matcher(receivedMessage))
                {
                    Assert.Pass();
                }
            }
            Assert.Fail("Message not found!");
        }

        protected async Task AssertMessageNotReceived<T>(int timeToWait=3) where T : MvxMessage
        {
            var waiter = Waiter.WithTimeOut(TimeSpan.FromSeconds(timeToWait));
            await waiter.Until(MessageReceived<T>);
            Assert.False(waiter.Result);
        }

        private bool MessageReceived<T>() where T : MvxMessage
        {
            return _receivedMessages.OfType<T>().Any();
        }

        protected async Task VerifyPoppedToViewModel<T>()
        {
            await Waiter.WithTimeOut(TimeSpan.FromSeconds(1))
                .Until(() => FindHint<PopToViewModelPresentationHint>() != null);

            var hint = FindHint<PopToViewModelPresentationHint>();
            Assert.That(hint, Is.Not.Null);
            Assert.That(hint.ViewModelType, Is.EqualTo(typeof(T)));
        }

        protected async Task VerifyNotPoppedToViewModel<T>()
        {
            await Waiter.WithTimeOut(TimeSpan.FromSeconds(1))
                .Until(() => FindHint<PopToViewModelPresentationHint>() != null);

            var hint = FindHint<PopToViewModelPresentationHint>();
            if (hint == null)
                Assert.Pass();
            else 
                Assert.That(hint.ViewModelType, Is.Not.EqualTo(typeof(T)));
        }
        
    }
}
