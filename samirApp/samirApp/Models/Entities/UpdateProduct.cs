﻿namespace samirApp.Models.Entities
{
    public class UpdateProduct
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductType { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
}
