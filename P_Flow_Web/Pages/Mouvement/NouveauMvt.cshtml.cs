using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Mouvement
{
    public class NouveauMvtModel : PageModel
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public string Login { get; set; }
        public string Type { get; set; }
        public Menu_Cl _menu = new Menu_Cl();
        public List<Menu_Cl> ParentMenu { get; set; }
        public List<Menu_Cl> ChildMenu { get; set; }
        public List<Menu_Cl> ParentMenuForm { get; set; }
        public List<string> TypePieceId { get; set; }
        public Client_Cl _client = new Client_Cl();
        public Mouvement_Cl _mouvement = new Mouvement_Cl();
        public void OnGet()
        {
            Login = HttpContext.Session.GetString("Login")!;
            Type = HttpContext.Session.GetString("Type")!;
            if (Login == null)
            {
                Response.Redirect("/");
                return;
            }
            else
            {

            }
        }
        public void OnPost()
        {
            try
            {
                _client.Nom = Request.Form["tbClientName"].ToString();
                _client.PieceId = Request.Form["cbPiece"].ToString();
                _client.RefPiece = Request.Form["tbRefPiece"].ToString();
                _client.NumeroPhone = Request.Form["tbPhoneClient"].ToString();
                if (_client.Nom == "" || _client.NumeroPhone == "")
                {
                    WarningMessage = "Le nom du client et son numéro de téléphone ne peuvent pas être vides. Veuillez remplir les vides";
                    return;
                }
                else
                {
                    var isAdded = _client.AddClient();
                    if (isAdded)
                    {

                    }
                    else
                    {
                        ErrorMessage = "Echec enregistrement du client " + _client.Nom;
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
            }            
        }



        /***
         * Liste de type de piece
         */
        public List<string> GetTypePieceId()
        {
            TypePieceId = new List<string>();
            TypePieceId.Add("Identité");
            TypePieceId.Add("Carte d'électeur");
            TypePieceId.Add("Passeport");
            TypePieceId.Add("Carte de service");


            return TypePieceId;
        }
        /***
         * Recupération de la liste du ménu
         * */
        public List<Menu_Cl> GetAdminParentMenu()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.menu where parent is null", cnx);
                var reader = cm.ExecuteReader();
                ParentMenu = new List<Menu_Cl>();
                while (reader.Read())
                {
                    Menu_Cl menu = new Menu_Cl();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    menu.Icon = reader.GetString(3);
                    menu.Url = reader.GetString(4);

                    ParentMenu.Add(menu);
                }
            }

            return ParentMenu;
        }
        public List<Menu_Cl> GetAdminChildMenu(string Code)
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.menu where parent is not null and parent=@parent", cnx);
                cm.Parameters.AddWithValue("@parent", Code);
                ChildMenu = new List<Menu_Cl>();
                var reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    Menu_Cl menu = new Menu_Cl();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    menu.Parent = reader.GetString(2);
                    menu.Icon = reader.GetString(3);
                    menu.Url = reader.GetString(4);

                    ChildMenu.Add(menu);
                }
            }
            return ChildMenu;
        }
        public List<Menu_Cl> GetParentMenu()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.menu where parent is null", cnx);
                var reader = cm.ExecuteReader();
                ParentMenuForm = new List<Menu_Cl>();
                while (reader.Read())
                {
                    Menu_Cl menu = new Menu_Cl();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    menu.Icon = reader.GetString(3);
                    menu.Url = reader.GetString(4);
                    ParentMenuForm.Add(menu);
                }
            }
            return ParentMenuForm;
        }


        //Balance du compte pour la mise à jour de la balance
        public decimal GetAccountBalance(string NumCompte)
        {
            var balance = 0.00M;

            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select solde from pf.compte where numero_compte=@num", cnx);
                cm.Parameters.AddWithValue("@num", NumCompte);
                var reader = cm.ExecuteReader();
                if (reader.Read())
                    balance = reader.GetDecimal(0);
            }

            return balance;
        }
    }
}
