using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grid.Data.Models;

[Table("bitcoin_detail")]
[Serializable]
public class BitcoinDetailModel
{
    [Key]
    public Guid Guid { get; set; }

    [Required]
    public decimal Price { get; set; }

    public string? Note { get; set; }

    public TimeOnly Time {get; set;}
}
