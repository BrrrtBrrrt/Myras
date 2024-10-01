namespace Myras.Services
{
    /// <summary>
    /// Provides scaling and reverse scaling functionality for values within a specified range.
    /// </summary>
    public class ScalerService
    {
        /// <summary>
        /// The minimum value of the original range.
        /// </summary>
        public float OriginalMin { get; set; }

        /// <summary>
        /// The maximum value of the original range.
        /// </summary>
        public float OriginalMax { get; set; }

        /// <summary>
        /// The minimum value of the new range.
        /// </summary>
        public float NewMin { get; set; }

        /// <summary>
        /// The maximum value of the new range.
        /// </summary>
        public float NewMax { get; set; }

        public IList<IList<float>> Scale(IList<IList<float>> values)
        {
            IList<IList<float>> result = [];
            foreach (IList<float> value in values)
            {
                result.Add(Scale(value));
            }
            return result;
        }

        public IList<IList<float>> ScaleBack(IList<IList<float>> values)
        {
            IList<IList<float>> result = [];
            foreach (IList<float> value in values)
            {
                result.Add(ScaleBack(value));
            }
            return result;
        }

        /// <summary>
        /// Scales a collection of values from the original range to the new range.
        /// </summary>
        /// <typeparam name="T">The type of the collection that implements <see cref="IList{T}"/> and has a parameterless constructor.</typeparam>
        /// <param name="values">The collection of values to scale.</param>
        /// <returns>A new collection containing the scaled values.</returns>
        public IList<float> Scale(IList<float> values)
        {
            IList<float> result = [];
            foreach (float value in values)
            {
                result.Add(Scale(value));
            }
            return result;
        }

        /// <summary>
        /// Scales a collection of values from the new range back to the original range.
        /// </summary>
        /// <typeparam name="T">The type of the collection that implements <see cref="IList{T}"/> and has a parameterless constructor.</typeparam>
        /// <param name="values">The collection of values to scale back.</param>
        /// <returns>A new collection containing the values scaled back to the original range.</returns>
        public IList<float> ScaleBack(IList<float> values)
        {
            IList<float> result = [];
            foreach (float value in values)
            {
                result.Add(ScaleBack(value));
            }
            return result;
        }

        /// <summary>
        /// Scales a value from the original range to the new range.
        /// </summary>
        /// <param name="value">The value to scale.</param>
        /// <returns>The scaled value within the new range.</returns>
        public float Scale(float value)
        {
            return ((value - OriginalMin) * (NewMax - NewMin) / (OriginalMax - OriginalMin)) + NewMin;
        }


        /// <summary>
        /// Scales a value from the new range back to the original range.
        /// </summary>
        /// <param name="value">The value to scale back.</param>
        /// <returns>The value scaled back to the original range.</returns>
        public float ScaleBack(float value)
        {
            return (value - NewMin) * (OriginalMax - OriginalMin) / (NewMax - NewMin) + OriginalMin;
        }
    }
}
