using System;

namespace CSVGeneratorSOLID
{
    public class BoolDataGenerator : IDataGenerator
    {
        private Random _random;

        public BoolDataGenerator()
        {
            _random = new Random();
        }

        public string GenerateValue(bool allowRepetition, int index)
        {
            if (allowRepetition)
            {
                // Valor boolean aleatorio
                return _random.Next(2) == 0 ? "false" : "true";
            }
            else
            {
                // Se podría generar alternando false, true, false, true...
                // O a partir de index % 2
                return (index % 2 == 0) ? "false" : "true";
            }
        }
    }
}
