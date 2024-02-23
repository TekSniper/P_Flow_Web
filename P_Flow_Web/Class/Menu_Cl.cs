using Npgsql;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P_Flow_Web.Class
{
    public class Menu_Cl
    {
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Parent { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public Dictionary<string, string> parentMenus1 { get; set; }
        public List<string> childMenus1 { get; set; }
        public List<Menu_Cl> parentMenus { get; set; }
        public List<Menu_Cl> childMenus { get; set; }


        //Recupération de la paire clé/valeur des menus
        public List<Menu_Cl> GetParentMenus(string Login)
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("GetParentMenu", cnx);
                cm.Parameters.AddWithValue("@login", Login);
                parentMenus = new List<Menu_Cl>();
                var reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    var menu = new Menu_Cl();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    if (reader.GetString(2) == null)
                        menu.Parent = "";
                    if (reader.GetString(3) == null)
                        menu.Url = "";
                    menu.Icon = reader.GetString(4);

                    parentMenus.Add(menu);
                }

                return parentMenus;
            }
        }
        public List<Menu_Cl> GetChildMenu(string Login, string Code)
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("GetMenuTabByParent", cnx);
                cm.CommandType = System.Data.CommandType.StoredProcedure;

                cm.Parameters.AddWithValue("@login", Login);
                cm.Parameters.AddWithValue("@code", Code);
                childMenus = new List<Menu_Cl>();
                var reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    var menu = new Menu_Cl();
                    menu.Code = reader.GetString(0);
                    menu.Designation = reader.GetString(1);
                    menu.Parent = reader.GetString(2);
                    menu.Url = reader.GetString(3);
                    menu.Icon = reader.GetString(4);
                    childMenus.Add(menu);
                }


                return childMenus;
            }
        }
        public Dictionary<string, string> GetParentMenus1()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("GetParentMenu", cnx);
                cm.CommandType = System.Data.CommandType.StoredProcedure;
                parentMenus1 = new Dictionary<string, string>();
                var reader = cm.ExecuteReader();
                while (reader.Read())
                    parentMenus1.Add(reader.GetString(0), reader.GetString(1));

                return parentMenus1;
            }
        }
        public List<string> GetChildMenu1(string Code)
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("GetMenuTabByParent", cnx);
                cm.CommandType = System.Data.CommandType.StoredProcedure;
                //cm.Parameters.Add(new SqlParameter("@code", Code));
                childMenus1 = new List<string>();
                var reader = cm.ExecuteReader();
                while (reader.Read())
                    childMenus1.Add(reader.GetString(1));


                return childMenus1;
            }
        }
        
        public bool CreateParentMenu()
        {
            using(var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("insert into pf.menu(code,designation,icon,url) values(@code,@designation,@icon,@url)", cnx);
                cm.Parameters.AddWithValue("@code", this.Code);
                cm.Parameters.AddWithValue("@designation", this.Designation);
                cm.Parameters.AddWithValue("@icon", this.Icon);
                cm.Parameters.AddWithValue("@url", this.Url);

                var i = cm.ExecuteNonQuery();
                if (i != 0)
                    return true;
                else
                    return false;
            }
        }
        public bool CreateChildMenu()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("insert into pf.menu(code,designation,parent,icon,url) values" +
                    " (@code,@designation,@parent,@icon,@url)", cnx);
                cm.Parameters.AddWithValue("@code", this.Code);
                cm.Parameters.AddWithValue("@designation", this.Designation);
                cm.Parameters.AddWithValue("@parent", this.Parent);
                cm.Parameters.AddWithValue("@icon", this.Icon);
                cm.Parameters.AddWithValue("@url", this.Url);

                var i = cm.ExecuteNonQuery();
                if (i != 0)
                    return true;
                else
                    return false;
            }
        }
    }
}
