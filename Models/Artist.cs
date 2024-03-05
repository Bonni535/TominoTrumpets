using System.ComponentModel.DataAnnotations;

namespace TominoTrumpets.Models;


public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Bio { get; set; }
}