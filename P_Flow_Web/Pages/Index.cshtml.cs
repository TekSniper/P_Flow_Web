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

        }
    }
}
