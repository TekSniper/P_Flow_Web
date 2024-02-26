using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Compte;

public class Types : PageModel
{
    public string Login { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; 
    public Menu_Cl _menu = new Menu_Cl();
    public List<Menu_Cl> ParentMenu { get; set; }
    public List<Menu_Cl> ChildMenu { get; set; }
    public List<Menu_Cl> ParentMenuForm { get; set; }
    public List<TypeCompte_Cl> types { get; set; }
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
                var cm = new NpgsqlCommand("select * from pf.type_compte", cnx);
                var reader = cm.ExecuteReader();
                types = new List<TypeCompte_Cl>();
                while (reader.Read())
                {
                    TypeCompte_Cl type = new TypeCompte_Cl();
                    type.Id = reader.GetInt32(0);
                    type.Designation = reader.GetString(1);
                    type.DateCreation = reader.GetDateTime(2);
                    type.IdUser = reader.GetInt32(3);
                    
                    types.Add(type);
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
            using(var cnx = new dbConnection().GetConnection())
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