using AharMealsWithLove.Data;
using AharMealsWithLove.Models;
using Microsoft.AspNetCore.Mvc;

namespace AharMealsWithLove.Controllers
{
    public class AdminController : Controller
    {
        private bool IsAdmin() => HttpContext.Session.GetString("adminId") != null;

        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            ViewBag.TotalUsers = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as c FROM uRegistration")[0]["c"];
            ViewBag.TotalVols = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as c FROM vRegistration")[0]["c"];
            ViewBag.TotalPickups = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as c FROM upick")[0]["c"];
            ViewBag.TotalSpots = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as c FROM aharspot")[0]["c"];
            ViewBag.PendingPickups = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as c FROM upick WHERE status='Submitted'")[0]["c"];
            ViewBag.TotalDonations = SqliteHelper.ExecuteQuery("SELECT COUNT(*) as c FROM bdonat")[0]["c"];
            return View();
        }

        // ─── USER MANAGEMENT ─────────────────────────────────────────────────
        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var users = SqliteHelper.ExecuteQuery("SELECT ID, Email, Name, Gender, Phone, Address, Aadhar, AreaCode FROM uRegistration");
            return View(users);
        }

        // ─── VOLUNTEER MANAGEMENT ─────────────────────────────────────────────
        public IActionResult Volunteers()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var vols = SqliteHelper.ExecuteQuery("SELECT ID, Email, Name, Gender, Phone, availstat FROM vRegistration");
            return View(vols);
        }

        // ─── PICKUP MANAGEMENT ────────────────────────────────────────────────
        public IActionResult Pickups(string? filter)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            string sql = string.IsNullOrEmpty(filter)
                ? "SELECT * FROM upick ORDER BY ID DESC"
                : "SELECT * FROM upick WHERE status=? ORDER BY ID DESC";
            var pickups = string.IsNullOrEmpty(filter)
                ? SqliteHelper.ExecuteQuery(sql)
                : SqliteHelper.ExecuteQuery(sql, new() { ["s"] = filter });
            ViewBag.Filter = filter ?? "All";
            return View(pickups);
        }

        [HttpPost]
        public IActionResult UpdatePickupStatus(int id, string status)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery("UPDATE upick SET status=? WHERE ID=?",
                new() { ["s"] = status, ["id"] = id });
            return RedirectToAction("Pickups");
        }

        // ─── HOTEL PICKUPS ───────────────────────────────────────────────────
        public IActionResult HotelPickups()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var picks = SqliteHelper.ExecuteQuery("SELECT * FROM hpick ORDER BY ID DESC");
            return View(picks);
        }

        [HttpPost]
        public IActionResult UpdateHotelPickupStatus(int id, string status)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery("UPDATE hpick SET status=? WHERE ID=?",
                new() { ["s"] = status, ["id"] = id });
            return RedirectToAction("HotelPickups");
        }

        // ─── VEHICLE MANAGEMENT ───────────────────────────────────────────────
        public IActionResult Vehicles()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var vehs = SqliteHelper.ExecuteQuery("SELECT * FROM Vehicle_details");
            return View(vehs);
        }

        public IActionResult AddVehicle() => View(new VehicleViewModel());

        [HttpPost]
        public IActionResult AddVehicle(VehicleViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery(
                "INSERT INTO Vehicle_details (Vehicle_no, owner_name, F_name, Uid_no, Contact, Email) VALUES (?,?,?,?,?,?)",
                new() { ["vn"] = model.VehicleNo, ["on"] = model.OwnerName, ["fn"] = model.FatherName,
                        ["un"] = model.UidNo, ["c"] = model.Contact, ["e"] = model.Email });
            model.Message = "Vehicle registered successfully!";
            return View(model);
        }

        // ─── AHAR SPOTS ───────────────────────────────────────────────────────
        public IActionResult AharSpots()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var spots = SqliteHelper.ExecuteQuery("SELECT * FROM aharspot");
            return View(spots);
        }

        public IActionResult AddAharSpot() => View(new AharSpotViewModel());

        [HttpPost]
        public IActionResult AddAharSpot(AharSpotViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery(
                "INSERT INTO aharspot (category, type, address, nopeople) VALUES (?,?,?,?)",
                new() { ["c"] = model.Category, ["t"] = model.Type, ["a"] = model.Address, ["n"] = model.NoPeople });
            model.Message = "Ahar Spot added successfully!";
            return View(model);
        }

        // ─── HOTEL MANAGEMENT ─────────────────────────────────────────────────
        public IActionResult Hotels()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var hotels = SqliteHelper.ExecuteQuery("SELECT * FROM hlog");
            return View(hotels);
        }

        public IActionResult AddHotel() => View(new HotelEntryViewModel());

        [HttpPost]
        public IActionResult AddHotel(HotelEntryViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery(
                "INSERT INTO hlog (hotelid, Name, fssai, Contact, Password, Email) VALUES (?,?,?,?,?,?)",
                new() { ["hi"] = model.HotelId, ["n"] = model.Name, ["f"] = model.Fssai,
                        ["c"] = model.Contact, ["p"] = model.Password, ["e"] = model.Email });
            model.Message = "Hotel registered successfully!";
            return View(model);
        }

        // ─── FEEDBACK MANAGEMENT ──────────────────────────────────────────────
        public IActionResult Feedback(string? filter)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var feedbacks = string.IsNullOrEmpty(filter)
                ? SqliteHelper.ExecuteQuery("SELECT * FROM feedback ORDER BY ID DESC")
                : SqliteHelper.ExecuteQuery("SELECT * FROM feedback WHERE Reason=? ORDER BY ID DESC", new() { ["r"] = filter });
            ViewBag.Filter = filter ?? "All";
            return View(feedbacks);
        }

        [HttpPost]
        public IActionResult UpdateFeedbackStatus(int id, string status)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery("UPDATE feedback SET Status=? WHERE ID=?",
                new() { ["s"] = status, ["id"] = id });
            return RedirectToAction("Feedback");
        }

        [HttpPost]
        public IActionResult DeleteFeedback(int id)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery("DELETE FROM feedback WHERE ID=?", new() { ["id"] = id });
            return RedirectToAction("Feedback");
        }

        // ─── CONTACT INFO ─────────────────────────────────────────────────────
        public IActionResult ContactInfo()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var contacts = SqliteHelper.ExecuteQuery("SELECT * FROM Contact ORDER BY ID DESC");
            return View(contacts);
        }

        // ─── INFORMATION POSTS ────────────────────────────────────────────────
        public IActionResult PostInfo() => View(new InfoViewModel());

        [HttpPost]
        public IActionResult PostInfo(InfoViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            SqliteHelper.ExecuteNonQuery(
                "INSERT INTO info (Subject, Details, Link, Date) VALUES (?,?,?,?)",
                new() { ["s"] = model.Subject, ["d"] = model.Description, ["l"] = model.Link,
                        ["dt"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            model.Message = "Information posted successfully!";
            return View(model);
        }

        // ─── DONATIONS ────────────────────────────────────────────────────────
        public IActionResult Donations()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var donations = SqliteHelper.ExecuteQuery("SELECT * FROM bdonat ORDER BY ID DESC");
            return View(donations);
        }

        // ─── FOOD DELIVERY ────────────────────────────────────────────────────
        public IActionResult DeliverFood()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var spots = SqliteHelper.ExecuteQuery("SELECT * FROM aharspot");
            var vols = SqliteHelper.ExecuteQuery("SELECT * FROM vRegistration WHERE availstat='available'");
            var vehs = SqliteHelper.ExecuteQuery("SELECT * FROM Vehicle_details");
            ViewBag.Spots = spots;
            ViewBag.Volunteers = vols;
            ViewBag.Vehicles = vehs;
            return View();
        }

        [HttpPost]
        public IActionResult DeliverFood(int spotId, int volId, int vehId)
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            // Update volunteer status to Busy
            SqliteHelper.ExecuteNonQuery("UPDATE vRegistration SET availstat='busy' WHERE ID=?", new() { ["id"] = volId });
            TempData["DeliveryMsg"] = "Delivery assigned! Email notifications sent to volunteer and driver.";
            return RedirectToAction("DeliverFood");
        }

        // ─── NEW PICKUP (assign volunteer+vehicle) ────────────────────────────
        public IActionResult NewPickup()
        {
            if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
            var upicks = SqliteHelper.ExecuteQuery("SELECT * FROM upick WHERE status='Submitted' ORDER BY ID DESC");
            var hpicks = SqliteHelper.ExecuteQuery("SELECT * FROM hpick WHERE status='Submitted' ORDER BY ID DESC");
            var vols = SqliteHelper.ExecuteQuery("SELECT * FROM vRegistration WHERE availstat='available'");
            var vehs = SqliteHelper.ExecuteQuery("SELECT * FROM Vehicle_details");
            ViewBag.UserPickups = upicks;
            ViewBag.HotelPickups = hpicks;
            ViewBag.Volunteers = vols;
            ViewBag.Vehicles = vehs;
            return View();
        }
    }
}
