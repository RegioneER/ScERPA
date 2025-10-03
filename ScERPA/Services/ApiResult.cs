using Microsoft.AspNetCore.Http;

namespace ScERPA.Services
{
    public enum ApiResultStatus { 
        Unknown = 0,
        Success = 1,
        NotFound = 2,
        Deleted = 3,
        GenericError = 99,
        DeserializeError = 100,
        EmptyResult=101,
        BadRequest=102,
        Failed = 999,
        FailedOnServiceProvider = 1000,
        FailedOnServiceProviderGeneric = 1001,
        FailedServiceCall = 1002,
        FailedApiManagerToken = 1003,
        FailedPurpouseConfig =1004,
        FailedServiceConfig=1005,
        FailedApiManagerGetToken=1006        
    }
  

    public class ApiResult<T>
    {
        public ApiResult(string? callGuid) 
        {
            OperationGuid = string.IsNullOrEmpty(callGuid)  ? Guid.NewGuid().ToString() : callGuid ;
        }

        public ApiResultStatus Status { get; set; } = ApiResultStatus.Unknown;

        public string? Message { get; set; }

        public T? Data { get; set; }

        public string OperationGuid { get; set; } = string.Empty;
        public string? ResultGuid { get; set; }= string.Empty;
    }
}
