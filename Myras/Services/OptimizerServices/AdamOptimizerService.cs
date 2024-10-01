using Myras.Types;
using Myras.Utils;

namespace Myras.Services.OptimizerImpl
{
    /// <summary>
    /// Implementation of the Adam optimization algorithm, which combines the advantages of two other extensions of stochastic gradient descent:
    /// 1. **Momentum**: This method accelerates SGD in the relevant direction by using a moving average of past gradients, helping to smooth out updates and reduce oscillations.
    /// 2. **RMSProp**: This method adjusts the learning rate for each parameter based on the average of recent magnitudes of the gradients, preventing the learning rate from becoming too small or too large.
    /// 
    /// Adam (Adaptive Moment Estimation) integrates these techniques to provide an efficient and adaptive optimization method that adjusts the learning rate for each parameter 
    /// and maintains a moving average of both the gradients and their squares.
    /// </summary>
    internal class AdamOptimizerService : OptimizerService
    {
        /// <summary>
        /// Moving average of the gradients
        /// </summary>
        public IList<Tensor> FirstMomentumVector { get; private set; } = [];

        /// <summary>
        /// Moving average of the squared gradients
        /// </summary>
        public IList<Tensor> SecondMomentumVector { get; private set; } = [];
        public float LearningRate { get; private set; } = 0.001f;
        public float DecayRate1 { get; private set; } = 0.9f;
        public float DecayRate2 { get; private set; } = 0.999f;
        public float Epsilon { get; private set; } = 1E-7f;

        private int iteration = 0;

        private Tensor learningRateTensor = new(0);
        private Tensor decayRate1Tensor = new(0);
        private Tensor decayRate2Tensor = new(0);
        private Tensor epsilonTensor = new(0);
        private Tensor oneMinusDecayRate1 = new(0);
        private Tensor oneMinusDecayRate2 = new(0);

        public override void Initialize(Dictionary<string, dynamic> parameters)
        {
            if (parameters.TryGetValue(nameof(FirstMomentumVector), out dynamic? parameter))
                FirstMomentumVector = parameter;

            if (parameters.TryGetValue(nameof(SecondMomentumVector), out parameter))
                SecondMomentumVector = parameter;

            if (parameters.TryGetValue(nameof(LearningRate), out parameter))
                LearningRate = parameter;

            if (parameters.TryGetValue(nameof(DecayRate1), out parameter))
                DecayRate1 = parameter;

            if (parameters.TryGetValue(nameof(DecayRate2), out parameter))
                DecayRate2 = parameter;

            if (parameters.TryGetValue(nameof(Epsilon), out parameter))
                Epsilon = parameter;

            if (FirstMomentumVector.Count == 0 || SecondMomentumVector.Count == 0)
            {
                throw new ArgumentException($"{nameof(FirstMomentumVector)} and {nameof(SecondMomentumVector)} must be initialized");
            }

            learningRateTensor = new(LearningRate);
            decayRate1Tensor = new(DecayRate1);
            decayRate2Tensor = new(DecayRate2);
            epsilonTensor = new(Epsilon);
            oneMinusDecayRate1 = new(1f - DecayRate1);
            oneMinusDecayRate2 = new(1f - DecayRate2);
        }

