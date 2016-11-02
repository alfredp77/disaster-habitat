using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.WebBrowser;

namespace Tap2Give.Core.ViewModels
{
    public class DisasterIncidentAidViewModel : BaseViewModel
    {
        private static readonly Random _random = new Random();

        public Disaster DisasterIncident { get; set; }
        public string SelectText { get; set; }
        public string DisasterId { get; set; }
        public string Name { get { return DisasterIncident.Name; } }
        public string Description { get { return DisasterIncident.Description; } }
        public string ImageUrl { get { return DisasterIncident.ImageUrl; } }
        
        
        public ObservableRangeCollection<DisasterIncidentAid> DisasterAidItems { get; } = new ObservableRangeCollection<DisasterIncidentAid>();

        public void Init(Disaster disaster)
        {
            DisasterIncident = disaster;
        }

        public List<string> AidValues
        {
            get
            {
                return DisasterAidItems.GroupBy(d => d.DollarValue).Select(g => GetDollarValue(g.Key, g.ToList())).ToList();            
            }
        }

        private string GetDollarValue(string key, List<DisasterIncidentAid> textForValue)
        {
            return key + " - " + textForValue[_random.Next(textForValue.Count)];
        }

        public override Task Initialize()
        {
            return Load();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.Loading);

            var tapToGiveService = Resolve<ITap2HelpService>();
            try
            {
                Title = Name;
                SelectText = Messages.AmountToDonate;
                var incidentAids = await tapToGiveService.GetAidsForDisaster(DisasterId);
                DisasterAidItems.AddRange(incidentAids);
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Assessment, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Assessment. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }

        public DisasterIncidentAidViewModel(Disaster value)
        {
            DisasterIncident = value;
        }       

        public MvxCommand CancelCommand => new MvxCommand(Close);

        public MvxCommand DonateCommand => new MvxCommand(DoDonateCommand);
        private void DoDonateCommand()
        {
            try
            {
                PluginLoader.Instance.EnsureLoaded();
                var task = Mvx.Resolve<IMvxWebBrowserTask>();
                task.ShowWebPage(DisasterIncident.GiveUrl);

                /*
                //TODO: Start Browser with the Value.GiveUrl
                var uri = Android.Net.Uri.Parse(Value.GiveUrl);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
                */

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
