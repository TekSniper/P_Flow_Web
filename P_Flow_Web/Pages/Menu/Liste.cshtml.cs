using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Menu;

public class Liste : PageModel
{
    public List<Menu_Cl> ParentMenu { get; set; }
    public List<Menu_Cl> ChildMenu { get; set; }
    public List<Menu_Cl> ParentMenuForm { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<Menu_Cl> menus { get; set; }
    public void OnGet(string Parent)
    {
        Login = HttpContext.Session.GetString("Login")!;
        Type = HttpContext.Session.GetString("Type")!;
        if(Login == string.Empty)
            Response.Redirect("/");
        else
        {
            if (string.IsNullOrEmpty(Parent) || string.IsNullOrWhiteSpace(Parent))
            {
                menus = new List<Menu_Cl>();
                menus = GetMenus();
            }
            else
            {
                menus = new List<Menu_Cl>();
                menus = GetMenus(Parent);
            }
        }
    }

    private List<Menu_Cl> GetMenus()
    {
        var list_menu = new List<Menu_Cl>();
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("select * from pf.menu", cnx);
            menus = new List<Menu_Cl>();
            var reader = cm.ExecuteReader();
            while (reader.Read())
            {
                Menu_Cl menu = new Menu_Cl();
                menu.Code = reader.GetString(0);
                menu.Designation = reader.GetString(1);
                if (reader.IsDBNull(2))
                    menu.Parent = "NULL";
                else
                    menu.Parent = reader.GetString(2);
                menu.Icon = reader.GetString(3);
                if (reader.IsDBNull(4))
                    menu.Url = "Pas de lien";
                else 
                    menu.Url = reader.GetString(4);

                list_menu.Add(menu);
            }
        }
        return list_menu;
    }

    private List<Menu_Cl> GetMenus(string parent)
    {
        var list_menu = new List<Menu_Cl>();
        using (var cnx = new dbConnection().GetConnection())
        {
            cnx.Open();
            var cm = new NpgsqlCommand("select * from pf.menu where parent=@parent", cnx);
            cm.Parameters.AddWithValue("@parent", parent);
            menus = new List<Menu_Cl>();
            var reader = cm.ExecuteReader();
            while (reader.Read())
            {
                Menu_Cl menu = new Menu_Cl();
                menu.Code = reader.GetString(0);
                menu.Designation = reader.GetString(1);
                if (reader.IsDBNull(2))
                    menu.Parent = "NULL";
                else
                    menu.Parent = reader.GetString(2);
                menu.Icon = reader.GetString(3);
                if (reader.IsDBNull(4))
                    menu.Url = "Pas de lien";
                else 
                    menu.Url = reader.GetString(4);

                list_menu.Add(menu);
            }
        }
        return list_menu;
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