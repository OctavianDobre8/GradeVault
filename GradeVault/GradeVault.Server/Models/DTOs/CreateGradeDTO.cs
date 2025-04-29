namespace GradeVault.Server.Models.DTOs
{
   public class CreateGradeDTO
{
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public int Value { get; set; }
}
}