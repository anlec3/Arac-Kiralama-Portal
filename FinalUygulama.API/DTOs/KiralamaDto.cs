
namespace FinalUygulama.API.DTOs
{
    public class KiralamaDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ArabaId { get; set; }
        public string? AppUserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Order { get; set; }

        public ArabaDto Araba { get; set; }
        public UserDto User { get; set; }
    }
}
