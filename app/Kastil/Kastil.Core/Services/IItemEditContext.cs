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
    public interface IItemEditContext
    {
        string ItemName { get; set; }
        string ItemLocation { get; set; }
        bool IsNew { get; }
        void AddOrUpdateAttribute(Attribute attribute, string value);
        void DeleteAttribute(string attributeName);
        Attribute SelectedAttribute { get; set; }
        IEnumerable<Attribute> Attributes { get; }
        Task CommitChanges();
    }
    

    public abstract class ItemEditContext<T> : BaseService, IItemEditContext where T : Item
    {
        private Dictionary<string, Attribute> _attributesMap;
        public Attribute SelectedAttribute { get; set; }
        public T Item { get; private set; }

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
            get { return Item?.LocationName; }
            set
            {
                if (Item != null)
                    Item.LocationName = value;
            }
        }

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
        public void Initialize(T item = null, string disasterId = null)
        {
            IsNew = item == null;
            if (item == null)
            {
                Item = CreateItem(disasterId);
            }
            else
            {
                var serializer = Resolve<IJsonSerializer>();
                Item = serializer.Clone(item);
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

        protected abstract T CreateItem(string disasterId);
        public abstract Task CommitChanges();

    }
  
}
