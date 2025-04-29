using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeVault.Server.Models
{
   /**
    * @brief Entity class representing a student's enrollment in a class
    *
    * This is a many-to-many relationship entity connecting students to classes,
    * with a composite primary key of StudentId and ClassId.
    */
   public class ClassEnrollment
   {
        /**
         * @brief Part of the composite primary key - Student identifier
         *
         * Represents the student who is enrolled in the class.
         */
        [Key]
        [Column(Order = 0)]
        public int StudentId { get; set; }
        
        /**
         * @brief Part of the composite primary key - Class identifier
         *
         * Represents the class in which the student is enrolled.
         */
        [Key]
        [Column(Order = 1)]
        public int ClassId { get; set; }
        
        /**
         * @brief Date when the student enrolled in the class
         */
        public DateTime EnrollmentDate { get; set; }
        
        // Relationships
        /**
         * @brief Navigation property to the associated student
         */
        public Student Student { get; set; }
        
        /**
         * @brief Navigation property to the associated class
         */
        public Class Class { get; set; }
    }
}