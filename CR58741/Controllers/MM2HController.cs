using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourlistWebAPI.ClassLib;
using TourlistWeb.Models;
using Tourlist.Common;
using TourlistWebAPI.Models;
using System.Configuration;
using TourlistDataLayer.DataModel;
using System.IO;
using TourlistDataLayer.Persistence;
using Newtonsoft.Json;

namespace TourlistWeb.Controllers
{
    public class MM2HController : TourlistController
    {
        // GET: MM2H
        TourlistWebAPI.ClassLib.MM2HHelper mM2HHelper = new TourlistWebAPI.ClassLib.MM2HHelper();
        TourlistWebAPI.ClassLib.CoreHelper coreHelper = new TourlistWebAPI.ClassLib.CoreHelper();
        TourlistWebAPI.ClassLib.CoreOrganizationHelper coreOrgHelper = new TourlistWebAPI.ClassLib.CoreOrganizationHelper();
        TourlistBusinessLayer.BLL.TobtabBLL tobtabBLL = new TourlistBusinessLayer.BLL.TobtabBLL();
        private TourlistUnitOfWork unitOfWork
        {
            get
            {
                return this.TourlistUnitOfWork;
            }
        }

        RefHelper refHelper = new RefHelper();

        RefCountryHelper refCountryHelper = new RefCountryHelper();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard(string btn, string module,string AppID,string AppNo)
        {
            if (Session["UID"] == null)
                return RedirectToAction("Index", "Account");

            string userID = Session["UID"].ToString();

            string smodule = "";
            string sstatus = "";
            string sAppID = "";
            string scode_status = "";
            List<TourlistWebAPI.Models.MM2HModels.mm2h_app> app = new List<TourlistWebAPI.Models.MM2HModels.mm2h_app>();

            app = mM2HHelper.GetMM2HStatus(userID, module);
            if (app.Count > 0)
            {
                foreach (var st in app)
                {
                    sstatus = st.Status.ToString();
                    smodule = st.Module.ToString();
                    sAppID = st.AppID.ToString();
                    scode_status = st.Code_Status;
                }
                ViewBag.NewStatus = "false";
            }
            else
            {
                ViewBag.NewStatus = "true";
            }

            List<TourlistWebAPI.Models.MM2HModels.mm2h_app> latestApp = new List<TourlistWebAPI.Models.MM2HModels.mm2h_app>();            

            latestApp = mM2HHelper.GetMM2HStatus(userID, null);

            string sstatus1 = "";
            string sModule1 = "";
            string sAppNo = "";
            if (latestApp.Count > 0)
            {
                foreach (var st in latestApp)
                {
                    sstatus1 = st.Code_Status.ToString();
                    sModule1 = st.Module.ToString();
                    if (st.AppNo !=null)
                        sAppNo=st.AppNo.ToString();
                    else
                        sAppNo = st.AppNo;
                }
            }

            if(AppNo != null)
            {
                ViewBag.Appno = AppNo;
            }
            else
            {
                ViewBag.Appno = sAppNo;
            }

            if (AppID != null)
            {
                ViewBag.AppID = AppID;
            }
            else
            {
                ViewBag.AppID = sAppID;
            }

       
            //
            List<TourlistWebAPI.Models.MM2HModels.mm2h_app> mm2hnew = new List<TourlistWebAPI.Models.MM2HModels.mm2h_app>();

            mm2hnew = mM2HHelper.GetMM2HStatus(userID, "MM2H_NEW");

            if (mm2hnew.Count > 0)
            {
                ViewBag.mm2h_new = true;
            }
            else
            {
                ViewBag.mm2h_new = false;
            }

            //

            ViewBag.LatestModule = sModule1;
            ViewBag.LatestStatus = sstatus1;
            ViewBag.Status = sstatus;
            ViewBag.Code_Status = scode_status;
            ViewBag.Mod = smodule;
           
            Session["AppID"] = sAppID;
            ViewBag.MSG_MANDATORY = "Sila masukkan data";

            List<CoreOrganizationModel.CoreOrg> clsOrg = new List<CoreOrganizationModel.CoreOrg>();

            clsOrg = coreOrgHelper.GetOrgHeader(userID);
            string NoPendaftaranSyarikat = "";
            string OrganizationID = "";
            string NamaSyarikat = "";
            foreach (var Org in clsOrg)
            {
                NoPendaftaranSyarikat = Org.NoPendaftaranSyarikat;
                OrganizationID = Org.OrganizationID;
                NamaSyarikat = Org.NamaSyarikat;
            };
            ViewBag.Company = NamaSyarikat;
            //var syarikat = coreOrgHelper.GetCompany(NoPendaftaranSyarikat);

            List<TourlistDataLayer.DataModel.core_license> clsLicense = new List<TourlistDataLayer.DataModel.core_license>();
            clsLicense = mM2HHelper.GetMM2HLicenseNo(OrganizationID);

            foreach (var item in clsLicense)
            {
                ViewBag.LicenseNo = item.license_no;
                string sdate_start = String.Format("{0:dd/MM/yyyy}", item.start_dt);
                string sdate_end = String.Format("{0:dd/MM/yyyy}", item.end_dt);
                ViewBag.DateStart = sdate_start + " - " + sdate_end;
                
                ViewBag.LicenseField = "Inbound";
            }

            if (smodule == "MM2H_NEW")
                ViewBag.DashBoardList = mM2HHelper.GetDashboardList(sAppID, "MM2H_NEW_MOHON", scode_status);
            else if (smodule == "MM2H_RENEW")
                ViewBag.DashBoardList = mM2HHelper.GetDashboardList(sAppID, "MM2H_RENEW_MOHON", scode_status);
            else if (smodule == "MM2H_ADDBRANCH")
                if (AppID == null)
                    ViewBag.DashBoardList = mM2HHelper.GetDashboardList(sAppID, "MM2H_ADDBRANCH_MOHON", scode_status);
                else
                    ViewBag.DashBoardList = mM2HHelper.GetDashboardList(AppID, "MM2H_ADDBRANCH_MOHON", scode_status);
            else if (smodule == "MM2H_CHANGESTATUS")
                ViewBag.DashBoardList = mM2HHelper.GetDashboardList(sAppID, "MM2H_CHANGESTATUS_MOHON", scode_status);
            else if (smodule == "MM2H_CANCELLICENSE")
                ViewBag.DashBoardList = mM2HHelper.GetDashboardList(sAppID, "MM2H_CANCELLICENCE_MOHON", scode_status);
            else
                ViewBag.DashBoardList = null;

            ViewBag.mm2h_renew = mM2HHelper.GetMM2HStatusModul(OrganizationID, "MM2H_RENEW");
            ViewBag.mm2h_branch = mM2HHelper.GetMM2HStatusModul(OrganizationID, "MM2H_ADDBRANCH");
            ViewBag.mm2h_new = mM2HHelper.GetMM2HStatusModul(OrganizationID, "MM2H_NEW");
            ViewBag.mm2h_changestatus = mM2HHelper.GetMM2HStatusModul(OrganizationID, "MM2H_CHANGESTATUS");


            if (ViewBag.DashBoardList != null)
            {
                string SSM = "false";
                foreach (var desc in ViewBag.DashBoardList)
                {
                    if (desc.descr_bool1 == "SSM" && desc.bool1 == "1")
                    {
                        SSM = "true";
                        break;
                    }
                    else if (desc.descr_bool1 == "SSM" && desc.bool1 == "0")
                    {
                        SSM = "false";
                        break;
                    }
                    else
                    {
                        SSM = "nothing";
                    }

                }
                ViewBag.SSM = SSM;
                int iCountItem = 0;


                foreach (var item in ViewBag.DashBoardList)
                {
                    if (item.bool1 == "0")
                    {
                        if (item.descr_bool1 == "Maklumat Cawangan" && ViewBag.mm2h_branch > 0)
                            iCountItem++;
                    }
                }
                ViewBag.Hantar = "false";
                if (iCountItem == 0)
                {
                    ViewBag.Hantar = "true";
                }

            }





            ViewBag.OrganizationID = OrganizationID;
            List<SelectListItem> NamaPengarah = new List<SelectListItem>();
            NamaPengarah = GetDirectorDropDown(OrganizationID);
            ViewData["Director_"] = NamaPengarah;

            if (sAppID != "")
            {
                List<CoreOrganizationModel.CoreAcknowledgement> clsAcknowledge = new List<CoreOrganizationModel.CoreAcknowledgement>();
                clsAcknowledge = coreOrgHelper.getAcknowledge(sAppID);
                var acknowledgement_idx = Guid.Empty;
                var acknowledge_person_ref = Guid.Empty;
                if (clsAcknowledge.Count > 0)
                {
                    foreach (var Acknowledge in clsAcknowledge)
                    {
                        acknowledgement_idx = Acknowledge.acknowledgement_idx;
                        acknowledge_person_ref = Acknowledge.acknowledge_person_ref;
                    };

                }
                ViewBag.acknowledgement_idx = acknowledgement_idx;
                ViewBag.acknowledge_person_ref = acknowledge_person_ref;
            }



            //ViewBag.Status = "new";
            if (btn == "EditPembaharuanLesen")
            {
                return RedirectToAction("SerahBatallesen", "MM2H", new
                {
                    module = module
                });
            }

            ViewBag.module = module;


            List<SelectListItem> JustifikasiBatal = new List<SelectListItem>();
            JustifikasiBatal = GetJustifikasiSerahLesenDropDown();
            ViewData["JustifikasiBatalLesen_"] = JustifikasiBatal;

            List<SelectListItem> TempohPembaharuan = new List<SelectListItem>();
            TempohPembaharuan = GetTahun();
            ViewData["TempohPembaharuan_"] = TempohPembaharuan;
            ViewBag.Message = "Berjaya!";

            ViewBag.endDatePopup = "0";
            ViewBag.renewPopup = "0";
            var coreLicense = tobtabBLL.GetCoreLicens(OrganizationID);
            if (coreLicense == null)
            {
                coreLicense = new core_license();
            }
            else
            {
                if (coreLicense.end_dt != null)
                {
                    DateTime endDate = (DateTime)coreLicense.end_dt;
                    DateTime currentDate = DateTime.Now;
                    double days = (endDate - currentDate).TotalDays;
                    if (days < 183)
                    {
                        ViewBag.endDatePopup = "1";
                    }
                    if (days > 60)
                    {
                        ViewBag.renewPopup = "1";
                    }
                }
            }
            ViewBag.coreLicense = coreLicense;



            return View();
        }
        public ActionResult PermohonanBaharu()
        {

            return View();
        }
        public ActionResult PermohonanPembaharuan()
        {
            return View();
        }
        public ActionResult SerahBatalLesen(string btn, string module)
        {
            if (btn == "Next")
            {
                return RedirectToAction("TambahCawangan", "MM2H", new
                {
                    module = module
                });
            }
            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }
            List<SelectListItem> JustifikasiBatal = new List<SelectListItem>();
            JustifikasiBatal = GetJustifikasiSerahLesenDropDown();
            ViewData["JustifikasiBatalLesen_"] = JustifikasiBatal;

