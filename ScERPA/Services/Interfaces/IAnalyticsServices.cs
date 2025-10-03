using ScERPA.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ScERPA.Services.Interfaces
{
    public interface IAnalyticsServices
    {
        public Task<TopServicesChartDto> GetTopServices(int first);

    }
}
