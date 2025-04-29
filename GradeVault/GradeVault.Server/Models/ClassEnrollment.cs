using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeVault.Server.Models
{
   public class ClassEnrollment
   {
        [Key]
        [Column(Order = 0)]
        public int StudentId { get; set; }
        
        [Key]
        [Column(Order = 1)]
        public int ClassId { get; set; }
        
        public DateTime EnrollmentDate { get; set; }
        
        // Relationships
        public Student Student { get; set; }
        public Class Class { get; set; }
    }
}