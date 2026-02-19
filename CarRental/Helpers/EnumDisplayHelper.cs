using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CarRental.Helpers
{
    public static class EnumDisplayHelper
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).First();

            DisplayAttribute? displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}
