using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Compte
{
    public class ComptesModel : PageModel
    {
        public string Login { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; 
        public Menu_Cl _menu = new Menu_Cl();
        public List<Menu_Cl> ParentMenu { get; set; }
        public List<Menu_Cl> ChildMenu { get; set; }
        public List<Menu_Cl> ParentMenuForm { get; set; }
        public List<Compte_Cl> comptes { get; set; }
        public Compte_Cl compte = new Compte_Cl();
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;
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
                try
                {
                    using (var cnx = new dbConnection().GetConnection())
                    {
                        cnx.Open();
                        var cm = new NpgsqlCommand(
                            "select * from pf.get_comptes_tab()", cnx);
                        //cm.CommandType = System.Data.CommandType.StoredProcedure;
                        //cm.CommandType = System.Data.CommandType.Text;
                        var reader = cm.ExecuteReader();
                        comptes = new List<Compte_Cl>();

                        while (reader.Read())
                        {
                            Compte_Cl _compte = new Compte_Cl();
                            _compte.NumeroCompte = reader.GetString(0);
                            _compte.DesignationType = reader.GetString(1);
                            _compte.Intitule = reader.GetString(2);
                            _compte.NumeroPhone = reader.GetString(3);
                            _compte.Solde = reader.GetDecimal(4);
                            _compte.Devise = reader.GetString(5);
                            _compte.UserLogin = new Utilisateur_Cl().GetLoginUser(reader.GetInt32(6));
                            _compte.CodeReseau = reader.GetString(7);

                            comptes.Add(_compte);
                        }                   
                    }
                }
                catch (Exception e)
                {
                    ErrorMessage = e.HResult+")"+e.Message + ". " + e.Source + ". " + e.HelpLink;
                }
            }
        }

        public void OnPost()
        {
            
        }

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
    }
}
