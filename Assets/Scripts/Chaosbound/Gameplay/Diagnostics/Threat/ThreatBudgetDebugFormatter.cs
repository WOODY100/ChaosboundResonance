using System;
using System.Text;

namespace Chaosbound.Gameplay.Diagnostics.Threat
{
    /// <summary>
    /// Formats Threat Budget diagnostic snapshots into
    /// human-readable reports.
    /// </summary>
    public sealed class ThreatBudgetDebugFormatter
    {
        /// <summary>
        /// Formats the supplied snapshot.
        /// </summary>
        public string Format(
            ThreatBudgetDebugSnapshot snapshot)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            StringBuilder builder =
                new StringBuilder();

            builder.AppendLine("================================");
            builder.AppendLine(" THREAT BUDGET");
            builder.AppendLine("================================");

            builder.AppendLine(
                $"Time............... {snapshot.Time:F2}");

            builder.AppendLine(
                $"Pressure........... {snapshot.Pressure:F2}");

            builder.AppendLine(
                $"Capacity........... {snapshot.Capacity:F2}");

            builder.AppendLine(
                $"Invested........... {snapshot.InvestedThreat:F2}");

            builder.AppendLine(
                $"Available.......... {snapshot.AvailableThreat:F2}");

            builder.AppendLine(
                $"Alive Enemies...... {snapshot.AliveEnemies}");

            builder.AppendLine(
                $"Alive Enemies...... {snapshot.AliveEnemies}");

            builder.AppendLine();
            builder.AppendLine("COMPOSITION");

            foreach (RuntimeCompositionDebugEntry entry
                in snapshot.Composition)
            {
                builder.AppendLine(
                    $"  {entry.Name,-18} {entry.AliveCount}");
            }

            builder.AppendLine("================================");

            return builder.ToString();
        }
    }
}