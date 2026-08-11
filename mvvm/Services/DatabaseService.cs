using MySql.Data.MySqlClient;
using System.IO;
using System.Text.Json;

namespace WpfApp1.mvvm.Services
{
    public static class DatabaseService
    {
        private static readonly string ConnStr = LoadConnectionString();

        private static string LoadConnectionString()
        {
            string filePath = Path.Combine(
                AppContext.BaseDirectory,
                "appsettings.json"
            );

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "Le fichier appsettings.json est introuvable.",
                    filePath
                );
            }

            string json = File.ReadAllText(filePath);

            using JsonDocument document = JsonDocument.Parse(json);

            string? connectionString =
                document.RootElement
                    .GetProperty("ConnectionStrings")
                    .GetProperty("DefaultConnection")
                    .GetString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "La chaîne de connexion est vide ou invalide."
                );
            }

            return connectionString;
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnStr);
        }
    }
}