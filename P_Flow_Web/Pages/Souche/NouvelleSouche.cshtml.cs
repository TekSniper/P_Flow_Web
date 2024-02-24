using Microsoft.AspNetCore.Mvc.RazorPages;
using P_Flow_Web.Class;
using Npgsql;

namespace P_Flow_Web.Pages.Souche;

public class NouvelleSouche : PageModel
{
    public Souche_Cl souche = new Souche_Cl();
    public string Login { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string WarningMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;
    public List<Menu_Cl> ParentMenu { get; set; }
    public List<Menu_Cl> ChildMenu { get; set; }
    public List<Menu_Cl> ParentMenuForm { get; set; }
    public void OnGet()
    {
        Login = HttpContext.Session.GetString("Login")!;
        Type = HttpContext.Session.GetString("Type")!;
        if(Login == null)
            Response.Redirect("/");
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
            souche.Designation = Request.Form["tbDesignation"].ToString();
            souche.Prefixe = Request.Form["tbPrefixe"].ToString();
            souche.NumSeq = 0;
            if (souche.Designation == "" || souche.Prefixe == "")
            {
                WarningMessage = "Remplissez les vides svp !";
                return;
            }
            else
            {
                var isCreated = souche.CreateSouche();
                switch (isCreated)
                {
                    case true:
                    {
                        SuccessMessage = "La souche " + souche.Designation + " est créée avec succès.";
                        souche.Designation = string.Empty;
                        souche.Prefixe = string.Empty;
                        souche.NumSeq = 0;
                    }
                        break;
                    case false:
                    {
                        ErrorMessage = "Echec de création de la souche " + souche.Designation;
                    }
                        break;
                }
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