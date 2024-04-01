using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using P_Flow_Web.Class;

namespace P_Flow_Web.Pages.Mouvement
{
    public class NouveauMvtModel : PageModel
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public string WarningMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public string Login { get; set; }
        public string Type { get; set; }
        public Menu_Cl _menu = new Menu_Cl();
        public List<Menu_Cl> ParentMenu { get; set; }
        public List<Menu_Cl> ChildMenu { get; set; }
        public List<Menu_Cl> ParentMenuForm { get; set; }
        public List<string> TypePieceId { get; set; }
        public Client_Cl _client = new Client_Cl();
        public Mouvement_Cl _mouvement = new Mouvement_Cl();
        public TypeMouvement_Cl type_mvt { get; set; }
        public Souche_Cl souche = new Souche_Cl();
        public Dictionary<int, string> listTypeMouvement = new Dictionary<int, string>();
        public Dictionary<string, string> listDevise = new Dictionary<string, string>();
        public Dictionary<string, string> listCompte = new Dictionary<string, string>();
        public void OnGet()
        {
            Login = HttpContext.Session.GetString("Login")!;
            Type = HttpContext.Session.GetString("Type")!;
            if (Login == null)
            {
                Response.Redirect("/");
                return;
            }
            else
            {
                type_mvt = new TypeMouvement_Cl();
                listTypeMouvement = type_mvt.GetIdDesignation();
                listDevise = new Devise_Cl().GetCodeAndDesignation();
                listCompte = new Compte_Cl().GetNumDesignation();
            }
        }
        public void OnPost()
        {
            Login = HttpContext.Session.GetString("Login")!;
            if (Request.Form["tbClientName"].ToString() == "")
                _client.Nom = "CLIENT P2LIX";
            else
                _client.Nom = Request.Form["tbClientName"].ToString();

            _client.PieceId = Request.Form["cbPiece"].ToString();
            _client.RefPiece = Request.Form["tbRefPiece"].ToString();
            _client.NumeroPhone = Request.Form["tbPhoneClient"].ToString();
            if (_client.NumeroPhone == "")
            {
                WarningMessage = "Le numéro de téléphone du client ne peut pas être vide. Veuillez remplir le vide";
                return;
            }
            else
            {
                var isAdded = _client.AddClient();
                if (isAdded)
                {
                    type_mvt = new TypeMouvement_Cl();
                    souche.Designation = "Mouvement";
                    _mouvement.NumMouvement = souche.GetSouche();
                    souche.Id = souche.GetIdSouche();
                    var IdUser = new Utilisateur_Cl().GetIdUser(Login);
                    var IdClient = _client.GetIdClient();
                    _mouvement.IdUser = IdUser;
                    _mouvement.IdClient = IdClient;
                    type_mvt.Id = int.Parse(Request.Form["cbTypeMvt"].ToString());
                    var percent = type_mvt.GetPercentCom();
                    _mouvement.DateMvt = Convert.ToDateTime(Request.Form["tbDate"].ToString());
                    _mouvement.Designation = Request.Form["tbDesignation"].ToString();
                    _mouvement.Volume = Request.Form["tbVolume"].ToString();
                    _mouvement.Montant = decimal.Parse(Request.Form["tbMontant"].ToString());
                    _mouvement.MontantAPayer = decimal.Parse(Request.Form["tbMontantP"].ToString());
                    _mouvement.FraisTrs = decimal.Parse(Request.Form["tbFrais"].ToString());
                    _mouvement.NumCompte = Request.Form["cbCompte"].ToString();
                    _mouvement.NumDestinataire = _client.NumeroPhone;
                    _mouvement.Commision = (decimal.Parse(Request.Form["tbMontant"].ToString()) * 6) / 100;

                    var _compte = new Compte_Cl();
                    _compte.NumeroCompte = _mouvement.NumCompte;
                    _mouvement.Devise = _compte.GetDevise();
                    var date = DateTime.Now.Date;
                    var convertDate = DateTime.TryParse(_mouvement.DateMvt.ToString(), out date);
                    var dec = 0.00M;
                    var convertDec_1 = decimal.TryParse(_mouvement.Montant.ToString(), out dec);
                    var convertDec_2 = decimal.TryParse(_mouvement.MontantAPayer.ToString(), out dec);
                    var convertDec_3 = decimal.TryParse(_mouvement.FraisTrs.ToString(), out dec);

                    if (_mouvement.NumMouvement == "" || _mouvement.Designation == "" || _mouvement.Volume == "" ||
                        _mouvement.Devise == "" || _mouvement.NumCompte == "" || _mouvement.NumDestinataire == "")
                    {
                        WarningMessage = "Remplissez les vides des détails de l'opération";
                        return;
                    }
                    else if (!convertDate)
                    {
                        WarningMessage = "Le format de la date n'est pas correct.";
                    }
                    else if (convertDec_1 || convertDec_2 || convertDec_3)
                    {
                        WarningMessage = "Les montants et le frais sont des valeurs numériques. Ils doivent être en format numérique";
                    }
                    else
                    {
                        var isNew = _mouvement.NouvelleOperation();
                        if (isNew)
                        {
                            SuccessMessage = "L'opération est enrégistrée avec succès.";
                            var balance = _compte.GetBalance();
                            //var type_mvt = new TypeMouvement_Cl();
                            var logic_mvt = type_mvt.GetLogic();
                            switch (logic_mvt)
                            {
                                case "Entrée":
                                    {
                                        var isIncreased = _compte.BalanceIncrease(_mouvement.Montant);
                                        if (isIncreased)
                                            SuccessMessage = SuccessMessage + "\r\nIl y a un cumul de " + _mouvement.Montant.ToString() + " sur le solde de votre compte";
                                        else
                                            ErrorMessage = "Echec Increase balance";
                                    }
                                    break;
                                case "Sortie":
                                    {
                                        var isDecreased = _compte.BalanceDecrease(_mouvement.Montant);
                                        if (isDecreased)
                                            SuccessMessage = SuccessMessage + "\r\nIl y a une diminution de " + _mouvement.Montant.ToString() + " sur le solde de votre compte";
                                    }
                                    break;
                            }
                            _mouvement.NumMouvement = string.Empty;
                            _mouvement.IdUser = 0;
                            _mouvement.IdClient = 0;
                            _mouvement.Designation = string.Empty;
                            _mouvement.Volume = string.Empty;
                            _mouvement.Montant = 0;
                            _mouvement.MontantAPayer = 0;
                            _mouvement.NumDestinataire = string.Empty;
                            _mouvement.Commision = 0;
                            _mouvement.FraisTrs = 0;

                            return;
                        }
                        else
                        {
                            ErrorMessage = "Echec d'enrégistrement de l'opération";
                            var isDeleted = _client.DeleteClient();
                            if (isDeleted)
                            {
                                ErrorMessage = ErrorMessage + "\r\nLe Client avec le numero " + _client.NumeroPhone + " est supprimé.";
                            }
                            return;
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Echec enregistrement du client " + _client.Nom;
                }
            }
            try
            {
                //Login = HttpContext.Session.GetString("Login")!;
                //if (Request.Form["tbClientName"].ToString() == "")
                //    _client.Nom = "CLIENT P2LIX";
                //else
                //    _client.Nom = Request.Form["tbClientName"].ToString();

                //_client.PieceId = Request.Form["cbPiece"].ToString();
                //_client.RefPiece = Request.Form["tbRefPiece"].ToString();
                //_client.NumeroPhone = Request.Form["tbPhoneClient"].ToString();
                //if (_client.NumeroPhone == "")
                //{
                //    WarningMessage = "Le numéro de téléphone du client ne peut pas être vide. Veuillez remplir le vide";
                //    return;                    
                //}
                //else
                //{
                //    var isAdded = _client.AddClient();
                //    if (isAdded)
                //    {
                //        type_mvt = new TypeMouvement_Cl();
                //        souche.Designation = "Mouvement";
                //        _mouvement.NumMouvement = souche.GetSouche();
                //        souche.Id = souche.GetIdSouche();
                //        var IdUser = new Utilisateur_Cl().GetIdUser(Login);
                //        var IdClient = _client.GetIdClient();
                //        _mouvement.IdUser = IdUser;
                //        _mouvement.IdClient = IdClient;
                //        type_mvt.Id = int.Parse(Request.Form["cbTypeMvt"].ToString());
                //        var percent = type_mvt.GetPercentCom();
                //        _mouvement.DateMvt = Convert.ToDateTime(Request.Form["tbDate"].ToString());
                //        _mouvement.Designation = Request.Form["tbDesignation"].ToString();
                //        _mouvement.Volume = Request.Form["tbVolume"].ToString();
                //        _mouvement.Montant = decimal.Parse(Request.Form["tbMontant"].ToString());
                //        _mouvement.MontantAPayer = decimal.Parse(Request.Form["tbMontantP"].ToString());
                //        _mouvement.FraisTrs = decimal.Parse(Request.Form["tbFrais"].ToString());
                //        _mouvement.NumCompte = Request.Form["cbCompte"].ToString();
                //        _mouvement.NumDestinataire = _client.NumeroPhone;
                //        _mouvement.Commision = (decimal.Parse(Request.Form["tbMontant"].ToString()) * 6) / 100;

                //        var _compte = new Compte_Cl();
                //        _compte.NumeroPhone = Request.Form["cbCompte"].ToString();
                //        _mouvement.Devise = _compte.GetDevise();
                //        var date = DateTime.Now.Date;
                //        var convertDate = DateTime.TryParse(_mouvement.DateMvt.ToString(), out date);
                //        var dec = 0.00M;
                //        var convertDec_1 = decimal.TryParse(_mouvement.Montant.ToString(), out dec);
                //        var convertDec_2 = decimal.TryParse(_mouvement.MontantAPayer.ToString(), out dec);
                //        var convertDec_3 = decimal.TryParse(_mouvement.FraisTrs.ToString(), out dec);

                //        if (_mouvement.NumMouvement == "" || _mouvement.Designation == "" || _mouvement.Volume == "" || 
                //            _mouvement.Devise=="" || _mouvement.NumCompte == "" || _mouvement.NumDestinataire == "")
                //        {
                //            WarningMessage = "Remplissez les vides des détails de l'opération";
                //            return;
                //        }
                //        else if (!convertDate)
                //        {
                //            WarningMessage = "Le format de la date n'est pas correct.";
                //        }
                //        else if(convertDec_1 || convertDec_2 || convertDec_3)
                //        {
                //            WarningMessage = "Les montants et le frais sont des valeurs numériques. Ils doivent être en format numérique";
                //        }
                //        else
                //        {
                //            var isNew = _mouvement.NouvelleOperation();
                //            if (isNew)
                //            {
                //                SuccessMessage = "L'opération est enrégistrée avec succès.";
                //                var balance = _compte.GetBalance();
                //                //var type_mvt = new TypeMouvement_Cl();
                //                var logic_mvt = type_mvt.GetLogic();
                //                switch (logic_mvt)
                //                {
                //                    case "Entrée":
                //                        {
                //                            var isIncreased = _compte.BalanceIncrease(_mouvement.Montant);
                //                            if (isIncreased)
                //                                SuccessMessage = SuccessMessage + "\r\nIl y a un cumul de " + _mouvement.Montant.ToString() + " sur le solde de votre compte";
                //                            else
                //                                ErrorMessage = "Echec Increase balance";
                //                        }break;
                //                    case "Sortie":
                //                        {
                //                            var isDecreased = _compte.BalanceDecrease(_mouvement.Montant);
                //                            if(isDecreased)
                //                                SuccessMessage = SuccessMessage + "\r\nIl y a une diminution de " + _mouvement.Montant.ToString() + " sur le solde de votre compte";
                //                        }
                //                        break;
                //                }
                //                _mouvement.NumMouvement = string.Empty;
                //                _mouvement.IdUser = 0;
                //                _mouvement.IdClient = 0;
                //                _mouvement.Designation = string.Empty;
                //                _mouvement.Volume = string.Empty;
                //                _mouvement.Montant = 0;
                //                _mouvement.MontantAPayer = 0;
                //                _mouvement.NumDestinataire = string.Empty;
                //                _mouvement.Commision = 0;
                //                _mouvement.FraisTrs = 0;

                //                return;
                //            }
                //            else
                //            {
                //                ErrorMessage = "Echec d'enrégistrement de l'opération";
                //                var isDeleted = _client.DeleteClient();
                //                if(isDeleted)
                //                {
                //                    ErrorMessage = ErrorMessage + "\r\nLe Client avec le numero " + _client.NumeroPhone + " est supprimé.";
                //                }
                //                return;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        ErrorMessage = "Echec enregistrement du client " + _client.Nom;
                //    }
                //}
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
            }            
        }



        /***
         * Liste de type de piece
         */
        public List<string> GetTypePieceId()
        {
            TypePieceId = new List<string>();
            TypePieceId.Add("Identité");
            TypePieceId.Add("Carte d'électeur");
            TypePieceId.Add("Passeport");
            TypePieceId.Add("Carte de service");


            return TypePieceId;
        }
        /***
         * Recupération de la liste du ménu
         * */
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
