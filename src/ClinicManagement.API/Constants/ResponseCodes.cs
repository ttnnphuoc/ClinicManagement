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
    }

    public static class Common
    {
        public const string Success = "SUCCESS";
        public const string BadRequest = "BAD_REQUEST";
        public const string NotFound = "NOT_FOUND";
        public const string InternalError = "INTERNAL_ERROR";
        public const string ValidationError = "VALIDATION_ERROR";
    }
}