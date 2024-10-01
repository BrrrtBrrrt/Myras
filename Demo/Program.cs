using Myras.Enums;
using Myras.Extensions;
using Myras.Services;
using Myras.Types;
using Myras.Types.LayerTypes;
using Myras.Utils;

namespace Demo
{
    internal class Program
    {
        private static readonly Random random = new(Seed: 23144532);

        static void Main(string[] args)
        {
            // 0. Configurations

            // For test purpouses sine wave data is generated
            int dataSetSize = 1500;
            int yStartIndex = 1;
            float xMin = MathF.PI * -2;
            float xMax = MathF.PI * 2;
            float xRange = xMax - xMin;
            float xStepLength = xRange / dataSetSize;
            float noiseFactor = 0.05f;
            float scaleXMin = -1;
            float scaleXMax = 1;
            float scaleYMin = -1;
            float scaleYMax = 1;
            float trainTestSplitFaktor = 0.75f;
            int batchSize = 32;
            float learningRate = 0.001f;
            int epochs = 100;
            OptimizerType optimizer = OptimizerType.ADAM;
            LossFunctionType lossFunction = LossFunctionType.MSE;

            // 1. Generate raw Data

            float minXGenerated = float.MaxValue;
            float maxXGenerated = float.MinValue;
            float minYGenerated = float.MaxValue;
            float maxYGenerated = float.MinValue;
            float[][] data = GenerateData(dataSetSize, xMin, xStepLength, noiseFactor, ref minXGenerated, ref maxXGenerated, ref minYGenerated, ref maxYGenerated);

            float[][] dataShuffled = data.CopyDeep();

            // 2. Prepare X Y Train/Test Data

            XYData trainTestDataOriginalOrder = XYData.Convert(data, yStartIndex);

            ScalerService scalerServiceX = new()
            {
                OriginalMin = minXGenerated,
                OriginalMax = maxXGenerated,
                NewMin = scaleXMin,
                NewMax = scaleXMax,
            };

            ScalerService scalerServiceY = new()
            {
                OriginalMin = minYGenerated,
                OriginalMax = maxYGenerated,
                NewMin = scaleYMin,
                NewMax = scaleYMax,
            };
            XYData trainTestDataPrepared = XYData.Convert(dataShuffled, yStartIndex);

            foreach (XYDataRow row in trainTestDataPrepared.Rows)
            {
                row.X = scalerServiceX.Scale(row.X);
                row.Y = scalerServiceY.Scale(row.Y);
            }
            (XYData trainDataPrepared, XYData testDataPrepared) = XYData.Split(trainTestDataPrepared, trainTestSplitFaktor);

            trainDataPrepared.Rows.Shuffle();
            testDataPrepared.Rows.Shuffle();

            // 3. Define Model

            Input input = Layers.Input(shape: new([1]), batchSize: batchSize);
            Layer x = Layers.Dense(units: 50, activation: ActivationFunctionType.RE_LU)(input);
            x = Layers.Dense(units: 100, activation: ActivationFunctionType.RE_LU)(x);
            x = Layers.Dense(units: 200, activation: ActivationFunctionType.RE_LU)(x);
            Layer output = Layers.Dense(units: 1, activation: ActivationFunctionType.LINEAR)(x);

            Model model = new([input], [output]);

            // 4. Compile Model

            model.Compile(optimizer, new()
            {
                { "LearningRate", learningRate}
            }, lossFunction);

            // 5. Train Model

            model.Fit(trainData: trainDataPrepared, testData: testDataPrepared, epochs: epochs, batchSize: batchSize);

            // 6. Plot predictions for training data

            Test(model, testDataPrepared, batchSize);
        }

        private static float[][] GenerateData(int dataSetSize, float xMin, float xStepLength, float noiseFactor, ref float minXGenerated, ref float maxXGenerated, ref float minYGenerated, ref float maxYGenerated)
        {
            float[][] data = new float[dataSetSize][];

            for (int dataSetRowIndex = 0; dataSetRowIndex < dataSetSize; dataSetRowIndex++)
            {
                float[] dataSetRow = new float[2];
                float x = xMin + (xStepLength * dataSetRowIndex);
                float y = MathF.Sin(x) + (noiseFactor * GenerateGaussianNoise());
                dataSetRow[0] = x;
                dataSetRow[1] = y;
                data[dataSetRowIndex] = dataSetRow;

                if (x < minXGenerated) minXGenerated = x;
                if (x > maxXGenerated) maxXGenerated = x;

                if (y < minYGenerated) minYGenerated = y;
                if (y > maxYGenerated) maxYGenerated = y;
            }

            return data;
        }

        /// <summary>
        /// Generates Gaussian noise using the Box-Muller transform.
        /// </summary>
        /// <returns>A float representing Gaussian noise with mean 0 and standard deviation 1.</returns>
        public static float GenerateGaussianNoise()
        {
            // Using Box-Muller transform to generate Gaussian noise
            float u1 = 1.0f - random.NextFloat(); // Uniform(0,1] random floats
            float u2 = 1.0f - random.NextFloat();
            float randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Sin(2.0f * MathF.PI * u2); // Random normal (0,1)
            return randStdNormal;
        }

        private static void Test(Model model, XYData data, int? batchSize)
        {
            IList<float> xValues = [];
            IList<float> yPredictedValues = [];
            IList<float> yTargetValues = [];

            for (int i = 0; i < data.Rows.Count; i += batchSize ?? 1)
            {
                IList<float> xBatchRaw = [];
                IList<float> yBatchRaw = [];
                bool isRemainingDataToSmallForBatch = false;
                for (int j = i; j < i + (batchSize ?? 1); j++)
                {
                    if (j == data.Rows.Count)
                    {
                        isRemainingDataToSmallForBatch = true;
                        break;
                    }

                    xBatchRaw = [.. xBatchRaw, .. data.Rows[j].X];
                    yBatchRaw = [.. yBatchRaw, .. data.Rows[j].Y];
                }

                if (isRemainingDataToSmallForBatch)
                    break;

                Tensor xBatch = new(new Shape([batchSize ?? 1, data.Rows[i].X.Count]), xBatchRaw);
                Tensor yBatchTarget = new(new Shape([batchSize ?? 1, data.Rows[i].Y.Count]), yBatchRaw);

                Tensor yBatchPredicted = model.ForwardPass([xBatch])[0];

                xValues = [.. xValues, .. xBatch.Values.Values];
                yPredictedValues = [.. yPredictedValues, .. yBatchPredicted.Values.Values];
                yTargetValues = [.. yTargetValues, .. yBatchTarget.Values.Values];
            }

            List<float> xValuesTmp = [.. xValues];

            xValuesTmp.SortWithRelatedLists((a, b) =>
            {
                return a.CompareTo(b);
            }, [xValues, yPredictedValues, yTargetValues]);


            Console.WriteLine($"yTarget\tyPredicted:");
            for (int i = 0; i < xValues.Count; i++)
            {
                float xValue = xValues[i];
                float yTargetValue = yTargetValues[i];
                float yPredictedValue = yPredictedValues[i];

                Console.WriteLine($"{yTargetValue}\t{yPredictedValue}");
            }
        }
    }
}
