
namespace Wpf
{
    public struct RangedValue
    {
        public RangedValue(double value)
            : this(value, null, null)
        {
        }
        public RangedValue(double value, double? minimum, double? maximum)
        {
            _value = value;
            _minimum = minimum;
            _maximum = maximum;
        }

        private double _value;
        public double Value
        {
            get { return _value; }
        }

        private double? _minimum;
        public double? Minimum
        {
            get { return _minimum; }
        }

        private double? _maximum;
        public double? Maximum
        {
            get { return _maximum; }
        }
    }
}
