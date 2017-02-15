using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task CommitChanges(IEnumerable<ValuedAttribute> modifiedAttributes, IEnumerable<ValuedAttribute> deletedAttributes)
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(_assessment);

			foreach (var modifiedAttribute in modifiedAttributes.OfType<AssessmentAttribute>())
            {
				modifiedAttribute.ItemId = _assessment.ObjectId;
                await service.SaveAssessmentAttribute(modifiedAttribute);
            }
            foreach (var deletedAttribute in deletedAttributes.OfType<AssessmentAttribute>())
            {
                await service.DeleteAssessmentAttribute(deletedAttribute.ObjectId);
            }

        }

		public ValuedAttribute CreateAttributeFrom(Attribute source)
		{
		    return source.CreateValuedAttribute<AssessmentAttribute>();
		}

        public async Task<IEnumerable<ValuedAttribute>> GetAttributes()
        {
            var service = Resolve<ITap2HelpService>();
            return await service.GetAssessmentAttributes(_assessment.ObjectId);
        }

        public string NamePlaceholderText => "Enter assessment name";
        public string LocationPlaceholderText => "Where was this assessment made?";
        public string ItemType => typeof(Assessment).Name;
    }
}