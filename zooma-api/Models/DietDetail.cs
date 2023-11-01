using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class DietDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime? ScheduleAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string? FeedingDate { get; set; }
        public TimeSpan? FeedingTime { get; set; }
        public double? Quantity { get; set; }
        public bool Status { get; set; }
        public int DietId { get; set; }
        public int FoodId { get; set; }

        public virtual Diet Diet { get; set; }
        public virtual Food Food { get; set; }
    }
}
