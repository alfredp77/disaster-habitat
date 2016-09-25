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

        public AssesmentEditContext(Assesment assesment, ITap2HelpService tap2HelpService)
        {
            Assesment = assesment;
            _tap2HelpService = tap2HelpService;
        }

        public Assesment Assesment { get; }

        public void AddOrUpdateAttribute(string attributeName, string attributeValue)
        {

        }

        public void DeleteAttribute(string attributeName)
        {
            var attributes = Assesment.Attributes;
            attributes.Remove(attributes.Single(attr => attr.Key == attributeName));
        }

        public IEnumerable<Attribute> Attributes => Assesment.Attributes;
    }
}
