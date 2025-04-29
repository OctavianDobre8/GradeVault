namespace GradeVault.Server.Models.DTOs
{
   /**
    * @brief Data Transfer Object for creating a new grade
    *
    * This DTO is used to transfer grade creation data from client to server,
    * containing the essential information needed to create a new grade record.
    */
   public class CreateGradeDTO
{
    /// <summary>
    /// Identifier of the student who received the grade
    /// </summary>
    public int StudentId { get; set; }
    
    /// <summary>
    /// Identifier of the class/course in which the grade was given
    /// </summary>
    public int ClassId { get; set; }
    
    /// <summary>
    /// Numerical value of the grade
    /// </summary>
    public int Value { get; set; }
}
}