namespace Myras.Enums
{
    /// <summary>
    /// Represents the various types of tensor operations that can be performed.
    /// </summary>
    public enum TensorOperationType
    {
        /// <summary>
        /// Represents the input operation, where data is fed into the tensor.
        /// </summary>
        INPUT,

        /// <summary>
        /// Represents the output operation, where data is retrieved from the tensor.
        /// </summary>
        OUTPUT,

        /// <summary>
        /// Represents the addition operation performed on tensors.
        /// </summary>
        ADDITION,

        /// <summary>
        /// Represents the subtraction operation performed on tensors.
        /// </summary>
        SUBTRACTION,

        /// <summary>
        /// Represents the multiplication operation performed on tensors.
        /// </summary>
        MULTIPLICATION,

        /// <summary>
        /// Represents the division operation performed on tensors.
        /// </summary>
        DIVISION,

        /// <summary>
        /// Represents the square root operation performed on tensors.
        /// </summary>
        SQUARE_ROOT,

        /// <summary>
        /// Represents the dot product operation between tensors.
        /// </summary>
        DOT_PRODUCT,

        /// <summary>
        /// Represents the transpose operation, which flips a tensor's dimensions.
        /// </summary>
        TRANSPOSE,

        /// <summary>
        /// Represents the Mean Squared Error (MSE) loss function.
        /// </summary>
        LOSS_FUNCTION_MSE,

        /// <summary>
        /// Represents the Rectified Linear Unit (ReLU) activation function.
        /// </summary>
        ACTIVATION_FUNCTION_RELU,

        /// <summary>
        /// Represents the linear activation function.
        /// </summary>
        ACTIVATION_FUNCTION_LINEAR,
    }
}
