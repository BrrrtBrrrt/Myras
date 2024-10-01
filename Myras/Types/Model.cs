using Myras.Enums;
using Myras.Extensions;
using Myras.Services;
using Myras.Services.OptimizerImpl;
using Myras.Types.LayerTypes;
using Myras.Utils;

namespace Myras.Types
{
    /// <summary>
    /// Represents a neural network model.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// A list of input layers for the model, which define the model's input shape and data.
        /// </summary>
        private readonly IList<Input> inputs;

        /// <summary>
        /// A list of output layers for the model, which define the model's output shape and data.
        /// </summary>
        private readonly IList<Layer> outputs;

        /// <summary>
        /// The optimizer service used for training the model. This can be <c>null</c> if the model is not yet compiled.
        /// </summary>
        private OptimizerService? optimizer = null;

        /// <summary>
        /// The type of loss function used to evaluate the model's performance during training.
        /// </summary>
        private LossFunctionType lossFunction;

        /// <summary>
        /// A list of tensors representing the trainable weights of the model. These weights are updated during the training process.
        /// </summary>
        private readonly List<Tensor> trainableWeights = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class with specified inputs and outputs.
        /// </summary>
        /// <param name="inputs">A list of input layers for the model.</param>
        /// <param name="outputs">A list of output layers for the model.</param>
        public Model(IList<Input> inputs, IList<Layer> outputs)
        {
            this.inputs = inputs;
            this.outputs = outputs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class with a single input and output layer.
        /// </summary>
        /// <param name="input">The input layer for the model.</param>
        /// <param name="output">The output layer for the model.</param>
        public Model(Input input, Layer output)
        {
            inputs = [input];
            outputs = [output];

        }

        /// <summary>
        /// Compiles the model by setting the optimizer and loss function, and initializing trainable weights.
        /// </summary>
        /// <param name="optimizer">The optimizer type to be used for training.</param>
        /// <param name="optimizerParameters">Parameters to configure the optimizer.</param>
        /// <param name="lossFunction">The loss function type to be used during training.</param>
        public void Compile(OptimizerType optimizer, Dictionary<string, dynamic> optimizerParameters, LossFunctionType lossFunction)
        {
            IList<Layer> layers = [];

            foreach (Input layer in inputs)
                layers.Add(layer);

            while (layers.Count > 0)
            {
                IList<Layer> layersTmp = [];

                foreach (Layer layer in layers)
                {
                    switch (layer.Type)
                    {
                        case LayerType.INPUT:
                            break;
                        case LayerType.DENSE:
                            Dense denseLayer = (Dense)layer;
                            int previousLayerUnitCount = denseLayer.LayersPrevious.Count == 0 ? 0 : denseLayer.LayersPrevious[0].OutputShape.Dimensions[^1];
                            int nextLayerUnitCount = denseLayer.LayersNext.Count == 0 ? 0 : denseLayer.LayersNext[0].InputShape.Dimensions[^1];

                            for (int i = 0; i < denseLayer.Kernel.Values.Values.Count; i++)
                            {
                                float range = MathF.Sqrt(6f / (previousLayerUnitCount + nextLayerUnitCount));
                                float weight = Constants.random.NextFloat() * (range - (-range)) + (-range);
                                denseLayer.Kernel.Values.Values[i] = weight;
                            }
                            if (denseLayer.Trainable) trainableWeights.AddRange([denseLayer.Kernel, denseLayer.Biases]);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    layersTmp = [.. layersTmp, .. layer.LayersNext];
                }

                layers = layersTmp;
            }



            switch (optimizer)
            {
                case OptimizerType.ADAM:
                    IList<Tensor> firstMomentumVector = [];
                    IList<Tensor> secondMomentumVector = [];

                    layers = [];

                    foreach (Input layer in inputs)
                        layers.Add(layer);

                    while (layers.Count > 0)
                    {
                        IList<Layer> layersTmp = [];

                        foreach (Layer layer in layers)
                        {
                            switch (layer.Type)
                            {
                                case LayerType.INPUT:
                                    break;
                                case LayerType.DENSE:
                                    Dense denseLayer = (Dense)layer;
                                    firstMomentumVector.Add(new(denseLayer.Kernel.Shape));
                                    firstMomentumVector.Add(new(denseLayer.Biases.Shape));
                                    secondMomentumVector.Add(new(denseLayer.Kernel.Shape));
                                    secondMomentumVector.Add(new(denseLayer.Biases.Shape));
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            layersTmp = [.. layersTmp, .. layer.LayersNext];
                        }

                        layers = layersTmp;
                    }

                    optimizerParameters.Add(nameof(AdamOptimizerService.FirstMomentumVector), firstMomentumVector);
                    optimizerParameters.Add(nameof(AdamOptimizerService.SecondMomentumVector), secondMomentumVector);

                    this.optimizer = new AdamOptimizerService();
                    this.optimizer.Initialize(optimizerParameters);
                    break;
                default:
                    throw new NotImplementedException();
            }
            this.lossFunction = lossFunction;
        }

        /// <summary>
        /// Trains the model on the provided training and testing data for a specified number of epochs.
        /// </summary>
        /// <param name="trainData">The training data containing input and target values.</param>
        /// <param name="testData">The testing data to evaluate the model's performance.</param>
        /// <param name="batchSize">The size of the batches for training (optional).</param>
        /// <param name="epochs">The number of epochs to train the model.</param>
        public void Fit(XYData trainData, XYData testData, int? batchSize = null, int epochs = 1)
        {
            // Ensure that the optimizer is set before training
            if (optimizer == null)
                throw new ArgumentException("Model optimizer is not set, model is probably not compiled");

            // Lists to store training and testing loss for each epoch
            IList<float> epochErrorsTrain = [];
            IList<float> epochErrorsTest = [];

            Console.WriteLine("Training started");

            // Start the training process
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                IList<float> epochErrorsTrainTmp = [];

                // Iterate over the training data in batches
                for (int i = 0; i < trainData.Rows.Count; i += batchSize ?? 1)
                {
                    // Prepare mini-batches for both input (X) and target (Y) data
                    IList<float> xBatchRaw = [];
                    IList<float> yBatchRaw = [];
                    bool isRemainingDataToSmallForBatch = false;

                    // Collect the batch of data
                    for (int j = i; j < i + (batchSize ?? 1); j++)
                    {
                        // Check if there's enough data for the batch
                        if (j == testData.Rows.Count)
                        {
                            isRemainingDataToSmallForBatch = true;
                            break;
                        }

                        // Concatenate data for the current batch
                        xBatchRaw = [.. xBatchRaw, .. trainData.Rows[j].X];
                        yBatchRaw = [.. yBatchRaw, .. trainData.Rows[j].Y];
                    }

                    // Exit the loop if there is not enough data for a complete batch
                    if (isRemainingDataToSmallForBatch)
                        break;

                    // Convert raw data into tensors for input and target batches
                    Tensor xBatch = new(new([batchSize ?? 1, trainData.Rows[i].X.Count]), xBatchRaw);
                    Tensor yBatchTarget = new(new([batchSize ?? 1, trainData.Rows[i].Y.Count]), yBatchRaw);

                    Tensor[] gradients;
                    using (GradientTape tape = new())
                    {
                        // Perform the forward pass: predict outputs for the batch
                        Tensor yBatchPredicted = ForwardPass([xBatch], tape)[0];

                        // Calculate the error (loss) between the predicted and actual targets
                        Tensor error = lossFunction switch
                        {
                            LossFunctionType.MSE => MathT.Mse(yBatchPredicted, yBatchTarget, tape),
                            //LossFunctionType.MAE => MathT.Mae(yBatchPredicted, yBatchTarget),
                            _ => throw new NotImplementedException(),
                        };

                        gradients = tape.GetGradients(error, [.. trainableWeights]);

                        // Track the error for the current batch
                        epochErrorsTrainTmp.Add(error.Values.Values[0]);
                    }

                    // Debug output of the tensor computation graph for analysis
                    //Console.WriteLine(error.GetGraphCode());

                    // Perform the backward pass to propagate the error and update weights
                    BackwardPass(gradients);
                }

                epochErrorsTrain.Add(epochErrorsTrainTmp.Average());
                epochErrorsTrainTmp.Clear();

                IList<float> epochErrorsTestTmp = [];

                for (int i = 0; i < testData.Rows.Count; i += batchSize ?? 1)
                {
                    // 1. Prepare batch

                    IList<float> xBatchRaw = [];
                    IList<float> yBatchRaw = [];
                    bool isRemainingDataToSmallForBatch = false;

                    for (int j = i; j < i + (batchSize ?? 1); j++)
                    {
                        if (j == testData.Rows.Count)
                        {
                            isRemainingDataToSmallForBatch = true;
                            break;
                        }

                        xBatchRaw = [.. xBatchRaw, .. testData.Rows[j].X];
                        yBatchRaw = [.. yBatchRaw, .. testData.Rows[j].Y];
                    }

                    if (isRemainingDataToSmallForBatch)
                        break;

                    Tensor xBatch = new(new([batchSize ?? 1, testData.Rows[i].X.Count]), xBatchRaw);
                    Tensor yBatchTarget = new(new([batchSize ?? 1, testData.Rows[i].Y.Count]), yBatchRaw);

                    // 2. Forward pass

                    Tensor yBatchPredicted = ForwardPass([xBatch])[0];

                    // 3. Calculate error

                    Tensor error = lossFunction switch
                    {
                        LossFunctionType.MSE => MathT.Mse(yBatchPredicted, yBatchTarget),
                        //LossFunctionType.MAE => MathT.Mae(yBatchPredicted, yBatchTarget),
                        _ => throw new NotImplementedException(),
                    };

                    epochErrorsTestTmp.Add(error.Values.Values[0]);
                }

                epochErrorsTest.Add(epochErrorsTestTmp.Average());
                epochErrorsTestTmp.Clear();

                Console.WriteLine($"Epoch {epoch + 1}/{epochs} - Training loss: {epochErrorsTrain[^1]} Test loss: {epochErrorsTest[^1]}");
            }

            Console.WriteLine("Training done");
        }

        /// <summary>
        /// Performs a backward pass to optimize the model weights based on the computed gradients.
        /// </summary>
        /// <param name="gradients">An array of tensors representing the gradients of the loss function.</param>
        private void BackwardPass(Tensor[] gradients)
        {
            if (optimizer == null)
                throw new ArgumentException("Model optimizer is not set, model is probably not compiled");

            optimizer.Optimize([.. trainableWeights], gradients);
        }

        /// <summary>
        /// Executes the forward pass through the model, producing predictions based on the input tensors.
        /// </summary>
        /// <param name="x">A list of input tensors.</param>
        /// <param name="tape">An optional <see cref="GradientTape"/> for automatic differentiation.</param>
        /// <returns>A list of tensors representing the outputs from the model's output layers.</returns>
        public IList<Tensor> ForwardPass(IList<Tensor> x, GradientTape? tape = null)
        {
            // 1. Provide inputs
            IList<Layer> nextLayers = [];
            for (int i = 0; i < inputs.Count; i++)
            {
                Input input = inputs[i];
                input.Input = x[i];
                input.ForwardPass(tape);
                nextLayers = [.. nextLayers, .. input.LayersNext];
            }

            // 2. Process hidden and output layers

            while (nextLayers.Count > 0)
            {
                IList<Layer> nextLayersTmp = [];

                foreach (Layer layer in nextLayers)
                {
                    layer.ForwardPass(tape);
                    nextLayersTmp = [.. nextLayersTmp, .. layer.LayersNext];
                }

                nextLayers = nextLayersTmp;
            }

            // 3. Collect activations from output layers

            IList<Tensor> outputLayersActivations = [];

            foreach (Layer layer in outputs)
            {
                outputLayersActivations.Add(layer.Output);
            }

            return outputLayersActivations;
        }
    }
}
