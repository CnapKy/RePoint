using System.ComponentModel.DataAnnotations;
namespace RePoint.Web.ViewModels;
public class HandoverCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Избери пункт.")] public int CollectionPointId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Избери материал.")] public int MaterialId { get; set; }
    [Range(typeof(decimal), "0.001", "500", ErrorMessage = "Количеството трябва да е между 0,001 и 500 кг.")]
    public decimal EstimatedKg { get; set; }
}
