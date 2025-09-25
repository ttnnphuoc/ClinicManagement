namespace ClinicManagement.API.Constants;

public static class ResponseCodes
{
    public static class Auth
    {
        public const string LoginSuccess = "AUTH_LOGIN_SUCCESS";
        public const string FieldsRequired = "AUTH_FIELDS_REQUIRED";
        public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
        public const string Unauthorized = "AUTH_UNAUTHORIZED";
        public const string ClinicAccessDenied = "AUTH_CLINIC_ACCESS_DENIED";
        public const string EmailExists = "AUTH_EMAIL_EXISTS";
    }

    public static class Common
    {
        public const string Success = "SUCCESS";
        public const string BadRequest = "BAD_REQUEST";
        public const string NotFound = "NOT_FOUND";
        public const string InternalError = "INTERNAL_ERROR";
        public const string ValidationError = "VALIDATION_ERROR";
        public const string InvalidInput = "INVALID_INPUT";
    }

    public static class Appointment
    {
        public const string TimeSlotNotAvailable = "APPOINTMENT_TIME_SLOT_NOT_AVAILABLE";
    }

    public static class Subscription
    {
        public const string NoActiveSubscription = "SUBSCRIPTION_NO_ACTIVE";
        public const string SubscriptionExpired = "SUBSCRIPTION_EXPIRED";
        public const string LimitExceeded = "SUBSCRIPTION_LIMIT_EXCEEDED";
        public const string PackageNotFound = "SUBSCRIPTION_PACKAGE_NOT_FOUND";
        public const string SubscriptionExists = "SUBSCRIPTION_ALREADY_EXISTS";
        public const string InvalidUpgrade = "SUBSCRIPTION_INVALID_UPGRADE";
        public const string RenewalNotAllowed = "SUBSCRIPTION_RENEWAL_NOT_ALLOWED";
    }
}