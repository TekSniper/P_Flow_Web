using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages
{
    public class CreateAdminModel : PageModel
    {
        public Utilisateur_Cl utilisateur = new Utilisateur_Cl();
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public void OnGet()
        {
        }
        public void OnPost()
        {
            utilisateur.Prenom = Request.Form["tbPrenom"].ToString();
            utilisateur.Nom = Request.Form["tbNom"].ToString();
            utilisateur.Sexe = Convert.ToChar(Request.Form["sexe"].ToString());
            utilisateur.TypeUser= "Admin";
            utilisateur.Login = Request.Form["tbLogin"].ToString();
            utilisateur.Motdepasse = Request.Form["tbPwd"].ToString();
            var confirm = Request.Form["tbConfirm"].ToString();

            //SuccessMessage = sexe.ToString();
            if (utilisateur.Prenom.Length <= 0 || utilisateur.Nom.Length <= 0 || utilisateur.Sexe.ToString().Length <= 0 || utilisateur.Login.Length <= 0 || utilisateur.Motdepasse.Length <= 0)
            {
                WarningMessage = "Vous devez remplir les vides !";
            }
            else
            {
                if (confirm != utilisateur.Motdepasse)
                    ConfirmPassword = "Les mots de passe ne sont pas identiques !\nVeuiilez confirmer le mot de passe que vous avez saisi.";
                else
                {
                    var existsUser = utilisateur.ExistsUser();
                    if (existsUser)
                        WarningMessage = "Il existe déjà un compte utilisateur avec cet identifiant.";
                    else
                    {
                        var isCreated = utilisateur.CreateUser();
                        if (isCreated)
                            Response.Redirect("/");
                        else
                            ErrorMessage = "La création du compte a échoué";
                    }
                }
            }
        }
    }
}
