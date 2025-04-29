namespace GradeVault.Server.Models
{
    /**
     * @brief Entity class representing a grade assigned to a student
     *
     * This model represents an academic grade with information about its value,
     * when it was assigned, and relationships to the student and class.
     */
    public class Grade
    {
        /**
         * @brief Unique identifier for the grade record
         */
        public int Id { get; set; }
        
        /**
         * @brief Numerical value of the grade (typically on a scale of 1-10)
         */
        public int Value { get; set; } // e.g., 1-10
        
        /**
         * @brief Date and time when the grade was assigned
         */
        public DateTime DateAssigned { get; set; }

        // Foreign Keys
        /**
         * @brief Foreign key reference to the student who received this grade
         */
        public int StudentId { get; set; }
        
        /**
         * @brief Foreign key reference to the class in which this grade was given
         */
        public int ClassId { get; set; }

        // Relationships
        /**
         * @brief Navigation property to the associated student
         */
        public Student? Student { get; set; }
        
        /**
         * @brief Navigation property to the associated class
         */
        public Class? Class { get; set; }
    }

}