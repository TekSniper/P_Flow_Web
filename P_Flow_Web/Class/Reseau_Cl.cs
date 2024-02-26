using Npgsql;

namespace P_Flow_Web.Class;

public class Reseau_Cl
{
    public string Code { get; set; }
    public string Designation { get; set; }
    
    private Dictionary<string,string> reseaux { get; set; }

    public Dictionary<string, string> GetKeyValueReseau()
    {
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("select * from pf.reseau", cnx);
            reseaux = new Dictionary<string, string>();
            var reader = cm.ExecuteReader();
            while (reader.Read())
            {
                reseaux.Add(reader.GetString(0),reader.GetString(1));
            }

            return reseaux;
        }
    }
}