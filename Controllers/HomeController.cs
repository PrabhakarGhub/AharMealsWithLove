using AharMealsWithLove.Data;
using Microsoft.AspNetCore.Mvc;

namespace AharMealsWithLove.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var spots = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as cnt FROM aharspot");
            var pickups = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as cnt FROM upick WHERE status='Accept'");
            ViewBag.AharSpots = spots.Count > 0 ? spots[0]["cnt"] : "0";
            ViewBag.PickupsCompleted = pickups.Count > 0 ? pickups[0]["cnt"] : "0";
            return View();
        }
        public IActionResult HowItWorks() => View();
        public IActionResult Awards() => View();
        public IActionResult Team() => View();
        public IActionResult FAQ() => View();
        public IActionResult Gallery() => View();
        public IActionResult AharATM() => View();
        public IActionResult AharSpots()
        {
            var spots = SqliteHelper.ExecuteQuery("SELECT * FROM aharspot");
            return View(spots);
        }
        public IActionResult Contact() => View(new Models.ContactViewModel());
        [HttpPost]
        public IActionResult Contact(Models.ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                SqliteHelper.ExecuteNonQuery(
                    "INSERT INTO Contact (Name, Contact, Message, Date) VALUES (?, ?, ?, ?)",
                    new() { ["n"] = model.Name, ["c"] = model.Email, ["m"] = model.Message, ["d"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                return View(new Models.ContactViewModel { SuccessMessage = "Message sent successfully! We will contact you soon." });
            }
            return View(model);
        }
        public IActionResult Privacy() => View();
        public IActionResult Disclaimer() => View();
        public IActionResult Refund() => View();
        public IActionResult TnC() => View();
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
