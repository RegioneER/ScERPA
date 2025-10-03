using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ScERPA.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string? displayName= "";

            try
            {
               displayName = enumValue?.GetType()
              ?.GetMember(enumValue.ToString())
              ?.FirstOrDefault()
              ?.GetCustomAttribute<DisplayAttribute>()
              ?.GetName();
            } catch
            {
                displayName = enumValue.ToString();
            }

            return displayName ?? "";
        }
    }
}
