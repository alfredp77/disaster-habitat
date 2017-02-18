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
    public class ShelterItemHandler : BaseService, IAttributedItemHandler
    {
        private readonly Shelter _shelter;
        public ShelterItemHandler(Shelter shelter, string disasterId)
        {
            if (shelter == null)
            {
                _shelter = new Shelter { DisasterId = disasterId };
            }
            else
            {
                _shelter = Resolve<IJsonSerializer>().Clone(shelter);
            }
        }

        public Attributed CurrentItem => _shelter;
        public async Task CommitChanges(IEnumerable<ValuedAttribute> modifiedAttributes, IEnumerable<ValuedAttribute> deletedAttributes)
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(_shelter);
			foreach (var modifiedAttribute in modifiedAttributes.OfType<ShelterAttribute>())
            {
				modifiedAttribute.ItemId = _shelter.ObjectId;
                await service.SaveShelterAttribute(modifiedAttribute);
            }
            foreach (var deletedAttribute in deletedAttributes.OfType<ShelterAttribute>())
            {
                await service.DeleteShelterAttribute(deletedAttribute.ObjectId);
            }

        }

        public ValuedAttribute CreateAttributeFrom(Attribute source)
		{
		    return source.CreateValuedAttribute<ShelterAttribute>();
		}

        public async Task<IEnumerable<ValuedAttribute>> GetAttributes()
        {
            var service = Resolve<ITap2HelpService>();
            return await service.GetShelterAttributes(_shelter.ObjectId);
        }

        public string NamePlaceholderText => "Enter shelter name";
        public string LocationPlaceholderText => "Where is this shelter located?";
        public string ItemType => typeof(Shelter).Name;
    }
}