namespace Myras.Types
{
    /// <summary>
    /// Represents a row of data containing X and Y data points.
    /// </summary>
    public class XYDataRow
    {
        /// <summary>
        /// List of X values for the row.
        /// </summary>
        public IList<float> X { get; set; } = [];

        /// <summary>
        /// List of Y values for the row.
        /// </summary>
        public IList<float> Y { get; set; } = [];
    }
}
