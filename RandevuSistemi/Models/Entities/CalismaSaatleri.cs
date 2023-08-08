using System.ComponentModel.DataAnnotations;

namespace RandevuSistemi.Models.Entities
{
    public class CalismaSaatleri
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public DateTime CalismaZamani { get; set; }

        public string DoctorAdi { get; set; }
    }
}
