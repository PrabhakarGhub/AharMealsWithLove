using AharMealsWithLove.Data;
using AharMealsWithLove.Models;
using Microsoft.AspNetCore.Mvc;

namespace AharMealsWithLove.Controllers
{
    public class UserController : Controller
    {
        private string? GetUserEmail() => HttpContext.Session.GetString("umail");
        private bool IsLoggedIn() => GetUserEmail() != null;

        public IActionResult Dashboard()
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;
            var info = SqliteHelper.ExecuteQuery("SELECT * FROM info ORDER BY Date DESC LIMIT 10");
            var user = SqliteHelper.ExecuteQuerySingle("SELECT * FROM uRegistration WHERE Email=?", new() { ["e"] = email });
            ViewBag.User = user;
            ViewBag.InfoFeed = info;
            return View();
        }

        public IActionResult Profile()
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;
            var user = SqliteHelper.ExecuteQuerySingle("SELECT * FROM uRegistration WHERE Email=?", new() { ["e"] = email });
            var model = new ProfileViewModel { Email = email };
            if (user != null)
            {
                model.Name = user.GetValueOrDefault("Name", "");
                model.Gender = user.GetValueOrDefault("Gender", "");
                model.DOB = user.GetValueOrDefault("DOB", "");
                model.Phone = user.GetValueOrDefault("Phone", "");
                model.Address = user.GetValueOrDefault("Address", "");
                model.Aadhar = user.GetValueOrDefault("Aadhar", "");
                model.AreaCode = user.GetValueOrDefault("AreaCode", "110");
                model.ExistingImage = user.GetValueOrDefault("Image", "");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;
            string imagePath = model.ExistingImage ?? "";

            if (model.Image != null && model.Image.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await model.Image.CopyToAsync(stream);
                imagePath = $"/uploads/{fileName}";
            }

            SqliteHelper.ExecuteNonQuery(
                "UPDATE uRegistration SET Name=?, Gender=?, DOB=?, Phone=?, Address=?, Aadhar=?, AreaCode=?, Image=? WHERE Email=?",
                new() { ["n"] = model.Name, ["g"] = model.Gender, ["d"] = model.DOB, ["ph"] = model.Phone,
                        ["a"] = model.Address, ["aa"] = model.Aadhar, ["ac"] = model.AreaCode,
                        ["img"] = imagePath, ["e"] = email });

            HttpContext.Session.SetString("uname", model.Name ?? "User");
            model.Message = "Profile updated successfully!";
            model.ExistingImage = imagePath;
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;

            // If logged in normally, verify old password
            var sessionName = HttpContext.Session.GetString("uname");
            if (sessionName != null && !string.IsNullOrEmpty(model.OldPassword))
            {
                var user = SqliteHelper.ExecuteQuerySingle(
                    "SELECT Password FROM uRegistration WHERE Email=?", new() { ["e"] = email });
                if (user?["Password"] != SecurityHelper.Encrypt(model.OldPassword))
                {
                    model.Message = "Old password is incorrect.";
                    return View(model);
                }
            }

            SqliteHelper.ExecuteNonQuery(
                "UPDATE uRegistration SET Password=? WHERE Email=?",
                new() { ["p"] = SecurityHelper.Encrypt(model.Password), ["e"] = email });

            model.Message = "Password updated successfully! Redirecting to login...";
            // Clear forgot-password session
            HttpContext.Session.Remove("umail");
            return View(model);
        }