            ViewBag.module = module;
            return View();
        }
        public ActionResult TambahCawangan(string btn, string module,string AppID,string AppNo)
        {

            ViewBag.itemID = Request.QueryString["item"];

            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }

            string userID = Session["UID"].ToString();

            List<TourlistWebAPI.Models.MM2HModels.mm2h_app> latestApp = new List<TourlistWebAPI.Models.MM2HModels.mm2h_app>();

            if (AppID !=null)
                latestApp = mM2HHelper.GetMM2HAddBranchStatus(AppID);

            else
                latestApp = mM2HHelper.GetMM2HStatus(userID, null);

            string sstatus1 = "";
            string appId = "";
            string appNo = "";

            
            if (latestApp.Count > 0)
            {
                foreach (var st in latestApp)
                {
                    sstatus1 = st.Code_Status.ToString();
                    appId = st.AppID;
                    appNo = st.AppNo;
                }
            }


            ViewBag.LatestStatus = sstatus1;
            ViewBag.AppID = appId;
            ViewBag.AppNo = appNo;
            ViewBag.CodeStatus = sstatus1;

            ViewBag.MSG_MANDATORY = "Sila masukkan data";
            List<CoreOrganizationModel.CoreOrg> clsOrg = new List<CoreOrganizationModel.CoreOrg>();


            clsOrg = coreOrgHelper.GetOrgHeader(userID);
            string OrganizationID = "";
            foreach (var Org in clsOrg)
            {
                OrganizationID = Org.OrganizationID;

            };


