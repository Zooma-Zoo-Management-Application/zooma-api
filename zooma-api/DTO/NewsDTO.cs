namespace zooma_api.DTO
{
    public class NewsDTO
    {
        public short Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? Date { get; set; }
        public bool Status { get; set; }
        public short UserId { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        public UserDTO User { get; set; }
    }
}
