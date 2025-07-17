using System;

//Код генерирует значения "true" или "false".

namespace Utils
{
    public class BoolDataGenerador : IDataGenerator
    {
        private Random _random;

        public BoolDataGenerador()
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
                // Se generando alternando false, true, false, true...
                // O a partir de index % 2
                return (index % 2 == 0) ? "false" : "true";
            }
        }
    }
}
