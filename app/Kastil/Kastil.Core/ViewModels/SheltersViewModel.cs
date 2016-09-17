using System.Threading.Tasks;

namespace Kastil.Core.ViewModels
{
    public class SheltersViewModel : BaseViewModel
    {
        public void Init(string disasterId)
        {
        }

		protected override Task DoSettingCommand ()
		{
			return Task.Factory.StartNew(Close);
		}
    }
}