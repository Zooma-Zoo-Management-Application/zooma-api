﻿namespace zooma_api.DTO
{
    public class SpeciesDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool Status { get; set; }
        public int? TypeId { get; set; }
    }
}
