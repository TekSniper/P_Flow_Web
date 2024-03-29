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
        public int IdClient{get;set;}
        public string DesignationDevise{get;set;}


        /***
         * 
         * Enregistrement d'un mouvement (opération)
         * 
         */
        public bool NouvelleOpération()
        {
            var isTrue = false;
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("", cnx);
            }

            return isTrue;
        }
    }
}
