using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourlistWeb.Filters;
using TourlistWebAPI.ClassLib;
using TourlistWebAPI.DataModels;
using TourlistBusinessLayer.Models;
using TourlistDataLayer.DataModel;
using System.IO;
using Newtonsoft.Json.Linq;
using TourlistDataLayer.ViewModels.Tobtab;
using TourlistWebAPI.Models;
using Tourlist.Common;
using TourlistBusinessLayer.BLL;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using TourlistDataLayer.ViewModels.Ilp;
using Microsoft.Ajax.Utilities;
using System.ComponentModel.Design;

namespace TourlistWeb.Controllers
{
    [SessionTimeout]
    public class ILPController : TourlistController
    {

        TourlistBusinessLayer.BLL.IlpBLL ilpBLL = new TourlistBusinessLayer.BLL.IlpBLL();
        List<SelectListItem> shareholders = new List<SelectListItem>();
        TourlistWebAPI.ClassLib.CoreOrganizationHelper coreOrgHelper = new TourlistWebAPI.ClassLib.CoreOrganizationHelper();
        TourlistWebAPI.ClassLib.CoreHelper coreHelper = new TourlistWebAPI.ClassLib.CoreHelper();
        RefHelper refHelper = new RefHelper();
        RefCountryHelper refCountryHelper = new RefCountryHelper();

        public ActionResult ILPIndex()
        {
            licenseCheck();
            if (ViewBag.PermohonanBaharuExist != null && ViewBag.PermohonanBaharuExist == "Deraf" || ViewBag.PermohonanBaharuExist.Contains("Menunggu") || ViewBag.PermohonanBaharuExist.Contains("Dihantar") || ViewBag.PermohonanBaharuExist.Contains("Penyediaan Premis") || ViewBag.PermohonanBaharuExist.Contains("Selesai")) return RedirectToAction("LesenBaharuIndex");
            if (ViewBag.PembaharuanLesenExist != null && ViewBag.PembaharuanLesenExist == "Deraf" || ViewBag.PembaharuanLesenExist.Contains("Menunggu") || ViewBag.PembaharuanLesenExist.Contains("Dihantar")) return RedirectToAction("PembaharuanLesenIndex");
            if (ViewBag.TambahCawanganExist != null && ViewBag.TambahCawanganExist == "Deraf" || ViewBag.TambahCawanganExist.Contains("Menunggu") || ViewBag.TambahCawanganExist.Contains("Dihantar")) return RedirectToAction("TambahCawanganIndex");
            if (ViewBag.TukarStatusExist != null && ViewBag.TukarStatusExist == "Deraf" || ViewBag.TukarStatusExist.Contains("Menunggu") || ViewBag.TukarStatusExist.Contains("Dihantar")) return RedirectToAction("TukarStatusIndex");
            if (ViewBag.PermitMengajarExist != null && ViewBag.PermitMengajarExist == "Deraf" || ViewBag.PermitMengajarExist.Contains("Menunggu") || ViewBag.PermitMengajarExist.Contains("Dihantar")) return RedirectToAction("PermitMengajarIndex");
            if (ViewBag.SerahLesenExist != null && ViewBag.SerahLesenExist == "Deraf" || ViewBag.SerahLesenExist.Contains("Menunggu") || ViewBag.SerahLesenExist.Contains("Dihantar")) return RedirectToAction("SerahBatalLesenIndex");

            return View();
        }

        public void licenseCheck()
        {
            var lesenBaharu = ilpBLL.getLicenseStatus(Guid.Parse(Session["UID"].ToString()), "ILP_NEW");
            var pembaharuanLesen = ilpBLL.getLicenseStatus(Guid.Parse(Session["UID"].ToString()), "ILP_RENEW");
            var tambahCawangan = ilpBLL.getLicenseStatus(Guid.Parse(Session["UID"].ToString()), "ILP_ADDBRANCH");
            var tukarStatus = ilpBLL.getLicenseStatus(Guid.Parse(Session["UID"].ToString()), "ILP_CHANGESTATUS");
            var permitMengajar = ilpBLL.getLicenseStatus(Guid.Parse(Session["UID"].ToString()), "ILP_PERMIT");
            var serahLesen = ilpBLL.getLicenseStatus(Guid.Parse(Session["UID"].ToString()), "ILP_CANCELLICENSE");

            ViewBag.PermohonanBaharuExist = lesenBaharu;
            ViewBag.PembaharuanLesenExist = pembaharuanLesen;
            ViewBag.TambahCawanganExist = tambahCawangan;
            ViewBag.TukarStatusExist = tukarStatus;
            ViewBag.PermitMengajarExist = permitMengajar;
            ViewBag.SerahLesenExist = serahLesen;

            string userID = Session["UID"].ToString();
            var organization = ilpBLL.GetOrganization(userID);
            ViewBag.SSMNo = organization.organization_identifier;

            List<TourlistDataLayer.DataModel.core_license> clsLicense = new List<TourlistDataLayer.DataModel.core_license>();
            clsLicense = coreOrgHelper.GetCoreLicenseNo(organization.organization_idx.ToString());
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
            ViewBag.endDatePopup = "0";
            ViewBag.renewPopup = "0";
            var coreLicense = coreOrgHelper.GetCoreLicens(organization.organization_idx.ToString());
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


            ViewBag.ilp_branch = ilpBLL.GetILPStatusModul(organization.organization_idx.ToString(), "ILP_ADDBRANCH");

            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_ADDBRANCH");
            List<IlpViewModels.ilp_application> app = new List<IlpViewModels.ilp_application>();
            string smodule = "ILP_ADDBRANCH";
            string sstatus = "";
            string sAppID = "";
            string status_code = "";
            string appDate = "";
            app = ilpBLL.GetApplicationStatusMain(userID, null, organization.organization_idx);
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

            List<IlpViewModels.ilp_application> latestApp = new List<IlpViewModels.ilp_application>();

            latestApp = ilpBLL.GetApplicationStatusMain1(userID, null);

            string latestStatus = "";
            string latestModule = "";
            string latestAppNo = "";
            if (latestApp.Count > 0)
            {
                foreach (var st in latestApp)
                {
                    latestStatus = st.status_code.ToString();
                    latestModule = st.module_name.ToString();
                    if (st.application_no != null)
                        latestAppNo = st.application_no.ToString();
                    else
                        latestAppNo = st.application_no;
                    break;
                }

            }
            ViewBag.latestStatus = latestStatus;
            ViewBag.latestModule = latestModule;
            ViewBag.latestAppNo = latestAppNo;

        }

        public ActionResult ILPDokumen(Guid chkitem_instance_idx, Guid application_ref, String license_type, Guid stubid)
        {

            var license1 = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), license_type);
            ViewBag.StatusCode = license1.application_status_code;

            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.id = stubid;
            ViewBag.license_type = license_type;
            ViewBag.application_status_code = ilpBLL.GetApplicationStatusCode(stubid);
            string userID = Session["UID"].ToString();

            // int iCount = ilpBLL.checkDokumenSokongan(stubid.ToString(), license1.application_module, userID);
            int iCount = ilpBLL.checkDokumenSokongan(stubid.ToString(), "ILP_DOKUMEN", userID);

            if (iCount == 0)
            {

                coreOrgHelper.updateListing(chkitem_instance_idx.ToString());
            }

            var license = ilpBLL.GetPermohonanBaharuByIdx(Guid.Parse(Request["application_ref"].ToString()));
            var checklist = ilpBLL.GetIlpChecklistDokumen(license.supporting_document_list);
            var list = checklist.OrderBy(c => c.order);

            int iCountItem = 0;
            foreach (var item in checklist)
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

