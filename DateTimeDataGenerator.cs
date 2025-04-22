using System;

namespace CSVGeneratorSOLID
{
    public class DateTimeDataGenerator : IDataGenerator
    {
        private Random _random;
        private DateTime _startDate;

        public DateTimeDataGenerator()
        {
            _random = new Random();
            // Definimos una fecha inicial arbitraria, p.ej. 1/1/2000
            _startDate = new DateTime(2000, 1, 1);
        }

        public string GenerateValue(bool allowRepetition, int index)
        {
            if (allowRepetition)
            {
                // Generar una fecha aleatoria entre 2000-01-01 y 2050-12-31 (por ejemplo)
                int range = (new DateTime(2050, 12, 31) - _startDate).Days;
                return _startDate.AddDays(_random.Next(range)).ToString("yyyy-MM-dd");
            }
            else
            {
                // Secuencia de fechas: iniciar en 2000-01-01 y avanzar un día por índice
                return _startDate.AddDays(index).ToString("yyyy-MM-dd");
            }
        }
    }
}
