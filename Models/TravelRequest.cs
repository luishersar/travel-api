
public class TravelRequest
{
    public int Id { get; set; }

    public required string EmployeeName { get; set; }

    public required string Origin { get; set; }

    public required string Destination { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public required string Purpose { get; set; }

    public TravelStatus Status { get; set; } = TravelStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    public int UserId { get; set; }
    }

