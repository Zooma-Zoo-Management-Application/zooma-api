using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class User
    {
        public User()
        {
            AnimalUsers = new HashSet<AnimalUser>();
            News = new HashSet<News>();
            Orders = new HashSet<Order>();
            TrainerExps = new HashSet<TrainerExp>();
        }

        public short Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string AvatarUrl { get; set; }
        public bool Status { get; set; }
        public byte RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<AnimalUser> AnimalUsers { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<TrainerExp> TrainerExps { get; set; }
    }
}
