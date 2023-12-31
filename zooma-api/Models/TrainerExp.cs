﻿using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class TrainerExp
    {
        public int Id { get; set; }
        public byte YearOfExperience { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public short UserId { get; set; }
        public short SkillId { get; set; }

        public virtual Skill Skill { get; set; }
        public virtual User User { get; set; }
    }
}
