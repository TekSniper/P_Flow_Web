using Npgsql;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P_Flow_Web.Class
{
    public class Utilisateur_Cl
    {
        public int Id { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public char Sexe { get; set;}
        public string Login { get; set;}
        public string Motdepasse { get; set; }
        public DateTime DateConnexion { get; set; }
        public DateTime HeureConnexion { get; set; }
        public string IpHost { get; set; }
        public string NomHost { get; set; }
        public string TypeUser { get; set; }


        public bool ExistsOneOrMorUsers()
        {
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select count(*) from pf.utilisateur", cnx);
                var reader = cm.ExecuteReader();
                var check = false;
                if(reader.Read())
                {
                    if (reader.GetInt32(0) == 0)
                        check = false;
                    else
                        check = true;
                }

                return check;
            }
        }
    }
}
