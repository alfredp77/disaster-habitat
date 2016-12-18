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
        private Dictionary<string, Attribute> _attributesMap;
        public Attribute SelectedAttribute { get; set; }
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

        public string NamePlaceholderText => ItemHandler.NamePlaceholderText;
        public string LocationPlaceholderText => ItemHandler.LocationPlaceholderText;

        public bool IsNew { get; private set; }

        public IEnumerable<Attribute> Attributes
        {
            get
            {
                if (Item == null)
                    return Enumerable.Empty<Attribute>();
                return Item.Attributes.Select(a => a);
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

            _attributesMap = Item.Attributes.ToDictionary(k => k.Key);

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
			AvailableAttributes = availableAttributes;
		}

        public void AddOrUpdateAttribute(Attribute attribute, string value)
        {
            Attribute attr;
            if (!_attributesMap.TryGetValue(attribute.Key, out attr))
            {
				attr = ItemHandler.CreateAttributeFrom(attribute);
                attr.ObjectId = null;
                _attributesMap.Add(attribute.Key, attr);
                Item.Attributes.Add(attr);
				SetAvailableAttributes();
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
				SetAvailableAttributes();
            }
        }

        public Task CommitChanges()
        {
			if (Item.IsNew())
				Item.StampNewId();
            return ItemHandler.CommitChanges();
        }
    }
}