using Npgsql;

namespace P_Flow_Web.Class;

public class Souche_Cl
{
    public int Id { get; set; }
    public string Designation { get; set; }
    public string Prefixe { get; set; }
    public int NumSeq { get; set; }

    public bool CreateSouche()
    {
        var isTrue = false;
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("insert into pf.souche(designation,prefixe,num_seq) values " +
                                       "(@designation,@prefixe,@num)", cnx);
            cm.Parameters.AddWithValue("@designation", Designation);
            cm.Parameters.AddWithValue("@prefixe", Prefixe);
            cm.Parameters.AddWithValue("@num", NumSeq);
            var i = cm.ExecuteNonQuery();
            if (i != 0) isTrue = true;
            else isTrue = false;
        }

        return isTrue;
    }

    public int GetIdSouche()
    {
        var id = 0;
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("select * from pf.souche where designation=@designation", cnx);
            cm.Parameters.AddWithValue("@designation", Designation);
            var reader = cm.ExecuteReader();

            if (reader.Read())
                id = reader.GetInt32(0);
        }

        return id;
    }
    public bool UpdateSouche()
    {
        var isTrue = false;
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("update pf.souche set (num_seq=num_seq+1) where id=@id", cnx);
            cm.Parameters.AddWithValue("@id", GetIdSouche());
            var i = cm.ExecuteNonQuery();
            if (i != 0) isTrue = true;
            else isTrue = false;
        }

        return isTrue;
    }
    public string GetSouche()
    {
        var souche = string.Empty;
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("select prefixe,num_seq+1 as IncNum from pf.souche where id=@id", cnx);
            cm.Parameters.AddWithValue("@id", Id);
            var reader = cm.ExecuteReader();
            if (reader.Read())
                souche = reader.GetString(0) + "-" + reader.GetInt32(1);
        }
        return souche;
    }
}