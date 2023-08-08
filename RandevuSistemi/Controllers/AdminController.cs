using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RandevuSistemi.Models;
using RandevuSistemi.Models.Dto;
using RandevuSistemi.Models.Entities;
using System.Globalization;

namespace HayvanBarinagi.Controllers
{
    

    [Authorize]
    public class AdminController : Controller
    {

        Context context = new Context();

        // Admin kontrolü yapıyoruz. Kullanıcı Adı admin'e uyuyor mu ?
        public bool AdminControl()
        {
            if (User.Identity.IsAuthenticated) 
            {
                
                string userName = User.Identity.Name;

                if (userName == "admin") // Kullanıcı adı "admin" mi?
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Admin controlleri çalıştığında Indexi getir.
        // Index'te Ana bilim Dallarını listele
        // Admin değilse kullanıcı sayfasına at
        public IActionResult Index()
        {
            if (AdminControl())
            {
                var anaBilimDallari = context.AnaBilimDallari.ToList();
                return View(anaBilimDallari);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
      
        [HttpGet]
        public IActionResult AnaBilimDaliEkle()
        {
            if (AdminControl())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // Ana Bilim dalı ekleme
        [HttpPost]
        public IActionResult AnaBilimDaliEkle(AnaBilimDali anaBilimDali)
        {
            var newAnaBilimDali = new AnaBilimDali
            {
                Name = anaBilimDali.Name // parametreden gelen değer ile yeni nesne oluştur
            };

            context.AnaBilimDallari.Add(newAnaBilimDali); // bu nesneyi tabloya ekle
            context.SaveChanges(); // Tabloyu kaydet

            return RedirectToAction("Index", "Admin"); // Admin/Index'e geri dön
        }

        [HttpGet]
        public IActionResult AnaBilimDaliDuzenle(int Id)
        {
            if (AdminControl())
            {
                var anaBilimDali = context.AnaBilimDallari.Find(Id);
                return View(anaBilimDali);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult AnaBilimDaliDuzenle(AnaBilimDali anaBilimDali)
        {
            var selectedAnimal = context.AnaBilimDallari.Find(anaBilimDali.Id);

            selectedAnimal.Name = anaBilimDali.Name;
            context.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        // Ana bilim dalı silme
        public IActionResult AnaBilimDaliSil(int Id)
        {
            var anaBilimDali = context.AnaBilimDallari.Find(Id);
            context.AnaBilimDallari.Remove(anaBilimDali);
            context.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        //Hizmetleri Admin Panelinde Görüntüle
        public IActionResult Hizmetler()
        {
            if (AdminControl())
            {
                var tumHizmetler = context.Hizmetler.ToList();
                return View(tumHizmetler);
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        // Ana bilim dalında olduğu gibi Hizmet ekledik.
        [HttpPost]
        public IActionResult Hizmetler(string hizmetAdi)
        {
            var hizmetEkle = new Hizmetler
            {
                HizmetAdi = hizmetAdi
            };

            context.Hizmetler.Add(hizmetEkle);
            context.SaveChanges();

            return RedirectToAction("Hizmetler", "Admin");
        }

        // Gelen id'değerine göre tablodan Id'si parametreden gelen hizmeti bul ve kaldır.
        public IActionResult HizmetSil(int Id)
        {
            var hizmet = context.Hizmetler.Find(Id);
            context.Hizmetler.Remove(hizmet);
            context.SaveChanges();
            return RedirectToAction("Hizmetler", "Admin");
        }


        public IActionResult Poliklinik()
        {
            if (AdminControl())
            {
                var poliklinikler = context.Poliklinikler.ToList();
                var tumAnabilimDallari = context.AnaBilimDallari.ToList();

                var poliklinikModelList = new List<PoliklinikveAnabilimDaliAdi>();

                foreach (var poliklinik in poliklinikler)
                {
                    var anaBilimDali = tumAnabilimDallari.FirstOrDefault(abd => abd.Id == poliklinik.AnaBilimDaliId);

                    var poliklinikModel = new PoliklinikveAnabilimDaliAdi
                    {
                        poliklinik = poliklinik,
                        AnaBilimDaliAdi = anaBilimDali?.Name
                    };

                    poliklinikModelList.Add(poliklinikModel);
                }

                return View(poliklinikModelList);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult PoliklinikEkle()
        {
            if (AdminControl())
            {
                var anaBilimDaliListesi = context.AnaBilimDallari.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                }).ToList();

                return View(anaBilimDaliListesi);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult PoliklinikEkle(IFormCollection form)
        {
            // Form verilerini almak için FormCollection nesnesini kullanımı

            string name = form["Name"];
            int selectedAnaBilimDaliId = Convert.ToInt32(form["id"]);

            using (var dbContext = new Context())
            {
                Poliklinik yeniPoliklinik = new Poliklinik
                {
                    Name = name,
                    AnaBilimDaliId = selectedAnaBilimDaliId
                };

                dbContext.Poliklinikler.Add(yeniPoliklinik);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Poliklinik", "Admin");
        }

        public IActionResult PoliklinikSil(int Id)
        {
            var poliklinik = context.Poliklinikler.Find(Id);
            context.Poliklinikler.Remove(poliklinik);
            context.SaveChanges();

            return RedirectToAction("Poliklinik", "Admin");
        }

        public IActionResult Doktor()
        {
            if (AdminControl())
            {
                var tumDoktorlar = context.Doktorlar.ToList();
                var tumPoliklinikler = context.Poliklinikler.ToList();

                var poliklinikModelList = new List<DoktorVePoliklinikAdi>();

                foreach (var doktor in tumDoktorlar)
                {
                    var poliklinikler = tumPoliklinikler.FirstOrDefault(abd => abd.Id == doktor.PoliklinikId);

                    var doktorVePoliklinik = new DoktorVePoliklinikAdi
                    {
                        doctor = doktor,
                        PoliklinikAdi = poliklinikler?.Name
                    };

                    poliklinikModelList.Add(doktorVePoliklinik);
                }

                return View(poliklinikModelList);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult DoktorEkle()
        {
            if (AdminControl())
            {
                var poliklinikListesi = context.Poliklinikler.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                }).ToList();

                return View(poliklinikListesi);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult DoktorEkle(IFormCollection form)
        {
            // Form verilerini almak için FormCollection nesnesini kullanımı

            string name = form["Name"];
            int selectedpoliklinikId = Convert.ToInt32(form["id"]);

            using (var dbContext = new Context())
            {
                Doktor yeniDoktor = new Doktor
                {
                    AdSoyad = name,
                    PoliklinikId = selectedpoliklinikId
                };

                dbContext.Doktorlar.Add(yeniDoktor);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Doktor", "Admin");
        }

        // Admin panelinden gelen doktor id sine göre ilgili doktoru bulma ve kaldırma.
        public IActionResult DoktorSil(int Id)
        {
            var doktor = context.Doktorlar.Find(Id);
            context.Doktorlar.Remove(doktor);
            context.SaveChanges();

            return RedirectToAction("Doktor", "Admin");
        }

        //Doktorlar için çalışma saati oluşturalım ki , hastalar randevuları görüntüleyebilsin.
        //[HttpPost]
        //public IActionResult CalismaSaatiOlustur(int Id, DateTime CalismaSaati)
        //{
        //    if (AdminControl())
        //    {
        //        var doctor = context.Doktorlar.Find(Id); // View'dan gelen seçilen doktor id'sine göre doktoru bulma

        //        if (doctor != null) // doktor nesnesi varsa yani null değilse
        //        {
        //            var yeniCalismaSaati = new CalismaSaatleri // yeni çalışma saati oluştur
        //            {
        //                DoctorId = doctor.Id,
        //                CalismaZamani = CalismaSaati,   // Doktorun Adı ve hangi saatilerin müsait olduğu çalışma saatleri tablosuna eklemek için nesne üret.
        //                DoctorAdi = doctor.AdSoyad

        //            };

        //            context.CalismaSaatleri.Add(yeniCalismaSaati);  // üretilen nesneyi veritabanına ekle
        //            context.SaveChanges(); // veritabanı değişikliklerini kaydet

        //            return RedirectToAction("Doktor", "Admin");
        //        }
        //    }

        //    // Başarısız durumda başka bir sayfaya yönlendirme yapabilirsiniz.
        //    return RedirectToAction("Index", "Home");
        //}

        [HttpPost]
        public IActionResult CalismaSaatiOlustur(int Id, string CalismaZamani)
        {
            if (AdminControl())
            {
                var doctor = context.Doktorlar.Find(Id);

                if (doctor != null)
                {
                    DateTime calismaZamani = DateTime.ParseExact(CalismaZamani, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);

                    var yeniCalismaSaati = new CalismaSaatleri
                    {
                        DoctorId = doctor.Id,
                        CalismaZamani = calismaZamani,
                        DoctorAdi = doctor.AdSoyad
                    };

                    context.CalismaSaatleri.Add(yeniCalismaSaati);
                    context.SaveChanges();

                    return RedirectToAction("Doktor", "Admin");
                }
                else
                {
                    // Doctor nesnesi null ise burada uygun bir sonuç döndürülmeli
                    // Örneğin, hata mesajı veya uygun bir HTTP durum kodu döndürülebilir.
                    return RedirectToAction("HataSayfasi", "Admin");
                }
            }
            else
            {
                // Admin kontrolü geçilemediyse burada uygun bir sonuç döndürülmeli
                // Örneğin, hata mesajı veya uygun bir HTTP durum kodu döndürülebilir.
                return RedirectToAction("HataSayfasi", "Admin");
            }
        }






        [HttpGet]
        public IActionResult DoktorCalismaOlustur(int Id)
        {
            if (AdminControl())
            {
                var doctor = context.Doktorlar.Find(Id); 
                if (doctor != null)
                {
                    return View(doctor);
                }
            }
            // Başarısız durumda başka bir sayfaya yönlendirme yapabilirsiniz.
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Randevu()
        {
            if (AdminControl())
            {
                var randevular = context.Randevular.ToList();
                return View(randevular);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult RandevuSil(int Id)
        {        
          var silinecekRandevu = context.Randevular.Find(Id);

          if (silinecekRandevu != null)
                {
                    context.Remove(silinecekRandevu);
                    context.SaveChanges();
                    return RedirectToAction("Randevu", "Admin");
                }
                else {
                    return RedirectToAction("Randevu", "Admin");
                }
            
        }

    }
}
