namespace RandevuSistemi.Models.Entities
{
    public class Randevu
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime RandevuSaati { get; set; }
        public string DoctorAdi { get; set; }
        public int DoctorId { get; set; }
    }
}
