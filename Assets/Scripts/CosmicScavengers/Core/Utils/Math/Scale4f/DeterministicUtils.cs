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
        /// Converts a Unity float to a deterministic unscaled long.
        /// Example: 1.23456f -> 12345L
        /// </summary>
        public static long ToUnscaled(float value)
        {
            return (long)Math.Round(value * SCALE_FACTOR);
        }

        /// <summary>
        /// Converts an unscaled long back to a Unity float.
        /// </summary>
        public static float FromUnscaled(long unscaledValue)
        {
            return (float)unscaledValue / SCALE_FACTOR;
        }
    }
}
