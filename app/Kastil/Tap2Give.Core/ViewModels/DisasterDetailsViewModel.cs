using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Models;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.WebBrowser;
using Tap2Give.Core.Services;

namespace Tap2Give.Core.ViewModels
{
    public class DisasterDetailsViewModel : BaseViewModel
    {
        private static readonly Random _random = new Random();
        public string SelectText => Messages.AmountToDonate;

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        private string _description = "abc";
        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged(); }
        }
        private string _location;
        public string Location
        {
            get { return _location; }
            private set { _location = value; RaisePropertyChanged(); }
        }

		private string _aidDetails;
		public string AidDetails {
			get { return _aidDetails; }
			private set { _aidDetails = value; RaisePropertyChanged(); }
		}

        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; RaisePropertyChanged(); }
        }
        public ObservableRangeCollection<string> AidValues { get; } = new ObservableRangeCollection<string>();

        public override Task Initialize()
        {
            return Load();
        }

        private Disaster _disaster;
        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.Loading);

            var context = Resolve<IDisasterContext>();
            _disaster = context.Disaster;
            Name = _disaster.Name;
			Title = Name;
            AllowCancelCommand = true;
            Description = _disaster.Description;
            ImageUrl = _disaster.ImageUrl;
            Location = _disaster.Location;

            try
            {
                AidValues.Clear();
                AidValues.AddRange(_disaster.DisasterAids.GroupBy(d => d.DollarValue)                                   
				                   .Select(g => new { value = g.Key.GetTrailingNumbers(), displayText = GetDonationDisplayText(g) })
                                   .OrderBy(g => g.value)
                                   .Select(g => g.displayText));
				AidDetails = string.Join(Environment.NewLine, AidValues);
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load disaster details, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load disaster details. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }

		private string GetDonationDisplayText(IEnumerable<DisasterAid> aidDetails)
		{
			var asList = aidDetails.ToList();
			var aidDetail = asList[_random.Next(asList.Count)];
			return $"{aidDetail.DollarValue} - {aidDetail.Description}";
		}

        public MvxCommand CancelCommand => new MvxCommand(Close);

        public MvxCommand DonateCommand => new MvxCommand(DoDonateCommand);
        private void DoDonateCommand()
        {
            try
            {
                var task = Mvx.Resolve<IMvxWebBrowserTask>();
                task.ShowWebPage(_disaster.GiveUrl);                
            }
            catch (Exception ex)
            {
                Mvx.Trace("Unable to launch browser. Exception: {0}", ex);
                var dialog = Mvx.Resolve<IUserDialogs>();                
                dialog.AlertAsync("Unable to launch browser. Please try again");
            }
        }
    }
}


