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
    public interface IAssessmentEditContext : IItemEditContext
    {
        void Initialize(Assessment assessment = null, string disasterId = null);
        Assessment Item { get; }
    }

    public class AssessmentEditContext : BaseService, IAssessmentEditContext
    {
        private Dictionary<string, Attribute> _attributesMap;
        public Attribute SelectedAttribute { get; set; }
        public Assessment Item { get; private set; }
        public bool IsNew { get; private set; }

        public void Initialize(Assessment assessment = null, string disasterId = null)
        {
            IsNew = assessment == null;
            if (assessment == null)
            {
                Item = new Assessment { Id = Guid.NewGuid().ToString(), DisasterId = disasterId };
            }
            else
            {
                var serializer = Resolve<IJsonSerializer>();
                Item = serializer.Clone(assessment);
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

        public async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(Item);
        }
    }
}