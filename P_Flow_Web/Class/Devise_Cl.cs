using Npgsql;

namespace P_Flow_Web.Class;

public class Devise_Cl
{
    public string Code { get; set; }
    public string Designation { get; set; }
    private Dictionary<string, string> _pairCodeDesignation { get; set; }

    public bool CreateDevise()
    {
        var isTrue = false;
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("insert into pf.devise values(@code,@designation)", cnx);
            cm.Parameters.AddWithValue("@code", Code);
            cm.Parameters.AddWithValue("@designation", Designation);

            var i = cm.ExecuteNonQuery();
            if (i != 0)
                isTrue = true;
            else
                isTrue = false;
        }

        return isTrue;
    }

    public Dictionary<string, string> GetCodeAndDesignation()
    {
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("select * from pf.devise", cnx);
            var reader = cm.ExecuteReader();
            _pairCodeDesignation = new Dictionary<string, string>();
            while (reader.Read())
            {
                _pairCodeDesignation.Add(reader.GetString(0),reader.GetString(1));
            }

            return _pairCodeDesignation;
        }
    }
}