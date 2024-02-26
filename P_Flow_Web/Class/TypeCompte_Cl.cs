using System.Runtime.InteropServices.JavaScript;
using Npgsql;

namespace P_Flow_Web.Class
{
    public class TypeCompte_Cl
    {
        public int Id { get; set; }
        public string Designation { get; set; }
        public DateTime DateCreation { get; set; }
        public int IdUser { get; set; }
        private Dictionary<int, string> types_key_value { get; set; }

        public bool CreateAccountType()
        {
            var isTrue = false;
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("insert into pf.type_compte(designation,date_creation,id_user) " +
                                           "values(@designation,@date,@user)", cnx);
                cm.Parameters.AddWithValue("@designation", Designation);
                cm.Parameters.AddWithValue("@date", DateCreation);
                cm.Parameters.AddWithValue("@user", IdUser);

                var i = cm.ExecuteNonQuery();
                if (i != 0) isTrue = true;
                else isTrue = false;
            }

            return isTrue;
        }

        public Dictionary<int, string> GetKeyValueAccountTypes()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.type_compte", cnx);
                types_key_value = new Dictionary<int, string>();
                var reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    types_key_value.Add(reader.GetInt32(0),reader.GetString(1));
                }

                return types_key_value;
            }
        }
    }
}
