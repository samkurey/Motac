using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tourlist.Common;
using TourlistDataLayer.DataModel;
using TourlistDataLayer.ViewModels.Common;
using TourlistDataLayer.ViewModels.Tobtab;
using TourlistWeb.Filters;
using TourlistWebAPI.ClassLib;
using TourlistWebAPI.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static TourlistDataLayer.ViewModels.Common.ApplicationDetailViewModel;
using TourlistBusinessLayer.Models;

namespace TourlistWeb.Controllers
{
    [SessionTimeout]
    public class TobtabController : TourlistController
    {
        // GET: Tobtab

        TourlistWebAPI.ClassLib.TobtabHelper tobtabHelper = new TourlistWebAPI.ClassLib.TobtabHelper();
        //TourlistWebAPI.ClassLib.RefCountryHelper refcountryHelper = new TourlistWebAPI.ClassLib.RefCountryHelper();
        TourlistWebAPI.ClassLib.RefGeoHelper refGeoHelper = new TourlistWebAPI.ClassLib.RefGeoHelper();

        TourlistWebAPI.DataModels.tobtab_main tb_main_model = new TourlistWebAPI.DataModels.tobtab_main();

        TourlistBusinessLayer.BLL.TobtabBLL tobtabBLL = new TourlistBusinessLayer.BLL.TobtabBLL();
        TourlistWebAPI.ClassLib.CoreHelper coreHelper = new TourlistWebAPI.ClassLib.CoreHelper();
        TourlistWebAPI.ClassLib.CoreOrganizationHelper coreOrgHelper = new TourlistWebAPI.ClassLib.CoreOrganizationHelper();
        RefHelper refHelper = new RefHelper();

