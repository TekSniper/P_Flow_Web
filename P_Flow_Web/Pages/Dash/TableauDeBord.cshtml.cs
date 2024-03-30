using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Dash
{
    public class TableauDeBordModel : PageModel
    {
        public string Login { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<Menu_Cl> ParentMenu { get; set; }
        public List<Menu_Cl> ChildMenu { get; set; }
        public List<Compte_Cl> list_comptes { get; set; }
        
        public void OnGet()
        {
            Login = HttpContext.Session.GetString("Login")!;
            Type = HttpContext.Session.GetString("Type")!;
            if (Login == null || Type == null)
                Response.Redirect("/");
            else
            {
                //var getChildMenu = menu.GetChildMenu();
                using(var cnx = new dbConnection().GetConnection())
                {
                    cnx.Open();
                    var cm = new NpgsqlCommand("SELECT * from pf.compte c WHERE c.id_type=2 ORDER BY c.intitule", cnx);
                    var reader = cm.ExecuteReader();
                    list_comptes = new List<Compte_Cl>();
                    while(reader.Read())
                    {
                        var _compte = new Compte_Cl();
                        _compte.NumeroCompte = reader.GetString(0);
                        _compte.IdType = reader.GetInt32(1);
                        _compte.Intitule = reader.GetString(2);
                        _compte.NumeroPhone = reader.GetString(3);
                        _compte.Solde = reader.GetDecimal(4);
                        _compte.Devise = reader.GetString(5);
                        _compte.IdUser = reader.GetInt32(6);
                        _compte.CodeReseau = reader.GetString(7);

                        list_comptes.Add(_compte);
                    }
                }
            }
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
            using(var cnx = new dbConnection().GetConnection()) { 
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
    }
}
