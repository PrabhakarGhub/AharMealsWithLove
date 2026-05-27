using AharMealsWithLove.Data;
using AharMealsWithLove.Models;
using Microsoft.AspNetCore.Mvc;

namespace AharMealsWithLove.Controllers
{
    // ─── VOLUNTEER CONTROLLER ──────────────────────────────────────────────────
    public class VolunteerController : Controller
    {
        private string? GetVolEmail() => HttpContext.Session.GetString("vmail");
        private bool IsLoggedIn() => GetVolEmail() != null;

        public IActionResult Dashboard()
        {
            if (!IsLoggedIn()) return RedirectToAction("VolunteerLogin", "Account");
            var email = GetVolEmail()!;
            var vol = SqliteHelper.ExecuteQuerySingle("SELECT * FROM vRegistration WHERE Email=?", new() { ["e"] = email });
            var info = SqliteHelper.ExecuteQuery("SELECT * FROM info ORDER BY Date DESC LIMIT 10");
            ViewBag.Vol = vol;
            ViewBag.InfoFeed = info;
            return View();
        }

        public IActionResult Profile()
        {
            if (!IsLoggedIn()) return RedirectToAction("VolunteerLogin", "Account");
            var email = GetVolEmail()!;
            var vol = SqliteHelper.ExecuteQuerySingle("SELECT * FROM vRegistration WHERE Email=?", new() { ["e"] = email });
            var model = new ProfileViewModel { Email = email };
            if (vol != null)
            {
                model.Name = vol.GetValueOrDefault("Name", "");
                model.Gender = vol.GetValueOrDefault("Gender", "");
                model.DOB = vol.GetValueOrDefault("DOB", "");
                model.Phone = vol.GetValueOrDefault("Phone", "");
                model.Address = vol.GetValueOrDefault("Address", "");
                model.Aadhar = vol.GetValueOrDefault("Aadhar", "");
                model.ExistingImage = vol.GetValueOrDefault("Image", "");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("VolunteerLogin", "Account");
            var email = GetVolEmail()!;
            string imagePath = model.ExistingImage ?? "";
            if (model.Image != null && model.Image.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                using var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create);
                await model.Image.CopyToAsync(stream);
                imagePath = $"/uploads/{fileName}";
            }
            SqliteHelper.ExecuteNonQuery(
                "UPDATE vRegistration SET Name=?, Gender=?, DOB=?, Phone=?, Address=?, Aadhar=?, Image=? WHERE Email=?",
                new() { ["n"] = model.Name, ["g"] = model.Gender, ["d"] = model.DOB, ["ph"] = model.Phone,
                        ["a"] = model.Address, ["aa"] = model.Aadhar, ["img"] = imagePath, ["e"] = email });
            model.Message = "Profile updated!"; model.ExistingImage = imagePath;
            return View(model);
        }

        public IActionResult Availability()
        {
            if (!IsLoggedIn()) return RedirectToAction("VolunteerLogin", "Account");
            var email = GetVolEmail()!;
            var vol = SqliteHelper.ExecuteQuerySingle("SELECT availstat FROM vRegistration WHERE Email=?", new() { ["e"] = email });
            ViewBag.CurrentStatus = vol?["availstat"] ?? "unavailable";
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public IActionResult Availability(string status)
        {
            if (!IsLoggedIn()) return RedirectToAction("VolunteerLogin", "Account");
            SqliteHelper.ExecuteNonQuery(
                "UPDATE vRegistration SET availstat=? WHERE Email=?",
                new() { ["s"] = status, ["e"] = GetVolEmail()! });
            ViewBag.CurrentStatus = status;
            ViewBag.Message = "Availability updated successfully!";
            return View();
        }

        public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("VolunteerLogin", "Account");
            if (!string.IsNullOrEmpty(model.OldPassword))
            {
                var vol = SqliteHelper.ExecuteQuerySingle("SELECT Password FROM vRegistration WHERE Email=?", new() { ["e"] = GetVolEmail()! });
                if (vol?["Password"] != SecurityHelper.Encrypt(model.OldPassword))
                { model.Message = "Old password incorrect."; return View(model); }
            }
            SqliteHelper.ExecuteNonQuery("UPDATE vRegistration SET Password=? WHERE Email=?",
                new() { ["p"] = SecurityHelper.Encrypt(model.Password), ["e"] = GetVolEmail()! });
            model.Message = "Password updated!";
            return View(model);
        }

        public IActionResult MyPickups()
        {
            if (!IsLoggedIn()) return RedirectToAction("VolunteerLogin", "Account");
            var pickups = SqliteHelper.ExecuteQuery("SELECT * FROM upick WHERE status='Accept' ORDER BY ID DESC");
            return View(pickups);
        }
    }

    // ─── HOTEL CONTROLLER ──────────────────────────────────────────────────────
    public class HotelController : Controller
    {
        private string? GetHotelEmail() => HttpContext.Session.GetString("hmail");
        private bool IsLoggedIn() => GetHotelEmail() != null;

        public IActionResult Dashboard()
        {
            if (!IsLoggedIn()) return RedirectToAction("HotelLogin", "Account");
            var hid = HttpContext.Session.GetString("hid");
            var picks = SqliteHelper.ExecuteQuery("SELECT * FROM hpick WHERE Email=? ORDER BY ID DESC", new() { ["e"] = GetHotelEmail()! });
            ViewBag.Pickups = picks;
            ViewBag.HotelName = HttpContext.Session.GetString("hname");
            return View();
        }

        public IActionResult FoodPickup()
        {
            if (!IsLoggedIn()) return RedirectToAction("HotelLogin", "Account");
            return View(new PickupRequest());
        }

        [HttpPost]
        public IActionResult FoodPickup(PickupRequest model)
        {
            if (!IsLoggedIn()) return RedirectToAction("HotelLogin", "Account");
            var email = GetHotelEmail()!;
            SqliteHelper.ExecuteNonQuery(
                "INSERT INTO hpick (uname, Email, eventdetail, venue, date, category, quantity, cooktime, exptime, areacode, status) VALUES (?,?,?,?,?,?,?,?,?,?,'Submitted')",
                new() { ["un"] = HttpContext.Session.GetString("hname"), ["em"] = email,
                        ["ed"] = model.EventDetail, ["v"] = model.Venue, ["dt"] = model.Date,
                        ["cat"] = model.Category, ["q"] = model.Quantity, ["ct"] = model.CookTime,
                        ["et"] = model.ExpTime, ["ac"] = model.AreaCode });
            model.Message = "Pickup request submitted!";
            return View(model);
        }

        public IActionResult Profile()
        {
            if (!IsLoggedIn()) return RedirectToAction("HotelLogin", "Account");
            var email = GetHotelEmail()!;
            var hotel = SqliteHelper.ExecuteQuerySingle("SELECT * FROM hlog WHERE Email=?", new() { ["e"] = email });
            return View(hotel);
        }
    }
}