        public IActionResult FoodPickup()
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            return View(new PickupRequest());
        }

        [HttpPost]
        public IActionResult FoodPickup(PickupRequest model)
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;
            var user = SqliteHelper.ExecuteQuerySingle("SELECT Name FROM uRegistration WHERE Email=?", new() { ["e"] = email });
            string uname = user?["Name"] ?? email;

            SqliteHelper.ExecuteNonQuery(
                "INSERT INTO upick (uname, Email, eventdetail, venue, date, category, quantity, cooktime, exptime, areacode, status) VALUES (?,?,?,?,?,?,?,?,?,?,'Submitted')",
                new() { ["un"] = uname, ["em"] = email, ["ed"] = model.EventDetail, ["v"] = model.Venue,
                        ["dt"] = model.Date, ["cat"] = model.Category, ["q"] = model.Quantity,
                        ["ct"] = model.CookTime, ["et"] = model.ExpTime, ["ac"] = model.AreaCode });

            // Send confirmation email
            AharMealsWithLove.Data.EmailService.SendEmail(
                email,
                "AHAR - Pickup Request Received",
                AharMealsWithLove.Data.EmailService.BuildPickupEmailBody(
                    uname, model.Venue ?? "", model.Date ?? "", model.Quantity ?? ""));
            model.Message = "Thank you! Your pickup request has been submitted. Our team will contact you soon.";
            return View(model);
        }

        public IActionResult MyPickups()
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;
            var pickups = SqliteHelper.ExecuteQuery("SELECT * FROM upick WHERE Email=? ORDER BY ID DESC", new() { ["e"] = email });
            return View(pickups);
        }

        public IActionResult Feedback()
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            return View(new FeedbackViewModel());
        }

        [HttpPost]
        public IActionResult Feedback(FeedbackViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;
            var user = SqliteHelper.ExecuteQuerySingle("SELECT ID FROM uRegistration WHERE Email=?", new() { ["e"] = email });
            int uid = int.Parse(user?["ID"] ?? "0");
            int ticket = model.Reason == "Complaint" ? new Random().Next(100000, 999999) : 0;

            SqliteHelper.ExecuteNonQuery(
                "INSERT INTO feedback (Reason, ComplainFor, Detail, UID, Ticket, Status) VALUES (?,?,?,?,?,'Submitted')",
                new() { ["r"] = model.Reason, ["cf"] = model.ComplainFor, ["d"] = model.Detail, ["u"] = uid, ["t"] = ticket });

            model.Message = ticket != 0
                ? $"Submitted! Your Ticket#: {ticket}. Use this to track status."
                : "Feedback submitted successfully!";
            model.Ticket = ticket;
            return View(model);
        }

        public IActionResult FeedbackStatus(string? ticket)
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            List<Dictionary<string, string>> results = new();
            if (!string.IsNullOrEmpty(ticket))
                results = SqliteHelper.ExecuteQuery("SELECT Ticket, Status FROM feedback WHERE Ticket=?", new() { ["t"] = ticket });
            ViewBag.Results = results;
            ViewBag.Ticket = ticket ?? "";
            return View();
        }

        // ─── DONATION / BIRTHDAY ──────────────────────────────────────────────
        public IActionResult Donate() => View();

        public IActionResult ShareBirthday() => View(new BirthdayViewModel());

        [HttpPost]
        public IActionResult ShareBirthday(BirthdayViewModel model)
        {
            HttpContext.Session.SetString("bname", model.Name ?? "");
            HttpContext.Session.SetString("bemail", model.Email ?? "");
            HttpContext.Session.SetString("bmobile", model.Mobile ?? "");
            HttpContext.Session.SetString("bdob", model.DOB ?? "");
            HttpContext.Session.SetString("baddress", model.Address ?? "");
            return RedirectToAction("SelectCapacity");
        }

        public IActionResult SelectCapacity() => View(new BirthdayViewModel());

        [HttpPost]
        public IActionResult SelectCapacity(BirthdayViewModel model)
        {
            string amount = model.Capacity switch { "200" => "7000", "300" => "10500", _ => "3500" };
            HttpContext.Session.SetString("bcapacity", model.Capacity);
            HttpContext.Session.SetString("bamount", amount);
            HttpContext.Session.SetString("bservice", "Ahar Food Donation");
            HttpContext.Session.SetString("baddress2", "Tatisilwai Ranchi");
            return RedirectToAction("PaymentSummary");
        }

        public IActionResult PaymentSummary()
        {
            var model = new DonationViewModel
            {
                BillingAddress = HttpContext.Session.GetString("baddress2") ?? "Ahar Office, Ranchi",
                ServiceType = HttpContext.Session.GetString("bservice") ?? "Ahar Food Donation",
                Amount = HttpContext.Session.GetString("bamount") ?? "3500",
                TotalAmount = HttpContext.Session.GetString("bamount") ?? "3500"
            };
            return View(model);
        }

        public IActionResult PaymentMethod()
        {
            var banks = SqliteHelper.ExecuteQuery("SELECT Bank FROM Netbanking");
            var model = new PaymentViewModel { Banks = banks.Select(b => b["Bank"]).ToList() };
            return View(model);
        }

        [HttpPost]
        public IActionResult PaymentMethod(PaymentViewModel model)
        {
            HttpContext.Session.SetString("paymethod", model.Method ?? "Card");
            return RedirectToAction("Gateway");
        }

        public IActionResult Gateway()
        {
            var method = HttpContext.Session.GetString("paymethod") ?? "Card";
            ViewBag.Method = method;
            if (method == "Net Banking")
                return RedirectToAction("Receipt");
            var banks = SqliteHelper.ExecuteQuery("SELECT Bank FROM Netbanking");
            return View(new PaymentViewModel { Method = method, Banks = banks.Select(b => b["Bank"]).ToList() });
        }

        [HttpPost]
        public IActionResult Gateway(PaymentViewModel model)
        {
            if (string.IsNullOrEmpty(model.CardNumber) || string.IsNullOrEmpty(model.Expiry) ||
                string.IsNullOrEmpty(model.CVV) || string.IsNullOrEmpty(model.CardName))
            {
                model.ErrorMessage = "Please fill all card details.";
                var banks = SqliteHelper.ExecuteQuery("SELECT Bank FROM Netbanking");
                model.Banks = banks.Select(b => b["Bank"]).ToList();
                return View(model);
            }
            return RedirectToAction("Receipt");
        }

        public IActionResult Receipt()
        {
            string name = HttpContext.Session.GetString("bname") ?? "";
            string email = HttpContext.Session.GetString("bemail") ?? "";
            string mobile = HttpContext.Session.GetString("bmobile") ?? "";
            string dob = HttpContext.Session.GetString("bdob") ?? "";
            string address = HttpContext.Session.GetString("baddress") ?? "";
            string capacity = HttpContext.Session.GetString("bcapacity") ?? "100";
            string amount = HttpContext.Session.GetString("bamount") ?? "3500";

            if (!string.IsNullOrEmpty(email))
            {
                SqliteHelper.ExecuteNonQuery(
                    "INSERT INTO bdonat (name,email,mobile,dob,address,people,amount,date) VALUES (?,?,?,?,?,?,?,?)",
                    new() { ["n"] = name, ["e"] = email, ["m"] = mobile, ["d"] = dob,
                            ["a"] = address, ["p"] = capacity, ["am"] = amount,
                            ["dt"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }

            var model = new DonationViewModel
            {
                BillingAddress = HttpContext.Session.GetString("baddress2") ?? "Ahar Office, Ranchi",
                ServiceType = HttpContext.Session.GetString("bservice") ?? "Ahar Food Donation",
                Amount = amount,
                TotalAmount = amount
            };
            HttpContext.Session.Remove("bname"); HttpContext.Session.Remove("bemail");
            HttpContext.Session.Remove("bmobile"); HttpContext.Session.Remove("bdob");
            HttpContext.Session.Remove("baddress"); HttpContext.Session.Remove("bcapacity");
            HttpContext.Session.Remove("bamount"); HttpContext.Session.Remove("bservice");
            return View(model);
        }

        public IActionResult PrintProfile()
        {
            if (!IsLoggedIn()) return RedirectToAction("UserLogin", "Account");
            var email = GetUserEmail()!;
            var user = SqliteHelper.ExecuteQuerySingle("SELECT * FROM uRegistration WHERE Email=?", new() { ["e"] = email });
            return View(user);
        }
    }
}

// The above class already handles all routes. Email service integrated below.
