using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class News
    {
        public short Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
        public short UserId { get; set; }

        public virtual User User { get; set; }
    }
}
