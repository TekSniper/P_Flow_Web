using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Utilisateur_Cl utilisateur = new Utilisateur_Cl();
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty; 
        public string Login { get; set; } = string.Empty;
        public string TypeUser { get; set; } = string.Empty;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var checkEistsUsers = new Class.Utilisateur_Cl().ExistsOneOrMorUsers();
            if (checkEistsUsers)
            {

            }
            else
                Response.Redirect("/CreateAdmin");
        }
        public void OnPost()
        {
            utilisateur.Login = Request.Form["tbLogin"].ToString();
            utilisateur.Motdepasse = Request.Form["tbPwd"].ToString();

            var isExists = utilisateur.ExistsUser();
            switch (isExists)
            {
                case true:
                    {                        
                        var isAuthenticated = utilisateur.Authentication();
                        if (isAuthenticated)
                        {
                            var isActivated = utilisateur.GetStatus();
                            switch (isActivated)
                            {
                                case true:
                                    {
                                        HttpContext.Session.SetString("Login", utilisateur.Login);
                                        HttpContext.Session.SetString("Type", utilisateur.GetTypeUser());
                                        Response.Redirect("/Dash/TableauDeBord");
                                    }
                                    break;
                                case false:
                                    {
                                        WarningMessage = "Ce compte n'est pas autorisé à se connecter à l'application.\nVotre profil est désactivé. Pour plus d'infos vueillez contacter l'administrateur.\nMerci!";
                                    }
                                    break;
                            }                            
                        }
                        else
                        {
                            ErrorMessage = "Mot de passe incorrêt... Vueillez rééssayer.";
                            return;
                        }
                    }
                    break;
                case false:
                    {
                        WarningMessage = "Ce compte utilisateur n'existe pas dans le système.";
                        //return;
                    }
                    break;
            }
        }
    }
}
