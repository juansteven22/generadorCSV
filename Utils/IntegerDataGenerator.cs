using System;

namespace Utils
{
    public class IntegerDataGenerator : IDataGenerator
    {
        private readonly Random _rnd = new();
        private readonly int _min;
        private readonly int _max;

        public IntegerDataGenerator(int? min = null, int? max = null)
        {
            _min = min ?? 1;
            _max = max ?? 100_000;
            if (_max < _min) (_min, _max) = (_max, _min);
        }

        public string GenerateValue(bool allowRep, int index)
        {
            if (allowRep)
                return _rnd.Next(_min, _max + 1).ToString();

            return (_min + index).ToString();   // sin repetición
        }

        public int RangeSize() => _max - _min + 1;
    }
}
