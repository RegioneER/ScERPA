using ScERPA.Areas.Administration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class ConfigurationEditCreateViewModel
    {
        public ConfigurationEditInputModel inputModel { get; set; } = new();

    }
}
