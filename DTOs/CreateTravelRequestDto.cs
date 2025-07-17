
    public class CreateTravelRequestDto
    {
        public required string EmployeeName { get; set; }

        public required string Origin { get; set; }

        public required string Destination { get; set; }

        public required DateTime StartDate { get; set; }

        public required DateTime EndDate { get; set; }

        public required string Purpose { get; set; }
    }