            return View();
        }

        public ActionResult LesenBaharuIndex(string lesenBaharu)
        {
            licenseCheck();
            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_NEW");
            ViewBag.code_setup = license.application_status_code;
            ViewBag.id = license.stub_ref;
            ViewBag.OrgID = license.organization_ref;
            //if (license.active_status == 0)
            //if (license.ilp_idx == Guid.Empty)
            if (lesenBaharu=="True")
            {
                //ilpBLL.CreateLesenBaharu(Guid.Parse(Session["UID"].ToString()), "PERMOHONAN BAHARU", "ILP Permohonan Baharu", "ILP Dokumen Sokongan", "ILP_NEW");
                var userID = Session["UID"];
                Guid Id = new Guid(userID.ToString());
                var user = coreHelper.GetCoreUserByGuid(Id);
                if (user != null && user.user_organization == null)
                {
                    user.user_organization = user.person_ref;
                }
                var stub = coreHelper.GenerateApplicationStubs(
                       Tourlist.Common.TourlistEnums.MotacModule.ILP,
                       Tourlist.Common.TourlistEnums.ModuleLicenseType.TIADA,
                       Tourlist.Common.TourlistEnums.SolModulesType.ILP_NEW,
                       Tourlist.Common.TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                         user);
            }
            ViewBag.Licenses = license;
            var checklist = ilpBLL.GetIlpChecklist(license.stub_ref, "ILP_PERMOHONAN_BAHARU", license.application_status_code);
            ViewBag.Checklist = checklist.OrderBy(c => c.order);
            ViewBag.OpenChecklist = 1;
            ViewBag.ReadOnly = false;
            ViewBag.Module = license.application_module;

            return View();
        }
        public JsonResult ajaxDeleteApplication(string id)
        {
            var Application = ilpBLL.DeleteApplication(id);
            return Json(Application, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public ActionResult ILPChecklist(Guid application_ref, String license_type, String page, string StatusCode)
        {
            licenseCheck();
            var license = ilpBLL.GetPermohonanBaharuByIdx(application_ref);

            var checklist = ilpBLL.GetIlpChecklist(license.stub_ref, license_type, StatusCode);
            ViewBag.Checklist = checklist.OrderBy(c => c.order);
            ViewBag.Licenses = license;
            ViewBag.OpenChecklist = 1;
            return View(page);
        }

        [HttpGet]
        public ActionResult ILPChecklistAddBranch(Guid application_ref, String license_type, String page, string StatusCode, string AppNo)
        {
            licenseCheck();
            var license = ilpBLL.GetPermohonanBaharuByIdx(application_ref);

            var checklist = ilpBLL.GetIlpChecklist(license.stub_ref, license_type, StatusCode);
            ViewBag.Checklist = checklist.OrderBy(c => c.order);
            ViewBag.Licenses = license;
            ViewBag.AppNo = AppNo;
            ViewBag.OpenChecklist = 1;
            return View(page);
        }

        [HttpPost]
        public JsonResult ILPDocumentChecklist(Guid application_ref)
        {

            var license = ilpBLL.GetPermohonanBaharuByIdx(Guid.Parse(Request["application_ref"].ToString()));
            var checklist = ilpBLL.GetIlpChecklistDokumen(license.supporting_document_list);
            var list = checklist.OrderBy(c => c.order);
            Object[] obj = {
                new { list }
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPostcodeByIdx()
        {
            var postcode = ilpBLL.GetPostcodeByIdx(Guid.Parse(Request["postcode_idx"]));
            if (postcode == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(postcode, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TownAndStateList()
        {
            var postcode = ilpBLL.GetPostcodeByCode(Request["postcode"].ToString());
            if (postcode == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var town = ilpBLL.GetTown(postcode.postcode_code);
            var state = ilpBLL.GetState(town.district_ref);
            Object[] obj = {
                new { postcode, town, state }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetOrganization()
        {
            var organization = ilpBLL.GetOrganization(Guid.Parse(Session["UID"].ToString()));
            Object[] obj = {
                organization
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserOrganization()
        {
            var organization = ilpBLL.GetUserOrganization(Guid.Parse(Session["UID"].ToString()));
            var ilp_idx = Request["ilp_idx"];
            Object[] obj = {
                organization
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string UpdateCompanyDetails(TourlistDataLayer.DataModel.core_organizations orgData)
        {
            var user_idx = Guid.Parse(Session["UID"].ToString());

            return ilpBLL.UpdateCompanyDetails(orgData, user_idx);
        }

        [HttpPost]
        public bool UpdateChecklistStatus()
        {
            var ilp_idx = Request["ilp_idx"];
            var module_id = Request["module_id"];
            if (ilp_idx != null && module_id != null && module_id == "ILP_ADDBRANCH")
            {
                var ilpBranch = ilpBLL.GetIlpBranches(Guid.Parse(ilp_idx)).ToList();
                if (ilpBranch != null && ilpBranch.Count > 0)
                {
                    ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));
                }
            }
            else
            {
                ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));
            }
            return true;
        }

        [HttpPost]
        public bool submitPermohonan()
        {
            var userID = Session["UID"];
            Guid Id = new Guid(userID.ToString());
            //checking if status Draft & generateAppNo
            coreHelper.generateApplicationNo(Guid.Parse(Request["stub_ref"].ToString()), Id);
            //ilpBLL.UpdatePermohonanStatus(Guid.Parse(Request["stub_ref"].ToString()));
            var app = ilpBLL.GetApplicationStubs(Guid.Parse(Request["stub_ref"].ToString()));
            var solModule = ilpBLL.GetSolModule(app.apply_module);
            if (solModule.module_name == "ILP_RENEW")
            {
                ilpBLL.UpdatePermohonanStatus(Guid.Parse(Request["stub_ref"].ToString()));
                TourlistWebAPI.ClassLib.CoreHelper coreHelper = new TourlistWebAPI.ClassLib.CoreHelper();
                var ilpLicense = ilpBLL.GeIlpLicense(Guid.Parse(Request["stub_ref"].ToString()));
                string orgRef = ilpLicense.organization_ref.ToString();
                var coreLicense = coreHelper.getCoreLicense(Guid.Parse(orgRef));
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
                            string refCode = "COMP_ACT_ILP";
                            coreHelper.saveCompoundExpiredDate(app.apply_idx, Id, end, refCode);
                        }
                    }
                }
            }
            else if (solModule.module_name == "ILP_PERMIT" || solModule.module_name == "ILP_CANCELLICENSE" || solModule.module_name == "ILP_CHANGESTATUS")
            {
                ilpBLL.UpdatePermohonanInprocess(Guid.Parse(Request["stub_ref"].ToString()));
            }
            else
            {
                ilpBLL.UpdatePermohonanStatus(Guid.Parse(Request["stub_ref"].ToString()));
            }

            ilpBLL.insertAuditTrailApplication(Guid.Parse(Request["stub_ref"].ToString()), Id);

            return true;
        }

        public bool StorePerakuan()
        {
            Acknowledgement coreAcknowledgement = new Acknowledgement();
            coreAcknowledgement.acknowledge_person_name = Request["acknowledge_person_name"];
            coreAcknowledgement.acknowledge_person_icno = Request["acknowledge_person_icno"];
            coreAcknowledgement.acknowledge_position = Request["acknowledge_position"];
            coreAcknowledgement.acknowledge_organization_name = Request["acknowledge_organization_name"];
            coreAcknowledgement.is_acknowledged = short.Parse(Request["is_acknowledged"]);
            coreAcknowledgement.license_type_ref = Guid.Parse(Request["license_type_ref"].ToString());
            coreAcknowledgement.stub_ref = Guid.Parse(Request["stub_ref"].ToString());
            ilpBLL.StoreAcknowledgement(coreAcknowledgement, Guid.Parse(Session["UID"].ToString()));
            ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));
            return true;
        }

        [HttpPost]
        public JsonResult GetPerakuan()
        {
            var acknowledgement = ilpBLL.GetAcknowledgement(Guid.Parse(Request["license_type_ref"].ToString()));
            Object[] obj = {
                new { acknowledgement }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LesenBaharuCreate()
        {
            return View();
        }

        public ActionResult LesenBaharuSaham(Guid chkitem_instance_idx, Guid application_ref, String license_type, Guid stubid)
        {
            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.license_type = license_type;
            ViewBag.application_status_code = ilpBLL.GetApplicationStatusCode(stubid);
            return View();
        }

        [HttpGet]
        public JsonResult Shareholders()
        {
            var shareholders = ilpBLL.GetShareholders(Guid.Parse(Session["UID"].ToString()));
            Object[] obj = {
                new { shareholders }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ShareholdersCommon(string organization)
        {
            Guid Useridx = TourlistUnitOfWork.CoreUsersRepository.Find(x => x.user_organization.ToString() == organization).Select(x => x.user_idx).FirstOrDefault();
            var shareholders = ilpBLL.GetShareholders(Useridx);
            Object[] obj = {
                new { shareholders }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LesenBaharuPengarah(Guid chkitem_instance_idx, Guid application_ref, String license_type, Guid stubid)
        {
            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.license_type = license_type;
            ViewBag.application_status_code = ilpBLL.GetApplicationStatusCode(stubid);
            return View();
        }

        [HttpGet]
        public JsonResult Directors()
        {
            var directors = ilpBLL.GetDirectors(Guid.Parse(Session["UID"].ToString()));
            Object[] obj = {
                new { directors }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LesenBaharuPengajar(Guid chkitem_instance_idx, Guid application_ref, String license_type, Guid stubid)
        {
            var newLicense = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_NEW");

            ViewBag.new_license = newLicense.ilp_idx;
            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.license_type = license_type;
            ViewBag.application_status_code = ilpBLL.GetApplicationStatusCode(stubid);
            return View();
        }

        [HttpPost]
        public bool StoreLesenBaharuMaklumatSyarikat()
        {
            Organization coreOrganizations = new Organization();
            coreOrganizations.incorporation_date = DateTime.ParseExact(Request["incorporation_date"].ToString(), "dd/MM/yyyy", null);
            coreOrganizations.registered_addr_1 = Request["registered_addr_1"].ToString();
            coreOrganizations.registered_addr_2 = Request["registered_addr_2"].ToString();
            coreOrganizations.registered_addr_3 = Request["registered_addr_3"].ToString();
            coreOrganizations.registered_postcode = Request["registered_postcode"].ToString();
            coreOrganizations.registered_town = Request["registered_town"].ToString();
            coreOrganizations.office_addr_1 = Request["office_addr_1"].ToString();
            coreOrganizations.office_addr_2 = Request["office_addr_2"].ToString();
            coreOrganizations.office_addr_3 = Request["office_addr_3"].ToString();
            coreOrganizations.office_postcode = Request["office_postcode"].ToString();
            coreOrganizations.office_town = Request["office_town"].ToString();
            //coreOrganizations.cosec_addr_1 = Request["cosec_addr_1"].ToString();
            //coreOrganizations.cosec_addr_2 = Request["cosec_addr_2"].ToString();
            //coreOrganizations.cosec_addr_3 = Request["cosec_addr_3"].ToString();
            //coreOrganizations.cosec_postcode = Request["cosec_postcode"].ToString();
            //coreOrganizations.cosec_name = Request["cosec_name"].ToString();
            coreOrganizations.company_regno = Request["company_regno"].ToString();
            coreOrganizations.company_newregno = Request["company_newregno"].ToString();
            ilpBLL.CreateOrganization(coreOrganizations, Request["organization_identifier"].ToString());
            Session["CompRegNo"] = coreOrganizations.company_newregno;
            return true;
        }

        [HttpPost]
        public bool UpdateLesenBaharuMaklumatSyarikat()
        {
            Organization coreOrganizations = new Organization();
            coreOrganizations.organization_name = Request["organization_name"].ToString();
            coreOrganizations.registered_mobile_no = Request["registered_mobile_no"].ToString();
            coreOrganizations.registered_addr_1 = Request["registered_addr_1"].ToString();
            coreOrganizations.registered_phone_no = Request["registered_phone_no"].ToString();
            coreOrganizations.registered_addr_2 = Request["registered_addr_2"].ToString();
            coreOrganizations.registered_fax_no = Request["registered_fax_no"].ToString();
            coreOrganizations.registered_addr_3 = Request["registered_addr_3"].ToString();
            coreOrganizations.registered_email = Request["registered_email"].ToString();
            coreOrganizations.registered_postcode = Request["registered_postcode"].ToString();
            coreOrganizations.registered_city = Guid.Parse(Request["registered_city"].ToString());
            coreOrganizations.registered_state = Guid.Parse(Request["registered_state"].ToString());
            coreOrganizations.cosec_name = Request["cosec_name"].ToString();
            coreOrganizations.cosec_mobile_no = Request["cosec_mobile_no"].ToString();
            coreOrganizations.cosec_addr_1 = Request["cosec_addr_1"].ToString();
            coreOrganizations.cosec_phone_no = Request["cosec_phone_no"].ToString();
            coreOrganizations.cosec_addr_2 = Request["cosec_addr_2"].ToString();
            coreOrganizations.cosec_fax_no = Request["cosec_fax_no"].ToString();
            coreOrganizations.cosec_addr_3 = Request["cosec_addr_3"].ToString();
            coreOrganizations.cosec_email = Request["cosec_email"].ToString();
            coreOrganizations.cosec_postcode = Request["cosec_postcode"].ToString();
            if (Request["cosec_city"].ToString() != "") coreOrganizations.cosec_city = Guid.Parse(Request["cosec_city"].ToString());
            if (Request["cosec_state"].ToString() != "") coreOrganizations.cosec_state = Guid.Parse(Request["cosec_state"].ToString());
            coreOrganizations.office_addr_1 = Request["office_addr_1"].ToString();
            coreOrganizations.office_addr_2 = Request["office_addr_2"].ToString();
            coreOrganizations.office_addr_3 = Request["office_addr_3"].ToString();
            coreOrganizations.office_mobile_no = Request["office_mobile_no"].ToString();
            coreOrganizations.office_postcode = Request["office_postcode"].ToString();
            coreOrganizations.office_phone_no = Request["office_phone_no"].ToString();
            coreOrganizations.office_city = Guid.Parse(Request["office_city"].ToString());
            coreOrganizations.office_fax_no = Request["office_fax_no"].ToString();
            coreOrganizations.office_state = Guid.Parse(Request["office_state"].ToString());
            coreOrganizations.office_email = Request["office_email"].ToString();
            coreOrganizations.office_website = Request["office_website"].ToString();
            coreOrganizations.office_size = int.Parse(Request["office_size"]);
            coreOrganizations.organization_identifier = Request["organization_identifier"].ToString();
            coreOrganizations.incorporation_date = Convert.ToDateTime(Request["incorporation_date"].ToString());
            coreOrganizations.authorized_capital = Decimal.Parse(Request["authorized_capital"]);
            coreOrganizations.paid_capital = Decimal.Parse(Request["paid_capital"]);
            coreOrganizations.parent_org_idx = Guid.Parse(Request["parent_org_idx"].ToString());
            coreOrganizations.pbt_ref = Guid.Parse(Request["pbt_ref"].ToString());
            ilpBLL.UpdateOrganization(coreOrganizations);
            ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));

            return true;
        }

        [HttpPost]
        public bool StoreLesenBaharuMaklumatSaham()
        {
            var shareholderData = ilpBLL.GetShareholdersByOrgRef(Guid.Parse(Request["organization_idx"].ToString()));
            if (shareholderData == false)
            {
                dynamic shareholders = Newtonsoft.Json.JsonConvert.DeserializeObject(Request["shareholders"]);
                foreach (var data in shareholders)
                {
                    Shareholder shareholder = new Shareholder();
                    shareholder.person_identifier = data["person_identifier"];
                    shareholder.person_name = data["person_name"];
                    shareholder.number_of_shares = decimal.Parse(data["number_of_shares"].ToString());
                    //shareholder.organization_identifier = data["organization_identifier"];
                    shareholder.organization_idx = Request["organization_idx"].ToString();
                    ilpBLL.StoreShareholder(shareholder, Guid.Parse(Session["UID"].ToString()));
                }
            }
            return true;
        }

        [HttpPost]
        public bool UpdatePemegangSaham()
        {
            Shareholder shareholder = new Shareholder();
            var isPerson = Request["isPerson"].ToString();

            shareholder.status_shareholder = Guid.Parse(Request["status_shareholder"].ToString());
            shareholder.organization_shareholder_idx = Guid.Parse(Request["organization_shareholder_idx"].ToString());


            if (isPerson == "true")
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string name;
                        DateTime current = DateTime.Now;
                        long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                        name = unixTime + "_" + file.FileName;

                        fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                        string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                                                                                                                 //file.SaveAs(fname);
                                                                                                                 //use common upload in base contoller
                        this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);

                        if (files.AllKeys[i] == "id_upload")
                        {
                            var upload = ilpBLL.StoreILPUploads(Guid.Parse(Request["shareholder_person_ref"].ToString()), file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "MYKAD");
                            shareholder.person_id_upload = upload.uploads_freeform_by_persons_idx.ToString();
                        }
                    }
                }
                shareholder.person_mobile_no = Request["contact_mobile_no"].ToString();
                shareholder.person_birthday = Convert.ToDateTime(Request["person_birthday"].ToString());
                shareholder.person_addr1 = Request["contact_addr_1"].ToString();
                shareholder.person_addr2 = Request["contact_addr_2"].ToString();
                shareholder.person_addr3 = Request["contact_addr_3"].ToString();
                shareholder.person_age = int.Parse(Request["person_age"].ToString());
                shareholder.person_gender = Guid.Parse(Request["person_gender"].ToString());
                /*if (Request["status"].ToString() != "SYARIKAT TEMPATAN") */
                shareholder.person_religion = Guid.Parse(Request["person_religion"].ToString());
                shareholder.person_nationality = Guid.Parse(Request["person_nationality"].ToString());
                shareholder.person_postcode = Request["contact_postcode"].ToString();
                shareholder.person_city = Guid.Parse(Request["contact_city"].ToString());
                shareholder.person_state = Guid.Parse(Request["contact_state"].ToString());
                shareholder.shareholder_person_ref = Guid.Parse(Request["shareholder_person_ref"].ToString());
            }

            else
            {
                //shareholder.status = Request["status"].ToString();
                shareholder.organization_mobile_no = Request["office_mobile_no"].ToString();
                shareholder.organization_incorporation_date = Convert.ToDateTime(Request["incorporation_date"].ToString());
                shareholder.organization_addr1 = Request["office_addr_1"].ToString();
                shareholder.organization_addr2 = Request["office_addr_2"].ToString();
                shareholder.organization_addr3 = Request["office_addr_3"].ToString();
                shareholder.organization_postcode = Request["office_postcode"].ToString();
                shareholder.organization_city = Guid.Parse(Request["office_city"].ToString());
                shareholder.organization_state = Guid.Parse(Request["office_state"].ToString());
                shareholder.organization_country = Guid.Parse(Request["country_ref"].ToString());
                shareholder.shareholder_organization_ref = Guid.Parse(Request["shareholder_organization_ref"].ToString());
            }

            TourlistDataLayer.DataModel.core_organization_shareholders share = ilpBLL.UpdateShareholder(shareholder, isPerson);


            int shareholderCheck = coreOrgHelper.CheckShareHolder(share.organization_ref.ToString());
            if (shareholderCheck == 0)
            {
                ilpBLL.UpdateChecklistStatus(null, Request["chkitem_instance_idx"].ToString(), 1);
                //coreOrgHelper.updateListing(itemIDx);
            }

            return true;
        }

        [HttpPost]
        public bool StoreLesenBaharuMaklumatPengarah()
        {
            var directorData = ilpBLL.GetDirectorsByOrgRef(Guid.Parse(Request["organization_idx"].ToString()));
            if (directorData == false)
            {
                dynamic directors = Newtonsoft.Json.JsonConvert.DeserializeObject(Request["directors"]);
                foreach (var data in directors)
                {
                    Director director = new Director();
                    director.person_name = data["person_name"];
                    director.person_identifier = data["person_identifier"];
                    director.person_addr1 = data["contact_addr_1"];
                    director.person_addr2 = data["contact_addr_2"];
                    director.person_addr3 = data["contact_addr_3"];
                    director.person_postcode = data["contact_postcode"];
                    director.person_town = data["contact_town"];
                    //director.organization_identifier = data["organization_identifier"];
                    director.organization_idx = Request["organization_idx"].ToString();
                    ilpBLL.StoreDirector(director, Guid.Parse(Session["UID"].ToString()));
                }
            }

            return true;
        }

        [HttpPost]
        public bool UpdatePengarah()
        {
            Director director = new Director();
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname;
                    string name;
                    DateTime current = DateTime.Now;
                    long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                    name = unixTime + "_" + file.FileName;

                    fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                    string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                                                                                                             //file.SaveAs(fname);
                                                                                                             //use common upload in base contoller
                    this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);

                    if (files.AllKeys[i] == "id_upload")
                    {
                        var upload = ilpBLL.StoreILPUploads(Guid.Parse(Request["person_ref"].ToString()), file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "MYKAD");
                        director.person_id_upload = upload.uploads_freeform_by_persons_idx.ToString();
                    }

                    if (files.AllKeys[i] == "passport_upload")
                    {
                        var upload = ilpBLL.StoreILPUploads(Guid.Parse(Request["person_ref"].ToString()), file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "PAS_PENGGAJIAN");
                        director.person_cert_upload = upload.uploads_freeform_by_persons_idx.ToString();
                    }
                }
            }
            director.person_mobile_no = Request["contact_mobile_no"].ToString();
            director.person_phone_no = Request["contact_phone_no"].ToString();
            director.person_age = int.Parse(Request["person_age"].ToString());
            director.person_birthday = Convert.ToDateTime(Request["person_birthday"].ToString());
            director.person_nationality = Guid.Parse(Request["person_nationality"].ToString());
            director.person_gender = Guid.Parse(Request["person_gender"].ToString());
            director.person_addr1 = Request["contact_addr_1"].ToString();
            director.person_addr2 = Request["contact_addr_2"].ToString();
            director.person_addr3 = Request["contact_addr_3"].ToString();
            director.person_postcode = Request["contact_postcode"].ToString();
            director.person_city = Guid.Parse(Request["contact_city"].ToString());
            director.person_state = Guid.Parse(Request["contact_state"].ToString());
            director.person_ref = Guid.Parse(Request["person_ref"].ToString());
            ilpBLL.UpdateDirector(director);

            var ilpLicense = ilpBLL.getIlpLicense(Request["application_ref"].ToString());
            int shareholder = coreOrgHelper.CheckDirector(ilpLicense.organization_ref.ToString());
            if (shareholder == 0)
            {
                ilpBLL.UpdateChecklistStatus(null, Request["chkitem_instance_idx"].ToString(), 1);
                //coreOrgHelper.updateListing(itemIDx);
            }
            return true;
        }

        [HttpPost]
        public bool StorePermitMengajarMaklumatAkademik()
        {
            var exist = Request["qualification_idx"].ToString();
            PersonQualification qualification = new PersonQualification();
            string qualifications = Request["qualification_level_idx"]; ;
            string[] qualificationIds = qualifications.Split(',');
            var person = ilpBLL.GetPerson(Guid.Parse(Request["ilp_license_ref"].ToString()));

            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname;
                    string name;
                    DateTime current = DateTime.Now;
                    long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                    name = unixTime + "_" + file.FileName;

                    fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                    string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                    //file.SaveAs(fname);
                    //use common upload in base contoller
                    this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);

                    if (files.AllKeys[i] == "qualification_upload")
                    {
                        var upload = ilpBLL.StoreILPUploads(person.person_idx, file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "DOKUMEN");
                        qualification.qualification_upload = upload.uploads_freeform_by_persons_idx.ToString();
                    }
                }
            }

            if (exist != "")
            {
                qualification.qualification_institution_name = Request["qualification_institution_name"].ToString();
                qualification.qualification_date_start = Convert.ToDateTime(Request["qualification_date_start"].ToString());
                qualification.qualification_name = Request["qualification_name"].ToString();
                qualification.qualification_idx = Guid.Parse(Request["qualification_idx"].ToString());
                var updatedData = ilpBLL.UpdatePersonQualification(qualification);

                ilpBLL.DestroyMultiSelect((Guid)updatedData.qualification_level_idx);

                if (qualificationIds != null && qualificationIds.Length != 0)
                {
                    foreach (var qualificationId in qualificationIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)updatedData.qualification_level_idx;
                        select.details_ref = Guid.Parse(qualificationId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }
            }
            else
            {
                qualification.person_idx = person.person_idx;
                qualification.qualification_institution_name = Request["qualification_institution_name"].ToString();
                qualification.qualification_date_start = Convert.ToDateTime(Request["qualification_date_start"].ToString());
                qualification.qualification_name = Request["qualification_name"].ToString();
                var personQualification = ilpBLL.CreatePersonQualification(qualification, Guid.Parse(Session["UID"].ToString()));

                if (qualificationIds != null && qualificationIds.Length != 0)
                {
                    foreach (var qualificationId in qualificationIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)personQualification.qualification_level_idx;
                        select.details_ref = Guid.Parse(qualificationId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }
            }

            return true;
        }

        public ActionResult PembaharuanLesenIndex()
        {
            licenseCheck();
            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_RENEW");
            ViewBag.code_setup = license.application_status_code;
            ViewBag.id = license.stub_ref;
            //if (license.active_status == 0)
            if (license.ilp_idx == Guid.Empty || license.application_status_code == "COMPLETED")
            {
                // ilpBLL.CreateLesenBaharu(Guid.Parse(Session["UID"].ToString()), "PEMBAHARUAN LESEN", "ILP Pembaharuan Lesen", "ILP Dokumen Sokongan", "ILP_RENEW");
                var userID = Session["UID"];
                Guid Id = new Guid(userID.ToString());
                var user = coreHelper.GetCoreUserByGuid(Id);
                if (user != null && user.user_organization == null)
                {
                    user.user_organization = user.person_ref;
                }
                var stub = coreHelper.GenerateApplicationStubs(
                       Tourlist.Common.TourlistEnums.MotacModule.ILP,
                       Tourlist.Common.TourlistEnums.ModuleLicenseType.TIADA,
                       Tourlist.Common.TourlistEnums.SolModulesType.ILP_RENEW,
                       Tourlist.Common.TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                         user);
                license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_RENEW");
            }
            ViewBag.Licenses = license;
            var checklist = ilpBLL.GetIlpChecklist(license.stub_ref, "ILP_PEMBAHARUAN_LESEN", license.application_status_code);
            ViewBag.Checklist = checklist.OrderBy(c => c.order);
            ViewBag.OpenChecklist = 1;
            ViewBag.ReadOnly = true;
            return View();
        }

        [HttpPost]
        public bool UpdateRenewalDuration()
        {
            ilpBLL.UpdateRenewalDuration(Guid.Parse(Request["license_ref"].ToString()), int.Parse(Request["renewal_duration"].ToString()));
            ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));
            return true;
        }

        [HttpPost]
        public JsonResult GetRenewalDuration()
        {
            var duration = ilpBLL.GetRenewalDuration(Guid.Parse(Request["license_ref"].ToString()));
            Object[] obj = {
                duration
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRenewalDurationCommon(string applyid)
        {
            var ilplicense = TourlistUnitOfWork.IlpLicenses.Find(x => x.stub_ref.ToString() == applyid).FirstOrDefault();

            var duration = ilpBLL.GetRenewalDuration(ilplicense.ilp_idx);
            Object[] obj = {
                duration
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PembaharuanLesenCreate()
        {
            return View();
        }

        public ActionResult TambahCawanganIndex()
        {
            string userID = Session["UID"].ToString();
            var organization = ilpBLL.GetOrganization(userID);
            ViewBag.OrgID = organization;
            ViewBag.ilp_branch = ilpBLL.GetILPStatusModul(organization.organization_idx.ToString(), "ILP_ADDBRANCH");
            //licenseCheck();
            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_ADDBRANCH");

            ViewBag.license_type = "ILP_ADDBRANCH";
            ViewBag.Licenses = license;
            ViewBag.AppNo = license.application_no;
            ViewBag.code_setup = license.application_status_code;
            return ILPChecklist(license.ilp_idx, "ILP_TAMBAH_CAWANGAN", "TambahCawanganIndex", license.application_status_code);
            // return View();
        }

        public void ajaxGenerateApplicationStubs(string Module, string OrgID)
        {
            var userID = Session["UID"];
            Guid Id = new Guid(userID.ToString());
            Guid OrganizationID = Guid.Parse(OrgID);
            var user = coreHelper.GetCoreUserByGuid(Id);

            if (Module == "Branch")
            {
                var stub = coreHelper.GenerateApplicationStubs(
                Tourlist.Common.TourlistEnums.MotacModule.ILP,
                Tourlist.Common.TourlistEnums.ModuleLicenseType.TIADA,
                Tourlist.Common.TourlistEnums.SolModulesType.ILP_ADDBRANCH,
                Tourlist.Common.TourlistEnums.ApplicationStatusRefType.STATUSAWAM, user);
            }
            else if (Module == "Tukar Status")
            {
                var stub = coreHelper.GenerateApplicationStubs(
                Tourlist.Common.TourlistEnums.MotacModule.ILP,
                Tourlist.Common.TourlistEnums.ModuleLicenseType.TIADA,
                Tourlist.Common.TourlistEnums.SolModulesType.ILP_CHANGESTATUS,
                Tourlist.Common.TourlistEnums.ApplicationStatusRefType.STATUSAWAM, user);

                Guid stubID = stub.application_Stubs.apply_idx;

                coreOrgHelper.ChangeStatusOrganization_SaveNew(OrganizationID, stubID, Id);
                coreOrgHelper.ChangeStatusShareHolder_SaveNew(OrganizationID, stubID, Id);
                coreOrgHelper.ChangeStatusDirector_SaveNew(OrganizationID, stubID, Id);

            }



        }

        [HttpPost]
        public void CreateLicense()
        {
            ilpBLL.CreateLesenBaharu(Guid.Parse(Session["UID"].ToString()), Request["license_type"].ToString(), Request["description"].ToString(), Request["document"].ToString(), Request["module_name"].ToString());
        }

        public ActionResult TambahCawanganCreate(Guid chkitem_instance_idx, Guid application_ref, String license_type)
        {
            ViewBag.application_status_code = ilpBLL.GetLicenseStatusCode(application_ref);
            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.license_type = license_type;
            return View();
        }

        public bool StoreTambahCawangan()
        {
            var exist = Request["ilp_branch_idx"].ToString();
            var userID = Session["UID"].ToString();
            var user = coreHelper.GetCoreUserByGuid(Guid.Parse(userID));

            ILPBranch branch = new ILPBranch();
            string utilities = Request["utility[]"];
            string[] utilityIds = utilities.Split(',');

            if (exist != "")
            {
                branch.ilp_license_idx = Guid.Parse(Request["ilp_license_idx"].ToString());
                branch.ilp_branch_idx = Guid.Parse(Request["ilp_branch_idx"].ToString());
                branch.paid_capital = Decimal.Parse(Request["paid_capital"]);
                branch.authorized_capital = Decimal.Parse(Request["authorized_capital"]);
                branch.branch_addr_1 = Request["branch_addr_1"];
                branch.branch_addr_2 = Request["branch_addr_2"];
                branch.branch_addr_3 = Request["branch_addr_3"];
                branch.branch_postcode = Request["branch_postcode"];
                branch.branch_city = Guid.Parse(Request["branch_city"].ToString());
                branch.branch_state = Guid.Parse(Request["branch_state"].ToString());
                branch.branch_mobile_no = Request["branch_mobile_no"];
                branch.branch_phone_no = Request["branch_phone_no"];
                branch.branch_fax_no = Request["branch_fax_no"];
                branch.branch_email = Request["branch_email"];
                branch.branch_website = Request["branch_website"];
                branch.branch_size = int.Parse(Request["branch_size"]);
                branch.utility_others = Request["utility_others"];
                branch.organization_ref = user.user_organization;
                branch.pbt_ref = Guid.Parse(Request["pbt_ref"].ToString());
                var updatedData = ilpBLL.UpdateILPBranch(branch);

                ilpBLL.DestroyMultiSelect((Guid)updatedData.utility);

                if (utilityIds != null && utilityIds.Length != 0)
                {
                    foreach (var utilityId in utilityIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)updatedData.utility;
                        select.details_ref = Guid.Parse(utilityId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }
            }
            else
            {
                branch.ilp_license_idx = Guid.Parse(Request["ilp_license_idx"].ToString());
                var a = Request["paid_capital"];
                if (Request["paid_capital"] != "")
                {
                    branch.paid_capital = Decimal.Parse(Request["paid_capital"]);
                }
                if (Request["authorized_capital"] != "")
                {
                    branch.authorized_capital = Decimal.Parse(Request["authorized_capital"]);
                }

                branch.branch_addr_1 = Request["branch_addr_1"];
                branch.branch_addr_2 = Request["branch_addr_2"];
                branch.branch_addr_3 = Request["branch_addr_3"];
                branch.branch_postcode = Request["branch_postcode"];
                branch.branch_city = Guid.Parse(Request["branch_city"].ToString());
                branch.branch_state = Guid.Parse(Request["branch_state"].ToString());
                branch.branch_mobile_no = Request["branch_mobile_no"];
                branch.branch_phone_no = Request["branch_phone_no"];
                branch.branch_fax_no = Request["branch_fax_no"];
                branch.branch_email = Request["branch_email"];
                branch.branch_website = Request["branch_website"];
                branch.branch_size = int.Parse(Request["branch_size"]);
                branch.utility_others = Request["utility_others"];
                branch.organization_ref = user.user_organization;
                branch.pbt_ref = Guid.Parse(Request["pbt_ref"].ToString());
                var newBranch = ilpBLL.StoreILPBranch(branch, Guid.Parse(Session["UID"].ToString()));

                if (utilityIds != null && utilityIds.Length != 0)
                {
                    foreach (var utilityId in utilityIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)newBranch.utility;
                        select.details_ref = Guid.Parse(utilityId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }
            }
            return true;
        }

        [HttpGet]
        public JsonResult ILPBranches(Guid application_ref)
        {
            var branches = ilpBLL.GetIlpBranches(application_ref);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#57259)  on 2 Jan 2024
        [HttpGet]
        public JsonResult ILPBranchesActive(Guid Org_ref)
        {
            var branches = ilpBLL.GetIlpBranchesActive(Org_ref);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#57259)  on 2 Jan 2024
        [HttpGet]
        public JsonResult ILPBranchesbyBranchIdx(Guid branches_idx)
        {
            var branches = ilpBLL.GetIlpBranchesbyBranchIdx(branches_idx);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ILPBranchesByUser()
        {
            var branches = ilpBLL.GetIlpBranchesByUser(Guid.Parse(Session["UID"].ToString()));
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ILPBranchesByUserCommon(string applyId)
        {
            var user = TourlistUnitOfWork.FlowApplicationStubs.Find(x => x.apply_idx.ToString() == applyId).FirstOrDefault();
            var branches = ilpBLL.GetIlpBranchesByUser(user.apply_user);
            Object[] obj = {
                new { branches }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteIlpBranch()
        {
            var ilp_branch_idx = Guid.Parse(Request["ilp_branch_idx"]);
            var response = ilpBLL.DeleteIlpBranch(ilp_branch_idx);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePersonQualification()
        {
            var qualification_idx = Guid.Parse(Request["qualification_idx"]);
            var response = ilpBLL.DeletePersonQualification(qualification_idx);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePersonExperience()
        {
            var experience_idx = Guid.Parse(Request["experience_idx"]);
            var response = ilpBLL.DeletePersonExperience(experience_idx);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeletePersonCourse()
        {
            var person_course_idx = Guid.Parse(Request["person_course_idx"]);
            var response = ilpBLL.DeletePersonCourse(person_course_idx);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ILPLicenses(String license_type)
        {
            var licenses = ilpBLL.GetIlpLicenses(license_type, Guid.Parse(Session["UID"].ToString()));
            Object[] obj = {
                new { licenses }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TukarStatusIndex()
        {
            string appID = null;
            var userID = Session["UID"];
            licenseCheck();
            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_CHANGESTATUS");
            ViewBag.code_setup = license.application_status_code;
            ViewBag.id = license.stub_ref;
            appID = license.stub_ref.ToString();
            //if (license.active_status == 0)
            //{
            //    //ilpBLL.CreateLesenBaharu(Guid.Parse(Session["UID"].ToString()), "TUKAR STATUS", "ILP Tukar Status", "ILP Tukar Status Dokumen Sokongan", "ILP_CHANGESTATUS");
            //    Guid Id = new Guid(userID.ToString());
            //    var user = coreHelper.GetCoreUserByGuid(Id);
            //    if (user != null && user.user_organization == null)
            //    {
            //        user.user_organization = user.person_ref;
            //    }
            //    var stub = coreHelper.GenerateApplicationStubs(
            //           Tourlist.Common.TourlistEnums.MotacModule.ILP,
            //           Tourlist.Common.TourlistEnums.ModuleLicenseType.TIADA,
            //           Tourlist.Common.TourlistEnums.SolModulesType.ILP_CHANGESTATUS,
            //           Tourlist.Common.TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
            //             user);
            //    Guid stubID = stub.application_Stubs.apply_idx;
            //    appID = stubID.ToString(); ;
            //    coreOrgHelper.ChangeStatusOrganization_SaveNew(Guid.Parse(user.user_organization.ToString()), stubID, Id);
            //    coreOrgHelper.ChangeStatusShareHolder_SaveNew(Guid.Parse(user.user_organization.ToString()), stubID, Id);
            //    coreOrgHelper.ChangeStatusDirector_SaveNew(Guid.Parse(user.user_organization.ToString()), stubID, Id);
            //}
            //else
            //{
            //    appID = license.stub_ref.ToString(); 
            //}
            Session["AppID"] = appID;
            ViewBag.Licenses = license;

            ViewBag.module = "ILP_CHANGESTATUS";
            var checklist = ilpBLL.GetIlpChecklist(Guid.Parse(appID), "ILP_TUKAR_STATUS", license.application_status_code);
            if (checklist != null)
                ViewBag.Checklist = checklist.OrderBy(c => c.order);


            return View();
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
        // public ActionResult TukarStatusCreate(Guid chkitem_instance_idx, Guid application_ref, String license_type, string btn, string module, Guid stubid)
        public ActionResult TukarStatusCreate(string btn, string module)
        {
            var chkitem_instance_idx = Request.QueryString["chkitem_instance_idx"];
            var application_ref = Request.QueryString["application_ref"];
            var license_type = Request.QueryString["license_type"];

            if (chkitem_instance_idx != null)
                ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            if (application_ref != null)
                ViewBag.application_ref = application_ref;
            if (license_type != null)
                ViewBag.license_type = license_type;
            //if (chkitem_instance_idx != null)
            //    ViewBag.itemID = chkitem_instance_idx;


            ViewBag.itemID = chkitem_instance_idx;
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
                    // model.number_of_shares_string = String.Format("{0:C}", shareholder.number_of_shares);
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
            Guid stubid = Guid.Parse(Session["AppID"].ToString());
            ViewBag.application_status_code = ilpBLL.GetApplicationStatusCode(stubid);
            return View();
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
        public ActionResult PermitMengajarIndex()
        {
            licenseCheck();
            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_PERMIT");
            ViewBag.code_setup = license.application_status_code;
            ViewBag.id = license.stub_ref;
            if (license.active_status == 0)
            {
                //ilpBLL.CreateLesenBaharu(Guid.Parse(Session["UID"].ToString()), "PERMIT MENGAJAR", "ILP Permit Mengajar", "ILP Permit Mengajar Dokumen Sokongan", "ILP_PERMIT");
                var userID = Session["UID"];
                Guid Id = new Guid(userID.ToString());
                var user = coreHelper.GetCoreUserByGuid(Id);
                if (user != null && user.user_organization == null)
                {
                    user.user_organization = user.person_ref;
                }
                var stub = coreHelper.GenerateApplicationStubs(
                       Tourlist.Common.TourlistEnums.MotacModule.ILP,
                       Tourlist.Common.TourlistEnums.ModuleLicenseType.TIADA,
                       Tourlist.Common.TourlistEnums.SolModulesType.ILP_PERMIT,
                       Tourlist.Common.TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                         user);
            }
            ViewBag.Licenses = license;
            List<SelectListItem> Gender = new List<SelectListItem>();
            Gender = GetGender();
            ViewData["Gender_"] = Gender;
            return ILPChecklist(license.ilp_idx, "ILP_PERMIT_MENGAJAR", "PermitMengajarIndex", license.application_status_code);
        }
        public List<SelectListItem> GetGender()
        {
            List<SelectListItem> Gender = new List<SelectListItem>();
            var data = refHelper.GetRefListByType("GENDER");
            foreach (var app in data)
            {
                Gender.Add(new SelectListItem
                {
                    Value = app.ref_idx.ToString(),
                    Text = app.ref_description.ToUpper(),
                });
            }
            return Gender;
        }

        [HttpPost]
        public bool StorePermitMengajarMaklumatPemohon()
        {
            var exist = Request["person_idx"].ToString();
            ILPPerson person = new ILPPerson();

            if (exist != "")
            {
                person.person_idx = Guid.Parse(Request["person_idx"].ToString());
                person.person_name = Request["person_name"].ToString();
                person.person_gender = Guid.Parse(Request["person_gender"].ToString());
                person.person_identifier = Request["person_identifier"].ToString();
                person.person_birthday = Convert.ToDateTime(Request["person_birthday"].ToString());
                person.residential_addr_1 = Request["residential_addr_1"].ToString();
                person.person_birthplace = Request["person_birthplace"].ToString();
                person.residential_addr_2 = Request["residential_addr_2"].ToString();
                person.person_nationality = Guid.Parse(Request["person_nationality"].ToString());
                person.residential_addr_3 = Request["residential_addr_3"].ToString();
                person.contact_mobile_no = Request["contact_mobile_no"].ToString();
                person.residential_postcode = Request["residential_postcode"].ToString();
                person.contact_phone_no = Request["contact_phone_no"].ToString();
                person.residential_city = Guid.Parse(Request["residential_city"].ToString());
                person.person_employ_permit_no = Request["person_employ_permit_no"].ToString();
                person.residential_state = Guid.Parse(Request["residential_state"].ToString());
                person.person_employ_permit_released_place = Request["person_employ_permit_released_place"].ToString();

                var dateStart = Request["person_employ_date_start"];
                if (dateStart == "")
                    person.person_employ_date_start = null;
                else
                    person.person_employ_date_start = Convert.ToDateTime(dateStart);

                var dateEnd = Request["person_employ_date_end"];
                if (dateEnd == "")
                    person.person_employ_date_end = null;
                else
                    person.person_employ_date_end = Convert.ToDateTime(dateEnd);

                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string name;
                        DateTime current = DateTime.Now;
                        long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                        name = unixTime + "_" + file.FileName;

                        fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                        string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                                                                                                                 //file.SaveAs(fname);
                                                                                                                 //use common upload in base contoller
                        this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);

                        if (files.AllKeys[i] == "person_cert_upload")
                        {
                            var upload = ilpBLL.StoreILPUploads(person.person_idx, file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "JPK");
                            person.person_cert_upload = upload.uploads_freeform_by_persons_idx.ToString();
                        }
                    }
                }
                ilpBLL.UpdatePengajarPemohon(person);
            }
            else
            {
                person.person_name = Request["person_name"].ToString();
                person.person_gender = Guid.Parse(Request["person_gender"].ToString());
                person.person_identifier = Request["person_identifier"].ToString();
                person.person_birthday = Convert.ToDateTime(Request["person_birthday"].ToString());
                person.residential_addr_1 = Request["residential_addr_1"].ToString();
                person.person_birthplace = Request["person_birthplace"].ToString();
                person.residential_addr_2 = Request["residential_addr_2"].ToString();
                person.person_nationality = Guid.Parse(Request["person_nationality"].ToString());
                person.residential_addr_3 = Request["residential_addr_3"].ToString();
                person.contact_mobile_no = Request["contact_mobile_no"].ToString();
                person.residential_postcode = Request["residential_postcode"].ToString();
                person.contact_phone_no = Request["contact_phone_no"].ToString();
                person.residential_city = Guid.Parse(Request["residential_city"].ToString());
                person.person_employ_permit_no = Request["person_employ_permit_no"].ToString();
                person.residential_state = Guid.Parse(Request["residential_state"].ToString());
                person.person_employ_permit_released_place = Request["person_employ_permit_released_place"].ToString();

                var dateStart = Request["person_employ_date_start"];
                if (dateStart == "")
                    person.person_employ_date_start = null;
                else
                    person.person_employ_date_start = Convert.ToDateTime(dateStart);

                var dateEnd = Request["person_employ_date_end"];
                if (dateEnd == "")
                    person.person_employ_date_end = null;
                else
                    person.person_employ_date_end = Convert.ToDateTime(dateEnd);

                var newPerson = ilpBLL.CreatePerson(person, Guid.Parse(Session["UID"].ToString()));
                ilpBLL.CreateILPPermit(newPerson.person_idx, Guid.Parse(Request["ilp_license_ref"].ToString()), Guid.Parse(Session["UID"].ToString()));
                ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string name;
                        DateTime current = DateTime.Now;
                        long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                        name = unixTime + "_" + file.FileName;

                        fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                        string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                                                                                                                 //file.SaveAs(fname);
                                                                                                                 //use common upload in base contoller
                        this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);

                        if (files.AllKeys[i] == "person_cert_upload")
                        {
                            var upload = ilpBLL.StoreILPUploads(newPerson.person_idx, file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "JPK");
                            ilpBLL.UpdatePersonUpload(newPerson.person_idx, upload.uploads_freeform_by_persons_idx);
                        }
                    }
                }
            }

            return true;
        }

        [HttpPost]
        public bool StorePermitMengajarMaklumatRujukan()
        {
            var exist1 = Request["person_reference_idx_1"].ToString();
            var exist2 = Request["person_reference_idx_2"].ToString();
            ILPPersonReference personReference1 = new ILPPersonReference();
            ILPPersonReference personReference2 = new ILPPersonReference();

            if (exist1 != "")
            {
                personReference1.person_reference_idx = Guid.Parse(Request["person_reference_idx_1"].ToString());
                personReference1.person_ref = Guid.Parse(Request["person_ref_1"].ToString());
                personReference1.person_name = Request["person_name_1"].ToString();
                personReference1.person_occupation = Request["person_occupation_1"].ToString();
                personReference1.person_addr1 = Request["person_addr1_1"].ToString();
                personReference1.person_known_duration = int.Parse(Request["person_known_duration_1"].ToString());
                personReference1.person_addr2 = Request["person_addr2_1"].ToString();
                personReference1.person_mobile_no = Request["person_mobile_no_1"].ToString();
                personReference1.person_addr3 = Request["person_addr3_1"].ToString();
                personReference1.person_email = Request["person_email_1"].ToString();
                personReference1.person_postcode = Request["person_postcode_1"].ToString();
                personReference1.person_state = Guid.Parse(Request["person_state_1"].ToString());
                personReference1.person_city = Guid.Parse(Request["person_city_1"].ToString());
                ilpBLL.UpdatePersonReference(personReference1);
            }
            else
            {
                personReference1.person_name = Request["person_name_1"].ToString();
                personReference1.person_occupation = Request["person_occupation_1"].ToString();
                personReference1.person_addr1 = Request["person_addr1_1"].ToString();
                personReference1.person_known_duration = int.Parse(Request["person_known_duration_1"].ToString());
                personReference1.person_addr2 = Request["person_addr2_1"].ToString();
                personReference1.person_mobile_no = Request["person_mobile_no_1"].ToString();
                personReference1.person_addr3 = Request["person_addr3_1"].ToString();
                personReference1.person_email = Request["person_email_1"].ToString();
                personReference1.person_postcode = Request["person_postcode_1"].ToString();
                personReference1.person_state = Guid.Parse(Request["person_state_1"].ToString());
                personReference1.person_city = Guid.Parse(Request["person_city_1"].ToString());
                ilpBLL.CreatePersonReference(personReference1, Guid.Parse(Session["UID"].ToString()), Guid.Parse(Request["ilp_license_ref"].ToString()));
            }

            if (Request["person_name_2"] != "")
            {
                if (exist2 != "")
                {
                    personReference2.person_reference_idx = Guid.Parse(Request["person_reference_idx_2"].ToString());
                    personReference2.person_ref = Guid.Parse(Request["person_ref_2"].ToString());
                    personReference2.person_name = Request["person_name_2"].ToString();
                    personReference2.person_occupation = Request["person_occupation_2"].ToString();
                    personReference2.person_addr1 = Request["person_addr1_2"].ToString();
                    personReference2.person_known_duration = int.Parse(Request["person_known_duration_2"].ToString());
                    personReference2.person_addr2 = Request["person_addr2_2"].ToString();
                    personReference2.person_mobile_no = Request["person_mobile_no_2"].ToString();
                    personReference2.person_addr3 = Request["person_addr3_2"].ToString();
                    personReference2.person_email = Request["person_email_2"].ToString();
                    personReference2.person_postcode = Request["person_postcode_2"].ToString();
                    personReference2.person_state = Guid.Parse(Request["person_state_2"].ToString());
                    personReference2.person_city = Guid.Parse(Request["person_city_2"].ToString());
                    ilpBLL.UpdatePersonReference(personReference2);
                }
                else
                {
                    personReference2.person_name = Request["person_name_2"].ToString();
                    personReference2.person_occupation = Request["person_occupation_2"].ToString();
                    personReference2.person_addr1 = Request["person_addr1_2"].ToString();
                    personReference2.person_known_duration = int.Parse(Request["person_known_duration_2"].ToString());
                    personReference2.person_addr2 = Request["person_addr2_2"].ToString();
                    personReference2.person_mobile_no = Request["person_mobile_no_2"].ToString();
                    personReference2.person_addr3 = Request["person_addr3_2"].ToString();
                    personReference2.person_email = Request["person_email_2"].ToString();
                    personReference2.person_postcode = Request["person_postcode_2"].ToString();
                    personReference2.person_state = Guid.Parse(Request["person_state_2"].ToString());
                    personReference2.person_city = Guid.Parse(Request["person_city_2"].ToString());
                    ilpBLL.CreatePersonReference(personReference2, Guid.Parse(Session["UID"].ToString()), Guid.Parse(Request["ilp_license_ref"].ToString()));
                }
            }

            ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));

            return true;
        }

        [HttpPost]
        public JsonResult GetPersonReference()
        {
            var personReference = ilpBLL.GetPersonReference(Guid.Parse(Request["ilp_license_ref"].ToString()));
            Object[] obj = {
                personReference
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PermitMengajarAkademik(Guid chkitem_instance_idx, Guid application_ref, String license_type, Guid stubid)
        {
            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.license_type = license_type;
            ViewBag.application_status_code = ilpBLL.GetApplicationStatusCode(stubid);
            return View();
        }

        [HttpPost]
        public bool StoreLesenBaharuMaklumatPengajar()
        {
            var exist = Request["instructor_courses_idx"].ToString();
            ILPInstructorCourse instructorCourse = new ILPInstructorCourse();
            string courses = Request["course_details"];
            string facilities = Request["facility_details"]; ;
            string[] courseIds = courses.Split(',');
            string[] facilityIds = facilities.Split(',');

            if (exist != "")
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string name;
                        DateTime current = DateTime.Now;
                        long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                        name = unixTime + "_" + file.FileName;

                        fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                        string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                                                                                                                 //file.SaveAs(fname);
                                                                                                                 //use common upload in base contoller
                        this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);

                        if (files.AllKeys[i] == "course_details_upload")
                        {
                            var upload = ilpBLL.StoreILPUploads(Guid.Parse(Request["person_ref"].ToString()), file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "KURSUS");
                            instructorCourse.course_details_upload = upload.uploads_freeform_by_persons_idx.ToString();
                        }
                    }
                }

                instructorCourse.license_ref = Guid.Parse(Request["ilp_license_ref"].ToString());
                instructorCourse.person_ref = Guid.Parse(Request["person_ref"].ToString());
                instructorCourse.person_name = Request["person_name"].ToString();
                instructorCourse.person_identifier = Request["person_identifier"].ToString();
                instructorCourse.person_mobile_no = Request["person_mobile_no"].ToString();
                instructorCourse.person_phone_no = Request["person_phone_no"].ToString();
                instructorCourse.person_country = Guid.Parse(Request["person_country"].ToString());
                instructorCourse.course_details_others = Request["course_details_others"].ToString();
                instructorCourse.facility_details_others = Request["facility_details_others"].ToString();
                instructorCourse.premise_is_shared = short.Parse(Request["premise_is_shared"].ToString());
                instructorCourse.instructor_courses_idx = Guid.Parse(Request["instructor_courses_idx"].ToString());

                var updatedData = ilpBLL.UpdateInstructorCourse(instructorCourse);

                ilpBLL.DestroyMultiSelect((Guid)updatedData.course_details);
                ilpBLL.DestroyMultiSelect(updatedData.facility_details);

                if (courseIds != null && courseIds.Length != 0)
                {
                    foreach (var courseId in courseIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)updatedData.course_details;
                        select.details_ref = Guid.Parse(courseId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }

                if (facilityIds != null && facilityIds.Length != 0)
                {
                    foreach (var facilityId in facilityIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = updatedData.facility_details;
                        select.details_ref = Guid.Parse(facilityId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }


            }
            else
            {
                instructorCourse.license_ref = Guid.Parse(Request["ilp_license_ref"].ToString());
                instructorCourse.person_name = Request["person_name"].ToString();
                instructorCourse.person_identifier = Request["person_identifier"].ToString();
                instructorCourse.person_mobile_no = Request["person_mobile_no"].ToString();
                instructorCourse.person_phone_no = Request["person_phone_no"].ToString();
                instructorCourse.person_country = Guid.Parse(Request["person_country"].ToString());
                instructorCourse.course_details_others = Request["course_details_others"].ToString();
                instructorCourse.facility_details_others = Request["facility_details_others"].ToString();
                instructorCourse.premise_is_shared = short.Parse(Request["premise_is_shared"].ToString());

                var course = ilpBLL.CreateInstructorCourse(instructorCourse, Guid.Parse(Session["UID"].ToString()));

                if (courseIds != null && courseIds.Length != 0)
                {
                    foreach (var courseId in courseIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)course.course_details;
                        select.details_ref = Guid.Parse(courseId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }

                if (facilityIds != null && facilityIds.Length != 0)
                {
                    foreach (var facilityId in facilityIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)course.facility_details;
                        select.details_ref = Guid.Parse(facilityId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }

                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string name;
                        DateTime current = DateTime.Now;
                        long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                        name = unixTime + "_" + file.FileName;

                        fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                        string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                                                                                                                 //file.SaveAs(fname);
                                                                                                                 //use common upload in base contoller
                        this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);

                        if (files.AllKeys[i] == "course_details_upload")
                        {
                            var upload = ilpBLL.StoreILPUploads(course.person_ref, file.FileName, name, path, Guid.Parse(Session["UID"].ToString()), "KURSUS");
                            ilpBLL.UpdateInstructorUpload(course.person_ref, upload.uploads_freeform_by_persons_idx);
                        }
                    }
                }
            }


            return true;
        }

        [HttpGet]
        public JsonResult ILPInstructorCourses(Guid application_ref) // Variable supposed to be named as ilp_idx. Don't know why previous dev named is application_ref
        {
            var user_idx = Guid.Parse(Session["UID"].ToString());
            var instructorCourses = ilpBLL.GetInstructorCourses(application_ref, user_idx);
            Object[] obj = {
                new { instructorCourses }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ILPInstructorCoursesCommon(string applyid)
        {
            flow_application_stubs current = TourlistUnitOfWork.FlowApplicationStubs.Find(x => x.apply_idx.ToString() == applyid).FirstOrDefault();
            var user_idx = current.apply_user;
            Guid licenses = TourlistUnitOfWork.IlpLicenses.Find(x => x.stub_ref.ToString() == applyid).Select(x => x.ilp_idx).FirstOrDefault();
            var instructorCourses = ilpBLL.GetInstructorCourses(licenses, user_idx);
            Object[] obj = {
                new { instructorCourses }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ILPPersonQualification(Guid application_ref)
        {
            var qualifications = ilpBLL.GetIlpPersonQualifications(application_ref);
            Object[] obj = {
                new { qualifications }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PermitMengajarPengalaman(Guid chkitem_instance_idx, Guid application_ref, String license_type, Guid stubid)
        {
            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.license_type = license_type;
            ViewBag.application_status_code = ilpBLL.GetApplicationStatusCode(stubid);
            return View();
        }

        [HttpPost]
        public bool StorePermitMengajarMaklumatPengalaman()
        {
            var exist = Request["experience_idx"].ToString();
            PersonExperience experience = new PersonExperience();
            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_PERMIT");
            var person = ilpBLL.GetPerson(Guid.Parse(Request["ilp_license_ref"].ToString()));

            if (exist != "")
            {
                experience.experience_idx = Guid.Parse(Request["experience_idx"].ToString());
                experience.experience_employer = Request["experience_employer"].ToString();
                experience.experience_position = Request["experience_position"].ToString();
                experience.experience_date_start = Convert.ToDateTime(Request["experience_date_start"].ToString());
                experience.experience_date_end = Convert.ToDateTime(Request["experience_date_end"].ToString());
                experience.experience_employer_address = Request["experience_employer_address"].ToString();
                experience.experience_employer_address2 = Request["experience_employer_address2"].ToString();
                experience.experience_employer_address3 = Request["experience_employer_address3"].ToString();
                experience.experience_employer_poscode = Guid.Parse(Request["experience_employer_poscode"]);
                experience.experience_employer_city = Guid.Parse(Request["experience_employer_city"]);
                experience.experience_employer_state = Guid.Parse(Request["experience_employer_state"]);
                ilpBLL.UpdatePersonExperience(experience);
            }
            else
            {
                experience.person_idx = person.person_idx;
                experience.stub_ref = license.stub_ref;
                experience.experience_employer = Request["experience_employer"].ToString();
                experience.experience_position = Request["experience_position"].ToString();
                experience.experience_date_start = Convert.ToDateTime(Request["experience_date_start"].ToString());
                experience.experience_date_end = Convert.ToDateTime(Request["experience_date_end"].ToString());
                experience.experience_employer_address = Request["experience_employer_address"].ToString();
                experience.experience_employer_address2 = Request["experience_employer_address2"].ToString();
                experience.experience_employer_address3 = Request["experience_employer_address3"].ToString();
                experience.experience_employer_poscode = Guid.Parse(Request["experience_employer_poscode"]);
                experience.experience_employer_city = Guid.Parse(Request["experience_employer_city"]);
                experience.experience_employer_state = Guid.Parse(Request["experience_employer_state"]);
                ilpBLL.CreatePersonExperience(experience, Guid.Parse(Session["UID"].ToString()));
            }


            return true;
        }

        [HttpGet]
        public JsonResult ILPPersonWorkExperience(Guid application_ref)
        {
            var experiences = ilpBLL.GetIlpPersonExperiences(application_ref);
            Object[] obj = {
                new { experiences }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PermitMengajarPelajaran(Guid chkitem_instance_idx, Guid application_ref, String license_type)
        {
            ViewBag.chkitem_instance_idx = chkitem_instance_idx;
            ViewBag.application_ref = application_ref;
            ViewBag.license_type = license_type;
            return View();
        }

        [HttpPost]
        public bool StorePermitMengajarMaklumatPelajaran()
        {
            var exist = Request["person_course_idx"].ToString();
            PersonCourse course = new PersonCourse();
            string course_subjects = Request["course_subject_idx[]"];
            string[] courseSubjectIds = course_subjects.Split(',');

            if (exist != "")
            {
                course.person_course_idx = Guid.Parse(Request["person_course_idx"].ToString());
                course.course_name = Request["course_name"].ToString();
                var updatedData = ilpBLL.UpdatePersonCourse(course);

                ilpBLL.DestroyMultiSelect((Guid)updatedData.course_subject_idx);

                if (courseSubjectIds != null && courseSubjectIds.Length != 0)
                {
                    foreach (var courseSubjectId in courseSubjectIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)updatedData.course_subject_idx;
                        select.details_ref = Guid.Parse(courseSubjectId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }
            }
            else
            {
                var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_PERMIT");
                var person = ilpBLL.GetPerson(Guid.Parse(Request["ilp_license_ref"].ToString()));
                course.person_idx = person.person_idx;
                course.stub_ref = license.stub_ref;
                course.course_name = Request["course_name"].ToString();
                var newCourse = ilpBLL.CreatePersonCourse(course, Guid.Parse(Session["UID"].ToString()));

                if (courseSubjectIds != null && courseSubjectIds.Length != 0)
                {
                    foreach (var courseSubjectId in courseSubjectIds)
                    {
                        ILPMultiSelect select = new ILPMultiSelect();
                        select.parent_ref = (Guid)newCourse.course_subject_idx;
                        select.details_ref = Guid.Parse(courseSubjectId);
                        ilpBLL.StoreMultiSelect(select, Guid.Parse(Session["UID"].ToString()));
                    }
                }
            }

            return true;
        }

        [HttpGet]
        public JsonResult ILPPersonCourse(Guid application_ref)
        {
            var courses = ilpBLL.GetIlpPersonCourses(application_ref);

            Object[] obj = {
                new { courses }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SerahBatalLesenIndex()
        {
            licenseCheck();
            var license = ilpBLL.GetPermohonanBaharu(Guid.Parse(Session["UID"].ToString()), "ILP_CANCELLICENSE");
            ViewBag.code_setup = license.application_status_code;
            ViewBag.id = license.stub_ref;
            if (license.active_status == 0)
            {
                //ilpBLL.CreateLesenBaharu(Guid.Parse(Session["UID"].ToString()), "SERAH LESEN", "ILP Serah Lesen", "ILP Serah Lesen Dokumen Sokongan", "ILP_CANCELLICENSE");
                var userID = Session["UID"];
                Guid Id = new Guid(userID.ToString());
                var user = coreHelper.GetCoreUserByGuid(Id);
                if (user != null && user.user_organization == null)
                {
                    user.user_organization = user.person_ref;
                }
                var stub = coreHelper.GenerateApplicationStubs(
                       Tourlist.Common.TourlistEnums.MotacModule.ILP,
                       Tourlist.Common.TourlistEnums.ModuleLicenseType.TIADA,
                       Tourlist.Common.TourlistEnums.SolModulesType.ILP_CANCELLICENSE,
                       Tourlist.Common.TourlistEnums.ApplicationStatusRefType.STATUSAWAM,
                         user);

            }
            ViewBag.Licenses = license;
            var checklist = ilpBLL.GetIlpChecklist(license.stub_ref, "ILP_SERAH_LESEN", license.application_status_code);
            ViewBag.Checklist = checklist.OrderBy(c => c.order);
            ViewBag.OpenChecklist = 1;
            ViewBag.StatusCode = license.application_status_code;

            return View();
        }

        public bool StoreSerahBatalLesen()
        {
            var exist = Request["terminate_licenses_idx"].ToString();
            ILPTerminateLicense terminateLicense = new ILPTerminateLicense();
            string[] ids = Request.Form.GetValues("branches[]");

            if (exist != "")
            {
                terminateLicense.terminate_type = Request["terminate_type"];
                terminateLicense.terminate_reason = Guid.Parse(Request["terminate_reason"].ToString());
                terminateLicense.terminate_date = Convert.ToDateTime(Request["terminate_date"].ToString());
                terminateLicense.license_ref = Guid.Parse(Request["license_ref"].ToString());
                terminateLicense.terminate_license_idx = Guid.Parse(Request["terminate_licenses_idx"].ToString());
                var license = ilpBLL.UpdateTerminateLicense(terminateLicense);
                ilpBLL.DestroyTerminateBranches(license.terminate_license_idx);
                if (ids != null && ids.Length != 0)
                {
                    foreach (var id in ids)
                    {
                        ILPTerminateBranch branch = new ILPTerminateBranch();
                        branch.terminate_license_ref = license.terminate_license_idx;
                        branch.branch_ref = Guid.Parse(id);
                        ilpBLL.StoreTerminateBranch(branch, Guid.Parse(Session["UID"].ToString()));
                    }
                }
                ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));
                return true;
            }
            else
            {
                terminateLicense.terminate_type = Request["terminate_type"];
                terminateLicense.terminate_reason = Guid.Parse(Request["terminate_reason"].ToString());
                terminateLicense.terminate_date = Convert.ToDateTime(Request["terminate_date"].ToString());
                terminateLicense.license_ref = Guid.Parse(Request["license_ref"].ToString());
                var license = ilpBLL.StoreTerminateLicense(terminateLicense, Guid.Parse(Session["UID"].ToString()));
                if (ids != null && ids.Length != 0)
                {
                    foreach (var id in ids)
                    {
                        ILPTerminateBranch branch = new ILPTerminateBranch();
                        branch.terminate_license_ref = license.terminate_license_idx;
                        branch.branch_ref = Guid.Parse(id);
                        ilpBLL.StoreTerminateBranch(branch, Guid.Parse(Session["UID"].ToString()));
                    }
                }
                ilpBLL.UpdateChecklistStatus(Guid.Parse(Request["chkitem_instance_idx"].ToString()));
                return true;
            }

        }

        [HttpPost]
        public JsonResult GetTerminateLicense()
        {
            var terminate_license = ilpBLL.GetTerminateLicense(Guid.Parse(Request["license_ref"].ToString()));
            var terminate_branch = ilpBLL.GetTerminateBranch(terminate_license.terminate_license_idx);
            Object[] obj = {
                new { terminate_license, terminate_branch}
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTerminateLicenseCommon(string applyId)
        {
            var stubidx = TourlistUnitOfWork.IlpLicenses.Find(x => x.stub_ref.ToString() == applyId).FirstOrDefault();
            var terminate_license = ilpBLL.GetTerminateLicense(stubidx.ilp_idx);
            var terminate_branch = ilpBLL.GetTerminateBranch(terminate_license.terminate_license_idx);
            Object[] obj = {
                new { terminate_license, terminate_branch}
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCountries()
        {
            var countries = ilpBLL.GetCountries().OrderBy(c => c.country_name); ;
            Object[] obj = {
                new { countries }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPerson()
        {
            var person = ilpBLL.GetPerson(Guid.Parse(Request["ilp_license_ref"].ToString()));
            Object[] obj = {
                person
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetReferences()
        {
            var references = ilpBLL.GetReferences(Request["reference"].ToString());
            Object[] obj = {
                references
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadSupportDoc()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";
                        //string filename = Path.GetFileName(Request.Files[i].FileName);
                        var id = Request.Form.GetValues("id");
                        HttpPostedFileBase file = files[i];
                        string fname;
                        string name;
                        DateTime current = DateTime.Now;
                        long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                        name = unixTime + "_" + file.FileName;

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                        string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                        ilpBLL.UpdateSupportDoc(Guid.Parse(id[i]), file.FileName, name, path);
                        //file.SaveAs(fname);
                        //use common upload in base contoller
                        this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);
                    }
                    // Returns message that successfully uploaded

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
        public JsonResult ILPUploads()
        {
            var uploads = ilpBLL.GetIlpUploads(Guid.Parse(Request["application_ref"].ToString()), Request["type"]);
            var list = uploads.OrderBy(c => c.created_at);
            Object[] obj = {
                new { list }
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void StoreUpload()
        {
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string fname;
                    string name;
                    DateTime current = DateTime.Now;
                    long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
                    name = unixTime + "_" + file.FileName;

                    fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
                    string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
                    //file.SaveAs(fname);
                    //use common upload in base contoller
                    this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);
                    var license = ilpBLL.GetPermohonanBaharuByIdx(Guid.Parse(Request["application_ref"].ToString()));
                    ilpBLL.StoreAdditionalDoc(license.stub_ref, Request["document_name"].ToString(), file.FileName, path, Guid.Parse(Session["UID"].ToString()));
                }
            }
        }

        //[HttpPost]
        //public void UpdateUpload()
        //{
        //    if (Request.Files.Count > 0)
        //    {
        //        HttpFileCollectionBase files = Request.Files;
        //        for (int i = 0; i < files.Count; i++)
        //        {
        //            HttpPostedFileBase file = files[i];
        //            string fname;
        //            string name;
        //            DateTime current = DateTime.Now;
        //            long unixTime = ((DateTimeOffset)current).ToUnixTimeSeconds();
        //            name = unixTime + "_" + file.FileName;

        //            fname = Path.Combine(Server.MapPath("~/Attachment/"), name);
        //            string path = this.GetUploadFolder(Tourlist.Common.TourlistEnums.MotacModule.ILP, name); //"/Attachment/" + name;
        //            //file.SaveAs(fname);
        //            //use common upload in base contoller
        //            this.UploadSuppDocs(file, name, Tourlist.Common.TourlistEnums.MotacModule.ILP);
        //            ilpBLL.UpdateILPUploads(Guid.Parse(Request["id"].ToString()), file.FileName, name, path);
        //        }
        //    }
        //}

        [HttpPost]
        public void DeleteUpload()
        {
            ilpBLL.DestroyILPUpload(Guid.Parse(Request["ilp_upload_idx"]));
        }

        [HttpPost]
        public void DeleteInstructorCourse()
        {
            ilpBLL.DestroyILPInstructorCourse(Guid.Parse(Request["instructor_courses_idx"]));
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
            //  model_org.company_totalcharge = Request["company_totalcharge"].ToString();
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


            var Application = coreOrgHelper.UpdateCompanyDataSSM(module_id, userID, component_id, model_org, "ILP");

            return Json(Application, JsonRequestBehavior.DenyGet);
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

                Application = coreOrgHelper.ajaxUpdateCompanyDirectorsSSM(module_id, component_id, userID, model_org, "ILP");
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

                Application = coreOrgHelper.ajaxUpdateCompanyShareholdersSSM(module_id, component_id, userID, model_org, "ILP");
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
        public JsonResult ajaxApplication(string module)
        {
            var userID = Session["UID"].ToString();
            var Application = ilpBLL.GetApplicationStatusMain1(Session["UID"].ToString(), module);

            return Json(Application, JsonRequestBehavior.AllowGet);
        }

        public DateTime JsonDateTimeToNormal(string jsonDateTime)
        {
            jsonDateTime = @"""" + jsonDateTime + @"""";
            return Newtonsoft.Json.JsonConvert.DeserializeObject<DateTime>(jsonDateTime);
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
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", orgUpdate.stub_ref, "ilp"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", orgUpdate.stub_ref, "ilp"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", orgUpdate.stub_ref, "ilp"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                }
                else
                {
                    if (file_PerjanjianSewaBeliPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", orgUpdate.stub_ref, "ilp"); //"Perjanjian Sewa Beli Premis"
                        uploadDocSokongan(file_PerjanjianSewaBeliPermis, chkitem_instance);
                    }
                    if (file_PelanLantaiPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", orgUpdate.stub_ref, "ilp");//"Pelan Lantai Premis Perniagaan"
                        uploadDocSokongan(file_PelanLantaiPermis, chkitem_instance);
                    }
                    if (file_GambarPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", orgUpdate.stub_ref, "ilp");//"Salinan Lesen *"
                        uploadDocSokongan(file_GambarPermis, chkitem_instance);
                    }
                }
                if (file_salinanAsalLesen != null)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SALINANLESEN", orgUpdate.stub_ref, "ilp");//"Salinan Lesen *"
                    uploadDocSokongan(file_salinanAsalLesen, chkitem_instance);
                }
                bool itemID = coreOrgHelper.updateListing(ItemID);


            }



            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri (CR#57259)  on 5 jan 2024
        [HttpPost]
        public JsonResult ajaxChangeStatusBranchUpdate(core_organizations_updated orgUpdate, ilp_branches_updated ilpUpdate, 
            HttpPostedFileBase file_PerjanjianSewaBeliPermis,HttpPostedFileBase file_PelanLantaiPermis, 
            HttpPostedFileBase file_salinanAsalLesen, HttpPostedFileBase file_GambarPermis, string ItemID, string branch_stub_ref)
        {
            string userID = Session["UID"].ToString();

            Guid gUserID = Guid.Parse(userID);
            var is_premise = orgUpdate.is_premise_ready;

            TourlistUnitOfWork.IlpBranchesUpdated.SaveNewIlpBranch(ilpUpdate, gUserID);
            bool upd = coreOrgHelper.UpdateChangeStatusOrgforBranchInd(orgUpdate, gUserID);

            Guid chkitem_instance = Guid.Empty;

            Guid branch_stub_ref_idx = Guid.Parse(branch_stub_ref);
            uploadBranchUpdatedDoc(branch_stub_ref_idx, gUserID);

            if (upd == true)
            {

                if (is_premise == 0)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "ilp"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "ilp"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "ilp"); //"Perjanjian Sewa Beli Premis"
                    RemoveDocList(chkitem_instance);
                }
                else
                {
                    if (file_PerjanjianSewaBeliPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "ilp"); //"Perjanjian Sewa Beli Premis"
                        uploadDocSokongan(file_PerjanjianSewaBeliPermis, chkitem_instance);
                    }
                    if (file_PelanLantaiPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "ilp");//"Pelan Lantai Premis Perniagaan"
                        uploadDocSokongan(file_PelanLantaiPermis, chkitem_instance);
                    }
                    if (file_GambarPermis != null)
                    {
                        chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "ilp");//"Salinan Lesen *"
                        uploadDocSokongan(file_GambarPermis, chkitem_instance);
                    }
                }
                if (file_salinanAsalLesen != null)
                {
                    chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SALINANLESEN", branch_stub_ref_idx, "ilp");//"Salinan Lesen *"
                    uploadDocSokongan(file_salinanAsalLesen, chkitem_instance);
                }
                //bool itemID = coreOrgHelper.updateListing(ItemID);


            }

            return Json(upd, JsonRequestBehavior.AllowGet);
        }

        //added by samsuri on 9 jan 2024
        private void uploadBranchUpdatedDoc(Guid branch_stub_ref_idx, Guid userIdx)
        {
            Guid chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("SEWABELI", branch_stub_ref_idx, "ilp"); 

            if(chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = Guid.Parse("7A15C6B7-F69E-4907-905B-86727FEBBE30"); //SEWABELI
                var ilpBranchLic = TourlistUnitOfWork.IlpLicenses.GetIlpLicenseByStubRef(branch_stub_ref_idx);
                core_Chkitems_Instances.chklist_instance_ref = (Guid)ilpBranchLic.supporting_document_list;
                core_Chkitems_Instances.bool1 = 0;
                core_Chkitems_Instances.active_status = 1;
                core_Chkitems_Instances.created_at = DateTime.Now;
                core_Chkitems_Instances.modified_at = DateTime.Now;
                core_Chkitems_Instances.created_by = userIdx;
                core_Chkitems_Instances.modified_by = userIdx;
                TourlistUnitOfWork.CoreChkItemsInstancesRepository.Add(core_Chkitems_Instances);
                TourlistContext.SaveChanges();
            }
            
            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("PELANLANTAI", branch_stub_ref_idx, "ilp");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = Guid.Parse("BD1AC371-9F3E-4C8F-8484-EBDB0C4E7C73"); //PELANLANTAI
                var ilpBranchLic = TourlistUnitOfWork.IlpLicenses.GetIlpLicenseByStubRef(branch_stub_ref_idx);
                core_Chkitems_Instances.chklist_instance_ref = (Guid)ilpBranchLic.supporting_document_list;
                core_Chkitems_Instances.bool1 = 0;
                core_Chkitems_Instances.active_status = 1;
                core_Chkitems_Instances.created_at = DateTime.Now;
                core_Chkitems_Instances.modified_at = DateTime.Now;
                core_Chkitems_Instances.created_by = userIdx;
                core_Chkitems_Instances.modified_by = userIdx;
                TourlistUnitOfWork.CoreChkItemsInstancesRepository.Add(core_Chkitems_Instances);
                TourlistContext.SaveChanges();
            }

            chkitem_instance = coreOrgHelper.GetChkitemInstanceIdxByCode("GAMBAR", branch_stub_ref_idx, "ilp");
            if (chkitem_instance == Guid.Empty)
            {
                core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
                core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                core_Chkitems_Instances.chklist_tplt_item_ref = Guid.Parse("33C2DD1E-044C-4183-AF24-6390DAE72F5C"); //GAMBAR
                var ilpBranchLic = TourlistUnitOfWork.IlpLicenses.GetIlpLicenseByStubRef(branch_stub_ref_idx);
                core_Chkitems_Instances.chklist_instance_ref = (Guid)ilpBranchLic.supporting_document_list;
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

        public void uploadDocSokongan(HttpPostedFileBase file, Guid chkitem_instance)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            //string contentType = file.ContentType;
            string folder = "/Attachment/" + filename;
            string foldersave = Server.MapPath("~/Attachment");
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            doc.FileName = file.FileName;
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.ILP, filename); //folder;
            doc.chkitem_instanceID = chkitem_instance.ToString();
            doc.UploadFileName = filename;

            bool ID = coreOrgHelper.updateDocumentSokongan(doc);

            if (ID)
            {
                //use common upload in base contoller
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.ILP);

            }
        }
        public void RemoveDocList(Guid chkitem_instance)
        {

            //var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            //string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            ////string contentType = file.ContentType;
            //string folder = "/Attachment/" + filename;
            //string foldersave = Server.MapPath("~/Attachment");
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
        public JsonResult ajaxPBT(string NegeriID)
        {

            var PBT = coreOrgHelper.GetRefPBT(NegeriID);

            return Json(PBT, JsonRequestBehavior.AllowGet);
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

        public void uploadPersonID(HttpPostedFileBase file, string PersonID, string typeFile)
        {

            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            string filename = Path.GetFileName(Timestamp + "_" + file.FileName);
            string folder = "/Attachment/" + filename;
            string foldersave = Server.MapPath("~/Attachment");
            CoreOrganizationModel.CoreDocument doc = new CoreOrganizationModel.CoreDocument();

            var userID = Session["UID"].ToString();
            doc.FileName = file.FileName;
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.ILP, filename); //folder;
            doc.PersonID = PersonID;
            doc.UploadFileName = filename;
            doc.type = typeFile;
            doc.userID = userID;

            bool ID = coreOrgHelper.updateDocumentSokonganPerson(doc);

            if (ID)
            {
                //use common upload in base contoller
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.ILP);
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
            doc.UploadLocation = this.GetUploadFolder(TourlistEnums.MotacModule.ILP, filename); //folder;
            doc.OrgID = OrgID;
            doc.UploadFileName = filename;
            doc.type = typeFile;
            doc.userID = userID;

            bool ID = coreOrgHelper.updateDocumentSokonganOrganization(doc);

            if (ID)
            {
                this.UploadSuppDocs(file, filename, TourlistEnums.MotacModule.ILP);
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
            doc.document_upload_path = this.GetUploadFolder(TourlistEnums.MotacModule.ILP, filename); //folder;

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

                return true;
            }
            return false;

        }



        [HttpPost]
        public JsonResult ajaxShareHolderDetail(string identifier)
        {
            var ShareHolder = coreOrgHelper.GetShareHolderDetail(identifier);
            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ajaxShareHolderDetailByID(string identifier)
        {
            var ShareHolder = coreOrgHelper.GetShareHolderDetailByID(identifier);
            return Json(ShareHolder, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ajaxChangeStatusShareholderOrgUpdate(TourlistDataLayer.DataModel.core_organizations obj, string status_shareholder, string number_of_shares, string registered_year, string country_ref, string justification, string justificationID,
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
        public JsonResult ajaxChangeStatusDirectorDetail(string IDX)
        {
            //var directors = coreOrgHelper.GetDirectorDetail(IDX);
            var directors = coreOrgHelper.GetChangeStatusDirectorDetail(IDX);

            return Json(directors, JsonRequestBehavior.AllowGet);
        }
    }
}