using System.ComponentModel.DataAnnotations.Schema;

namespace RandevuSistemi.Models.Entities
{
    public class Poliklinik
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AnaBilimDaliId { get; set; }
    }
}
