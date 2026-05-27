using AharMealsWithLove.Data;
using AharMealsWithLove.Models;
using Microsoft.AspNetCore.Mvc;

namespace AharMealsWithLove.Controllers
{
    public class AccountController : Controller
    {
        // ─── USER SIGNUP ───────────────────────────────────────────────────────
        public IActionResult UserSignup() => View(new SignupViewModel());

        [HttpPost]
        public IActionResult UserSignup(SignupViewModel model, string action)
        {
            if (action == "sendotp")
            {
                var existing = SqliteHelper.ExecuteQuerySingle(
                    "SELECT ID FROM uRegistration WHERE Email=?",
                    new() { ["e"] = model.Email });
                if (existing != null)
                {
                    model.Message = "Email already registered. Please login.";
                    return View(model);
                }
                string otp = SecurityHelper.GenerateOTP();
                SqliteHelper.ExecuteNonQuery("DELETE FROM ureg WHERE Email=?", new() { ["e"] = model.Email });
                SqliteHelper.ExecuteNonQuery("INSERT INTO ureg (Email, OTP) VALUES (?,?)",
                    new() { ["e"] = model.Email, ["o"] = otp });
                model.OtpSent = true;
                var (emailSent, emailMsg) = AharMealsWithLove.Data.EmailService.SendEmail(
                    model.Email,
                    "AHAR Registration OTP",
                    AharMealsWithLove.Data.EmailService.BuildOtpEmailBody(otp));
                model.Message = emailSent
                    ? $"OTP sent to {model.Email}. Please check your inbox."
                    : $"OTP generated (Demo mode - OTP: {otp}). Configure SMTP in appsettings.json to send real emails.";
                return View(model);
            }
            else // verify otp
            {
                var reg = SqliteHelper.ExecuteQuerySingle(
                    "SELECT OTP FROM ureg WHERE Email=? AND OTP=?",
                    new() { ["e"] = model.Email, ["o"] = model.OTP });
                if (reg == null)
                {
                    model.OtpSent = true;
                    model.Message = "Invalid OTP. Please try again.";
                    return View(model);
                }
                SqliteHelper.ExecuteNonQuery("INSERT INTO uRegistration (Email) VALUES (?)",
                    new() { ["e"] = model.Email });
                HttpContext.Session.SetString("umail", model.Email);
                return RedirectToAction("ChangePassword", "User");
            }
        }

        // ─── USER LOGIN ────────────────────────────────────────────────────────
        public IActionResult UserLogin() => View(new LoginViewModel());

        [HttpPost]
        public IActionResult UserLogin(LoginViewModel model)
        {
            var user = SqliteHelper.ExecuteQuerySingle(
                "SELECT * FROM uRegistration WHERE Email=? AND Password=?",
                new() { ["e"] = model.Email, ["p"] = SecurityHelper.Encrypt(model.Password) });
            if (user == null)
            {
                model.ErrorMessage = "Invalid email or password.";
                return View(model);
            }
            HttpContext.Session.SetString("umail", user["Email"]);
            HttpContext.Session.SetString("uname", user["Name"] ?? "User");
            return RedirectToAction("Dashboard", "User");
        }

        // ─── VOLUNTEER SIGNUP ──────────────────────────────────────────────────
        public IActionResult VolunteerSignup() => View(new SignupViewModel());

        [HttpPost]
        public IActionResult VolunteerSignup(SignupViewModel model, string action)
        {
            if (action == "sendotp")
            {
                string otp = SecurityHelper.GenerateOTP();
                SqliteHelper.ExecuteNonQuery("DELETE FROM vreg WHERE Email=?", new() { ["e"] = model.Email });
                SqliteHelper.ExecuteNonQuery("INSERT INTO vreg (Email, OTP) VALUES (?,?)",
                    new() { ["e"] = model.Email, ["o"] = otp });
                model.OtpSent = true;
                model.Message = $"OTP sent! (Demo OTP: {otp})";
                return View(model);
            }
            else
            {
                var reg = SqliteHelper.ExecuteQuerySingle(
                    "SELECT OTP FROM vreg WHERE Email=? AND OTP=?",
                    new() { ["e"] = model.Email, ["o"] = model.OTP });
                if (reg == null)
                {
                    model.OtpSent = true;
                    model.Message = "Invalid OTP.";
                    return View(model);
                }
                SqliteHelper.ExecuteNonQuery("INSERT INTO vRegistration (Email) VALUES (?)",
                    new() { ["e"] = model.Email });
                HttpContext.Session.SetString("vmail", model.Email);
                return RedirectToAction("ChangePassword", "Volunteer");
            }
        }

