using MySql.Data.MySqlClient;

namespace Clínica_SePrice
{
    internal static class ConexionBaseDatos
    {
        // Configuración compartida por todas las pantallas de la aplicación.
        internal static string CadenaConexion { get; } = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            Database = "clinica_seprice",
            UserID = "root",
            Password = "MYSQL",
            CharacterSet = "utf8mb4"
        }.ConnectionString;
    }
}