using Kastil.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Core.Services
{
    public interface IEditContext<T> where T : Item
    {
        Attribute SelectedAttribute { get; set; }
        bool IsNew { get; }

        void Initialize(string disasterId = null);
        void Initialize(T item = null, string disasterId = null);
        void AddOrUpdateAttribute(Attribute attribute, string value);
        void DeleteAttribute(string attributeName);
        Task CommitChanges();        
    }

    public interface IEditContextFactory
    {
        IEditContext<T> CreateFor<T>() where T : Item;
    }
}
