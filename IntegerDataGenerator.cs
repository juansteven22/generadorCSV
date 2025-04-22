using System;

namespace CSVGeneratorSOLID
{
    public class IntegerDataGenerator : IDataGenerator
    {
        private Random _random;

        public IntegerDataGenerator()
        {
            _random = new Random();
        }

        public string GenerateValue(bool allowRepetition, int index)
        {
            if (allowRepetition)
            {
                // Generar un entero aleatorio
                return _random.Next(1, 100000).ToString();
            }
            else
            {
                // Secuencia sin repetición (1, 2, 3, ...)
                // Usamos 'index + 1' para que inicie en 1
                return (index + 1).ToString();
            }
        }
    }
}
