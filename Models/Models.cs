using System.ComponentModel.DataAnnotations;

namespace AharMealsWithLove.Models
{
    public class UserRegistration
    {
        public int ID { get; set; }
        [Required] public string Email { get; set; } = "";
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Aadhar { get; set; }
        public string? AreaCode { get; set; }
        public string? Image { get; set; }
    }

    public class VolunteerRegistration
    {
        public int ID { get; set; }
        [Required] public string Email { get; set; } = "";
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Aadhar { get; set; }
        public string? AreaCode { get; set; }
        public string? Image { get; set; }
        public string AvailStat { get; set; } = "unavailable";
    }

    public class LoginViewModel
    {
        [Required] public string Email { get; set; } = "";
        [Required] public string Password { get; set; } = "";
        public string? OTP { get; set; }
        public bool OtpSent { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class SignupViewModel
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        public string? OTP { get; set; }
        public bool OtpSent { get; set; }
        public string? Message { get; set; }
    }

    public class ProfileViewModel
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Aadhar { get; set; }
        public string? AreaCode { get; set; }
        public IFormFile? Image { get; set; }
        public string? ExistingImage { get; set; }
        public string? Message { get; set; }
    }

    public class PickupRequest
    {
        public int ID { get; set; }
        public string? EventDetail { get; set; }
        public string? Venue { get; set; }
        public string? Date { get; set; }
        public string Category { get; set; } = "veg";
        public string? Quantity { get; set; }
        public string? CookTime { get; set; }
        public string? ExpTime { get; set; }
        public string AreaCode { get; set; } = "110";
        public string Status { get; set; } = "Submitted";
        public string? Message { get; set; }
    }

    public class FeedbackViewModel
    {
        public string Reason { get; set; } = "Feedback";
        public string ComplainFor { get; set; } = "None";
        public string? Detail { get; set; }
        public string? Message { get; set; }
        public int? Ticket { get; set; }
    }

    public class ContactViewModel
    {
        [Required] public string Name { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Message { get; set; } = "";
        public string? SuccessMessage { get; set; }
    }

    public class BirthdayViewModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? DOB { get; set; }
        public string? Address { get; set; }
        public string Capacity { get; set; } = "100";
        public string? Amount { get; set; }
    }

    public class DonationViewModel
    {
        public string? BillingAddress { get; set; }
        public string? ServiceType { get; set; }
        public string? Amount { get; set; }
        public string? TotalAmount { get; set; }
    }

    public class PaymentViewModel
    {
        public string? Method { get; set; }
        public string? CardNumber { get; set; }
        public string? Expiry { get; set; }
        public string? CVV { get; set; }
        public string? CardName { get; set; }
        public string? SelectedBank { get; set; }
        public List<string> Banks { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class AdminLoginViewModel
    {
        [Required] public string AdminId { get; set; } = "";
        [Required] public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }
    }

    public class HotelLoginViewModel
    {
        [Required] public string HotelId { get; set; } = "";
        [Required] public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }
    }

    public class HotelEntryViewModel
    {
        public string? HotelId { get; set; }
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public string? Fssai { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Message { get; set; }
    }

    public class VehicleViewModel
    {
        public string? VehicleNo { get; set; }
        public string? OwnerName { get; set; }
        public string? FatherName { get; set; }
        public string? UidNo { get; set; }
        public string? Contact { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
    }

    public class AharSpotViewModel
    {
        public string Category { get; set; } = "Urban";
        public string Type { get; set; } = "Economically Backward Slums";
        public string? Address { get; set; }
        public string? NoPeople { get; set; }
        public string? Message { get; set; }
    }

    public class InfoViewModel
    {
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public string? Message { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string? OldPassword { get; set; }
        [Required] public string Password { get; set; } = "";
        [Required, Compare("Password")] public string ConfirmPassword { get; set; } = "";
        public string? Message { get; set; }
    }
}
