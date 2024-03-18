using System.ComponentModel.DataAnnotations;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplay(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return string.Empty;

            var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes.FirstOrDefault()?.Name ?? value.ToString();
        }
    }
}