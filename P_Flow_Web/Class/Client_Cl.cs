using Npgsql;

namespace P_Flow_Web.Class
{
    public class Client_Cl
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public string PieceId { get; set; }
        public string RefPiece { get; set; }
        public string NumeroPhone { get; set; }


        public bool AddClient()
        {
            var isTrue = false;
            using(var cnx =  new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("insert into pf.client(nom,piece_id,ref_piece,numero_phone)" +
                    " values(@nom,@piece,@ref,@phone)", cnx);
                cm.Parameters.AddWithValue("@nom", this.Nom);
                cm.Parameters.AddWithValue("@piece", this.PieceId);
                cm.Parameters.AddWithValue("@ref", this.RefPiece);
                cm.Parameters.AddWithValue("@phone", this.NumeroPhone);
                var i = cm.ExecuteNonQuery();

                if (i != 0) isTrue = true;
                else isTrue = false;
            }

            return isTrue;
        }
        public long GetIdClient()
        {
            long IdClient = 0;
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("SELECT max(id) FROM pf.client c WHERE c.numero_phone=@phone", cnx);
                cm.Parameters.AddWithValue("@phone", NumeroPhone);
                var reader = cm.ExecuteReader();
                if (reader.Read())
                    IdClient = reader.GetInt64(0);
            }

            return IdClient;
        }
        
        public bool DeleteClient()
        {
            var isTrue = false;

            using(var cnx=new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("delete from pf.client where id=(select max(id) from pf.client where numero_phone=@phone)", cnx);
                cm.Parameters.AddWithValue("@phone", NumeroPhone);
                var i = cm.ExecuteNonQuery();
                if (i != 0) isTrue = true;
                else isTrue = false;
            }

            return isTrue;
        }
    }
}
