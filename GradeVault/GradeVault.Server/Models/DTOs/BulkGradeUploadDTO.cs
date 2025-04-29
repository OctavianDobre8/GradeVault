namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data transfer object for bulk uploading multiple grades from CSV files.
     * 
     * This class maps to columns in a CSV file for batch importing grade data.
     * It contains the minimal information needed to create a grade: student ID and grade value.
     */
    public class BulkGradeUploadDTO
    {
        /**
         * @brief Gets or sets the ID of the student receiving the grade.
         * 
         * This should match an existing StudentId in the database.
         */
        public int StudentId { get; set; }

        /**
         * @brief Gets or sets the numeric value of the grade.
         * 
         * Valid values are typically between 1 and 10, inclusive.
         */
        public int Value { get; set; }
    }
}