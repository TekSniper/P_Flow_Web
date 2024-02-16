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
        public List<Menu_Cl> MenuList { get; set; }
        
        public void OnGet()
        {
            Login = HttpContext.Session.GetString("Login")!;
            Type = HttpContext.Session.GetString("Type")!;
            if (Login == string.Empty || Type == string.Empty)
                Response.Redirect("/");
            else
            {
                //var getChildMenu = menu.GetChildMenu();
            }
        }
        public List<Menu_Cl> GetAdminParentMenu()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.menu where parent is null", cnx);
                var reader = cm.ExecuteReader();
                MenuList = new List<Menu_Cl>();
                while (reader.Read())
                {
                    Menu_Cl menu = new Menu_Cl();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    menu.Icon = reader.GetString(3);
                    menu.Url = reader.GetString(4);

                    MenuList.Add(menu);
                }
            }

            return MenuList;
        }
        public List<Menu_Cl> GetAdminChildMenu(string Code)
        {
            using(var cnx = new dbConnection().GetConnection()) { 
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.menu where parent is not null and parent=@parent", cnx);
                cm.Parameters.AddWithValue("@parent", Code);
                MenuList = new List<Menu_Cl>();
                var reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    Menu_Cl menu = new Menu_Cl();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    menu.Parent = reader.GetString(2);
                    menu.Icon = reader.GetString(3);
                    menu.Url = reader.GetString(4);

                    MenuList.Add(menu);
                }
            }
            return MenuList;
        }
    }
}
