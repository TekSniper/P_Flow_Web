using Npgsql;

namespace P_Flow_Web.Class
{
    public class Client_Cl
    {
        public int Id { get; set; }
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
    }
}
