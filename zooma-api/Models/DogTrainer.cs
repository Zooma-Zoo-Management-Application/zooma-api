namespace zooma_api.Models
{
    public class DogTrainer
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public bool IsActive { get; set; }
    }
}