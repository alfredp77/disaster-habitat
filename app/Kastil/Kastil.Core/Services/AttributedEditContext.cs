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
    public class AttributedEditContext : BaseService
    {
        private Dictionary<string, ValuedAttribute> _attributesMap;
        public ValuedAttribute SelectedAttribute { get; set; }
        public Attributed Item { get; private set; }

        public string ItemName
        {
            get { return Item?.Name; }
            set
            {
                if (Item != null)
                    Item.Name = value;
            }
        }

        public string ItemLocation
        {
            get { return Item?.Location; }
            set
            {
                if (Item != null)
                    Item.Location = value;
            }
        }

        public bool IsNew { get; private set; }

        private List<ValuedAttribute> _modifiedAttributes;
        public IEnumerable<ValuedAttribute> ValuedAttributes
        {
            get
            {
                if (_modifiedAttributes == null)
                    return Enumerable.Empty<ValuedAttribute>();
                return _modifiedAttributes.Select(a => a);
            }
        }

        public string ItemType => ItemHandler.ItemType;

        public IEnumerable<Attribute> AvailableAttributes { get; private set; }
        public IAttributedItemHandler ItemHandler { get; private set; }
		private List<Attribute> _allAttributes;

        public async Task Initialize(IAttributedItemHandler handler)
        {
            ItemHandler = handler;
            IsNew = handler.CurrentItem.IsNew();
            Item = handler.CurrentItem;

            _modifiedAttributes = (await handler.GetAttributes()).ToList();
            _attributesMap = _modifiedAttributes.ToDictionary(k => k.Key);

            var service = Resolve<ITap2HelpService>();
            _allAttributes = (await service.GetAllAttributes()).ToList();
			SetAvailableAttributes();
        }

		private void SetAvailableAttributes()
		{
            var serializer = Resolve<IJsonSerializer>();
			var availableAttributes = new List<Attribute>();
			availableAttributes.AddRange(
				_allAttributes.Where(attr => !_attributesMap.ContainsKey(attr.Key))
					.Select(attr => serializer.Clone(attr)));
			AvailableAttributes = ItemHandler.FilterAvailableAttributes(availableAttributes);
		}

        public void AddOrUpdateAttribute(Attribute attribute, string value)
        {
            ValuedAttribute attr;
            if (!_attributesMap.TryGetValue(attribute.Key, out attr))
            {
				attr = ItemHandler.CreateAttributeFrom(attribute);
                attr.ObjectId = null;
                _attributesMap.Add(attribute.Key, attr);
                _modifiedAttributes.RemoveAll(v => v.Key == attribute.Key);
                _modifiedAttributes.Add(attr);
				SetAvailableAttributes();
            }

            attr.Value = value;
        }

        public void DeleteAttribute(string key)
        {
            ValuedAttribute attr;
            if (_attributesMap.TryGetValue(key, out attr))
            {
                _attributesMap.Remove(key);
                _modifiedAttributes.RemoveAll(v => v.Key == key);
                SetAvailableAttributes();
            }
        }

        public async Task CommitChanges()
        {
			if (Item.IsNew())
				Item.StampNewId();

            var originalAttributes = (await ItemHandler.GetAttributes()).ToDictionary(k => k.Key);
            foreach (var modifiedAttribute in _attributesMap.Values)
            {
                modifiedAttribute.StampNewId();
                ValuedAttribute originalAttribute;
                if (originalAttributes.TryGetValue(modifiedAttribute.Key, out originalAttribute))
                {
                    modifiedAttribute.ObjectId = originalAttribute.ObjectId;
                }
            }

            var deletedAttributes = new List<ValuedAttribute>();
            foreach (var originalAttribute in originalAttributes.Values)
            {
                if (!_attributesMap.ContainsKey(originalAttribute.Key))
                    deletedAttributes.Add(originalAttribute);

            }
            await ItemHandler.CommitChanges(_attributesMap.Values, deletedAttributes);
        }
    }
}