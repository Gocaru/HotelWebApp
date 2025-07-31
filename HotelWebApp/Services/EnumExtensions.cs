using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HotelWebApp.Services
{
    /// <summary>
    /// Provides extension methods for Enum types.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Retrieves the display name of an enum value.
        /// It reads the Name property from the [Display] attribute if it exists;
        /// otherwise, it falls back to the standard string representation of the enum member.
        /// </summary>
        /// <param name="enumValue">The enum value from which to get the display name.</param>
        /// <returns>The display name string.</returns>
        /// <example>
        /// <code>
        /// ReservationStatus.CheckedIn.GetDisplayName(); // Returns "Checked-In"
        /// </code>
        /// </example>
        public static string GetDisplayName(this Enum enumValue)
        {
            // Using reflection to get the MemberInfo of the enum value.
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            // We expect exactly one member, so First() is safe.
                            .First()
                            // Get the [Display] attribute from the member.
                            .GetCustomAttribute<DisplayAttribute>()?
                            // If the attribute exists, get its Name property.
                            .GetName() ??
                            // If the attribute or its Name is null, fall back to the default ToString().
                            enumValue.ToString();
        }
    }
}
