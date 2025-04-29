namespace GradeVault.Server.Models.DTOs
{
    /**
     * @brief Data transfer object for class information displayed to the client.
     *
     * This DTO represents a class entity with all fields needed for display
     * and includes information about the teacher assigned to the class.
     */
    public class ClassDTO
    {
        /**
         * @brief Gets or sets the unique identifier of the class.
         */
        public int Id { get; set; }

        /**
         * @brief Gets or sets the name of the class.
         */
        public string Name { get; set; }

        /**
         * @brief Gets or sets the description of the class.
         *
         * Was previously named "Subject" in earlier versions.
         */
        public string Description { get; set; } // Was Subject

        /**
         * @brief Gets or sets the room number where the class is held.
         */
        public string RoomNumber { get; set; }

        /**
         * @brief Gets or sets the name of the teacher responsible for the class.
         */
        public string TeacherName { get; set; }
    }

    /**
     * @brief Data transfer object for creating a new class.
     *
     * This DTO contains only the properties that can be set when creating a new class.
     * Teacher information is determined from the authenticated user making the request.
     */
    public class CreateClassDTO
    {
        /**
         * @brief Gets or sets the name of the class being created.
         */
        public string Name { get; set; }

        /**
         * @brief Gets or sets the description of the class being created.
         */
        public string Description { get; set; }

        /**
         * @brief Gets or sets the room number where the class will be held.
         */
        public string RoomNumber { get; set; }
    }
}