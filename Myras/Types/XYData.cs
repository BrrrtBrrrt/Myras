namespace Myras.Types
{
    /// <summary>
    /// Represents a dataset that contains X and Y data points.
    /// </summary>
    public class XYData
    {
        /// <summary>
        /// Collection of rows, where each row consists of X and Y data points.
        /// </summary>
        public IList<XYDataRow> Rows { get; set; } = [];

        /// <summary>
        /// Converts a two-dimensional float array into an XYData object.
        /// The array is split into X and Y parts based on the specified yStartIndex.
        /// </summary>
        /// <param name="data">Two-dimensional float array where each row contains data points.</param>
        /// <param name="yStartIndex">The index in each row at which the Y data starts (all values before this index are treated as X values).</param>
        /// <returns>A new XYData object with the data split into X and Y parts.</returns>
        public static XYData Convert(float[][] data, int yStartIndex)
        {
            XYData result = new();

            for (int rowIndex = 0; rowIndex < data.Length; rowIndex++)
            {
                float[] dataRow = data[rowIndex];
                List<float> x = [];
                List<float> y = [];

                for (int valueIndex = 0; valueIndex < dataRow.Length; valueIndex++)
                {
                    if (valueIndex < yStartIndex)
                    {
                        x.Add(dataRow[valueIndex]);
                    }
                    else
                    {
                        y.Add(dataRow[valueIndex]);
                    }
                }

                result.Rows.Add(new()
                {
                    X = x,
                    Y = y,
                });
            }

            return result;
        }

        /// <summary>
        /// Splits an XYData object into training and testing datasets based on the specified split factor.
        /// The test data points are selected evenly from the original dataset.
        /// </summary>
        /// <param name="data">The original XYData to be split into train and test datasets.</param>
        /// <param name="splitFactor">The proportion of data to be used for training (between 0 and 1).</param>
        /// <returns>A tuple containing the training data as the first item and test data as the second item.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when splitFactor is less than 0 or greater than 1.</exception>
        public static (XYData, XYData) Split(XYData data, float splitFactor = 0.8f)
        {
            if (splitFactor < 0 || splitFactor > 1)
                throw new ArgumentOutOfRangeException(nameof(splitFactor), "splitFactor must be between 0 and 1");

            XYData trainData = new();
            XYData testData = new();

            // Evenly pick test data points from the entire dataset
            int testDataSize = data.Rows.Count - (int)(splitFactor * data.Rows.Count);
            float testDataIndexStepRaw = (float)data.Rows.Count / testDataSize;
            float testDataIndexRaw = 0;

            for (int dataIndex = 0; dataIndex < data.Rows.Count; dataIndex++)
            {
                if (dataIndex >= (int)testDataIndexRaw)
                {
                    testData.Rows.Add(new()
                    {
                        X = data.Rows[dataIndex].X,
                        Y = data.Rows[dataIndex].Y,
                    });

                    testDataIndexRaw += testDataIndexStepRaw;
                    continue;
                }

                trainData.Rows.Add(new()
                {
                    X = data.Rows[dataIndex].X,
                    Y = data.Rows[dataIndex].Y,
                });
            }

            return (trainData, testData);
        }
    }
}
