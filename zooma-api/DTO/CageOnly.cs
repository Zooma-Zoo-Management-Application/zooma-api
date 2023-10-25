namespace zooma_api.DTO
{
    public class CageOnly
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public byte AnimalLimit { get; set; }
        public byte AnimalCount { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public short AreaId { get; set; }
    }
}
