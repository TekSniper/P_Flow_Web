using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using P_Flow_Web.Class;
using Npgsql;
using ZstdSharp.Unsafe;

namespace P_Flow_Web.Pages.Mouvement
{
    public class HistoriqueModel : PageModel
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<Menu_Cl> ParentMenu { get; set; }
        public List<Menu_Cl> ChildMenu { get; set; }
        public List<Menu_Cl> ParentMenuForm { get; set; }
        public List<Mouvement_Cl> mouvement_s{get;set;}
        public void OnGet(DateTime Date_1, DateTime Date_2, string Type_Mvt)
        {
            Login = HttpContext.Session.GetString("Login")!;
            this.Type = HttpContext.Session.GetString("Type")!;
            if (Login == string.Empty)
            {
                Response.Redirect("/");
                return;
            }
            else
            {
                DateTime? date1 = null;
                DateTime? date2 = null;
                date1 = Date_1;
                date2 = Date_2;
                if (date1.HasValue && date2.HasValue)
                {
                    
                }
                else
                {
                    try
                    {
                        using (var cnx = new dbConnection().GetConnection())
                        {
                            cnx.Open();
                            var cm = new NpgsqlCommand("select numero_mvt,type_mvt,date_mvt,mv.designation,volume," +
                                                     "montant,montant_a_payer," +
                                                     "devise,frais_trs,num_compte,id_client,dv.designation " +
                                                     "from pf.mouvement mv,pf.devise dv where devise=code", cnx);
                            var reader = cm.ExecuteReader();
                            mouvement_s = new List<Mouvement_Cl>();
                            while (reader.Read())
                            {
                                var mvt = new Mouvement_Cl();
                                mvt.NumMouvement = reader.GetString(0);
                                mvt.TypeMvt = reader.GetInt32(1);
                                mvt.DateMvt = reader.GetDateTime(2);
                                mvt.Designation = reader.GetString(3);
                                mvt.Volume = reader.GetString(4);
                                mvt.Montant = reader.GetDecimal(5);
                                mvt.MontantAPayer = reader.GetDecimal(6);
                                mvt.Devise = reader.GetString(7);
                                mvt.FraisTrs = reader.GetDecimal(8);
                                mvt.NumCompte = reader.GetString(9);
                                mvt.IdClient = reader.GetInt32(10);
                                mvt.DesignationDevise = reader.GetString(11);

                                mouvement_s.Add(mvt);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        ErrorMessage = ex.Message;
                    }                    
                }
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
