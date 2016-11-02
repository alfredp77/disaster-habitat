using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Common.Models;
using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Core.Services
{
    public interface IShelterEditContext : IItemEditContext
    {
        Shelter Item { get; }
        void Initialize(string disasterId = null);
        void Initialize(Shelter item = null, string disasterId = null);
        Task DeleteShelter();
    }

    public class ShelterEditContext : BaseService, IShelterEditContext
    {
        private Dictionary<string, Attribute> _attributesMap;        
        public Attribute SelectedAttribute { get; set; }
        public Shelter Item { get; private set; }
        public bool IsNew { get; private set; }

        public void Initialize(string disasterId = null)
        {
            IsNew = true;
            Item = new Shelter { Id = Guid.NewGuid().ToString(), DisasterId = disasterId };

            _attributesMap = Item.Attributes.ToDictionary(k => k.Key);
        }

        public void Initialize(Shelter shelter = null, string disasterId=null)
        {
            IsNew = shelter == null;
			if (shelter == null) 
			{
                Item = new Shelter { Id = Guid.NewGuid().ToString(), DisasterId = disasterId };
			} 
			else 
			{
				var serializer = Resolve<IJsonSerializer>();
                Item = serializer.Clone(shelter);
			}
            
            _attributesMap = Item.Attributes.ToDictionary(k => k.Key);
        }

        public void AddOrUpdateAttribute(Attribute attribute, string value)
        {
            Attribute attr;
            if (!_attributesMap.TryGetValue(attribute.Key, out attr))
            {
                var serializer = Resolve<IJsonSerializer>();
                attr = serializer.Clone(attribute);
                _attributesMap.Add(attribute.Key, attr);
                Item.Attributes.Add(attr);
            }

            attr.Value = value;
        }

        public void DeleteAttribute(string attributeName)
        {
            Attribute attr;
            if (_attributesMap.TryGetValue(attributeName, out attr))
            {
                _attributesMap.Remove(attributeName);
                Item.Attributes.Remove(attr);
            }
        }

        public async Task DeleteShelter()
        {
            var service = Resolve<ITap2HelpService>();
            await service.DeleteShelter(Item.Id);
        }

        public async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(Item);
        }
    }
}
