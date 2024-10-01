using Commons.ViewModels;

namespace MnistLib.ViewModels
{
    public class Mnist : ViewModel
    {
        public float MinY
        {
            get => float.Min(_values1.Min(), _values2.Min());
        }

        public float MaxY
        {
            get => float.Max(_values1.Max(), _values2.Max());
        }

        public float MinX
        {
            get => 0;
        }

        public float MaxX
        {
            get => float.Max(_values1.Length, _values2.Length);
        }

        private float[] _values1 = [0.1f, 0.7f, 0.5f, 0.4f, 0.3f, 0.6f];
        public float[] Values1
        {
            get => _values1;
            set => PropertyHasChangedSetterHandler(this, ref _values1, value, nameof(Values1));
        }

        private float[] _values2 = [0.6f, 0.2f, 0.3f, 0.4f, 0.2f, 0.5f];
        public float[] Values2
        {
            get => _values2;
            set => PropertyHasChangedSetterHandler(this, ref _values2, value, nameof(Values2));
        }
    }
}
