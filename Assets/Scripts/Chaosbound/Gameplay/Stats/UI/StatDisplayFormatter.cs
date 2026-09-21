using System.Globalization;

namespace Chaosbound.Gameplay.Stats.UI
{
    public static class StatDisplayFormatter
    {
        public static string Format(
            float value,
            StatDisplayFormat format)
        {
            switch (format)
            {
                case StatDisplayFormat.Integer:
                    return value.ToString(
                        "0",
                        CultureInfo.InvariantCulture);

                case StatDisplayFormat.Decimal:
                    return value.ToString(
                        "0.##",
                        CultureInfo.InvariantCulture);

                case StatDisplayFormat.Percent:
                    return (value * 100f).ToString(
                        "0.##",
                        CultureInfo.InvariantCulture) + "%";

                case StatDisplayFormat.PerSecond:
                    return value.ToString(
                        "0.##",
                        CultureInfo.InvariantCulture) + "/s";

                case StatDisplayFormat.Multiplier:
                    return (value * 100f).ToString(
                        "0.##",
                        CultureInfo.InvariantCulture) + "%";

                default:
                    return value.ToString(
                        "0.##",
                        CultureInfo.InvariantCulture);
            }
        }
    }
}