namespace zooma_api.DTO
{
    public class FoodSpecyDTO
    {
        public int Id { get; set; }
        public int SpeciesId { get; set; }
        public int FoodId { get; set; }
        public bool CompatibilityStatus { get; set; }
        public FoodDTO Food { get; set; }
        public SpeciesDTO Species { get; set; }
    }
}
