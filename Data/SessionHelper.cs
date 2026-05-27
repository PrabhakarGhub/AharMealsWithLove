namespace AharMealsWithLove.Data
{
    // Central helper to check login state across all controllers
    public static class SessionHelper
    {
        public static bool IsUserLoggedIn(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString("umail") != null;

        public static bool IsVolunteerLoggedIn(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString("vmail") != null;

        public static bool IsAdminLoggedIn(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString("adminId") != null;

        public static bool IsHotelLoggedIn(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString("hmail") != null;

        public static string GetUserEmail(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString("umail") ?? "";

        public static string GetUserName(IHttpContextAccessor ctx) =>
            ctx.HttpContext?.Session.GetString("uname") ?? "User";
    }
}
