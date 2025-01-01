using System.Text.Json.Serialization;

namespace Ailos.Application.Common.Models
{
    public enum ErrorTypesEnum
    {
        [JsonStringEnumMemberName(ErrorTypes.ValidationError)]
        ValidationError,

        [JsonStringEnumMemberName(ErrorTypes.InvalidAccount)]
        InvalidAccount,

        [JsonStringEnumMemberName(ErrorTypes.InactiveAccount)]
        InactiveAccount,

        [JsonStringEnumMemberName(ErrorTypes.InvalidValue)]
        InvalidValue,

        [JsonStringEnumMemberName(ErrorTypes.InvalidType)]
        InvalidType,

        [JsonStringEnumMemberName(ErrorTypes.InternalError)]
        InternalError,
    }

    public static class ErrorTypes
    {
        public const string ValidationError = "VALIDATION_ERROR";
        public const string InvalidAccount = "INVALID_ACCOUNT";
        public const string InactiveAccount = "INACTIVE_ACCOUNT";
        public const string InvalidValue = "INVALID_VALUE";
        public const string InvalidType = "INVALID_TYPE";
        public const string InternalError = "INTERNAL_ERROR";
    }
}