        // ─── VOLUNTEER LOGIN ───────────────────────────────────────────────────
        public IActionResult VolunteerLogin() => View(new LoginViewModel());

        [HttpPost]
        public IActionResult VolunteerLogin(LoginViewModel model)
        {
            var vol = SqliteHelper.ExecuteQuerySingle(
                "SELECT * FROM vRegistration WHERE Email=? AND Password=?",
                new() { ["e"] = model.Email, ["p"] = SecurityHelper.Encrypt(model.Password) });
            if (vol == null)
            {
                model.ErrorMessage = "Invalid email or password.";
                return View(model);
            }
            HttpContext.Session.SetString("vmail", vol["Email"]);
            HttpContext.Session.SetString("vname", vol["Name"] ?? "Volunteer");
            return RedirectToAction("Dashboard", "Volunteer");
        }

        // ─── HOTEL LOGIN ───────────────────────────────────────────────────────
        public IActionResult HotelLogin() => View(new HotelLoginViewModel());

        [HttpPost]
        public IActionResult HotelLogin(HotelLoginViewModel model)
        {
            var hotel = SqliteHelper.ExecuteQuerySingle(
                "SELECT * FROM hlog WHERE hotelid=? AND Password=?",
                new() { ["h"] = model.HotelId, ["p"] = model.Password });
            if (hotel == null)
            {
                model.ErrorMessage = "Invalid Hotel ID or password.";
                return View(model);
            }
            HttpContext.Session.SetString("hmail", hotel["Email"] ?? "");
            HttpContext.Session.SetString("hname", hotel["Name"] ?? "Hotel");
            HttpContext.Session.SetString("hid", hotel["hotelid"] ?? "");
            return RedirectToAction("Dashboard", "Hotel");
        }

        // ─── ADMIN LOGIN ───────────────────────────────────────────────────────
        public IActionResult AdminLogin() => View(new AdminLoginViewModel());

        [HttpPost]
        public IActionResult AdminLogin(AdminLoginViewModel model)
        {
            var admin = SqliteHelper.ExecuteQuerySingle(
                "SELECT * FROM AdminLogin WHERE admin_id=? AND Password=?",
                new() { ["a"] = model.AdminId, ["p"] = model.Password });
            if (admin == null)
            {
                model.ErrorMessage = "Invalid Admin ID or password.";
                return View(model);
            }
            HttpContext.Session.SetString("adminId", model.AdminId);
            return RedirectToAction("Dashboard", "Admin");
        }

        // ─── FORGOT PASSWORD ───────────────────────────────────────────────────
        public IActionResult ForgotPassword() => View(new SignupViewModel());

        [HttpPost]
        public IActionResult ForgotPassword(SignupViewModel model, string action)
        {
            if (action == "sendotp")
            {
                var user = SqliteHelper.ExecuteQuerySingle(
                    "SELECT ID FROM uRegistration WHERE Email=?",
                    new() { ["e"] = model.Email });
                if (user == null)
                {
                    model.Message = "Email not found.";
                    return View(model);
                }
                var otpRow = SqliteHelper.ExecuteQuerySingle("SELECT OTP FROM ureg WHERE Email=?", new() { ["e"] = model.Email });
                string otp = otpRow?["OTP"] ?? SecurityHelper.GenerateOTP();
                model.OtpSent = true;
                model.Message = $"OTP sent! (Demo OTP: {otp})";
                return View(model);
            }
            else
            {
                var reg = SqliteHelper.ExecuteQuerySingle(
                    "SELECT OTP FROM ureg WHERE Email=? AND OTP=?",
                    new() { ["e"] = model.Email, ["o"] = model.OTP });
                if (reg == null) { model.OtpSent = true; model.Message = "Invalid OTP."; return View(model); }
                HttpContext.Session.SetString("umail", model.Email);
                return RedirectToAction("ChangePassword", "User");
            }
        }

        // ─── LOGOUT ────────────────────────────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
