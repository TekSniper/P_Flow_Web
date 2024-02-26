using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Compte;

public class NouveauType : PageModel
{
    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string WarningMessage { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; 
    public int IdUser { get; set; }
    public Menu_Cl _menu = new Menu_Cl();
    public List<Menu_Cl> ParentMenu { get; set; }
    public List<Menu_Cl> ChildMenu { get; set; }
    public List<Menu_Cl> ParentMenuForm { get; set; }
    public TypeCompte_Cl typeCompte = new TypeCompte_Cl();
    
    public void OnGet()
    {
        Login = HttpContext.Session.GetString("Login")!;
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
            Login = HttpContext.Session.GetString("Login")!;
            typeCompte.Designation = Request.Form["tbDesignation"].ToString();
            typeCompte.DateCreation = Convert.ToDateTime(Request.Form["date"]);
            typeCompte.IdUser = new Utilisateur_Cl().GetIdUser(Login);
            this.IdUser = new Utilisateur_Cl().GetIdUser(Login);
            if (typeCompte.Designation == "")
            {
                WarningMessage = "Remplissez les vides svp !";
                return;
            }
            else
            {
                var isCreated = typeCompte.CreateAccountType();
                switch (isCreated)
                {
                    case true:
                    {
                        SuccessMessage = "Le type " + typeCompte.Designation + " est enrégistré avec succès.";
                        typeCompte.Designation = string.Empty;
                        typeCompte.DateCreation = DateTime.Parse(DateTime.Now.ToShortDateString());
                        
                    }
                        break;
                    case false:
                        ErrorMessage = "Erreur enrégistrement du type " + typeCompte.Designation + " !";
                        break;
                }
            }
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message+" ID User :"+this.IdUser;
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