using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Shared.Models;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Core.Services
{
    public class AssesmentEditContext
    {
        private readonly ITap2HelpService _tap2HelpService;

        public AssesmentEditContext(Assessment assessment, ITap2HelpService tap2HelpService)
        {
            Assessment = assessment;
            _tap2HelpService = tap2HelpService;
        }

        public Assessment Assessment { get; }

        public void AddOrUpdateAttribute(string attributeName, string attributeValue)
        {

        }

        public void DeleteAttribute(string attributeName)
        {
            var attributes = Assessment.Attributes;
            attributes.Remove(attributes.Single(attr => attr.Key == attributeName));
        }

        public IEnumerable<Attribute> Attributes => Assessment.Attributes;
    }
}
