using System;

namespace CosmicScavengers.Core.Systems.Utils.Scale4f
{
    /// <summary>
    /// Mirror of Java's DecimalUtils. Ensures that floats are converted to
    /// deterministic longs using Scale4f (4 decimal places).
    /// </summary>
    public static class DeterministicUtils
    {
        private const long SCALE_FACTOR = 10000;

        /// <summary>
        /// Converts a Unity float to a deterministic unscaled long (Quantize/Encode).
        /// Used when sending data to the server or saving.
        /// Example: 1.23456f -> 12345L
        /// </summary>
        public static long ToScaled(float value)
        {
            // Use MidpointRounding.AwayFromZero to match Java's standard rounding behavior
            return (long)Math.Round(value * SCALE_FACTOR, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Converts a deterministic long back to a Unity float (Decode).
        /// Used for Unity's transform.position and visual calculations.
        /// Example: 12345L -> 1.2345f
        /// </summary>
        public static float FromScaled(long scaledValue)
        {
            return (float)scaledValue / SCALE_FACTOR;
        }
    }
}
