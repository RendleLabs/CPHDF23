using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UfoDb;

[Table("sightings")]
public class DbSighting
{
    [Key]
    public int Id { get; set; }
    
    public DateTime Time { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Shape { get; set; }
    public string? Duration { get; set; }
    public string? Summary { get; set; }
    public DateTime Posted { get; set; }
    public string? Images { get; set; }
}
