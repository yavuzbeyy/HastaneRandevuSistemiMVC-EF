using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using RandevuSistemi.Models;
using RandevuSistemi.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RandevuSistemi.Models.Dto;
using System.Globalization;

namespace RandevuSistemi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult ChangeLanguage(string culture, string returnUrl)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            // Seçilen dili, tarayıcı çerezlerine kaydedelim
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            // returnUrl boş değilse ve güvenli bir yerel URL ise o sayfaya yönlendir
            return LocalRedirect(returnUrl);
        }
        else
        {
            // returnUrl boş veya güvenli bir yerel URL değilse varsayılan olarak Anasayfa'ya yönlendir
            return RedirectToAction("Index", "Home");
        }
    }

    Context context = new Context();

    //Başlangıç sayfası
    public IActionResult Index()
    {
        var hizmetListesi = context.Hizmetler.ToList();
        return View(hizmetListesi);
    }


    public IActionResult Poliklinikler()
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

    public String GetEslesenVeri(int anaBilimDaliId)
    {
        var eslesenAnaBilimDali = context.AnaBilimDallari.FirstOrDefault(x => x.Id == anaBilimDaliId);
        if (eslesenAnaBilimDali != null)
        {
            return eslesenAnaBilimDali.Name;
        }
        else
        {
            return "Eşleşen veri bulunamadı";
        }
    }

    public IActionResult DoktorlarVePoliklinik(int anaBilimDaliId)
    {
        var tumDoktorlar = context.Doktorlar.ToList();
        var tumPoliklinikler = context.Poliklinikler.ToList();

        var doktorlarBuAnabilimDalinda = tumDoktorlar.Where(d => tumPoliklinikler.Any(p => p.AnaBilimDaliId == anaBilimDaliId && p.Id == d.PoliklinikId)).ToList();

        var poliklinikModelList = new List<DoktorVePoliklinikAdi>();

        foreach (var doktor in doktorlarBuAnabilimDalinda)
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


    public IActionResult Doktorlar()
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

    [Authorize]
    [HttpGet]
    public IActionResult RandevuAl()
    {
        var poliklinikListesi = context.Poliklinikler.Select(h => new SelectListItem
        {
            Value = h.Id.ToString(),
            Text = h.Name
        }).ToList();

        return View(poliklinikListesi);
    }

    [HttpPost]
    public IActionResult RandevuAl(IFormCollection form)
    {
        int selectedPoliklinikId = Convert.ToInt32(form["id"]);

        var poliklinigeGoreDoktorlar = context.Doktorlar
            .Where(x => x.PoliklinikId == selectedPoliklinikId)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.AdSoyad
            })
            .ToList();

        ViewBag.Doktorlar = poliklinigeGoreDoktorlar;

        var poliklinikListesi = context.Poliklinikler.Select(h => new SelectListItem
        {
            Value = h.Id.ToString(),
            Text = h.Name
        }).ToList();

        return View(poliklinikListesi);
    }

    [HttpGet]
    public IActionResult DoktorSec(int Id)
    {
        var doktorCalisma = context.CalismaSaatleri.Where(x => x.DoctorId == Id)
            .ToList();

        if (doktorCalisma == null || doktorCalisma.Count == 0) // Doktorun uygun randevu saati yoksa
        {
            ViewBag.Mesaj = "Doktorun uygun randevusu bulunamadı.";
        }
        else
        {
            var model = new Tuple<List<CalismaSaatleri>, int>(doktorCalisma, Id);
            return View(model);
        }
        return View(); // Eğer doktorun uygun randevusu yoksa, hemen mesajı yazdırmak için sayfayı döndürüyoruz.
    }

    // idsine göre gelen doktoru bulan döndüren metod 
    public Doktor getDoctorValue(int Id)
    {
        var RandevuAlinanDoktor = context.Doktorlar.Where(x=> x.Id == Id).FirstOrDefault();
        return RandevuAlinanDoktor;
    }

    [HttpPost]
    public IActionResult RandevuOlustur(int doctorId, string selectedCardDate)
    {
        var currentUser = context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

        //randevu alınan doktoru idsine göre bul
        var randevuAlinanDoktor = getDoctorValue(doctorId);

        //randevu saatini al
        DateTime randevuSaati = DateTime.ParseExact(selectedCardDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

        //Doktorun id si ve çalışma saati veritabanında varsa yani böyle bir doktor varsa bunu değişkene ata çünk randevu alınca bunu kaldırmamız gerekecek.
        var calismaSaatiniBul = context.CalismaSaatleri.ToList().Where(x=> x.DoctorId == doctorId && x.CalismaZamani == randevuSaati).FirstOrDefault();
        

        var yeniRandevu = new Randevu
        {
            UserId = currentUser.Id,
            UserName = currentUser.AdSoyad,
            RandevuSaati = randevuSaati,
            DoctorAdi = randevuAlinanDoktor.AdSoyad,
            DoctorId = randevuAlinanDoktor.Id
        };

        context.Randevular.Add(yeniRandevu);
        if(calismaSaatiniBul != null)
        context.CalismaSaatleri.Remove(calismaSaatiniBul);

        context.SaveChanges();

        return RedirectToAction("Index", "Home");
    }

    public IActionResult RandevuIptal(int Id)
    {
        var silinecekRandevu = context.Randevular.Find(Id);

        if (silinecekRandevu != null)
        {
            context.Remove(silinecekRandevu);
            context.SaveChanges();
            return RedirectToAction("RandevulariGoruntule", "Home");
        }
        else
        {
            return RedirectToAction("RandevulariGoruntule", "Home");
        }
    }

    [Authorize]
    public IActionResult RandevulariGoruntule()
    {
        var aktifRandevular = context.Randevular.Where(x => x.UserName == User.Identity.Name).ToList(); // aktif kullanıcının adıyla bir randevu var mı.
        return View(aktifRandevular);
    }

    [HttpGet]
    public IActionResult UyeOl()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UyeOl(User user)
    {
        var yeniKullanici = new User
        {
            AdSoyad = user.AdSoyad,
            Email = user.Email,
            UserName = user.UserName,
            Password = user.Password
        };
        context.Users.Add(yeniKullanici);
        context.SaveChanges();
        return RedirectToAction("GirisYap");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult GirisYap(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl))
        {
            TempData["ReturnUrl"] = returnUrl;
        }
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> GirisYapAsync(User user)
    {
        var kontrol = context.Users.FirstOrDefault(x => x.UserName == user.UserName && x.Password == user.Password);
        if (kontrol != null)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                };
            var userIdentity = new ClaimsIdentity(claims, "Login");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
            string returnUrl = TempData["ReturnUrl"] as string;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // Burası güncellenecek öğrenci numarasına göre
            if (kontrol.UserName == "admin")
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "Home");
        }
        else
        {
            ViewBag.Message = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

    }

    public async Task<IActionResult> CikisYap()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}