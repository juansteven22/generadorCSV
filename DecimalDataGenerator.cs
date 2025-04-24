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
                double d = _rnd.NextDouble();
                decimal val = _min + (decimal)d * (_max - _min);
                return Math.Round(val, 2).ToString(CultureInfo.InvariantCulture);
            }

            // sin repetición → secuencia _min, _min+1, ...
            decimal seq = _min + index;
            return seq.ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
