using System;
using System.Globalization;

namespace CSVGeneratorSOLID
{
    public class DecimalDataGenerator : IDataGenerator
    {
        private readonly Random _rnd = new();
        private readonly decimal _min;
        private readonly decimal _max;

        public DecimalDataGenerator(decimal? min = null, decimal? max = null)
        {
            _min = min ?? 0m;
            _max = max ?? 100_000m;
            if (_max < _min) (_min, _max) = (_max, _min);
        }

        public string GenerateValue(bool allowRep, int index)
        {
            if (allowRep)
            {
                decimal val = _min + (decimal)_rnd.NextDouble() * (_max - _min);
                return Math.Round(val, 2).ToString(CultureInfo.InvariantCulture);
            }

            return (_min + index).ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
