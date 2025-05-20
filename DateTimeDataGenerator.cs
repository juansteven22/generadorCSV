using System;

namespace CSVGenerador
{
    public class DateTimeDataGenerator : IDataGenerator
    {
        private readonly Random _rnd = new();
        private readonly DateTime _min;
        private readonly DateTime _max;

        public DateTimeDataGenerator(DateTime? min = null, DateTime? max = null)
        {
            _min = min ?? new DateTime(2000, 1, 1);
            _max = max ?? new DateTime(2050, 12, 31);
            if (_max < _min) (_min, _max) = (_max, _min);
        }

        public string GenerateValue(bool allowRep, int index)
        {
            if (allowRep)
            {
                int days = (int)(_max - _min).TotalDays;
                return _min.AddDays(_rnd.Next(days + 1)).ToString("yyyy-MM-dd");
            }

            return _min.AddDays(index).ToString("yyyy-MM-dd");
        }

        public int RangeSizeDays() => (int)(_max - _min).TotalDays + 1;
    }
}
