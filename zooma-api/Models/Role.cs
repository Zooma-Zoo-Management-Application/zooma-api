using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public byte Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
