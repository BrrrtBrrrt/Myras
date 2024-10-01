namespace Myras.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Random"/> class.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Generates a random single-precision floating-point number in the range [0.0, 1.0).
        /// </summary>
        /// <param name="random">The instance of <see cref="Random"/> used to generate the random number.</param>
        /// <returns>
        /// A single-precision floating-point number that is greater than or equal to 0.0 
        /// and less than 1.0.
        /// </returns>
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }
    }
}
