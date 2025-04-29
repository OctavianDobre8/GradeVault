namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data Transfer Object for updating an existing grade
     *
     * This DTO contains only the properties that can be modified when updating a grade.
     * The grade ID is not included as it's typically provided in the request URL.
     */
    public class UpdateGradeDTO
    {
        /**
         * @brief New numerical value to assign to the grade
         * 
         * This represents the updated score for the student's grade record.
         */
        public int Value { get; set; }
    }
}