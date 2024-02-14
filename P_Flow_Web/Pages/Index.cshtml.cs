using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P_Flow_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

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
