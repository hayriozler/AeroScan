namespace Domain.Common;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } 
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } 
}
