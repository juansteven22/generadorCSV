namespace CSVGenerador
{
    public interface IDataGenerator
    {

        /// Genera un valor de la columna específica, dado si se permiten repeticiones o no,
        /// y el índice actual, con la finalidad de generar datos secuenciales si es sin repetición.

        /// allowRepetitionIndica si se permiten repeticiones.
        /// index Índice del registro a generar (para datos secuenciales).
        /// El valor generado como string
        string GenerateValue(bool allowRepetition, int index);
    }
}
