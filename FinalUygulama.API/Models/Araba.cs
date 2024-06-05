namespace FinalUygulama.API.Models
{
    public class Araba
    {
        public int Id { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public string Renk { get; set; }
        public string GunlukUcret { get; set; }
        public string Location { get; set; }
        public string? AppUserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Order { get; set; }
        public string Resim { get; set; } 
    }
}
