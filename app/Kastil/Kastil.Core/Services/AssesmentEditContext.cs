using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Core.Services
{
    public interface IAssessmentEditContext
    {
        void Initialize(Assessment assessment=null,string disasterId=null);
        void AddOrUpdateAttribute(Attribute attribute, string value);
        void DeleteAttribute(string attributeName);
        Task CommitChanges();
        Assessment Assessment { get; }
        Attribute SelectedAttribute { get; set; }
        bool IsNew { get; }
    }

    public class AssesmentEditContext : BaseService, IAssessmentEditContext
    {
        private Dictionary<string, Attribute> _attributesMap;        
        public Attribute SelectedAttribute { get; set; }

        public Assessment Assessment { get; private set; }
        public bool IsNew { get; private set; }

        public void Initialize(Assessment assessment = null, string disasterId=null)
        {
            IsNew = assessment == null;
			if (assessment == null) 
			{
				Assessment = new Assessment { Id = Guid.NewGuid ().ToString () , DisasterId = disasterId};
			} 
			else 
			{
				var serializer = Resolve<IJsonSerializer>();
				Assessment = serializer.Clone(assessment);
			}
            
            _attributesMap = Assessment.Attributes.ToDictionary(k => k.Key);
        }

        public void AddOrUpdateAttribute(Attribute attribute, string value)
        {
            Attribute attr;
            if (!_attributesMap.TryGetValue(attribute.Key, out attr))
            {
                var serializer = Resolve<IJsonSerializer>();
                attr = serializer.Clone(attribute);
                _attributesMap.Add(attribute.Key, attr);
                Assessment.Attributes.Add(attr);
            }

            attr.Value = value;
        }

        public void DeleteAttribute(string attributeName)
        {
            Attribute attr;
            if (_attributesMap.TryGetValue(attributeName, out attr))
            {
                _attributesMap.Remove(attributeName);
                Assessment.Attributes.Remove(attr);
            }
        }

        public async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(Assessment);
        }
    }
}
