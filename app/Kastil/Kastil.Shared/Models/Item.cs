﻿using System.Collections.Generic;

namespace Kastil.Shared.Models
{
    /// <summary>
    /// Since Assessment and Shelter have same attributes, let's call it as Item for now.
    /// </summary>
    public abstract class Item : BaseModel
    {
        public string DisasterId { get; set; }
        public string Name { get; set; }
        public string LocationName { get; set; }
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();
    }
}
