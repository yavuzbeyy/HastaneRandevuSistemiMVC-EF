using System.ComponentModel.DataAnnotations.Schema;

namespace RandevuSistemi.Models.Entities
{
    public class Doktor
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public int PoliklinikId { get; set; }

    }
}
