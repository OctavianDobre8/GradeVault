namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data Transfer Object for grade information
     *
     * This DTO is used to transfer complete grade information between client and server,
     * including related student and class details.
     */
    public class GradeDTO
    {
        /**
         * @brief Unique identifier for the grade record
         */
        public int Id { get; set; }
        
        /**
         * @brief Identifier of the student who received the grade
         */
        public int StudentId { get; set; }
        
        /**
         * @brief Full name of the student who received the grade
         */
        public string? StudentName { get; set; }
        
        /**
         * @brief Identifier of the class/course in which the grade was given
         */
        public int ClassId { get; set; }
        
        /**
         * @brief Name of the class/course in which the grade was given
         */
        public string? ClassName { get; set; }
        
        /**
         * @brief Numerical value of the grade
         */
        public int Value { get; set; }
        
        /**
         * @brief Date and time when the grade was assigned
         */
        public DateTime DateAssigned { get; set; }
    }
}