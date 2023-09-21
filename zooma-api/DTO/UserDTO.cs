﻿namespace zooma_api.DTO
{
    public class UserDTO
    {
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
    }
}
