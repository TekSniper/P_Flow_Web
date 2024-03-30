using Npgsql;

namespace P_Flow_Web.Class
{
    public class Mouvement_Cl
    {
        public string NumMouvement{get;set;}
        public int TypeMvt{get;set;}
        public DateTime DateMvt{get;set;}
        public string Designation{get;set;}
        public string NumDestinataire{get;set;}
        public string Volume{get;set;}
        public decimal Montant{get;set;}
        public decimal MontantAPayer{get;set;}
        public string Devise{get;set;}
        public decimal FraisTrs{get;set;}
        public string NumCompte{get;set;}
        public int IdUser{get;set;}
        public long IdClient{get;set;}
        public decimal Commision { get; set; }


        /***
         * 
         * Enregistrement d'un mouvement (opération)
         * 
         */
        public bool NouvelleOperation()
        {
            var isTrue = false;
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("INSERT INTO pf.mouvement VALUES "+
                             "(@numMvt,@type_mvt,@date,@designation,@numDest,@volume,@montant," +
                             "@montantAP,@devise,@frais,@numCompte,@user,@client,@commission)", cnx);
                cm.Parameters.AddWithValue("@numMvt", NumMouvement);
                cm.Parameters.AddWithValue("type_mvt", TypeMvt);
                cm.Parameters.AddWithValue("@date", DateMvt);
                cm.Parameters.AddWithValue("@designation", Designation);
                cm.Parameters.AddWithValue("@numDest", NumDestinataire);
                cm.Parameters.AddWithValue("@volume", Volume);
                cm.Parameters.AddWithValue("@montant", Montant);
                cm.Parameters.AddWithValue("@montantAP", MontantAPayer);
                cm.Parameters.AddWithValue("@frais", FraisTrs);
                cm.Parameters.AddWithValue("@numCompte", NumCompte);
                cm.Parameters.AddWithValue("@user", IdUser);
                cm.Parameters.AddWithValue("@client", IdClient);
                cm.Parameters.AddWithValue("@commission", Commision);
                cm.Parameters.AddWithValue("@devise", Devise);

                var i = cm.ExecuteNonQuery();

                if (i == 0) isTrue = false;
                else isTrue = true;
            }

            return isTrue;
        }
    }
}
