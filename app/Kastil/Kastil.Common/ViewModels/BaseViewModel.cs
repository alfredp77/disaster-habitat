using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Utils;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

namespace Kastil.Common.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        protected TService Resolve<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

		public virtual async Task Initialize()
		{ 
			
		}

        private readonly Dictionary<string, MvxSubscriptionToken> _eventTokens = new Dictionary<string, MvxSubscriptionToken>();

        protected void Subscribe<T>(Action<T> handler) where T : MvxMessage
        {
            Unsubscribe<T>();
            var key = typeof (T).FullName;
            var messenger = Resolve<IMvxMessenger>();
            var newToken = messenger.Subscribe(handler);
            _eventTokens[key] = newToken;
        }

        protected void Unsubscribe<T>() where T : MvxMessage
        {
            var key = typeof(T).FullName;
            MvxSubscriptionToken existing;
            if (_eventTokens.TryGetValue(key, out existing))
            {
                existing.Dispose();
                _eventTokens.Remove(key);
            }
        }

        protected void Publish<T>(T evt) where T : MvxMessage
        {
            var messenger = Resolve<IMvxMessenger>();
            messenger.Publish(evt);
        }

        public virtual void Close()
        {
            foreach (var eventToken in _eventTokens.Values)
            {
                eventToken.Dispose();
            }
            _eventTokens.Clear();
            Close(this);
        }
        
		public bool AllowAddCommand { get; protected set;}

        MvxAsyncCommand _addCommand;
        public MvxAsyncCommand AddCommand
        {
            get
            {
                if (_addCommand == null && AllowAddCommand)
                {
                    _addCommand = new MvxAsyncCommand(DoAddCommand);
                }
                return _addCommand;
            }
        }

        protected virtual Task DoAddCommand()
        {
			var dialog = Resolve<IUserDialogs> ();
			return dialog.AlertAsync ("Test");
        }

		public bool AllowSettingCommand { get; protected set; }
        MvxAsyncCommand _settingCommand;

        public MvxAsyncCommand SettingCommand
        {
            get
            {
                if (_settingCommand == null && AllowSettingCommand)
                {
                    _settingCommand = new MvxAsyncCommand(DoSettingCommand);
                }
                return _settingCommand;
            }
        }

        protected virtual Task DoSettingCommand()
        {
            return Asyncer.DoNothing();
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
    }
}