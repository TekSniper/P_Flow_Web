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
        //public Compte_Cl compte = new Compte_Cl();
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
                using (var cnx = new dbConnection().GetConnection())
                {
                    cnx.Open();
                    var cm = new NpgsqlCommand(
                        "select numero_compte,designation,intitule,numero_phone,solde,devise,ct.id_user " +
                        "from pf.compte ct, pf.type_compte tc where ct.id_type=tc.id", cnx);
                    var reader = cm.ExecuteReader();
                    comptes = new List<Compte_Cl>();

                    while (reader.Read())
                    {
                        Compte_Cl compte = new Compte_Cl();
                        compte.NumeroCompte = reader.GetString(0);
                        compte.DesignationType = reader.GetString(1);
                        compte.Intitule = reader.GetString(2);
                        compte.NumeroPhone = reader.GetString(3);
                        compte.Solde = reader.GetDecimal(4);
                        compte.Devise = reader.GetString(5);
                        compte.UserLogin = new Utilisateur_Cl().GetLoginUser(reader.GetInt32(6));
                        
                        comptes.Add(compte);
                    }
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
