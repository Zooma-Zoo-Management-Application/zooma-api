﻿using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Configuration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConfigDataType { get; set; }
        public string DataValue { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
