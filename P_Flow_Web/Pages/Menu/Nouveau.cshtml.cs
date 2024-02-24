using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Menu
{
    public class NouveauModel : PageModel
    {
        public string Login { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public Menu_Cl _menu = new Menu_Cl();
        public List<Menu_Cl> ParentMenu { get; set; }
        public List<Menu_Cl> ChildMenu { get; set; }
        public List<Menu_Cl> ParentMenuForm { get; set; }
        public void OnGet()
        {
            Login = HttpContext.Session.GetString("Login")!;
            Type = HttpContext.Session.GetString("Type")!;
            if(Login == null)
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
                Login = HttpContext.Session.GetString("Login")!;
            Type = HttpContext.Session.GetString("Type")!;
            var question = Request.Form["question"].ToString();
            switch (question)
            {
                case "parent":
                    {
                        _menu.Code = Request.Form["tbCode"].ToString();
                        _menu.Designation = Request.Form["designation"].ToString();
                        _menu.Icon = Request.Form["icon"].ToString();
                        _menu.Url = Request.Form["url"].ToString();
                        if(_menu.Code == "" || _menu.Designation == "")
                        {
                            WarningMessage = "Le code, la designation et le lien du menu sont obligatoires.";
                            return;
                        }
                        else
                        {
                            var isCreated = _menu.CreateParentMenu();
                            if (isCreated)
                            {
                                SuccessMessage = "Le menu est cr�� avec succ�s.";
                                _menu.Code = "";
                                _menu.Designation = "";
                                _menu.Icon = "";
                                _menu.Url = "";
                            }
                            else
                            {
                                ErrorMessage = "Echec de cr�ation du menu.";
                                return;
                            }
                        }
                    }
                    break;
                case "sous-menu":
                    {
                        _menu.Code = Request.Form["tbCode"].ToString();
                        _menu.Designation = Request.Form["designation"].ToString();
                        _menu.Parent = Request.Form["parent"].ToString();
                        _menu.Icon = Request.Form["icon"].ToString();
                        _menu.Url = Request.Form["url"].ToString();

                        var isCreated = _menu.CreateChildMenu();
                        if (isCreated)
                        {
                            SuccessMessage = "Le menu est cr�� avec succ�s.";
                            _menu.Code = string.Empty;
                            _menu.Designation = string.Empty;
                            _menu.Icon = string.Empty;
                            _menu.Url = string.Empty;                            
                        }
                        else
                        {
                            ErrorMessage = "Echec de cr�ation du menu.";
                            return;
                        }
                    }
                    break;
            } 
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
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
}
