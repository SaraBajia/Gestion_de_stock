using System;

namespace WpfApp1
{
    /// <summary>
    /// Classe statique pour stocker les informations de l'utilisateur connecté.
    /// Accessible depuis toutes les fenêtres de l'application.
    /// </summary>
    public static class Class1
    {
        public static int IdUtilisateur { get; set; }
        public static string Nom { get; set; } = "";
        public static string Prenom { get; set; } = "";
        public static string Email { get; set; } = "";
        public static string Role { get; set; } = "";

        /// <summary>Retourne "Prénom Nom" ex: Sara El Bajia</summary>
        public static string NomComplet => $"{Prenom} {Nom}".Trim();

        /// <summary>Retourne les initiales ex: "SB"</summary>
        public static string Initiales
        {
            get
            {
                string ini = "";
                if (!string.IsNullOrEmpty(Prenom)) ini += Prenom[0];
                if (!string.IsNullOrEmpty(Nom)) ini += Nom[0];
                return ini.ToUpper();
            }
        }

        /// <summary>Réinitialise la session (déconnexion)</summary>
        public static void Deconnecter()
        {
            IdUtilisateur = 0;
            Nom = "";
            Prenom = "";
            Email = "";
        }
    }
}