            ViewBag.OrganizationID = OrganizationID;
            ViewBag.module = module;
            return View();
        }

        public ActionResult TukarStatus(string btn, string module)
        {
            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }
            ViewBag.MSG_MANDATORY = "Sila masukkan data";
            List<SelectListItem> PemegangSahamPerson = new List<SelectListItem>();
            PemegangSahamPerson = GetStatusPemeganganSahamDropDown("Person");
            ViewData["StatusPemegangSahamPerson_"] = PemegangSahamPerson;

            List<SelectListItem> PemegangSahamOrganization = new List<SelectListItem>();
            PemegangSahamOrganization = GetStatusPemeganganSahamDropDown("Organization");
            ViewData["StatusPemegangSahamOrganization_"] = PemegangSahamOrganization;

            List<SelectListItem> PemegangSahamAll = new List<SelectListItem>();
            PemegangSahamAll = GetStatusPemeganganSahamDropDown("All");
            ViewData["StatusPemegangSahamAll_"] = PemegangSahamAll;

            List<SelectListItem> religion = new List<SelectListItem>();
            religion = GetAgamaDropDown();
            ViewData["religionlist"] = religion;

            List<SelectListItem> gender = new List<SelectListItem>();
            gender = GetJantina();
            ViewData["genderlist"] = gender;

            ViewBag.itemID = Request.QueryString["item"];

            TourlistWebAPI.ClassLib.RefCountryHelper countryHelper = new TourlistWebAPI.ClassLib.RefCountryHelper();

            List<SelectListItem> ref_malaysia = new List<SelectListItem>();
            ref_malaysia = GetMalaysiaDropDown();
            ViewData["Malaysialist_"] = ref_malaysia;

            List<SelectListItem> ref_asean = new List<SelectListItem>();
            ref_asean = GetAseanDropDown();
            ViewData["Aseanlist_"] = ref_asean;

            List<SelectListItem> ref_nonasean = new List<SelectListItem>();
            ref_nonasean = GetNonAseanDropDown();
            ViewData["NonAseanlist_"] = ref_nonasean;

            //List<SelectListItem> ref_country = new List<SelectListItem>();
            //ref_country = GetCountryDropDown();
            //ViewData["Countrylist_"] = ref_country;

            List<SelectListItem> ref_country = new List<SelectListItem>();
            ref_country = GetCountryDropDown();
            //ViewData["Countrylist_"] = ref_country;
            ViewBag.Country = ref_country;


            var Malaysia = ref_country.Where(d => d.Text.ToString() == "MALAYSIA").ToList();
            var MalaysiaVal = "";
            foreach (var item in Malaysia)
            {
                MalaysiaVal = item.Value.ToString();
            }

            ViewBag.Malaysia = MalaysiaVal;

            List<SelectListItem> JustifikasiPemegangSaham = new List<SelectListItem>();
            JustifikasiPemegangSaham = GetJustifikasiPemegangSahamDropDown();
            ViewData["JustifikasiPemegangSaham_"] = JustifikasiPemegangSaham;

            //modified by awie
            List<SelectListItem> JustifikasiPemegangSahamOrg = new List<SelectListItem>();
            JustifikasiPemegangSahamOrg = GetJustifikasiSerahLesenDropDown();
            ViewData["JustifikasiPemegangSahamOrg_"] = JustifikasiPemegangSahamOrg;
            //modified by awie          

            ViewBag.AppID = Session["AppID"].ToString();
            List<CoreOrganizationModel.CoreOrg> clsOrg = new List<CoreOrganizationModel.CoreOrg>();
            string userID = Session["UID"].ToString();

            clsOrg = coreOrgHelper.GetOrgHeader(userID);
            string OrganizationID = "";
            foreach (var Org in clsOrg)
            {
                OrganizationID = Org.OrganizationID;

            };

            List<SelectListItem> NamaPemegangSaham = new List<SelectListItem>();
            NamaPemegangSaham = GetPemegangSahamDropDown(OrganizationID);
            ViewData["PemegangSaham_"] = NamaPemegangSaham;
            ViewBag.OrgID = OrganizationID;
            //  ViewBag.ShareHolderAsal = coreOrgHelper.GetShareHolder(OrganizationID);
            var clsshareholder = coreOrgHelper.GetShareHolder(OrganizationID);

            decimal sumNumberOfShare = clsshareholder.Sum(d => d.number_of_shares);

            List<CoreOrganizationModel.core_org_shareholderList> modelList = new List<CoreOrganizationModel.core_org_shareholderList>();

            foreach (var shareholder in clsshareholder)
            {
                CoreOrganizationModel.core_org_shareholderList model = new CoreOrganizationModel.core_org_shareholderList();
                {
                    model.organization_shareholder_idx = shareholder.organization_shareholder_idx;
                    if (shareholder.person_name != null)
                        model.shareholder_name = shareholder.person_name;
                    else
                        model.shareholder_name = shareholder.organization_name;

                    if (shareholder.person_identifier != null)
                        model.shareholder_identifier = shareholder.person_identifier;
                    else
                        model.shareholder_identifier = shareholder.organization_identifier;

                    model.status_pegangan = shareholder.status_pegangan;
                    decimal share = (shareholder.number_of_shares / sumNumberOfShare * 100);
                    model.number_of_shares = shareholder.number_of_shares;
                    model.number_of_shares_string = String.Format("{0:#,#.}", shareholder.number_of_shares);
                    model.share_percentage = share;
                    if (shareholder.person_identifier != null)
                    {
                        model.type = "Person";
                    }
                    else
                    {
                        model.type = "Organization";
                    }


                }
                modelList.Add(model);
            }

            ViewBag.ShareHolderAsal = modelList;


            ViewBag.module = module;
            return View();
        }

        public ActionResult PemegangSaham(string module, string btn)
        {
            ViewBag.itemID = Request.QueryString["item"];
            if (btn == "Next")
            {
                return RedirectToAction("Pengarah", "MM2H", new
                {
                    module = module
                });
            }
            if (btn == "Save")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }
            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }
            ViewBag.module = module;
            ViewBag.MSG_MANDATORY = "Sila masukkan data";
            List<SelectListItem> PemegangSahamPerson = new List<SelectListItem>();
            PemegangSahamPerson = GetStatusPemeganganSahamDropDown("Person");
            ViewData["StatusPemegangSahamPerson_"] = PemegangSahamPerson;

            List<SelectListItem> PemegangSahamOrganization = new List<SelectListItem>();
            PemegangSahamOrganization = GetStatusPemeganganSahamDropDown("Organization");
            ViewData["StatusPemegangSahamOrganization_"] = PemegangSahamOrganization;

            List<SelectListItem> religion = new List<SelectListItem>();
            religion = GetAgamaDropDown();
            ViewData["religionlist"] = religion;

            List<SelectListItem> gender = new List<SelectListItem>();
            gender = GetJantina();
            ViewData["genderlist"] = gender;

            TourlistWebAPI.ClassLib.RefCountryHelper countryHelper = new TourlistWebAPI.ClassLib.RefCountryHelper();

            List<SelectListItem> ref_malaysia = new List<SelectListItem>();
            ref_malaysia = GetMalaysiaDropDown();
            ViewData["Malaysialist_"] = ref_malaysia;

            List<SelectListItem> ref_asean = new List<SelectListItem>();
            ref_asean = GetAseanDropDown();
            ViewData["Aseanlist_"] = ref_asean;

            List<SelectListItem> ref_nonasean = new List<SelectListItem>();
            ref_nonasean = GetNonAseanDropDown();
            ViewData["NonAseanlist_"] = ref_nonasean;

            List<SelectListItem> ref_country = new List<SelectListItem>();
            ref_country = GetCountryDropDown();
            ViewData["Countrylist_"] = ref_country;

            string userID = Session["UID"].ToString();

            string sAppID = "";
            string scode_status = "";
            List<TourlistWebAPI.Models.MM2HModels.mm2h_app> app = new List<TourlistWebAPI.Models.MM2HModels.mm2h_app>();

            app = mM2HHelper.GetMM2HStatus(userID, module);

            foreach (var st in app)
            {
                sAppID = st.AppID.ToString();
                scode_status = st.Code_Status;
            }

            ViewBag.AppID = sAppID;
            ViewBag.Code_Status = scode_status;

            List<CoreOrganizationModel.CoreOrg> clsOrg = new List<CoreOrganizationModel.CoreOrg>();

            clsOrg = coreOrgHelper.GetOrgHeader(userID);
            string OrganizationID = "";

            foreach (var Org in clsOrg)
            {
                OrganizationID = Org.OrganizationID;
            };

            ViewBag.OrganizationID = OrganizationID;

            var Malaysia = ref_country.Where(d => d.Text.ToString() == "MALAYSIA").ToList();
            var MalaysiaVal = "";
            foreach (var item in Malaysia)
            {
                MalaysiaVal = item.Value.ToString();
            }

            ViewBag.Malaysia = MalaysiaVal;


            return View();

        }
        public ActionResult Pengarah(string module, string btn)
        {
            if (btn == "Next")
            {
                return RedirectToAction("DokumenSokongan", "MM2H", new
                {
                    module = module
                });
            }
            if (btn == "Save")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }
            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }
            ViewBag.MSG_MANDATORY = "Sila masukkan data";

            ViewBag.itemID = Request.QueryString["item"];
            ViewBag.module = module;
            //List<SelectListItem> NamaPemegangSaham = new List<SelectListItem>();
            //NamaPemegangSaham = GetPemegangSahamDropDown("");
            //ViewData["PemegangSaham_"] = NamaPemegangSaham;

            List<SelectListItem> gender = new List<SelectListItem>();
            gender = GetJantina();
            ViewData["genderlist"] = gender;

            //TourlistWebAPI.ClassLib.RefCountryHelper countryHelper = new TourlistWebAPI.ClassLib.RefCountryHelper();
            //var ref_country = countryHelper.GetCountryList();
            //ViewData["Country"] = ref_country;

            List<SelectListItem> ref_country = new List<SelectListItem>();
            ref_country = GetCountryDropDown();
            //ViewData["Countrylist_"] = ref_country;
            ViewBag.Country = ref_country;


            var Malaysia = ref_country.Where(d => d.Text.ToString() == "MALAYSIA").ToList();
            var MalaysiaVal = "";
            foreach (var item in Malaysia)
            {
                MalaysiaVal = item.Value.ToString();
            }

            ViewBag.Malaysia = MalaysiaVal;

            string sAppID = "";
            string scode_status = "";
            List<TourlistWebAPI.Models.MM2HModels.mm2h_app> app = new List<TourlistWebAPI.Models.MM2HModels.mm2h_app>();
            string userID = Session["UID"].ToString();
            app = mM2HHelper.GetMM2HStatus(userID, module);

            foreach (var st in app)
            {
                sAppID = st.AppID.ToString();
                scode_status = st.Code_Status;
            }

            ViewBag.AppID = sAppID;
            ViewBag.Code_Status = scode_status;
            List<CoreOrganizationModel.CoreOrg> clsOrg = new List<CoreOrganizationModel.CoreOrg>();

            clsOrg = coreOrgHelper.GetOrgHeader(userID);
            string OrganizationID = "";

            foreach (var Org in clsOrg)
            {
                OrganizationID = Org.OrganizationID;
            };

            ViewBag.OrganizationID = OrganizationID;

            return View();
        }
        public ActionResult DokumenSokongan(string module, string ItemID, string btn, List<HttpPostedFileBase> filesupload, List<HttpPostedFileBase> otherFilesupload, List<string> chkitem_instance, List<string> uploads_freeform_by_modules_idx, List<string> statusDeleted)
        {


            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "MM2H", new
                {
                    module = module
                });
            }
            string sAppID = "";
            string smoduleID = "";
            string scode_status = "";
            string userID = Session["UID"].ToString();
            List<TourlistWebAPI.Models.MM2HModels.mm2h_app> app = new List<TourlistWebAPI.Models.MM2HModels.mm2h_app>();
            sAppID = Session["AppID"].ToString();
            ViewBag.MSG_MANDATORY = "Sila masukkan data";

            app = mM2HHelper.GetMM2HStatus(userID, null);

            var itemID = Request.QueryString["item"];

            if (itemID == null)
            {
                ViewBag.itemID = ItemID;
                itemID = ItemID;
            }
            else
            {
                itemID = Request.QueryString["item"];
                ViewBag.itemID = itemID;
            }


            foreach (var st in app)
            {
                //sstatus = st.Status.ToString();
                smoduleID = st.ModuleID.ToString();
                sAppID = st.AppID.ToString();
                scode_status = st.Code_Status;
            }
            ViewBag.Code_Status = scode_status;

            string sModule = "";
            if (module == "MM2H_NEW")
                sModule = "MM2H_NEW_DOKUMEN";
            else if (module == "MM2H_RENEW")
                sModule = "MM2H_RENEW_DOKUMEN";
            else if (module == "MM2H_ADDBRANCH")
                sModule = "MM2H_ADDBRANCH_DOKUMEN";
            else if (module == "MM2H_CHANGESTATUS")
                sModule = "MM2H_CHANGESTATUS_DOKUMEN";
            else if (module == "MM2H_CANCELLICENSE")
                sModule = "MM2H_CANCELLICENCE_DOKUMEN";



            if (btn == "Simpan")
            {
                if (filesupload != null)
                {
                    int filecount = filesupload.Count();

                    for (int i = 0; i < filecount; i++)
                    {
                        if (filesupload[i] != null)
                        {
                            upload(filesupload[i], chkitem_instance[i]);
                        }
                    }

                    int iCount = mM2HHelper.checkDokumenSokongan(sAppID, sModule, userID);

                    if (iCount == 0)
                    {

                        coreOrgHelper.updateListing(itemID);
                    }
                }
                if (otherFilesupload != null)
                {
                    int filecount1 = otherFilesupload.Count();

                    for (int i = 0; i < filecount1; i++)
                    {
                        if (otherFilesupload[i] != null)
                        {
                            uploadOtherDocument(otherFilesupload[i], "", uploads_freeform_by_modules_idx[i], smoduleID, sAppID);
                        }
                    }
                }
            }

            ViewBag.module = module;

            ViewBag.ModuleID = smoduleID;
            ViewBag.AppID = sAppID;


            ViewBag.DashBoardList = mM2HHelper.GetDashboardList(sAppID, sModule, "");


            List<CoreOrganizationModel.CoreOrg> clsOrg = new List<CoreOrganizationModel.CoreOrg>();

            clsOrg = coreOrgHelper.GetOrgHeader(userID);
            string OrganizationID = "";

            foreach (var Org in clsOrg)
            {
                OrganizationID = Org.OrganizationID;
            };

            int iCountItem = 0;
            foreach (var item in ViewBag.DashBoardList)
            {
                if (item.upload_location == null && item.descr_bool2 == "2")
                {
                    iCountItem++;
                }
            }
            ViewBag.Hantar = "false";
            if (iCountItem == 0)
            {
                ViewBag.Hantar = "true";
            }


            ViewBag.OtherDocumentList = coreOrgHelper.GetOtherDocument(sAppID);
            if (module == "MM2H_CHANGESTATUS")
            {
                ViewBag.ShareholderList = coreOrgHelper.GetShareHolderChangeStatusAttachment(sAppID);
                ViewBag.PengarahList = coreOrgHelper.GetDirectorChangeStatusAttachment(sAppID);

            }
            else
            {
                ViewBag.PengarahList = coreOrgHelper.GetDirectorAttachment(OrganizationID);
                ViewBag.ShareholderList = coreOrgHelper.GetShareHolderAttachment(OrganizationID);
            }


            return View();
        }


        public void upload(HttpPostedFileBase file, string chkitem_instance)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            //string contentType = file.ContentType;
            string folder = "/Attachment/" + filename;
            string foldersave = Server.MapPath("~/Attachment");
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            doc.FileName = file.FileName;
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.MM2H, filename); //folder;
            doc.chkitem_instanceID = chkitem_instance;
            doc.UploadFileName = filename;

            bool ID = coreOrgHelper.updateDocumentSokongan(doc);

            if (ID)
            {
                //use common upload in base contoller
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.MM2H);
                //using (Stream fs = file.InputStream)
                //{
                //    using (BinaryReader br = new BinaryReader(fs))
                //    {
                //        byte[] bytes = br.ReadBytes((Int32)fs.Length);
                //        file.SaveAs(Path.Combine(foldersave, filename));
                //    }
                //}

            }
        }

        public void uploadPersonID(HttpPostedFileBase file, string PersonID, string typeFile)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            string folder = "/Attachment/" + filename;
            string foldersave = Server.MapPath("~/Attachment");
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            var userID = Session["UID"].ToString();
            doc.FileName = file.FileName;
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.MM2H, filename); //folder;
            doc.PersonID = PersonID;
            doc.UploadFileName = filename;
            doc.type = typeFile;
            doc.userID = userID;

            bool ID = coreOrgHelper.updateDocumentSokonganPerson(doc);

            if (ID)
            {
                //use common upload in base contoller
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.MM2H);
                //using (Stream fs = file.InputStream)
                //{
                //    using (BinaryReader br = new BinaryReader(fs))
                //    {
                //        byte[] bytes = br.ReadBytes((Int32)fs.Length);
                //        file.SaveAs(Path.Combine(foldersave, filename));
                //    }
                //}

            }
        }

        public void uploadOrganizationID(HttpPostedFileBase file, string OrgID, string typeFile)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            string folder = "/Attachment/" + filename;
            string foldersave = Server.MapPath("~/Attachment");
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            var userID = Session["UID"].ToString();
            doc.FileName = file.FileName;
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.MM2H, filename); //folder;
            doc.OrgID = OrgID;
            doc.UploadFileName = filename;
            doc.type = typeFile;
            doc.userID = userID;

            bool ID = coreOrgHelper.updateDocumentSokonganOrganization(doc);

            if (ID)
            {
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.MM2H);
            }
        }

        public bool uploadOtherDocument(HttpPostedFileBase file, string FileName, string DocID, string moduleID, string appID)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            string folder = "/Attachment/" + filename;

            CoreOrganizationModel.CoreOtherDocument doc = new CoreOrganizationModel.CoreOtherDocument();

            var userID = Session["UID"].ToString();

            doc.module_ref = moduleID;
            doc.transaction_ref = appID;
            doc.document_upload_path = this.GetUploadFolder(TourlistEnums.MotacModule.MM2H, filename); //folder;

            doc.document_description = file.FileName;
            doc.UserID = userID;

            bool ID = false;
            if (FileName != "")
            {
                doc.document_name = FileName;
                ID = coreOrgHelper.OtherDocument_SaveNew(doc);
            }
            else
            {
                doc.docID = DocID;
                ID = coreOrgHelper.UpdateOtherDocument(doc);
            }


            string foldersave = Server.MapPath("~/Attachment");
            if (ID)
            {
                //use common upload in base contoller
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.MM2H);
                //using (Stream fs = file.InputStream)
                //{
                //    using (BinaryReader br = new BinaryReader(fs))
                //    {
                //        byte[] bytes = br.ReadBytes((Int32)fs.Length);
                //        file.SaveAs(Path.Combine(foldersave, filename));
                //    }
                //}
                return true;
            }
            return false;

        }



        public ActionResult download(string file)
        {
            string folder = Server.MapPath("~/Attachment/");
            string Filename = folder + file;
            var stream = new MemoryStream();

            /// return File(stream, "application/octet-stream", Filename);

            // string location = id;
            try
            {
                if (System.IO.File.Exists(Filename))
                {

                    FileStream F = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(F, "application/octet-stream", file);
                }

            }
            catch (IOException ex)
            {


            }
            return View();
        }



        public List<SelectListItem> GetStatusPemeganganSahamDropDown(string pemegangsaham)
        {

            List<SelectListItem> StatusDropdown = new List<SelectListItem>();
            var status = coreOrgHelper.GetRefListByType("STATUSSHAREDHOLDER");

            foreach (var app in status)
            {
                if (pemegangsaham == "All")
                {
                    StatusDropdown.Add(new SelectListItem
                    {
                        Value = app.ref_idx.ToString(),
                        Text = app.ref_description,
                    });

                }
                else
                {
                    if (app.text_field == pemegangsaham)
                    {
                        StatusDropdown.Add(new SelectListItem
                        {
                            Value = app.ref_idx.ToString(),
                            Text = app.ref_description,
                        });
                    }
                }


            }
            return StatusDropdown;
        }

        public List<SelectListItem> GetJantina()
        {
            List<SelectListItem> StatusDropdown = new List<SelectListItem>();
            var status = refHelper.GetRefListByType("GENDER");
            foreach (var app in status)
            {
                StatusDropdown.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description,
                });
            }
            return StatusDropdown;
        }

        public List<SelectListItem> GetTahun()
        {
            List<SelectListItem> yearsDropdown = new List<SelectListItem>();
            var years = coreOrgHelper.GetRefListByType("RENEWALYEAR");
            foreach (var app in years)
            {
                yearsDropdown.Add(new SelectListItem
                {
                    Value = app.ref_code.ToString(),
                    Text = app.ref_description,
                });
            }
            return yearsDropdown;
        }
        public List<SelectListItem> GetAgamaDropDown()
        {
            List<SelectListItem> ReligionDropdown = new List<SelectListItem>();
            var religion = refHelper.GetRefListByType("RELIGION");
            foreach (var app in religion)
            {
                ReligionDropdown.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_name,
                });
            }
            return ReligionDropdown;
        }
        public List<SelectListItem> GetAseanDropDown()
        {

            List<SelectListItem> AseanDropdown = new List<SelectListItem>();
            var status = refCountryHelper.GetAseanList();
            foreach (var app in status)
            {
                AseanDropdown.Add(new SelectListItem
                {
                    Value = app.country_idx.ToString(),
                    Text = app.country_name,
                });
            }
            return AseanDropdown;
        }

        public List<SelectListItem> GetMalaysiaDropDown()
        {

            List<SelectListItem> MalaysiaDropdown = new List<SelectListItem>();
            var status = refCountryHelper.GetMalaysiaList();
            foreach (var app in status)
            {
                MalaysiaDropdown.Add(new SelectListItem
                {
                    Value = app.country_idx.ToString(),
                    Text = app.country_name,

                });

            }
            return MalaysiaDropdown;
        }

        public List<SelectListItem> GetNonAseanDropDown()
        {
            List<SelectListItem> NonAseanDropdown = new List<SelectListItem>();
            var status = refCountryHelper.GetNonAseanList();
            foreach (var app in status)
            {
                NonAseanDropdown.Add(new SelectListItem
                {
                    Value = app.country_idx.ToString(),
                    Text = app.country_name,
                });
            }
            return NonAseanDropdown;
        }
        public List<SelectListItem> GetCountryDropDown()
        {
            List<SelectListItem> CountryDropdown = new List<SelectListItem>();
            var status = refCountryHelper.GetCountryList();
            foreach (var app in status)
            {
                CountryDropdown.Add(new SelectListItem
                {
                    Value = app.country_idx.ToString(),
                    Text = app.country_name,
                });
            }
            return CountryDropdown;
        }
        public List<SelectListItem> GetDirectorDropDown(string orgID)
        {
            var Directors = coreOrgHelper.GetDirectorPerakuan(orgID);
            List<SelectListItem> DirectorDropdown = new List<SelectListItem>();

            //DirectorDropdown.Add(new SelectListItem
            //{
            //    Value = "0",
            //    Text = "Sila Pilih",
            //});
            if (Directors != null)
            {
                foreach (var director in Directors)
                {
                    DirectorDropdown.Add(new SelectListItem
                    {
                        Value = director.person_idx.ToString(),
                        Text = director.person_name,
                    });
                }
            }


            return DirectorDropdown;
        }

        public List<SelectListItem> GetPemegangSahamDropDown(string orgID)
        {
            var ShareHolder = coreOrgHelper.GetShareHolder(orgID);
            List<SelectListItem> PemegangSaham = new List<SelectListItem>();

            foreach (var app in ShareHolder)
            {
                if (app.person_name != "" && app.person_name != null)
                {
                    PemegangSaham.Add(new SelectListItem
                    {
                        Value = app.person_identifier,
                        Text = app.person_name,
                    });
                }

            }

            return PemegangSaham;
        }
        public List<SelectListItem> GetJustifikasiSerahLesenDropDown()
        {
            List<SelectListItem> Lesen = new List<SelectListItem>();
            var licenses = refHelper.GetRefListByType("REASONCANCEL");
            foreach (var app in licenses)
            {
                Lesen.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description,
                });
            }
            return Lesen;
        }

        public List<SelectListItem> GetJustifikasiPemegangSahamDropDown()
        {
            List<SelectListItem> shareholders = new List<SelectListItem>();
            var shareholder = refHelper.GetRefListByType("JUSTIFICATIONSHAREHOLDER");
            foreach (var app in shareholder)
            {
                shareholders.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description,
                });
            }


            return shareholders;
        }

        public List<SelectListItem> GetJustifikasiPemegangSahamOrgDropDown()
        {
            List<SelectListItem> shareholders = new List<SelectListItem>();
            var shareholder = refHelper.GetRefListByType("JustifikasiBatalLesen_");
            foreach (var app in shareholder)
            {
                shareholders.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description,
                });
            }


            return shareholders;
        }


        [HttpPost]
        public JsonResult ajaxApplication(string Module,Guid OrganizationID)
        {
            string userID = Session["UID"].ToString();

            var Application = mM2HHelper.GetApplicationMain(userID, Module, OrganizationID);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ajaxShareHolder(string OrganizationID)
        {
            var ShareHolder = coreOrgHelper.GetShareHolder(OrganizationID);

            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxShareHolderList(string OrganizationID)
        {
            var ShareHolder = coreOrgHelper.GetShareHolderMM2HList(OrganizationID);
            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxShareHolderDetail(string organization_shareholder_idx)
        {
            var ShareHolder = coreOrgHelper.GetShareHolderDetailByID(organization_shareholder_idx);
            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult findShareHolderDetail(string identifier, string person_id)
        {
            if (person_id != "")
            {
                var guid = Guid.Parse(identifier);
                var person = unitOfWork.CorePersonsRepository.Find(x => x.person_identifier == person_id).FirstOrDefault();
                var idx = unitOfWork.CoreOrganizationShareholders.Find(x => x.organization_ref == guid && x.shareholder_person_ref == person.person_idx).Select(x => x.organization_shareholder_idx).FirstOrDefault().ToString();
                var ShareHolder = coreOrgHelper.GetShareHolderDetailByID(idx);
                return Json(ShareHolder, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ajaxAddBranch(string OrganizationID)
        {
            var AddBranch = mM2HHelper.GetBranchList(OrganizationID);

            return Json(AddBranch, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxAddBranchAppID(string AppID)
        {
            var AddBranch = mM2HHelper.GetBranchAppIDList(AppID);

            return Json(AddBranch, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxSerahBatalLesenDetail(string OrganizationID)
        {
            var terminateDetail = mM2HHelper.GetSerahBatalLesenDetail(OrganizationID);

            return Json(terminateDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxAddBranchDetail(string branches_idx)
        {
            var AddBranchDetail = mM2HHelper.GetBranchDetail(branches_idx);

            return Json(AddBranchDetail, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ajaxSyarikat(string sRegistrationNo)
        {
            // var syarikat = mM2HHelper.GetCompany(sRegistrationNo);
            var syarikat = coreOrgHelper.GetCompany(sRegistrationNo);

            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxOrganizationUpdate(Guid AppID)
        {
            // var syarikat = mM2HHelper.GetCompany(sRegistrationNo);

            var syarikat = coreOrgHelper.GetOrganizationUpdateByCode(AppID, "mm2h");


            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxRenewYear(string AppID)
        {
            // var syarikat = mM2HHelper.GetCompany(sRegistrationNo);
            var year = coreOrgHelper.GetRenewYear(AppID);

            return Json(year, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ajaxLicense(string OrganizationID)
        {

            var syarikat = mM2HHelper.GetMM2Hlicence(OrganizationID);

            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxPerakuan(string PersonID)
        {

            var Director = coreOrgHelper.GetDirectorPerakuanDetail(PersonID);

            return Json(Director, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxLicenseHeader()
        {
            string userID = Session["UID"].ToString();
            var syarikat = mM2HHelper.GetMM2HHeader(userID);

            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ajaxLicenseHeaderCommon(string applyId)
        {
            var cuser = TourlistUnitOfWork.FlowApplicationStubs.Find(x => x.apply_idx.ToString() == applyId).FirstOrDefault();
            string userID = cuser.apply_user.ToString();
            var syarikat = mM2HHelper.GetMM2HHeader(userID);

            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
public void ajaxGenerateApplicationStubs(string Module, Guid OrganizationID)
        {
            var userID = Session["UID"];
            Guid Id = new Guid(userID.ToString());
            var user = coreHelper.GetCoreUserByGuid(Id);

            if (Module == "New")
            {
                coreHelper.GenerateApplicationStubs(
                        TourlistEnums.MotacModule.MM2H,
                        TourlistEnums.ModuleLicenseType.TIADA,
                        TourlistEnums.SolModulesType.MM2H_NEW,
                        TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                        user);
            }
            else if (Module == "Renewal")
            {
                var stub = coreHelper.GenerateApplicationStubs(
                        TourlistEnums.MotacModule.MM2H,
                        TourlistEnums.ModuleLicenseType.TIADA,
                        TourlistEnums.SolModulesType.MM2H_RENEW,
                        TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                        user);

                Guid stubID = stub.application_Stubs.apply_idx;
                coreOrgHelper.updateMM2HLicenseRenewal(OrganizationID, stubID, Id);
            }
            else if (Module == "Branch")
            {
                coreHelper.GenerateApplicationStubs(
                        TourlistEnums.MotacModule.MM2H,
                        TourlistEnums.ModuleLicenseType.TIADA,
                        TourlistEnums.SolModulesType.MM2H_ADDBRANCH,
                        TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                        user);
            }
            else if (Module == "ChangeStatus")
            {
                var stub = coreHelper.GenerateApplicationStubs(
                         TourlistEnums.MotacModule.MM2H,
                         TourlistEnums.ModuleLicenseType.TIADA,
                         TourlistEnums.SolModulesType.MM2H_CHANGESTATUS,
                         TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                         user);

                Guid stubID = stub.application_Stubs.apply_idx;


                coreOrgHelper.ChangeStatusOrganization_SaveNew(OrganizationID, stubID, Id);
                coreOrgHelper.ChangeStatusShareHolder_SaveNew(OrganizationID, stubID, Id);
                coreOrgHelper.ChangeStatusDirector_SaveNew(OrganizationID, stubID, Id);

            }
            else if (Module == "Cancel")
            {
                coreHelper.GenerateApplicationStubs(
                        TourlistEnums.MotacModule.MM2H,
                        TourlistEnums.ModuleLicenseType.TIADA,
                        TourlistEnums.SolModulesType.MM2H_CANCELLICENSE,
                        TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                        user);
            }


            //  return Json(syarikat, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public void ajaxCompanyUpdate(core_organizations obj, string ItemID)
        {

            if (obj.registered_mobile_no == null)
            {
                obj.registered_mobile_no = "";
            }

            if (obj.registered_phone_no == null)
            {
                obj.registered_phone_no = "";
            }
            if (obj.registered_fax_no == null)
            {
                obj.registered_fax_no = "";
            }
            if (obj.registered_email == null)
            {
                obj.registered_email = "";
            }
            if (obj.registered_postcode == null)
            {
                obj.registered_postcode = "";
            }

            if (obj.office_mobile_no == null)
            {
                obj.office_mobile_no = "";
            }
            if (obj.office_phone_no == null)
            {
                obj.office_phone_no = "";
            }
            if (obj.office_email == null)
            {
                obj.office_email = "";
            }
            if (obj.office_fax_no == null)
            {
                obj.office_fax_no = "";
            }
            if (obj.cosec_mobile_no == null)
            {
                obj.cosec_mobile_no = "";
            }
            if (obj.cosec_phone_no == null)
            {
                obj.cosec_phone_no = "";
            }
            if (obj.cosec_email == null)
            {
                obj.cosec_email = "";
            }
            if (obj.cosec_fax_no == null)
            {
                obj.cosec_fax_no = "";
            }
            if (obj.cosec_fax_no == null)
            {
                obj.cosec_fax_no = "";
            }
            if (obj.cosec_name == null)
            {
                obj.cosec_name = "";
            }
            if (obj.cosec_addr_1 == null)
            {
                obj.cosec_addr_1 = "";
            }
            if (obj.cosec_addr_2 == null)
            {
                obj.cosec_addr_2 = "";
            }
            if (obj.cosec_addr_3 == null)
            {
                obj.cosec_addr_3 = "";
            }

            if (obj.cosec_postcode == null)
            {
                obj.cosec_postcode = "";
            }

           

            var userID = Session["UID"].ToString();

            obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.UpdateOrganization(obj);

            if (ID == true)
            {
                bool itemID = coreOrgHelper.updateListing(ItemID);
            }

        }

        [HttpPost]
        public JsonResult ajaxShareholderPersonUpdate(core_persons obj, string status_shareholder, string shareholderID, HttpPostedFileBase file_MyKad, string typeFile, string OrganizationID, string itemIDx)
        {

            var userID = Session["UID"].ToString();
            obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.UpdatePerson(obj);

            if (ID == true)
            {
                //    bool itemID = coreOrgHelper.updateListing(ItemID);
                bool itemID = coreOrgHelper.updateStatusShareHolder(status_shareholder, shareholderID, "");

                if (file_MyKad != null)
                {
                    uploadPersonID(file_MyKad, obj.person_idx.ToString(), typeFile);
                }

            }

            int shareholder = coreOrgHelper.CheckShareHolder(OrganizationID);

            if (shareholder == 0)
            {

                coreOrgHelper.updateListing(itemIDx);
            }
            return Json(ID, JsonRequestBehavior.AllowGet);

        }




        [HttpPost]
        public JsonResult ajaxShareholderOrganizationUpdate(CoreOrganizationModel.core_organizations obj, string status_shareholder, string shareholderID, string OrganizationID, string itemIDx, string registered_year)
        {

            var userID = Session["UID"].ToString();
            obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.UpdateOrganizationShareholder(obj);

            if (ID == true)
            {
                bool itemID = coreOrgHelper.updateStatusShareHolder(status_shareholder, shareholderID, registered_year);
            }

            int shareholder = coreOrgHelper.CheckShareHolder(OrganizationID);

            if (shareholder == 0)
            {

                coreOrgHelper.updateListing(itemIDx);
            }
            return Json(ID, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxDirector(string OrganizationID)
        {
            var directors = coreOrgHelper.GetDirector(OrganizationID);

            return Json(directors, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult ajaxDirectorUpdate(core_persons persons, HttpPostedFileBase file_MyKad, HttpPostedFileBase file_MyPengajian, string typeFile, string OrganizationID, string itemIDx)
        {
            bool directors = coreOrgHelper.UpdateDirector(persons);
            if (directors == true)
            {
                if (file_MyKad != null)
                {
                    uploadPersonID(file_MyKad, persons.person_idx.ToString(), typeFile);
                }

                if (file_MyPengajian != null)
                {
                    uploadPersonID(file_MyPengajian, persons.person_idx.ToString(), "PAS_PENGGAJIAN");
                }
            }

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var type_pas = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN" && c.ref_type == refType.ref_idx).FirstOrDefault();

            //  var type_pas = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN").FirstOrDefault();

            var doc_id = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == persons.person_idx && c.upload_type_ref == type_pas.ref_idx).FirstOrDefault();
            var malaysia_id = unitOfWork.RefGeoCountriesRepository.Find(c => c.country_code == "MY").FirstOrDefault();
            if (persons.person_nationality == malaysia_id.country_idx)
            {
                if (doc_id != null)
                {
                    coreOrgHelper.DeleteDocumentPasPengajian(doc_id.uploads_freeform_by_persons_idx.ToString());
                }
            }
            int shareholder = coreOrgHelper.CheckDirector(OrganizationID);
            if (shareholder == 0)
            {
                coreOrgHelper.updateListing(itemIDx);
            }
            return Json(directors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxSaveOtherDocument(HttpPostedFileBase file_Dokumen, string filename, string ModuleID, string AppId)
        {

            bool doc = uploadOtherDocument(file_Dokumen, filename, "", ModuleID, AppId);

            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxDeleteApplication(string id)
        {
            var application = mM2HHelper.DeleteApplication(id);
            return Json(application, JsonRequestBehavior.DenyGet);
        }
        public JsonResult ajaxDeleteOtherDocument(string idx)
        {

            bool doc = coreOrgHelper.DeleteOtherDocument(idx);

            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxDeleteBranch(string idx, string itemID, string organizationID)
        {

            bool doc = mM2HHelper.DeleteBranch(idx, itemID, organizationID);


            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#58741)  on 24 Jan 2024
        [HttpGet]
        public JsonResult MM2HBranchesActive(Guid Org_ref)
        {
            var branches = tobtabBLL.GetMM2HBranchesActive(Org_ref);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#58741)  on 24 Jan 2024
        [HttpGet]
        public JsonResult MM2HBranchesbyBranchIdx(Guid branches_idx)
        {
            var branches = tobtabBLL.GetMM2HBranchesbyBranchIdx(branches_idx);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ajaxSaveAcknowledge(string appid, string personid, string acknowlegdeID, string itemID)
        {
            CoreOrganizationModel.CoreAcknowledgement model = new CoreOrganizationModel.CoreAcknowledgement();
            var userID = Session["UID"].ToString();

            model.UserID = Guid.Parse(userID);
            model.acknowledge_person_ref = Guid.Parse(personid);
            model.stub_ref = Guid.Parse(appid);

            //var acknowledge = coreOrgHelper.getAcknowledge(appid);
            bool ID = false;
            if (acknowlegdeID != "00000000-0000-0000-0000-000000000000")
            {
                model.acknowledgement_idx = Guid.Parse(acknowlegdeID);
                ID = coreOrgHelper.UpdateAcknowledgement(model);
                bool listID = coreOrgHelper.updateListing(itemID);
            }
            else
            {
                ID = coreOrgHelper.Acknowledgement_SaveNew(model);
                if (ID == true)
                {
                    bool listID = coreOrgHelper.updateListing(itemID);
                }
            }
            return Json(ID, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult ajaxListingUpdate(string ItemID)
        {
            bool itemID = coreOrgHelper.updateListing(ItemID);

            return Json(itemID, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ajaxUpdateRenewYear(string TempohPembaharuan, string AppID, string ItemList)
        {
            bool itemID = false;
            if (TempohPembaharuan == "")
            {
                itemID = true;
            }
            else
            {
                itemID = coreOrgHelper.updateTempohPembaharuan(TempohPembaharuan, AppID);
                if (itemID)
                {
                    bool itemID1 = coreOrgHelper.updateListing(ItemList);
                }

            }

            return Json(itemID, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ajaxUpdateSSM(string ItemID)
        {
            bool itemID = coreOrgHelper.updateListing(ItemID);
        }

        [HttpPost]
        public JsonResult ajaxSaveApplication(string AppID, string Module, string noLicence, string OrganizationID)
        {

            string userID = Session["UID"].ToString();
            //checking if status Draft & generateAppNo
            coreHelper.generateApplicationNo(Guid.Parse(AppID), Guid.Parse(userID));
            bool itemID = false;
            if (Module == "MM2H_NEW")
            {
                itemID = mM2HHelper.UpdateApplicationNew(AppID, userID);
            }
            else if (Module == "MM2H_RENEW")
            {
                itemID = mM2HHelper.UpdateApplicationRenew(AppID, noLicence, userID);

                Guid Id = new Guid(userID.ToString());
                var coreLicense = coreHelper.getCoreLicense(Guid.Parse(AppID));
                DateTime now = DateTime.Now;
                if (coreLicense != null && coreLicense.end_dt != null)
                {
                    DateTime end = (DateTime)coreLicense.end_dt;
                    int day = (now - end).Days;
                    if (day > 30)
                    {
                        var compound = coreHelper.getCompoundByStubRef(Guid.Parse(AppID), end);
                        if (compound == null)
                        {
                            string refCode = "COMP_ACT_MM2H";
                            coreHelper.saveCompoundExpiredDate(Guid.Parse(AppID), Id, end, refCode);
                        }
                    }

                }

            }
            else if (Module == "MM2H_ADDBRANCH")
            {
                itemID = mM2HHelper.UpdateApplication(AppID, userID);

                var AddBranch = mM2HHelper.GetBranchList(OrganizationID);
                foreach (var item in AddBranch)
                {
                    if (item.status == "Deraf")
                    {
                        bool branchUpdate = mM2HHelper.UpdateBranchStatus(item.mm2h_add_branches_idx, userID);
                    }


                }


            }
            else if (Module == "MM2H_CANCELLICENSE")
            {
                itemID = mM2HHelper.UpdateApplicationProcess(AppID, userID);
            }
            else if (Module == "MM2H_CHANGESTATUS")
            {
                itemID = mM2HHelper.UpdateApplicationProcess(AppID, userID);
            }


            return Json(itemID, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult ajaxSavePremiseDoc(string AppID, string itemID)
        {
            string userID = Session["UID"].ToString();
            bool bAppID = false;
            bool bItemID = false;

            bAppID = coreOrgHelper.updatePremiseApp(AppID, userID);

            if (bAppID)
            {
                bItemID = coreOrgHelper.updateListing(itemID);
            }

            return Json(bItemID, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        public JsonResult ajaxUpdateCompanyDataSSM()
        {

            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TourlistWebAPI.Models.MM2HModels.mm2h_ssm_organization();

            model_org.date_of_change = Request["date_of_change"].ToString();
            var status = Request["status"];
            if (status != null)
                model_org.status = Request["status"].ToString();
            else
                model_org.status = "";
            model_org.origin = Request["origin"].ToString();
            model_org.company_name = Request["company_name"].ToString();
            model_org.company_oldname = Request["company_oldname"].ToString();
            model_org.company_regno = Request["company_regno"].ToString();
            model_org.company_newregno = Request["company_newregno"].ToString();
            model_org.company_incdate = Request["company_incdate"].ToString();
            model_org.company_regdate = Request["company_regdate"].ToString();
            model_org.company_curassets = Request["company_curassets"].ToString();
            // model_org.company_totalcharge = Request["company_totalcharge"].ToString();
            model_org.company_curliabilities = Request["company_curliabilities"].ToString();

            model_org.companyb_addr1 = Request["companyb_addr1"].ToString();
            model_org.companyb_addr2 = Request["companyb_addr2"].ToString();
            model_org.companyb_addr3 = Request["companyb_addr3"].ToString();
            model_org.companyb_postcode = Request["companyb_postcode"].ToString();
            model_org.companyb_town = Request["companyb_town"].ToString();
            model_org.companyb_state = Request["companyb_state"].ToString();

            model_org.companyr_addr1 = Request["companyr_addr1"].ToString();
            model_org.companyr_addr2 = Request["companyr_addr2"].ToString();
            model_org.companyr_addr3 = Request["companyr_addr3"].ToString();
            model_org.companyr_postcode = Request["companyr_postcode"].ToString();
            model_org.companyr_town = Request["companyr_town"].ToString();
            model_org.companyr_state = Request["companyr_state"].ToString();

            //model_org.cosec_name = Request["cosec_name"].ToString();
            //model_org.cosec_no = Request["cosec_no"].ToString();
            //model_org.cosec_addr1 = Request["cosec_addr1"].ToString();
            //model_org.cosec_addr2 = Request["cosec_addr2"].ToString();
            //model_org.cosec_addr3 = Request["cosec_addr3"].ToString();
            //model_org.cosec_postcode = Request["cosec_postcode"].ToString();
            //model_org.cosec_town = Request["cosec_town"].ToString();
            //model_org.cosec_state = Request["cosec_state"].ToString();
            model_org.nature_of_business = Request["nature_of_business"].ToString();
            model_org.auditor_name = Request["auditor_name"].ToString();
            model_org.auditor_no = Request["auditor_no"].ToString();
            model_org.auditor_addr1 = Request["auditor_addr1"].ToString();
            model_org.auditor_addr2 = Request["auditor_addr2"].ToString();
            model_org.auditor_addr3 = Request["auditor_addr3"].ToString();
            model_org.auditor_postcode = Request["auditor_postcode"].ToString();
            model_org.audtitor_town = Request["audtitor_town"].ToString();
            model_org.auditor_state = Request["auditor_state"].ToString();
            model_org.financial_year_end = Request["financial_year_end"].ToString();
            model_org.is_unqualified_report = Request["is_unqualified_report"].ToString();
            model_org.is_consilidated_account = Request["is_consilidated_account"].ToString();
            model_org.tabling_date = Request["tabling_date"].ToString();
            model_org.non_current_assets = Request["non_current_assets"].ToString();
            model_org.current_assets = Request["current_assets"].ToString();
            model_org.non_current_liabilities = Request["non_current_liabilities"].ToString();
            model_org.current_liabilities = Request["current_liabilities"].ToString();
            model_org.share_capital = Request["share_capital"].ToString();
            model_org.reserve = Request["reserve"].ToString();
            model_org.retain_earning = Request["retain_earning"].ToString();
            model_org.bal_minority_interest = Request["bal_minority_interest"].ToString();
            model_org.revenue = Request["revenue"].ToString();
            model_org.profit_lost_before_tax = Request["profit_lost_before_tax"].ToString();
            model_org.profit_lost_after_tax = Request["profit_lost_after_tax"].ToString();
            model_org.net_dividend = Request["net_dividend"].ToString();
            model_org.income_minority_interest = Request["income_minority_interest"].ToString();
            model_org.origin = Request["Origin"].ToString();


            model_org.capital_ordinary_cash = Request["capital_ordinary_cash"].ToString();
            model_org.capital_ordinary_otherwise = Request["capital_ordinary_otherwise"].ToString();
            model_org.capital_preference_cash = Request["capital_preference_cash"].ToString();
            model_org.capital_preference_otherwise = Request["capital_preference_otherwise"].ToString();
            model_org.capital_others_cash = Request["capital_others_cash"].ToString();
            model_org.capital_others_otherwise = Request["capital_others_otherwise"].ToString();


            model_org.last_old_name = Request["last_old_name"].ToString();

            //var dDateOfChange = Request["date_of_change"].ToString();
            //if (dDateOfChange == "/Date(-62135596800000)/")
            //    model_org.date_of_change = DateTime.Parse("1970-01-01");
            //else
            //    model_org.date_of_change = JsonDateTimeToNormal(dDateOfChange);

            // model_org.date_of_change = Request["date_of_change"].ToString();
            model_org.registration_date = Request["registration_date"].ToString();

            model_org.company_type = Request["company_type"].ToString();
            model_org.company_status = Request["company_status"].ToString();
            model_org.company_category = Request["company_category"].ToString();

            var Application = mM2HHelper.UpdateCompanyDataSSM(module_id, component_id, model_org);

            if (Application == "success")
            {
                bool itemID = coreOrgHelper.updateListing(component_id);
            }




            return Json(Application, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult ajaxUpdateCompanyDirectorsSSM()
        {

            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TourlistWebAPI.Models.MM2HModels.mm2h_ssm_directors();
            var Application = "";

            if (Request["director_name"] != null)
            {

                model_org.director_name = Request["director_name"].ToString();
                model_org.director_docno = Request["director_docno"].ToString();
                model_org.director_addr1 = Request["director_addr1"].ToString();
                model_org.director_addr2 = Request["director_addr2"].ToString();
                model_org.director_addr3 = Request["director_addr3"].ToString();
                model_org.director_postcode = Request["director_postcode"].ToString();
                model_org.director_town = Request["director_town"].ToString();
               // model_org.director_date_appointed = Request["director_date_appointed"].ToString();

                var dDateOfappointed = Request["director_date_appointed"].ToString();
                model_org.director_date_appointed = Request["director_date_appointed"] == null ? DateTime.Parse("1970-01-01") : DateTime.ParseExact(Request["director_date_appointed"].ToString(), "dd/MM/yyyy", null);
                //if (dDateOfappointed == "/Date(-62135596800000)/")
                //    model_org.director_date_appointed = DateTime.Parse("1970-01-01");
                //else
                //    model_org.director_date_appointed = JsonDateTimeToNormal(dDateOfappointed);

                model_org.director_designation = Request["director_designation"].ToString();
                model_org.director_idType = Request["director_idType"].ToString();
                Application = mM2HHelper.ajaxUpdateCompanyDirectorsSSM(module_id, component_id, model_org);
            }


            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxUpdateCompanyShareholdersSSM()
        {
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TourlistWebAPI.Models.MM2HModels.mm2h_ssm_shareholders();
            var Application = "";

            if (Request["shareholder_name"] != null)
            {

                model_org.shareholder_name = Request["shareholder_name"].ToString();
                model_org.shareholder_docno = Request["shareholder_docno"].ToString();
                model_org.shareholder_totalshare = Request["shareholder_totalshare"].ToString();
                model_org.shareholder_idType = Request["shareholder_idtype"].ToString();

                Application = mM2HHelper.ajaxUpdateCompanyShareholdersSSM(module_id, component_id, model_org);
            }


            return Json(Application, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult ajaxUpdateCompanyChargesSSM()
        {

            var organization_id = Request["OrganizationID"].ToString();
            var Application = "";
            if (Request["charge_num"] != null)
            {
                var charger = new TourlistWebAPI.Models.MM2HModels.mm2h_ssm_charges();
                var dDateCreation = Request["date_creation"].ToString();
                charger.date_creation = Request["date_creation"] == null ? DateTime.Parse("1970-01-01") : DateTime.ParseExact(Request["date_creation"].ToString(), "dd/MM/yyyy", null);
                //if (dDateCreation == "/Date(-62135596800000)/")
                //    charger.date_creation = DateTime.Parse("1970-01-01");
                //else
                //    charger.date_creation = JsonDateTimeToNormal(dDateCreation);

                var cs = Request["charge_status"].ToString();
                string ChangeStatus = "";
                if (cs == "S")
                    ChangeStatus = "FULLY SATISFIED";
                else if (cs == "p")
                    ChangeStatus = "PARTLY SATISFIED";
                else if (cs == "R")
                    ChangeStatus = "FULLY RELEASED";
                else if (cs == "Q")
                    ChangeStatus = "PARTLY RELEASED";
                else if (cs == "U")
                    ChangeStatus = "UNSATISFIED";
                else if (cs == "B")
                    ChangeStatus = "CANCELLATION";
                else if (cs == "C")
                    ChangeStatus = "FULLY CEASED";

                charger.charge_num = Request["charge_num"].ToString();
                charger.chargee_name = Request["chargee_name"].ToString();
                charger.total_charge = Request["total_charge"].ToString();
                charger.charge_status = ChangeStatus;
                string userID = Session["UID"].ToString();
                Application = mM2HHelper.ajaxUpdateCompanyChargersSSM(organization_id, charger.charge_num, charger, Guid.Parse(userID));
            }


            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxSaveBranch(TourlistWebAPI.Models.MM2HModels.mm2h_branch branch, string status, string itemIDx, string license_no)
        {

            var userID = Session["UID"].ToString();
            branch.user_id = userID;
            //  branch.branch_license_ref_code = mM2HHelper.GetBranchLicenseNo(license_no, branch.OrganizationID);
            branch.branch_license_ref_code = "";
            bool ID = false;
            if (status == "Add")
                ID = mM2HHelper.AddBranch_SaveNew(branch);
            else
                ID = mM2HHelper.UpdateAddBranch(branch);


            coreOrgHelper.updateListing(itemIDx);
            return Json(ID, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ajaxCancelLicense(TourlistWebAPI.Models.MM2HModels.mm2h_terminate terminate, string ItemList, string sType, string OrganizationID)
        {
            string userID = Session["UID"].ToString();

            terminate.user_id = userID;
            bool doc = mM2HHelper.CancelLicense(terminate, sType, OrganizationID);

            coreOrgHelper.updateListing(ItemList);


            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ajaxUpload(HttpPostedFileBase file_Dokumen, string chkitem_instance, string module, string AppId, string itemID)
        {
            upload(file_Dokumen, chkitem_instance);
            string userID = Session["UID"].ToString();

            string sModule = "";
            if (module == "MM2H_NEW")
                sModule = "MM2H_NEW_DOKUMEN";
            else if (module == "MM2H_RENEW")
                sModule = "MM2H_RENEW_DOKUMEN";
            else if (module == "MM2H_ADDBRANCH")
                sModule = "MM2H_ADDBRANCH_DOKUMEN";
            else if (module == "MM2H_CHANGESTATUS")
                sModule = "MM2H_CHANGESTATUS_DOKUMEN";
            else if (module == "MM2H_CANCELLICENSE")
                sModule = "MM2H_CANCELLICENCE_DOKUMEN";

            int iCount = mM2HHelper.checkDokumenSokongan(AppId, sModule, userID);

            if (iCount == 0)
            {
                coreOrgHelper.updateListing(itemID);
            }
        }


        public void uploadDocSokongan(HttpPostedFileBase file, Guid chkitem_instance)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            //string contentType = file.ContentType;
            string folder = "/Attachment/" + filename;
            string foldersave = Server.MapPath("~/Attachment");
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            doc.FileName = file.FileName;
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.MM2H, filename); //folder;
            doc.chkitem_instanceID = chkitem_instance.ToString();
            doc.UploadFileName = filename;

            bool ID = coreOrgHelper.updateDocumentSokongan(doc);

            if (ID)
            {
                //use common upload in base contoller
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.MM2H);

            }
        }

        //added by samsuri (CR#58741)  on 26 jan 2024
        [HttpPost]
        public JsonResult ajaxChangeStatusBranchUpdate(core_organizations_updated orgUpdate, mm2h_add_branches_updated mm2hUpdate,
            HttpPostedFileBase file_PerjanjianSewaBeliPermis, HttpPostedFileBase file_PelanLantaiPermis,
            HttpPostedFileBase file_salinanAsalLesen, HttpPostedFileBase file_GambarPermis, string ItemID, string branch_stub_ref)
        {
            string userID = Session["UID"].ToString();

            Guid gUserID = Guid.Parse(userID);
            var is_premise = orgUpdate.is_premise_ready;
            //mm2hUpdate.stub_ref = Guid.Parse(branch_stub_ref);
            
            TourlistUnitOfWork.MM2HAddBranchesUpdatedRepository.SaveNewMM2HBranch(mm2hUpdate, gUserID);

            bool upd = coreOrgHelper.UpdateChangeStatusOrgforBranchInd(orgUpdate, gUserID);

            Guid chkitem_instance = Guid.Empty;

            Guid branch_stub_ref_idx = Guid.Parse(branch_stub_ref);            
            uploadBranchUpdatedDoc(branch_stub_ref_idx, gUserID);

            //WriteLog("upd:" + upd + " is_premise:" + is_premise                     
            //        + " branch_stub_ref_idx:" + branch_stub_ref_idx
            //        + " mm2hUpdate.stub_ref:"+ mm2hUpdate.stub_ref);

            if (upd == true)
            {

                if (is_premise == 0)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "mm2h"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "mm2h"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "mm2h"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                }
                else
                {
                    if (file_PerjanjianSewaBeliPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "mm2h"); //"Perjanjian Sewa Beli Premis"
                        uploadDocSokongan(file_PerjanjianSewaBeliPermis, chkitem_instance);
                        //WriteLog("chkitem_instance SEWABELI:" + chkitem_instance);
                    }
                    if (file_PelanLantaiPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "mm2h");//"Pelan Lantai Premis Perniagaan"
                        uploadDocSokongan(file_PelanLantaiPermis, chkitem_instance);
                    }
                    if (file_GambarPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "mm2h");//"Salinan Lesen *"
                        uploadDocSokongan(file_GambarPermis, chkitem_instance);
                    }
                }
                if (file_salinanAsalLesen != null)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SALINANLESEN", branch_stub_ref_idx, "mm2h");//"Salinan Lesen *"
                    uploadDocSokongan(file_salinanAsalLesen, chkitem_instance);
                }
                bool itemID = coreOrgHelper.updateListing(ItemID);
            }

            

            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#58741)  on 26 jan 2024
        private void uploadBranchUpdatedDoc(Guid branch_stub_ref_idx, Guid userIdx)
        {

            //make sure supporting_document_list has value or generate new one.
            var mm2hBranchLic = TourlistUnitOfWork.MM2HLicensesRepository.GetMM2HLicenseByStubRef(branch_stub_ref_idx);
            var TT_supporting_document_list = Guid.Empty;
            if (mm2hBranchLic.supporting_document_list == null)
            {
                TT_supporting_document_list = TourlistUnitOfWork.MM2HLicensesRepository.UpdateSupportingDocList(mm2hBranchLic.mm2h_idx);
            }
            else TT_supporting_document_list = (Guid)mm2hBranchLic.supporting_document_list;

            Guid chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "mm2h");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = coreOrgHelper.GetcoreChkitemsInstancesByCode("SEWABELI", "mm2h");//Guid.Parse("ABD910AE-66E5-46C7-953C-7D5F89D9CFC2"); //SEWABELI                
                
                core_Chkitems_Instances.chklist_instance_ref = TT_supporting_document_list;
                core_Chkitems_Instances.bool1 = 0;
                core_Chkitems_Instances.active_status = 1;
                core_Chkitems_Instances.created_at = DateTime.Now;
                core_Chkitems_Instances.modified_at = DateTime.Now;
                core_Chkitems_Instances.created_by = userIdx;
                core_Chkitems_Instances.modified_by = userIdx;
                TourlistUnitOfWork.CoreChkItemsInstancesRepository.Add(core_Chkitems_Instances);
                TourlistContext.SaveChanges();
                
            }

            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "mm2h");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = coreOrgHelper.GetcoreChkitemsInstancesByCode("PELANLANTAI", "mm2h");//Guid.Parse("BE7F9E08-936A-4964-A817-E8762B639C9F"); //PELANLANTAI                
                core_Chkitems_Instances.chklist_instance_ref = TT_supporting_document_list;
                core_Chkitems_Instances.bool1 = 0;
                core_Chkitems_Instances.active_status = 1;
                core_Chkitems_Instances.created_at = DateTime.Now;
                core_Chkitems_Instances.modified_at = DateTime.Now;
                core_Chkitems_Instances.created_by = userIdx;
                core_Chkitems_Instances.modified_by = userIdx;
                TourlistUnitOfWork.CoreChkItemsInstancesRepository.Add(core_Chkitems_Instances);
                TourlistContext.SaveChanges();
            }

            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "mm2h");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = coreOrgHelper.GetcoreChkitemsInstancesByCode("GAMBAR", "mm2h");//Guid.Parse("7BC79CDE-4CC8-42B4-B626-D66A80E1A173"); //GAMBAR                
                core_Chkitems_Instances.chklist_instance_ref = TT_supporting_document_list;
                core_Chkitems_Instances.bool1 = 0;
                core_Chkitems_Instances.active_status = 1;
                core_Chkitems_Instances.created_at = DateTime.Now;
                core_Chkitems_Instances.modified_at = DateTime.Now;
                core_Chkitems_Instances.created_by = userIdx;
                core_Chkitems_Instances.modified_by = userIdx;
                TourlistUnitOfWork.CoreChkItemsInstancesRepository.Add(core_Chkitems_Instances);
                TourlistContext.SaveChanges();
            }

        }


        public void RemoveDocList(Guid chkitem_instance)
        {
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            doc.FileName = null;
            doc.UploadLocation = null; //folder;
            doc.chkitem_instanceID = chkitem_instance.ToString();
            doc.UploadFileName = null;

            bool ID = coreOrgHelper.updateDocumentSokongan(doc);
        }


        [HttpPost]
        public JsonResult ajaxCheckCompanyDataSSM()
        {

            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TourlistWebAPI.Models.MM2HModels.mm2h_ssm_organization();
            // List<core_organizations> Company = new List<core_organizations>();

            var Company = unitOfWork.CoreOrganizations.Find(c => (c.organization_identifier == Request["company_regno"])).FirstOrDefault();

            string sChecking = "";
            if (model_org.company_name == Company.organization_name)
            {
                sChecking = model_org.company_name + "<BR >";
            }

            if (model_org.companyb_addr1 == Company.office_addr_1)
            {
                sChecking = sChecking + model_org.company_name + "<BR >";
            }
            if (model_org.companyb_addr2 == Company.office_addr_2)
            {
                sChecking = sChecking + model_org.companyb_addr2 + "<BR >";
            }

            if (model_org.companyb_addr3 == Company.office_addr_3)
            {
                sChecking = sChecking + model_org.companyb_addr3 + "<BR >";
            }
            if (model_org.companyb_postcode == Company.office_postcode)
            {
                sChecking = sChecking + model_org.companyb_postcode + "<BR >";
            }

            return Json(sChecking, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult ajaxCheckCompanyDirectorsSSM()
        {

            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TourlistWebAPI.Models.MM2HModels.mm2h_ssm_directors();
            var Application = "";

            if (Request["director_name"] != null)
            {

                model_org.director_name = Request["director_name"].ToString();
                model_org.director_docno = Request["director_docno"].ToString();
                model_org.director_addr1 = Request["director_addr1"].ToString();
                model_org.director_addr2 = Request["director_addr2"].ToString();
                model_org.director_addr3 = Request["director_addr3"].ToString();
                model_org.director_postcode = Request["director_postcode"].ToString();
                model_org.director_town = Request["director_town"].ToString();
               // model_org.director_date_appointed = Request["director_date_appointed"].ToString();
                var dDateOfappointed = Request["director_date_appointed"].ToString();
                model_org.director_date_appointed = Request["director_date_appointed"] == null ? DateTime.Parse("1970-01-01") : DateTime.ParseExact(Request["director_date_appointed"].ToString(), "dd/MM/yyyy", null);
                
                model_org.director_designation = Request["director_designation"].ToString();
                model_org.director_idType = Request["director_idType"].ToString();
                Application = mM2HHelper.ajaxUpdateCompanyDirectorsSSM(module_id, component_id, model_org);
            }


            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxCheckCompanyShareholdersSSM()
        {

            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TourlistWebAPI.Models.MM2HModels.mm2h_ssm_shareholders();
            var Application = "";

            if (Request["shareholder_name"] != null)
            {

                model_org.shareholder_name = Request["shareholder_name"].ToString();
                model_org.shareholder_docno = Request["shareholder_docno"].ToString();
                model_org.shareholder_totalshare = Request["shareholder_totalshare"].ToString();
                model_org.shareholder_idType = Request["shareholder_idtype"].ToString();

                Application = mM2HHelper.ajaxUpdateCompanyShareholdersSSM(module_id, component_id, model_org);
            }


            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxDirectorDetail(string identifier)
        {
            var directors = coreOrgHelper.GetDirectorDetailByDirectorID(identifier);
            return Json(directors, JsonRequestBehavior.AllowGet);
        }

        public JsonResult findDirectorDetail(string identifier)
        {
            var idx = unitOfWork.CorePersonsRepository.Find(x => x.person_identifier == identifier).Select(x => x.person_idx).FirstOrDefault().ToString();

            if (idx != "00000000-0000-0000-0000-000000000000")
            {
                var directors = coreOrgHelper.GetDirectorDetailByDirectorID(idx);
                return Json(directors, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        //Change Status Company
        [HttpPost]
        public JsonResult ajaxChangeStatusUpdate(core_organizations_updated orgUpdate, HttpPostedFileBase file_PerjanjianSewaBeliPermis,
            HttpPostedFileBase file_PelanLantaiPermis, HttpPostedFileBase file_salinanAsalLesen, HttpPostedFileBase file_GambarPermis, string ItemID)
        {
            string userID = Session["UID"].ToString();

            Guid gUserID = Guid.Parse(userID);
            var is_premise = orgUpdate.is_premise_ready;


            bool upd = coreOrgHelper.UpdateChangeStatusOrg(orgUpdate, gUserID);
            Guid chkitem_instance = Guid.Empty;

            if (upd == true)
            {

                if (is_premise == 0)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", orgUpdate.stub_ref, "mm2h"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", orgUpdate.stub_ref, "mm2h"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", orgUpdate.stub_ref, "mm2h"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                }
                else
                {
                    if (file_PerjanjianSewaBeliPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", orgUpdate.stub_ref, "mm2h"); //"Perjanjian Sewa Beli Premis"
                        uploadDocSokongan(file_PerjanjianSewaBeliPermis, chkitem_instance);
                    }
                    if (file_PelanLantaiPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", orgUpdate.stub_ref, "mm2h");//"Pelan Lantai Premis Perniagaan"
                        uploadDocSokongan(file_PelanLantaiPermis, chkitem_instance);
                    }
                    if (file_GambarPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", orgUpdate.stub_ref, "mm2h");//"Salinan Lesen *"
                        uploadDocSokongan(file_GambarPermis, chkitem_instance);
                    }
                }
                if (file_salinanAsalLesen != null)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SALINANLESEN", orgUpdate.stub_ref, "mm2h");//"Salinan Lesen *"
                    uploadDocSokongan(file_salinanAsalLesen, chkitem_instance);
                }
                bool itemID = coreOrgHelper.updateListing(ItemID);


            }



            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        public void uploadChangeStatus(HttpPostedFileBase file, string chkitem_instance)
        {

            //var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            //string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            //string folder = "/Attachment/" + filename;
            //string foldersave = Server.MapPath("~/Attachment");
            //CoreOrganizationModel.CoreOtherDocument doc = new CoreOrganizationModel.CoreDocument();

            //var userID = Session["UID"].ToString();
            //doc.FileName = file.FileName;
            //doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.MM2H, filename); //folder;
            //doc.PersonID = PersonID;
            //doc.UploadFileName = filename;
            //doc.type = typeFile;
            //doc.userID = userID;

            //bool ID = coreOrgHelper.updateDocumentSokonganPerson(doc);

            //if (ID)
            //{               
            //    this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.MM2H);              

            //}
        }
        //Change Status Shareholder
        [HttpPost]
        public JsonResult ajaxChangeStatusShareholder(Guid orgID, Guid appID)
        {
            string userID = Session["UID"].ToString();
            Guid gUserID = Guid.Parse(userID);

            //coreOrgHelper.ChangeStatusOrganization_SaveNew(orgID, appID, gUserID);
            coreOrgHelper.ChangeStatusShareHolder_SaveNew(orgID, appID, gUserID);

            bool upd = true;
            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxChangeStatusShareHolderBaru(string AppID)
        {
            var ShareHolder = coreOrgHelper.GetChangeStatusShareHolder(AppID);

            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxChangeStatusShareholderPersonUpdate(core_persons obj, string status_shareholder, decimal number_of_shares, string justification,
            string justificationID, HttpPostedFileBase file_MyKad, string typeFile, HttpPostedFileBase file_JustPerson, string typeFileJust, Guid AppID)
        {

            var userID = Session["UID"].ToString();
            obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.ChangeStatusUpdatePerson(obj, status_shareholder, number_of_shares, "Shareholder", justification, justificationID);


            if (ID == true)
            {
                //    bool itemID = coreOrgHelper.updateListing(ItemID);
                //  bool itemID = coreOrgHelper.updateStatusShareHolder(status_shareholder, shareholderID, "");

                if (file_MyKad != null)
                {
                    uploadPersonID(file_MyKad, obj.person_idx.ToString(), typeFile);
                }

                if (file_JustPerson != null)
                {
                    uploadPersonID(file_JustPerson, obj.person_idx.ToString(), typeFileJust);
                }



                coreOrgHelper.UpdateChangeStatusIS(Guid.Parse(userID), "shareholder", AppID);


            }

            return Json(ID, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        public JsonResult ajaxChangeStatusShareholderOrgUpdate(core_organizations obj, string status_shareholder, string number_of_shares, string registered_year, string country_ref, string justification, string justificationID,
            HttpPostedFileBase file_Just, string typeFileJust, Guid AppID)
        {

            var userID = Session["UID"].ToString();
            obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.ChangeStatusUpdateOrganization(obj, status_shareholder, decimal.Parse(number_of_shares), registered_year, country_ref, userID, justification, justificationID);
            if (ID == true)
            {

                if (file_Just != null)
                {
                    uploadOrganizationID(file_Just, obj.organization_idx.ToString(), typeFileJust);
                }



                coreOrgHelper.UpdateChangeStatusIS(Guid.Parse(userID), "shareholder", AppID);

            }
            return Json(ID, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxChangeStatusShareholderOrgAdd(core_organizations obj, string status_shareholder, decimal number_of_shares, string registered_year, string country_ref, Guid AppID)
        {

            var userID = Session["UID"].ToString();
            //obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.ChangeStatusUpdateOrganization_SaveNew(obj, status_shareholder, number_of_shares, registered_year, country_ref, userID, AppID);
            return Json(ID, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxShareHolderChangeStatusOrgDetail(Guid IDX)
        {
            var ShareHolder = coreOrgHelper.GetOrgChangeStatusDetail(IDX);
            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }

        //Change Status Director
        [HttpPost]
        public JsonResult ajaxChangeStatusDirector(Guid orgID, Guid appID)
        {
            string userID = Session["UID"].ToString();
            Guid gUserID = Guid.Parse(userID);


            coreOrgHelper.ChangeStatusDirector_SaveNew(orgID, appID, gUserID);

            bool upd = true;
            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxChangeStatusDirectorDetail(string IDX)
        {
            //var directors = coreOrgHelper.GetDirectorDetail(IDX);
            var directors = coreOrgHelper.GetChangeStatusDirectorDetail(IDX);

            return Json(directors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxChangeStatusPersonDetail(string IDX)
        {

            var ShareHolder = coreOrgHelper.GetChangeStatusShareholderDetail(IDX);


            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ajaxChangeStatusDirectorUpdate(core_persons obj, HttpPostedFileBase file_MyKad, HttpPostedFileBase file_MyPengajian, HttpPostedFileBase file_JustDirector,
            string typeFile, string justification, string justificationID, Guid AppID)
        {
            var userID = Session["UID"].ToString();
            obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.ChangeStatusUpdatePerson(obj, "", 0, "Director", justification, justificationID);


            if (ID == true)
            {
                if (file_MyKad != null)
                {
                    uploadPersonID(file_MyKad, obj.person_idx.ToString(), typeFile);
                }
                
                if (file_MyPengajian != null)
                {
                    uploadPersonID(file_MyPengajian, obj.person_idx.ToString(), "PAS_PENGGAJIAN");
                }

                if (file_JustDirector != null)
                {
                    uploadPersonID(file_JustDirector, obj.person_idx.ToString(), "JUSTIFICATION");
                }

                coreOrgHelper.UpdateChangeStatusIS(Guid.Parse(userID), "director", AppID);

            }
            return Json(ID, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult ajaxChangeStatusPersonAdd(core_persons obj, string status_shareholder, HttpPostedFileBase file_MyKad, HttpPostedFileBase file_PassPengajian, decimal number_of_shares, Guid OrgID, Guid AppID, string type, string typeFile, string upload_name, string file_name, string shareholderID_add)
        {

            var userID = Session["UID"].ToString();
            Guid Idx = Guid.NewGuid();
            obj.person_idx = Idx;
            obj.modified_by = Guid.Parse(userID);
            bool file = false;
            if (file_MyKad == null)
            {
                file = true;
            }

            bool ID = coreOrgHelper.ChangeStatusUpdatePerson_SaveNew(obj, status_shareholder, number_of_shares, AppID, OrgID, Guid.Parse(userID), type, file, shareholderID_add);

            if (ID == true)
            {
                //    bool itemID = coreOrgHelper.updateListing(ItemID);
                //  bool itemID = coreOrgHelper.updateStatusShareHolder(status_shareholder, shareholderID, "");

                if (file_MyKad != null)
                {
                    uploadPersonID(file_MyKad, Idx.ToString(), typeFile);
                }

                if (file_PassPengajian != null)
                {
                    uploadPersonID(file_PassPengajian, Idx.ToString(), "PASS_PENGGAJIAN");
                }

                coreOrgHelper.UpdateChangeStatusIS(Guid.Parse(userID), type, AppID);

            }
            return Json(ID, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult ajaxChangeStatusShareholderAdd(core_persons obj, string status_shareholder, HttpPostedFileBase file_MyKad, decimal number_of_shares, Guid OrgID, Guid AppID, string type, string filename)
        {

            var userID = Session["UID"].ToString();
            Guid Idx = Guid.NewGuid();
            obj.person_idx = Idx;
            // Guid OrgID = Guid.Empty;
            obj.modified_by = Guid.Parse(userID);

            bool ID = coreOrgHelper.ChangeStatusUpdatePerson_SaveNew(obj, status_shareholder, number_of_shares, AppID, OrgID, Guid.Parse(userID), type, false, "");

            if (ID == true)
            {
                //    bool itemID = coreOrgHelper.updateListing(ItemID);
                //  bool itemID = coreOrgHelper.updateStatusShareHolder(status_shareholder, shareholderID, "");

                if (file_MyKad != null)
                {
                    uploadPersonID(file_MyKad, Idx.ToString(), "MYKAD");


                }

                coreOrgHelper.UpdateChangeStatusIS(Guid.Parse(userID), "shareholder", AppID);

            }
            return Json(ID, JsonRequestBehavior.AllowGet);

        }




        [HttpPost]
        public JsonResult ajaxChangeStatusShareHolderDelete(string idx)
        {

            bool ID = coreOrgHelper.ChangeStatusUpdateShareholder_Delete(idx);
            return Json(ID, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ajaxChangeStatusDirectorDelete(string idx)
        {

            bool ID = coreOrgHelper.ChangeStatusUpdateDirector_Delete(idx);

            return Json(ID, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ajaxChangeStatusDirectorBaru(string AppID)
        {
            var Director = coreOrgHelper.GetChangeStatusDirector(AppID);

            return Json(Director, JsonRequestBehavior.AllowGet);
        }



        public void ChangeStatusDirectorBaru(HttpPostedFileBase file, string PersonID, string typeFile)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            string folder = "/Attachment/" + filename;
            string foldersave = Server.MapPath("~/Attachment");
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            var userID = Session["UID"].ToString();
            doc.FileName = file.FileName;
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.MM2H, filename); //folder;
            doc.PersonID = PersonID;
            doc.UploadFileName = filename;
            doc.type = typeFile;
            doc.userID = userID;

            bool ID = coreOrgHelper.updateDocumentSokonganPerson(doc);
        }

        public DateTime JsonDateTimeToNormal(string jsonDateTime)
        {
            jsonDateTime = @"""" + jsonDateTime + @"""";
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(jsonDateTime);
        }

        [HttpPost]
        public JsonResult ajaxPBT(string NegeriID)
        {

            var PBT = coreOrgHelper.GetRefPBT(NegeriID);

            return Json(PBT, JsonRequestBehavior.AllowGet);
        }
    }
}