        public ActionResult Dashboard(string btn, string module, string AppID, string AppNo, string AddBranchStatus)
        {
            if (module == null)
            {
                module = "TOBTAB_NEW";
            }

            ViewBag.module = module;
            string userID = Session["UID"].ToString();
            string chkitemInstanceIdx = Request["item"];
            //ViewBag.MenuStatus = tobtabBLL.GetActiveMenu(Guid.Parse(userID));
            getCoreOrganizationDetail(userID);

            string smodule = "";
            string sstatus = "";
            string sAppID = "";
            string status_code = "";

            List<TobtabViewModels.tobtab_application> app = new List<TobtabViewModels.tobtab_application>();
            var organization = tobtabBLL.GetOrganization(userID);
            app = tobtabBLL.GetApplicationStatusMain(userID, module, organization.organization_idx);

            ViewBag.Hantar = false;
            List<SelectListItem> Pengarah = new List<SelectListItem>();
            if (app.Count > 0)
            {
                foreach (var st in app)
                {
                    sstatus = st.status.ToString();
                    smodule = st.module_name.ToString();
                    sAppID = st.apply_idx.ToString();
                    status_code = st.status_code.ToString();
                }
                ViewBag.NewStatus = "false";
            }
            else
            {
                ViewBag.NewStatus = "true";
            }
            if (AddBranchStatus != null)
                status_code = AddBranchStatus;
            string sstatus1 = "";
            string sModule1 = "";
            string sAppNo = "";
            List<TobtabViewModels.tobtab_application> latestApp = new List<TobtabViewModels.tobtab_application>();
            latestApp = tobtabBLL.GetApplicationStatusMain(userID, null, organization.organization_idx);
            if (latestApp.Count > 0)
            {
                foreach (var st in latestApp)
                {
                    sstatus1 = st.status_code.ToString();
                    sModule1 = st.module_name.ToString();
                }
            }
            
            if (module == "TOBTAB_ADD_BRANCH")
            {
                if (AppID != null)
                {
                    latestApp = tobtabBLL.GetApplicationStatusByAppID(Guid.Parse(AppID));
                }                
                if (latestApp.Count > 0)
                {
                    foreach (var st in latestApp)
                    {
                        sstatus1 = st.status_code.ToString();
                        sModule1 = st.module_name.ToString();
                        ViewBag.AppNo = st.application_no;
                        if (st.application_no != null)
                            sAppNo = st.application_no.ToString();
                        else
                            sAppNo = st.application_no;
                        sAppID = st.apply_idx.ToString();
                    }
                }
            }
                        
            if (AppNo != null)
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
            List<TobtabViewModels.tobtab_application> tobtabnew = new List<TobtabViewModels.tobtab_application>();
            tobtabnew = tobtabBLL.GetApplicationStatusMain(userID, "TOBTAB_NEW", organization.organization_idx);
            if (tobtabnew.Count > 0)
            {
                ViewBag.tobtab_new = true;
            }
            else
            {
                ViewBag.tobtab_new = false;
            }
            ViewBag.endDatePopup = "0";
            ViewBag.renewPopup = "0";
            var coreLicense = tobtabBLL.GetCoreLicens(organization.organization_idx.ToString());
            if (coreLicense == null)
            {
                coreLicense = new core_license();
            }
            else if (coreLicense!=null && coreLicense.end_dt == null)
            {
                coreLicense = new core_license();
            }
            else
            {       
                ViewBag.ActiveStatus = coreLicense.active_status.ToString();

                DateTime endDate = (DateTime)coreLicense.end_dt;
                DateTime currentDate = DateTime.Now;
                double days = (endDate - currentDate).TotalDays;
                if (days < 183)
                {
                    ViewBag.endDatePopup = "1";
                }
                if (days > 90)
                {
                    ViewBag.renewPopup = "1";
                }
            }

            ViewBag.coreLicense = coreLicense;
            ViewBag.SSMNo = organization.organization_identifier;
            ViewBag.StatusCode = status_code;
            ViewBag.LatestStatus = sstatus1;
            ViewBag.LatestModule = sModule1;
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
           // ViewBag.AppID = sAppID;
            Session["AppID"] = sAppID;
            ViewBag.OrganizationIdx = organization.organization_idx;
            ViewBag.Organization = organization;
            ViewBag.tobtab_renew = tobtabBLL.GetTobtabStatusModul(organization.organization_idx.ToString(), "TOBTAB_RENEW");
            ViewBag.tobtab_branch = tobtabBLL.GetBranchList(organization.organization_idx.ToString()).Count(); 
            if(ViewBag.tobtab_branch == 0)
            {
                ViewBag.tobtab_branch = tobtabBLL.GetTobtabStatusModul(organization.organization_idx.ToString(), "TOBTAB_ADD_BRANCH");
            }
            ViewBag.tobtab_new = tobtabBLL.GetTobtabStatusModul(organization.organization_idx.ToString(), "TOBTAB_NEW");
            ViewBag.tobtab_field = tobtabBLL.GetTobtabStatusModul(organization.organization_idx.ToString(), "TOBTAB_ADD_FIELD");
            //ViewBag.tg_exemption_status = tobtabBLL.GetTobtabStatusModulStatus(organization.organization_idx.ToString(), "TOBTAB_TG_EXCEPTIONS", "COMPLETED");
            ViewBag.tg_exemption_status = tobtabBLL.GetTobtabStatusModulStatus(organization.organization_idx.ToString(), "TOBTAB_TG_EXCEPTIONS", "DRAFT");

            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            ViewBag.TgExceptionInfo = new tobtab_tg_exceptions();
            Session["umrah"] = false;
            Pengarah = GetPengarahDropDown(organization.organization_idx.ToString(), ViewBag.License.tobtab_idx.ToString());
            if (smodule == "TOBTAB_NEW")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_NEW_MOHON", status_code);
                ViewBag.PageTitle = "Lesen Baru";
            }
            else if (smodule == "TOBTAB_RENEW")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_RENEW_MOHON", status_code);
                ViewBag.PageTitle = "Pembaharuan Lesen";
                var bidang = tobtabBLL.GetCoreLicens(organization.organization_idx.ToString());
                if (bidang != null && bidang.inbound == 1 && bidang.outbound == 1 && bidang.ticketing == 1)
                {
                    Session["umrah"] = true;
                }
            }
            else if (smodule == "TOBTAB_ADD_BRANCH")
            {
                if (AppID == null)
                {
                    
                    Session["AppID"] = sAppID;
                    ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_ADD_BRANCH", status_code);
                }
                    
                else
                {
                    Session["AppID"] = AppID;
                    ViewBag.Status = sstatus1;
                    ViewBag.DashBoardList = tobtabBLL.GetDashboardList(AppID, "TOBTAB_ADD_BRANCH", status_code);
                }
                    
                ViewBag.PageTitle = "Tambah Cawangan";
            }
            else if (smodule == "TOBTAB_CHANGE_STATUS")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_CHANGE_STATUS", status_code);
                ViewBag.PageTitle = "Tukar Status";
            }
            else if (smodule == "TOBTAB_RETURN_LICENSE")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_RETURN_LICENSE", status_code);
                ViewBag.PageTitle = "Serah Batal Lesen";
            }
            else if (smodule == "TOBTAB_TG_EXCEPTIONS")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_TG_EXCEPTIONS", status_code);
                ViewBag.PageTitle = "Pengecualian TG";
                ViewBag.TgExceptionInfo = tobtabBLL.getTgExceptionInfoByGuid(ViewBag.License.tobtab_idx);

            }
            else if (smodule == "TOBTAB_UMRAH")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_UMRAH", status_code);
                ViewBag.PageTitle = "Umrah";
            }
            else if (smodule == "TOBTAB_MARKETING_OFFICER")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_MARKETING_OFFICER", status_code);
                ViewBag.PageTitle = "Pegawai Pemasaran";
            }
            else if (smodule == "TOBTAB_ADD_FIELD")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_ADD_FIELD", status_code);
                ViewBag.PageTitle = "Tambah Bidang";
                Session["ticketing"] = 0;
                Session["inbound"] = 0;
                Session["outbound"] = 0;
                var bidang = tobtabBLL.GetCoreLicens(organization.organization_idx.ToString());
                if (bidang != null && bidang.inbound == 1 && bidang.outbound == 1 && bidang.ticketing == 1)
                {
                    Session["ticketing"] = 1;
                    Session["inbound"] = 1;
                    Session["outbound"] = 1;
                    Session["umrah"] = true;
                }
                else if (bidang != null)
                {
                    Session["ticketing"] = bidang.ticketing;
                    Session["inbound"] = bidang.inbound;
                    Session["outbound"] = bidang.outbound;
                    Session["umrah"] = false;
                }
            }

            bool checktambahbidang= checkTambahBidang();
            if (checktambahbidang == true)
                ViewBag.checkTambahBidang = "true";
            else
                ViewBag.checkTambahBidang = "false";

            if (ViewBag.DashBoardList != null)
            {
                Boolean isBranch = false;
                string SSM = "false";
                int iCountItem = 0;
                foreach (var item in ViewBag.DashBoardList)
                {
                    if (item.descr_bool1 == "SSM" && item.bool1 == "1")
                    {
                        SSM = "true";
                    }
                    else if (item.descr_bool1 == "SSM" && item.bool1 == "0")
                    {
                        SSM = "false";
                    }
                    else
                    {
                        SSM = "nothing";
                    }
                    if (ViewBag.License.outbound == 0 && item.descr_string3 == "Outbound")
                    {
                        item.descr_bool1 = "1";
                    }

                    if (item.bool1 == "0")
                    {
                        if (sModule1 == "TOBTAB_RENEW")
                        {
                            List<TobtabViewModels.tobtab_branch> branchLst = tobtabBLL.GetBranchList(organization.organization_idx.ToString());
                            if (branchLst != null && branchLst.Count > 0)
                            {
                                iCountItem++;
                                isBranch = true;
                            }
                            else
                            {
                                iCountItem++;
                            }
                        }
                        else
                        {
                            iCountItem++;
                        }
                    }
                }
                ViewBag.SSM = SSM;
                if (iCountItem == 0 && sModule1 == "TOBTAB_RENEW" && !isBranch)
                {
                    ViewBag.Hantar = true;
                }else if (iCountItem == 0 && sModule1 != "TOBTAB_RENEW") {
                    ViewBag.Hantar = true;
                }
            }

            ViewData["Pengarah_"] = Pengarah;
            List<SelectListItem> Negeri = new List<SelectListItem>();

            List<SelectListItem> Lesen = new List<SelectListItem>();
            Lesen = GetLesenDropDown(userID);
            ViewData["Lesen_"] = Lesen;


            List<SelectListItem> JustifikasiBatal = new List<SelectListItem>();
            JustifikasiBatal = GetJustifikasiSerahLesenDropDown();
            ViewData["JustifikasiBatalLesen_"] = JustifikasiBatal;

            List<SelectListItem> TempohPembaharuan = new List<SelectListItem>();
            TempohPembaharuan = GetTahun();
            ViewData["TempohPembaharuan_"] = TempohPembaharuan;

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
           


            return View();
        }

        public JsonResult PenganjurCommon(string applyId)
        {
            Penganjur peng = new Penganjur();
            flow_application_stubs iduser = TourlistUnitOfWork.FlowApplicationStubs.Find(x => x.apply_idx.ToString() == applyId).FirstOrDefault();
            var Org = tobtabBLL.GetOrganization(iduser.apply_user.ToString());
            var coreLicense = tobtabBLL.GetCoreLicens(Org.organization_idx.ToString());
            tobtab_licenses licenses = TourlistUnitOfWork.TobtabLicenses.Find(x => x.stub_ref.ToString() == applyId).FirstOrDefault();
            var Tg = tobtabBLL.getTgExceptionInfoByGuid(licenses.tobtab_idx);
            peng.TgExceptionInfo_company_representative_name = Tg.company_representative_name;
            peng.Organization_office_email = Org.office_email;
            peng.TgExceptionInfo_company_representative_phone_no = Tg.company_representative_phone_no;
            peng.Organization_organization_identifier = Org.organization_identifier;
            peng.Organization_organization_name = Org.organization_name;
            peng.Organization_office_addr_1 = Org.office_addr_1;
            peng.Organization_office_addr_2 = Org.office_addr_2;
            peng.Organization_office_addr_3 = Org.office_addr_3;
            peng.Organization_office_postcode = Org.office_postcode;
            peng.TgExceptionInfo_bus_owner = Tg.bus_owner;
            peng.TgExceptionInfo_bus_owner_kpl_licenses = Tg.bus_owner_kpl_licenses;
            peng.TgExceptionInfo_bus_no_plate = Tg.bus_no_plate;
            peng.TgExceptionInfo_bus_seat_load = Tg.bus_seat_load;
            peng.TgExceptionInfo_bus_total_passenger = Tg.bus_total_passenger;
            peng.TgExceptionInfo_organizer_name = Tg.organizer_name;
            peng.TgExceptionInfo_organizer_mobile_no = Tg.organizer_mobile_no;
            peng.TgExceptionInfo_organizer_addr_1 = Tg.organizer_addr_1;
            peng.TgExceptionInfo_organizer_email = Tg.organizer_email;
            peng.TgExceptionInfo_organizer_addr_2 = Tg.organizer_addr_2;
            peng.TgExceptionInfo_organizer_addr_3 = Tg.organizer_addr_3;
            peng.TgExceptionInfo_organizer_postcode = Tg.organizer_postcode;
            peng.CoreLicense_license_no = coreLicense != null ? coreLicense.license_no : "-";
            return Json(peng, JsonRequestBehavior.AllowGet);
        }

        #region functions

        public void getCoreOrganizationDetail(string userID)
        {
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
            ViewBag.OrganizationIdx = OrganizationID;
            ViewBag.NoPendaftaranSyarikat = NoPendaftaranSyarikat;
            Session["CompRegNo"] = NoPendaftaranSyarikat;
            List<TourlistDataLayer.DataModel.core_license> clsLicense = new List<TourlistDataLayer.DataModel.core_license>();
            clsLicense = coreOrgHelper.GetCoreLicenseNo(OrganizationID,"TOBTAB");
            foreach (var item in clsLicense)
            {
                ViewBag.LicenseNo = item.license_no;
                string sdate_start = String.Format("{0:dd/MM/yyyy}", item.start_dt);
                string sdate_end = String.Format("{0:dd/MM/yyyy}", item.end_dt);
                ViewBag.DateStart = sdate_start + " - " + sdate_end;
                string field = "";
                int i = 0;
                if (item.inbound == 1)
                {
                    field = "Inbound";
                    i = 1;
                }
                if (item.outbound == 1)
                {
                    if (i == 1)
                    {
                        field = field + " , " + "Outbound";
                    }
                    else
                    {
                        field = "Outbound";
                    }
                    i = 1;
                }
                if (item.ticketing == 1)
                {
                    if (i == 1)
                    {
                        field = field + " , " + "Ticketing";
                    }
                    else
                    {
                        field = "Ticketing";
                    }
                }
                if (item.umrah == 1)
                {
                    if (i == 1)
                    {
                        field = field + " , " + "Umrah";
                    }
                    else
                    {
                        field = "Umrah";
                    }
                }
                ViewBag.LicenseField = field.ToUpper();
            }
        }

        public bool checkTambahBidang()
        {
            string userID = Session["UID"].ToString();
            bool latestApp = tobtabBLL.GetCheckingBidang(userID);
            return latestApp;
        }
        #endregion

        #region new

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
        [HttpPost]
        public JsonResult ajaxPerakuan(string PersonID)
        {
            var Director = tobtabBLL.getDirectorInfoByPerson(PersonID);
            //var Director = coreOrgHelper.GetDirectorPerakuanDetail(PersonID);

            return Json(Director, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Shareholder(string module, string btn)
        {
            ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();

            string smodule = module;
            string sstatus = "";
            string sAppID = "";
            string status_code = "";

            List<TobtabViewModels.tobtab_application> app = new List<TobtabViewModels.tobtab_application>();
            var organization = tobtabBLL.GetOrganization(userID);
            app = tobtabBLL.GetApplicationStatusMain(userID, module, organization.organization_idx);
            //app = tobtabBLL.GetTobtabStatus(userID, smodule);

            foreach (var st in app)
            {
                sstatus = st.status.ToString();
                smodule = st.module_name.ToString();
                sAppID = st.apply_idx.ToString();
                status_code = st.status_code.ToString();
            }
            ViewBag.StatusCode = status_code;
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
            ViewBag.AppID = sAppID;

            //ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            ViewBag.Company = tobtabBLL.GetOrganization(userID);
            ViewBag.SSMNo = ViewBag.Company.organization_identifier;
            getPageTitle(smodule);
            //ViewBag.checklistItem = tobtabBLL.GetChecklistItem(sAppID, "75A5F309-7CDD-42E8-99EB-562AE9D0D2E2", smodule);

            List<SelectListItem> PemegangSahamPerson = new List<SelectListItem>();
            PemegangSahamPerson = GetStatusPemeganganSahamDropDown("Person");
            ViewData["StatusPemegangSahamPerson_"] = PemegangSahamPerson;

            List<SelectListItem> PemegangSahamOrganization = new List<SelectListItem>();
            PemegangSahamOrganization = GetStatusPemeganganSahamDropDown("Organization");
            ViewData["StatusPemegangSahamOrganization_"] = PemegangSahamOrganization;

            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;

            List<SelectListItem> ref_malaysia = new List<SelectListItem>();
            ref_malaysia = GetMalaysiaDropDown();
            ViewData["Malaysialist_"] = ref_malaysia;


            List<SelectListItem> Agama = new List<SelectListItem>();
            Agama = GetAgamaDropDown();
            ViewData["Agama_"] = Agama;

            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;

            List<SelectListItem> Asean = new List<SelectListItem>();
            Asean = GetAseanDropDown();
            ViewData["Aseanlist"] = Asean;

            List<SelectListItem> NonAsean = new List<SelectListItem>();
            NonAsean = GetNonAseanDropDown();
            ViewData["NonAseanlist_"] = NonAsean;

            List<SelectListItem> ref_country = new List<SelectListItem>();
            ref_country = GetCountryDropDown();
            ViewData["Countrylist_"] = ref_country;
            var Malaysia = ref_country.Where(d => d.Text.ToString() == "Malaysia").ToList();
            var MalaysiaVal = "";
            foreach (var item in Malaysia)
            {
                MalaysiaVal = item.Value.ToString();
            }

            ViewBag.Malaysia = MalaysiaVal;

            return View();
        }

        public void getPageTitle(string smodule)
        {
            if (smodule == "TOBTAB_NEW")
            {
                ViewBag.PageTitle = "Lesen Baru";
            }
            else if (smodule == "TOBTAB_RENEW")
            {
                ViewBag.PageTitle = "Pembaharuan Lesen";
            }
            else if (smodule == "TOBTAB_ADD_BRANCH")
            {
                ViewBag.PageTitle = "Tambah Cawangan";
            }
            else if (smodule == "TOBTAB_CHANGE_STATUS")
            {
                ViewBag.PageTitle = "Tukar Status";
            }
            else if (smodule == "TOBTAB_RETURN_LICENSE")
            {
                ViewBag.PageTitle = "Serah Batal Lesen";
            }
            else if (smodule == "TOBTAB_TG_EXCEPTIONS")
            {
                ViewBag.PageTitle = "Pengecualian TG";
            }
            else if (smodule == "TOBTAB_UMRAH")
            {
                ViewBag.PageTitle = "Umrah";
            }
            else if (smodule == "TOBTAB_MARKETING_OFFICER")
            {
                ViewBag.PageTitle = "Pegawai Pemasaran";
            }
            else if (smodule == "TOBTAB_ADD_FIELD")
            {
                ViewBag.PageTitle = "Tambah Bidang";
            }
        }
        public ActionResult Director(string module, string btn)
        {
            ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();

            string smodule = module;
            string sstatus = "";
            string sAppID = "";

            List<TobtabViewModels.tobtab_app> app = new List<TobtabViewModels.tobtab_app>();

            app = tobtabBLL.GetTobtabStatus(userID, smodule);

            foreach (var st in app)
            {
                sstatus = st.Status.ToString();
                smodule = st.Module.ToString();
                sAppID = st.AppID.ToString();
            }
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
            ViewBag.AppID = sAppID;

            ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            ViewBag.Company = tobtabBLL.GetOrganization(userID);
            //ViewBag.checklistItem = tobtabBLL.GetChecklistItem(sAppID, "98B65F55-2950-4523-827C-32CC90228EA9", smodule);
            getPageTitle(smodule);
            List<SelectListItem> PemegangSaham = new List<SelectListItem>();
            PemegangSaham = GetStatusPemeganganSahamDropDown("Person");
            ViewData["StatusPemegangSaham_"] = PemegangSaham;

            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;

            List<SelectListItem> ref_country = new List<SelectListItem>();
            ref_country = GetCountryDropDown();
            //ViewData["Countrylist_"] = ref_country;
            ViewBag.Country = ref_country;


            List<SelectListItem> Agama = new List<SelectListItem>();
            Agama = GetAgamaDropDown();
            ViewData["Agama_"] = Agama;

            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;

            List<SelectListItem> Asean = new List<SelectListItem>();
            Asean = GetAseanDropDown();
            ViewData["Asean_"] = Asean;

            List<SelectListItem> NonAsean = new List<SelectListItem>();
            NonAsean = GetNonAseanDropDown();
            ViewData["NonAsean_"] = NonAsean;

            return View();
        }

        public ActionResult OverseasTouristCompany(string module, string btn)
        {
            ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();

            string smodule = module;
            string sstatus = "";
            string sAppID = Session["AppID"].ToString();

            List<TobtabViewModels.tobtab_app> app = new List<TobtabViewModels.tobtab_app>();

            app = tobtabBLL.GetTobtabStatusByAppIdx(userID, smodule, sAppID);

            foreach (var st in app)
            {
                sstatus = st.Status.ToString();
                smodule = st.Module.ToString();
                sAppID = st.AppID.ToString();
            }
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
            ViewBag.AppID = sAppID;

            ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            ViewBag.Company = tobtabBLL.GetOrganization(userID);
            getPageTitle(smodule);
            //ViewBag.checklistItem = tobtabBLL.GetChecklistItem(sAppID, "24B511C4-14CC-4339-AA02-2968F9F584E9", smodule);

            /* List<SelectListItem> PemegangSaham = new List<SelectListItem>();
             PemegangSaham = GetStatusPemeganganSahamDropDown("Person");
             ViewData["StatusPemegangSaham_"] = PemegangSaham;*/

            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;


            List<SelectListItem> Agama = new List<SelectListItem>();
            Agama = GetAgamaDropDown();
            ViewData["Agama_"] = Agama;

            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;

            List<SelectListItem> Asean = new List<SelectListItem>();
            Asean = GetAseanDropDown();
            ViewData["Asean_"] = Asean;

            List<SelectListItem> NonAsean = new List<SelectListItem>();
            NonAsean = GetNonAseanDropDown();
            ViewData["NonAsean_"] = NonAsean;

            return View();
        }


        public ActionResult SupportingDocument(string module, string btn)
        {
            ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();
            string smodule = module;
            string sstatus = "";
            string sAppID = Session["AppID"].ToString();

            List<TobtabViewModels.tobtab_app> app = new List<TobtabViewModels.tobtab_app>();
            app = tobtabBLL.GetTobtabStatusByAppIdx(userID, smodule, sAppID);
            foreach (var st in app)
            {
                sstatus = st.Status.ToString();
                smodule = st.Module.ToString();
                sAppID = st.AppID.ToString();
                break;
            }
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
            ViewBag.AppID = sAppID;
            getPageTitle(smodule);

            ViewBag.ModuleID = tobtabBLL.GetModuleID(smodule);
            var licenses = tobtabBLL.GetLicenseApplication(sAppID);
            ViewBag.License = licenses;
            ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            var company = tobtabBLL.GetOrganization(userID);
            ViewBag.Company = company;
            //ViewBag.checklistItem = tobtabBLL.GetChecklistItem(sAppID, "D6E0C1E4-B834-4839-BFA4-E088BEF3E8CF", smodule);
            string sModuleDoc = "";
            if (module == "TOBTAB_NEW")
                sModuleDoc = "TOBTAB_NEW_DOKUMEN";
            else if (module == "TOBTAB_RENEW")
                sModuleDoc = "TOBTAB_RENEW_DOKUMEN";
            else if (module == "TOBTAB_ADD_BRANCH")
                sModuleDoc = "TOBTAB_ADD_BRANCH_DOCS";
            else if (module == "TOBTAB_CHANGE_STATUS")
                sModuleDoc = "TOBTAB_CHANGE_STATUS_DOCUMENT";// MM2H_CHANGESTATUS_DOKUMEN //TOBTAB_CHANGE_STATUS_DOCUMENT
            else if (module == "TOBTAB_RETURN_LICENSE")
                sModuleDoc = "TOBTAB_RETURN_LICENSE_DOCS";
            else if (module == "TOBTAB_RETURN_LICENSE")
                sModuleDoc = "TOBTAB_RETURN_LICENSE_DOCS";
            else if (module == "TOBTAB_TG_EXCEPTIONS")
                sModuleDoc = "TOBTAB_TG_EXCEPTIONS_DOCS";
            else if (module == "TOBTAB_UMRAH")
                sModuleDoc = "TOBTAB_UMRAH_DOCS";
            else if (module == "TOBTAB_MARKETING_OFFICER")
                sModuleDoc = "TOBTAB_MARKETING_OFFICERS_DOCS";
            else if (module == "TOBTAB_ADD_FIELD")
                sModuleDoc = "TOBTAB_ADD_FIELD_DOCS";

            ViewBag.docList = tobtabBLL.GetDashboardList(sAppID, sModuleDoc, sstatus);

            int iCountItem = 0;
            foreach (var item in ViewBag.docList)
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


            ViewBag.StatusCode = sstatus;
            //ViewBag.docDirectorIDList = tobtabBLL.GetDocumentsShareholder("TOBTAB_DIRECTOR_ID", licenses.tobtab_idx.ToString());
            //ViewBag.docShareholderList = tobtabBLL.GetDocumentsShareholderByAppId("TOBTAB_SHAREHOLDER_ID", licenses.tobtab_idx.ToString(), company.organization_idx.ToString());
            //ViewBag.OtherDocumentList = coreOrgHelper.GetOtherDocument(sAppID);


            if (module == "TOBTAB_CHANGE_STATUS")
            {
                ViewBag.ShareholderList = coreOrgHelper.GetShareHolderChangeStatusAttachment(sAppID);
                ViewBag.PengarahList = coreOrgHelper.GetDirectorChangeStatusAttachment(sAppID);

            }
            else
            {
                ViewBag.PengarahList = coreOrgHelper.GetDirectorAttachment(company.organization_idx.ToString());
                ViewBag.ShareholderList = coreOrgHelper.GetShareHolderAttachment(company.organization_idx.ToString());
            }

            //ViewBag.PengarahList = coreOrgHelper.GetDirectorAttachment(company.organization_idx.ToString());
            //ViewBag.ShareholderList = coreOrgHelper.GetShareholderDocAttachment(company.organization_idx.ToString());
            ViewBag.OtherDocumentList = coreOrgHelper.GetOtherDocument(sAppID);

            return View();
        }

        #endregion

        #region renew

        public void getAppDetail(string module)
        {
            ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();
            string smodule = module;
            string sstatus = "";
            string sAppID = "";
            string status_code = "";
            string appDate = "";
            List<TobtabViewModels.tobtab_application> app = new List<TobtabViewModels.tobtab_application>();
            var organization = tobtabBLL.GetOrganization(userID);
            app = tobtabBLL.GetApplicationStatusMain(userID, module,organization.organization_idx);
            foreach (var st in app)
            {
                sstatus = st.status.ToString();
                smodule = st.module_name.ToString();
                sAppID = st.apply_idx.ToString();
                status_code = st.status_code.ToString();
                appDate = st.Application_date.ToString();
            }
            ViewBag.LatestStatus = status_code;
            ViewBag.StatusCode = status_code;
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
            ViewBag.AppID = sAppID;
            getPageTitle(smodule);
            ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            //var organization = tobtabBLL.GetOrganization(userID);
            ViewBag.Company = organization;
            ViewBag.OrganizationID = organization.organization_idx;
            ViewBag.MSG_MANDATORY = "Sila masukkan data";
            var coreLicense = tobtabBLL.GetCoreLicens(organization.organization_idx.ToString());
            ViewBag.NoLicense = organization.organization_identifier;
            ViewBag.StartDateLicense = appDate; //temp
            ViewBag.EndDateLicense = appDate; //temp
            if (coreLicense != null)
            {
                ViewBag.NoLicense = coreLicense.license_no; ;
                ViewBag.EndDateLicense = String.Format("{0:dd/MM/yyyy}", coreLicense.end_dt);
            }
        }

        public void getAppDetailBranch(string module,string Appid)
        {
            ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();
            string smodule = module;
            string sstatus = "";
            string sAppID = "";
            string status_code = "";
            string appDate = "";
            List<TobtabViewModels.tobtab_application> app = new List<TobtabViewModels.tobtab_application>();
            var organization = tobtabBLL.GetOrganization(userID);
            // app = tobtabBLL.GetApplicationStatusMain(userID, module, organization.organization_idx);
            app = tobtabBLL.GetTobtabAddBranchStatus(Appid);

            foreach (var st in app)
            {
                sstatus = st.status.ToString();
                smodule = st.module_name.ToString();
                sAppID = st.apply_idx.ToString();
                status_code = st.status_code.ToString();
                appDate = st.Application_date.ToString();
            }
            ViewBag.LatestStatus = status_code;
            ViewBag.StatusCode = status_code;
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
            if (Appid != null)
            {
                ViewBag.AppID = Appid;
            }
            else
            {
                ViewBag.AppID = sAppID;
            }

            
            getPageTitle(smodule);
            ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            //var organization = tobtabBLL.GetOrganization(userID);
            ViewBag.Company = organization;
            ViewBag.OrganizationID = organization.organization_idx;
            ViewBag.MSG_MANDATORY = "Sila masukkan data";
            var coreLicense = tobtabBLL.GetCoreLicens(organization.organization_idx.ToString());
            ViewBag.NoLicense = organization.organization_identifier;
            ViewBag.StartDateLicense = appDate; //temp
            ViewBag.EndDateLicense = appDate; //temp
            if (coreLicense != null)
            {
                ViewBag.NoLicense = coreLicense.license_no; ;
                ViewBag.EndDateLicense = String.Format("{0:dd/MM/yyyy}", coreLicense.end_dt);
            }
        }

        public JsonResult getAppDetailCommon(string module, string applyId)
        {
            TbTambahBidang bidang = new TbTambahBidang();
            ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();
            string smodule = module;
            string sstatus = "";
            string sAppID = "";
            string status_code = "";
            string appDate = "";
            List<TobtabViewModels.tobtab_application> app = new List<TobtabViewModels.tobtab_application>();
            flow_application_stubs Bidang = TourlistUnitOfWork.FlowApplicationStubs.Find(x => x.apply_idx.ToString() == applyId).FirstOrDefault();


            var organization = tobtabBLL.GetOrganization(Bidang.apply_user.ToString());
            // app = tobtabBLL.GetApplicationStatusMain(Bidang.apply_user.ToString(), module,organization.organization_idx);
            app = tobtabBLL.GetApplicationStatusMain(Bidang.apply_user.ToString(), module, organization.organization_idx);
            foreach (var st in app)
            {
                sstatus = st.status.ToString();
                smodule = st.module_name.ToString();
                sAppID = st.apply_idx.ToString();
                status_code = st.status_code.ToString();
                appDate = st.Application_date.ToString();
            }
            bidang.LatestStatus = status_code;
            bidang.StatusCode = status_code;
            bidang.Status = sstatus;
            bidang.Mod = smodule;
            bidang.AppID = sAppID;
            getPageTitle(smodule);
            bidang.SSMNo = tobtabBLL.GetSSMNo(Bidang.apply_user.ToString());
           // var Lsn = tobtabBLL.GetLicenseApplication(sAppID);
            var Lsn = tobtabBLL.GetLicenseApplication(applyId);
            bidang.License = Lsn.tobtab_idx.ToString();
            bidang.inbound = Lsn.inbound.ToString();
            bidang.ticketing = Lsn.ticketing.ToString();
            bidang.umrah = Lsn.umrah.ToString();
            bidang.outbound = Lsn.outbound.ToString();
          //  var organization = tobtabBLL.GetOrganization(Bidang.apply_user.ToString());
            bidang.Company = organization.organization_idx.ToString();
            bidang.OrganizationID = organization.organization_idx.ToString();
            bidang.MSG_MANDATORY = "Sila masukkan data";
            var coreLicense = tobtabBLL.GetCoreLicens(organization.organization_idx.ToString());
            bidang.NoLicense = organization.organization_identifier;
            bidang.StartDateLicense = appDate; //temp
            bidang.EndDateLicense = appDate; //temp
            if (coreLicense != null)
            {
                bidang.NoLicense = coreLicense.license_no; ;
                bidang.EndDateLicense = String.Format("{0:dd/MM/yyyy}", coreLicense.end_dt);
            }
            return Json(bidang, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RenewalBranch(string module, string btn, string AppID, string AppNo)
        {

            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "Tobtab", new
                {
                    module = module
                });
            }
            getAppDetailBranch(module, AppID);

            List<SelectListItem> PemegangSaham = new List<SelectListItem>();
            PemegangSaham = GetStatusPemeganganSahamDropDown("Person");
            ViewData["StatusPemegangSaham_"] = PemegangSaham;

            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;


            List<SelectListItem> Agama = new List<SelectListItem>();
            Agama = GetAgamaDropDown();
            ViewData["Agama_"] = Agama;

            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;

            List<SelectListItem> Asean = new List<SelectListItem>();
            Asean = GetAseanDropDown();
            ViewData["Asean_"] = Asean;

            List<SelectListItem> NonAsean = new List<SelectListItem>();
            NonAsean = GetNonAseanDropDown();
            ViewData["NonAsean_"] = NonAsean;

            if (module == "TOBTAB_RENEW")
            {
                coreOrgHelper.updateListing(Request.QueryString["item"]);
            }
            return View();
        }

        #endregion

        #region Pengecualian TG
        public ActionResult TGExceptions(string module, string btn)
        {
            getAppDetail(module);
            string userID = Session["UID"].ToString();
            /*ViewBag.itemID = Request.QueryString["item"];
            string userID = Session["UID"].ToString();
           // ViewBag.MenuStatus = tobtabBLL.GetActiveMenu(Guid.Parse(userID));
            string smodule = "";
            string sstatus = "";
            string sAppID = "";
            List<TobtabViewModels.tobtab_app> app = new List<TobtabViewModels.tobtab_app>();
            app = tobtabBLL.GetTobtabStatus(userID, "TOBTAB_TG_EXCEPTIONS");
            List<SelectListItem> Pengarah = new List<SelectListItem>();
            if (app.Count > 0)
            {
                foreach (var st in app)
                {
                    sstatus = st.Status.ToString();
                    smodule = st.Module.ToString();
                    sAppID = st.AppID.ToString();
                }
                ViewBag.Status = sstatus;
                ViewBag.Mod = smodule;
                ViewBag.AppID = sAppID;
                ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
                var organization = tobtabBLL.GetOrganization(userID);
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_TG_EXCEPTIONS");
                ViewBag.License = tobtabBLL.GetTGExceptionsApplication(sAppID);
            }*/
            var organization = ViewBag.Company;
            List<SelectListItem> Pengarah = new List<SelectListItem>();
            Pengarah = GetPengarahDropDown(organization.organization_idx.ToString(), ViewBag.License.tobtab_tg_exceptions_idx.ToString());
            ViewData["Pengarah_"] = Pengarah;


            List<SelectListItem> Lesen = new List<SelectListItem>();
            Lesen = GetLesenDropDown(userID);
            ViewData["Lesen_"] = Lesen;

            List<SelectListItem> JustifikasiBatal = new List<SelectListItem>();
            JustifikasiBatal = GetJustifikasiSerahLesenDropDown();
            ViewData["JustifikasiBatalLesen_"] = JustifikasiBatal;

            List<SelectListItem> TempohPembaharuan = new List<SelectListItem>();
            TempohPembaharuan = GetTempohPembaharuanDropDown();
            ViewData["TempohPembaharuan_"] = TempohPembaharuan;

            return View();
        }
        public ActionResult TGExceptionsTripInfo(string module, string btn)
        {

            getAppDetail(module);
            ViewBag.TgException = tobtabBLL.getTgExceptionInfoByGuid(ViewBag.License.tobtab_idx);
            List<SelectListItem> Negeri = new List<SelectListItem>();
            Negeri = GeStateDropDown();
            ViewData["Negeri_"] = Negeri;

            List<SelectListItem> Daerah = new List<SelectListItem>();
            Daerah = GetDaerahDropDown();
            ViewData["Daerah_"] = Daerah;

            List<SelectListItem> Purpose = new List<SelectListItem>();
            Purpose = GetPurposeDropDown();
            ViewData["Purpose_"] = Purpose;

            return View();
        }

        #endregion

        #region tambah bidang

        public ActionResult ChangeStatusInfo(string module, string btn)
        {
            getAppDetail(module);

            if (btn == "Dashboard")
            {
                return RedirectToAction("Dashboard", "Tobtab", new
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


            var Malaysia = ref_country.Where(d => d.Text.ToString() == "Malaysia").ToList();
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
                    //model.number_of_shares_string = String.Format("{0:#,#.}", shareholder.number_of_shares);
                    model.number_of_shares_string = shareholder.number_of_shares.ToString();
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
        public ActionResult AddFieldInfo(string module, string btn)
        {
            getAppDetail(module);
            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;
            ViewBag.shareholderCount = coreHelper.getShareholderByOrgIdxIsNonMuslim(ViewBag.OrganizationID);
            return View();
        }

        public JsonResult MarketingPersonalInfoCommon(string module, string applyId)
        {
            var data = getAppDetailCommon(module, applyId);

            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddFieldInfoCommon(string module, string applyId)
        {
            var data = getAppDetailCommon(module, applyId);

            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion


        public ActionResult MarketingPersonInfo(string module, string btn)
        {
            getAppDetail(module);
            List<SelectListItem> PemegangSahamPerson = new List<SelectListItem>();
            PemegangSahamPerson = GetStatusPemeganganSahamDropDown("Person");
            ViewData["StatusPemegangSahamPerson_"] = PemegangSahamPerson;

            List<SelectListItem> PemegangSahamOrganization = new List<SelectListItem>();
            PemegangSahamOrganization = GetStatusPemeganganSahamDropDown("Organization");
            ViewData["StatusPemegangSahamOrganization_"] = PemegangSahamOrganization;

            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;

            List<SelectListItem> Agama = new List<SelectListItem>();
            Agama = GetAgamaDropDown();
            ViewData["Agama_"] = Agama;

            List<SelectListItem> Country = new List<SelectListItem>();
            Country = GetWorldDropDown();
            ViewData["World_"] = Country;

            List<SelectListItem> State = new List<SelectListItem>();
            State = GeStateBranchDropDown(ViewBag.OrganizationID);
            ViewData["State_"] = State;
            return View();
        }

        public ActionResult AdvertisingDashboard(string btn, string module)
        {
            if (module == null)
            {
                module = "TOBTAB_UMRAH";
            }
            ViewBag.module = module;
            string userID = Session["UID"].ToString();
            //ViewBag.MenuStatus = tobtabBLL.GetActiveMenu(Guid.Parse(userID));
            getCoreOrganizationDetail(userID);

            string smodule = "";
            string sstatus = "";
            string sAppID = "";
            string status_code = "";

            List<TobtabViewModels.tobtab_application> app = new List<TobtabViewModels.tobtab_application>();
            var organization = tobtabBLL.GetOrganization(userID);
            app = tobtabBLL.GetApplicationStatusMain(userID, module,organization.organization_idx);

            ViewBag.Hantar = false;
            List<SelectListItem> Pengarah = new List<SelectListItem>();
            if (app.Count > 0)
            {
                foreach (var st in app)
                {
                    sstatus = st.status.ToString();
                    smodule = st.module_name.ToString();
                    sAppID = st.apply_idx.ToString();
                    status_code = st.status_code.ToString();
                }
                ViewBag.NewStatus = "false";
            }
            else
            {
                ViewBag.NewStatus = "true";
            }

          //  var organization = tobtabBLL.GetOrganization(userID);
            List<TobtabViewModels.tobtab_application> latestApp = new List<TobtabViewModels.tobtab_application>();
            latestApp = tobtabBLL.GetApplicationStatusMain(userID, null,organization.organization_idx);
            string sstatus1 = "";
            string sModule1 = "";
            if (latestApp.Count > 0)
            {
                foreach (var st in latestApp)
                {
                    sstatus1 = st.status_code.ToString();
                    sModule1 = st.module_name.ToString();
                }
            }
            //
            List<TobtabViewModels.tobtab_application> tobtabnew = new List<TobtabViewModels.tobtab_application>();
            tobtabnew = tobtabBLL.GetApplicationStatusMain(userID, "TOBTAB_UMRAH",organization.organization_idx);
            if (tobtabnew.Count > 0)
            {
                ViewBag.tobtab_new = true;
            }
            else
            {
                ViewBag.tobtab_new = false;
            }

            ViewBag.StatusCode = status_code;
            ViewBag.LatestStatus = sstatus1;
            ViewBag.LatestModule = sModule1;
            ViewBag.Status = sstatus;
            ViewBag.Mod = smodule;
            ViewBag.AppID = sAppID;
            Session["AppID"] = sAppID;
            ViewBag.OrganizationIdx = organization.organization_idx;

            if (smodule == "TOBTAB_UMRAH")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_UMRAH", status_code);
                ViewBag.PageTitle = "Umrah";
            }
            else if (smodule == "TOBTAB_MARKETING_OFFICER")
            {
                ViewBag.DashBoardList = tobtabBLL.GetDashboardList(sAppID, "TOBTAB_MARKETING_OFFICER", status_code);
                ViewBag.PageTitle = "Pegawai Pemasaran";
            }

            ViewBag.tobtab_officer = tobtabBLL.GetTobtabStatusModul(organization.organization_idx.ToString(), "TOBTAB_MARKETING_OFFICER");
            ViewBag.tobtab_umrah = tobtabBLL.GetTobtabStatusModul(organization.organization_idx.ToString(), "TOBTAB_UMRAH");
            ViewBag.tobtab_new = tobtabBLL.GetTobtabStatusModul(organization.organization_idx.ToString(), "TOBTAB_NEW");
            ViewBag.License = tobtabBLL.GetLicenseApplication(sAppID);
            Pengarah = GetPengarahDropDown(organization.organization_idx.ToString(), ViewBag.License.tobtab_idx.ToString());

            if (ViewBag.DashBoardList != null)
            {
                int iCountItem = 0;
                foreach (var item in ViewBag.DashBoardList)
                {
                    if (item.bool1 == "0")
                    {
                        iCountItem++;
                    }
                }
                if (iCountItem == 0)
                {
                    ViewBag.Hantar = true;
                }
            }

            ViewBag.SSMNo = tobtabBLL.GetSSMNo(userID);
            ViewData["Pengarah_"] = Pengarah;
            List<SelectListItem> Negeri = new List<SelectListItem>();

            List<SelectListItem> Lesen = new List<SelectListItem>();
            Lesen = GetLesenDropDown(userID);
            ViewData["Lesen_"] = Lesen;

            List<SelectListItem> JustifikasiBatal = new List<SelectListItem>();
            JustifikasiBatal = GetJustifikasiSerahLesenDropDown();
            ViewData["JustifikasiBatalLesen_"] = JustifikasiBatal;

            List<SelectListItem> TempohPembaharuan = new List<SelectListItem>();
            TempohPembaharuan = GetTahun();
            ViewData["TempohPembaharuan_"] = TempohPembaharuan;

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
            return View();
        }

        public ActionResult AdvertisingInfo(string module, string btn)
        {
            getAppDetail(module);
            string userID = Session["UID"].ToString();
            var tobtab_idx = ViewBag.License.tobtab_idx;
            tobtab_umrah_advertising advertising = tobtabBLL.GetUmrahAdvertising(tobtab_idx);
            if (advertising == null)
            {
                advertising = tobtabBLL.saveUmrahAdvertising(advertising, tobtab_idx);
            }
            ViewBag.tobtab_umrah_advertising_ref = advertising.tobtab_umrah_advertising_idx;
            ViewBag.TajukIklan = advertising.advertise_title;
            ViewBag.TermaIklan = advertising.advertise_terms_conditions;
            ViewBag.hargaPakejFrom = advertising.payment_package_madefrom;
            ViewBag.hargaPakejTo = advertising.payment_package_madeto;
            //ViewBag.jadualPerjalanan = advertising.trip_schedule;
            //ViewBag.Company = tobtabBLL.GetOrganization(userID);

            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;

            List<SelectListItem> Language = new List<SelectListItem>();
            Language = GetLanguageDown();
            ViewData["Language_"] = Language;


            return View();
        }

        public JsonResult AdvertisingInfoCommon(string applyidx, string module)
        {

            UmrahInformation umrah = new UmrahInformation();
            tobtab_licenses license = TourlistUnitOfWork.TobtabLicenses.Find(x => x.stub_ref.ToString() == applyidx).FirstOrDefault();

            tobtab_umrah_advertising advertising = tobtabBLL.GetUmrahAdvertising(license.tobtab_idx);
            if (advertising == null)
            {
                advertising = tobtabBLL.saveUmrahAdvertising(advertising, license.tobtab_idx);
            }

            var cuser = TourlistUnitOfWork.FlowApplicationStubs.Find(x => x.apply_idx.ToString() == applyidx).FirstOrDefault();
            var organization = tobtabBLL.GetOrganization(cuser.apply_user.ToString());

            var app = tobtabBLL.GetApplicationStatusMain(cuser.apply_user.ToString(), module,organization.organization_idx);
            var appDate = "";
            foreach (var st in app)
            {
                appDate = st.Application_date.ToString();
            }

            var coreLicense = tobtabBLL.GetCoreLicens(organization.organization_idx.ToString());
            if (coreLicense != null)
            {
                umrah.NoLicense = coreLicense.license_no;

            } else
            {
                umrah.NoLicense = organization.organization_identifier;
            }


            
            umrah.StartDateLicense = appDate; //temp
            umrah.EndDateLicense = appDate; //temp
            umrah.tobtab_umrah_advertising_ref = advertising.tobtab_umrah_advertising_idx.ToString();
            umrah.TajukIklan = advertising.advertise_title;
            umrah.TermaIklan = advertising.advertise_terms_conditions;
            umrah.hargaPakejFrom = advertising.payment_package_madefrom.ToString();
            umrah.hargaPakejTo = advertising.payment_package_madeto.ToString();
            //umrah.jadualPerjalanan = advertising.trip_schedule;

            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;

            List<SelectListItem> Language = new List<SelectListItem>();
            Language = GetLanguageDown();
            ViewData["Language_"] = Language;


            return Json(umrah, JsonRequestBehavior.AllowGet);
        }

        public bool uploadOtherDocument(HttpPostedFileBase file, string FileName, string DocID, string moduleID, string appID)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            string folder = "/Attachment/Tobtab/" + filename;

            CoreOrganizationModel.CoreOtherDocument doc = new CoreOrganizationModel.CoreOtherDocument();

            var userID = Session["UID"].ToString();

            doc.module_ref = moduleID;
            doc.transaction_ref = appID;
            doc.document_upload_path = this.GetUploadFolder(TourlistEnums.MotacModule.TOBTAB, filename); //folder;

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


            string foldersave = Server.MapPath("~/Attachment/Tobtab");
            if (ID)
            {
                //use common upload in base contoller
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.TOBTAB);
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
        #region AJAX CALL

        [HttpPost]
        public JsonResult ajaxSaveOtherDocument(HttpPostedFileBase file_Dokumen, string filename, string ModuleID, string AppId)
        {

            bool doc = uploadOtherDocument(file_Dokumen, filename, "", ModuleID, AppId);

            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxDeleteOtherDocument(string idx)
        {

            bool doc = coreOrgHelper.DeleteOtherDocument(idx);

            return Json(doc, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult deletePartner(string partner_idx)
        {
            bool doc = tobtabBLL.deleteOverseasPartners(Guid.Parse(partner_idx));
            return Json(doc, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult deletePackage(string package_idx)
        {
            bool doc = tobtabBLL.deleteOverseasPackages(Guid.Parse(package_idx));
            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ajaxGenerateApplicationStubs(string Module, Guid OrganizationID)
        {

            var userID = Session["UID"];
            Guid Id = new Guid(userID.ToString());
            var user = coreHelper.GetCoreUserByGuid(Id);
            if (user != null && user.user_organization == null)
            {
                user.user_organization = user.person_ref;
            }
            //checking duplicate
            var duplicate = tobtabBLL.duplicateFlowApplicationStub(Id);
            if (!duplicate || Module == "TgExecption" || Module == "Branch")
            {
                if (Module == "New")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_NEW,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                }
                else
                if (Module == "Renewal")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_RENEW,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                    //get bidang from core_license & insert into tobtab_license
                    Guid stubID = stub.application_Stubs.apply_idx;
                    coreOrgHelper.updateTobtabLicenseRenewal(OrganizationID, stubID, Id);

                }
                else
                if (Module == "Branch")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_ADD_BRANCH,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                }
                else
                if (Module == "ChangeStatus")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_CHANGE_STATUS,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                    Guid stubID = stub.application_Stubs.apply_idx;

                    coreOrgHelper.ChangeStatusOrganization_SaveNew(OrganizationID, stubID, Id);
                    coreOrgHelper.ChangeStatusShareHolder_SaveNew(OrganizationID, stubID, Id);
                    coreOrgHelper.ChangeStatusDirector_SaveNew(OrganizationID, stubID, Id);
                }
                else
                if (Module == "ReturnLicense")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_RETURN_LICENSE,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                }
                else
                if (Module == "Umrah")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_UMRAH,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                }
                else
                if (Module == "MarketingOfficer")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_MARKETING_OFFICER,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                }
                else
                if (Module == "TgExecption")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_TG_EXCEPTIONS,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                }
                else
                if (Module == "AddField")
                {
                    var stub = coreHelper.GenerateApplicationStubs(
                           TourlistEnums.MotacModule.TOBTAB,
                           TourlistEnums.ModuleLicenseType.TIADA,
                           TourlistEnums.SolModulesType.TOBTAB_ADD_FIELD,
                           TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                             user);
                }

            }
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

        public JsonResult ajaxAddTGTrip()
        {
            tobtab_tg_traveling tgData = new tobtab_tg_traveling();
            var userID = Session["UID"].ToString();
            var module = Request["module"].ToString();
            var component_id = Request["component_id"].ToString();
            var license_ref = Request["license_ref"].ToString();
            var trip_option = Request["trip_option"].ToString();
            var purpose_idx = Request["purpose_idx"].ToString();
            var travel_state = Request["travel_state"].ToString();
            var travel_town = Request["travel_town"].ToString();
            var destination_state = Request["destination_state"].ToString();
            var destination_town = Request["destination_town"].ToString();
            var travel_location = Request["travel_location"].ToString();
            var travel_destination = Request["travel_destination"].ToString();
            var travel_route = Request["travel_route"].ToString();
            var travel_leader = Request["travel_leader"].ToString();
            var travel_accommodation = Request["travel_accommodation"].ToString();
            var destination_location = Request["destination_location"].ToString();
            var travel_start_date = Request["travel_start_date"];
            var travel_start_time = Request["travel_start_time"];
            // var travel_end_date = Request["travel_end_date"];
            // var travel_end_time = Request["travel_end_time"];
            var destination_start_date = Request["destination_start_date"];
            var destination_start_time = Request["destination_start_time"];
            var other_purpose = Request["other_purpose"];

            tobtab_tg_exceptions tgExcept = tobtabBLL.GetTgExceptionInfo(license_ref);
            tgData.tobtab_tg_traveling_idx = Guid.NewGuid();
            if(tgExcept != null)
            {
                tgData.tobtab_tg_exceptions_idx = tgExcept.tobtab_tg_exceptions_idx;
            }
            tgData.created_by = Guid.Parse(userID);
            tgData.created_at = DateTime.Now;
            tgData.active_status = 1;
            tgData.travel_purpose = Guid.Parse(purpose_idx);
            tgData.travel_state = Guid.Parse(travel_state);
            tgData.travel_town = Guid.Parse(travel_town);
            tgData.destination_state = Guid.Parse(destination_state);
            tgData.destination_town = Guid.Parse(destination_town);
            tgData.travel_location = travel_location;
            tgData.travel_destination = travel_destination;
            tgData.travel_route = travel_route;
            tgData.travel_leader = travel_leader;
            tgData.travel_accommodation = travel_accommodation;
            tgData.destination_location = destination_location;
            tgData.travel_purpose_others = other_purpose;
            tgData.travel_purpose = Guid.Parse(purpose_idx);
            if (trip_option == "1")
            {
                tgData.one_way = 1;
            }
            else
            {
                tgData.two_way = 1;
                tgData.two_way_travel_state_ref = Guid.Parse(Request["two_way_travel_state_ref"].ToString());
                tgData.two_way_travel_town_ref = Guid.Parse(Request["two_way_travel_town_ref"].ToString());
                tgData.two_way_destination_state_ref = Guid.Parse(Request["two_way_destination_state_ref"].ToString());
                tgData.two_way_destination_town_ref = Guid.Parse(Request["two_way_destination_town_ref"].ToString());
                tgData.two_way_travel_location = Request["two_way_travel_location"];
                tgData.two_way_destination_location = Request["two_way_destination_location"];
                if (Request["two_way_travel_start_date"] != null && Request["two_way_travel_start_date"] != "")
                {
                    tgData.two_way_travel_start_date = DateTime.Parse(Request["travel_start_date"]);
                }
                if (Request["two_way_travel_start_time"] != null && Request["two_way_travel_start_time"] != "")
                {
                    tgData.two_way_travel_start_time = Request["two_way_travel_start_time"];
                }
                if (Request["two_way_destination_start_date"] != null && Request["two_way_destination_start_date"] != "")
                {
                    tgData.two_way_destination_start_date = DateTime.Parse(Request["two_way_destination_start_date"]);
                }
                if (Request["two_way_destination_start_time"] != null && Request["two_way_destination_start_time"] != "")
                {
                    tgData.two_way_destination_start_time = Request["two_way_destination_start_time"];
                }
            }
            if (travel_start_date != null && travel_start_date != "")
            {
                tgData.travel_start_date = DateTime.Parse(travel_start_date);
            }
            if (travel_start_time != null && travel_start_time != "")
            {
                tgData.travel_start_time = travel_start_time;
            }
            if (destination_start_date != null && destination_start_date != "")
            {
                tgData.destination_start_date = DateTime.Parse(destination_start_date);
            }
            if (destination_start_time != null && destination_start_time != "")
            {
                tgData.destination_start_time = destination_start_time;
            }

            //saveUmrahInfo();
            var Application = tobtabBLL.addTgTrip(tgData);
            return Json(Application, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getTGTrip(string travelingIdx)
        {
            var application = tobtabBLL.GetTgTrip(travelingIdx);
            var departDate = ((DateTime)application.travel_start_date).ToString("yyyy-MM-dd");
            var destinationDate = ((DateTime)application.destination_start_date).ToString("yyyy-MM-dd");
            var twoWayDepartDate = "";
            var twoWayDestinationDate = "";
            if (application.two_way_travel_start_date != null)
            {
                 twoWayDepartDate = ((DateTime)application.two_way_travel_start_date).ToString("yyyy-MM-dd");
            }
            if(application.two_way_destination_start_date != null)
            {
                 twoWayDestinationDate = ((DateTime)application.two_way_destination_start_date).ToString("yyyy-MM-dd");
            }
            var response = new {
                application,
                departDate,
                destinationDate,
                twoWayDepartDate,
                twoWayDestinationDate
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateTgTrip()
        {
            tobtab_tg_traveling tgData = new tobtab_tg_traveling();
            var user_idx = Guid.Parse(Session["UID"].ToString());

            var trip_option = Request["trip_option"].ToString();
            tgData.tobtab_tg_traveling_idx = Guid.Parse(Request["traveling_idx"].ToString());
            tgData.travel_purpose = Guid.Parse(Request["travel_purpose"].ToString());
            tgData.travel_purpose_others = Request["other_purpose"];
            tgData.travel_state = Guid.Parse(Request["travel_state"].ToString());
            tgData.travel_town = Guid.Parse(Request["travel_town"].ToString());
            tgData.destination_state = Guid.Parse(Request["destination_state"].ToString());
            tgData.destination_town = Guid.Parse(Request["destination_town"].ToString());
            tgData.travel_location = Request["travel_location"].ToString();
            tgData.travel_destination = Request["travel_destination"].ToString();
            tgData.travel_route = Request["travel_route"].ToString();
            tgData.travel_leader = Request["travel_leader"].ToString();
            tgData.travel_accommodation = Request["travel_accommodation"].ToString();
            tgData.destination_location = Request["destination_location"].ToString();
            tgData.travel_start_date = DateTime.Parse(Request["travel_start_date"]);
            tgData.travel_start_time = Request["travel_start_time"];
            // var travel_end_date = Request["travel_end_date"];
            // var travel_end_time = Request["travel_end_time"];
            tgData.destination_start_date = DateTime.Parse(Request["destination_start_date"]);
            tgData.destination_start_time = Request["destination_start_time"];
            //tgData.one_way = Byte.Parse(Request["radioc_sehala"]);
            //tgData.two_way = Byte.Parse(Request["radioc_duahala"]);
            if (trip_option == "1")
            {
                tgData.one_way = 1;
            }
            else
            {
                tgData.two_way = 1;
                tgData.two_way_travel_state_ref = Guid.Parse(Request["two_way_travel_state_ref"].ToString());
                tgData.two_way_travel_town_ref = Guid.Parse(Request["two_way_travel_town_ref"].ToString());
                tgData.two_way_travel_location = Request["two_way_travel_location"];
                tgData.two_way_travel_start_date = DateTime.Parse(Request["two_way_travel_start_date"]);
                tgData.two_way_travel_start_time = Request["two_way_travel_start_time"];

                tgData.two_way_destination_state_ref = Guid.Parse(Request["two_way_destination_state_ref"].ToString());
                tgData.two_way_destination_town_ref = Guid.Parse(Request["two_way_destination_town_ref"].ToString());
                tgData.two_way_destination_location = Request["two_way_destination_location"];
                tgData.two_way_destination_start_date = DateTime.Parse(Request["two_way_destination_start_date"]);
                tgData.two_way_destination_start_time = Request["two_way_destination_start_time"];

            }


            var application = tobtabBLL.UpdateTgTrip(tgData);
            return Json(application, JsonRequestBehavior.AllowGet);

        }

        public List<SelectListItem> GetCountryDropDown()
        {
            RefCountryHelper refCountryHelper = new RefCountryHelper();
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
        public JsonResult addMarketingArea(string marketing_agent_ref, string person_identifier, string license_ref, string state_idx)
        {
            var userID = Session["UID"].ToString();

            if (marketing_agent_ref == null || marketing_agent_ref == "" || marketing_agent_ref == "undefined")
            {
                marketing_agent_ref = tobtabBLL.addMarketingArea(marketing_agent_ref, person_identifier, license_ref, state_idx, userID);
                return Json(marketing_agent_ref, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Boolean result = tobtabBLL.checkDuplicateState(marketing_agent_ref, state_idx);
                if (!result)
                {
                    marketing_agent_ref = tobtabBLL.addMarketingArea(marketing_agent_ref, person_identifier, license_ref, state_idx, userID);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ajaxAddSchedule(string tobtab_umrah_advertising_ref, string license_ref, System.DateTime schedule_start_date, System.DateTime schedule_end_date)
        {
            var userID = Session["UID"].ToString();
            //saveUmrahInfo();
            var Application = tobtabBLL.addSchedule(tobtab_umrah_advertising_ref, schedule_start_date, schedule_end_date, userID);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxAddLanguage(string tobtab_umrah_advertising_ref, string license_ref, string language_idx)
        {
            var userID = Session["UID"].ToString();
            //saveUmrahInfo();
            Boolean result = tobtabBLL.checkDuplicateLanguage(tobtab_umrah_advertising_ref, language_idx);
            if (!result)
            {
                var Application = tobtabBLL.addLanguage(tobtab_umrah_advertising_ref, license_ref, language_idx, userID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getTgTravelingList(string tgExceptionIdx)
        {
            var userID = Session["UID"].ToString();
            var Application = tobtabBLL.getTgTravelingList(tgExceptionIdx);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxUmrahAdvertiseList(string licensesIdx)
        {
            var userID = Session["UID"].ToString();
            var Application = tobtabBLL.getUmrahAdvertiseList(licensesIdx);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxMarketingAgentList(string licensesIdx)
        {
            var userID = Session["UID"].ToString();
            var Application = tobtabBLL.getMarketingAgentList(licensesIdx);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxApplication(string module, Guid OrganizationID)
        {
            var userID = Session["UID"].ToString();
            var Application = tobtabBLL.GetApplicationStatusMain(Session["UID"].ToString(), module, OrganizationID);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxApplicationMain(string module)
        {
            var userID = Session["UID"].ToString();
            var Application = tobtabBLL.GetApplicationMain(Session["UID"].ToString(), module);
            return Json(Application, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ajaxApplicationStatus(string module, string status)
        {
            var Application = tobtabBLL.GetApplicationStatus(module, status);
            return Json(Application, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ajaxChecklistUpdate(string id)
        {
            var Application = tobtabBLL.GetChecklistMain(id);
            return Json(Application, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxDeleteApplication(string id)
        {
            var Application = tobtabBLL.DeleteApplication(id);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxDeleteApplicationModule(string id, string module)
        {
            var Application = tobtabBLL.DeleteApplicationModule(id, module);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult deleteRecordTraveling(string travelingIdx)
        {
            var Application = tobtabBLL.deleteTravelingRecord(travelingIdx);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult deleteUmrahSchedule(string tobtab_umrah_schedule_idx)
        {
            var Application = tobtabBLL.deleteUmrahSchedule(tobtab_umrah_schedule_idx);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult deleteUmrahLanguage(string tobtab_umrah_language_idx)
        {
            var Application = tobtabBLL.deleteUmrahLanguage(tobtab_umrah_language_idx);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult deleteMarketingArea(string marketing_area_idx)
        {
            var Application = tobtabBLL.deleteMarketingArea(marketing_area_idx);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult deleteMarketingAgent(string marketing_agent_idx)
        {
            var Application = tobtabBLL.deleteMarketingAgent(marketing_agent_idx);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxUpdateStatus(string module_id, string component_id, short status)
        {
            var Application = tobtabBLL.UpdateChecklistStatus(module_id, component_id, status);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxUpdateAddField()
        {
            var userID = Session["UID"].ToString();
            Guid Id = new Guid(userID.ToString());
            var tobtab_idx = Request["license_ref"].ToString();
            var inbound = Request["inbound"].ToString();
            var outbound = Request["outbound"].ToString();
            var ticketing = Request["ticketing"].ToString();
            var umrah = Request["umrah"].ToString();

            var Application = tobtabBLL.UpdateAddField(tobtab_idx, inbound, outbound, ticketing, umrah, Id);

            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxLicenseHeader()
        {
            string userID = Session["UID"].ToString();
            var syarikat = tobtabBLL.GetTobtabHeader(userID);

            return Json(syarikat, JsonRequestBehavior.AllowGet);
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
        public JsonResult ajaxCheckApplicationStatus(string license_id)
        {
            var Application = tobtabBLL.CheckApplicationStatus(license_id);

            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxSyarikat(string sRegistrationNo)
        {
            var syarikat = tobtabBLL.GetCompany(sRegistrationNo);

            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxSaveTGException()
        {
            string Inbound = Request["Inbound"].ToString();
            string Outbound = Request["Outbound"].ToString();
            string Ticketing = Request["Ticketing"].ToString();
            string company_regno = Request["company_regno"].ToString();
            string company_inc_date = Request["company_inc_date"].ToString();
            string paid_capital = Request["paid_capital"].ToString();
            string allowed_capital = Request["allowed_capital"].ToString();
            string company_name = Request["company_name"].ToString();
            string company_addr_1 = Request["company_addr_1"].ToString();
            string company_addr_2 = Request["company_addr_2"].ToString();
            string company_addr_3 = Request["company_addr_3"].ToString();
            string company_postcode = Request["company_postcode"].ToString();
            string company_city = Request["company_city"].ToString();
            string company_state = Request["company_state"].ToString();
            string company_mobile_no = Request["company_mobile_no"].ToString();
            string company_phone_no = Request["company_phone_no"].ToString();
            string company_fax_no = Request["company_fax_no"].ToString();
            string company_email = Request["company_email"].ToString();

            string sc_name = Request["sc_name"].ToString();
            string sc_addr_1 = Request["sc_addr_1"].ToString();
            string sc_addr_2 = Request["sc_addr_2"].ToString();
            string sc_addr_3 = Request["sc_addr_3"].ToString();
            string sc_postcode = Request["sc_postcode"].ToString();
            string sc_city = Request["sc_city"].ToString();
            string sc_state = Request["sc_state"].ToString();
            string sc_mobile_no = Request["sc_mobile_no"].ToString();
            string sc_phone_no = Request["sc_phone_no"].ToString();
            string sc_fax_no = Request["sc_fax_no"].ToString();
            string sc_email = Request["sc_email"].ToString();

            string company_detail_addr_1 = Request["company_detail_addr_1"].ToString();
            string company_detail_addr_2 = Request["company_detail_addr_2"].ToString();
            string company_detail_addr_3 = Request["company_detail_addr_3"].ToString();
            string company_detail_postcode = Request["company_detail_postcode"].ToString();
            string company_detail_city = Request["company_detail_city"].ToString();
            string company_detail_state = Request["company_detail_state"].ToString();
            string company_detail_mobile_no = Request["company_detail_mobile_no"].ToString();
            string company_detail_phone_no = Request["company_detail_phone_no"].ToString();
            string company_detail_fax_no = Request["company_detail_fax_no"].ToString();
            string company_detail_email = Request["company_detail_email"].ToString();
            string company_detail_website = Request["company_detail_website"].ToString();

            string representative_name = Request["representative_name"].ToString();
            string representative_nric = Request["representative_nric"].ToString();
            string representative_company_name = Request["representative_company_name"].ToString();
            string representative_position = Request["representative_position"].ToString();

            string is_have_office = Request["is_have_office"].ToString();
            string signature_name = Request["signature_name"].ToString();
            string signature_icno = Request["signature_icno"].ToString();
            string signature_position = Request["signature_position"].ToString();

            string registration_status = Request["registration_status"].ToString();
            string appl_date = Request["appl_date"].ToString();

            //string refNo = "TB_" + DateTime.Now.ToString("yyyyMMdd") + "-" + RandomString(8);
            string Src = Request["Src"].ToString();

            string SrcDirector = Request["SrcDirector"].ToString();

            if (Src == "MS" || Src == "MS-UPD")
            {

                tb_main_model.user_idx = Guid.Parse(Session["UID"].ToString());
                //tb_main_model.ref_no = refNo;
                tb_main_model.appl_date = DateTime.Now;
                tb_main_model.p_inbound = (!string.IsNullOrEmpty(Inbound)) ? Inbound : "";
                tb_main_model.p_outbound = (!string.IsNullOrEmpty(Outbound)) ? Outbound : "";
                tb_main_model.p_ticketing = (!string.IsNullOrEmpty(Ticketing)) ? Ticketing : "";
                tb_main_model.company_regno = (!string.IsNullOrEmpty(company_regno)) ? company_regno : "";
                tb_main_model.company_inc_date = string.IsNullOrEmpty(company_inc_date) ? DateTime.Now : DateTime.Parse(company_inc_date);
                tb_main_model.paid_capital = (!string.IsNullOrEmpty(paid_capital)) ? Decimal.Parse(paid_capital) : Decimal.Parse("0");
                tb_main_model.allowed_capital = (!string.IsNullOrEmpty(allowed_capital)) ? Decimal.Parse(allowed_capital) : Decimal.Parse("0");

                tb_main_model.company_name = (!string.IsNullOrEmpty(company_name)) ? company_name : "";
                tb_main_model.company_addr_1 = (!string.IsNullOrEmpty(company_addr_1)) ? company_addr_1 : "";
                tb_main_model.company_addr_2 = (!string.IsNullOrEmpty(company_addr_2)) ? company_addr_2 : "";
                tb_main_model.company_addr_3 = (!string.IsNullOrEmpty(company_addr_3)) ? company_addr_3 : "";
                tb_main_model.company_postcode = (!string.IsNullOrEmpty(company_postcode)) ? company_postcode : "";
                tb_main_model.company_city = (!string.IsNullOrEmpty(company_city)) ? Guid.Parse(company_city) : Guid.Parse(null);
                tb_main_model.company_state = (!string.IsNullOrEmpty(company_state)) ? Guid.Parse(company_state) : Guid.Parse(null);
                tb_main_model.company_mobile_no = (!string.IsNullOrEmpty(company_mobile_no)) ? company_mobile_no : "";
                tb_main_model.company_phone_no = (!string.IsNullOrEmpty(company_phone_no)) ? company_phone_no : "";
                tb_main_model.company_fax_no = (!string.IsNullOrEmpty(company_fax_no)) ? company_fax_no : "";
                tb_main_model.company_email = (!string.IsNullOrEmpty(company_email)) ? company_email : "";

                tb_main_model.sc_name = (!string.IsNullOrEmpty(sc_name)) ? sc_name : "";
                tb_main_model.sc_addr_1 = (!string.IsNullOrEmpty(sc_addr_1)) ? sc_addr_1 : "";
                tb_main_model.sc_addr_2 = (!string.IsNullOrEmpty(sc_addr_2)) ? sc_addr_2 : "";
                tb_main_model.sc_addr_3 = (!string.IsNullOrEmpty(sc_addr_3)) ? sc_addr_3 : "";
                tb_main_model.sc_postcode = (!string.IsNullOrEmpty(sc_postcode)) ? sc_postcode : "";
                tb_main_model.sc_city = (!string.IsNullOrEmpty(sc_city)) ? Guid.Parse(sc_city) : Guid.Parse(null);
                tb_main_model.sc_state = (!string.IsNullOrEmpty(sc_state)) ? Guid.Parse(sc_state) : Guid.Parse(null);
                tb_main_model.sc_mobile_no = (!string.IsNullOrEmpty(sc_mobile_no)) ? sc_mobile_no : "";
                tb_main_model.sc_phone_no = (!string.IsNullOrEmpty(sc_phone_no)) ? sc_phone_no : "";
                tb_main_model.sc_fax_no = (!string.IsNullOrEmpty(sc_fax_no)) ? sc_fax_no : "";
                tb_main_model.sc_email = (!string.IsNullOrEmpty(sc_email)) ? sc_email : "";

                tb_main_model.company_detail_addr_1 = (!string.IsNullOrEmpty(company_detail_addr_1)) ? company_detail_addr_1 : "";
                tb_main_model.company_detail_addr_2 = (!string.IsNullOrEmpty(company_detail_addr_2)) ? company_detail_addr_2 : "";
                tb_main_model.company_detail_addr_3 = (!string.IsNullOrEmpty(company_detail_addr_3)) ? company_detail_addr_3 : "";
                tb_main_model.company_detail_postcode = (!string.IsNullOrEmpty(company_detail_postcode)) ? company_detail_postcode : "";
                tb_main_model.company_detail_city = (!string.IsNullOrEmpty(company_detail_city)) ? Guid.Parse(company_detail_city) : Guid.Parse(null);
                tb_main_model.company_detail_state = (!string.IsNullOrEmpty(company_detail_state)) ? Guid.Parse(company_detail_state) : Guid.Parse(null);
                tb_main_model.company_detail_mobile_no = (!string.IsNullOrEmpty(company_detail_mobile_no)) ? company_detail_mobile_no : "";
                tb_main_model.company_detail_phone_no = (!string.IsNullOrEmpty(company_detail_phone_no)) ? company_detail_phone_no : "";
                tb_main_model.company_detail_fax_no = (!string.IsNullOrEmpty(company_detail_fax_no)) ? company_detail_fax_no : "";
                tb_main_model.company_detail_email = (!string.IsNullOrEmpty(company_detail_email)) ? company_detail_email : "";
                tb_main_model.company_detail_website = (!string.IsNullOrEmpty(company_detail_website)) ? company_detail_website : "";

                tb_main_model.representative_name = (!string.IsNullOrEmpty(representative_name)) ? representative_name : "";
                tb_main_model.representative_nric = (!string.IsNullOrEmpty(representative_nric)) ? representative_nric : "";
                tb_main_model.representative_company_name = (!string.IsNullOrEmpty(representative_company_name)) ? representative_company_name : "";
                tb_main_model.representative_position = (!string.IsNullOrEmpty(representative_position)) ? representative_position : "";

                tb_main_model.is_have_office = (!string.IsNullOrEmpty(is_have_office)) ? bool.Parse(is_have_office) : false;
                tb_main_model.signature_name = (!string.IsNullOrEmpty(signature_name)) ? signature_name : "";
                tb_main_model.signature_icno = (!string.IsNullOrEmpty(signature_icno)) ? signature_icno : "";
                tb_main_model.signature_position = (!string.IsNullOrEmpty(signature_position)) ? signature_position : "";

                tb_main_model.registration_status = (!string.IsNullOrEmpty(registration_status)) ? Guid.Parse(registration_status) : Guid.Parse("0");
                tb_main_model.appl_date = (!string.IsNullOrEmpty(appl_date)) ? DateTime.Parse(appl_date) : DateTime.Now;

                var data = tobtabHelper.SaveNewTobtabMain(tb_main_model);

            }
            else if (SrcDirector == "BumiNon")
            {

            }
            else if (SrcDirector == "AseanNon")
            {

            }
            return Json(SrcDirector, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult ajaxPostSyarikat()
        {
            var organizationIdx = Request["organizationIdx"].ToString();

            var model = new TobtabViewModels.tobtab_core_organizations();
            //model.organization_identifier = sRegistrationNo;
            model.organization_idx = Guid.Parse(organizationIdx.ToString());

            model.registered_mobile_no = Request["syarikat_telHP"].ToString();
            model.registered_phone_no = Request["syarikat_telPejabat"].ToString();
            model.registered_fax_no = Request["syarikat_NoFaks"].ToString();
            model.registered_email = Request["syarikat_eMail"].ToString();

            model.cosec_mobile_no = Request["setiausahaSyarikat_telHP"].ToString();
            model.cosec_phone_no = Request["setiausahaSyarikat_telPejabat"].ToString();
            model.cosec_fax_no = Request["setiausahaSyarikat_NoFaks"].ToString();
            model.cosec_email = Request["setiausahaSyarikat_eMail"].ToString();
            model.cosec_addr_1 = Request["setiausahaSyarikat_AlamatBerdaftar"].ToString();
            model.cosec_addr_2 = Request["setiausahaSyarikat_AlamatBerdaftar1"].ToString();
            model.cosec_addr_3 = Request["setiausahaSyarikat_AlamatBerdaftar2"].ToString();
            model.cosec_postcode = Request["setiausahaSyarikat_poskod"].ToString();
            model.cosec_name = Request["setiausahaSyarikat_NamaPenuh"].ToString();
            var test = Request["setiausahaSyarikat_bandar"];
            if (Request["setiausahaSyarikat_bandar"] != "")
                model.cosec_city = Guid.Parse(Request["setiausahaSyarikat_bandar"]);
            else
                model.cosec_city = Guid.Empty;

            if (Request["setiausahaSyarikat_negeri_idx"] != null)
                model.cosec_state = Guid.Parse(Request["setiausahaSyarikat_negeri_idx"]);
            else
                model.cosec_state = Guid.Empty;

            model.is_has_business_address = (short?)((Request["maklumatPerniagaaan"].ToString() == "Ya") ? 1 : 0);

            model.office_mobile_no = Request["maklumatPerniagaaan_telHP"].ToString();
            model.office_phone_no = Request["maklumatPerniagaaan_telPejabat"].ToString();
            model.office_fax_no = Request["maklumatPerniagaaan_NoFaks"].ToString();
            model.office_email = Request["maklumatPerniagaaan_eMail"].ToString();
            model.website = Request["maklumatPerniagaaan_Web"].ToString();
            

            if (model.is_has_business_address == 0)
            {
                var a =Request["maklumatPerniagaaan_PBTNo"];
                model.office_state = Guid.Parse(Request["maklumatPerniagaaan_Negeri_no"]);
                model.office_city = Guid.Parse(Request["maklumatPerniagaaan_Bandar_no"]);
                model.office_mobile_no = Request["maklumatPerniagaaan_telHP_no"].ToString();
                model.office_email = Request["maklumatPerniagaaan_eMail_no"].ToString();
                model.pbt_ref = Guid.Parse(Request["maklumatPerniagaaan_PBTNo"]);
                model.office_postcode = Request["maklumatPerniagaaan_poskod_no"].ToString();
            }
            else
            {
                model.office_state = Guid.Parse(Request["maklumatPerniagaaan_Negeri"]);
                model.office_city = Guid.Parse(Request["maklumatPerniagaaan_Bandar"]);
                model.pbt_ref = Guid.Parse(Request["maklumatPerniagaaan_PBT"]);
                model.office_postcode = Request["maklumatPerniagaaan_poskod"].ToString();
            }

            var syarikat = tobtabBLL.UpdateCompany(model);
            tobtabBLL.updateInactiveDirectorShareholder(model.organization_idx);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetCawangan()
        {
            var sRegistrationNo = Request["sRegistrationNo"].ToString();
            var module = Request["module"].ToString();

            var syarikat = tobtabBLL.GetBidang(sRegistrationNo, module);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetBidang()
        {
            var sRegistrationNo = Request["sRegistrationNo"].ToString();
            var module = Request["module"].ToString();
            var OrganizationIdx = Request["OrganizationIdx"].ToString();

            var syarikat = tobtabBLL.GetBidang(sRegistrationNo, module);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetBidangCommon(string apply_idx, string modulename)
        {
            var flows = TourlistUnitOfWork.TobtabLicenses.Find(x => x.stub_ref.ToString() == apply_idx).FirstOrDefault();
            //var sRegistrationNo = Request["sRegistrationNo"].ToString();
            //var module = Request["module"].ToString();
            //var OrganizationIdx = Request["OrganizationIdx"].ToString();

            var syarikat = tobtabBLL.GetBidang(flows.tobtab_idx.ToString(), modulename);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxPostBidang()
        {
            var sRegistrationNo = Request["sRegistrationNo"].ToString();
            var module = Request["module"].ToString();

            var model = new TourlistDataLayer.DataModel.tobtab_licenses();
            model.outbound = byte.Parse(Request["outbound"].ToString());
            model.inbound = byte.Parse(Request["inbound"].ToString());
            model.ticketing = byte.Parse(Request["ticketing"].ToString());
            //model.umrah = byte.Parse(Request["umrah"].ToString());
            if (module == "TOBTAB_RENEW")
            {
                model.renewal_duration_years = int.Parse(Request["renewal_duration_years"].ToString());
            }
            else
            {
                model.renewal_duration_years = null;
            }

            var syarikat = tobtabBLL.UpdateBidang(sRegistrationNo, model);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxPostPerakuan()
        {
            var sRegistrationNo = Request["sRegistrationNo"].ToString();
            var module_id = Request["module_id"].ToString();

            var model = new TourlistDataLayer.DataModel.core_acknowledgements();
            model.acknowledge_person_name = Request["perakuanfullname"].ToString();
            model.acknowledge_person_icno = Request["perakuanic"].ToString();
            model.acknowledge_position = Request["perakuanposition"].ToString();
            model.acknowledge_organization_name = Request["perakuancompany"].ToString();
            model.is_acknowledged = short.Parse(Request["perakuancheck"].ToString());

            var syarikat = tobtabBLL.UpdatePerakuan(sRegistrationNo, model, module_id);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxLoadOverseasPartnerData()
        {
            var data_idx = Request["data_idx"].ToString();

            var data = tobtabBLL.GetOverseasPartnerInfo(data_idx);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        public JsonResult ajaxLoadOverseasPartnerDataCommon(string applyId)
        {
            var data_idx = applyId;

            var data = tobtabBLL.GetOverseasPartnerInfo(data_idx);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxLoadOverseasPackageData()
        {
            var data_idx = Request["data_idx"].ToString();

            var data = tobtabBLL.GetOverseasPackageInfo(data_idx);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetShareholder()
        {
            var data_idx = Request["data_idx"].ToString();

            var data = tobtabBLL.GetShareholderInfo(data_idx);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetMarketingAgents()
        {
            var data_idx = Request["data_idx"].ToString();

            var data = tobtabBLL.ajaxGetMarketingAgents(data_idx);

            var user = Session["UID"].ToString();
            var uploadFreeform = tobtabBLL.GetUploadFreeformByDataIdx(data_idx);

            var response = new
            {
                data,
                uploadFreeform,
            };

            return Json(response, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetDirector()
        {
            var data_idx = Request["data_idx"].ToString();

            //var directors = coreOrgHelper.GetDirectorDetail(identifier);
            var data = tobtabBLL.GetDirectorInfo(data_idx);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetDirectorCommon(String data_idx)
        {

            var data = tobtabBLL.GetDirectorInfo(data_idx);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxUploadDocumentPerson()
        {
            var data_idx = Request["data_idx"].ToString();
            var module = Request["module"].ToString();
            var doc_name = Request["name"].ToString();
            var license_ref = Request["license_ref"].ToString();
            string userID = Session["UID"].ToString();
            string fname = "";
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        doc_name = fname;
                        var fpath = this.GetUploadFolder(TourlistEnums.MotacModule.TOBTAB, fname); ///Url.Content("~/Attachment/Tobtab/" + fname);
                        //fname = System.IO.Path.Combine(Server.MapPath("~/Attachment/Tobtab/"), fname);

                        var data = tobtabBLL.UploadDocumentPerson(data_idx, fpath, module, doc_name, license_ref, userID); ;
                        //file.SaveAs(fname);
                        this.UploadSuppDocs(file, fname, TourlistEnums.MotacModule.TOBTAB);
                    }
                    return Json(fname);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }

        }

        [HttpPost]
        public JsonResult ajaxGetDocumentPerson()
        {
            var data_idx = Request["data_idx"].ToString();
            var module = Request["module"].ToString();
            var shareholder_identifier = Request["shareholder_identifier"].ToString();
            var license_ref = Request["license_ref"].ToString();


            var data = tobtabBLL.GetDocumentPerson(data_idx, module, shareholder_identifier, license_ref); ;


            return Json(data);

        }

        [HttpPost]
        public JsonResult ajaxUploadDocumentChecklist()
        {
            var data_idx = Request["data_idx"].ToString();
            var module = Request["module"].ToString();
            var doc_name = Request["name"].ToString();
            var app_id = Request["app_id"].ToString();
            var component_id = Request["component_id"].ToString();
            var sModuleDoc = "";

            if (module == "TOBTAB_NEW")
                sModuleDoc = "TOBTAB_NEW_DOKUMEN";
            else if (module == "TOBTAB_RENEW")
                sModuleDoc = "TOBTAB_RENEW_DOKUMEN";
            else if (module == "TOBTAB_ADD_BRANCH")
                sModuleDoc = "TOBTAB_ADD_BRANCH_DOCS";
            else if (module == "TOBTAB_CHANGE_STATUS")
                sModuleDoc = "MM2H_CHANGESTATUS_DOKUMEN";
            else if (module == "TOBTAB_RETURN_LICENSE")
                sModuleDoc = "TOBTAB_RETURN_LICENSE_DOCS";
            else if (module == "TOBTAB_RETURN_LICENSE")
                sModuleDoc = "TOBTAB_RETURN_LICENSE_DOCS";
            else if (module == "TOBTAB_TG_EXCEPTIONS")
                sModuleDoc = "TOBTAB_TG_EXCEPTIONS_DOCS";
            else if (module == "TOBTAB_UMRAH")
                sModuleDoc = "TOBTAB_UMRAH_DOCS";
            else if (module == "TOBTAB_MARKETING_OFFICER")
                sModuleDoc = "TOBTAB_MARKETING_OFFICERS_DOCS";
            else if (module == "TOBTAB_ADD_FIELD")
                sModuleDoc = "TOBTAB_ADD_FIELD_DOCS";
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        var fpath = this.GetUploadFolder(TourlistEnums.MotacModule.TOBTAB, fname); //Url.Content("~/Attachment/Tobtab/" + fname);
                        doc_name = fname;
                        //fname = System.IO.Path.Combine(Server.MapPath("~/Attachment/Tobtab/"), fname);

                        var data = tobtabBLL.UploadDocumentChecklist(data_idx, fpath, doc_name); ;
                        //file.SaveAs(fname);
                        this.UploadSuppDocs(file, doc_name, TourlistEnums.MotacModule.TOBTAB);
                    }
                    string userID = Session["UID"].ToString();
                    int iCount = tobtabBLL.checkDokumenSokongan(app_id, sModuleDoc, userID);

                    if (iCount == 0)
                    {
                        tobtabBLL.UpdateChecklistStatus(sModuleDoc, component_id, 1);
                        //coreOrgHelper.updateListing(itemID);
                    }
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }

        }


        [HttpPost]
        public JsonResult ajaxUpdatePostForeignPartner()
        {
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();
            var data_idx = Request["data_idx"].ToString();
            var country = Request["partnerv_country"].ToString();

            var model = new TourlistDataLayer.DataModel.tobtab_foreign_partners();
            model.foreign_partner_idx = Guid.Parse(data_idx);
            model.tobtab_application_ref = Guid.Parse(license_ref);
            model.foreign_partner_name = Request["partnerv_name"].ToString();
            model.office_addr_1 = Request["partnerv_addr1"].ToString();
            model.office_addr_2 = Request["partnerv_addr2"].ToString();
            model.office_addr_3 = Request["partnerv_addr3"].ToString();
            model.office_phone_no = Request["partnerv_phone"].ToString();
            model.office_postcode = Request["partnerv_postcode"].ToString();
            model.office_city = Request["partnerv_city"].ToString();
            model.office_state = Request["partnerv_states"].ToString();
            model.office_country = Guid.Parse(country);

            var syarikat = tobtabBLL.UpdateForeignPartner(model);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxPostForeignPartner()
        {
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();
            var country = Request["partner_country"].ToString();

            var model = new TourlistDataLayer.DataModel.tobtab_foreign_partners();
            model.foreign_partner_idx = Guid.NewGuid();
            model.tobtab_application_ref = Guid.Parse(license_ref);
            model.foreign_partner_name = Request["partner_name"].ToString();
            model.office_addr_1 = Request["partner_addr1"].ToString();
            model.office_addr_2 = Request["partner_addr2"].ToString();
            model.office_addr_3 = Request["partner_addr3"].ToString();
            model.office_phone_no = Request["partner_phone"].ToString();
            model.office_postcode = Request["partner_postcode"].ToString();
            model.office_city = Request["partner_city"].ToString();
            model.office_state = Request["partner_states"].ToString();
            model.office_country = Guid.Parse(country);

            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string fname;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }
                if (fname != null && fname != "")
                {
                    var fpath = this.GetUploadFolder(TourlistEnums.MotacModule.TOBTAB, fname); //Url.Content("~/Attachment/Tobtab/" + fname);
                    model.document_upload_location = fpath;
                    model.document_upload_name = fname;
                    //fname = System.IO.Path.Combine(Server.MapPath("~/Attachment/Tobtab/"), fname);
                    //file.SaveAs(fname);
                    this.UploadSuppDocs(file, fname, TourlistEnums.MotacModule.TOBTAB);
                }
            }
            var syarikat = tobtabBLL.AddForeignPartner(model);
            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxPostForeignPackages()
        {
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();
            var country = Request["packages_country"].ToString();

            RefCountryHelper countryHelper = new RefCountryHelper();
            var country_name = countryHelper.GetCountryName(country);

            var model = new TourlistDataLayer.DataModel.tobtab_foreign_packages();
            model.foreign_paackage_idx = Guid.NewGuid();
            model.tobtab_application_ref = Guid.Parse(license_ref);
            model.foreign_application_description = country_name;

            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string fname;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                }
                if (fname != null && fname != "")
                {
                    var fpath = this.GetUploadFolder(TourlistEnums.MotacModule.TOBTAB, fname); //Url.Content("~/Attachment/Tobtab/" + fname);
                    model.document_upload_location = fpath;
                    model.document_upload_name = fname;
                    //fname = System.IO.Path.Combine(Server.MapPath("~/Attachment/Tobtab/"), fname);
                    //file.SaveAs(fname);
                    this.UploadSuppDocs(file, fname, TourlistEnums.MotacModule.TOBTAB);
                }
            }

            var syarikat = tobtabBLL.AddForeignPackages(model);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        public void saveUmrahInfo()
        {
            string userID = Session["UID"].ToString();
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();
            var tobtab_umrah_advertising_ref = Request["tobtab_umrah_advertising_ref"];
            var NoLesen = Request["NoLesen"].ToString();
            var TarikTamatLesen = Request["TarikTamatLesen"].ToString();
            var TajukIklan = Request["TajukIklan"].ToString();
            var TermaIklan = Request["TermaIklan"].ToString();
            var hargaPakejFrom = Request["hargaPakejFrom"].ToString();
            var hargaPakejTo = Request["hargaPakejTo"].ToString();
            //var jadualPerjalanan = Request["jadualPerjalanan"].ToString(); // log no 32213

            var model = new TourlistDataLayer.DataModel.tobtab_umrah_advertising();
            model.tobtab_umrah_advertising_idx = Guid.Parse(tobtab_umrah_advertising_ref);
            model.tobtab_licenses_ref = Guid.Parse(license_ref);
            //model.no_lesen = NoLesen;
            //model.license_expiry_date = DateTime.Parse(TarikTamatLesen);
            model.advertise_title = TajukIklan;
            model.advertise_terms_conditions = TermaIklan;
            //model.trip_schedule = jadualPerjalanan;
            model.payment_package_madefrom = decimal.Parse(hargaPakejFrom);
            model.payment_package_madeto = decimal.Parse(hargaPakejTo);
            model.created_at = DateTime.Now;
            model.created_by = Guid.Parse(userID);
            //pending schedule
            var syarikat = tobtabBLL.AddNewPermohonanIklan(model);
        }
        [HttpPost]
        public JsonResult ajaxPostAdvertising()
        {
            saveUmrahInfo();
            return Json("success", JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxPostShareholderPersonInfo()
        {
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();
            var shareholder_number_of_shares = Request["shareholder_number_of_shares"].ToString();

            RefCountryHelper countryHelper = new RefCountryHelper();
            var nationality = countryHelper.GetCountryGuid(Request["shareholder_nationality"].ToString());

            RefStatesHelper statesHelper = new RefStatesHelper();
            var states = statesHelper.GetStatesGuid(Request["shareholder_states"].ToString());
            DateTime shareholder_birthday = DateTime.Parse(Request["shareholder_birthday"].ToString());

            var model = new TourlistDataLayer.DataModel.core_persons();
            model.person_name = Request["shareholder_fullname"].ToString();
            model.person_identifier = Request["shareholder_identifier"].ToString();
            model.personal_mobile_no = Request["shareholder_mobile_no"].ToString();
            model.person_birthday = shareholder_birthday;
            model.residential_addr_1 = Request["shareholder_addr_1"].ToString();
            model.residential_addr_2 = Request["shareholder_addr_2"].ToString();
            model.residential_addr_3 = Request["shareholder_addr_3"].ToString();
            model.person_age = int.Parse(Request["shareholder_age"].ToString());
            model.person_gender = Guid.Parse(Request["shareholder_gender"].ToString());
            if (Request["shareholder_religion"] != null && Request["shareholder_religion"].ToString() != "")
            {
                model.person_religion = Guid.Parse(Request["shareholder_religion"].ToString());
            }
            model.residential_postcode = Request["shareholder_postcode"].ToString();
            model.person_nationality = nationality;

            model.residential_city = refGeoHelper.GetGuidTownByCode(Request["shareholder_city"].ToString());
            model.residential_state = states;
            model.person_is_bumiputera = short.Parse(Request["shareholder_isbumi"].ToString());
            var status_shareholder = Request["status_shareholder"].ToString();

            var syarikat = tobtabBLL.AddShareholderPersonInfo(company_id, license_ref, shareholder_number_of_shares, model, status_shareholder);

            int shareholder = coreOrgHelper.CheckShareHolder(company_id);

            if (shareholder == 0)
            {
                tobtabBLL.UpdateChecklistStatus(module_id, component_id, 1);
                //coreOrgHelper.updateListing(itemIDx);
            }

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxTempPostMarketingAgentPersonInfo()
        {
            var userID = Session["UID"].ToString();
            TourlistDataLayer.DataModel.core_users user = tobtabBLL.GetUserd(userID);
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();

            var model = new TourlistDataLayer.DataModel.core_persons();
            model.person_name = Request["shareholder_fullname"].ToString();
            model.person_identifier = Request["shareholder_identifier"].ToString();
            var personIdx = tobtabBLL.tempAddMarketingAgentPersonInfo(company_id, license_ref, model);
            coreOrgHelper.updateListing(component_id);
            return Json(personIdx, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult ajaxPostMarketingAgentPersonInfo()
        {
            var userID = Session["UID"].ToString();
            TourlistDataLayer.DataModel.core_users user = tobtabBLL.GetUserd(userID);
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();

            RefStatesHelper statesHelper = new RefStatesHelper();
            var states = statesHelper.GetStatesGuid(Request["shareholder_states"].ToString());
            DateTime shareholder_birthday = DateTime.Parse(Request["shareholder_birthday"].ToString());

            var model = new TourlistDataLayer.DataModel.core_persons();
            model.person_name = Request["shareholder_fullname"].ToString();
            model.person_identifier = Request["shareholder_identifier"].ToString();
            model.personal_mobile_no = Request["shareholder_mobile_no"].ToString();
            model.person_birthday = shareholder_birthday;
            model.residential_addr_1 = Request["shareholder_addr_1"].ToString();
            model.residential_addr_2 = Request["shareholder_addr_2"].ToString();
            model.residential_addr_3 = Request["shareholder_addr_3"].ToString();
            model.residential_postcode = Request["shareholder_postcode"].ToString();
            model.residential_city = refGeoHelper.GetGuidTownByCode(Request["shareholder_city"].ToString());
            model.residential_state = states;
            model.residential_email = Request["shareholder_email"].ToString();
            model.residential_phone_no = Request["shareholder_mobile_no"].ToString();
            model.contact_addr_1 = Request["shareholder_addr_1"].ToString();
            model.contact_addr_2 = Request["shareholder_addr_2"].ToString();
            model.contact_addr_3 = Request["shareholder_addr_3"].ToString();
            model.contact_postcode = Request["shareholder_postcode"].ToString();
            model.contact_city = refGeoHelper.GetGuidTownByCode(Request["shareholder_city"].ToString());
            model.contact_state = states;
            model.contact_email = Request["shareholder_email"].ToString();
            model.contact_mobile_no = Request["shareholder_mobile_no"].ToString();
            model.contact_phone_no = Request["shareholder_mobile_no"].ToString();
            model.person_nationality = Guid.Parse(Request["shareholder_nationality"].ToString());
            model.created_by = Guid.Parse(user.person_ref.ToString());
            model.person_age = int.Parse(Request["shareholder_age"].ToString());
            model.person_gender = Guid.Parse(Request["shareholder_gender"].ToString());
            model.person_religion = Guid.Parse(Request["shareholder_religion"].ToString());

            model.person_employ_position = Request["person_employ_position"].ToString();
            model.person_is_bumiputera = short.Parse(Request["shareholder_isbumi"].ToString());
            model.person_is_employed = short.Parse(Request["person_is_employed"].ToString());
            if (model.person_is_employed == 1 && Request["office_state"].ToString() != "")
            {
                //model.person_employer_organization = Request["person_employer_organization"].ToString();
                var officestates = statesHelper.GetStatesGuid(Request["office_state"].ToString());
                model.office_name = Request["office_name"].ToString();
                model.office_addr_1 = Request["office_addr_1"].ToString();
                model.office_addr_2 = Request["office_addr_2"].ToString();
                model.office_addr_3 = Request["office_addr_3"].ToString();
                model.office_postcode = Request["office_postcode"].ToString();
                model.office_city = refGeoHelper.GetGuidTownByCode(Request["office_city"].ToString());// Guid.Parse(Request["office_city"].ToString());
                model.office_state = officestates;
                model.office_mobile_no = Request["office_mobile_no"].ToString();
                model.office_phone_no = Request["office_mobile_no"].ToString();
                model.office_email = Request["office_email"].ToString();
                //var status_shareholder = Request["status_shareholder"].ToString();
            }
            var marketing_agent_idx = tobtabBLL.AddMarketingAgentPersonInfo(company_id, license_ref, model, userID);
            coreOrgHelper.updateListing(component_id);
            return Json(marketing_agent_idx, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxSaveTGOrganizer()
        {
            var userID = Session["UID"].ToString();
            TourlistDataLayer.DataModel.core_users user = tobtabBLL.GetUserd(userID);
            //var module_id = Request["module_id"].ToString();
            var itemId = Request["itemId"].ToString();
            var AppID = Request["AppID"].ToString();
            var license_id = Request["license_id"].ToString();

            /*RefCountryHelper countryHelper = new RefCountryHelper();
            var nationality = countryHelper.GetCountryGuid(Request["shareholder_nationality"].ToString());*/

            RefStatesHelper statesHelper = new RefStatesHelper();
            var model = tobtabBLL.getTgExceptionInfo(license_id);
            if (model == null)
            {
                model = new TourlistDataLayer.DataModel.tobtab_tg_exceptions();
            }
            model.company_representative_name = Request["company_representative_name"].ToString();
            model.company_representative_phone_no = Request["company_representative_phone_no"].ToString();
            model.bus_owner = Request["bus_owner"].ToString();
            //model.person_birthday = shareholder_birthday;
            model.bus_owner_kpl_licenses = Request["bus_owner_kpl_licenses"].ToString();
            model.bus_no_plate = Request["bus_no_plate"].ToString();
            model.bus_seat_load = Request["bus_seat_load"].ToString();
            model.bus_total_passenger = Request["bus_total_passenger"].ToString();
            model.created_by = Guid.Parse(user.person_ref.ToString());
            model.created_at = DateTime.Now;
            model.is_organizer = short.Parse(Request["is_organizer"].ToString());
            model.tobtab_licenses_ref = Guid.Parse(license_id);
            model.organizer_email = Request["tg_email"].ToString();
            if (model.is_organizer == 1)
            {
                model.organizer_name = Request["organizer_name"].ToString();
                model.organizer_addr_1 = Request["organizer_addr_1"].ToString();
                model.organizer_addr_2 = Request["organizer_addr_2"].ToString();
                model.organizer_addr_3 = Request["organizer_addr_3"].ToString();
                model.organizer_postcode = Request["organizer_postcode"].ToString();
                model.organizer_city = refGeoHelper.GetGuidTownByCode(Request["organizer_city"].ToString());
                var states = statesHelper.GetStatesGuid(Request["organizer_state"].ToString());
                model.organizer_state = states;
                model.organizer_mobile_no = Request["organizer_mobile_no"].ToString();
                model.organizer_phone_no = Request["organizer_phone_no"].ToString();
                model.organizer_email = Request["organizer_email"].ToString();
            }
            var syarikat = tobtabBLL.AddOrganizerInfo(itemId, license_id, model);
            coreOrgHelper.updateListing(itemId);
            //tobtabBLL.UpdateChecklistStatus(module_id, component_id, 1);
            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult ajaxPostDirectorPersonInfo()
        {
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();
            var company_id = Request["company_id"].ToString();
            var license_ref = Request["license_ref"].ToString();

            //RefCountryHelper countryHelper = new RefCountryHelper();
            //var nationality = countryHelper.GetCountryGuid(Request["director_nationality"].ToString());

            RefStatesHelper statesHelper = new RefStatesHelper();
            var states = statesHelper.GetStatesGuid(Request["director_states"].ToString());
            DateTime director_birthday = DateTime.Parse(Request["director_birthday"].ToString());

            var model = new TourlistDataLayer.DataModel.core_persons();
            model.person_identifier = Request["director_identifier"].ToString();
            model.personal_mobile_no = Request["director_mobile_no"].ToString();
            model.residential_phone_no = Request["director_phone_no"].ToString();
            model.person_birthday = director_birthday;
            model.person_age = int.Parse(Request["director_age"].ToString());
            model.person_gender = Guid.Parse(Request["director_gender"].ToString());
            model.person_nationality = Guid.Parse(Request["director_nationality"].ToString());
            model.person_religion = Guid.Parse(Request["director_religion"].ToString());

            var syarikat = tobtabBLL.AddDirectorPersonInfo(company_id, license_ref, model);

            int shareholder = coreOrgHelper.CheckDirector(company_id);
            if (shareholder == 0)
            {
                tobtabBLL.UpdateChecklistStatus(module_id, component_id, 1);
                //coreOrgHelper.updateListing(itemIDx);
            }
            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxGetPerakuan(string stubRef)
        {
            var syarikat = tobtabBLL.GetPerakuan(stubRef);

            return Json(syarikat, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxLoadOverseasPartner(string company_id, string license_ref)
        {
            var data = tobtabBLL.GetOverseasPartner(company_id, license_ref);

            return Json(data, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult getUmrahLanguage(string tobtab_umrah_advertising_ref, string license_ref)
        {
            var data = tobtabBLL.getUmrahLanguage(tobtab_umrah_advertising_ref);

            return Json(data, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult getUmrahSchedule(string tobtab_umrah_advertising_ref, string license_ref)
        {
            var data = tobtabBLL.getUmrahSchedule(tobtab_umrah_advertising_ref);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult getMarketingArea(string marketing_agent_ref, string license_ref)
        {
            var data = tobtabBLL.getMarketingArea(marketing_agent_ref);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxLoadOverseasPackages(string company_id, string license_ref)
        {
            var data = tobtabBLL.GetOverseasPackages(company_id, license_ref);

            return Json(data, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxShareHolderDetail(string identifier)
        {
            var ShareHolder = coreOrgHelper.GetShareHolderDetail(identifier);
            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ajaxChangeStatusShareHolderBaru(string AppID)
        {
            var ShareHolder = coreOrgHelper.GetChangeStatusShareHolder(AppID);

            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxShareholderInfo(string company_id, string license_ref)
        {
            // var Application = tobtabBLL.GetShareholder(company_id, license_ref);
            var ShareHolder = coreOrgHelper.GetShareHolderList(company_id);


            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ajaxDirector(string OrganizationID)
        {
            var directors = coreOrgHelper.GetDirector(OrganizationID);

            return Json(directors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxDirectorDetail(string identifier)
        {
            var directors = coreOrgHelper.GetDirectorDetail(identifier);

            return Json(directors, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxDirectorInfo(string company_id, string license_ref)
        {
            var Application = tobtabBLL.GetDirector(company_id, license_ref);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ajaxUpdateForeignStatus()
        {

            var license_ref = Request["license_ref"].ToString();

            var model = new TourlistDataLayer.DataModel.tobtab_licenses();

            model.foreign_relations = byte.Parse(Request["status"].ToString());
            if (license_ref != "00000000-0000-0000-0000-000000000000")
            {
                var Application = tobtabBLL.UpdateForeignStatus(license_ref, model);
                return Json(Application, JsonRequestBehavior.DenyGet);
            }

            return Json(model, JsonRequestBehavior.DenyGet);
        }


        [HttpPost]
        public JsonResult ajaxUpdateCompanyDataSSM()
        {

            var userID = Session["UID"].ToString();
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TobtabViewModels.tobtab_ssm_organization();

            model_org.date_of_change = Request["date_of_change"].ToString();
            model_org.status = Request["status"].ToString();
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

            model_org.capital_ordinary_cash = Request["capital_ordinary_cash"].ToString();
            model_org.capital_ordinary_otherwise = Request["capital_ordinary_otherwise"].ToString();
            model_org.capital_preference_cash = Request["capital_preference_cash"].ToString();
            model_org.capital_preference_otherwise = Request["capital_preference_otherwise"].ToString();
            model_org.capital_others_cash = Request["capital_others_cash"].ToString();
            model_org.capital_others_otherwise = Request["capital_others_otherwise"].ToString();

            var Application = coreOrgHelper.UpdateCompanyDataSSM(module_id, userID, component_id, model_org, "TOBTAB");

            return Json(Application, JsonRequestBehavior.DenyGet);
        }



        #region tambah cawangan

        [HttpPost]
        public void ajaxSaveBranch(TobtabViewModels.tobtab_branch branch, string status, string itemIDx, string license_no)
        {

            var userID = Session["UID"].ToString();
            branch.user_id = userID;
            branch.branch_license_ref_code = tobtabBLL.GetBranchLicenseNo(license_no, branch.OrganizationID);
            bool ID = false;
            if (status == "Add")
                ID = tobtabBLL.AddBranch_SaveNew(branch);
            else
                ID = tobtabBLL.UpdateAddBranch(branch);


            coreOrgHelper.updateListing(itemIDx);


        }
        #endregion
        [HttpPost]
        public JsonResult ajaxLicense(string OrganizationID)
        {

            var syarikat = tobtabBLL.GetTobtablicence(OrganizationID);

            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ajaxUpdateCompanyDirectorsSSM()
        {


            var userID = Session["UID"].ToString();
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TobtabViewModels.tobtab_ssm_directors();
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
                model_org.director_date_appointed = Request["director_date_appointed"].ToString();
                model_org.director_designation = Request["director_designation"].ToString();
                if (Request["director_idType"] != null)
                {
                    model_org.director_idType = Request["director_idType"].ToString();
                }

                Application = coreOrgHelper.ajaxUpdateCompanyDirectorsSSM(module_id, component_id, userID, model_org, "TOBTAB");
            }


            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ajaxUpdateCompanyShareholdersSSM()
        {

            var userID = Session["UID"].ToString();
            var module_id = Request["module_id"].ToString();
            var component_id = Request["component_id"].ToString();

            var model_org = new TobtabViewModels.tobtab_ssm_shareholders();
            var Application = "";

            if (Request["shareholder_name"] != null)
            {

                model_org.shareholder_name = Request["shareholder_name"].ToString();
                model_org.shareholder_docno = Request["shareholder_docno"].ToString();
                model_org.shareholder_totalshare = Request["shareholder_totalshare"].ToString();
                model_org.shareholder_idType = Request["shareholder_idtype"].ToString();

                Application = coreOrgHelper.ajaxUpdateCompanyShareholdersSSM(module_id, component_id, userID, model_org, "TOBTAB");
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
                Application = coreOrgHelper.ajaxUpdateCompanyChargersSSM(organization_id, charger.charge_num, charger, Guid.Parse(userID));
            }


            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        public JsonResult ajaxCancelLicense(TobtabViewModels.tobtab_terminate terminate, string ItemList, string sType, string OrganizationID,string terminate_idx)
        {
            string userID = Session["UID"].ToString();
            terminate.user_id = userID;
            bool doc = false;
            if (terminate_idx == "" || sType== "Branch")
            {
                doc = tobtabBLL.CancelLicense(terminate, sType, OrganizationID);
            }
            else
            {
                doc = tobtabBLL.updateCancelLicense(terminate, sType, OrganizationID,terminate_idx);
            }
           
            coreOrgHelper.updateListing(ItemList);
            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ajaxCancelBidang(string apply_idx, string Module, string RegistrationNo, bool outbound, bool inbound, bool ticketing, bool umrah)
        {
            bool sts = false;
            var sRegistrationNo = RegistrationNo;
            var module = Module;
            
            var syarikat = tobtabBLL.GetBidang(sRegistrationNo, module);

            int ret = tobtabBLL.CancelBidang(syarikat[0], Guid.Parse(apply_idx), outbound, inbound, ticketing, umrah, Guid.Parse(Session["UID"].ToString()));
            sts = ret > 0 ? true : false;

            return Json(sts, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxAddBranch(string OrganizationID)
        {
            var AddBranch = tobtabBLL.GetBranchList(OrganizationID);

            return Json(AddBranch, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxCheckSerahBatalLesen(string OrganizationID)
        {
            var hq = tobtabBLL.GetTerminateLicenseHQ(OrganizationID);

            return Json(hq, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ajaxAddBranchList(string OrganizationID, string module)
        {
            var AddBranch = tobtabBLL.GetBranch(OrganizationID, module);

            return Json(AddBranch, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxAddBranchListByStubRef(string OrganizationID, string module, string applyIdx)
        {
            var AddBranch = tobtabBLL.GetBranch(OrganizationID, module, applyIdx);

            return Json(AddBranch, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ajaxDeleteBranch(string idx, string itemID, string organizationID, string module)
        {

            bool doc = tobtabBLL.DeleteBranch(idx, itemID, organizationID, module);

            return Json(doc, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#57258)  on 10 Jan 2024
        [HttpGet]
        public JsonResult TOBTABBranchesActive(Guid Org_ref)
        {
            var branches = tobtabBLL.GetTOBTABBranchesActive(Org_ref);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#57258)  on 10 Jan 2024
        [HttpGet]
        public JsonResult TOBTABBranchesbyBranchIdx(Guid branches_idx)
        {
            var branches = tobtabBLL.GetTobtabBranchesbyBranchIdx(branches_idx);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxAddBranchDetail(string branches_idx)
        {
            var AddBranchDetail = tobtabBLL.GetBranchDetail(branches_idx);

            return Json(AddBranchDetail, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region dropdown

        public List<SelectListItem> GetPemegangSahamDropDown(string orgID)
        {
            var ShareHolder = coreOrgHelper.GetShareHolder(orgID);
            List<SelectListItem> PemegangSaham = new List<SelectListItem>();

            foreach (var app in ShareHolder)
            {
                if (app.person_name != "")
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
        public List<SelectListItem> GetDaerahDropDown()
        {
            List<SelectListItem> city = new List<SelectListItem>();
            city.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });

            var data = refGeoHelper.GetDDLDistrictsList();
            data = data.OrderBy(o => o.district_name).ToList();
            foreach (var app in data)
            {
                city.Add(new SelectListItem
                {
                    Value = app.district_idx.ToString(),
                    Text = app.district_name,
                });
            }

            return city;
        }
        public List<SelectListItem> GetStatusPemeganganSahamDropDown(string pemegangsaham)
        {

            List<SelectListItem> StatusDropdown = new List<SelectListItem>();
            var status = coreOrgHelper.GetRefListByType("STATUSSHAREDHOLDER");
            StatusDropdown.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });
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
        public List<SelectListItem> GetAgamaDropDown()
        {
            List<SelectListItem> ReligionDropdown = new List<SelectListItem>();
            var religion = refHelper.GetRefListByTypeOrderByTextField("RELIGION");
            ReligionDropdown.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });
            foreach (var app in religion)
            {
                ReligionDropdown.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description,
                });
            }
            return ReligionDropdown;
        }
        public List<SelectListItem> GetWorldDropDown()
        {
            List<SelectListItem> World = new List<SelectListItem>();
            World.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });

            var data = refGeoHelper.GetDDLCountryList();
            data = data.OrderBy(o => o.country_name).ToList();
            foreach (var app in data)
            {
                World.Add(new SelectListItem
                {
                    Value = app.country_idx.ToString(),
                    Text = app.country_name,
                });
            }

            return World;
        }

        public List<SelectListItem> GetLanguageDown()
        {
            List<SelectListItem> language = new List<SelectListItem>();
            language.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });

            var lang = refHelper.GetRefListByType("LANGUAGE");
            foreach (var app in lang)
            {
                language.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description,
                });
            }

            return language;
        }
        public List<SelectListItem> GeStateBranchDropDown(Guid organization_ref)
        {
            List<SelectListItem> State = new List<SelectListItem>();
            State.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });

            var data = refGeoHelper.GetStateByTobtabBranch(organization_ref);
            data = data.OrderBy(o => o.state_name).ToList();
            string value1 = "";
            string value2 = "";
            foreach (var app in data)
            {
                value1 = app.state_idx.ToString();
                if (value1 != value2)
                {
                    State.Add(new SelectListItem
                    {
                        Value = app.state_idx.ToString(),
                        Text = app.state_name,
                    });
                }
                value2 = app.state_idx.ToString();
            }
            return State;
        }
        public List<SelectListItem> GeStateDropDown()
        {
            List<SelectListItem> State = new List<SelectListItem>();
            State.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });

            var data = refGeoHelper.GetDDLStatesList();
            data = data.OrderBy(o => o.state_name).ToList();
            foreach (var app in data)
            {
                State.Add(new SelectListItem
                {
                    Value = app.state_idx.ToString(),
                    Text = app.state_name,
                });
            }

            return State;
        }
        public List<SelectListItem> GetPurposeDropDown()
        {
            List<SelectListItem> purpose = new List<SelectListItem>();
            purpose.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });

            var data = refHelper.GetRefListByType("TGEXEMPTION");
            data = data.OrderBy(o => o.ref_description).ToList();
            foreach (var app in data)
            {
                purpose.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description,
                });
            }

            return purpose;
        }


        RefCountryHelper refCountryHelper = new RefCountryHelper();
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

        public List<SelectListItem> GetAseanDropDown()
        {
            List<SelectListItem> World = new List<SelectListItem>();

            var data = refGeoHelper.GetAseanList();
            data = data.OrderBy(o => o.country_name).ToList();
            foreach (var app in data)
            {
                World.Add(new SelectListItem
                {
                    Value = app.country_idx.ToString(),
                    Text = app.country_name,
                });
            }

            return World;
        }

        public List<SelectListItem> GetNonAseanDropDown()
        {
            List<SelectListItem> World = new List<SelectListItem>();

            var data = refGeoHelper.GetNonAseanList();
            data = data.OrderBy(o => o.country_name).ToList();
            foreach (var app in data)
            {
                World.Add(new SelectListItem
                {
                    Value = app.country_idx.ToString(),
                    Text = app.country_name,
                });
            }

            return World;
        }
        public List<SelectListItem> GetPengarahDropDown(string company_id, string license_ref)
        {
            List<SelectListItem> Pengarah = new List<SelectListItem>();

            var Directors = coreOrgHelper.GetDirectorPerakuan(company_id);
            Pengarah.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });
            if (Directors != null)
            {
                foreach (var director in Directors)
                {
                    Pengarah.Add(new SelectListItem
                    {
                        Value = director.person_idx.ToString(),
                        Text = director.person_name,
                    });
                }
            }

            return Pengarah;
        }
        public List<SelectListItem> GetLesenDropDown(string userID)
        {
            List<SelectListItem> Lesen = new List<SelectListItem>();

            var licenses = tobtabBLL.GetLicenses(Guid.Parse(userID));

            foreach (var app in licenses)
            {
                Lesen.Add(new SelectListItem
                {
                    Value = app.tobtab_idx.ToString(),
                    Text = app.license_ref_code,
                });
            }

            //Lesen.Add(new SelectListItem
            //{
            //    Value = "KPK/LN2287",
            //    Text = "KPK/LN2287",
            //});
            //Lesen.Add(new SelectListItem
            //{
            //    Value = "KPK/LN2288",
            //    Text = "KPK/LN2288",
            //});
            //Lesen.Add(new SelectListItem
            //{
            //    Value = "KPK/LN2289",
            //    Text = "KPK/LN2289",
            //});

            return Lesen;
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

        public List<SelectListItem> GetTempohPembaharuanDropDown()
        {
            List<SelectListItem> Lesen = new List<SelectListItem>();

            Lesen.Add(new SelectListItem
            {
                Value = "1 Tahun",
                Text = "1 Tahun",
            });
            Lesen.Add(new SelectListItem
            {
                Value = "2 Tahun",
                Text = "2 Tahun",
            });
            Lesen.Add(new SelectListItem
            {
                Value = "3 Tahun",
                Text = "3 Tahun",
            });

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
        public List<SelectListItem> GetGender()
        {
            List<SelectListItem> Gender = new List<SelectListItem>();
            var data = refHelper.GetRefListByType("GENDER");
            Gender.Add(new SelectListItem
            {
                Value = "",
                Text = "Sila Pilih",
            });
            foreach (var app in data)
            {
                if(app!=null && (app.ref_code=="MALE" || app.ref_code == "FEMALE"))
                {
                    Gender.Add(new SelectListItem
                    {
                        Value = app.ref_idx.ToString(),
                        Text = app.ref_description.ToUpper(),
                    });
                }
            }
            return Gender;
        }
        #endregion

        public bool SendApplicationRefNo(TourlistDataLayer.DataModel.vw_tobtab_application app)
        {
            bool result = false;
            try
            {
                //String applicationNo = app.application_no;
                var email = "";
                if (app.office_email != null)
                {
                    email = app.office_email;
                }
                else
                {
                    email = app.registered_email;
                }
                var subject = "Notifikasi Pendaftaran Sebagai Pengguna Sistem Pelesenan & Penguatkuasaan Pelancongan (TOURLIST)";
                var receiver = email.ToString().Trim();

                var scheme = HttpContext.Request.Url.Scheme;
                var host = HttpContext.Request.Url.Host;
                var port = HttpContext.Request.Url.Port;
                //var activationcode = idx.ToString();
                var namaCompany = "Nama Syarikat :" + app.organization_name;
                var applicationNo = "No Rujukan Permohonan :" + app.application_no;
                //var verifyUrl = scheme + "://" + host + ":" + port + "/Account/ActivateAccount/?activationCode=" + activationcode;
                string content = "NO RUJUKKAN PERMOHONAN\n\n" +
                              "\nTourList telah menerima Permohonan Agensi Pelancongan - Lesen Baru anda. " +
                              "No Rujukan Permohonan ini juga telah dihantar ke E-Mel berikut.\n" +
                              "\nE-mail : " + receiver + " \n\n";
                content = content + namaCompany + "\n";
                content = content + applicationNo + "\n";
                content = content + "\nGunakan No Rujukkan Permohonan ini untuk menyemak atau mengemaskini permohonan anda." +
                              "\nNo Rujukkan Permohonan ini akan digunakan sepanjang proses permohonan. Setiap Permohonan mempunyai satu nombor rujukkan.";

                result = Utils.EmailHelper.SendEmail(subject, content, receiver);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
            return result;
        }

        [HttpPost]
        public JsonResult ajaxOrganizationUpdate(Guid AppID, string module)
        {
            // var syarikat = mM2HHelper.GetCompany(sRegistrationNo);

            //  var syarikat = coreOrgHelper.GetOrganizationUpdate(AppID, module);
            var syarikat = coreOrgHelper.GetOrganizationUpdateByCode(AppID, module);



            return Json(syarikat, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public JsonResult ajaxChangeStatusUpdate(core_organizations_updated orgUpdate, HttpPostedFileBase file_PerjanjianSewaBeliPermis, HttpPostedFileBase file_PelanLantaiPermis, HttpPostedFileBase file_salinanAsalLesen, string module, HttpPostedFileBase file_PerakuanPendaftar, string ItemID)
        //{
        //    string userID = Session["UID"].ToString();

        //    Guid gUserID = Guid.Parse(userID);

        //    bool upd = coreOrgHelper.UpdateChangeStatusOrg(orgUpdate, gUserID);
        //    Guid chkitem_instance = Guid.Empty;

        //    if (upd == true)
        //    {
        //        if (file_PerjanjianSewaBeliPermis != null)
        //        {
        //            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdx(2, orgUpdate.stub_ref, module);

        //            uploadDocSokongan(file_PerjanjianSewaBeliPermis, chkitem_instance);
        //        }

        //        if (file_PelanLantaiPermis != null)
        //        {
        //            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdx(3, orgUpdate.stub_ref, module);
        //            uploadDocSokongan(file_PelanLantaiPermis, chkitem_instance);
        //        }

        //        if (file_salinanAsalLesen != null)
        //        {
        //            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdx(0, orgUpdate.stub_ref, module);
        //            uploadDocSokongan(file_salinanAsalLesen, chkitem_instance);
        //        }

        //        if (file_PerakuanPendaftar != null)
        //        {

        //            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdx(5, orgUpdate.stub_ref, module);
        //            uploadDocSokongan(file_PerakuanPendaftar, chkitem_instance);
        //        }

        //        bool itemID = coreOrgHelper.updateListing(ItemID);

        //    }
        //    return Json(upd, JsonRequestBehavior.AllowGet);
        //}

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

        [HttpPost]
        public JsonResult ajaxChangeStatusShareholder(Guid orgID, Guid appID)
        {
            string userID = Session["UID"].ToString();
            Guid gUserID = Guid.Parse(userID);

            // coreOrgHelper.ChangeStatusOrganization_SaveNew(orgID, appID, gUserID);
            coreOrgHelper.ChangeStatusShareHolder_SaveNew(orgID, appID, gUserID);

            bool upd = true;
            return Json(upd, JsonRequestBehavior.AllowGet);
        }


        public DateTime JsonDateTimeToNormal(string jsonDateTime)
        {
            jsonDateTime = @"""" + jsonDateTime + @"""";
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(jsonDateTime);
        }

        public ActionResult LoadUploadPicture(List<HttpPostedFileBase> file_Dokumen, string marketing_agent_idx)
        {
            var agent = tobtabBLL.GetMarketingAgent(marketing_agent_idx);
            Guid uid = Guid.Parse(Session["UID"].ToString());
            core_users coreUser = coreHelper.GetCoreUserByGuid(uid);
            string personRef = "";
            if (agent != null)
            {
                personRef = agent.person_ref.ToString();
            }
            else
            {
                personRef = Session["UID"].ToString(); //tumpang
            }
            if (file_Dokumen != null)
            {
                int filecount = file_Dokumen.Count();

                for (int i = 0; i < filecount; i++)
                {
                    if (file_Dokumen[i] != null)
                    {
                        //uploadPicture(file_Dokumen[i], coreUser);

                        this.UploadProfileImage(file_Dokumen[i], file_Dokumen[i].FileName, Session["UID"].ToString());

                    }
                }
            }
            var filenm = this.GetUploadImage(Session["UID"].ToString(), file_Dokumen[0].FileName);//a
            // Guid person_ref = Guid.Parse(coreUser.person_ref.ToString());
            var ma = TourlistUnitOfWork.RefReferencesTypesRepository.Find(x => x.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var ma_ref = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ma.ref_idx && x.ref_code == "MARKETING_AGENT").FirstOrDefault();
            var ma1 = ma_ref != null ? ma_ref.ref_description : "";
            var ma2 = ma_ref != null ? ma_ref.ref_idx : Guid.Empty;

            var objRef = new core_uploads_freeform_by_persons();
            objRef.person_ref = Guid.Parse(personRef);
            objRef.upload_path = filenm;
            objRef.upload_name = file_Dokumen[0].FileName;
            objRef.upload_type_ref = ma2;
            objRef.upload_description = ma1;
            /*objRef.person_id_upload = file_Dokumen[0].FileName;*/
            objRef.modified_by = uid;
            objRef.modified_at = DateTime.Now;
            /*ViewBag.Picture_FileId = file_Dokumen[0].FileName;*/
            var data = tobtabBLL.UpdateUserProfilePic(objRef);

            var persons = new core_persons();
            persons.person_idx = Guid.Parse(personRef);
            persons.person_photo_upload = filenm;
            persons.person_id_upload = file_Dokumen[0].FileName;
            persons.modified_by = uid;
            persons.modified_dt = DateTime.Now;
            /*ViewBag.Picture_FileId = file_Dokumen[0].FileName;*/
            var data2 = tobtabBLL.UpdatePersonPath(persons);



            return Json(filenm, JsonRequestBehavior.AllowGet);
        }

        public string GetUploadImage(string userid, string filename)
        {
            var motacModule = Tourlist.Common.TourlistEnums.MotacModule.TOBTAB;
            string foldersave = Server.MapPath("~/Attachment/" + motacModule.ToString() + "/" + userid);
            if (!Directory.Exists(foldersave))
                Directory.CreateDirectory(foldersave);

            return "/Attachment/" + motacModule.ToString() + "/" + userid + "/" + filename;
        }

        public bool UploadProfileImage(HttpPostedFileBase file, string uploadFileName, string userid)
        {
            try
            {
                string filename = uploadFileName;
                string foldersave = "";
                var motacModule = Tourlist.Common.TourlistEnums.MotacModule.TOBTAB;
                foldersave = Server.MapPath("~/Attachment/" + motacModule.ToString() + "/" + userid);
                if (!Directory.Exists(foldersave))
                    Directory.CreateDirectory(foldersave);

                using (Stream fs = file.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bytes = br.ReadBytes((Int32)fs.Length);
                        file.SaveAs(Path.Combine(foldersave, filename));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }

            return true;
        }

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
                if (is_premise != null)
                {


                    if (is_premise == 0)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", orgUpdate.stub_ref, "tobtab"); //"Perjanjian Sewa Beli Premis"
                        RemoveDocList(chkitem_instance);
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", orgUpdate.stub_ref, "tobtab"); //"Perjanjian Sewa Beli Premis"
                        RemoveDocList(chkitem_instance);
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", orgUpdate.stub_ref, "tobtab"); //"Perjanjian Sewa Beli Premis"
                        RemoveDocList(chkitem_instance);
                    }
                    else
                    {
                        if (file_PerjanjianSewaBeliPermis != null)
                        {
                            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", orgUpdate.stub_ref, "tobtab"); //"Perjanjian Sewa Beli Premis"
                            uploadDocSokongan(file_PerjanjianSewaBeliPermis, chkitem_instance);
                        }
                        if (file_PelanLantaiPermis != null)
                        {
                            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", orgUpdate.stub_ref, "tobtab");//"Pelan Lantai Premis Perniagaan"
                            uploadDocSokongan(file_PelanLantaiPermis, chkitem_instance);
                        }
                        if (file_GambarPermis != null)
                        {
                            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", orgUpdate.stub_ref, "tobtab");//"Salinan Lesen *"
                            uploadDocSokongan(file_GambarPermis, chkitem_instance);
                        }
                    }
                }
                if (file_salinanAsalLesen != null)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SALINANLESEN", orgUpdate.stub_ref, "tobtab");//"Salinan Lesen *"
                    uploadDocSokongan(file_salinanAsalLesen, chkitem_instance);
                }
                bool itemID = coreOrgHelper.updateListing(ItemID);


            }



            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri. temporary for debugging
        internal void WriteLog(string log)
        {
            var fileName = Path.Combine(Server.MapPath("~/Logs/"), "motacDebug_" + DateTime.Now.ToString("ddMMyyyy") + ".log");
            //string fileName = @"D:\Motac\motacDebug_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                writer.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " - " + log);
                writer.Close();
            }
        }

        //added by samsuri (CR#57258)  on 10 jan 2024
        [HttpPost]
        public JsonResult ajaxChangeStatusBranchUpdate(core_organizations_updated orgUpdate, tobtab_add_branches_updated tobtabUpdate,
            HttpPostedFileBase file_PerjanjianSewaBeliPermis, HttpPostedFileBase file_PelanLantaiPermis,
            HttpPostedFileBase file_salinanAsalLesen, HttpPostedFileBase file_GambarPermis, string ItemID, string branch_stub_ref)
        {
            string userID = Session["UID"].ToString();

            Guid gUserID = Guid.Parse(userID);
            var is_premise = orgUpdate.is_premise_ready;
            //tobtabUpdate.stub_ref = Guid.Parse(branch_stub_ref);
            WriteLog("ilpUpdate.stub_ref:" + tobtabUpdate.stub_ref);
            WriteLog("orgUpdate.stub_ref:" + orgUpdate.stub_ref);


            TourlistUnitOfWork.TobtabAddBranchesUpdatedRepository.SaveNewTobtabBranch(tobtabUpdate, gUserID);
            
            bool upd = coreOrgHelper.UpdateChangeStatusOrgforBranchInd(orgUpdate, gUserID);

            Guid chkitem_instance = Guid.Empty;

            Guid branch_stub_ref_idx = Guid.Parse(branch_stub_ref);
            WriteLog("tobtab branch_stub_ref_idx:" + branch_stub_ref_idx);
            uploadBranchUpdatedDoc(branch_stub_ref_idx, gUserID);
            
            //WriteLog("upd:" + upd + " is_premise:" + is_premise                     
            //        + " branch_stub_ref_idx:" + branch_stub_ref_idx
            //        + " tobtabUpdate.stub_ref:"+ tobtabUpdate.stub_ref);
            
            if (upd == true)
            {

                if (is_premise == 0)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "tobtab"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "tobtab"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "tobtab"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                }
                else
                {
                    if (file_PerjanjianSewaBeliPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "tobtab"); //"Perjanjian Sewa Beli Premis"
                        uploadDocSokongan(file_PerjanjianSewaBeliPermis, chkitem_instance);
                        //WriteLog("chkitem_instance SEWABELI:" + chkitem_instance);
                    }
                    if (file_PelanLantaiPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "tobtab");//"Pelan Lantai Premis Perniagaan"
                        uploadDocSokongan(file_PelanLantaiPermis, chkitem_instance);
                    }
                    if (file_GambarPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "tobtab");//"Salinan Lesen *"
                        uploadDocSokongan(file_GambarPermis, chkitem_instance);
                    }
                }
                if (file_salinanAsalLesen != null)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SALINANLESEN", branch_stub_ref_idx, "tobtab");//"Salinan Lesen *"
                    uploadDocSokongan(file_salinanAsalLesen, chkitem_instance);
                }
                bool itemID = coreOrgHelper.updateListing(ItemID);
            }

            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri on 9 jan 2024
        private void uploadBranchUpdatedDoc(Guid branch_stub_ref_idx, Guid userIdx)
        {

            //make sure supporting_document_list has value or generate new one.
            var TobtabBranchLic = TourlistUnitOfWork.TobtabLicenses.GetTobtabLicenseByStubRef(branch_stub_ref_idx);
            var TT_supporting_document_list = Guid.Empty;
            if (TobtabBranchLic.supporting_document_list == null)
            {
                TT_supporting_document_list = TourlistUnitOfWork.TobtabLicenses.UpdateSupportingDocList(TobtabBranchLic.tobtab_idx);
            }
            else TT_supporting_document_list = (Guid)TobtabBranchLic.supporting_document_list;

            Guid chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "tobtab");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = coreOrgHelper.GetcoreChkitemsInstancesByCode("SEWABELI", "tobtab");//Guid.Parse("ABD910AE-66E5-46C7-953C-7D5F89D9CFC2"); //SEWABELI                
                WriteLog("core_Chkitems_Instances.chklist_tplt_item_ref:" + core_Chkitems_Instances.chklist_tplt_item_ref);
                core_Chkitems_Instances.chklist_instance_ref = TT_supporting_document_list;
                core_Chkitems_Instances.bool1 = 0;
                core_Chkitems_Instances.active_status = 1;
                core_Chkitems_Instances.created_at = DateTime.Now;
                core_Chkitems_Instances.modified_at = DateTime.Now;
                core_Chkitems_Instances.created_by = userIdx;
                core_Chkitems_Instances.modified_by = userIdx;
                TourlistUnitOfWork.CoreChkItemsInstancesRepository.Add(core_Chkitems_Instances);
                TourlistContext.SaveChanges();
                WriteLog("SEWABELI SaveChanges done, TT_supporting_document_list:" + TT_supporting_document_list);
            }

            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "tobtab");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = coreOrgHelper.GetcoreChkitemsInstancesByCode("PELANLANTAI", "tobtab");//Guid.Parse("BE7F9E08-936A-4964-A817-E8762B639C9F"); //PELANLANTAI                
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

            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "tobtab");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = coreOrgHelper.GetcoreChkitemsInstancesByCode("GAMBAR", "tobtab");//Guid.Parse("7BC79CDE-4CC8-42B4-B626-D66A80E1A173"); //GAMBAR                
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

            //if (ID)
            //{
            //    //use common upload in base contoller
            //    this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.MM2H);

            //}
        }

        [HttpPost]
        public JsonResult ajaxPBT(string NegeriID)
        {

            var PBT = coreOrgHelper.GetRefPBT(NegeriID);

            return Json(PBT, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxUpdateApplicationStatus(string license_id, byte status, string stsref, string module)
        {
            var userID = Session["UID"];
            Guid Id = new Guid(userID.ToString());
            //checking if status Draft & generateAppNo
            var tobtabLicense = tobtabBLL.getTobtabLicense(license_id);
            coreHelper.generateApplicationNo(tobtabLicense.stub_ref, Id);

            var Application = tobtabBLL.UpdateApplicationStatus(license_id, status, Guid.Parse(stsref), module, Id);
            var user = coreHelper.GetCoreUserByGuid(Id);
            var app = new vw_tobtab_application();
            app = tobtabBLL.GetVwTobtabApplication(license_id);
            //SendApplicationRefNo(app);
            if (module == "TOBTAB_RENEW")
            {
                var coreLicense = coreHelper.getCoreLicense(app.organization_ref);
                DateTime now = DateTime.Now;
                if (coreLicense != null && coreLicense.end_dt != null)
                {
                    DateTime end = (DateTime)coreLicense.end_dt;
                    int day = (now - end).Days;
                    if (day > 30)
                    {
                        var compound = coreHelper.getCompoundByStubRef(app.apply_idx, end);
                        if (compound == null)
                        {
                            string refCode = "COMP_ACT_TOBTAB";
                            coreHelper.saveCompoundExpiredDate(app.apply_idx, Id, end, refCode);
                        }
                    }

                }
            }
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

    }
}