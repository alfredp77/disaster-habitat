using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Core.Services
{
    public class AssessmentItemHandler : BaseService, IAttributedItemHandler
    {
        private readonly Assessment _assessment;
        public AssessmentItemHandler(Assessment assessment, string disasterId)
        {
            if (assessment == null)
            {
                _assessment = new Assessment { DisasterId = disasterId};
            }
            else
            {
                _assessment = Resolve<IJsonSerializer>().Clone(assessment);
            }
        }

        public Attributed CurrentItem => _assessment;
        public async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(_assessment);
        }

		public Attribute CreateAttributeFrom(Attribute source)
		{
			var serializer = Resolve<IJsonSerializer>();
			var serialized = serializer.Serialize(source);
			return serializer.Deserialize<AssessmentAttribute>(serialized);
		}

        public string NamePlaceholderText => "Enter assessment name";
        public string LocationPlaceholderText => "Where was this assessment made?";
        public string ItemType => typeof(Assessment).Name;
    }
}