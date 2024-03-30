using Npgsql;

namespace P_Flow_Web.Class
{
    public class TypeMouvement_Cl
    {
        public int Id { get; set; }
        public string Designation { get; set; }
        public DateTime DateCreation { get; set; }
        public string Sens { get; set; }
        public decimal p_comm { get; set; }
        public int IdUser { get; set; }
        private Dictionary<int, string> IdDesignation { get; set; }


        public Dictionary<int, string> GetIdDesignation()
        {
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.type_mouvement", cnx);
                var reader = cm.ExecuteReader();
                IdDesignation = new Dictionary<int, string>();
                while (reader.Read())
                {
                    IdDesignation.Add(reader.GetInt32(0), reader.GetString(1));
                }

                return IdDesignation;
            }
        }
        public decimal GetPercentCom()
        {
            var percent = 0.00M;

            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("SELECT * from pf.type_mouvement tm WHERE tm.id=@id", cnx);
                cm.Parameters.AddWithValue("@id", Id);
                var reader = cm.ExecuteReader();
                if (reader.Read())
                    percent = reader.GetDecimal(5);
            }

            return percent;
        }
        public string GetLogic()
        {
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("SELECT tm.sens from pf.type_mouvement tm WHERE tm.id=@id", cnx);
                cm.Parameters.AddWithValue("@id", Id);
                var reader = cm.ExecuteReader();
                if (reader.Read())
                    Sens = reader.GetString(0);

                return Sens;
            }
        }
    }
}
