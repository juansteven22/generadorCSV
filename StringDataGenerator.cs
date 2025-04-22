using System;

namespace CSVGeneratorSOLID
{
    public class StringDataGenerator : IDataGenerator
    {
        private Random _random;
        private string _baseString;

        public StringDataGenerator(string baseString = "Nombre")
        {
            _random = new Random();
            _baseString = baseString;
        }

        public string GenerateValue(bool allowRepetition, int index)
        {
            if (allowRepetition)
            {
                // Generar algún número aleatorio para concatenar al baseString
                int randomNum = _random.Next(1, 100000);
                return $"{_baseString}{randomNum}";
            }
            else
            {
                // Secuencia sin repetición (Nombre1, Nombre2, ...)
                return $"{_baseString}{index + 1}";
            }
        }
    }
}
