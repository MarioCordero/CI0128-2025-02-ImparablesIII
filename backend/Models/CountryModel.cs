namespace backend_lab.Models;

public class CountryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Continent { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}