        /// <summary>
        /// Optimizes the trainable weights of the network using the Adam optimization algorithm.
        /// 
        /// The Adam optimizer maintains two momentum vectors for each trainable parameter:
        /// 1. **First momentum vector (mean of gradients)**: Tracks the exponentially weighted average of past gradients.
        /// 2. **Second momentum vector (variance of gradients)**: Tracks the exponentially weighted average of squared gradients.
        ///
        /// At each optimization step (iteration):
        /// - The first and second momentums are updated using the current gradient.
        /// - Bias-corrected versions of these momentum vectors are computed.
        /// - The weights are then adjusted by applying the Adam update rule:
        ///   w(t+1) = w(t) - learning_rate * m_hat / (sqrt(v_hat) + epsilon).
        /// 
        /// The function ensures that the weights are updated in a stable manner by normalizing the gradients
        /// through bias correction and the second momentum estimate (adaptive learning rate).
        /// 
        /// This method throws an exception if the gradient of any trainable weight has not been calculated.
        /// </summary>
        /// <param name="trainableWeights">A list of trainable tensors (weights) for which the optimizer will compute updates.</param>
        /// <exception cref="Exception">Thrown if any of the tensors in <paramref name="trainableWeights"/> has a null gradient.</exception>
        public override void Optimize(Tensor[] trainableWeights, Tensor[] gradients)
        {
            // Increment the iteration count (time step) for the optimizer
            iteration++;

            // Compute bias-correction factors for the current iteration based on the decay rates (Beta1, Beta2)
            // These are used to correct the biased estimates of the first and second momentums.
            Tensor decay1Iter = new(1f - MathF.Pow(DecayRate1, iteration));
            Tensor decay2Iter = new(1f - MathF.Pow(DecayRate2, iteration));

            // Iterate over all trainable weights
            for (int i = 0; i < FirstMomentumVector.Count; i++)
            {
                Tensor trainableWeight = trainableWeights[i];

                if (trainableWeights.Length != gradients.Length)
                    throw new Exception("Trainable weights count does not match the gradients count");

                if (trainableWeights.Length != FirstMomentumVector.Count)
                    throw new Exception("Trainable weights count does not match the first momentum vector size");

                // Update the first momentum vector (decay rate 1 corresponds to Beta1 in Adam)
                // FirstMomentumVector[i] = Beta1 * FirstMomentumVector[i] + (1 - Beta1) * Gradient
                FirstMomentumVector[i] = MathT.Addition(MathT.Multiplication(decayRate1Tensor, FirstMomentumVector[i]), MathT.Multiplication(oneMinusDecayRate1, gradients[i]));

                // Bias correction for the first momentum vector
                // FirstMomentumBiasCorrected = FirstMomentumVector[i] / (1 - Beta1^iteration)
                Tensor firstMomentumBiasCorrected = MathT.Division(FirstMomentumVector[i], decay1Iter);

                // Update the second momentum vector (decay rate 2 corresponds to Beta2 in Adam)
                // SecondMomentumVector[i] = Beta2 * SecondMomentumVector[i] + (1 - Beta2) * (Gradient^2)
                SecondMomentumVector[i] = MathT.Addition(MathT.Multiplication(decayRate2Tensor, SecondMomentumVector[i]), MathT.Multiplication(oneMinusDecayRate2, MathT.Multiplication(gradients[i], gradients[i])));

                // Bias correction for the second momentum vector
                // SecondMomentumBiasCorrected = SecondMomentumVector[i] / (1 - Beta2^iteration)
                Tensor secondMomentumBiasCorrected = MathT.Division(SecondMomentumVector[i], decay2Iter);

                // Calculate the denominator for the Adam update: sqrt(v_hat) + epsilon
                // Adding epsilon ensures numerical stability and avoids division by zero
                Tensor denominator = MathT.Addition(MathT.Sqrt(secondMomentumBiasCorrected), epsilonTensor);

                // Compute the change in weight (delta) using the Adam update rule
                // weightDelta = learningRate * m_hat / (sqrt(v_hat) + epsilon)
                Tensor weightDelta = MathT.Division(MathT.Multiplication(learningRateTensor, firstMomentumBiasCorrected), denominator);

                // Update the trainable weight by subtracting the computed weight delta
                //trainableWeights[i].UpdateValues(MathT.Subtraction(trainableWeight, weightDelta));

                Tensor delta = MathT.Subtraction(trainableWeight, weightDelta);

                trainableWeights[i].Values.Values.Clear();
                trainableWeights[i].Values.Values.AddRange(delta.Values.Values);
            }
        }
    }
}
