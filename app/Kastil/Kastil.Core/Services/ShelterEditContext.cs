using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Core.Services
{
    public interface IShelterEditContext
    {
        Shelter Item { get; }
        Attribute SelectedAttribute { get; set; }
        bool IsNew { get; }

        void Initialize(string disasterId = null);
        void Initialize(Shelter item = null, string disasterId = null);
        void AddOrUpdateAttribute(Attribute attribute, string value);
        void DeleteAttribute(string attributeName);
        Task DeleteShelter();
        Task CommitChanges();
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

        public void Initialize(Shelter Shelter = null, string disasterId=null)
        {
            IsNew = Shelter == null;
			if (Shelter == null) 
			{
				Shelter = new Shelter { Id = Guid.NewGuid ().ToString () , DisasterId = disasterId };
			} 
			else 
			{
				var serializer = Resolve<IJsonSerializer>();
				Shelter = serializer.Clone(Shelter);
			}
            
            _attributesMap = Shelter.Attributes.ToDictionary(k => k.Key);
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
