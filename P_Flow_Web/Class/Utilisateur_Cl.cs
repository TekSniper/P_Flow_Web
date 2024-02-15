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
        public bool ExistsUser()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.utilisateur where login=@login", cnx);
                cm.Parameters.AddWithValue("@login", this.Login);
                var reader = cm.ExecuteReader();
                if (reader.Read())
                    return true;
                else
                    return false;
            }
        }
        public bool CreateUser()
        {
            using(var cnx = new dbConnection().GetConnection()) 
            {
                cnx.Open();
                var cm = new NpgsqlCommand("insert into pf.utilisateur(prenom,nom,sexe,login,mot_de_passe,type_user)" +
                    " values(@prenom,@nom,@sexe,@login,@pwd,@type)", cnx);
                cm.Parameters.AddWithValue("@prenom", this.Prenom);
                cm.Parameters.AddWithValue("@nom", this.Nom);
                cm.Parameters.AddWithValue("@sexe", this.Sexe);
                cm.Parameters.AddWithValue("@login", this.Login);
                cm.Parameters.AddWithValue("@pwd", this.Motdepasse);
                cm.Parameters.AddWithValue("@type", this.TypeUser);

                var i = cm.ExecuteNonQuery();
                if (i != 0)
                    return true;
                else
                    return false;
            }
        }
        public bool Authentication()
        {
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.utilisateur where login=@login and mot_de_passe=@pwd", cnx);
                cm.Parameters.AddWithValue("@login", this.Login);
                cm.Parameters.AddWithValue("@pwd", this.Motdepasse);
                var reader = cm.ExecuteReader();
                var isTrue = false;

                if (reader.Read())
                    isTrue = true;
                else
                    isTrue = false;

                return isTrue;
            }
        }
    }
}
