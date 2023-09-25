namespace zooma_api.DTO
{
    public class UpdatePasswordBody
    {
        public short Id { get; set; }

        public string currentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
