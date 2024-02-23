using Npgsql;

namespace P_Flow_Web.Class
{
    public class Compte_Cl
    {
        public string NumeroCompte { get; set; }
        public int IdType { get; set; }
        public string Intitule { get; set; }
        public string NumeroPhone { get; set; }
        public decimal Solde { get; set; }
        public string Devise { get; set; }
        public int IdUser { get; set; }
        public string DesignationType { get; set; }
        public string UserLogin { get; set; }

        public bool CreateCompte()
        {
            var isTrue = false;
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand(
                    "insert into pf.compte values(@num_compte,@type,@intitule,@num_phone,@solde,@devise," +
                    "@id_user)",
                    cnx);
                cm.Parameters.AddWithValue("@num_compte", NumeroCompte);
                cm.Parameters.AddWithValue("@type", IdType);
                cm.Parameters.AddWithValue("@intitule", Intitule);
                cm.Parameters.AddWithValue("@num_phone", NumeroPhone);
                cm.Parameters.AddWithValue("@solde", Solde);
                cm.Parameters.AddWithValue("@devise", Devise);
                cm.Parameters.AddWithValue("@id_user", IdUser);
                var i = cm.ExecuteNonQuery();
                if (i != 0)
                    isTrue = true;
                else isTrue = false;
            }

            return isTrue;
        }
    }
}
