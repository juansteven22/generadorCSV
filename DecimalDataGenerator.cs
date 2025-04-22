using System;
using System.Globalization;

namespace CSVGeneratorSOLID
{
    public class DecimalDataGenerator : IDataGenerator
    {
        private Random _random;

        public DecimalDataGenerator()
        {
            _random = new Random();
        }

        public string GenerateValue(bool allowRepetition, int index)
        {
            if (allowRepetition)
            {
                // Generar un decimal aleatorio
                // Tomamos un número aleatorio y lo dividimos para crear decimales
                double randomValue = _random.NextDouble() * _random.Next(1, 100000);
                return randomValue.ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                // Generar un decimal secuencial: index + fracción .xx
                // Ej: 1.00, 2.00, 3.00... con "F2" para formatear 2 decimales
                return (index + 1).ToString("F2", CultureInfo.InvariantCulture);
            }
        }
    }
}
