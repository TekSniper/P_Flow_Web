using System.Data;
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
        public string CodeReseau { get; set; }

        public bool CreateCompte()
        {
            var isTrue = false;
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand(
                    "insert into pf.compte values(@num_compte,@type,@intitule,@num_phone,@solde,@devise," +
                    "@id_user,@code)",
                    cnx);
                cm.Parameters.AddWithValue("@num_compte", NumeroCompte);
                cm.Parameters.AddWithValue("@type", IdType);
                cm.Parameters.AddWithValue("@intitule", Intitule);
                cm.Parameters.AddWithValue("@num_phone", NumeroPhone);
                cm.Parameters.AddWithValue("@solde", Solde);
                cm.Parameters.AddWithValue("@devise", Devise);
                cm.Parameters.AddWithValue("@id_user", IdUser);
                cm.Parameters.AddWithValue("@code", CodeReseau);
                var i = cm.ExecuteNonQuery();
                if (i != 0)
                    isTrue = true;
                else isTrue = false;
            }

            return isTrue;
        }

        public decimal GetBalance()
        {
            var balance = 0.00M;
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.compte where numero_compte=@numero", cnx);
                cm.Parameters.AddWithValue("@numero", NumeroCompte);
                var reader = cm.ExecuteReader();
                if (reader.Read())
                    balance = reader.GetDecimal(4);
            }

            return balance;
        }
        /*public DataTable GetCompteTable()
        {
            var solde = 0.00M;
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.compte where numerocompte=@numcompte", cnx);
            }

            return solde;
        }*/
        public bool BalanceIncrease(decimal Amount)
        {
            var isTrue = false;
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var increase = GetBalance() + Amount;
                var cm = new NpgsqlCommand("update pf.compte set solde=@increase where numero_compte=@numero", cnx);
                cm.Parameters.AddWithValue("@increase", increase);
                cm.Parameters.AddWithValue("@numero", NumeroCompte);
                var i = cm.ExecuteNonQuery();
                if (i != 0) isTrue = true;
                else isTrue = false;
            }


            return isTrue;
        }

        public bool BalanceDecrease(decimal Amount)
        {
            var isTrue = false;
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var decrease = GetBalance() - Amount;
                var cm = new NpgsqlCommand("update pf.compte set solde=@decrease where numero_compte=@numero", cnx);
                cm.Parameters.AddWithValue("@decrease", decrease);
                cm.Parameters.AddWithValue("@numero", NumeroCompte);
                var i = cm.ExecuteNonQuery();
                if (i != 0) isTrue = true;
                else isTrue = false;
            }

            return isTrue;
        }
    }
}
