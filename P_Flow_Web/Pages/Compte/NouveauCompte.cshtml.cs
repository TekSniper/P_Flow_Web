using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P_Flow_Web.Class;
using Npgsql;

namespace P_Flow_Web.Pages.Compte
{
    public class NouveauCompteModel : PageModel
    {
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Souche_Cl souche = new Souche_Cl();
        public Compte_Cl compte = new Compte_Cl();
        private List<TypeCompte_Cl> _typeComptes { get; set; }
        public Menu_Cl _menu = new Menu_Cl();
        public List<Menu_Cl> ParentMenu { get; set; }
        public List<Menu_Cl> ChildMenu { get; set; }
        public List<Menu_Cl> ParentMenuForm { get; set; }
        public List<Compte_Cl> comptes { get; set; }
        public Dictionary<string, string> CodeDesignationDevise = new Dictionary<string, string>();
        public void OnGet()
        {
            Login = HttpContext.Session.GetString("Login");
            Type = HttpContext.Session.GetString("Type");
            if (Login == null)
            {
                Response.Redirect("/");
                return;
            }
            else
            {
                CodeDesignationDevise = new Devise_Cl().GetCodeAndDesignation();
            }
        }

        public void OnPost()
        {
            try
            {
                Login = HttpContext.Session.GetString("Login")!;
                int IdUser = new Utilisateur_Cl().GetIdUser(Login);
                souche.Designation = "Compte";
                compte.NumeroCompte = souche.GetSouche();
                compte.IdType = int.Parse(Request.Form["type"].ToString());
                compte.Intitule = Request.Form["intitule"].ToString();
                compte.NumeroPhone = Request.Form["phone"].ToString();
                compte.Solde = decimal.Parse(Request.Form["solde"].ToString());
                compte.Devise = Request.Form["devise"].ToString();
                compte.IdUser = IdUser;
                if (compte.NumeroCompte == "" || compte.IdType == 0 || compte.Intitule == "" ||
                    compte.NumeroPhone == "" ||
                    compte.Solde == 0.00M || compte.Devise == "" || IdUser == 0)
                {
                    WarningMessage = "Remplissez les vides SVP !";
                    return;
                }
                else
                {
                    var isCreated = compte.CreateCompte();
                    switch (isCreated)
                    {
                        case true:
                        {
                            SuccessMessage = "Le compte " + compte.Intitule + " est créé avec succès. Le numéro de compte" +
                                             " est " + compte.NumeroCompte;
                        }
                            break;
                        case false:
                        {
                            ErrorMessage = "Echec de création du compte " + compte.Intitule;
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

        public List<TypeCompte_Cl> GetTypeCompte()
        {
            using (var cnx = new dbConnection().GetConnection())
            {
                cnx.Open();
                var cm = new NpgsqlCommand("select * from pf.type_compte", cnx);
                var reader = cm.ExecuteReader();
                _typeComptes = new List<TypeCompte_Cl>();
                while (reader.Read())
                {
                    TypeCompte_Cl type = new TypeCompte_Cl();
                    type.Id = reader.GetInt32(0);
                    type.Designation = reader.GetString(1);
                    type.DateCreation = reader.GetDateTime(2);
                    type.IdUser = reader.GetInt32(3);
                    
                    _typeComptes.Add(type);
                }

                return _typeComptes;
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
