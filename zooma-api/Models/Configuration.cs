using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ConfigDataType { get; set; } = null!;
        public string DataValue { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Status { get; set; }
    }
}
