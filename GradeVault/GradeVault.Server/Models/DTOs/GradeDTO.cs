namespace GradeVault.Server.Models.DTOs
{
    public class GradeDTO
    {
        public int Id { get; set; }
        public string? ClassName { get; set; } 
        public int Value { get; set; }         
        public DateTime DateAssigned { get; set; } 
    }
}