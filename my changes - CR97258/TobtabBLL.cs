using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Tourlist.Common;
using TourlistBusinessLayer.Models;
using TourlistDataLayer.DataModel;
using TourlistDataLayer.Persistence;
using TourlistDataLayer.ViewModels.Tobtab;
using System.Globalization;
using static TourlistDataLayer.ViewModels.Tobtab.TobtabViewModels;

using System.Runtime.Remoting;
using System.Web.Mvc;

namespace TourlistBusinessLayer.BLL
{
    public class TobtabBLL : BusinessLayerBaseClass
    {

        private TourlistUnitOfWork unitOfWork
        {
            get
            {
                return this.TourlistUnitOfWork;
            }
        }

        public int GetLicense(Guid user_id)
        {

            var data = unitOfWork.TobtabLicenses.Find(i => (i.created_by.ToString() == user_id.ToString())).ToList();

            return data.Count;
        }

        public List<tobtab_licenses> GetLicenses(Guid user_id)
        {

            var data = unitOfWork.TobtabLicenses.Find(i => (
                                                        i.created_by.ToString() == user_id.ToString()
                                                    )).ToList();

            return data;
        }


        public Guid GetModuleID(string module_id)
        {
            return unitOfWork.CoreModules.GetCoreModuleIdxByName(module_id);
        }


        public List<ApplicationsLists> GetNewLicenseApplications(Guid user_id)
        {
            var module_idx = unitOfWork.CoreModules.GetCoreModuleIdxByName("TOBTAB_NEW");

            var data = GetActiveApplicationsByUserIDModule(user_id, module_idx);

            List<ApplicationsLists> app_data = new List<ApplicationsLists>();

            foreach (var app in data)
            {
                var app_list = new ApplicationsLists();
                app_list.module_idx = app.apply_module;
                app_list.user_idx = app.apply_user;
                app_list.apply_idx = app.apply_idx;

                app_data.Add(app_list);
            }

            return app_data;
        }

        public List<ApplicationsLists> GetRenewLicenseApplications(Guid user_id)
        {
            var module_idx = unitOfWork.CoreModules.GetCoreModuleIdxByName("TOBTAB_RENEW");

            var completed_status = Guid.Parse("5BE3C948-F569-40CB-B764-AF6E9E8CC302");

            var data = GetApplicationsByUserIDModuleStatus(user_id, module_idx, completed_status);
            List<ApplicationsLists> app_data = new List<ApplicationsLists>();

            foreach (var app in data)
            {
                var app_list = new ApplicationsLists();
                app_list.module_idx = app.apply_module;
                app_list.user_idx = app.apply_user;
                app_list.apply_idx = app.apply_idx;

                app_data.Add(app_list);
            }

            return app_data;
        }

        public List<ApplicationsLists> GetCompletedLicenseApplications(Guid user_id)
        {
            var module_idx = unitOfWork.CoreModules.GetCoreModuleIdxByName("TOBTAB_NEW");

            var completed_status = Guid.Parse("A45AD140-8592-4F17-BB1E-21C0289FC50A");

            var data = GetApplicationsByUserIDModuleStatus(user_id, module_idx, completed_status);
            List<ApplicationsLists> app_data = new List<ApplicationsLists>();

            foreach (var app in data)
            {
                var app_list = new ApplicationsLists();
                app_list.module_idx = app.apply_module;
                app_list.user_idx = app.apply_user;
                app_list.apply_idx = app.apply_idx;

                app_data.Add(app_list);
            }

            return app_data;
        }

        //added by samsuri (CR#57258)  on 2 jan 2024
        public List<TOBTABBranch> GetTOBTABBranchesActive(Guid Org_ref)
        {
            var TTBranches = new List<TOBTABBranch>();
            var datas = unitOfWork.TobtabAddBranchesRepository.GetTobtabBranchesByOrgID(Org_ref);

            foreach (var data in datas)
            {
                TOBTABBranch TTBranch = new TOBTABBranch();
                TTBranch.tobtab_add_branches_idx = data.tobtab_add_branches_idx;
                TTBranch.branch_name = data.branch_name;
                TTBranch.branch_addr_1 = data.branch_addr_1;
                TTBranch.branch_addr_2 = data.branch_addr_2;
                TTBranch.branch_addr_3 = data.branch_addr_3;
                TTBranch.branch_postcode = data.branch_postcode;
                TTBranch.branch_phone_no = data.branch_phone_no;
                TTBranch.branch_mobile_no = data.branch_mobile_no;
                TTBranch.branch_state = (Guid)data.branch_state;
                TTBranch.branch_city = (Guid)data.branch_city;
                TTBranch.branch_email = data.branch_email;
                TTBranch.branch_fax_no = data.branch_fax_no;
                //TTBranch.branch_size = data.branch_size;
                TTBranch.branch_email = data.branch_email;
                //TTBranch.utility = data.utility != null ? (Guid)data.utility : Guid.Empty; //modified b samsuri on 8 jan 2024
                //TTBranch.utility_others = data.utility_others;
                TTBranch.branch_website = data.branch_website;
                //TTBranch.authorized_capital = data.authorized_capital != null ? (Decimal)data.authorized_capital : 0; //modified b samsuri on 8 jan 2024
                //TTBranch.paid_capital = data.paid_capital != null ? (Decimal)data.paid_capital : 0; //modified b samsuri on 8 jan 2024
                TTBranch.application_stub_ref = data.application_stub_ref;
                TTBranch.pbt_ref = data.pbt_ref;

                ref_status_record statusRecord = unitOfWork.RefStatusRecordRepository.Find(x => x.status_idx == data.active_status).FirstOrDefault();
                var guidActive = statusRecord != null ? statusRecord.status_name : "";
                if (guidActive == "ACTIVE") TTBranch.active_status = data.active_status;

                //modified b samsuri on 8 jan 2024
                //if (TTBranch.utility != Guid.Empty)
                //{
                //    var utilities = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.utility);
                //    var multiUtilities = new List<ILPMultiSelect>();
                //    foreach (var utility in utilities)
                //    {
                //        ILPMultiSelect multiUtility = new ILPMultiSelect();
                //        multiUtility.parent_ref = utility.parent_ref;
                //        multiUtility.details_ref = utility.details_ref;
                //        multiUtility.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)utility.details_ref).ref_description;
                //        multiUtilities.Add(multiUtility);
                //    }
                //    ilpBranch.multi_utility = multiUtilities;
                //}

                TTBranches.Add(TTBranch);
            }
            return TTBranches;
        }

        //added by samsuri (CR#57258) on 10 jan 2024
        private Guid GetTOBTABBranchUploadedDoc(string code, Guid stubRef)
        {
            var TobtabLicense = unitOfWork.TobtabLicenses.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
            var coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "TOBTAB_CHANGE_STATUS_DOCUMENT")).FirstOrDefault();
            var coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();
            var coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == TobtabLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();
            if (coreChkitemsInstances == null) return Guid.Empty;
            return coreChkitemsInstances.chkitem_instance_idx;
        }

        //Added by samsuri (CR#57258)  on 10 Jan 2024
        public List<TOBTABBranch> GetTobtabBranchesbyBranchIdx(Guid Branch_Idx)
        {
            var TTBranches = new List<TOBTABBranch>();
            var datas = unitOfWork.TobtabAddBranchesRepository.GetTobtabBranchesByBranchIdx(Branch_Idx);

            foreach (var data in datas)
            {
                TOBTABBranch TTBranch = new TOBTABBranch();
                TTBranch.tobtab_add_branches_idx = data.tobtab_add_branches_idx;
                TTBranch.branch_name = data.branch_name;
                TTBranch.branch_addr_1 = data.branch_addr_1;
                TTBranch.branch_addr_2 = data.branch_addr_2;
                TTBranch.branch_addr_3 = data.branch_addr_3;
                TTBranch.branch_postcode = data.branch_postcode;
                TTBranch.branch_phone_no = data.branch_phone_no;
                TTBranch.branch_mobile_no = data.branch_mobile_no;
                TTBranch.branch_state = (Guid)data.branch_state;
                TTBranch.branch_city = (Guid)data.branch_city;
                TTBranch.branch_email = data.branch_email;
                TTBranch.branch_fax_no = data.branch_fax_no;
                //TTBranch.branch_size = data.branch_size;
                TTBranch.branch_email = data.branch_email;
                //TTBranch.utility = data.utility != null ? (Guid)data.utility : Guid.Empty; //modified b samsuri on 8 jan 2024
                //TTBranch.utility_others = data.utility_others;
                TTBranch.branch_website = data.branch_website;
                //TTBranch.authorized_capital = data.authorized_capital != null ? (Decimal)data.authorized_capital : 0; //modified b samsuri on 8 jan 2024
                //TTBranch.paid_capital = data.paid_capital != null ? (Decimal)data.paid_capital : 0; //modified b samsuri on 8 jan 2024
                //var ilpBranchLic = unitOfWork.IlpLicenses.GetIlpLicenseByIdx(data.ilp_license_idx);
                TTBranch.application_stub_ref = data.application_stub_ref; //use for upload docs
                TTBranch.pbt_ref = data.pbt_ref;

                var chkitem_instanceSewaBeliPremis = GetTOBTABBranchUploadedDoc("SEWABELI", TTBranch.application_stub_ref);//"Perjanjian Sewa Beli Premis"
                var perjanjianSewaBeliPremis = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSewaBeliPremis).FirstOrDefault();
                TTBranch.fileNameSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.string1;
                TTBranch.fileLocSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.upload_location;

                var chkitem_instancePelanLantai = GetTOBTABBranchUploadedDoc("PELANLANTAI", TTBranch.application_stub_ref);//"Pelan Lantai Premis Perniagaan"
                var pelanLantaiPremisPerniagaan = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instancePelanLantai).FirstOrDefault();
                TTBranch.fileNamepelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.string1;
                TTBranch.fileLocpelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.upload_location;

                var chkitem_instanceGambar = GetTOBTABBranchUploadedDoc("GAMBAR", TTBranch.application_stub_ref);//"Gambar Bahagian Dalam dan Luar Pejabat (Berwarna)"
                var gambar = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceGambar).FirstOrDefault();
                TTBranch.fileNameGambarPermis = gambar == null ? "" : gambar.string1;
                TTBranch.fileLocGambarPermis = gambar == null ? "" : gambar.upload_location;

                //modified b samsuri on 8 jan 2024
                //if (ilpBranch.utility != Guid.Empty)
                //{
                //    var utilities = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.utility);
                //    var multiUtilities = new List<ILPMultiSelect>();
                //    foreach (var utility in utilities)
                //    {
                //        ILPMultiSelect multiUtility = new ILPMultiSelect();
                //        multiUtility.parent_ref = utility.parent_ref;
                //        multiUtility.details_ref = utility.details_ref;
                //        multiUtility.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)utility.details_ref).ref_description;
                //        multiUtilities.Add(multiUtility);
                //    }
                //    ilpBranch.multi_utility = multiUtilities;
                //}

                var datasUpdated = unitOfWork.TobtabAddBranchesRepository.GetTobtabBranchesUpdatedByBranchIdx(Branch_Idx);
                var TTBranchUpdated = new List<TOBTABBranchUpdated>();
                foreach (var dataUpd in datasUpdated)
                {
                    TOBTABBranchUpdated TTBranchUpd = new TOBTABBranchUpdated();
                    TTBranchUpd.new_branch_addr_1 = dataUpd.new_branch_addr_1;
                    TTBranchUpd.new_branch_addr_2 = dataUpd.new_branch_addr_2;
                    TTBranchUpd.new_branch_addr_3 = dataUpd.new_branch_addr_3;
                    TTBranchUpd.new_branch_postcode = dataUpd.new_branch_postcode;
                    TTBranchUpd.new_branch_city = (Guid)dataUpd.new_branch_city;
                    TTBranchUpd.new_branch_state = (Guid)dataUpd.new_branch_state;

                    TTBranchUpd.old_branch_addr_1 = dataUpd.old_branch_addr_1;
                    TTBranchUpd.old_branch_addr_2 = dataUpd.old_branch_addr_2;
                    TTBranchUpd.old_branch_addr_3 = dataUpd.old_branch_addr_3;
                    TTBranchUpd.old_branch_postcode = dataUpd.old_branch_postcode;
                    TTBranchUpd.old_branch_city = (Guid)dataUpd.old_branch_city;
                    TTBranchUpd.old_branch_state = (Guid)dataUpd.old_branch_state;

                    TTBranchUpdated.Add(TTBranchUpd);
                }
                TTBranch.branch_updated = TTBranchUpdated;

                TTBranches.Add(TTBranch);
            }
            return TTBranches;
        }

        public List<TourlistDataLayer.DataModel.flow_application_stubs> GetActiveApplicationsByUserIDModule(Guid user_idx, Guid module_idx)
        {
            List<TourlistDataLayer.DataModel.flow_application_stubs> newList = new List<TourlistDataLayer.DataModel.flow_application_stubs>();
            var applications = unitOfWork.FlowApplicationStubs.Find(c => c.apply_user == user_idx && c.apply_module == module_idx).ToList();

            foreach (var app in applications)
            {

                var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == app.apply_idx.ToString())).FirstOrDefault();
                if (license != null)
                {
                    if (license.active_status == 1)
                    {
                        var app_list = new TourlistDataLayer.DataModel.flow_application_stubs();
                        app_list.apply_module = app.apply_module;
                        app_list.apply_user = app.apply_user;
                        app_list.apply_idx = app.apply_idx;
                        app_list.apply_status = app.apply_status;
                        app_list.application_date = app.application_date;
                        app_list.applied_begin_date = app.applied_begin_date;
                        app_list.applied_end_date = app.applied_end_date;
                        app_list.actual_begin_date = app.actual_begin_date;
                        app_list.actual_end_date = app.actual_end_date;
                        app_list.actual_period = app.actual_period;
                        app_list.created_dt = app.created_dt;
                        app_list.modified_dt = app.modified_dt;
                        app_list.created_by = app.created_by;
                        app_list.modified_by = app.modified_by;
                        newList.Add(app_list);
                    }
                }

            }
            return newList;
        }

        public List<TourlistDataLayer.DataModel.flow_application_stubs> GetApplicationsByUserIDModuleStatus(Guid user_idx, Guid module_idx, Guid status)
        {
            List<TourlistDataLayer.DataModel.flow_application_stubs> newList = new List<TourlistDataLayer.DataModel.flow_application_stubs>();
            var applications = unitOfWork.FlowApplicationStubs.Find(c => c.apply_user == user_idx && c.apply_module == module_idx && c.apply_status == status).ToList();

            foreach (var app in applications)
            {

                var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == app.apply_idx.ToString())).FirstOrDefault();
                if (license != null)
                {
                    var app_list = new TourlistDataLayer.DataModel.flow_application_stubs();
                    app_list.apply_module = app.apply_module;
                    app_list.apply_user = app.apply_user;
                    app_list.apply_idx = app.apply_idx;
                    app_list.apply_status = app.apply_status;
                    app_list.application_date = app.application_date;
                    app_list.applied_begin_date = app.applied_begin_date;
                    app_list.applied_end_date = app.applied_end_date;
                    app_list.actual_begin_date = app.actual_begin_date;
                    app_list.actual_end_date = app.actual_end_date;
                    app_list.actual_period = app.actual_period;
                    app_list.created_dt = app.created_dt;
                    app_list.modified_dt = app.modified_dt;
                    app_list.created_by = app.created_by;
                    app_list.modified_by = app.modified_by;
                    newList.Add(app_list);

                }

            }
            return newList;
        }

        public void CreateNewLicense()
        {
            var data = unitOfWork.TobtabLicenses.SaveNewLicense();
        }

        public TourlistDataLayer.DataModel.tobtab_licenses GetLicenseApplication(string tobtab_id)
        {
            var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == tobtab_id)).FirstOrDefault();

            var data = new TourlistDataLayer.DataModel.tobtab_licenses();

            if (license != null)
            {
                data.tobtab_idx = license.tobtab_idx;
                data.stub_ref = license.stub_ref;
                data.license_ref_code = license.license_ref_code;
                data.license_type_list = license.license_type_list;
                data.organization_ref = license.organization_ref;
                data.supporting_document_list = license.supporting_document_list;
                data.active_status = license.active_status;
                data.created_at = license.created_at;
                data.created_by = license.created_by;
                data.modified_at = license.modified_at;
                data.modified_by = license.modified_by;
                data.license_suspension_period_ref = license.license_suspension_period_ref;
                data.license_reason_delay_ref = license.license_reason_delay_ref;
                data.inbound = license.inbound;
                data.outbound = license.outbound;
                data.ticketing = license.ticketing;
                data.umrah = license.umrah;
                data.foreign_relations = license.foreign_relations;
            }
            return data;
        }

        public TourlistDataLayer.DataModel.tobtab_umrah_advertising GetUmrahAdvertising(Guid tobtab_id)
        {
            var license = unitOfWork.TobtabUmrahAdvertisingRepository.Find(i => (i.tobtab_licenses_ref == tobtab_id)).FirstOrDefault();
            return license;
        }
        public TourlistDataLayer.DataModel.tobtab_umrah_advertising saveUmrahAdvertising(tobtab_umrah_advertising umrah, Guid tobtab_idx)
        {
            umrah = new tobtab_umrah_advertising();
            umrah.tobtab_umrah_advertising_idx = Guid.NewGuid();
            umrah.tobtab_licenses_ref = tobtab_idx;
            umrah.created_at = DateTime.Now;
            unitOfWork.TobtabUmrahAdvertisingRepository.Add(umrah);
            unitOfWork.Complete();

            var license = unitOfWork.TobtabUmrahAdvertisingRepository.Find(i => (i.tobtab_umrah_advertising_idx == umrah.tobtab_umrah_advertising_idx)).FirstOrDefault();
            return license;
        }


        public string addMarketingArea(string marketing_agent_ref, string person_identifier, string license_ref, string state_idx, string userID)
        {
            string personIdx = null;
            if (marketing_agent_ref == null || marketing_agent_ref == "" || marketing_agent_ref == "undefined")
            {
                tobtab_marketing_agents agents = unitOfWork.TobtabMarketingAgentRepository.Find(i => (i.marketing_agent_idx.ToString() == marketing_agent_ref)).FirstOrDefault();
                if (agents != null)
                {
                    marketing_agent_ref = agents.marketing_agent_idx.ToString();
                    personIdx = agents.person_ref.ToString();
                }
                else
                {
                    core_persons person = unitOfWork.CorePersonsRepository.Find(i => (i.person_identifier.ToString() == person_identifier)).FirstOrDefault();
                    if (person == null)
                    {
                        person = new core_persons();
                        person.person_idx = Guid.NewGuid();
                        person.person_identifier = person_identifier;
                        person.person_name = person_identifier;
                        person.person_type = unitOfWork.RefUserTypesRepository.Find(c => (c.userType_name == "I")).FirstOrDefault().userType_idx;
                        person.person_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                        person.modified_dt = DateTime.Now;
                        person.created_dt = DateTime.Now;
                        person.created_by = Guid.Parse(userID);
                        person.modified_by = Guid.Parse(userID);
                        unitOfWork.CorePersonsRepository.Add(person);

                        tobtab_marketing_agents marketingAgent = new tobtab_marketing_agents();
                        marketingAgent.marketing_agent_idx = Guid.NewGuid();
                        marketingAgent.created_by = Guid.Parse(userID);
                        marketingAgent.created_at = DateTime.Now;
                        marketingAgent.tobtab_licenses_ref = Guid.Parse(license_ref);
                        marketingAgent.person_ref = person.person_idx;
                        marketingAgent.person_position = person.person_idx.ToString();
                        marketingAgent.active_status = 1;
                        unitOfWork.TobtabMarketingAgentRepository.Add(marketingAgent);
                        marketing_agent_ref = marketingAgent.marketing_agent_idx.ToString();
                        personIdx = person.person_idx.ToString();
                    }
                    else
                    {
                        agents = unitOfWork.TobtabMarketingAgentRepository.Find(i => (i.person_ref == person.person_idx)).FirstOrDefault();
                        if (agents != null)
                        {
                            marketing_agent_ref = agents.marketing_agent_idx.ToString();
                            personIdx = agents.person_ref.ToString();
                        }
                        else
                        {
                            tobtab_marketing_agents marketingAgent = new tobtab_marketing_agents();
                            marketingAgent.marketing_agent_idx = Guid.NewGuid();
                            marketingAgent.created_by = Guid.Parse(userID);
                            marketingAgent.created_at = DateTime.Now;
                            marketingAgent.tobtab_licenses_ref = Guid.Parse(license_ref);
                            marketingAgent.person_ref = person.person_idx;
                            marketingAgent.person_position = person.person_idx.ToString();
                            marketingAgent.active_status = 1;
                            unitOfWork.TobtabMarketingAgentRepository.Add(marketingAgent);
                            marketing_agent_ref = marketingAgent.marketing_agent_idx.ToString();
                            personIdx = person.person_idx.ToString();
                        }
                    }
                }
            }
            else
            {
                tobtab_marketing_agents agents = unitOfWork.TobtabMarketingAgentRepository.Find(i => (i.marketing_agent_idx.ToString() == marketing_agent_ref)).FirstOrDefault();
                if (agents != null)
                {
                    marketing_agent_ref = agents.marketing_agent_idx.ToString();
                    personIdx = agents.person_ref.ToString();
                }
            }
            tobtab_marketing_areas areas = new tobtab_marketing_areas();
            areas.marketing_areas_idx = Guid.NewGuid();
            areas.marketing_agent_idx = Guid.Parse(marketing_agent_ref);
            areas.state_marketing_ref = Guid.Parse(state_idx);
            areas.person_ref = Guid.Parse(personIdx);
            areas.created_at = DateTime.Now;
            areas.created_by = Guid.Parse(userID);
            areas.active_status = 1;
            unitOfWork.TobtabMarketingAreaRepository.Add(areas);
            unitOfWork.Complete();

            return marketing_agent_ref;
        }


        public string addTgTrip(tobtab_tg_traveling tgData)
        {
            unitOfWork.TobtabTGTravelingRepository.Add(tgData);
            unitOfWork.Complete();

            return "success";
        }

        public string addSchedule(string tobtab_umrah_advertising_ref, System.DateTime start_date, System.DateTime end_date, string userID)
        {
            tobtab_umrah_schedule schedule = new tobtab_umrah_schedule();
            schedule.tobtab_umrah_schedule_idx = Guid.NewGuid();
            schedule.tobtab_umrah_advertising_ref = Guid.Parse(tobtab_umrah_advertising_ref);
            if (start_date != null)
            {
                string date = start_date.ToString().Substring(0, 3);
                if (!date.ToString().Contains("0001"))
                {
                    schedule.schedule_start_date = start_date; //temp
                }
            }

            if (end_date != null)
            {
                string date = end_date.ToString().Substring(0, 3);
                if (!date.ToString().Contains("0001"))
                {
                    schedule.schedule_end_date = end_date; //temp
                }
            }
            schedule.created_at = DateTime.Now;
            schedule.created_by = Guid.Parse(userID);
            schedule.active_status = 1;
            unitOfWork.TobtabUmrahScheduleRepository.Add(schedule);
            unitOfWork.Complete();

            return "success";
        }
        public Boolean checkDuplicateState(string marketing_agent_idx, string state_idx)
        {
            Boolean result = false;
            var record = unitOfWork.TobtabMarketingAreaRepository.Find(i => i.marketing_agent_idx.ToString() == marketing_agent_idx
            && i.state_marketing_ref.ToString() == state_idx).FirstOrDefault();
            if (record != null)
            {
                result = true;
            }

            return result;
        }
        public Boolean checkDuplicateLanguage(string tobtab_umrah_advertising_ref, string language_idx)
        {
            Boolean result = false;
            var record = unitOfWork.TobtabUmrahLanguageRepository.Find(i => i.tobtab_umrah_advertising_ref.ToString() == tobtab_umrah_advertising_ref
            && i.language_ref.ToString() == language_idx).FirstOrDefault();
            if (record != null)
            {
                result = true;
            }

            return result;
        }
        public string addLanguage(string tobtab_umrah_advertising_ref, string license_ref, string language_idx, string userID)
        {
            tobtab_umrah_language language = new tobtab_umrah_language();
            language.tobtab_umrah_language_idx = Guid.NewGuid();
            language.tobtab_umrah_advertising_ref = Guid.Parse(tobtab_umrah_advertising_ref);
            language.language_ref = Guid.Parse(language_idx);
            language.created_at = DateTime.Now;
            language.created_by = Guid.Parse(userID);
            language.active_status = 1;
            unitOfWork.TobtabUmrahLanguageRepository.Add(language);
            unitOfWork.Complete();

            return "success";
        }
        public TourlistDataLayer.DataModel.tobtab_terminate_licenses GetLicenseReturnApplication(string tobtab_id)
        {
            var license = unitOfWork.TobtabTerminateLicenseRepository.Find(i => (i.terminate_license_idx.ToString() == tobtab_id)).FirstOrDefault();

            return license;
        }

        public tobtab_tg_exceptions GetTgExceptionInfo(string tobtab_idx)
        {
            var license = unitOfWork.TobtabTGExceptionsRepository.Find(i => (i.tobtab_licenses_ref.ToString() == tobtab_idx)).FirstOrDefault();
            return license;
        }
        public tobtab_tg_traveling GetTgTrip(string travelingIdx)
        {
            tobtab_tg_traveling tgTravel = new tobtab_tg_traveling();
            tgTravel = (from a in unitOfWork.TobtabTGTravelingRepository.Table
                                           where a.tobtab_tg_traveling_idx.ToString() == travelingIdx
                                           select a
                                           ).FirstOrDefault();
            //tobtab_tg_traveling tgTravel = unitOfWork.TobtabTGTravelingRepository.Find(i => i.tobtab_tg_traveling_idx.ToString() == travelingIdx).FirstOrDefault();
            return tgTravel;
        }
        public string UpdateTgTrip(tobtab_tg_traveling tgData)
        {
            tobtab_tg_traveling data = new tobtab_tg_traveling();
            data = unitOfWork.TobtabTGTravelingRepository.Find(i => i.tobtab_tg_traveling_idx == tgData.tobtab_tg_traveling_idx).FirstOrDefault();

            data.travel_purpose = tgData.travel_purpose;
            data.travel_purpose_others = tgData.travel_purpose_others;
            data.travel_destination = tgData.travel_destination;
            data.travel_leader = tgData.travel_leader;
            data.travel_route = tgData.travel_route;
            data.travel_accommodation = tgData.travel_accommodation;
            data.one_way = tgData.one_way;
            data.two_way = tgData.two_way;
            data.travel_state = tgData.travel_state;
            data.travel_town = tgData.travel_town;
            data.travel_location = tgData.travel_location;
            data.travel_start_date = tgData.travel_start_date;
            data.travel_start_time = tgData.travel_start_time;
            data.destination_state = tgData.destination_state;
            data.destination_town = tgData.destination_town;
            data.destination_location = tgData.destination_location;
            data.destination_start_date = tgData.destination_start_date;
            data.destination_start_time = tgData.destination_start_time;

            data.two_way_travel_state_ref = tgData.two_way_travel_state_ref;
            data.two_way_travel_town_ref = tgData.two_way_travel_town_ref;
            data.two_way_travel_location = tgData.two_way_travel_location;
            data.two_way_travel_start_date = tgData.two_way_travel_start_date;
            data.two_way_travel_start_time = tgData.two_way_travel_start_time;

            data.two_way_destination_state_ref = tgData.two_way_destination_state_ref;
            data.two_way_destination_town_ref = tgData.two_way_destination_town_ref;
            data.two_way_destination_location = tgData.two_way_destination_location;
            data.two_way_destination_start_date = tgData.two_way_destination_start_date;
            data.two_way_destination_start_time = tgData.two_way_destination_start_time;

            unitOfWork.TobtabTGTravelingRepository.Update(data);

            unitOfWork.Complete();

            return "success";

        }
        public tobtab_tg_traveling GetTgTravellingInfo(string tobtab_tg_traveling_idx)
        {
            var license = unitOfWork.TobtabTGTravelingRepository.Find(i => (i.tobtab_tg_traveling_idx.ToString() == tobtab_tg_traveling_idx)).FirstOrDefault();

            return license;
        }
        public TourlistDataLayer.DataModel.tobtab_add_branches GetAddBranchApplication(string tobtab_id)
        {
            var license = unitOfWork.TobtabAddBranchesRepository.Find(i => (i.application_stub_ref.ToString() == tobtab_id)).FirstOrDefault();

            return license;
        }


        public TourlistDataLayer.DataModel.core_users GetUserd(string sUserID)
        {
            TourlistDataLayer.DataModel.core_users users = unitOfWork.CoreUsersRepository.Find(i => (i.user_idx.ToString() == sUserID)).FirstOrDefault();
            return users;
        }
        public object GetTobtabHeader(string sUserID)
        {

            List<TourlistDataLayer.DataModel.core_users> users = new List<TourlistDataLayer.DataModel.core_users>();
            users = unitOfWork.CoreUsersRepository.Find(i => (i.user_idx.ToString() == sUserID)).ToList();

            var sOrg = "";
            foreach (var org in users)
            {
                sOrg = org.person_ref.ToString();
            }


            List<TourlistDataLayer.DataModel.core_organizations> organizations = new List<TourlistDataLayer.DataModel.core_organizations>();
            organizations = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == sOrg)).ToList();

            var clsApp = (from user in users
                          from organization in organizations

                          .Where(d => d.organization_idx.ToString() == user.person_ref.ToString())
                          .DefaultIfEmpty() // <== makes join left join  

                          select new
                          {
                              NamaSyarikat = organization.organization_name,
                              NoPendaftaranSyarikat = organization.organization_identifier,

                          }).ToList();

            return clsApp;


        }
        public object GetDokumenDashboardList(string AppID, string Module)
        {

            List<TourlistDataLayer.DataModel.tobtab_licenses> licenses = new List<TourlistDataLayer.DataModel.tobtab_licenses>();
            licenses = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == AppID)).ToList();


            List<TourlistDataLayer.DataModel.flow_application_stubs> applications = new List<TourlistDataLayer.DataModel.flow_application_stubs>();
            applications = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == AppID)).ToList();
            string modID = "";
            foreach (var application in applications)
            {
                modID = application.apply_module.ToString();
            }


            List<TourlistDataLayer.DataModel.core_sol_modules> modules = new List<TourlistDataLayer.DataModel.core_sol_modules>();
            modules = unitOfWork.CoreModules.Find(i => (i.modules_idx.ToString() == modID)).ToList();

            List<TourlistDataLayer.DataModel.core_chklist_instances> chklist_instances = new List<TourlistDataLayer.DataModel.core_chklist_instances>();
            chklist_instances = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.application_ref.ToString() == AppID)).ToList();

            List<TourlistDataLayer.DataModel.core_chkitems_instances> chkitems_instances = new List<TourlistDataLayer.DataModel.core_chkitems_instances>();
            chkitems_instances = unitOfWork.CoreChkItemsInstancesRepository.Find(i => i.created_by.ToString() != "FBC941C6-89B5-4DAE-B35A-4BD74460D891").ToList();

            List<TourlistDataLayer.DataModel.core_chklst_lists> chklst_lists = new List<TourlistDataLayer.DataModel.core_chklst_lists>();
            chklst_lists = unitOfWork.CoreChklstListsRepository.Find(i => (i.chklist_code.ToString() == Module)).ToList();

            List<TourlistDataLayer.DataModel.core_chklst_items> chklst_items = new List<TourlistDataLayer.DataModel.core_chklst_items>();
            chklst_items = unitOfWork.CoreChklstItemsRepository.GetAll().ToList();

            var clsList = (from license in licenses
                           from app in applications

                             .Where(d => d.apply_idx.ToString() == license.stub_ref.ToString())
                             .DefaultIfEmpty() // <== makes join left join  
                           from chklist_instance in chklist_instances
                                .Where(p => p.application_ref.ToString() == AppID)

                           from mod in modules
                                   .Where(p => p.modules_idx == app.apply_module)

                           from chklst_list in chklst_lists
                               .Where(p => p.chklist_idx.ToString() == chklist_instance.chklist_tplt_ref.ToString())

                           from chkitems_instance in chkitems_instances
                          .Where(p => p.chklist_instance_ref.ToString() == chklist_instance.chklist_instance_idx.ToString())

                           from chklst_item in chklst_items
                          .Where(p => p.item_idx.ToString() == chkitems_instance.chklist_tplt_item_ref.ToString())

                           select new
                           {
                               chkitem_instance_idx = chkitems_instance.chkitem_instance_idx.ToString(),
                               apply_idx = app.apply_idx.ToString(),
                               chklist_code = chklst_list.chklist_code.ToString(),
                               bool1 = chkitems_instance.bool1,
                               descr_bool1 = chklst_item.descr_bool1,
                               descr_bool2 = chklst_item.descr_bool2,
                               orderx = chklst_item.orderx,
                               descr_string1 = chklst_item.descr_string1,
                               descr_string2 = chklst_item.descr_string2,
                               upload_location = chkitems_instance.upload_location,
                               upload_date = chkitems_instance.date1,
                               file_name = chkitems_instance.string1,
                               Upload_file_name = chkitems_instance.string2,
                               module_name = mod.module_name

                           }).OrderBy(i => i.orderx).ToList();


            List<TobtabViewModels.tobtab_supporting_documents> modelList = new List<TobtabViewModels.tobtab_supporting_documents>();



            foreach (var dashboard in clsList)
            {

                TobtabViewModels.tobtab_supporting_documents model = new TobtabViewModels.tobtab_supporting_documents();
                {

                    model.chkitem_instance_idx = dashboard.chkitem_instance_idx;
                    model.apply_idx = dashboard.apply_idx;
                    model.chklist_code = dashboard.chklist_code.ToString();
                    model.bool1 = dashboard.bool1.ToString();
                    model.descr_bool1 = dashboard.descr_bool1;
                    model.descr_bool2 = dashboard.descr_bool2;
                    model.orderx = dashboard.orderx;
                    model.descr_string1 = dashboard.descr_string1;
                    model.descr_string2 = dashboard.descr_string2;
                    model.file_name = dashboard.file_name;
                    model.upload_location = dashboard.upload_location;
                    model.supload_date = String.Format("{0:dd/MM/yyyy}", dashboard.upload_date);
                    model.Upload_file_name = dashboard.Upload_file_name;
                    /*  if (model.descr_bool1 == "Bidang" && model.bool1 == "0")
                      {
                          updateListing(model.chkitem_instance_idx);
                      }*/
                    model.module_name = dashboard.module_name;

                }
                modelList.Add(model);


            }


            return modelList;

        }

        public string GetChecklistItem(string tobtab_id, string checklistid, string module)
        {

            var chklist = unitOfWork.CoreChklstListsRepository.Find(i => i.chklist_code.ToString() == module).FirstOrDefault();

            var chklistinstance = unitOfWork.CoreChkListInstancesRepository.Find(i => i.application_ref.ToString() == tobtab_id
                                    && i.chklist_tplt_ref.ToString() == chklist.chklist_idx.ToString()).FirstOrDefault();

            var item_ref = Guid.Parse(checklistid);

            var chklistitem = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (
                                    i.chklist_instance_ref == chklistinstance.chklist_instance_idx
                                    && i.chklist_tplt_item_ref == item_ref)).FirstOrDefault();

            var idx = "";
            if (chklistitem != null)
            {
                idx = chklistitem.chkitem_instance_idx.ToString();
            }
            return idx;
        }

        public string GetRefNo()
        {
            //MM2h/000001
            List<TourlistDataLayer.DataModel.tobtab_licenses> licenses = new List<TourlistDataLayer.DataModel.tobtab_licenses>();
            licenses = unitOfWork.TobtabLicenses.GetAll().OrderByDescending(i => i.license_ref_code).ToList();
            string sRefNo = "";
            string sNumber = "000000";
            if (licenses.Count > 0)
            {
                foreach (var license in licenses)
                {
                    string sRef = license.license_ref_code.Substring(7, 6);
                    //string sRef = "TOBTAB/000001".Substring(5, 6);

                    int iRef = int.Parse(sRef) + 1;
                    sRefNo = "TOBTAB/" + sNumber.Substring(0, (sNumber.Length - iRef.ToString().Length)) + iRef.ToString().Trim();
                }

                string sRef2 = sRefNo.Substring(7, 6);
                int iRef2 = int.Parse(sRef2) + 1;
                sRefNo = "TOBTAB/" + sNumber.Substring(0, (sNumber.Length - iRef2.ToString().Length)) + iRef2.ToString().Trim();
            }
            else
            {
                sRefNo = "TOBTAB/000001";
            }
            return sRefNo;
        }

        public string GetSSMNo(string userID)
        {
            var user = unitOfWork.CoreUsersRepository.Find(i => (i.user_idx.ToString() == userID.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            return organization.organization_identifier;
        }

        public string GetBranchLicenseNo(string licenseNo, string sOrganization)
        {
            Guid idx = Guid.Parse(sOrganization);
            var branch = unitOfWork.TobtabAddBranchesRepository.Find(i => i.organization_ref == idx).OrderByDescending(i => i.branch_license_ref_code).ToList();

            string sRefNo = "";

            if (branch != null)
            {
                //  string sRef = branch.branch_license_ref_code.Split("/").ToString();
                /*string[] sRef = branch.branch_license_ref_code.Split('/');
                int i = sRef.Length;
                string sNo = sRef[i-1].ToString();*/
                int iRef = branch.Count + 1;

                //int iRef = int.Parse(sNo) + 1;
                sRefNo = licenseNo + "/" + iRef.ToString().Trim();

            }
            else
            {
                sRefNo = licenseNo + "/1";
            }

            return sRefNo;
        }

        public bool DeleteBranch(string idx, string id, string organizationID, string module)
        {
            try
            {
                Guid Idx = Guid.Parse(idx);
                var del = unitOfWork.TobtabAddBranchesRepository.Find(c => c.tobtab_add_branches_idx == Idx).FirstOrDefault();

                if (del != null)
                {
                    unitOfWork.TobtabAddBranchesRepository.Remove(del);
                    unitOfWork.Complete(); //motacContext.SaveChanges();

                    var AddBranch = GetBranch(organizationID, module);

                    if (AddBranch.Count == 0)
                    {
                        updateListingFalse(id);
                    }


                    return true;
                }



                return true;


            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool updateListingFalse(string item)
        {
            try
            {
                Guid Idx = Guid.Parse(item);
                var upd_item = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == Idx).FirstOrDefault();
                upd_item.bool1 = 0;

                unitOfWork.CoreChkItemsInstancesRepository.TourlistContext.Entry(upd_item).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool AddBranch_SaveNew(TobtabViewModels.tobtab_branch branch)
        {
            try
            {
                var ref_type = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "STATUSAWAM").FirstOrDefault();
                var activeID = unitOfWork.RefReferencesRepository.Find(c => c.ref_name == "DRAFT" && c.ref_type == ref_type.ref_idx).FirstOrDefault();
                /*  Guid activeID = Guid.Empty;
                  if (upd_active != null)
                  {
                      activeID = upd_active.status_idx;

                  }*/
                tobtab_add_branches new_branch = new tobtab_add_branches(); //motacContext.core_persons.Create();
                Guid gBranch = Guid.NewGuid();
                new_branch.tobtab_add_branches_idx = gBranch;
                new_branch.application_stub_ref = Guid.Parse(branch.application_stub_ref.ToString());
                new_branch.branch_addr_1 = branch.branch_addr_1;
                new_branch.branch_addr_2 = branch.branch_addr_2;
                new_branch.branch_addr_3 = branch.branch_addr_3;
                new_branch.branch_email = branch.branch_email;
                new_branch.branch_fax_no = branch.branch_fax_no;
                new_branch.branch_mobile_no = branch.branch_mobile_no;
                new_branch.organization_ref = Guid.Parse(branch.OrganizationID);
                new_branch.active_status = activeID.ref_idx;
                //new_branch.branch_city = Guid.Parse(branch.branch_city);
                //new_branch.branch_state = Guid.Parse(branch.branch_state);
                new_branch.branch_city = unitOfWork.VwGeoListRepository.Find(i => i.postcode_code == branch.branch_postcode).FirstOrDefault().town_idx;
                new_branch.branch_state = unitOfWork.VwGeoListRepository.Find(i => i.postcode_code == branch.branch_postcode).FirstOrDefault().state_idx;
                new_branch.branch_postcode = branch.branch_postcode;
                new_branch.branch_phone_no = branch.branch_phone_no;
                new_branch.created_dt = DateTime.Now;
                new_branch.modified_dt = DateTime.Now;
                new_branch.created_by = Guid.Parse(branch.user_id);
                new_branch.modified_by = Guid.Parse(branch.user_id);
                new_branch.branch_website = branch.branch_website;
                new_branch.branch_license_ref_code = branch.branch_license_ref_code;
                new_branch.pbt_ref = Guid.Parse(branch.pbt);
                if (branch.tobtab_licenses_idx != null)
                {
                    new_branch.tobtab_license_ref = Guid.Parse(branch.tobtab_licenses_idx);
                }
                unitOfWork.TobtabAddBranchesRepository.Add(new_branch);
                var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateAddBranch(TobtabViewModels.tobtab_branch branch)
        {
            try
            {

                Guid Idx = Guid.Parse(branch.branch_id);
                var update_branch = unitOfWork.TobtabAddBranchesRepository.Find(c => c.tobtab_add_branches_idx == Idx).FirstOrDefault();

                //  mm2h_add_braches update_branch = new mm2h_add_braches(); //motacContext.core_persons.Create();

                update_branch.branch_addr_1 = branch.branch_addr_1;
                update_branch.branch_addr_2 = branch.branch_addr_2;
                update_branch.branch_addr_3 = branch.branch_addr_3;

                update_branch.branch_email = branch.branch_email;
                update_branch.branch_fax_no = branch.branch_fax_no;
                update_branch.branch_mobile_no = branch.branch_mobile_no;

                update_branch.branch_city = unitOfWork.VwGeoListRepository.Find(i => i.postcode_code == branch.branch_postcode).FirstOrDefault().town_idx;
                update_branch.branch_state = unitOfWork.VwGeoListRepository.Find(i => i.postcode_code == branch.branch_postcode).FirstOrDefault().state_idx;
                //update_branch.branch_city = Guid.Parse(branch.branch_city);
                //update_branch.branch_state = Guid.Parse(branch.branch_state);
                update_branch.branch_postcode = branch.branch_postcode;
                update_branch.branch_phone_no = branch.branch_phone_no;

                update_branch.modified_dt = DateTime.Now;
                update_branch.modified_by = Guid.Parse(branch.user_id);
                update_branch.branch_website = branch.branch_website;
                update_branch.application_stub_ref = Guid.Parse(branch.application_stub_ref.ToString());
                update_branch.pbt_ref = Guid.Parse(branch.pbt);
                unitOfWork.TobtabAddBranchesRepository.TourlistContext.Entry(update_branch).State = EntityState.Modified;
                unitOfWork.Complete();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public TourlistDataLayer.DataModel.core_organizations GetOrganization(string userID)
        {
            var user = unitOfWork.CoreUsersRepository.Find(i => (i.user_idx.ToString() == userID.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            return organization;
        }
        public TourlistDataLayer.DataModel.core_license GetCoreLicens(string organizationIdx)
        {
            core_sol_solutions core_Sol_Solutions = unitOfWork.CoreSolSolutionsRepository.Find(i => i.solution_name.ToString() == "TOBTAB").FirstOrDefault();
            string solSolution = core_Sol_Solutions.solutions_idx.ToString();
            var coreLicense = unitOfWork.CoreLicenseRepository.Find(i => (i.core_organization_ref.ToString() == organizationIdx.ToString() && i.core_sol_solution_ref.ToString() == solSolution)).FirstOrDefault();
            return coreLicense;
        }

        public string DeleteApplication(string appID)
        {
            var licenses = unitOfWork.TobtabLicenses.SingleOrDefault(i => (i.tobtab_idx.ToString() == appID.ToString()));
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == licenses.stub_ref.ToString())).FirstOrDefault();
            var chklist_Instances = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.application_ref.ToString() == application.apply_idx.ToString())).ToList();
            foreach (var app in chklist_Instances)
            {
                var chklistitems_Instances = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chklist_instance_ref.ToString() == app.chklist_instance_idx.ToString())).ToList();

                unitOfWork.CoreChkItemsInstancesRepository.RemoveRange(chklistitems_Instances);
            }

            unitOfWork.TobtabLicenses.Remove(licenses);
            unitOfWork.FlowApplicationStubs.Remove(application);
            unitOfWork.CoreChkListInstancesRepository.RemoveRange(chklist_Instances);
            unitOfWork.Complete();

            return "Success";
        }

        public string deleteMarketingAgent(string marketing_agent_idx)
        {
            tobtab_marketing_agents agent = unitOfWork.TobtabMarketingAgentRepository.Find(i => i.marketing_agent_idx.ToString() == marketing_agent_idx).FirstOrDefault();
            unitOfWork.TobtabMarketingAgentRepository.Remove(agent);
            unitOfWork.Complete();
            return "success";
        }
        public string deleteMarketingArea(string marketing_area_idx)
        {
            tobtab_marketing_areas area = unitOfWork.TobtabMarketingAreaRepository.Find(i => i.marketing_areas_idx.ToString() == marketing_area_idx).FirstOrDefault();
            unitOfWork.TobtabMarketingAreaRepository.Remove(area);
            unitOfWork.Complete();
            return "success";
        }
        public string deleteUmrahSchedule(string tobtab_umrah_schedule_idx)
        {
            tobtab_umrah_schedule schedule = unitOfWork.TobtabUmrahScheduleRepository.Find(i => i.tobtab_umrah_schedule_idx.ToString() == tobtab_umrah_schedule_idx).FirstOrDefault();
            unitOfWork.TobtabUmrahScheduleRepository.Remove(schedule);
            unitOfWork.Complete();
            return "success";
        }
        public string deleteUmrahLanguage(string tobtab_umrah_language_idx)
        {
            tobtab_umrah_language language = unitOfWork.TobtabUmrahLanguageRepository.Find(i => i.tobtab_umrah_language_idx.ToString() == tobtab_umrah_language_idx).FirstOrDefault();
            unitOfWork.TobtabUmrahLanguageRepository.Remove(language);
            unitOfWork.Complete();
            return "success";
        }
        public string deleteTravelingRecord(string travelingIdx)
        {
            tobtab_tg_traveling tgTravel = unitOfWork.TobtabTGTravelingRepository.Find(i => i.tobtab_tg_traveling_idx.ToString() == travelingIdx).FirstOrDefault();
            unitOfWork.TobtabTGTravelingRepository.Remove(tgTravel);
            unitOfWork.Complete();
            return "success";
        }

        public string DeleteApplicationModule(string appID, string Module)
        {
            var licenses = unitOfWork.TobtabLicenses.SingleOrDefault(i => (i.tobtab_idx.ToString() == appID.ToString()));
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == licenses.stub_ref.ToString())).FirstOrDefault();
            var chklist_Instances = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.application_ref.ToString() == application.apply_idx.ToString())).ToList();
            foreach (var app in chklist_Instances)
            {
                var chklistitems_Instances = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chklist_instance_ref.ToString() == app.chklist_instance_idx.ToString())).ToList();

                unitOfWork.CoreChkItemsInstancesRepository.RemoveRange(chklistitems_Instances);
            }

            unitOfWork.TobtabLicenses.Remove(licenses);
            unitOfWork.FlowApplicationStubs.Remove(application);
            unitOfWork.CoreChkListInstancesRepository.RemoveRange(chklist_Instances);

            unitOfWork.Complete();

            return "Success";
        }

        public static string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Guid GetGuidRefByRefTypeAndCode(string refType, string refCode)
        {
            var datalist = unitOfWork.VwRefReferencesRepository
                .Find(x => x.ref_type_name == refType && x.ref_code == refCode)
                .FirstOrDefault();

            return datalist.ref_idx;
        }

        public List<TobtabViewModels.tobtab_app> GetTobtabStatusByAppIdx(string sUserID, string moduleName, string applyIdx)
        {
            Guid statusComplete = this.GetGuidRefByRefTypeAndCode("STATUSAWAM", "COMPLETED");
            core_sol_modules solModule = unitOfWork.CoreSolModulesRepository.Find(i => i.module_name.ToString() == moduleName).FirstOrDefault();
            List<TourlistDataLayer.DataModel.flow_application_stubs> application = new List<TourlistDataLayer.DataModel.flow_application_stubs>();
            application = unitOfWork.FlowApplicationStubs.Find(i => i.apply_user.ToString() == sUserID && i.apply_module == solModule.modules_idx
            && i.apply_idx.ToString() == applyIdx).OrderByDescending(i => i.application_date).ToList();

            List<TourlistDataLayer.DataModel.tobtab_licenses> licenses = new List<TourlistDataLayer.DataModel.tobtab_licenses>();
            licenses = unitOfWork.TobtabLicenses.Find(i => i.created_by.ToString() == sUserID).ToList();

            List<TourlistDataLayer.DataModel.ref_references> references = new List<TourlistDataLayer.DataModel.ref_references>();
            references = unitOfWork.RefReferencesRepository.GetAll().ToList();

            List<TourlistDataLayer.DataModel.core_sol_modules> modules = new List<TourlistDataLayer.DataModel.core_sol_modules>();
            modules = unitOfWork.CoreModules.Find(i => (i.module_name.ToString() == moduleName)).ToList();

            var clsApp = (from app in application
                          from license in licenses

                          .Where(d => d.stub_ref.ToString() == app.apply_idx.ToString())
                          .DefaultIfEmpty() // <== makes join left join  

                          from ref1 in references
                          .Where(p => p.ref_idx.ToString() == app.apply_status.ToString())

                          from mod in modules
                          .Where(p => p.modules_idx.ToString() == app.apply_module.ToString())

                          select new
                          {
                              Module = mod.module_name,
                              Status = ref1.ref_name,
                              AppID = app.apply_idx.ToString(),

                          }).ToList();

            List<TobtabViewModels.tobtab_app> app1 = new List<TobtabViewModels.tobtab_app>();


            foreach (var app in clsApp)
            {
                app1.Add(new TobtabViewModels.tobtab_app
                {
                    Module = app.Module,
                    Status = app.Status,
                    AppID = app.AppID,
                });
            }

            return app1;


        }

        public List<TobtabViewModels.tobtab_app> GetTobtabStatus(string sUserID, string moduleName)
        {
            Guid statusComplete = this.GetGuidRefByRefTypeAndCode("STATUSAWAM", "COMPLETED");
            core_sol_modules solModule = unitOfWork.CoreSolModulesRepository.Find(i => i.module_name.ToString() == moduleName).FirstOrDefault();
            List<TourlistDataLayer.DataModel.flow_application_stubs> application = new List<TourlistDataLayer.DataModel.flow_application_stubs>();
            application = unitOfWork.FlowApplicationStubs.Find(i => i.apply_user.ToString() == sUserID && i.apply_module == solModule.modules_idx).OrderByDescending(i => i.application_date).ToList();

            List<TourlistDataLayer.DataModel.tobtab_licenses> licenses = new List<TourlistDataLayer.DataModel.tobtab_licenses>();
            licenses = unitOfWork.TobtabLicenses.Find(i => i.created_by.ToString() == sUserID).ToList();

            List<TourlistDataLayer.DataModel.ref_references> references = new List<TourlistDataLayer.DataModel.ref_references>();
            references = unitOfWork.RefReferencesRepository.GetAll().ToList();

            List<TourlistDataLayer.DataModel.core_sol_modules> modules = new List<TourlistDataLayer.DataModel.core_sol_modules>();
            modules = unitOfWork.CoreModules.Find(i => (i.module_name.ToString() == moduleName)).ToList();

            var clsApp = (from app in application
                          from license in licenses

                          .Where(d => d.stub_ref.ToString() == app.apply_idx.ToString())
                          .DefaultIfEmpty() // <== makes join left join  

                          from ref1 in references
                          .Where(p => p.ref_idx.ToString() == app.apply_status.ToString())

                          from mod in modules
                          .Where(p => p.modules_idx.ToString() == app.apply_module.ToString())

                          select new
                          {
                              Module = mod.module_name,
                              Status = ref1.ref_name,
                              AppID = app.apply_idx.ToString(),

                          }).ToList();

            List<TobtabViewModels.tobtab_app> app1 = new List<TobtabViewModels.tobtab_app>();


            foreach (var app in clsApp)
            {
                app1.Add(new TobtabViewModels.tobtab_app
                {
                    Module = app.Module,
                    Status = app.Status,
                    AppID = app.AppID,
                });
            }

            return app1;


        }

        public List<TobtabViewModels.tobtab_dashboard_list> GetDashboardList(string AppID, string Module, string status_code)
        {
            byte outbound = 0;
            //var licenses = new object();
            //List<TourlistDataLayer.DataModel.mm2h_licenses> licenses = new List<TourlistDataLayer.DataModel.mm2h_licenses>();
            List<tobtab_licenses> licenses = new List<tobtab_licenses>();
            List<TobtabViewModels.tobtab_dashboard_list> modelList = new List<TobtabViewModels.tobtab_dashboard_list>();
            licenses = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == AppID)).ToList();

            var tobtabLicense = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == AppID)).FirstOrDefault();
            if (tobtabLicense != null && tobtabLicense.outbound != null)
                outbound = (byte)tobtabLicense.outbound;

            var applications = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == AppID)).FirstOrDefault();

            var chklst_lists = unitOfWork.CoreChklstListsRepository.Find(i => (i.chklist_code.ToString() == Module)).FirstOrDefault();
            var modules = unitOfWork.CoreModules.Find(i => (i.modules_idx == applications.apply_module)).FirstOrDefault();

            var chklist_instances = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.application_ref.ToString() == AppID && i.chklist_tplt_ref == chklst_lists.chklist_idx)).FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_chkitems_instances> chkitems_instances = new List<TourlistDataLayer.DataModel.core_chkitems_instances>();
            if (chklist_instances != null)
            {
                chkitems_instances = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chklist_instance_ref == chklist_instances.chklist_instance_idx)).ToList();

                List<TourlistDataLayer.DataModel.core_chklst_items> chklst_items = new List<TourlistDataLayer.DataModel.core_chklst_items>();
                chklst_items = unitOfWork.CoreChklstItemsRepository.Find(i => (i.chklist_ref == chklst_lists.chklist_idx)).ToList();


                var clsList = (from license in licenses
                               from chkitems_instance in chkitems_instances
                              .Where(p => p.chklist_instance_ref.ToString() == chklist_instances.chklist_instance_idx.ToString())
                               from chklst_item in chklst_items
                              .Where(p => p.item_idx.ToString() == chkitems_instance.chklist_tplt_item_ref.ToString())
                               select new
                               {
                                   chkitem_instance_idx = chkitems_instance.chkitem_instance_idx.ToString(),
                                   apply_idx = applications.apply_idx.ToString(),
                                   chklist_code = chklst_lists.chklist_code.ToString(),
                                   bool1 = chkitems_instance.bool1,
                                   descr_bool1 = chklst_item.descr_bool1,
                                   descr_bool2 = chklst_item.descr_bool2,
                                   orderx = chklst_item.orderx,
                                   descr_string1 = chklst_item.descr_string1,
                                   descr_string2 = chklst_item.descr_string2,
                                   upload_location = chkitems_instance.upload_location,
                                   upload_date = chkitems_instance.date1,
                                   file_name = chkitems_instance.string1,
                                   Upload_file_name = chkitems_instance.string2,
                                   module_name = modules.module_name
                               }).OrderBy(i => i.orderx).ToList();

                foreach (var dashboard in clsList)
                {
                    TobtabViewModels.tobtab_dashboard_list model = new TobtabViewModels.tobtab_dashboard_list();
                    {
                        model.chkitem_instance_idx = dashboard.chkitem_instance_idx;
                        model.apply_idx = dashboard.apply_idx;
                        model.chklist_code = dashboard.chklist_code.ToString();
                        model.bool1 = dashboard.bool1.ToString();
                        model.descr_bool1 = dashboard.descr_bool1;
                        model.descr_bool2 = dashboard.descr_bool2;
                        model.orderx = dashboard.orderx;
                        model.descr_string1 = dashboard.descr_string1;
                        model.descr_string2 = dashboard.descr_string2;
                        model.file_name = dashboard.file_name;
                        model.upload_location = dashboard.upload_location;
                        model.supload_date = String.Format("{0:dd/MM/yyyy}", dashboard.upload_date);
                        model.Upload_file_name = dashboard.Upload_file_name;
                        model.module_name = dashboard.module_name;
                        if (model.descr_bool1 == "Dokumen Sokongan" && model.bool1 == "1" && status_code == "PREMISE_PREPARE")
                        {
                            updateListingFalse(model.chkitem_instance_idx);
                            model.bool1 = "0";
                        }

                    }
                    if (model.orderx != 6)
                    {
                        if(model.orderx == 7 && model.module_name == "TOBTAB_RENEW")
                        {
                            var tobtab_branch = this.GetBranch(tobtabLicense.organization_ref.ToString(),"");
                            if(tobtab_branch!=null && tobtab_branch.Count > 0)
                            {
                                modelList.Add(model);
                            }
                        }
                        else
                        {
                            modelList.Add(model);
                        }
                    }
                    else if (model.orderx == 6 && outbound == 1)
                    {
                        modelList.Add(model);
                    }
                    else if (model.orderx == 6 && Module.Contains("TOBTAB_TG_EXCEPTION"))
                    {
                        modelList.Add(model);
                    }
                }
                return modelList;
            }
            return modelList;
        }

        public bool updateListing(string item)
        {
            try
            {
                Guid Idx = Guid.Parse(item);
                var upd_item = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == Idx).FirstOrDefault();
                upd_item.bool1 = 1;

                unitOfWork.CoreChkItemsInstancesRepository.TourlistContext.Entry(upd_item).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public string GetCountryName(string guid)
        {
            var data = unitOfWork.RefGeoCountriesRepository.Find(c => c.country_idx.ToString() == guid).FirstOrDefault();
            return data.country_name;
        }
        public string GetStateName(Nullable<System.Guid> guid)
        {
            var data = unitOfWork.RefGeoStatesRepository.Find(c => c.state_idx == guid).FirstOrDefault();
            return data.state_name;
        }

        public List<TobtabViewModels.tobtab_tg_exception> getTgTravelingList(string tgExceptionIdx)
        {
            // Guid gUserID = Guid.Parse(sUserID);
            List<TourlistDataLayer.DataModel.tobtab_tg_traveling> clsApplication = new List<TourlistDataLayer.DataModel.tobtab_tg_traveling>();
            clsApplication = unitOfWork.TobtabTGTravelingRepository.Find(c => (c.tobtab_tg_exceptions_idx.ToString() == tgExceptionIdx)).ToList();

            List<TobtabViewModels.tobtab_tg_exception> modelList = new List<TobtabViewModels.tobtab_tg_exception>();
            foreach (var app in clsApplication)
            {
                TobtabViewModels.tobtab_tg_exception model = new TobtabViewModels.tobtab_tg_exception();
                //var person = unitOfWork.CorePersonsRepository.Find(i => (i.person_idx == app.person_ref)).First();
                var refTraveling = unitOfWork.RefReferencesRepository.Find(i => (i.ref_idx == app.travel_purpose)).First();
                string travelingPurpose = "";
                if (refTraveling != null && refTraveling.ref_code == "05")
                {
                    // travelingPurpose = refTraveling.ref_description + "("+ app.travel_purpose_others +")";
                }
                else
                {
                    travelingPurpose = refTraveling.ref_description;
                }
                model.tobtab_tg_exceptions_idx = app.tobtab_tg_exceptions_idx;
                model.tobtab_tg_traveling_idx = app.tobtab_tg_traveling_idx;
                model.travel_purpose = travelingPurpose;
                model.travel_location = app.travel_location;
                model.stated_desc = GetStateName(app.travel_state);
                model.city_desc = GetCityName(app.travel_town);
                model.depart_time = app.travel_start_time;
                model.depart_date = String.Format("{0:dd/MM/yyyy}", app.travel_start_date);
                model.return_date = app.destination_start_date == null ? "-" : String.Format("{0:dd/MM/yyyy}", app.destination_start_date);
                model.return_time = app.destination_start_time == null ? "-" : app.destination_start_time;
                model.return_two_way_date = app.two_way_destination_start_date == null ? "-" : String.Format("{0:dd/MM/yyyy}", app.two_way_destination_start_date);
                model.return_two_way_time = app.two_way_destination_start_time == null ? "-" : app.two_way_destination_start_time;
                // model.person_name = person.person_name;
                //model.nationality = GetCountryName(person.person_nationality.ToString());

                modelList.Add(model);
            }
            return modelList;

        }

        public string GetCityName(Nullable<System.Guid> guid)
        {
            var data = unitOfWork.RefGeoDistrictsRepository.Find(c => c.district_idx == guid).FirstOrDefault();
            return data.district_name;
        }
        public List<TobtabViewModels.tobtab_umrah> getUmrahAdvertiseList(string licensesIdx)
        {
            // Guid gUserID = Guid.Parse(sUserID);
            List<TourlistDataLayer.DataModel.tobtab_umrah_advertising> clsApplication = new List<TourlistDataLayer.DataModel.tobtab_umrah_advertising>();
            clsApplication = unitOfWork.TobtabUmrahAdvertisingRepository.Find(c => (c.tobtab_licenses_ref.ToString() == licensesIdx)).ToList();

            List<TobtabViewModels.tobtab_umrah> modelList = new List<TobtabViewModels.tobtab_umrah>();
            foreach (var app in clsApplication)
            {
                TobtabViewModels.tobtab_umrah model = new TobtabViewModels.tobtab_umrah();
                //var person = unitOfWork.CorePersonsRepository.Find(i => (i.person_idx == app.person_ref)).First();
                model.tobtab_umrah_advertising_idx = app.tobtab_umrah_advertising_idx;
                model.apply_date = String.Format("{0:dd/MM/yyyy}", app.created_at);
                if (app.advertise_approval_date != null)
                {
                    model.advertise_approval_date = String.Format("{0:dd/MM/yyyy}", app.advertise_approval_date);
                }
                else
                {
                    model.advertise_approval_date = "-";
                }
                if (app.advertise_expiry_date != null)
                {
                    model.advertise_expiry_date = String.Format("{0:dd/MM/yyyy}", app.advertise_expiry_date);
                }
                else
                {
                    model.advertise_expiry_date = "-";
                }
                model.advertise_title = app.advertise_title;
                String kod = app.advertise_code;
                if (kod == null)
                {
                    kod = "-";
                }
                model.advertise_code = kod;
                if (app.active_status == 1)
                {
                    model.active_status = "Aktif";
                }
                else
                {
                    model.active_status = "Tamat Tempoh";
                }
                // model.person_name = person.person_name;
                //model.nationality = GetCountryName(person.person_nationality.ToString());

                modelList.Add(model);
            }
            return modelList;

        }

        public List<TobtabViewModels.tobtab_marketing> getMarketingAgentList(string licensesIdx)
        {
            // Guid gUserID = Guid.Parse(sUserID);
            List<TourlistDataLayer.DataModel.tobtab_marketing_agents> clsApplication = new List<TourlistDataLayer.DataModel.tobtab_marketing_agents>();
            clsApplication = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.tobtab_licenses_ref.ToString() == licensesIdx)).ToList();

            List<TobtabViewModels.tobtab_marketing> modelList = new List<TobtabViewModels.tobtab_marketing>();
            foreach (var app in clsApplication)
            {
                TobtabViewModels.tobtab_marketing model = new TobtabViewModels.tobtab_marketing();

                var person = unitOfWork.CorePersonsRepository.Find(i => (i.person_idx == app.person_ref)).First();
                model.marketing_agent_idx = app.marketing_agent_idx;
                model.person_ref = person.person_idx;
                model.person_identifier = person.person_identifier;
                model.person_emel = person.contact_email;
                model.person_name = person.person_name;
                if (person.person_nationality.ToString() != "")
                {
                    model.person_nationality = GetCountryName(person.person_nationality.ToString());
                }
                else
                {
                    model.person_nationality = "-";
                }
                List<TourlistDataLayer.DataModel.tobtab_marketing_areas> areas = new List<TourlistDataLayer.DataModel.tobtab_marketing_areas>();
                areas = unitOfWork.TobtabMarketingAreaRepository.Find(c => (c.marketing_agent_idx == app.marketing_agent_idx)).ToList();
                foreach (var state in areas)
                {
                    model.state_desc += GetStateName(state.state_marketing_ref) + ", ";
                    if (model.person_name == "")
                    { //delete rubbish data
                        unitOfWork.TobtabMarketingAreaRepository.Remove(state);
                    }
                }
                if (model.state_desc != null)
                {
                    model.state_desc = model.state_desc.Substring(0, model.state_desc.Length - 2);
                }
                if (model.person_name == "")
                { //delete rubbish data
                    unitOfWork.CorePersonsRepository.Remove(person);
                    unitOfWork.TobtabMarketingAgentRepository.Remove(app);
                    unitOfWork.Complete();
                }
                else
                {
                    modelList.Add(model);
                }
            }
            return modelList;

        }
        public List<TobtabViewModels.tobtab_application> GetApplicationStatusMain(string sUserID, string module, Guid organization_ID)
        {
            Guid gUserID = Guid.Parse(sUserID);
            List<TourlistDataLayer.DataModel.vw_tobtab_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_tobtab_application>();
            if (module == null)
            {
               /* clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.status == "Deraf") && c.apply_user == gUserID).OrderByDescending(i => i.module_name).ToList();

                if (clsApplication.Count == 0)
                {*/
                    var appActive = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_user == gUserID)).OrderByDescending(i => i.application_date).FirstOrDefault();
                    if (appActive != null)
                    {
                        clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_idx == appActive.apply_idx)).ToList();
                    }
              //  }
            }
            else
            {
                //var appActive = unitOfWork.VWPPPAplicationRepository.Find(c => (c.apply_user == gUserID && c.status == "Selesai" && c.module_name == module)).OrderByDescending(i => i.application_date).FirstOrDefault();
              

                if (module == "TOBTAB_ADD_BRANCH")
                {
                    var appActive = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_user == gUserID && c.module_name == module)).OrderByDescending(i => i.application_date).ToList();
                    if (appActive != null)
                    {
                        clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.organization_ref == organization_ID && c.module_name == module)).OrderByDescending(i => i.application_date).ToList();
                    }
                }
                else
                {
                    var appActive = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_user == gUserID && c.module_name == module)).OrderByDescending(i => i.application_date).FirstOrDefault();
                    if (appActive != null)
                    {
                        clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_idx == appActive.apply_idx)).ToList();
                    }
                }



            }
            List<TobtabViewModels.tobtab_application> modelList = new List<TobtabViewModels.tobtab_application>();
            foreach (var app in clsApplication)
            {
                TobtabViewModels.tobtab_application model = new TobtabViewModels.tobtab_application();
                {
                    model.apply_idx = app.apply_idx;
                    model.tobtab_idx = app.tobtab_idx;
                    //  model.license_ref_code = app.license_no;
                    model.license_ref_code = app.license_ref_code;
                    if (app.application_no == null)
                        model.application_no = "";
                    else
                        model.application_no = app.application_no;
                    model.Application_date = String.Format("{0:dd/MM/yyyy}", app.application_date);
                    model.module_name = app.module_name;
                    model.solution_name = app.solution_name;
                    model.status = app.status;
                    model.status_code = app.ref_code.ToString();
                    model.module_desc = app.module_desc;
                }
                modelList.Add(model);
            }
            return modelList;

        }

        public bool GetCheckingBidang(string sUserID)
        {
            Guid gUserID = Guid.Parse(sUserID);
           // List<TourlistDataLayer.DataModel.vw_tobtab_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_tobtab_application>();
                      
            var appActive = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_user == gUserID && c.module_name == "TOBTAB_ADD_FIELD")).OrderByDescending(i => i.application_date).FirstOrDefault();
            bool bRefCode = false;
            
            if (appActive !=null)
            {
                var coreLicense = GetCoreLicens(appActive.organization_ref.ToString());
                var activestatus = coreLicense.active_status.ToString();
                if (activestatus == "1")
                {
                    if (appActive.ref_code == "SUBMITTED")
                        bRefCode = true;
                    else if (appActive.ref_code == "IN_PROCESS")
                        bRefCode = true;
                    else if (appActive.ref_code == "PENDING_PAY_LICENSE")
                        bRefCode = true;
                    else if (appActive.ref_code == "CANCELLED")
                        bRefCode = true;
                    else if (appActive.ref_code == "COMPLETED")
                        bRefCode = true;
                }                                      
            }
            return bRefCode;

        }

        public List<TobtabViewModels.tobtab_application> GetApplicationStatusByAppID(Guid App_ID)
        {
            List<TourlistDataLayer.DataModel.vw_tobtab_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_tobtab_application>();
                    
            clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_idx == App_ID)).ToList();
            
            List<TobtabViewModels.tobtab_application> modelList = new List<TobtabViewModels.tobtab_application>();
            foreach (var app in clsApplication)
            {
                TobtabViewModels.tobtab_application model = new TobtabViewModels.tobtab_application();
                {
                    model.apply_idx = app.apply_idx;
                    model.tobtab_idx = app.tobtab_idx;
                    //  model.license_ref_code = app.license_no;
                    model.license_ref_code = app.license_ref_code;
                    if (app.application_no == null)
                        model.application_no = "";
                    else
                        model.application_no = app.application_no;
                    model.Application_date = String.Format("{0:dd/MM/yyyy}", app.application_date);
                    model.module_name = app.module_name;
                    model.solution_name = app.solution_name;
                    model.status = app.status;
                    model.status_code = app.ref_code.ToString();
                    model.module_desc = app.module_desc;
                }
                modelList.Add(model);
            }
            return modelList;

        }
        public List<TobtabViewModels.tobtab_application> GetApplicationMain(string userID, string ApplicationStatus)
        {

            List<TobtabViewModels.tobtab_application> clsApplication = new List<TobtabViewModels.tobtab_application>();
            var modules = unitOfWork.CoreSolModulesRepository.Find(c => (c.module_name == ApplicationStatus)).OrderByDescending(i => i.module_name).FirstOrDefault();
            var solutions = unitOfWork.CoreSolSolutionsRepository.Find(c => (c.solutions_idx == modules.solution)).FirstOrDefault();

            var stubs = unitOfWork.FlowApplicationStubs.Find(c => (c.apply_module == modules.modules_idx && c.apply_user.ToString() == userID)).ToList();

            List<TobtabViewModels.tobtab_application> modelList = new List<TobtabViewModels.tobtab_application>();


            if (stubs.Count == 0)
            {
                return modelList;

            }

            foreach (var app in stubs)
            {

                TobtabViewModels.tobtab_application model = new TobtabViewModels.tobtab_application();
                var licence = unitOfWork.TobtabLicenses.Find(c => (c.stub_ref == app.apply_idx)).FirstOrDefault();
                if (licence != null)
                {

                    model.tobtab_idx = licence.tobtab_idx;
                    model.license_ref_code = licence.license_ref_code;
                    model.active_status = (byte)licence.active_status;
                    model.Application_date = String.Format("{0:dd/MM/yyyy}", app.application_date);
                    model.module_name = modules.module_name;
                    model.solution_name = solutions.solution_name;

                    modelList.Add(model);
                }


            }

            return modelList;

        }



        public List<TobtabViewModels.tobtab_application> GetApplicationStatus(string modules_name, string status)
        {

            List<TobtabViewModels.tobtab_application> clsApplication = new List<TobtabViewModels.tobtab_application>();
            var modules = unitOfWork.CoreSolModulesRepository.Find(c => (c.module_name == modules_name)).OrderByDescending(i => i.module_name).FirstOrDefault();
            var solutions = unitOfWork.CoreSolSolutionsRepository.Find(c => (c.solutions_idx == modules.solution)).FirstOrDefault();

            var stubs = unitOfWork.FlowApplicationStubs.Find(c => (c.apply_module == modules.modules_idx && c.apply_status.ToString() == status)).ToList();

            List<TobtabViewModels.tobtab_application> modelList = new List<TobtabViewModels.tobtab_application>();


            if (stubs.Count == 0)
            {
                return modelList;

            }

            foreach (var app in stubs)
            {

                var licence = unitOfWork.TobtabLicenses.Find(c => (c.stub_ref == app.apply_idx)).FirstOrDefault();
                TobtabViewModels.tobtab_application model = new TobtabViewModels.tobtab_application();
                if (licence != null)
                {

                    model.tobtab_idx = licence.tobtab_idx;
                    model.license_ref_code = licence.license_ref_code;
                    model.active_status = (byte)licence.active_status;
                    model.Application_date = String.Format("{0:dd/MM/yyyy}", app.application_date);
                    model.module_name = modules.module_name;
                    model.solution_name = solutions.solution_name;

                    modelList.Add(model);
                }
            }

            return modelList;

        }


        public List<TobtabViewModels.tobtab_marketing> getMarketingArea(string marketing_agent_idx)
        {

            List<tobtab_marketing_areas> clsApplication = new List<tobtab_marketing_areas>();
            var data = unitOfWork.TobtabMarketingAreaRepository.Find(c => (c.marketing_agent_idx.ToString() == marketing_agent_idx)).ToList();
            List<TobtabViewModels.tobtab_marketing> modelList = new List<TobtabViewModels.tobtab_marketing>();
            foreach (var app in data)
            {
                ref_geo_states state = unitOfWork.RefGeoStatesRepository.Find(i => i.state_idx == app.state_marketing_ref).FirstOrDefault();
                TobtabViewModels.tobtab_marketing model = new TobtabViewModels.tobtab_marketing();
                model.marketing_area_idx = app.marketing_areas_idx;
                model.state_desc = state.state_name.ToUpper();
                modelList.Add(model);
            }
            return modelList;

        }

        public List<TobtabViewModels.tobtab_umrah> getUmrahSchedule(string tobtab_umrah_advertising_ref)
        {

            List<tobtab_umrah_schedule> clsApplication = new List<tobtab_umrah_schedule>();
            var data = unitOfWork.TobtabUmrahScheduleRepository.Find(c => (c.tobtab_umrah_advertising_ref.ToString() == tobtab_umrah_advertising_ref)).ToList();
            List<TobtabViewModels.tobtab_umrah> modelList = new List<TobtabViewModels.tobtab_umrah>();
            foreach (var app in data)
            {
                //ref_references language = unitOfWork.RefReferencesRepository.Find(i => i.ref_idx == app.language_ref).FirstOrDefault();
                TobtabViewModels.tobtab_umrah model = new TobtabViewModels.tobtab_umrah();
                model.tobtab_umrah_schedule_idx = app.tobtab_umrah_schedule_idx;
                model.start_date = String.Format("{0:dd/MM/yyyy}", app.schedule_start_date);
                model.end_date = String.Format("{0:dd/MM/yyyy}", app.schedule_end_date);
                TimeSpan difference = (TimeSpan)(app.schedule_end_date - app.schedule_start_date);
                model.no_of_day = difference.Days.ToString();
                // int i = (app.schedule_end_date - app.schedule_start_date).to;
                modelList.Add(model);
            }
            return modelList;

        }
        public List<TobtabViewModels.tobtab_umrah> getUmrahLanguage(string tobtab_umrah_advertising_ref)
        {

            List<tobtab_umrah_language> clsApplication = new List<tobtab_umrah_language>();
            var data = unitOfWork.TobtabUmrahLanguageRepository.Find(c => (c.tobtab_umrah_advertising_ref.ToString() == tobtab_umrah_advertising_ref)).ToList();
            List<TobtabViewModels.tobtab_umrah> modelList = new List<TobtabViewModels.tobtab_umrah>();
            foreach (var app in data)
            {
                ref_references language = unitOfWork.RefReferencesRepository.Find(i => i.ref_idx == app.language_ref).FirstOrDefault();
                TobtabViewModels.tobtab_umrah model = new TobtabViewModels.tobtab_umrah();
                model.tobtab_umrah_language_idx = app.tobtab_umrah_language_idx;
                model.language_desc = language.ref_description.ToUpper();
                modelList.Add(model);
            }
            return modelList;

        }

        public List<TobtabViewModels.tobtab_overseas_partner> GetOverseasPartner(string company_id, string license_ref)
        {

            List<TobtabViewModels.tobtab_overseas_partner> clsApplication = new List<TobtabViewModels.tobtab_overseas_partner>();

            var data = unitOfWork.TobtabForeignPartnersRepository.Find(c => (c.tobtab_application_ref.ToString() == license_ref)).ToList();

            List<TobtabViewModels.tobtab_overseas_partner> modelList = new List<TobtabViewModels.tobtab_overseas_partner>();


            if (data.Count == 0)
            {
                return modelList;

            }

            foreach (var app in data)
            {
                TobtabViewModels.tobtab_overseas_partner model = new TobtabViewModels.tobtab_overseas_partner();
                var country = "";
                if (app.office_country != null)
                {
                    var country_data = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == app.office_country)).FirstOrDefault();
                    if (country_data != null)
                    {
                        country = country_data.country_name;
                    }
                }

                var address_str = app.office_addr_1 + "," + app.office_addr_2 + "," + app.office_addr_3 + "," + app.office_postcode + "," + app.office_city + "," + app.office_state + "," + country;

                model.uuid = app.foreign_partner_idx.ToString();
                model.name = app.foreign_partner_name.ToUpper().ToString();
                model.address = address_str.ToUpper().ToString();
                model.documents_path = app.document_upload_location;
                model.documents_name = app.document_upload_name;

                modelList.Add(model);

            }

            return modelList;

        }

        public Boolean deleteOverseasPartners(Guid partnerIdx)
        {
            var data = unitOfWork.TobtabForeignPartnersRepository.Find(i => i.foreign_partner_idx == partnerIdx).FirstOrDefault();
            unitOfWork.TobtabForeignPartnersRepository.Remove(data);
            unitOfWork.Complete();
            return true;
        }
        public Boolean deleteOverseasPackages(Guid packageIdx)
        {
            var data = unitOfWork.TobtabForeignPackagesRepository.Find(i => i.foreign_paackage_idx == packageIdx).FirstOrDefault();
            unitOfWork.TobtabForeignPackagesRepository.Remove(data);
            unitOfWork.Complete();
            return true;
        }

        public List<TobtabViewModels.tobtab_overseas_package> GetOverseasPackages(string company_id, string license_ref)
        {

            List<TobtabViewModels.tobtab_overseas_package> clsApplication = new List<TobtabViewModels.tobtab_overseas_package>();

            var data = unitOfWork.TobtabForeignPackagesRepository.Find(c => (c.tobtab_application_ref.ToString() == license_ref)).ToList();

            List<TobtabViewModels.tobtab_overseas_package> modelList = new List<TobtabViewModels.tobtab_overseas_package>();


            if (data.Count == 0)
            {
                return modelList;

            }

            foreach (var app in data)
            {
                TobtabViewModels.tobtab_overseas_package model = new TobtabViewModels.tobtab_overseas_package();

                model.uuid = app.foreign_paackage_idx.ToString();
                model.country = app.foreign_application_description.ToUpper().ToString();
                model.documents_path = app.document_upload_location;
                model.documents_name = app.document_upload_name;

                modelList.Add(model);

            }

            return modelList;

        }


        public List<TobtabViewModels.tobtab_shareholder> GetShareholder(string company_id, string license_ref)
        {

            List<TobtabViewModels.tobtab_application> clsApplication = new List<TobtabViewModels.tobtab_application>();

            var organization = unitOfWork.CoreOrganizationShareholders.Find(c => (c.organization_ref.ToString() == company_id)).ToList();

            List<TobtabViewModels.tobtab_shareholder> modelList = new List<TobtabViewModels.tobtab_shareholder>();


            if (organization.Count == 0)
            {
                return modelList;

            }

            foreach (var app in organization)
            {
                if (app.shareholder_person_ref != null)
                {
                    var person = unitOfWork.Persons.Find(c => (c.person_idx == app.shareholder_person_ref)).FirstOrDefault();
                    var status = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == app.status_shareholder)).FirstOrDefault();
                    TobtabViewModels.tobtab_shareholder model = new TobtabViewModels.tobtab_shareholder();

                    model.organization_shareholder_idx = app.organization_shareholder_idx;
                    model.person_name = person.person_name;
                    model.person_identifier = person.person_identifier;
                    model.license_ref_code = license_ref;
                    model.organization_name = "";
                    model.organization_identifier = "";
                    model.number_of_shares = app.number_of_shares;
                    model.status_pegangan = (status != null) ? status.ref_description : "";

                    if (model.person_identifier != null)
                    {
                        model.type = "Person";
                    }
                    else
                    {
                        model.type = "Organization";
                    }

                    modelList.Add(model);
                }

                if (app.shareholder_organization_ref != null)
                {
                    var person = unitOfWork.CoreOrganizations.Find(c => (c.organization_idx == app.shareholder_organization_ref)).FirstOrDefault();
                    var status = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == app.status_shareholder)).FirstOrDefault();
                    TobtabViewModels.tobtab_shareholder model = new TobtabViewModels.tobtab_shareholder();

                    model.organization_shareholder_idx = app.organization_shareholder_idx;
                    model.organization_name = person.organization_name;
                    model.organization_identifier = person.organization_identifier;
                    model.license_ref_code = license_ref;
                    model.person_name = "";
                    model.person_identifier = "";
                    model.number_of_shares = app.number_of_shares;
                    model.status_pegangan = (status != null) ? status.ref_description : "";
                    if (model.person_identifier != null)
                    {
                        model.type = "Person";
                    }
                    else
                    {
                        model.type = "Organization";
                    }

                    modelList.Add(model);
                }

            }

            return modelList;

        }

        public List<TobtabViewModels.tobtab_director> GetDirector(string company_id, string license_ref)
        {

            List<TobtabViewModels.tobtab_application> clsApplication = new List<TobtabViewModels.tobtab_application>();

            var organization= unitOfWork.CoreOrganizationDirectorsRepository.Find(c => c.organization_ref.ToString() == company_id && c.active_status == 1).ToList();
            if (company_id == license_ref)
            {
                organization = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => c.organization_ref.ToString() == company_id).ToList();
            }

            List<TobtabViewModels.tobtab_director> modelList = new List<TobtabViewModels.tobtab_director>();


            if (organization.Count == 0)
            {
                return modelList;

            }

            foreach (var app in organization)
            {
                if (app.person_ref != null)
                {
                    var person = unitOfWork.Persons.Find(c => (c.person_idx == app.person_ref)).FirstOrDefault();
                    TobtabViewModels.tobtab_director model = new TobtabViewModels.tobtab_director();

                    model.organization_director_idx = app.organization_director_idx;
                    model.person_name = person.person_name;
                    model.person_identifier = person.person_identifier;
                    model.license_ref_code = license_ref;
                    model.organization_name = "";
                    model.organization_identifier = "";
                    model.gender = "";
                    model.nationality = "";

                    if (person.person_gender != null)
                    {
                        var gender = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_gender)).FirstOrDefault();
                        if (gender != null)
                            model.gender = gender.ref_description;
                    }
                    if (person.person_nationality != null)
                    {
                        var nationality = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == person.person_nationality)).FirstOrDefault();
                        if (nationality != null)
                            model.nationality = nationality.country_name;
                    }
                    if (app.active_status!=null && app.active_status == 1)
                    {
                        model.active_status = "Active";
                    }
                    else
                    {
                        model.active_status = "Inactive";
                    }

                    modelList.Add(model);
                }

                if (app.organization_ref != null)
                {
                    var person = unitOfWork.CoreOrganizations.Find(c => (c.organization_idx == app.organization_ref)).FirstOrDefault();
                    TobtabViewModels.tobtab_director model = new TobtabViewModels.tobtab_director();

                    model.organization_director_idx = app.organization_director_idx;
                    model.organization_name = person.organization_name;
                    model.organization_identifier = person.organization_identifier;
                    model.license_ref_code = license_ref;
                    model.person_name = "";
                    model.person_identifier = "";
                    model.gender = "";
                    model.nationality = "";
                    if (app.active_status != null && app.active_status == 1)
                    {
                        model.active_status = "Active";
                    }
                    else
                    {
                        model.active_status = "Not Active";
                    }

                    modelList.Add(model);
                }

            }

            return modelList;

        }
        public List<TobtabViewModels.tobtab_overseas_package> GetOverseasPackageInfo(string shareholder_id)
        {

            var data = unitOfWork.TobtabForeignPackagesRepository.Find(c => (c.foreign_paackage_idx.ToString() == shareholder_id)).FirstOrDefault();

            List<TobtabViewModels.tobtab_overseas_package> modelList = new List<TobtabViewModels.tobtab_overseas_package>();

            TobtabViewModels.tobtab_overseas_package model = new TobtabViewModels.tobtab_overseas_package();

            var country = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_name == data.foreign_application_description)).FirstOrDefault();

            model.uuid = data.foreign_paackage_idx.ToString();
            model.country = data.foreign_application_description;
            model.country_uuid = country.country_idx.ToString();

            model.documents_path = data.document_upload_location;

            modelList.Add(model);

            return modelList;

        }

        public List<TobtabViewModels.tobtab_overseas_partner_modal> GetOverseasPartnerInfo(string shareholder_id)
        {

            var data = unitOfWork.TobtabForeignPartnersRepository.Find(c => (c.foreign_partner_idx.ToString() == shareholder_id)).FirstOrDefault();

            List<TobtabViewModels.tobtab_overseas_partner_modal> modelList = new List<TobtabViewModels.tobtab_overseas_partner_modal>();

            TobtabViewModels.tobtab_overseas_partner_modal model = new TobtabViewModels.tobtab_overseas_partner_modal();

            var country = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == data.office_country)).FirstOrDefault();

            model.uuid = data.foreign_partner_idx.ToString();
            model.name = data.foreign_partner_name;
            model.office_addr_1 = data.office_addr_1;
            model.office_addr_2 = data.office_addr_2;
            model.office_addr_3 = data.office_addr_3;
            model.office_postcode = data.office_postcode;
            model.office_city = data.office_city;
            model.office_state = data.office_state;
            model.office_country = data.office_country.ToString();
            model.office_phone_no = data.office_phone_no;
            model.documents_path = data.document_upload_location;

            modelList.Add(model);

            return modelList;

        }

        public List<vw_core_org_shareholder> GetShareHolderDetail(string identifier)
        {
            return unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_identifier.ToString().Contains(identifier) || x.person_identifier == identifier)
               .ToList();

        }

        public List<TobtabViewModels.tobtab_marketing> ajaxGetMarketingAgents(string marketing_agent_idx)
        {

            var agent = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.marketing_agent_idx.ToString() == marketing_agent_idx)).FirstOrDefault();
            List<TobtabViewModels.tobtab_marketing> modelList = new List<TobtabViewModels.tobtab_marketing>();
            if (agent != null && agent.person_ref != null)
            {
                var person = unitOfWork.Persons.Find(c => (c.person_idx == agent.person_ref)).FirstOrDefault();
                TobtabViewModels.tobtab_marketing model = new TobtabViewModels.tobtab_marketing();

                var states = unitOfWork.RefGeoStatesRepository.Find(c => (c.state_idx == person.residential_state)).FirstOrDefault();
                var nationality = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == person.person_nationality)).FirstOrDefault();
                var gender = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_gender)).FirstOrDefault();
                var religion = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_religion)).FirstOrDefault();
                var city = unitOfWork.RefGeoTownsRepository.Find(c => (c.town_idx == person.residential_city)).FirstOrDefault();
                //var file = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx)).FirstOrDefault();

                model.person_name = person.person_name;
                model.person_identifier = person.person_identifier;

                if (person.person_birthday != null)
                {
                    model.person_birthday = String.Format("{0:yyyy-MM-dd}", person.person_birthday);
                }

                model.person_addr_1 = person.residential_addr_1;
                model.person_addr_2 = person.residential_addr_2;
                model.person_addr_3 = person.residential_addr_3;
                model.person_postcode = person.residential_postcode;
                model.person_emel = person.residential_email;
                model.person_state = (states != null) ? states.state_name : "";
                model.person_age = (person.person_age != null) ? person.person_age.ToString() : "";
                model.person_gender = (gender != null) ? gender.ref_idx.ToString() : "";
                model.person_religion = (religion != null) ? religion.ref_idx.ToString() : "";
                model.person_nationality = (person.person_nationality != null) ? person.person_nationality.ToString() : "d570ac21-073f-43cc-bdc9-1614cfdb136e";
                model.person_phone = (person.personal_mobile_no != null) ? person.personal_mobile_no : "";
                model.person_city = (city != null) ? city.town_idx.ToString() : "";
                model.is_bumiputera = (person.person_is_bumiputera != null) ? person.person_is_bumiputera.ToString() : "";
                model.is_employer = (person.person_is_employed != null) ? person.person_is_employed.ToString() : "";

                if (person.office_city != null)
                {
                    city = unitOfWork.RefGeoTownsRepository.Find(c => (c.town_idx == person.office_city)).FirstOrDefault();
                    model.employer_city = (city != null) ? city.town_idx.ToString() : "";
                }
                if (person.office_state != null)
                {
                    states = unitOfWork.RefGeoStatesRepository.Find(c => (c.state_idx == person.office_state)).FirstOrDefault();
                    model.employer_state = (states != null) ? states.state_name : "";
                }
                model.employer_name = person.office_name;
                model.employer_addr_1 = person.office_addr_1;
                model.employer_addr_2 = person.office_addr_2;
                model.employer_addr_3 = person.office_addr_3;
                model.employer_postcode = person.office_postcode;
                model.employer_position = person.person_employ_position;
                model.employer_phone = person.office_phone_no;
                model.employer_emel = person.office_email;
                if (agent.person_id_upload_ref != null)
                {
                    model.person_id_upload_file_location = agent.person_id_upload_ref.ToString();
                    model.person_id_upload_file_name = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.uploads_freeform_by_persons_idx == agent.person_id_upload_ref).FirstOrDefault().upload_name;
                }
                if (agent.person_bankrupt_clearance_upload_ref != null)
                {
                    model.person_bankrupt_clearance_upload_file_location = agent.person_bankrupt_clearance_upload_ref.ToString();
                    model.person_bankrupt_clearance_upload_file_name = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.uploads_freeform_by_persons_idx == agent.person_bankrupt_clearance_upload_ref).FirstOrDefault().upload_name;
                }
                if (agent.person_offer_letter_ref != null)
                {
                    model.person_offer_letter_file_location = agent.person_offer_letter_ref.ToString();
                    model.person_offer_letter_file_name = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.uploads_freeform_by_persons_idx == agent.person_offer_letter_ref).FirstOrDefault().upload_name;
                }
                if (agent.employer_permission_upload_ref != null)
                {
                    model.employer_permission_upload_file_location = agent.employer_permission_upload_ref.ToString();
                    model.employer_permission_upload_file_name = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.uploads_freeform_by_persons_idx == agent.employer_permission_upload_ref).FirstOrDefault().upload_name;
                }

                modelList.Add(model);
            }
            return modelList;

        }

        public List<TobtabViewModels.tobtab_shareholder_data> GetShareholderInfo(string shareholder_id)
        {

            var shareholder = unitOfWork.CoreOrganizationShareholders.Find(c => (c.organization_shareholder_idx.ToString() == shareholder_id)).FirstOrDefault();

            List<TobtabViewModels.tobtab_shareholder_data> modelList = new List<TobtabViewModels.tobtab_shareholder_data>();


            if (shareholder.shareholder_person_ref != null)
            {
                var person = unitOfWork.Persons.Find(c => (c.person_idx == shareholder.shareholder_person_ref)).FirstOrDefault();
                TobtabViewModels.tobtab_shareholder_data model = new TobtabViewModels.tobtab_shareholder_data();

                var states = unitOfWork.RefGeoStatesRepository.Find(c => (c.state_idx == person.residential_state)).FirstOrDefault();
                var nationality = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == person.person_nationality)).FirstOrDefault();
                var gender = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_gender)).FirstOrDefault();
                var religion = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_religion)).FirstOrDefault();
                var city = unitOfWork.RefGeoTownsRepository.Find(c => (c.town_idx == person.residential_city)).FirstOrDefault();
                var file = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx)).FirstOrDefault();

                model.organization_shareholder_idx = shareholder.organization_shareholder_idx;
                model.shareholder_fullname = person.person_name;
                model.shareholder_identifier = person.person_identifier;

                if (person.person_birthday != null)
                {
                    DateTime birthdate = (DateTime)person.person_birthday;
                    model.shareholder_birthday = birthdate.ToString("yyyy-MM-dd");
                }

                model.shareholder_organization_idx = person.person_idx;
                model.shareholder_addr_1 = person.residential_addr_1;
                model.shareholder_addr_2 = person.residential_addr_2;
                model.shareholder_addr_3 = person.residential_addr_3;
                model.shareholder_postcode = person.residential_postcode;
                model.shareholder_states = (states != null) ? states.state_name : "";
                model.shareholder_age = (person.person_birthday != null) ? (DateTime.Now.Year - person.person_birthday.Value.Year).ToString() : "";
                model.shareholder_gender = (gender != null) ? gender.ref_idx.ToString() : "";
                model.shareholder_religion = (religion != null) ? religion.ref_idx.ToString() : "";
                model.shareholder_nationality = (nationality != null) ? nationality.country_name : "";
                model.shareholder_mobile_no = (person.personal_mobile_no != null) ? person.personal_mobile_no : "";
                model.shareholder_city = (city != null) ? city.town_idx.ToString() : "";
                model.number_of_shares = shareholder.number_of_shares;
                model.shareholder_isbumi = (person.person_is_bumiputera != null) ? person.person_is_bumiputera.ToString() : "";
                model.status_shareholder = shareholder.status_shareholder.ToString();
                model.active_status = shareholder.active_status;
                model.shareholder_filepath = (file != null) ? file.upload_path : "";
                model.status_type = "Person";

                modelList.Add(model);
            }

            if (shareholder.shareholder_organization_ref != null)
            {
                var organization = unitOfWork.CoreOrganizations.Find(c => (c.organization_idx == shareholder.shareholder_organization_ref)).FirstOrDefault();
                TobtabViewModels.tobtab_shareholder_data model = new TobtabViewModels.tobtab_shareholder_data();

                var states = unitOfWork.RefGeoStatesRepository.Find(c => (c.state_idx == organization.registered_state)).FirstOrDefault();
                var nationality = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == organization.country_ref)).FirstOrDefault();
                //var religion = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.re)).FirstOrDefault();
                var city = unitOfWork.RefGeoTownsRepository.Find(c => (c.town_idx == organization.registered_city)).FirstOrDefault();
                //var file = unitOfWork.CorePersonFreeUploadsRepository.Find(c => (c.person_idx == person.organization_idx)).FirstOrDefault();

                model.organization_shareholder_idx = shareholder.organization_shareholder_idx;
                model.shareholder_fullname = organization.organization_name;
                model.shareholder_identifier = organization.organization_identifier;
                model.shareholder_organization_idx = organization.organization_idx;

                //if (person.da != null)
                //{
                //    DateTime birthdate = (DateTime)person.person_birthday;
                //    model.shareholder_birthday = birthdate.ToString("yyyy-MM-dd");
                //}
                model.registered_year = shareholder.registered_year;
                model.shareholder_addr_1 = organization.office_addr_1;
                model.shareholder_addr_2 = organization.office_addr_2;
                model.shareholder_addr_3 = organization.office_addr_3;
                model.shareholder_postcode = organization.office_postcode;
                model.shareholder_states = (states != null) ? states.state_code : "";
                model.shareholder_country = organization.country_ref;
                //model.shareholder_age = (person.person_age != null) ? person.person_age.ToString() : "";
                // model.shareholder_gender = (gender != null) ? gender.ref_idx.ToString() : "";
                //model.shareholder_religion = (religion != null) ? religion.ref_idx.ToString() : "";
                model.shareholder_nationality = organization.country_ref.ToString();
                model.shareholder_mobile_no = (organization.office_mobile_no != null) ? organization.office_mobile_no : "";
                //model.shareholder_city = (city != null) ? city.office_mobile_no.ToString() : "";
                model.number_of_shares = shareholder.number_of_shares;
                //model.shareholder_isbumi = (person.person_is_bumiputera != null) ? person.person_is_bumiputera.ToString() : "";
                model.status_shareholder = shareholder.status_shareholder.ToString();
                //model.shareholder_filepath = (file != null) ? file.upload_path : "";
                model.active_status = shareholder.active_status;
                model.status_type = "Organization";
                modelList.Add(model);
            }


            return modelList;

        }
        public List<TobtabViewModels.tobtab_director_data> getDirectorInfoByPerson(string personIdx)
        {

            var shareholder = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.person_ref.ToString() == personIdx)).FirstOrDefault();

            List<TobtabViewModels.tobtab_director_data> modelList = new List<TobtabViewModels.tobtab_director_data>();
            if (personIdx != "")
            {

                if (shareholder != null && shareholder.person_ref != null)
                {
                    var person = unitOfWork.Persons.Find(c => (c.person_idx == shareholder.person_ref)).FirstOrDefault();
                    TobtabViewModels.tobtab_director_data model = new TobtabViewModels.tobtab_director_data();

                    var states = unitOfWork.RefGeoStatesRepository.Find(c => (c.state_idx == person.residential_state)).FirstOrDefault();
                    var nationality = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == person.person_nationality)).FirstOrDefault();
                    var gender = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_gender)).FirstOrDefault();
                    var religion = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_religion)).FirstOrDefault();
                    var city = unitOfWork.RefGeoTownsRepository.Find(c => (c.town_idx == person.residential_city)).FirstOrDefault();
                    var file = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx)).FirstOrDefault();

                    model.organization_director_idx = shareholder.organization_director_idx;
                    model.director_fullname = person.person_name;
                    model.director_identifier = person.person_identifier;

                    if (person.person_birthday != null)
                    {
                        DateTime birthdate = (DateTime)person.person_birthday;
                        model.director_birthday = birthdate.ToString("yyyy-MM-dd");
                    }

                    model.director_addr_1 = person.residential_addr_1;
                    model.director_addr_2 = person.residential_addr_2;
                    model.director_addr_3 = person.residential_addr_3;
                    model.director_postcode = person.residential_postcode;
                    model.director_states = (states != null) ? states.state_name : "";
                    model.director_age = (person.person_age != null) ? person.person_age.ToString() : "";
                    model.director_gender = (gender != null) ? gender.ref_idx.ToString() : "";
                    model.director_religion = (religion != null) ? religion.ref_idx.ToString() : "";
                    model.director_nationality = (nationality != null) ? nationality.country_name : "";
                    model.director_phone_no = (person.residential_phone_no != null) ? person.residential_phone_no : "";
                    model.director_mobile_no = (person.personal_mobile_no != null) ? person.personal_mobile_no : "";
                    model.director_city = (city != null) ? city.town_idx.ToString() : "";
                    model.director_isbumi = (person.person_is_bumiputera != null) ? person.person_is_bumiputera.ToString() : "";
                    model.director_filepath = (file != null) ? file.upload_path : "";

                    modelList.Add(model);
                }
            }
            return modelList;

        }

        public List<TobtabViewModels.tobtab_director_data> GetDirectorInfo(string person_id)
        {

            //var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.organization_director_idx.ToString() == director_id)).FirstOrDefault();
            var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.person_ref.ToString() == person_id || c.organization_director_idx.ToString() == person_id)).FirstOrDefault();

            List<TobtabViewModels.tobtab_director_data> modelList = new List<TobtabViewModels.tobtab_director_data>();
            if (person_id != "")
            {
                if (director.person_ref != null)
                {
                    var person = unitOfWork.Persons.Find(c => (c.person_idx == director.person_ref)).FirstOrDefault();
                    var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD").FirstOrDefault();
                    var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT").FirstOrDefault();
                    var pasPengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN").FirstOrDefault();
                    //List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
                    core_uploads_freeform_by_persons clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx && (c.upload_type_ref == mykad.ref_idx || c.upload_type_ref == passport.ref_idx)).FirstOrDefault();
                    //List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
                    core_uploads_freeform_by_persons clsUploadPasPengajian = new core_uploads_freeform_by_persons();
                    if (pasPengajian != null)
                    {
                        clsUploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx && c.upload_type_ref == pasPengajian.ref_idx).FirstOrDefault();
                    }

                    TobtabViewModels.tobtab_director_data model = new TobtabViewModels.tobtab_director_data();

                    var states = unitOfWork.RefGeoStatesRepository.Find(c => (c.state_idx == person.residential_state)).FirstOrDefault();
                    var nationality = unitOfWork.RefGeoCountriesRepository.Find(c => (c.country_idx == person.person_nationality)).FirstOrDefault();
                    var gender = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_gender)).FirstOrDefault();
                    var religion = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == person.person_religion)).FirstOrDefault();
                    var city = unitOfWork.RefGeoTownsRepository.Find(c => (c.town_idx == person.residential_city)).FirstOrDefault();
                    //var file = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx)).FirstOrDefault();

                    model.organization_director_idx = director.organization_director_idx;
                    model.director_fullname = person.person_name;
                    model.director_identifier = person.person_identifier;

                    if (person.person_birthday != null)
                    {
                        DateTime birthdate = (DateTime)person.person_birthday;
                        model.director_birthday = birthdate.ToString("yyyy-MM-dd");
                    }

                    if (director.active_status != null && director.active_status == 1)
                    {
                        model.active_status = 1;
                    }
                    else
                    {
                        model.active_status = 0;
                    }
                    model.director_addr_1 = person.residential_addr_1;
                    model.director_addr_2 = person.residential_addr_2;
                    model.director_addr_3 = person.residential_addr_3;
                    model.director_postcode = person.residential_postcode;
                    model.director_states = (states != null) ? states.state_name : "";
                    model.director_age = (person.person_birthday != null) ? (DateTime.Now.Year - person.person_birthday.Value.Year).ToString() : "";
                    model.director_gender = (gender != null) ? gender.ref_idx.ToString() : "";
                    model.director_religion = (religion != null) ? religion.ref_idx.ToString() : "";
                    model.director_nationality = (nationality != null) ? nationality.country_name : "";
                    model.director_nationality_idx = (person.person_nationality != null) ? person.person_nationality.ToString() : "";
                    model.director_phone_no = (person.residential_phone_no != null) ? person.residential_phone_no : (person.contact_phone_no != null) ? person.contact_phone_no : "";
                    model.director_mobile_no = (person.personal_mobile_no != null) ? person.personal_mobile_no : (person.contact_mobile_no != null) ? person.contact_mobile_no : "";
                    model.director_city = (city != null) ? city.town_idx.ToString() : "";
                    model.director_isbumi = (person.person_is_bumiputera != null) ? person.person_is_bumiputera.ToString() : "";
                    model.director_filepath = (clsUploadPerson != null) ? clsUploadPerson.upload_path : "";
                    model.director_filepath_pas_pengajian = (clsUploadPasPengajian != null) ? clsUploadPasPengajian.upload_path : "";

                    modelList.Add(model);
                }

            }
            return modelList;

        }

        public string UpdateChecklistStatus(string module_id, string component_id, short status)
        {

            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            chklistitems.bool1 = status;
            unitOfWork.Complete();
            return "success";
        }

        public string UpdateAddField(string tobtab_idx, string inbound, string outbound, string ticketing, string umrah, Guid core_Users)
        {
            var solSolution = unitOfWork.CoreSolSolutionsRepository.Find(i => i.solution_name == "TOBTAB").FirstOrDefault();
            var coreUser= unitOfWork.CoreUsersRepository.Find(i => i.user_idx == core_Users).FirstOrDefault();
            var license = unitOfWork.TobtabLicenses.Find(i => (i.tobtab_idx.ToString() == tobtab_idx)).FirstOrDefault();
            var coreLicense = unitOfWork.CoreLicenseRepository.Find(i => i.core_organization_ref == coreUser.user_organization && i.core_sol_solution_ref == solSolution.solutions_idx).FirstOrDefault();

            if (inbound == "1" && coreLicense.inbound==1)
            {
                inbound = "0";
            }
            if (outbound == "1" && coreLicense.outbound == 1)
            {
                outbound = "0";
            }
            if (ticketing == "1" && coreLicense.ticketing == 1)
            {
                ticketing = "0";
            }

            license.inbound = (byte?)((inbound == "1") ? 1 : 0);
            license.outbound = (byte?)((outbound == "1") ? 1 : 0);
            license.ticketing = (byte?)((ticketing == "1") ? 1 : 0);
            license.umrah = (byte?)((umrah == "1") ? 1 : 0);

            if (umrah == "1")
            {
                insertSupplortingDocumentUmrah(license.stub_ref, core_Users);
            }
            else
            {
                //checking if umrah exist
                checkAndDeleteDocumentUmrah(license.stub_ref);
            }

            unitOfWork.Complete();
            return "success";
        }
        public void checkAndDeleteDocumentUmrah(Guid applyIdx)
        {
            #region Delete Supporting Docs
            core_chklist_instances core_Chklist_Instances = new core_chklist_instances();
            core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
            core_chklist_instances core_Chklist_Instances_SuppDocs = new core_chklist_instances();
            core_chkitems_instances core_Chkitems_Instances_SuppDocs = new core_chkitems_instances();
            core_chklst_lists core_Chklst_Lists_SuppDocs = null;
            List<core_chklst_items> core_Chklst_Items_SuppDocs = null;
            var _moduleSuppDocsList = this.GetModuleSupportingDocsList();
            core_Chklst_Lists_SuppDocs = _moduleSuppDocsList.core_chklst_lists;
            core_Chklst_Items_SuppDocs = _moduleSuppDocsList.core_Chklst_Items;

            if (core_Chklst_Lists_SuppDocs != null && core_Chklst_Lists_SuppDocs.chklist_idx != null)
            {
                var chklst_Instance = unitOfWork.CoreChkListInstancesRepository.Find(i => i.application_ref == applyIdx && i.chklist_tplt_ref == core_Chklst_Lists_SuppDocs.chklist_idx).FirstOrDefault();
                if (chklst_Instance != null)
                {
                    List<core_chkitems_instances> chkItemList = unitOfWork.CoreChkItemsInstancesRepository.Find(i => i.chklist_instance_ref == chklst_Instance.chklist_instance_idx).ToList();
                    foreach (core_chkitems_instances item in chkItemList)
                    {
                        unitOfWork.CoreChkItemsInstancesRepository.Remove(item);
                    }
                    unitOfWork.CoreChkListInstancesRepository.Remove(chklst_Instance);
                }
                //unitOfWork.Complete();
            }
            #endregion
        }

        public void insertSupplortingDocumentUmrah(Guid applyIdx, Guid core_Users)
        {
            #region Supporting Docs
            core_chklist_instances core_Chklist_Instances = new core_chklist_instances();
            core_chkitems_instances core_Chkitems_Instances = new core_chkitems_instances();
            core_chklist_instances core_Chklist_Instances_SuppDocs = new core_chklist_instances();
            core_chkitems_instances core_Chkitems_Instances_SuppDocs = new core_chkitems_instances();
            core_chklst_lists core_Chklst_Lists_SuppDocs = null;
            List<core_chklst_items> core_Chklst_Items_SuppDocs = null;
            var _moduleSuppDocsList = this.GetModuleSupportingDocsList();
            core_Chklst_Lists_SuppDocs = _moduleSuppDocsList.core_chklst_lists;
            core_Chklst_Items_SuppDocs = _moduleSuppDocsList.core_Chklst_Items;

            if (core_Chklst_Lists_SuppDocs.chklist_idx != Guid.Empty)
            {
                //check duplicate applyIdx & core_Chklst_Lists_SuppDocs.chklist_idx
                var duplicate = unitOfWork.CoreChkListInstancesRepository.Find(i => i.chklist_tplt_ref == core_Chklst_Lists_SuppDocs.chklist_idx && i.application_ref == applyIdx).ToList();
                if (duplicate.Count == 0)
                {
                    core_Chklist_Instances = new core_chklist_instances();
                    core_Chklist_Instances.chklist_instance_idx = Guid.NewGuid();
                    core_Chklist_Instances.chklist_tplt_ref = core_Chklst_Lists_SuppDocs.chklist_idx;
                    //TODO: need to auto generate application no
                    core_Chklist_Instances.application_ref = applyIdx;
                    //Guid.NewGuid();
                    core_Chklist_Instances.active_status = 1;
                    core_Chklist_Instances.created_at = DateTime.Now;
                    core_Chklist_Instances.modified_at = DateTime.Now;
                    core_Chklist_Instances.created_by = core_Users;
                    core_Chklist_Instances.modified_by = core_Users;
                    unitOfWork.CoreChkListInstancesRepository.Add(core_Chklist_Instances);

                    foreach (core_chklst_items item in core_Chklst_Items_SuppDocs)
                    {
                        core_Chkitems_Instances = new core_chkitems_instances();
                        core_Chkitems_Instances.chkitem_instance_idx = Guid.NewGuid();
                        core_Chkitems_Instances.chklist_tplt_item_ref = item.item_idx;
                        core_Chkitems_Instances.chklist_instance_ref = core_Chklist_Instances.chklist_instance_idx;
                        core_Chkitems_Instances.bool1 = 0;
                        core_Chkitems_Instances.active_status = 1;
                        core_Chkitems_Instances.created_at = DateTime.Now;
                        core_Chkitems_Instances.modified_at = DateTime.Now;
                        core_Chkitems_Instances.created_by = core_Users;
                        core_Chkitems_Instances.modified_by = core_Users;
                        unitOfWork.CoreChkItemsInstancesRepository.Add(core_Chkitems_Instances);
                    }
                }
                //unitOfWork.Complete();
            }

            #endregion
        }

        private (core_chklst_lists core_chklst_lists, List<core_chklst_items> core_Chklst_Items)
        GetModuleSupportingDocsList()
        {
            core_chklst_lists core_chklst_lists = new core_chklst_lists();
            List<core_chklst_items> core_Chklst_Items = new List<core_chklst_items>();
            try
            {
                core_chklst_lists = unitOfWork.CoreChklstListsRepository.Find(x => x.chklist_code == TourlistEnums.ModuleSupportingDocs.TOBTAB_ADD_FIELD_DOCS.ToString()).FirstOrDefault();
                core_Chklst_Items = unitOfWork.CoreChklstItemsRepository.Find(x => x.chklist_ref == core_chklst_lists.chklist_idx && x.active_status == 1).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }

            return (core_chklst_lists, core_Chklst_Items);
        }

        public vw_tobtab_application GetVwTobtabApplication(string tobtabIdx)
        {
            vw_tobtab_application clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.tobtab_idx.ToString() == tobtabIdx.ToString())).FirstOrDefault();
            return clsApplication;
        }

        public vw_tobtab_application GetVwTobtabApplicationModule(string tobtabIdx, string module)
        {
            vw_tobtab_application clsApplication = new vw_tobtab_application();

            var modules = unitOfWork.CoreModules.Find(i => (i.module_name.ToString() == module)).FirstOrDefault();
            var solutions = unitOfWork.CoreSolSolutionsRepository.Find(c => (c.solutions_idx == modules.solution)).FirstOrDefault();

            var organisation = new core_organizations();
            Guid tb_idx = new Guid();
            var stub = new flow_application_stubs();
            var status = unitOfWork.RefReferencesRepository.Find(c => (c.ref_idx == stub.apply_status)).FirstOrDefault();

            clsApplication.module_name = modules.module_name;
            clsApplication.solution_name = solutions.solution_name;
            clsApplication.apply_user = stub.apply_user;
            clsApplication.status = status.ref_description;
            clsApplication.apply_idx = stub.apply_idx;
            clsApplication.modules_idx = modules.modules_idx;
            clsApplication.module_desc = modules.module_desc;
            clsApplication.organization_name = organisation.organization_name;
            clsApplication.office_email = organisation.office_email;
            clsApplication.registered_email = organisation.registered_email;
            var actual_begin_date = stub.actual_begin_date;
            var actual_end_date = stub.actual_end_date;
            clsApplication.actual_begin_date = actual_begin_date;
            clsApplication.actual_end_date = actual_end_date;

            return clsApplication;
        }

        public bool UpdatePermohonanInprocess(string license_id)
        {
            var refType = unitOfWork.RefReferencesTypesRepository.GetReferenceTypeByName("STATUSAWAM");
            var new_status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "IN_PROCESS");
            // unitOfWork.FlowApplicationStubs.UpdateApplyStatus(stub_ref, new_status.ref_idx);
            var license = unitOfWork.TobtabLicenses.Find(i => (i.tobtab_idx.ToString() == license_id)).FirstOrDefault();
            if (license != null)
            {

                var stubs = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx == license.stub_ref)).First();
                stubs.apply_status = new_status.ref_idx;
                license.active_status = 1;
                unitOfWork.Complete();
            }
            return true;
        }

        public tobtab_licenses getTobtabLicense(string license_id)
        {
            return unitOfWork.TobtabLicenses.Find(i => (i.tobtab_idx.ToString() == license_id)).FirstOrDefault();
        }

        public string UpdateApplicationStatus(string license_id, byte status, Guid stub_status, string module, Guid userid)
        {
            var refType = unitOfWork.RefReferencesTypesRepository.GetReferenceTypeByName("STATUSAWAM");
            var draft_status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "DRAFT");
            if (module == "TOBTAB_UMRAH" || module == "TOBTAB_CHANGE_STATUS" || module == "TOBTAB_RETURN_LICENSE")
            {
                var new_status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "IN_PROCESS");
                stub_status = new_status.ref_idx;
            }
            var license = unitOfWork.TobtabLicenses.Find(i => (i.tobtab_idx.ToString() == license_id)).FirstOrDefault();
            if (license != null)
            {
                var stubs = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx == license.stub_ref)).First();
                if (stubs.apply_status == draft_status.ref_idx)
                {
                    stubs.apply_status = stub_status;
                    license.active_status = status;
                }

                #region Audit Trail Application
                common_audit_trail_application commonAuditTrailApp = new common_audit_trail_application();
                commonAuditTrailApp.audit_trail_apps_idx = Guid.NewGuid();
                commonAuditTrailApp.stub_ref = stubs.apply_idx;
                commonAuditTrailApp.module_ref = stubs.apply_module;
                commonAuditTrailApp.status_ref = stubs.apply_status;
                commonAuditTrailApp.active_status = 1;
                commonAuditTrailApp.created_at = DateTime.Now;
                commonAuditTrailApp.created_by = userid;
                unitOfWork.CommonAuditTrailApplicationRepository.Add(commonAuditTrailApp);
                #endregion

                unitOfWork.Complete();
                return "success";
            }


            return "failed";
        }

        public string CheckApplicationStatus(string license_id)
        {

            var license = unitOfWork.TobtabLicenses.Find(i => (i.tobtab_idx.ToString() == license_id)).FirstOrDefault();
            if (license != null)
            {
                if (license.active_status == 2)
                {
                    return "complete";
                }
                else
                {
                    return "incomplete";
                }
            }
            return "incomplete";

        }

        public List<TobtabViewModels.tobtab_dashboard_list> GetChecklistMain(string id)
        {

            List<TobtabViewModels.tobtab_dashboard_list> clsDashboard = new List<TobtabViewModels.tobtab_dashboard_list>();

            var chklist_Instances = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.chklist_instance_idx.ToString() == id)).FirstOrDefault();
            var chklistitems_Instances = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chklist_instance_ref.ToString() == id)).ToList();

            var chklist = unitOfWork.CoreChklstListsRepository.Find(i => (i.chklist_idx.ToString() == chklist_Instances.chklist_tplt_ref.ToString())).FirstOrDefault();
            var chklistitems = unitOfWork.CoreChklstItemsRepository.Find(i => (i.chklist_ref.ToString() == chklist.chklist_idx.ToString())).ToList();


            var counter = 0;
            foreach (var items in chklistitems_Instances)
            {
                clsDashboard.Add(new TobtabViewModels.tobtab_dashboard_list
                {
                    apply_idx = chklist_Instances.application_ref.ToString(),
                    chkitem_instance_idx = items.chkitem_instance_idx.ToString(),
                    chklist_instance_ref = items.chklist_instance_ref,
                    chklist_code = chklist.chklist_code,
                    bool1 = items.bool1.ToString(),
                    descr_bool1 = chklistitems[counter].descr_bool1,
                    orderx = chklistitems[counter].orderx,
                    descr_string1 = chklistitems[counter].descr_string1,
                    descr_string2 = chklistitems[counter].descr_string2,
                    descr_string3 = chklistitems[counter].descr_string3,

                });
                counter++;

            }


            return clsDashboard;

        }

        public List<TobtabViewModels.tobtab_core_organizations> GetCompany(string RegistrationNo)
        {
            List<TourlistDataLayer.DataModel.core_organizations> clsCompany = new List<TourlistDataLayer.DataModel.core_organizations>();

            var user = unitOfWork.CoreUsersRepository.Find(i => (i.user_idx.ToString() == RegistrationNo.ToString())).FirstOrDefault();
            clsCompany = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).ToList();
            //clsCompany = unitOfWork.CoreOrganizations.Find(c => (c.organization_identifier == RegistrationNo)).ToList();

            TobtabViewModels.tobtab_core_organizations model = new TobtabViewModels.tobtab_core_organizations();
            List<TobtabViewModels.tobtab_core_organizations> modelList = new List<TobtabViewModels.tobtab_core_organizations>();
            foreach (var app in clsCompany)
            {
                {
                    model.organization_idx = app.organization_idx;
                    model.authorized_capital = app.authorized_capital;
                    model.is_has_business_address = app.is_has_business_address;
                    model.cosec_addr_1 = app.cosec_addr_1;
                    model.cosec_addr_2 = app.cosec_addr_2;
                    model.cosec_addr_3 = app.cosec_addr_3;
                    model.cosec_city = app.cosec_city;
                    model.cosec_state = app.cosec_state;
                    model.cosec_email = app.cosec_email;
                    model.cosec_fax_no = app.cosec_fax_no;
                    model.cosec_mobile_no = app.cosec_mobile_no;
                    model.cosec_name = app.cosec_name;
                    model.cosec_phone_no = app.cosec_phone_no;
                    model.cosec_postcode = app.cosec_postcode;
                    model.cosec_state = app.cosec_state;
                    model.incorporation_date = String.Format("{0:dd/MM/yyyy}", app.incorporation_date);
                    model.office_addr_1 = app.office_addr_1;
                    model.office_addr_2 = app.office_addr_2;
                    model.office_addr_3 = app.office_addr_3;
                    model.office_city = app.office_city;
                    model.office_email = app.office_email;
                    model.office_fax_no = app.office_fax_no;
                    model.office_mobile_no = app.office_mobile_no;
                    model.office_phone_no = app.office_phone_no;
                    model.office_postcode = app.office_postcode;
                    model.office_state = app.office_state;
                    model.organization_identifier = app.organization_identifier;
                    model.organization_location_category = app.organization_location_category;
                    model.organization_name = app.organization_name;
                    model.organization_status = app.organization_status;
                    model.organization_type = app.organization_type;

                    model.paid_capital = app.paid_capital;
                    model.registered_addr_1 = app.registered_addr_1;
                    model.registered_addr_2 = app.registered_addr_2;
                    model.registered_addr_3 = app.registered_addr_3;
                    model.registered_city = app.registered_city;
                    model.registered_email = app.registered_email;
                    model.registered_fax_no = app.registered_fax_no;
                    model.registered_mobile_no = app.registered_mobile_no;
                    model.registered_phone_no = app.registered_phone_no;
                    model.registered_postcode = app.registered_postcode;
                    model.registered_state = app.registered_state;
                    model.nature_of_business = app.nature_of_business;
                    model.website = app.website;
                    model.pbt_ref = app.pbt_ref;
                }
                modelList.Add(model);
            }


            return modelList;
        }

        public bool updateInactiveDirectorShareholder(Guid OrganizationIdx)
        {
            try
            {
                if (OrganizationIdx != null)
                {
                    //update existing record to active_status=0
                    var directorList = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => i.organization_ref == OrganizationIdx && i.active_status == 1 && i.modified_at == null).ToList();
                    foreach (var app in directorList)
                    {
                        core_organization_directors director = app;
                        director.active_status = 0;
                        unitOfWork.CoreOrganizationDirectorsRepository.Update(director);
                    }
                    var shareholderList = unitOfWork.CoreOrganizationShareholders.Find(i => i.organization_ref == OrganizationIdx && i.active_status == 1 && i.modified_at == null).ToList();
                    foreach (var app in shareholderList)
                    {
                        core_organization_shareholders shareholder = app;
                        shareholder.active_status = 0;
                        unitOfWork.CoreOrganizationShareholders.Update(shareholder);
                    }
                    unitOfWork.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string UpdateCompany(TobtabViewModels.tobtab_core_organizations data)
        {


            var clsCompany = new TourlistDataLayer.DataModel.core_organizations();

            clsCompany = unitOfWork.CoreOrganizations.Find(c => (c.organization_idx == data.organization_idx)).First();

            clsCompany.cosec_email = data.cosec_email;
            clsCompany.cosec_fax_no = data.cosec_fax_no;
            clsCompany.cosec_mobile_no = data.cosec_mobile_no;
            clsCompany.cosec_phone_no = data.cosec_phone_no;
            clsCompany.cosec_addr_1 = data.cosec_addr_1;
            clsCompany.cosec_addr_2 = data.cosec_addr_2;
            clsCompany.cosec_addr_3 = data.cosec_addr_3;
            clsCompany.cosec_postcode = data.cosec_postcode;
            clsCompany.cosec_city = data.cosec_city;
            clsCompany.cosec_state = data.cosec_state;
            clsCompany.cosec_name = data.cosec_name;

            clsCompany.office_email = data.office_email;
            clsCompany.office_fax_no = data.office_fax_no;
            clsCompany.office_mobile_no = data.office_mobile_no;
            clsCompany.office_phone_no = data.office_phone_no;
            clsCompany.office_city = data.office_city;
            clsCompany.office_state = data.office_state;
            clsCompany.office_postcode = data.office_postcode;
            clsCompany.pbt_ref = data.pbt_ref;
            clsCompany.is_has_business_address = data.is_has_business_address;

            clsCompany.registered_email = data.registered_email;
            clsCompany.registered_fax_no = data.registered_fax_no;
            clsCompany.registered_mobile_no = data.registered_mobile_no;
            clsCompany.registered_phone_no = data.registered_phone_no;
            clsCompany.website = data.website;

            unitOfWork.Complete();

            return "success";
        }


        public List<TourlistDataLayer.DataModel.tobtab_licenses> GetBidang(string RegistrationNo, string module)
        {
            List<TourlistDataLayer.DataModel.tobtab_licenses> modelList = new List<TourlistDataLayer.DataModel.tobtab_licenses>();
            /*if (module == "TOBTAB_NEW")
            {*/
            List<TourlistDataLayer.DataModel.tobtab_licenses> clsCompany = new List<TourlistDataLayer.DataModel.tobtab_licenses>();
            clsCompany = unitOfWork.TobtabLicenses.Find(c => (c.tobtab_idx.ToString() == RegistrationNo)).ToList();

            TourlistDataLayer.DataModel.tobtab_licenses model = new TourlistDataLayer.DataModel.tobtab_licenses();
            foreach (var app in clsCompany)
            {
                {
                    model.outbound = app.outbound;
                    model.inbound = app.inbound;
                    model.umrah = app.umrah;
                    model.ticketing = app.ticketing;
                    model.renewal_duration_years = app.renewal_duration_years;
                    model.tobtab_idx = app.tobtab_idx;
                }
                modelList.Add(model);
            }
            //}
            /* if (module == "TOBTAB_RENEW")
             {
                 List<TourlistDataLayer.DataModel.tobtab_renewals> clsCompany = new List<TourlistDataLayer.DataModel.tobtab_renewals>();
                 clsCompany = unitOfWork.TobtabRenewal.Find(c => (c.tobtab_renewal_idx.ToString() == RegistrationNo)).ToList();

                 TourlistDataLayer.DataModel.tobtab_licenses model = new TourlistDataLayer.DataModel.tobtab_licenses();
                 foreach (var app in clsCompany)
                 {
                     {
                         model.outbound = app.outbound;
                         model.inbound = app.inbound;
                         model.ticketing = app.ticketing;
                         model.umrah = app.umrah;
                         model.tobtab_idx = app.tobtab_renewal_idx;
                     }
                     modelList.Add(model);
                 }
             }*/
            return modelList;
        }

        public List<TourlistDataLayer.DataModel.core_license> GetBidangCoreLicense(string orgIdx, string module)
        {
            List<TourlistDataLayer.DataModel.core_license> modelList = new List<TourlistDataLayer.DataModel.core_license>();
            /*if (module == "TOBTAB_NEW")
            {*/
            List<TourlistDataLayer.DataModel.core_license> clsCompany = new List<TourlistDataLayer.DataModel.core_license>();
            clsCompany = unitOfWork.CoreLicenseRepository.Find(c => (c.core_organization_ref.ToString() == orgIdx)).ToList();

            TourlistDataLayer.DataModel.core_license model = new TourlistDataLayer.DataModel.core_license();
            foreach (var app in clsCompany)
            {
                {
                    model.outbound = app.outbound;
                    model.inbound = app.inbound;
                    model.ticketing = app.ticketing;
                    model.period = app.period;
                }
                modelList.Add(model);
            }
            return modelList;
        }

        public string UpdateBidang(string registrationNo, TourlistDataLayer.DataModel.tobtab_licenses data)
        {
            var model = new TourlistDataLayer.DataModel.tobtab_licenses();
            model = unitOfWork.TobtabLicenses.Find(c => (c.tobtab_idx.ToString() == registrationNo)).First();
            model.outbound = data.outbound;
            model.inbound = data.inbound;
            model.ticketing = data.ticketing;
           // model.umrah = data.umrah;
            model.renewal_duration_years = data.renewal_duration_years;

            unitOfWork.Complete();

            return "success";
        }

        public string UpdateReturnLicense(string registrationNo, TourlistDataLayer.DataModel.tobtab_terminate_licenses data)
        {


            /*  var model = new TourlistDataLayer.DataModel.tobtab_terminate_licenses();

              model = unitOfWork.TobtabTerminateLicenseRepository.Find(c => (c.tobtab_terminate_license_idx.ToString() == registrationNo)).First();

              model.tobtab_application_ref = data.tobtab_application_ref;
              model.terminate_reason_idx = data.terminate_reason_idx;
              model.terminate_reason = data.terminate_reason;
              model.return_license_date = data.return_license_date;*/

            unitOfWork.Complete();

            return "success";
        }

        public string UpdateAcknowledgeReturnLicense(string registrationNo, TourlistDataLayer.DataModel.tobtab_terminate_licenses data)
        {


            /*var model = new TourlistDataLayer.DataModel.tobtab_terminate_licenses();

            model = unitOfWork.TobtabTerminateLicenseRepository.Find(c => (c.tobtab_terminate_license_idx.ToString() == registrationNo)).First();

            model.is_acknowledged = data.is_acknowledged;
*/
            unitOfWork.Complete();

            return "success";
        }

        public tobtab_tg_exceptions getTgExceptionInfoByGuid(Guid license_ref)
        {
            var tgException = unitOfWork.TobtabTGExceptionsRepository.Find(c => (c.tobtab_licenses_ref == license_ref)).FirstOrDefault();
            if (tgException == null)
            {
                tgException = new tobtab_tg_exceptions();
            }
            return tgException;
        }
        public tobtab_tg_exceptions getTgExceptionInfo(String license_ref)
        {
            var tgException = unitOfWork.TobtabTGExceptionsRepository.Find(c => (c.tobtab_licenses_ref.ToString() == license_ref)).FirstOrDefault();
            return tgException;
        }
        public string AddOrganizerInfo(string company_id, string license_ref, TourlistDataLayer.DataModel.tobtab_tg_exceptions data)
        {
            var tgException = unitOfWork.TobtabTGExceptionsRepository.Find(c => (c.tobtab_licenses_ref.ToString() == license_ref)).FirstOrDefault();
            if (tgException == null)
            {
                data.tobtab_tg_exceptions_idx = Guid.NewGuid();
                unitOfWork.TobtabTGExceptionsRepository.Add(data);
            }
            else
            {
                data.modified_at = DateTime.Now;
                data.modified_by = data.created_by;
                unitOfWork.TobtabTGExceptionsRepository.Update(data);
            }
            unitOfWork.Complete();

            return "success";
        }

        public string AddMarketingAgentPersonInfo(string company_id, string license_ref, TourlistDataLayer.DataModel.core_persons data, string userid)
        {

            var person = unitOfWork.CorePersonsRepository.Find(c => (c.person_identifier.ToString() == data.person_identifier)).FirstOrDefault();
            if (person == null)
            {
                person = data;
                person.person_idx = Guid.NewGuid();
                person.person_type = unitOfWork.RefUserTypesRepository.Find(c => (c.userType_name == "I")).FirstOrDefault().userType_idx;
                person.modified_dt = DateTime.Now;
                person.created_dt = DateTime.Now;
                person.created_by = data.created_by;
                person.modified_by = data.created_by;
                unitOfWork.CorePersonsRepository.Add(person);
            }
            else
            {
                person.person_name = data.person_name;
                person.person_identifier = data.person_identifier;
                person.personal_mobile_no = data.personal_mobile_no;
                person.residential_phone_no = data.personal_mobile_no;
                person.person_birthday = data.person_birthday;
                person.residential_addr_1 = data.residential_addr_1;
                person.residential_addr_2 = data.residential_addr_2;
                person.residential_addr_3 = data.residential_addr_3;
                person.residential_postcode = data.residential_postcode;
                person.residential_city = data.residential_city;
                person.residential_state = data.residential_state;
                person.residential_email = data.residential_email;
                person.contact_addr_1 = data.residential_addr_1;
                person.contact_addr_2 = data.residential_addr_2;
                person.contact_addr_3 = data.residential_addr_3;
                person.contact_postcode = data.residential_postcode;
                person.contact_city = data.residential_city;
                person.contact_state = data.residential_state;
                person.contact_mobile_no = data.personal_mobile_no;
                person.contact_phone_no = data.personal_mobile_no;
                person.contact_email = data.contact_email;
                person.person_age = data.person_age;
                person.person_gender = data.person_gender;
                person.person_religion = data.person_religion;
                person.person_nationality = data.person_nationality;
                person.person_is_bumiputera = data.person_is_bumiputera;
                person.person_employ_position = data.person_employ_position;
                person.person_is_bumiputera = data.person_is_bumiputera;
                person.person_is_employed = data.person_is_employed;
                if (person.person_is_employed == 1)
                {
                    //model.person_employer_organization = Request["person_employer_organization"].ToString();
                    person.office_name = data.office_name;
                    person.office_addr_1 = data.office_addr_1;
                    person.office_addr_2 = data.office_addr_2;
                    person.office_addr_3 = data.office_addr_3;
                    person.office_postcode = data.office_postcode;
                    person.office_city = data.office_city;// Guid.Parse(Request["office_city"].ToString());
                    person.office_state = data.office_state;
                    person.office_mobile_no = data.office_mobile_no;
                    person.office_phone_no = data.office_phone_no;
                    person.office_email = data.office_email;
                }
                person.person_type = unitOfWork.RefUserTypesRepository.Find(c => (c.userType_name == "I")).FirstOrDefault().userType_idx;
                person.modified_dt = DateTime.Now;
                person.modified_by = data.created_by;
                unitOfWork.CorePersonsRepository.Update(person);
            }
            var marketingAgent = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.person_ref == person.person_idx)).FirstOrDefault();
            if (marketingAgent != null)
            {
                marketingAgent.modified_by = data.created_by;
                marketingAgent.modified_at = DateTime.Now;
                marketingAgent.tobtab_licenses_ref = Guid.Parse(license_ref);
                marketingAgent.person_ref = person.person_idx;
                if (person.person_employ_position != null && person.person_employ_position != "")
                {
                    marketingAgent.person_position = person.person_employ_position;
                }
                else
                {
                    marketingAgent.person_position = person.person_idx.ToString();
                }
                marketingAgent.is_employed = data.person_is_employed;
                marketingAgent.active_status = 1;
                unitOfWork.TobtabMarketingAgentRepository.Update(marketingAgent);
            }
            else
            {
                marketingAgent = new tobtab_marketing_agents();
                marketingAgent.marketing_agent_idx = Guid.NewGuid();
                marketingAgent.created_by = data.created_by;
                marketingAgent.created_at = DateTime.Now;
                marketingAgent.tobtab_licenses_ref = Guid.Parse(license_ref);
                marketingAgent.person_ref = person.person_idx;
                if (person.person_employ_position != null && person.person_employ_position != "")
                {
                    marketingAgent.person_position = person.person_employ_position;
                }
                else
                {
                    marketingAgent.person_position = person.person_idx.ToString();
                }
                marketingAgent.is_employed = data.person_is_employed;
                marketingAgent.active_status = 1;
                unitOfWork.TobtabMarketingAgentRepository.Add(marketingAgent);
            }
            //checking uploadImage
            var upload = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.person_ref.ToString() == userid && i.upload_description == userid).FirstOrDefault();
            if (upload != null)
            {
                upload.person_ref = marketingAgent.person_ref;
                upload.upload_description = null;
                unitOfWork.CoreUploadsFreeFormPersonRepository.Update(upload);
            }

            unitOfWork.Complete();

            return person.person_idx.ToString();
        }
        public string tempAddMarketingAgentPersonInfo(string company_id, string license_ref, TourlistDataLayer.DataModel.core_persons data)
        {

            var person = unitOfWork.CorePersonsRepository.Find(c => (c.person_identifier.ToString() == data.person_identifier)).FirstOrDefault();
            if (person == null)
            {
                person = data;
                person.person_idx = Guid.NewGuid();
                person.person_type = unitOfWork.RefUserTypesRepository.Find(c => (c.userType_name == "I")).FirstOrDefault().userType_idx;
                person.modified_dt = DateTime.Now;
                person.created_dt = DateTime.Now;
                person.created_by = data.created_by;
                person.modified_by = data.created_by;
                unitOfWork.CorePersonsRepository.Add(person);
            }
            else
            {
                person.person_name = data.person_name;
                person.person_identifier = data.person_identifier;
                person.person_type = unitOfWork.RefUserTypesRepository.Find(c => (c.userType_name == "I")).FirstOrDefault().userType_idx;
                person.modified_dt = DateTime.Now;
                person.modified_by = data.created_by;
                unitOfWork.CorePersonsRepository.Update(person);
            }
            var marketingAgent = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.person_ref == person.person_idx)).FirstOrDefault();
            if (marketingAgent != null)
            {
                marketingAgent.modified_by = data.created_by;
                marketingAgent.modified_at = DateTime.Now;
                marketingAgent.tobtab_licenses_ref = Guid.Parse(license_ref);
                marketingAgent.person_ref = person.person_idx;
                if (person.person_employ_position != null && person.person_employ_position != "")
                {
                    marketingAgent.person_position = person.person_employ_position;
                }
                else
                {
                    marketingAgent.person_position = person.person_idx.ToString();
                }
                marketingAgent.is_employed = data.person_is_employed;
                marketingAgent.active_status = 1;
                unitOfWork.TobtabMarketingAgentRepository.Update(marketingAgent);
            }
            else
            {
                marketingAgent = new tobtab_marketing_agents();
                marketingAgent.marketing_agent_idx = Guid.NewGuid();
                marketingAgent.created_by = data.created_by;
                marketingAgent.created_at = DateTime.Now;
                marketingAgent.tobtab_licenses_ref = Guid.Parse(license_ref);
                marketingAgent.person_ref = person.person_idx;
                if (person.person_employ_position != null && person.person_employ_position != "")
                {
                    marketingAgent.person_position = person.person_employ_position;
                }
                else
                {
                    marketingAgent.person_position = person.person_idx.ToString();
                }
                marketingAgent.is_employed = data.person_is_employed;
                marketingAgent.active_status = 1;
                unitOfWork.TobtabMarketingAgentRepository.Add(marketingAgent);
            }
            unitOfWork.Complete();

            return marketingAgent.marketing_agent_idx.ToString();
        }

        public string AddShareholderPersonInfo(string company_id, string license_ref, string shareholder_number_of_shares, TourlistDataLayer.DataModel.core_persons data, string status_shareholder)
        {

            var shareholder = unitOfWork.CorePersonsRepository.Find(c => (c.person_identifier.ToString() == data.person_identifier)).FirstOrDefault();
            var shareholder_org = unitOfWork.CoreOrganizationShareholders.Find(c => (c.shareholder_person_ref.ToString() == shareholder.person_idx.ToString()
            && c.organization_ref.ToString() == company_id)).FirstOrDefault();

            shareholder.person_name = data.person_name;
            shareholder.person_identifier = data.person_identifier;
            shareholder.personal_mobile_no = data.personal_mobile_no;
            shareholder.office_mobile_no = data.personal_mobile_no;
            shareholder.contact_mobile_no = data.personal_mobile_no;
            shareholder.person_birthday = data.person_birthday;
            shareholder.residential_addr_1 = data.residential_addr_1;
            shareholder.residential_addr_2 = data.residential_addr_2;
            shareholder.residential_addr_3 = data.residential_addr_3;
            shareholder.person_age = data.person_age;
            shareholder.person_gender = data.person_gender;
            shareholder.person_religion = data.person_religion;
            shareholder.residential_postcode = data.residential_postcode;
            shareholder.person_nationality = data.person_nationality;
            shareholder.residential_city = data.residential_city;
            shareholder.residential_state = data.residential_state;
            shareholder.person_is_bumiputera = data.person_is_bumiputera;

            shareholder.person_type = unitOfWork.RefUserTypesRepository.Find(c => (c.userType_name == "I")).FirstOrDefault().userType_idx;
            shareholder.modified_dt = DateTime.Now;

            if (shareholder_org == null)
            {
                var model_org = new TourlistDataLayer.DataModel.core_organization_shareholders();

                model_org.organization_shareholder_idx = Guid.NewGuid();
                model_org.organization_ref = Guid.Parse(company_id);
                model_org.shareholder_person_ref = data.person_idx;
                model_org.number_of_shares = Decimal.Parse(shareholder_number_of_shares);
                model_org.status_shareholder = Guid.Parse(status_shareholder);
                model_org.active_status = 1;

                unitOfWork.CoreOrganizationShareholders.Add(model_org);
            }

            shareholder_org.status_shareholder = Guid.Parse(status_shareholder);
            shareholder_org.number_of_shares = Decimal.Parse(shareholder_number_of_shares);
            shareholder_org.active_status = 1;
            unitOfWork.CoreOrganizationShareholders.Update(shareholder_org);

            unitOfWork.Complete();

            return "success";
        }

        public string AddDirectorPersonInfo(string company_id, string license_ref, TourlistDataLayer.DataModel.core_persons data)
        {

            var director = unitOfWork.Persons.Find(c => (c.person_identifier.ToString() == data.person_identifier)).First();

            //director.person_name = data.person_name;
            //director.person_identifier = data.person_identifier;
            director.residential_phone_no = data.residential_phone_no;
            director.personal_mobile_no = data.personal_mobile_no;
            director.person_birthday = data.person_birthday;
            director.person_age = data.person_age;
            director.person_gender = data.person_gender;
            //director.residential_postcode = data.residential_postcode;
            director.person_nationality = data.person_nationality;
            director.person_religion = data.person_religion;
            //director.residential_city = data.residential_city;
            //director.residential_state = data.residential_state;

            director.person_type = unitOfWork.RefUserTypesRepository.Find(c => (c.userType_name == "I")).FirstOrDefault().userType_idx;
            director.modified_dt = DateTime.Now;

            //checking if Warganegara,delete Pas Pengajian
            var isMalaysia = unitOfWork.RefGeoCountriesRepository.Find(i => i.country_code == "MY" && i.country_idx == director.person_nationality).FirstOrDefault();
            if (isMalaysia != null)
            {
                var refStatusPasPengajian = unitOfWork.VwRefReferencesRepository.Find(i => i.ref_type_name == "UPLOADTYPE" && i.ref_name == "PAS_PENGGAJIAN").FirstOrDefault();
                var coreUpload = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.upload_type_ref == refStatusPasPengajian.ref_idx && i.person_ref == director.person_idx).FirstOrDefault();
                if (coreUpload != null)
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Remove(coreUpload);
            }
            unitOfWork.Complete();
            return "success";
        }

        public string UpdatePerakuan(string registrationNo, TourlistDataLayer.DataModel.core_acknowledgements data, string module_id)
        {

            var stubref = "";
            var tobtab = unitOfWork.TobtabLicenses.Find(c => (c.tobtab_idx.ToString() == registrationNo)).First();
            stubref = tobtab.stub_ref.ToString();

            var acknowledgement = unitOfWork.CoreAcknowledgementsRepository.Find(c => (c.license_type_ref.ToString() == registrationNo)).FirstOrDefault();
            //var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.organization_director_idx.ToString() == data.acknowledge_person_name)).FirstOrDefault();
            var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.person_ref.ToString() == data.acknowledge_person_name)).FirstOrDefault();
            var person = unitOfWork.CorePersonsRepository.Find(c => (c.person_idx == director.person_ref)).FirstOrDefault();
            var module = unitOfWork.CoreSolModulesRepository.Find(c => (c.module_name == module_id)).FirstOrDefault();
            if (acknowledgement != null)
            {
                acknowledgement.acknowledge_person_name = person.person_name;
                acknowledgement.acknowledge_person_ref = person.person_idx;
                acknowledgement.module_ref = module.modules_idx;
                acknowledgement.acknowledge_person_icno = data.acknowledge_person_icno;
                acknowledgement.acknowledge_position = data.acknowledge_position;
                acknowledgement.acknowledge_organization_name = data.acknowledge_organization_name;
                acknowledgement.is_acknowledged = data.is_acknowledged;
                acknowledgement.license_type_ref = Guid.Parse(registrationNo.ToString());
                acknowledgement.stub_ref = Guid.Parse(stubref);
                acknowledgement.acknowledge_date = DateTime.Now;
                acknowledgement.active_status = 1;
                acknowledgement.created_at = DateTime.Now;
                acknowledgement.modified_at = DateTime.Now;
                acknowledgement.created_by = person.person_idx;
                acknowledgement.modified_by = person.person_idx;

                unitOfWork.Complete();
            }
            else
            {
                var model = new TourlistDataLayer.DataModel.core_acknowledgements();

                model.acknowledgement_idx = Guid.NewGuid();

                model.acknowledge_person_name = person.person_name;
                model.acknowledge_person_ref = person.person_idx;
                model.module_ref = module.modules_idx;
                model.acknowledge_person_icno = data.acknowledge_person_icno;
                model.acknowledge_position = data.acknowledge_position;
                model.acknowledge_organization_name = data.acknowledge_organization_name;
                model.is_acknowledged = data.is_acknowledged;
                model.license_type_ref = Guid.Parse(registrationNo);
                model.stub_ref = Guid.Parse(stubref);
                model.acknowledge_date = DateTime.Now;
                model.active_status = 1;
                model.created_at = DateTime.Now;
                model.modified_at = DateTime.Now;
                model.created_by = person.person_idx;
                model.modified_by = person.person_idx;

                unitOfWork.CoreAcknowledgementsRepository.Add(model);
                unitOfWork.Complete();
            }


            return "success";
        }


        public List<TourlistDataLayer.DataModel.core_acknowledgements> GetPerakuan(string stubRef)
        {
            List<TourlistDataLayer.DataModel.core_acknowledgements> clsCompany = new List<TourlistDataLayer.DataModel.core_acknowledgements>();
            clsCompany = unitOfWork.CoreAcknowledgementsRepository.Find(c => (c.stub_ref.ToString() == stubRef)).ToList();

            TourlistDataLayer.DataModel.core_acknowledgements model = new TourlistDataLayer.DataModel.core_acknowledgements();
            List<TourlistDataLayer.DataModel.core_acknowledgements> modelList = new List<TourlistDataLayer.DataModel.core_acknowledgements>();
            foreach (var app in clsCompany)
            {
                {
                    model.acknowledgement_idx = Guid.NewGuid();

                    model.acknowledge_person_name = app.acknowledge_person_name;
                    model.acknowledge_person_icno = app.acknowledge_person_icno;
                    model.acknowledge_position = app.acknowledge_position;
                    model.acknowledge_organization_name = app.acknowledge_organization_name;
                    model.is_acknowledged = app.is_acknowledged;
                    model.license_type_ref = app.license_type_ref;
                    model.stub_ref = app.stub_ref;
                    model.acknowledge_person_ref = app.acknowledge_person_ref;
                }
                modelList.Add(model);
            }


            return modelList;
        }

        public string UploadDocumentPerson(string subTxn_idx, string fname, string module_name, string doc_name, string license_ref,string userID)
        {

            var shareholder = unitOfWork.CoreOrganizationShareholders.Find(c => (c.organization_shareholder_idx.ToString() == subTxn_idx)).FirstOrDefault();
            var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.organization_director_idx.ToString() == subTxn_idx)).FirstOrDefault(); ;
            var marketingAgent = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.marketing_agent_idx.ToString() == subTxn_idx)).FirstOrDefault();
            var license = unitOfWork.TobtabLicenses.Find(c => (c.tobtab_idx.ToString() == license_ref)).FirstOrDefault();


            if (shareholder != null)
            {
                var person = unitOfWork.Persons.Find(c => (c.person_idx == shareholder.shareholder_person_ref)).FirstOrDefault();
                var refupload = unitOfWork.RefReferencesRepository.Find(c => (c.ref_code == module_name)).FirstOrDefault();
                var personUploadExist = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx && c.upload_type_ref == refupload.ref_idx)).FirstOrDefault();
                TourlistDataLayer.DataModel.core_uploads_freeform_by_persons person_upload = new TourlistDataLayer.DataModel.core_uploads_freeform_by_persons();

                if (personUploadExist != null)
                {
                    personUploadExist.upload_name = doc_name;
                    personUploadExist.upload_description = license.tobtab_idx.ToString();
                    personUploadExist.active_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7"); //Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7")
                    personUploadExist.upload_path = fname;
                    personUploadExist.created_at = DateTime.Now;
                    personUploadExist.created_by = Guid.Parse(userID);
                    personUploadExist.modified_at = DateTime.Now;
                    personUploadExist.modified_by = Guid.Parse(userID);
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Update(personUploadExist);
                }
                else
                {
                    person_upload.uploads_freeform_by_persons_idx = Guid.NewGuid();
                    person_upload.person_ref = person.person_idx;
                    person_upload.upload_type_ref = refupload.ref_idx;
                    person_upload.upload_name = doc_name;
                    person_upload.upload_description = license.tobtab_idx.ToString();
                    person_upload.active_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7"); //Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7")
                    person_upload.upload_path = fname;
                    person_upload.created_at = DateTime.Now;
                    person_upload.created_by = Guid.Parse(userID);
                    person_upload.modified_at = DateTime.Now;
                    person_upload.modified_by = Guid.Parse(userID);
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Add(person_upload);
                }

                unitOfWork.Complete();

            }
            else if (director != null)
            {
                var person = unitOfWork.Persons.Find(c => (c.person_idx == director.person_ref)).FirstOrDefault();
                var refupload = unitOfWork.RefReferencesRepository.Find(c => (c.ref_code == module_name)).FirstOrDefault();
                var personUploadExist = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx && c.upload_type_ref == refupload.ref_idx)).FirstOrDefault();
                TourlistDataLayer.DataModel.core_uploads_freeform_by_persons person_upload = new TourlistDataLayer.DataModel.core_uploads_freeform_by_persons();
                if (personUploadExist != null)
                {
                    personUploadExist.upload_name = doc_name;
                    personUploadExist.upload_description = license.tobtab_idx.ToString();
                    personUploadExist.active_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7"); //Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7")
                    personUploadExist.upload_path = fname;
                    personUploadExist.created_at = DateTime.Now;
                    personUploadExist.created_by = Guid.Parse(userID);
                    personUploadExist.modified_at = DateTime.Now;
                    personUploadExist.modified_by = Guid.Parse(userID);
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Update(personUploadExist);
                }
                else
                {
                    person_upload.uploads_freeform_by_persons_idx = Guid.NewGuid();
                    person_upload.person_ref = person.person_idx;
                    person_upload.upload_type_ref = refupload.ref_idx;
                    person_upload.upload_name = doc_name;
                    person_upload.upload_description = license.tobtab_idx.ToString();
                    person_upload.active_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7"); //Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7")
                    person_upload.upload_path = fname;
                    person_upload.created_at = DateTime.Now;
                    person_upload.created_by = Guid.Parse(userID);
                    person_upload.modified_at = DateTime.Now;
                    person_upload.modified_by = Guid.Parse(userID);
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Add(person_upload);
                }
                unitOfWork.Complete();

            }
            else if (marketingAgent != null)
            {
                var person = unitOfWork.Persons.Find(c => (c.person_idx == marketingAgent.person_ref)).FirstOrDefault();
                var refupload = unitOfWork.RefReferencesRepository.Find(c => (c.ref_code == module_name)).FirstOrDefault();
                var personUploadExist = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx && c.upload_type_ref == refupload.ref_idx)).FirstOrDefault();
                TourlistDataLayer.DataModel.core_uploads_freeform_by_persons person_upload = new TourlistDataLayer.DataModel.core_uploads_freeform_by_persons();
                String uploadIdx = "";
                if (personUploadExist != null)
                {
                    personUploadExist.upload_name = doc_name;
                    personUploadExist.upload_description = refupload.ref_description;
                    personUploadExist.active_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7"); //Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7")
                    personUploadExist.upload_path = fname;
                    personUploadExist.created_at = DateTime.Now;
                    personUploadExist.created_by = Guid.Parse(userID);
                    personUploadExist.modified_at = DateTime.Now;
                    personUploadExist.modified_by = Guid.Parse(userID);
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Update(personUploadExist);
                    uploadIdx = personUploadExist.uploads_freeform_by_persons_idx.ToString();
                }
                else
                {
                    person_upload.uploads_freeform_by_persons_idx = Guid.NewGuid();
                    person_upload.person_ref = person.person_idx;
                    person_upload.upload_type_ref = refupload.ref_idx;
                    person_upload.upload_name = doc_name;
                    person_upload.upload_description = refupload.ref_description;
                    person_upload.active_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7"); //Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7")
                    person_upload.upload_path = fname;
                    person_upload.created_at = DateTime.Now;
                    person_upload.created_by = Guid.Parse(userID);
                    person_upload.modified_at = DateTime.Now;
                    person_upload.modified_by = Guid.Parse(userID);
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Add(person_upload);
                    uploadIdx = person_upload.uploads_freeform_by_persons_idx.ToString();
                }
                if (module_name == "MYKAD")
                {
                    marketingAgent.person_id_upload_ref = Guid.Parse(uploadIdx);
                }
                else if (module_name == "OFFER_LETTER")
                {
                    marketingAgent.person_offer_letter_ref = Guid.Parse(uploadIdx);
                }
                else if (module_name == "INSOLVENCY_STATUS")
                {
                    marketingAgent.person_bankrupt_clearance_upload_ref = Guid.Parse(uploadIdx);
                }
                else if (module_name == "HOD_APPROVAL")
                {
                    marketingAgent.employer_permission_upload_ref = Guid.Parse(uploadIdx);
                }
                marketingAgent.modified_at = DateTime.Now;
                marketingAgent.modified_by = Guid.Parse(license.modified_by.ToString());
                unitOfWork.TobtabMarketingAgentRepository.Update(marketingAgent);
                unitOfWork.Complete();
            }
            return "success";

        }


        public core_uploads_freeform_by_persons GetDocumentPerson(string subTxn_idx, string module_name, string shareholder_identifier, string license_ref)
        {

            var shareholder = unitOfWork.CoreOrganizationShareholders.Find(c => (c.organization_shareholder_idx.ToString() == subTxn_idx)).FirstOrDefault();
            var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.organization_director_idx.ToString() == subTxn_idx)).FirstOrDefault();
            var marketingAgent = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.marketing_agent_idx.ToString() == subTxn_idx)).FirstOrDefault();
            var license = unitOfWork.TobtabLicenses.Find(c => (c.tobtab_idx.ToString() == license_ref)).FirstOrDefault();
            var ut = TourlistUnitOfWork.RefReferencesTypesRepository.Find(x => x.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var ut_ref = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ut.ref_idx && x.ref_code == "MYKAD").FirstOrDefault();
            var ut_ref2 = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ut.ref_idx && x.ref_code == "HOD_APPROVAL").FirstOrDefault();
            var ut_ref3 = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ut.ref_idx && x.ref_code == "INSOLVENCY_STATUS").FirstOrDefault();
            var ut_ref4 = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ut.ref_idx && x.ref_code == "OFFER_LETTER").FirstOrDefault();
            var ut1 = ut_ref != null ? ut_ref.ref_idx : Guid.Empty;
            var ut2 = ut_ref2 != null ? ut_ref2.ref_idx : Guid.Empty;
            var ut3 = ut_ref3 != null ? ut_ref3.ref_idx : Guid.Empty;
            var ut4 = ut_ref4 != null ? ut_ref4.ref_idx : Guid.Empty;

            var uploads = new core_uploads_freeform_by_persons();

            if (shareholder != null)
            {
                var person = unitOfWork.CorePersonsRepository.Find(c => (c.person_idx == shareholder.shareholder_person_ref)).FirstOrDefault();
                var refupload = unitOfWork.RefReferencesRepository.Find(c => (c.ref_code == module_name)).FirstOrDefault();

                if (module_name == "MYKAD")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut1).FirstOrDefault();
                }
                else if (module_name == "HOD_APPROVAL")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut2).FirstOrDefault();
                }
                else if (module_name == "INSOLVENCY_STATUS")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut3).FirstOrDefault();
                }
                else
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut4).FirstOrDefault();
                }


                //uploads.upload_name
                return uploads;

            }
            else if (director != null)
            {
                var person = unitOfWork.Persons.Find(c => (c.person_idx == director.person_ref)).FirstOrDefault();
                var refupload = unitOfWork.RefReferencesRepository.Find(c => (c.ref_code == module_name)).FirstOrDefault();

                if (module_name == "MYKAD")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut1).FirstOrDefault();
                }
                else if (module_name == "HOD_APPROVAL")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut2).FirstOrDefault();
                }
                else if (module_name == "INSOLVENCY_STATUS")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut3).FirstOrDefault();
                }
                else
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut4).FirstOrDefault();
                }
                //uploads.upload_name
                return uploads;

            }
            else if (marketingAgent != null)
            {
                var person = unitOfWork.Persons.Find(c => (c.person_idx == marketingAgent.person_ref)).FirstOrDefault();
                var refupload = unitOfWork.RefReferencesRepository.Find(c => (c.ref_code == module_name)).FirstOrDefault();

                if (module_name == "MYKAD")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut1).FirstOrDefault();
                }
                else if (module_name == "HOD_APPROVAL")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut2).FirstOrDefault();
                }
                else if (module_name == "INSOLVENCY_STATUS")
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut3).FirstOrDefault();
                }
                else
                {
                    uploads = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == person.person_idx).Where(x => x.upload_type_ref == ut4).FirstOrDefault();
                }
                //uploads.upload_name
                return uploads;

            }

            return uploads;

        }
        public string UploadDocumentChecklist(string checklist_id, string fname, string doc_name)
        {

            var checklist = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chkitem_instance_idx.ToString() == checklist_id)).FirstOrDefault();
            if (checklist != null)
            {
                checklist.string1 = doc_name;
                //checklist.string2 = fname;
                checklist.upload_location = fname;
                checklist.date1 = DateTime.Now;
                unitOfWork.CoreChkItemsInstancesRepository.Update(checklist);
                unitOfWork.Complete();
            }

            return "success";

        }

        public int checkDokumenSokongan(string AppID, string Module, string userID)
        {

            List<TourlistDataLayer.DataModel.tobtab_licenses> licenses = new List<TourlistDataLayer.DataModel.tobtab_licenses>();
            licenses = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == AppID)).ToList();

            List<TourlistDataLayer.DataModel.flow_application_stubs> application = new List<TourlistDataLayer.DataModel.flow_application_stubs>();
            application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == AppID)).ToList();

            List<TourlistDataLayer.DataModel.core_chklist_instances> chklist_instances = new List<TourlistDataLayer.DataModel.core_chklist_instances>();
            chklist_instances = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.application_ref.ToString() == AppID)).ToList();



            List<TourlistDataLayer.DataModel.core_chkitems_instances> chkitems_instances = new List<TourlistDataLayer.DataModel.core_chkitems_instances>();
            //chkitems_instances = unitOfWork.CoreChkItemsInstancesRepository.GetAll().ToList();
            chkitems_instances = unitOfWork.CoreChkItemsInstancesRepository.Find(i => i.created_by.ToString() == userID).ToList();

            List<TourlistDataLayer.DataModel.core_chklst_lists> chklst_lists = new List<TourlistDataLayer.DataModel.core_chklst_lists>();
            chklst_lists = unitOfWork.CoreChklstListsRepository.Find(i => (i.chklist_code == Module)).ToList();

            List<TourlistDataLayer.DataModel.core_chklst_items> chklst_items = new List<TourlistDataLayer.DataModel.core_chklst_items>();
            chklst_items = unitOfWork.CoreChklstItemsRepository.GetAll().ToList();

            var clsList = (from license in licenses
                           from app in application

                               .Where(d => d.apply_idx.ToString() == license.stub_ref.ToString())
                               .DefaultIfEmpty() // <== makes join left join  

                           from chklist_instance in chklist_instances
                                   .Where(p => p.application_ref.ToString() == AppID)

                           from chklst_list in chklst_lists
                               .Where(p => p.chklist_idx.ToString() == chklist_instance.chklist_tplt_ref.ToString())

                           from chkitems_instance in chkitems_instances
                           .Where(p => p.chklist_instance_ref.ToString() == chklist_instance.chklist_instance_idx.ToString() && p.string1 == null)

                           from chklst_item in chklst_items
                           .Where(p => p.item_idx.ToString() == chkitems_instance.chklist_tplt_item_ref.ToString() && p.descr_bool2 == "1")

                           select new
                           {
                               chkitem_instance_idx = chkitems_instance.chkitem_instance_idx.ToString(),
                               descr_bool2 = chklst_item.descr_bool2,

                           }).ToList();
            List<TobtabViewModels.tobtab_supporting_documents> modelList = new List<TobtabViewModels.tobtab_supporting_documents>();

            return clsList.Count;

        }

        public string UpdateCompanyDataSSM(string module_id, string component_id, TobtabViewModels.tobtab_ssm_organization model_org)
        {

            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            var chklist = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.chklist_instance_idx.ToString() == chklistitems.chklist_instance_ref.ToString())).First();
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == chklist.application_ref.ToString())).First();
            var user = new core_users();
            var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
            user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == license.created_by.ToString())).First();
            var person = unitOfWork.Persons.Find(i => (i.person_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            CultureInfo provider = CultureInfo.InvariantCulture;

            if (organization == null)
            {
                organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.user_organization.ToString())).FirstOrDefault();
            }
            if (person == null && organization == null)
            {
                var data_org = new TourlistDataLayer.DataModel.core_organizations();
                data_org.organization_idx = Guid.NewGuid();
                data_org.organization_name = model_org.company_name;
                data_org.old_organization_name = model_org.company_oldname;
                data_org.organization_identifier = model_org.company_newregno;
                data_org.nature_of_business = model_org.nature_of_business;
                string sIncdate = model_org.company_incdate;
                DateTime dIncdate = DateTime.ParseExact(sIncdate, "dd/MM/yyyy", provider);
                organization.incorporation_date = dIncdate;
                data_org.is_has_business_address = (short?)((model_org.companyb_addr1 != "") ? 1 : 0);
                data_org.office_addr_1 = model_org.companyb_addr1;
                data_org.office_addr_2 = model_org.companyb_addr2;
                data_org.office_addr_3 = model_org.companyb_addr3;
                data_org.office_postcode = model_org.companyb_postcode;
                var vStateOff = unitOfWork.RefGeoStatesRepository.Find(i => (i.ssm_code.Contains(model_org.companyb_state))).FirstOrDefault();
                data_org.office_state = (vStateOff == null) ? Guid.Empty : vStateOff.state_idx;
                if (vStateOff != null)
                {
                    var vPostcode_Office = unitOfWork.VwGeoListRepository.Find(i => (i.town_name == model_org.companyb_town && i.state_code == vStateOff.state_code)).FirstOrDefault();
                    data_org.office_city = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.town_idx;
                }
                else
                {
                    var vPostcode_Office = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.companyb_postcode)).FirstOrDefault();
                    data_org.office_city = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.town_idx;
                    data_org.office_state = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.state_idx;
                }

                data_org.registered_addr_1 = model_org.companyr_addr1;
                data_org.registered_addr_2 = model_org.companyr_addr2;
                data_org.registered_addr_3 = model_org.companyr_addr3;
                data_org.registered_postcode = model_org.companyr_postcode;
                var vStateReg = unitOfWork.RefGeoStatesRepository.Find(i => (i.ssm_code.Contains(model_org.companyr_state))).FirstOrDefault();
                data_org.registered_state = (vStateReg == null) ? Guid.Empty : vStateReg.state_idx;
                if (vStateReg != null)
                {
                    var vPostcode_Reg = unitOfWork.VwGeoListRepository.Find(i => (i.town_name == model_org.companyr_town && i.state_code == vStateReg.state_code)).FirstOrDefault();
                    data_org.registered_state = (vPostcode_Reg == null) ? Guid.Empty : vPostcode_Reg.town_idx;
                }
                else
                {
                    var vPostcode_reg = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.companyr_postcode)).FirstOrDefault();
                    data_org.registered_city = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.town_idx;
                    data_org.registered_state = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.state_idx;
                }
                //data_org.cosec_name = model_org.cosec_name;
                //data_org.cosec_addr_1 = model_org.cosec_addr1;
                //data_org.cosec_addr_2 = model_org.cosec_addr2;
                //data_org.cosec_addr_3 = model_org.cosec_addr3;
                //data_org.cosec_postcode = model_org.cosec_postcode;

                //// var stateSec = model_org.cosec_state;
                //var vStateSec = unitOfWork.RefGeoStatesRepository.Find(i => (i.ssm_code.Contains(model_org.cosec_state))).FirstOrDefault();
                //data_org.cosec_state = (vStateSec == null) ? Guid.Empty : vStateSec.state_idx;
                //if (vStateSec != null)
                //{
                //    var vPostcode_Sec = unitOfWork.VwGeoListRepository.Find(i => (i.town_name == model_org.cosec_town && i.state_code == vStateSec.state_code)).FirstOrDefault();
                //    data_org.cosec_state = (vPostcode_Sec == null) ? Guid.Empty : vPostcode_Sec.town_idx;
                //}
                //else
                //{
                //    var vPostcode_cosec = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.cosec_postcode)).FirstOrDefault();
                //    data_org.cosec_city = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.town_idx;
                //    data_org.cosec_state = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.state_idx;
                //}
                data_org.modified_by = user.user_idx;
                data_org.modified_dt = DateTime.Now;
                var data_FI = new TourlistDataLayer.DataModel.core_organization_financial_info();
                data_FI.organization_financial_info_idx = Guid.NewGuid();
                data_FI.organization_ref = data_org.organization_idx;

                string sFinancialYear = model_org.financial_year_end;
                string stabling_date = model_org.tabling_date;
                if (sFinancialYear != null && sFinancialYear != "")
                {
                    DateTime dFinancialYear = DateTime.ParseExact(sFinancialYear, "dd/MM/yyyy", provider);
                    data_FI.financial_year_date = dFinancialYear;
                }
                if (sFinancialYear != null && sFinancialYear != "")
                {
                    DateTime dtabling_date = DateTime.ParseExact(stabling_date, "dd/MM/yyyy", provider);
                    data_FI.tabling_date = dtabling_date;
                }

                if (model_org.is_unqualified_report == "Y")
                    data_FI.is_unqualified_report = 1;
                else
                    data_FI.is_unqualified_report = 0;

                if (model_org.is_consilidated_account == "Y")
                    data_FI.is_consilidated_account = 1;
                else
                    data_FI.is_consilidated_account = 0;

                data_FI.non_current_assets = Decimal.Parse(model_org.non_current_assets);
                data_FI.current_assets = Decimal.Parse(model_org.current_assets);
                data_FI.non_current_liabilities = Decimal.Parse(model_org.non_current_liabilities);
                data_FI.current_liabilities = Decimal.Parse(model_org.current_liabilities);
                data_FI.share_capital = Decimal.Parse(model_org.share_capital);
                data_FI.reserve = Decimal.Parse(model_org.reserve);
                data_FI.retain_earning = Decimal.Parse(model_org.retain_earning);
                data_FI.bal_minority_interest = Decimal.Parse(model_org.bal_minority_interest);
                data_FI.revenue = Decimal.Parse(model_org.revenue);
                data_FI.profit_lost_before_tax = Decimal.Parse(model_org.profit_lost_before_tax);
                data_FI.profit_lost_after_tax = Decimal.Parse(model_org.profit_lost_after_tax);
                data_FI.net_dividend = Decimal.Parse(model_org.net_dividend);
                data_FI.income_minority_interest = Decimal.Parse(model_org.income_minority_interest);
                data_FI.active_status = 1;
                data_FI.created_at = DateTime.Now;
                data_FI.created_by = user.user_idx;
                data_FI.modified_at = DateTime.Now;
                data_FI.modified_by = user.user_idx;

                unitOfWork.CoreOrganizationFinancialInfoRepository.Add(data_FI);

            }
            if (person == null && organization != null)
            {
                organization.organization_name = model_org.company_name;
                organization.old_organization_name = model_org.company_oldname;
                organization.organization_identifier = model_org.company_newregno;

                string sIncorporation = model_org.company_incdate;
                DateTime dIncorporation = DateTime.ParseExact(sIncorporation, "dd/MM/yyyy", provider);
                organization.incorporation_date = dIncorporation;
                organization.is_has_business_address = (short?)((model_org.companyb_addr1 != "") ? 1 : 0);
                organization.office_addr_1 = model_org.companyb_addr1;
                organization.office_addr_2 = model_org.companyb_addr2;
                organization.office_addr_3 = model_org.companyb_addr3;
                organization.office_postcode = model_org.companyb_postcode;
                var vStateOff = unitOfWork.RefGeoStatesRepository.Find(i => (i.ssm_code.Contains(model_org.companyb_state))).FirstOrDefault();
                organization.office_state = (vStateOff == null) ? Guid.Empty : vStateOff.state_idx;

                if (vStateOff != null)
                {
                    var vPostcode_Office = unitOfWork.VwGeoListRepository.Find(i => (i.town_name == model_org.companyb_town && i.state_code == vStateOff.state_code)).FirstOrDefault();
                    organization.office_city = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.town_idx;
                }
                else
                {
                    var vPostcode_Office = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.companyb_postcode)).FirstOrDefault();
                    organization.office_city = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.town_idx;
                    organization.office_state = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.state_idx;
                }
                organization.nature_of_business = model_org.nature_of_business;
                organization.registered_addr_1 = model_org.companyr_addr1;
                organization.registered_addr_2 = model_org.companyr_addr2;
                organization.registered_addr_3 = model_org.companyr_addr3;
                organization.registered_postcode = model_org.companyr_postcode;

                var vStateReg = unitOfWork.RefGeoStatesRepository.Find(i => (i.ssm_code.Contains(model_org.companyr_state))).FirstOrDefault();

                organization.registered_state = (vStateReg == null) ? Guid.Empty : vStateReg.state_idx;

                if (vStateReg != null)
                {
                    var vPostcode_Reg = unitOfWork.VwGeoListRepository.Find(i => (i.town_name == model_org.companyr_town && i.state_code == vStateReg.state_code)).FirstOrDefault();
                    organization.registered_state = (vPostcode_Reg == null) ? Guid.Empty : vPostcode_Reg.town_idx;
                }
                else
                {
                    var vPostcode_reg = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.companyr_postcode)).FirstOrDefault();
                    organization.registered_city = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.town_idx;
                    organization.registered_state = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.state_idx;
                }

                //organization.cosec_name = model_org.cosec_name;
                ////organization.cosec_no = model_org.cosec_no;
                //organization.cosec_addr_1 = model_org.cosec_addr1;
                //organization.cosec_addr_2 = model_org.cosec_addr2;
                //organization.cosec_addr_3 = model_org.cosec_addr3;
                //organization.cosec_postcode = model_org.cosec_postcode;

                //var vStateSec = unitOfWork.RefGeoStatesRepository.Find(i => (i.ssm_code.Contains(model_org.cosec_state))).FirstOrDefault();

                //organization.cosec_state = (vStateSec == null) ? Guid.Empty : vStateSec.state_idx;

                //if (vStateSec != null)
                //{
                //    var vPostcode_Sec = unitOfWork.VwGeoListRepository.Find(i => (i.town_name == model_org.cosec_town && i.state_code == vStateSec.state_code)).FirstOrDefault();
                //    organization.cosec_state = (vPostcode_Sec == null) ? Guid.Empty : vPostcode_Sec.town_idx;
                //}
                //else
                //{
                //    var vPostcode_cosec = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.cosec_postcode)).FirstOrDefault();
                //    organization.cosec_city = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.town_idx;
                //    organization.cosec_state = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.state_idx;
                //}

                var financialInfo = unitOfWork.CoreOrganizationFinancialInfoRepository.Find(i => (i.organization_ref == organization.organization_idx)).FirstOrDefault();

                if (financialInfo == null)
                {
                    var data_FI = new TourlistDataLayer.DataModel.core_organization_financial_info();

                    data_FI.organization_financial_info_idx = Guid.NewGuid();
                    data_FI.organization_ref = organization.organization_idx;
                    data_FI.auditor_organization_ref = Guid.Empty;
                    string sFinancialYear = model_org.financial_year_end;
                    string stabling_date = model_org.tabling_date;
                    if (sFinancialYear != null && sFinancialYear != "")
                    {
                        DateTime dFinancialYear = DateTime.ParseExact(sFinancialYear, "dd/MM/yyyy", provider);
                        data_FI.financial_year_date = dFinancialYear;
                    }
                    if (sFinancialYear != null && sFinancialYear != "")
                    {
                        DateTime dtabling_date = DateTime.ParseExact(stabling_date, "dd/MM/yyyy", provider);
                        data_FI.tabling_date = dtabling_date;
                    }
                    if (model_org.is_unqualified_report == "Y")
                        data_FI.is_unqualified_report = 1;
                    else
                        data_FI.is_unqualified_report = 0;

                    if (model_org.is_consilidated_account == "Y")
                        data_FI.is_consilidated_account = 1;
                    else
                        data_FI.is_consilidated_account = 0;

                    data_FI.non_current_assets = Decimal.Parse(model_org.non_current_assets);
                    data_FI.current_assets = Decimal.Parse(model_org.current_assets);
                    data_FI.non_current_liabilities = Decimal.Parse(model_org.non_current_liabilities);
                    data_FI.current_liabilities = Decimal.Parse(model_org.current_liabilities);
                    data_FI.share_capital = Decimal.Parse(model_org.share_capital);
                    data_FI.reserve = Decimal.Parse(model_org.reserve);
                    data_FI.retain_earning = Decimal.Parse(model_org.retain_earning);
                    data_FI.bal_minority_interest = Decimal.Parse(model_org.bal_minority_interest);
                    data_FI.revenue = Decimal.Parse(model_org.revenue);
                    data_FI.profit_lost_before_tax = Decimal.Parse(model_org.profit_lost_before_tax);
                    data_FI.profit_lost_after_tax = Decimal.Parse(model_org.profit_lost_after_tax);
                    data_FI.net_dividend = Decimal.Parse(model_org.net_dividend);
                    data_FI.income_minority_interest = Decimal.Parse(model_org.income_minority_interest);
                    data_FI.active_status = 1;
                    data_FI.created_at = DateTime.Now;
                    data_FI.created_by = user.user_idx;
                    data_FI.modified_at = DateTime.Now;
                    data_FI.modified_by = user.user_idx;
                    unitOfWork.CoreOrganizationFinancialInfoRepository.Add(data_FI);
                }
                else
                {
                    string sFinancialYear = model_org.financial_year_end;
                    string stabling_date = model_org.tabling_date;
                    if (sFinancialYear != null && sFinancialYear != "")
                    {
                        DateTime dFinancialYear = DateTime.ParseExact(sFinancialYear, "dd/MM/yyyy", provider);
                        financialInfo.financial_year_date = dFinancialYear;
                    }
                    if (sFinancialYear != null && sFinancialYear != "")
                    {
                        DateTime dtabling_date = DateTime.ParseExact(stabling_date, "dd/MM/yyyy", provider);
                        financialInfo.tabling_date = dtabling_date;
                    }

                    if (model_org.is_unqualified_report == "Y")
                        financialInfo.is_unqualified_report = 1;
                    else
                        financialInfo.is_unqualified_report = 0;

                    if (model_org.is_consilidated_account == "Y")
                        financialInfo.is_consilidated_account = 1;
                    else
                        financialInfo.is_consilidated_account = 0;

                    // data_FI.is_consilidated_account = Int32.Parse(model_org.is_consilidated_account);

                    financialInfo.non_current_assets = Decimal.Parse(model_org.non_current_assets);
                    financialInfo.current_assets = Decimal.Parse(model_org.current_assets);
                    financialInfo.non_current_liabilities = Decimal.Parse(model_org.non_current_liabilities);
                    financialInfo.current_liabilities = Decimal.Parse(model_org.current_liabilities);
                    financialInfo.share_capital = Decimal.Parse(model_org.share_capital);
                    financialInfo.reserve = Decimal.Parse(model_org.reserve);
                    financialInfo.retain_earning = Decimal.Parse(model_org.retain_earning);
                    financialInfo.bal_minority_interest = Decimal.Parse(model_org.bal_minority_interest);
                    financialInfo.revenue = Decimal.Parse(model_org.revenue);
                    financialInfo.profit_lost_before_tax = Decimal.Parse(model_org.profit_lost_before_tax);
                    financialInfo.profit_lost_after_tax = Decimal.Parse(model_org.profit_lost_after_tax);
                    financialInfo.net_dividend = Decimal.Parse(model_org.net_dividend);
                    financialInfo.income_minority_interest = Decimal.Parse(model_org.income_minority_interest);
                    financialInfo.active_status = 1;

                    financialInfo.modified_at = DateTime.Now;
                    financialInfo.modified_by = user.user_idx;
                    unitOfWork.CoreOrganizationFinancialInfoRepository.TourlistContext.Entry(financialInfo).State = EntityState.Modified;
                }

            }


            //chklistitems.bool1 = status;
            unitOfWork.Complete();
            return "success";
        }
        public string ajaxUpdateCompanyDirectorsSSM(string module_id, string component_id, TobtabViewModels.tobtab_ssm_directors model_org)
        {

            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            var chklist = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.chklist_instance_idx.ToString() == chklistitems.chklist_instance_ref.ToString())).First();
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == chklist.application_ref.ToString())).First();
            var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
            var user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == license.created_by.ToString())).First();

            var person = unitOfWork.Persons.Find(i => (i.person_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();

            var dir_person = unitOfWork.Persons.Find(i => (i.person_identifier.ToString() == model_org.director_docno.ToString())).FirstOrDefault();

            var genderMale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "MALE")).FirstOrDefault();
            var genderFemale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "FEMALE")).FirstOrDefault();

            if (dir_person == null)
            {
                var data_person = new TourlistDataLayer.DataModel.core_persons();
                data_person.person_idx = Guid.NewGuid();
                data_person.person_identifier = model_org.director_docno;
                data_person.person_name = model_org.director_name;
                data_person.contact_addr_1 = model_org.director_addr1;
                data_person.contact_addr_2 = model_org.director_addr2;
                data_person.contact_addr_3 = model_org.director_addr3;
                data_person.contact_postcode = model_org.director_postcode;

                //var contact_city = unitOfWork.RefGeoTownsRepository.Find(i => (i.town_name.ToLower() == model_org.director_town.ToLower())).FirstOrDefault();
                //data_person.contact_city = (contact_city == null) ? Guid.Empty : contact_city.town_idx;

                var vPostcode_cosec = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.director_postcode)).FirstOrDefault();
                data_person.contact_city = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.town_idx;
                data_person.contact_state = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.state_idx;

                data_person.residential_addr_1 = model_org.director_addr1;
                data_person.residential_addr_2 = model_org.director_addr2;
                data_person.residential_addr_3 = model_org.director_addr3;
                data_person.residential_postcode = model_org.director_postcode;
                data_person.residential_city = data_person.contact_city;
                data_person.residential_state = data_person.contact_state;
                data_person.person_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                data_person.created_dt = DateTime.Now;
                data_person.created_by = user.user_idx;
                data_person.modified_dt = DateTime.Now;
                data_person.modified_by = user.user_idx;

                if (model_org.director_designation == "D")
                    data_person.person_employ_position = "DIRECTOR";
                else if (model_org.director_designation == "M")
                    data_person.person_employ_position = "MANAGER";
                else if (model_org.director_designation == "S")
                    data_person.person_employ_position = "SECRETARY";

                if (model_org.director_idType == "MK")
                {
                    var icNo = model_org.director_docno;
                    int lastIcNo = int.Parse(icNo.Substring(icNo.Length - 1, 1));
                    if (lastIcNo % 2 == 0)
                    {
                        data_person.person_gender = genderFemale.ref_idx;
                    }
                    else
                    {
                        data_person.person_gender = genderMale.ref_idx;
                    }

                    DateTime d = DateTime.ParseExact(icNo.Substring(0, 6), "yyMMdd", CultureInfo.InvariantCulture);


                    data_person.person_birthday = d;

                }

                var data_org = new TourlistDataLayer.DataModel.core_organization_directors();

                data_org.organization_director_idx = Guid.NewGuid();
                data_org.organization_ref = organization.organization_idx;
                data_org.person_ref = data_person.person_idx;
                //data_org.date_appointed = DateTime.Parse(model_org.director_date_appointed); //
                data_org.is_executive = (short)((model_org.director_designation != "") ? 1 : 0);
                data_org.active_status = 1;
                data_org.created_at = DateTime.Now;
                data_org.created_by = user.user_idx;
                data_org.modified_at = DateTime.Now;
                data_org.modified_by = user.user_idx;

                unitOfWork.Persons.Add(data_person);
                unitOfWork.CoreOrganizationDirectorsRepository.Add(data_org);

            }

            if (dir_person != null)
            {
                var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => (i.organization_ref == organization.organization_idx && i.person_ref.ToString() == dir_person.person_idx.ToString())).FirstOrDefault();

                dir_person.person_identifier = model_org.director_docno;
                dir_person.person_name = model_org.director_name;
                dir_person.contact_addr_1 = model_org.director_addr1;
                dir_person.contact_addr_2 = model_org.director_addr2;
                dir_person.contact_addr_3 = model_org.director_addr3;
                dir_person.contact_postcode = model_org.director_postcode;

                //var contact_city = unitOfWork.RefGeoTownsRepository.Find(i => (i.town_name.ToLower() == model_org.director_town.ToLower())).FirstOrDefault();
                //data_person.contact_city = (contact_city == null) ? Guid.Empty : contact_city.town_idx;

                var vPostcode_cosec = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == model_org.director_postcode)).FirstOrDefault();
                dir_person.contact_city = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.town_idx;
                dir_person.contact_state = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.state_idx;

                dir_person.residential_addr_1 = model_org.director_addr1;
                dir_person.residential_addr_2 = model_org.director_addr2;
                dir_person.residential_addr_3 = model_org.director_addr3;
                dir_person.residential_postcode = model_org.director_postcode;
                dir_person.residential_city = dir_person.contact_city;
                dir_person.residential_state = dir_person.contact_state;

                dir_person.person_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                dir_person.created_dt = DateTime.Now;
                dir_person.created_by = user.user_idx;
                dir_person.modified_dt = DateTime.Now;
                dir_person.modified_by = user.user_idx;

                if (director == null)
                {

                    var data_org = new TourlistDataLayer.DataModel.core_organization_directors();
                    data_org.organization_director_idx = Guid.NewGuid();
                    data_org.organization_ref = organization.organization_idx;
                    data_org.person_ref = dir_person.person_idx;
                    //data_org.date_appointed = DateTime.Parse(model_org.director_date_appointed);
                    data_org.is_executive = (short)((model_org.director_designation != "") ? 1 : 0);
                    data_org.active_status = 1;
                    data_org.created_at = DateTime.Now;
                    data_org.created_by = user.user_idx;
                    data_org.modified_at = DateTime.Now;
                    data_org.modified_by = user.user_idx;
                    unitOfWork.CoreOrganizationDirectorsRepository.Add(data_org);
                }
                else
                {
                    director.organization_ref = organization.organization_idx;
                    director.person_ref = dir_person.person_idx;
                    //director.date_appointed = DateTime.Parse(model_org.director_date_appointed);
                    director.is_executive = (short)((model_org.director_designation != "") ? 1 : 0);
                    director.active_status = 1;
                    director.created_at = DateTime.Now;
                    director.created_by = user.user_idx;
                    director.modified_at = DateTime.Now;
                    director.modified_by = user.user_idx;
                    unitOfWork.CoreOrganizationDirectorsRepository.Update(director);
                }
            }

            unitOfWork.Complete();
            return "success";
        }

        public string ajaxUpdateCompanyShareholdersSSM(string module_id, string component_id, TobtabViewModels.tobtab_ssm_shareholders model_org)
        {
            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            var chklist = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.chklist_instance_idx.ToString() == chklistitems.chklist_instance_ref.ToString())).First();
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == chklist.application_ref.ToString())).First();
            var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
            var user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == license.created_by.ToString())).First();

            var person = unitOfWork.Persons.Find(i => (i.person_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();

            var sh_person = unitOfWork.Persons.Find(i => (i.person_identifier.ToString() == model_org.shareholder_docno.ToString())).FirstOrDefault();
            var sh_organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_identifier.ToString() == model_org.shareholder_docno.ToString())).FirstOrDefault();

            var genderMale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "MALE")).FirstOrDefault();
            var genderFemale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "FEMALE")).FirstOrDefault();


            if (model_org.shareholder_idType == "C")
            {
                if (sh_organization == null)
                {
                    var data_org = new TourlistDataLayer.DataModel.core_organizations();
                    data_org.organization_idx = Guid.NewGuid();
                    data_org.organization_identifier = model_org.shareholder_docno;
                    data_org.organization_name = model_org.shareholder_name;

                    data_org.organization_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                    data_org.created_dt = DateTime.Now;
                    data_org.created_by = user.user_idx;
                    data_org.modified_dt = DateTime.Now;
                    data_org.modified_by = user.user_idx;

                    unitOfWork.CoreOrganizations.Add(data_org);

                    unitOfWork.Complete();

                    var sh_org = unitOfWork.CoreOrganizations.Find(i => (i.organization_identifier.ToString() == model_org.shareholder_docno.ToString())).FirstOrDefault();



                    var data_orgShare = new TourlistDataLayer.DataModel.core_organization_shareholders();

                    data_orgShare.organization_shareholder_idx = Guid.NewGuid();
                    data_orgShare.organization_ref = organization.organization_idx;
                    data_orgShare.shareholder_organization_ref = sh_org.organization_idx;

                    data_orgShare.number_of_shares = Decimal.Parse(model_org.shareholder_totalshare);
                    data_orgShare.active_status = 1;
                    data_orgShare.created_at = DateTime.Now;
                    data_orgShare.created_by = user.user_idx;
                    data_orgShare.modified_at = DateTime.Now;
                    data_orgShare.modified_by = user.user_idx;


                    unitOfWork.CoreOrganizationShareholders.Add(data_orgShare);
                }
                else
                {
                    var shareholder = unitOfWork.CoreOrganizationShareholders.Find(i => (i.shareholder_organization_ref.ToString() == organization.organization_idx.ToString())).FirstOrDefault();

                    organization.organization_identifier = model_org.shareholder_docno;
                    organization.organization_name = model_org.shareholder_name;

                    organization.organization_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                    organization.created_dt = DateTime.Now;
                    organization.created_by = user.user_idx;
                    organization.modified_dt = DateTime.Now;
                    organization.modified_by = user.user_idx;
                    if (shareholder == null)
                    {
                        var data_org = new TourlistDataLayer.DataModel.core_organization_shareholders();

                        data_org.organization_shareholder_idx = Guid.NewGuid();
                        data_org.organization_ref = organization.organization_idx;
                        data_org.shareholder_organization_ref = organization.organization_idx;

                        data_org.number_of_shares = Decimal.Parse(model_org.shareholder_totalshare);

                        data_org.active_status = 1;
                        data_org.created_at = DateTime.Now;
                        data_org.created_by = user.user_idx;
                        data_org.modified_at = DateTime.Now;
                        data_org.modified_by = user.user_idx;

                        unitOfWork.CoreOrganizationShareholders.Add(data_org);
                        unitOfWork.Complete();
                    }
                    else
                    {
                        shareholder.organization_ref = organization.organization_idx;
                        //  shareholder.shareholder_person_ref = sh_person.person_idx;

                        shareholder.number_of_shares = Decimal.Parse(model_org.shareholder_totalshare);
                        shareholder.active_status = 1;
                        shareholder.created_at = DateTime.Now;
                        shareholder.created_by = user.user_idx;
                        shareholder.modified_at = DateTime.Now;
                        shareholder.modified_by = user.user_idx;

                    }

                }
            }
            else
            {
                if (sh_person == null)
                {
                    var data_person = new TourlistDataLayer.DataModel.core_persons();
                    data_person.person_idx = Guid.NewGuid();
                    data_person.person_identifier = model_org.shareholder_docno;
                    data_person.person_name = model_org.shareholder_name;
                    data_person.person_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                    data_person.created_dt = DateTime.Now;
                    data_person.created_by = user.user_idx;
                    data_person.modified_dt = DateTime.Now;
                    data_person.modified_by = user.user_idx;

                    if (model_org.shareholder_idType == "MK")
                    {
                        var icNo = model_org.shareholder_docno;
                        int lastIcNo = int.Parse(icNo.Substring(icNo.Length - 1, 1));
                        if (lastIcNo % 2 == 0)
                        {
                            data_person.person_gender = genderFemale.ref_idx;
                        }
                        else
                        {
                            data_person.person_gender = genderMale.ref_idx;
                        }
                        DateTime d = DateTime.ParseExact(icNo.Substring(0, 6), "yyMMdd", CultureInfo.InvariantCulture);
                        data_person.person_birthday = d;
                    }
                    unitOfWork.CorePersonsRepository.Add(data_person);
                    unitOfWork.Complete();

                    var sh_org = unitOfWork.CorePersonsRepository.Find(i => (i.person_identifier.ToString() == model_org.shareholder_docno.ToString())).FirstOrDefault();
                    var data_org = new TourlistDataLayer.DataModel.core_organization_shareholders();
                    data_org.organization_shareholder_idx = Guid.NewGuid();
                    data_org.organization_ref = organization.organization_idx;
                    data_org.shareholder_person_ref = data_person.person_idx;
                    data_org.number_of_shares = Decimal.Parse(model_org.shareholder_totalshare);
                    data_org.active_status = 1;
                    data_org.created_at = DateTime.Now;
                    data_org.created_by = user.user_idx;
                    data_org.modified_at = DateTime.Now;
                    data_org.modified_by = user.user_idx;
                    // unitOfWork.Persons.Add(data_person);
                    unitOfWork.CoreOrganizationShareholders.Add(data_org);
                    unitOfWork.Complete();
                }
                else
                {
                    var shareholder = unitOfWork.CoreOrganizationShareholders.Find(i => (i.organization_ref == organization.organization_idx && i.shareholder_person_ref.ToString() == sh_person.person_idx.ToString())).FirstOrDefault();
                    sh_person.person_identifier = model_org.shareholder_docno;
                    sh_person.person_name = model_org.shareholder_name;
                    sh_person.person_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                    sh_person.created_dt = DateTime.Now;
                    sh_person.created_by = user.user_idx;
                    sh_person.modified_dt = DateTime.Now;
                    sh_person.modified_by = user.user_idx;
                    if (shareholder == null)
                    {
                        var data_org = new TourlistDataLayer.DataModel.core_organization_shareholders();
                        data_org.organization_shareholder_idx = Guid.NewGuid();
                        data_org.organization_ref = organization.organization_idx;
                        data_org.shareholder_person_ref = sh_person.person_idx;
                        data_org.number_of_shares = Decimal.Parse(model_org.shareholder_totalshare);
                        data_org.active_status = 1;
                        data_org.created_at = DateTime.Now;
                        data_org.created_by = user.user_idx;
                        data_org.modified_at = DateTime.Now;
                        data_org.modified_by = user.user_idx;
                        unitOfWork.CoreOrganizationShareholders.Add(data_org);
                        // unitOfWork.Complete();
                    }
                    else
                    {
                        shareholder.organization_ref = organization.organization_idx;
                        shareholder.shareholder_person_ref = sh_person.person_idx;
                        shareholder.number_of_shares = Decimal.Parse(model_org.shareholder_totalshare);
                        shareholder.active_status = 1;
                        shareholder.created_at = DateTime.Now;
                        shareholder.created_by = user.user_idx;
                        shareholder.modified_at = DateTime.Now;
                        shareholder.modified_by = user.user_idx;
                        unitOfWork.CoreOrganizationShareholders.Update(shareholder);
                    }
                }
            }
            unitOfWork.Complete();
            return "success";
        }


        public List<TobtabViewModels.tobtab_supporting_documents> GetDocumentListInstance(string tobtab_id, string module)
        {

            var chklist = unitOfWork.CoreChklstListsRepository.Find(i => i.chklist_code.ToString() == module).FirstOrDefault();

            var chklistinstance = unitOfWork.CoreChkListInstancesRepository.Find(i => i.application_ref.ToString() == tobtab_id
                                    && i.chklist_tplt_ref.ToString() == chklist.chklist_idx.ToString()).FirstOrDefault();
            var chklistitem = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chklist_instance_ref.ToString() == chklistinstance.chklist_instance_idx.ToString()
                                    && i.active_status == 1)).ToList();

            List<TobtabViewModels.tobtab_supporting_documents> model = new List<TobtabViewModels.tobtab_supporting_documents>();
            foreach (var item in chklistitem)
            {

                var listitem = unitOfWork.CoreChklstItemsRepository.Find(i => (i.item_idx.ToString() == item.chklist_tplt_item_ref.ToString()
                                  )).OrderBy(x => x.orderx).FirstOrDefault();
                if (listitem != null)
                {
                    if (listitem.active_status == 1)
                    {

                        var model_d = new TobtabViewModels.tobtab_supporting_documents();
                        model_d.chkitem_instance_idx = item.chkitem_instance_idx.ToString();
                        model_d.apply_idx = chklistinstance.application_ref.ToString();
                        model_d.chklist_code = module;
                        model_d.bool1 = item.bool1.ToString();
                        model_d.descr_bool1 = listitem.descr_bool1;
                        model_d.orderx = listitem.orderx;
                        model_d.descr_string1 = listitem.descr_string1;
                        model_d.descr_string2 = listitem.descr_string2;
                        model_d.upload_location = item.string2;
                        model_d.file_name = item.string1;
                        model_d.Upload_file_name = item.string1;
                        //model_d.upload_date = "";
                        model_d.supload_date = String.Format("{0:dd/MM/yyyy}", item.date1);

                        model.Add(model_d);
                    }
                }

            }


            return model;
        }

        public string UpdateForeignPartner(TourlistDataLayer.DataModel.tobtab_foreign_partners data)
        {
            tobtab_foreign_partners updateData = unitOfWork.TobtabForeignPartnersRepository.Find(i => i.foreign_partner_idx == data.foreign_partner_idx).FirstOrDefault();
            if (updateData != null)
            {
                updateData.foreign_partner_name = data.foreign_partner_name;
                //updateData.document_upload_name = data.document_upload_name;
                //updateData.document_upload_location = data.document_upload_location;
                updateData.office_addr_1 = data.office_addr_1;
                updateData.office_addr_2 = data.office_addr_2;
                updateData.office_addr_3 = data.office_addr_3;
                updateData.office_country = data.office_country;
                updateData.office_city = data.office_city;
                updateData.office_phone_no = data.office_phone_no;
                updateData.office_postcode = data.office_postcode;
                updateData.office_state = data.office_state;
                //updateData.documents_path = 
                unitOfWork.TobtabForeignPartnersRepository.Update(data);
            }

            unitOfWork.Complete();

            return "success";
        }
        public string AddForeignPartner(TourlistDataLayer.DataModel.tobtab_foreign_partners data)
        {

            unitOfWork.TobtabForeignPartnersRepository.Add(data);

            unitOfWork.Complete();

            return "success";
        }

        public string AddForeignPackages(TourlistDataLayer.DataModel.tobtab_foreign_packages data)
        {

            unitOfWork.TobtabForeignPackagesRepository.Add(data);

            unitOfWork.Complete();

            return "success";
        }

        public string AddNewPermohonanIklan(TourlistDataLayer.DataModel.tobtab_umrah_advertising data)
        {

            var permohonan = unitOfWork.TobtabUmrahAdvertisingRepository.Find(c => (c.tobtab_umrah_advertising_idx == data.tobtab_umrah_advertising_idx)).FirstOrDefault();

            if (permohonan == null)
            {
                data.tobtab_umrah_advertising_idx = Guid.NewGuid();
                unitOfWork.TobtabUmrahAdvertisingRepository.Add(data);
            }
            else
            {
                permohonan.no_lesen = data.no_lesen;
                permohonan.license_expiry_date = data.license_expiry_date;
                permohonan.advertise_title = data.advertise_title;
                permohonan.advertise_terms_conditions = data.advertise_terms_conditions;
                permohonan.trip_schedule = data.trip_schedule;
                permohonan.payment_package_madefrom = data.payment_package_madefrom;
                permohonan.payment_package_madeto = data.payment_package_madeto;
                permohonan.modified_at = DateTime.Now;
                permohonan.modified_by = data.created_by;
                unitOfWork.TobtabUmrahAdvertisingRepository.Update(permohonan);
            }

            unitOfWork.Complete();

            return "success";
        }

        public string UpdateForeignStatus(string license_ref, TourlistDataLayer.DataModel.tobtab_licenses data)
        {
            var licence = unitOfWork.TobtabLicenses.Find(c => (c.tobtab_idx.ToString() == license_ref)).FirstOrDefault();
            licence.foreign_relations = data.foreign_relations;

            unitOfWork.Complete();

            return "success";
        }

        //ref_sequence
        public int GetSeqNoByModule(string module_name)
        {
            int returnVal = 0;
            if (!string.IsNullOrEmpty(module_name))
            {
                var datarow = unitOfWork.RefSequenceRepository.Find(x => x.module_name == module_name).FirstOrDefault();
                if (datarow != null) returnVal = (int)(datarow.number + 1);
            }

            return returnVal;
        }

        public bool UpdateNewSeqNo(string module_name, Guid userIdx)
        {
            int returnVal = 0;
            bool returnResult = false;
            if (!string.IsNullOrEmpty(module_name))
            {
                var datarow = unitOfWork.RefSequenceRepository.Find(x => x.module_name == module_name).FirstOrDefault();
                if (datarow != null)
                {
                    datarow.number = datarow.number + 1;
                    datarow.modified_by = userIdx;
                    datarow.modified_at = DateTime.Now;
                    //unitOfWork.RefSequenceRepository.TourlistContext.Entry(datarow).State = EntityState.Modified;
                    unitOfWork.RefSequenceRepository.Update(datarow);
                    returnVal = unitOfWork.Complete();
                }

            }
            if (returnVal > 0)
            {
                returnResult = true;
            }

            return returnResult;
        }

        public string GetRefSequenceNo(Guid userID, string prefix, string sequence_name, int str_length)
        {
            string sRefNo = "";
            int iRef = this.GetSeqNoByModule(sequence_name);
            bool updateSeq = this.UpdateNewSeqNo(sequence_name, userID);
            if (updateSeq == true)
            {
                int N = iRef; // Number to be pad
                int P = str_length; // Number of leading zeros

                string s = "{0:";
                for (int i = 0; i < P; i++)
                {
                    s += "0";
                }
                s += "}";

                string sNumber = string.Format(s, N);
                sRefNo = prefix + " " + sNumber;
            }
            return sRefNo;
        }
        public int GetTobtabStatusModul(string sOrganizationID, string sModule)
        {
            Guid gOrgID = Guid.Parse(sOrganizationID);
            List<TourlistDataLayer.DataModel.vw_tobtab_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_tobtab_application>();
            clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.organization_ref == gOrgID && c.module_name == sModule)).ToList();

            return clsApplication.Count;
        }
        public int GetTobtabStatusModulStatus(string sOrganizationID, string sModule, string status)
        {
            Guid gOrgID = Guid.Parse(sOrganizationID);
            List<TourlistDataLayer.DataModel.vw_tobtab_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_tobtab_application>();
            var appActive = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.organization_ref == gOrgID)).OrderByDescending(i => i.application_date).FirstOrDefault();
            //clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_idx == gOrgID && c.module_name == sModule)).OrderByDescending(i => i.application_date).ToList();
            if(appActive != null && appActive.ref_code != null && appActive.ref_code == "COMPLETED")
            {
                return 0;
            }
            else
            {
                return 1;
            }

            //return clsApplication.Count;
        }
        public List<core_uploads_freeform_by_persons> GetDocumentPersons(string upload_type)
        {
            var refupload = unitOfWork.RefReferencesRepository.Find(x => x.ref_code == upload_type).FirstOrDefault();
            List<core_uploads_freeform_by_persons> dataList = new List<core_uploads_freeform_by_persons>();
            if (refupload != null)
            {

                dataList = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(x => x.upload_type_ref == refupload.ref_idx).ToList();

            }
            return dataList;
        }


        public List<tobtab_shareholder_documents> GetDocumentsShareholder(string upload_type, string idx)
        {
            var refupload = unitOfWork.RefReferencesRepository.Find(x => x.ref_code == upload_type).FirstOrDefault();
            List<tobtab_shareholder_documents> dataList = new List<tobtab_shareholder_documents>();
            if (refupload != null)
            {

                var person_upload = unitOfWork.CoreUploadsFreeFormPersonRepository
                    .Find(x => x.upload_type_ref == refupload.ref_idx && x.upload_description == idx)
                        .OrderByDescending(x => x.created_at)
                        .Select(s => s.person_ref).Distinct()
                        .ToList();
                foreach (var persons in person_upload)
                {
                    var person = unitOfWork.CorePersonsRepository.Find(x => x.person_idx == persons).FirstOrDefault();
                    var person_upload_data = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(x => x.person_ref == persons).FirstOrDefault();
                    tobtab_shareholder_documents docs = new tobtab_shareholder_documents();
                    docs.person_upload_idx = person_upload_data.uploads_freeform_by_persons_idx.ToString();
                    docs.shareholder_name = person.person_name.ToString();
                    docs.shareholder_identifier = person.person_identifier.ToString();
                    docs.filepath = person_upload_data.upload_path.ToString();
                    docs.upload_name = person_upload_data.upload_name.ToString();
                    docs.upload_fname = person_upload_data.upload_description.ToString();
                    DateTime date = DateTime.Parse(person_upload_data.created_at.ToString(), null);
                    docs.supload_date = date.ToString("dd/MMM/yyyy");
                    dataList.Add(docs);
                }

            }
            return dataList;
        }

        public List<tobtab_shareholder_documents> GetDocumentsShareholderByAppId(string upload_type, string idx, string sOrganization)
        {
            List<vw_core_org_shareholder> shareholders = new List<vw_core_org_shareholder>();
            shareholders = unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_ref.ToString() == sOrganization && x.person_name != null)
               .ToList();
            var refupload = unitOfWork.RefReferencesRepository.Find(x => x.ref_code == upload_type).FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.GetAll().ToList();
            List<tobtab_shareholder_documents> dataList = new List<tobtab_shareholder_documents>();
            /*if (refupload != null)
            {

                var person_upload = unitOfWork.CorePersonFreeUploadsRepository
                    .Find(x => x.upload_type_idx == refupload.ref_idx && x.upload_description == idx)
                        .OrderByDescending(x => x.created_at)
                        .Select(s => s.person_idx).Distinct()
                        .ToList();
                foreach (var persons in person_upload)
                {
                    var person = unitOfWork.CorePersonsRepository.Find(x => x.person_idx == persons).FirstOrDefault();
                    var person_upload_data = unitOfWork.CorePersonFreeUploadsRepository.Find(x => x.person_idx == persons).FirstOrDefault();
                    tobtab_shareholder_documents docs = new tobtab_shareholder_documents();
                    docs.person_upload_idx = person_upload_data.person_upload_idx.ToString();
                    docs.shareholder_name = person.person_name.ToString();
                    docs.shareholder_identifier = person.person_identifier.ToString();
                    docs.filepath = person_upload_data.upload_path.ToString();
                    docs.upload_name = person_upload_data.upload_name.ToString();
                    docs.upload_fname = person_upload_data.upload_description.ToString();
                    DateTime date = DateTime.Parse(person_upload_data.created_at.ToString(), null);
                    docs.supload_date = date.ToString("dd/MMM/yyyy");
                    dataList.Add(docs);
                }

            }*/
            var clsDirector = (from shareholder in shareholders
                               from upload in clsUploadPerson
                               .Where(d => d.person_ref == shareholder.person_idx)
                               .DefaultIfEmpty() // <== makes join left join  
                               select new
                               {
                                   name = shareholder.person_name,
                                   indentifier = shareholder.person_identifier,
                                   fileName = upload == null ? "" : upload.upload_name,
                                   locationFile = upload == null ? "" : upload.upload_path
                               }).ToList();

            //List<CoreOrganizationModel.CorePersonDoc> modelList = new List<CoreOrganizationModel.CorePersonDoc>();

            foreach (var dashboard in clsDirector)
            {
                tobtab_shareholder_documents model = new tobtab_shareholder_documents();
                {
                    model.upload_name = dashboard.fileName;
                    model.shareholder_identifier = dashboard.indentifier;
                    model.filepath = dashboard.locationFile;
                    model.shareholder_name = dashboard.name;

                }
                dataList.Add(model);
            }
            return dataList;
        }

              
        public List<TobtabViewModels.tobtab_branch> GetBranchList(string sOrganization)
        {

            var ref_type = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "STATUSAWAM").FirstOrDefault();
            var result = unitOfWork.Complete(); //motacContext.SaveChanges();
            var activeID = unitOfWork.RefReferencesRepository.Find(c => (c.ref_name == "ACTIVE") && c.ref_type == ref_type.ref_idx).FirstOrDefault();
            var cancelID = unitOfWork.RefReferencesRepository.Find(c => (c.ref_name == "CANCEL_PROCESS") && c.ref_type == ref_type.ref_idx).FirstOrDefault();

            var refReferenceTypes = unitOfWork.RefReferencesTypesRepository.Find(c => (c.ref_type_name == "STATUSAWAM")).FirstOrDefault();

            var refReference = unitOfWork.RefReferencesRepository.Find(c => (c.ref_type == refReferenceTypes.ref_idx)).ToList();

            List<TourlistDataLayer.DataModel.tobtab_add_branches> clsBranch = new List<TourlistDataLayer.DataModel.tobtab_add_branches>();
            clsBranch = unitOfWork.TobtabAddBranchesRepository.Find(c => (c.organization_ref.ToString() == sOrganization && (c.active_status == activeID.ref_idx || c.active_status == cancelID.ref_idx))).OrderBy(i => i.branch_license_ref_code).ToList();

            var clsList = (from branch in clsBranch
                           from item in refReference
                            .Where(d => d.ref_idx.ToString() == branch.active_status.ToString())
                                .DefaultIfEmpty() // <== makes join left join  

                           select new
                           {
                               branch_license_ref_code = branch.branch_license_ref_code,
                               branch_addr_1 = branch.branch_addr_1,
                               branch_addr_2 = branch.branch_addr_2,
                               branch_addr_3 = branch.branch_addr_3,
                               branch_mobile_no = branch.branch_mobile_no,
                               branch_email = branch.branch_email,
                               tobtab_add_branches_idx = branch.tobtab_add_branches_idx,
                               status = item.ref_description

                           }).ToList();

            List<TobtabViewModels.tobtab_branch> modelList = new List<TobtabViewModels.tobtab_branch>();

            foreach (var item in clsList)
            {

                TobtabViewModels.tobtab_branch model = new TobtabViewModels.tobtab_branch();
                {
                    model.tobtab_add_branches_idx = item.tobtab_add_branches_idx;
                    model.branch_addr_1 = item.branch_addr_1;
                    model.branch_addr_2 = item.branch_addr_2;
                    model.branch_addr_3 = item.branch_addr_3;
                    model.branch_mobile_no = item.branch_mobile_no;
                    model.branch_email = item.branch_email;
                    model.status = item.status;
                    model.branch_license_ref_code = item.branch_license_ref_code;
                }
                modelList.Add(model);
            }
            return modelList;

        }

        public List<TobtabViewModels.tobtab_branch> GetBranch(string sOrganization, string module)
        {
            var ref_type = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "STATUSAWAM").FirstOrDefault();
            var activeID = unitOfWork.RefReferencesRepository.Find(c => (c.ref_name == "ACTIVE") && c.ref_type == ref_type.ref_idx).FirstOrDefault();
            var draftID = unitOfWork.RefReferencesRepository.Find(c => (c.ref_name == "DRAFT") && c.ref_type == ref_type.ref_idx).FirstOrDefault();
            Guid in_activeID = Guid.Empty;
            if (activeID != null)
            {
                in_activeID = activeID.ref_idx;

            }
            if (module == "TOBTAB_ADD_BRANCH")
            {
                in_activeID = draftID.ref_idx; // new branch
            }
            List<TourlistDataLayer.DataModel.tobtab_add_branches> clsBranch = new List<TourlistDataLayer.DataModel.tobtab_add_branches>();
            clsBranch = unitOfWork.TobtabAddBranchesRepository.Find(c => (c.organization_ref.ToString() == sOrganization && c.active_status == in_activeID)).OrderBy(i => i.branch_license_ref_code).ToList();

            List<TobtabViewModels.tobtab_branch> branchList = new List<TobtabViewModels.tobtab_branch>();

            foreach (var branch in clsBranch)
            {
                TobtabViewModels.tobtab_branch model = new TobtabViewModels.tobtab_branch();
                {
                    model.tobtab_add_branches_idx = branch.tobtab_add_branches_idx;
                    model.tobtab_licenses_idx = branch.tobtab_license_ref.ToString();
                    model.branch_email = branch.branch_email;
                    model.branch_addr_1 = branch.branch_addr_1;
                    model.branch_addr_2 = branch.branch_addr_2 == null ? "" : branch.branch_addr_2;
                    model.branch_addr_3 = branch.branch_addr_3 == null ? "" : branch.branch_addr_3;
                    model.branch_postcode = branch.branch_postcode;
                    model.branch_name = branch.branch_name == null ? "" : branch.branch_name;
                    model.branch_phone_no = branch.branch_phone_no;
                    model.branch_mobile_no = branch.branch_phone_no;
                    model.branch_license_ref_code = branch.branch_license_ref_code;
                    if (branch.branch_city != null)
                    {
                        var city = unitOfWork.VwGeoListRepository.Find(x => x.town_idx == branch.branch_city).FirstOrDefault();
                        if(city!=null && city.town_name != null)
                        {
                            model.branch_city = city.town_name;
                        }
                    }
                    else
                    {
                        model.branch_city = "";
                    }

                    if (branch.branch_state != null)
                    {
                        model.branch_state = unitOfWork.VwGeoListRepository.Find(x => x.state_idx == branch.branch_state).FirstOrDefault().state_name;
                    }
                    else
                    {
                        model.branch_state = "";
                    }

                    branchList.Add(model);
                }
            }
            return branchList;
        }
        public List<TobtabViewModels.tobtab_branch> GetBranch(string sOrganization, string module, string applyIdx)
        {
            var ref_type = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "STATUSAWAM").FirstOrDefault();
            var activeID = unitOfWork.RefReferencesRepository.Find(c => (c.ref_name == "ACTIVE") && c.ref_type == ref_type.ref_idx).FirstOrDefault();
            var draftID = unitOfWork.RefReferencesRepository.Find(c => (c.ref_name == "DRAFT") && c.ref_type == ref_type.ref_idx).FirstOrDefault();
            Guid in_activeID = Guid.Empty;
            if (activeID != null)
            {
                in_activeID = activeID.ref_idx;

            }
            if (module == "TOBTAB_ADD_BRANCH")
            {
                in_activeID = draftID.ref_idx; // new branch
            }
            Guid applyIdxGuid = Guid.Parse(applyIdx);
            List<TourlistDataLayer.DataModel.tobtab_add_branches> clsBranch = new List<TourlistDataLayer.DataModel.tobtab_add_branches>();
            clsBranch = unitOfWork.TobtabAddBranchesRepository.Find(c => (c.organization_ref.ToString() == sOrganization && c.application_stub_ref == applyIdxGuid && c.active_status == in_activeID)).OrderBy(i => i.branch_license_ref_code).ToList();

            List<TobtabViewModels.tobtab_branch> branchList = new List<TobtabViewModels.tobtab_branch>();

            foreach (var branch in clsBranch)
            {
                TobtabViewModels.tobtab_branch model = new TobtabViewModels.tobtab_branch();
                {
                    model.tobtab_add_branches_idx = branch.tobtab_add_branches_idx;
                    model.tobtab_licenses_idx = branch.tobtab_license_ref.ToString();
                    model.branch_email = branch.branch_email;
                    model.branch_addr_1 = branch.branch_addr_1;
                    model.branch_addr_2 = branch.branch_addr_2 == null ? "" : branch.branch_addr_2;
                    model.branch_addr_3 = branch.branch_addr_3 == null ? "" : branch.branch_addr_3;
                    model.branch_postcode = branch.branch_postcode;
                    model.branch_name = branch.branch_name == null ? "" : branch.branch_name;
                    model.branch_phone_no = branch.branch_phone_no;
                    model.branch_mobile_no = branch.branch_phone_no;
                    model.branch_license_ref_code = branch.branch_license_ref_code;
                    if (branch.branch_city != null)
                    {
                        var city = unitOfWork.VwGeoListRepository.Find(x => x.town_idx == branch.branch_city).FirstOrDefault();
                        if (city != null && city.town_name != null)
                        {
                            model.branch_city = city.town_name;
                        }
                    }
                    else
                    {
                        model.branch_city = "";
                    }

                    if (branch.branch_state != null)
                    {
                        model.branch_state = unitOfWork.VwGeoListRepository.Find(x => x.state_idx == branch.branch_state).FirstOrDefault().state_name;
                    }
                    else
                    {
                        model.branch_state = "";
                    }

                    branchList.Add(model);
                }
            }
            return branchList;
        }

        public object GetTobtablicence(string sOrganization)
        {
            var clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.organization_ref.ToString() == sOrganization && c.license_ref_code != null)).OrderByDescending(i => i.actual_begin_date).FirstOrDefault();
            return clsApplication;

        }

        public List<TourlistDataLayer.DataModel.tobtab_add_branches> GetBranchDetail(string branchID)
        {
            List<TourlistDataLayer.DataModel.tobtab_add_branches> clsBranch = new List<TourlistDataLayer.DataModel.tobtab_add_branches>();
            clsBranch = unitOfWork.TobtabAddBranchesRepository.Find(c => (c.tobtab_add_branches_idx.ToString() == branchID)).ToList();
            return clsBranch;
        }

        public List<TobtabViewModels.tobtab_terminate> GetTerminateLicenseHQ(string OrgID)
        {
            List<TourlistDataLayer.DataModel.tobtab_terminate_licenses> clsTerminate = new List<TourlistDataLayer.DataModel.tobtab_terminate_licenses>();
            clsTerminate = unitOfWork.TobtabTerminateLicenseRepository.Find(c => (c.organization_ref.ToString() == OrgID)).OrderByDescending(i => i.created_dt).ToList();

            List<TobtabViewModels.tobtab_terminate> terminateList = new List<TobtabViewModels.tobtab_terminate>();

            foreach (var app in clsTerminate)
            {
                TobtabViewModels.tobtab_terminate model = new TobtabViewModels.tobtab_terminate();
                {
                    ref_references type = unitOfWork.RefReferencesRepository.Find(i => i.ref_idx == app.terminate_type).FirstOrDefault();
                    model.stype = type.ref_description;
                    model.terminate_license_idx = app.terminate_license_idx;
                    model.sterminate_date = app.terminate_date.ToString("yyyy-MM-dd");
                    model.terminate_type = app.terminate_type;
                    model.terminate_reason = app.terminate_reason;                  

                }
                terminateList.Add(model);
                break;
            }
            return terminateList;
        }


        public List<TobtabViewModels.tobtab_terminate> GetTerminateLicenseCaw(string OrgID)
        {
            List<TourlistDataLayer.DataModel.tobtab_terminate_licenses> clsTerminate = new List<TourlistDataLayer.DataModel.tobtab_terminate_licenses>();
            clsTerminate = unitOfWork.TobtabTerminateLicenseRepository.Find(c => (c.organization_ref.ToString() == OrgID)).OrderByDescending(i => i.created_dt).ToList();

            List<TobtabViewModels.tobtab_terminate> terminateList = new List<TobtabViewModels.tobtab_terminate>();

            foreach (var app in clsTerminate)
            {
                TobtabViewModels.tobtab_terminate model = new TobtabViewModels.tobtab_terminate();
                {
                    ref_references type = unitOfWork.RefReferencesRepository.Find(i => i.ref_idx == app.terminate_type).FirstOrDefault();
                    model.stype = type.ref_description;
                    model.terminate_license_idx = app.terminate_license_idx;
                    model.sterminate_date = app.terminate_date.ToString("yyyy-MM-dd");
                    model.terminate_type = app.terminate_type;
                    model.terminate_reason = app.terminate_reason;

                }
                terminateList.Add(model);
                break;
            }
            return terminateList;
        }


        public bool CancelLicense(TobtabViewModels.tobtab_terminate terminate, string sType, string sOrganization)
        {
            try
            {
                tobtab_terminate_licenses new_terminate = new tobtab_terminate_licenses(); //motacContext.core_persons.Create();
                if (sType == "HQ")
                {
                    new_terminate.organization_ref = Guid.Parse(sOrganization);
                    terminate.branch_ref = Guid.Empty;
                }
                else
                {
                    new_terminate.branch_ref = terminate.branch_ref;
                    terminate.organization_ref = Guid.Empty;
                }

                Guid gBranch = Guid.NewGuid();
                new_terminate.terminate_license_idx = gBranch;
                new_terminate.terminate_type = terminate.terminate_type;
                new_terminate.terminate_reason = terminate.terminate_reason;
                new_terminate.terminate_date = terminate.terminate_date;
                new_terminate.created_dt = DateTime.Now;
                new_terminate.modified_dt = DateTime.Now;
                new_terminate.created_by = Guid.Parse(terminate.user_id);
                new_terminate.modified_by = Guid.Parse(terminate.user_id);
                unitOfWork.TobtabTerminateLicenseRepository.Add(new_terminate);

                unitOfWork.Complete(); //motacContext.SaveChanges();

                var type = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "STATUSAWAM").FirstOrDefault();

                var upd_active = unitOfWork.RefReferencesRepository.Find(c => c.ref_name == "INACTIVE" && c.ref_type== type.ref_idx).FirstOrDefault();

                Guid in_activeID = Guid.Empty;
                if (upd_active != null)
                {
                    in_activeID = upd_active.ref_idx;
                }

                if (sType == "HQ")
                {
                    List<TourlistDataLayer.DataModel.tobtab_add_branches> clsBranch = new List<TourlistDataLayer.DataModel.tobtab_add_branches>();
                    clsBranch = unitOfWork.TobtabAddBranchesRepository.Find(c => c.organization_ref.ToString() == sOrganization).ToList();
                    foreach (var item in clsBranch)
                    {
                        item.active_status = in_activeID;
                        item.modified_by = Guid.Parse(terminate.user_id);
                        item.modified_dt = DateTime.Now;
                        unitOfWork.TobtabAddBranchesRepository.TourlistContext.Entry(item).State = EntityState.Modified;
                        unitOfWork.Complete();
                    }
                    var update_organization = unitOfWork.CoreOrganizations.Find(c => c.organization_idx.ToString() == sOrganization).FirstOrDefault();
                    update_organization.organization_status = in_activeID;
                    update_organization.modified_by = Guid.Parse(terminate.user_id);
                    update_organization.modified_dt = DateTime.Now;
                    unitOfWork.TobtabAddBranchesRepository.TourlistContext.Entry(update_organization).State = EntityState.Modified;
                    unitOfWork.Complete();
                }
                else
                {
                    var update_branch = unitOfWork.TobtabAddBranchesRepository.Find(c => c.tobtab_add_branches_idx == terminate.branch_ref).FirstOrDefault();
                    update_branch.active_status = in_activeID;
                    update_branch.modified_by = Guid.Parse(terminate.user_id);
                    update_branch.modified_dt = DateTime.Now;
                    unitOfWork.TobtabAddBranchesRepository.TourlistContext.Entry(update_branch).State = EntityState.Modified;
                    unitOfWork.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool updateCancelLicense(TobtabViewModels.tobtab_terminate terminate, string sType, string sOrganization,string sTerminate_idx)
        {
            try
            {
                //  tobtab_terminate_licenses upd_terminate = new tobtab_terminate_licenses(); //motacContext.core_persons.Create();
                var upd_terminate = unitOfWork.TobtabTerminateLicenseRepository.Find(c => c.terminate_license_idx.ToString() == sTerminate_idx).FirstOrDefault();

                if (sType == "HQ")
                {
                    upd_terminate.organization_ref = Guid.Parse(sOrganization);
                    terminate.branch_ref = Guid.Empty;
                }
                else
                {
                    upd_terminate.branch_ref = terminate.branch_ref;
                    terminate.organization_ref = Guid.Empty;
                }

                ref_references type = unitOfWork.RefReferencesRepository.Find(i => i.ref_idx == terminate.terminate_type).FirstOrDefault();
                if (type.ref_description=="LAIN-LAIN")
                {
                    upd_terminate.terminate_reason = terminate.terminate_reason;
                }
                else
                {
                    upd_terminate.terminate_reason = "";
                }


                upd_terminate.terminate_type = terminate.terminate_type;
               
                upd_terminate.terminate_date = terminate.terminate_date;            
                upd_terminate.modified_dt = DateTime.Now;                
                upd_terminate.modified_by = Guid.Parse(terminate.user_id);
    
                unitOfWork.TobtabTerminateLicenseRepository.TourlistContext.Entry(upd_terminate).State = EntityState.Modified;
                unitOfWork.Complete();
              
                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "INACTIVE").FirstOrDefault();

                Guid in_activeID = Guid.Empty;
                if (upd_active != null)
                {
                    in_activeID = upd_active.status_idx;
                }

                if (sType == "HQ")
                {
                    List<TourlistDataLayer.DataModel.tobtab_add_branches> clsBranch = new List<TourlistDataLayer.DataModel.tobtab_add_branches>();
                    clsBranch = unitOfWork.TobtabAddBranchesRepository.Find(c => c.organization_ref.ToString() == sOrganization).ToList();
                    foreach (var item in clsBranch)
                    {
                        item.active_status = in_activeID;
                        item.modified_by = Guid.Parse(terminate.user_id);
                        item.modified_dt = DateTime.Now;
                        unitOfWork.TobtabAddBranchesRepository.TourlistContext.Entry(item).State = EntityState.Modified;
                        unitOfWork.Complete();
                    }
                    var update_organization = unitOfWork.CoreOrganizations.Find(c => c.organization_idx.ToString() == sOrganization).FirstOrDefault();
                    update_organization.organization_status = in_activeID;
                    update_organization.modified_by = Guid.Parse(terminate.user_id);
                    update_organization.modified_dt = DateTime.Now;
                    unitOfWork.TobtabAddBranchesRepository.TourlistContext.Entry(update_organization).State = EntityState.Modified;
                    unitOfWork.Complete();
                }
                else
                {
                    var update_branch = unitOfWork.TobtabAddBranchesRepository.Find(c => c.tobtab_add_branches_idx == terminate.branch_ref).FirstOrDefault();
                    update_branch.active_status = in_activeID;
                    update_branch.modified_by = Guid.Parse(terminate.user_id);
                    update_branch.modified_dt = DateTime.Now;
                    unitOfWork.TobtabAddBranchesRepository.TourlistContext.Entry(update_branch).State = EntityState.Modified;
                    unitOfWork.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool duplicateFlowApplicationStub(Guid userId)
        {
            
            Boolean duplicate = false;
            Guid TgExemption = Guid.Parse("A65BC8C3-4023-481F-BD71-AA7174698D04");
            Guid reNew = Guid.Parse("B68F3D73-0D3C-414A-BBCF-73205986CB6B");
            try
            {
                var queryresult = (from tobtab in unitOfWork.TobtabLicenses.Find(i => i.created_by == userId).ToList()
                                   join stub in unitOfWork.FlowApplicationStubs.Find(i => i.apply_user == userId).ToList()
                                   on tobtab.stub_ref equals stub.apply_idx
                                   join reff in unitOfWork.RefReferencesRepository.GetAll()
                                   on stub.apply_status equals reff.ref_idx
                                   where stub.apply_user == userId && reff.ref_code == "DRAFT" && stub.spip == 0 && stub.apply_module != TgExemption
                                   select new applicationStub()
                                   {
                                       apply_idx = stub.apply_idx,
                                       apply_user = stub.apply_user,
                                   }).ToList();
                if (queryresult.Count > 0)
                {
                    duplicate = true;
                }

                return duplicate;
            }
            catch (Exception ex)
            {
                return duplicate;
            }
        }

        public class applicationStub
        {
            public Guid apply_idx { get; set; }
            public Guid apply_user { get; set; }
        }

        public core_uploads_freeform_by_persons UpdateUserProfilePic(core_uploads_freeform_by_persons objReference)
        {
            var ma = TourlistUnitOfWork.RefReferencesTypesRepository.Find(x => x.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var ma_ref = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ma.ref_idx && x.ref_code == "MARKETING_AGENT").FirstOrDefault();
            var ma1 = ma_ref != null ? ma_ref.ref_idx : Guid.Empty;

            /*int returnVal = 0;*/
            var editRow = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(x => x.person_ref == objReference.person_ref).Where(x => x.upload_type_ref == ma1).FirstOrDefault();

            /*RefHelper refHelper = new RefHelper();*/

            if (editRow != null)
            {
                editRow.person_ref = objReference.person_ref;
                editRow.upload_path = objReference.upload_path;
                editRow.modified_by = objReference.modified_by;
                editRow.modified_at = objReference.modified_at;
                editRow.upload_name = objReference.upload_name;
                editRow.upload_type_ref = objReference.upload_type_ref;
                editRow.upload_description = objReference.upload_description;

                /*unitOfWork.CorePersonsRepository.TourlistContext.Entry(editRow).State = EntityState.Modified;*/
                unitOfWork.CoreUploadsFreeFormPersonRepository.Update(editRow);
            }
            else
            {
                editRow = new core_uploads_freeform_by_persons();
                editRow.uploads_freeform_by_persons_idx = Guid.NewGuid();
                editRow.person_ref = objReference.person_ref;
                editRow.upload_path = objReference.upload_path;
                editRow.created_by = objReference.modified_by;
                editRow.created_at = objReference.modified_at;
                editRow.modified_by = objReference.modified_by;
                editRow.modified_at = objReference.modified_at;
                editRow.upload_name = objReference.upload_name;
                editRow.upload_type_ref = objReference.upload_type_ref;

                editRow.upload_description = objReference.upload_description;
                unitOfWork.CoreUploadsFreeFormPersonRepository.Add(editRow);
            }
            unitOfWork.Complete();

            return editRow;
        }

        public core_persons UpdatePersonPath(core_persons objReference)
        {
            var ma = TourlistUnitOfWork.RefReferencesTypesRepository.Find(x => x.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var ma_ref = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ma.ref_idx && x.ref_code == "MARKETING_AGENT").FirstOrDefault();
            var ma1 = ma_ref != null ? ma_ref.ref_idx : Guid.Empty;

            /*int returnVal = 0;*/
            var editRow = unitOfWork.CorePersonsRepository.Find(x => x.person_idx == objReference.person_idx).FirstOrDefault();

            /*RefHelper refHelper = new RefHelper();*/

            
            editRow.person_idx = objReference.person_idx;
            editRow.person_photo_upload = objReference.person_photo_upload;
            editRow.person_id_upload = objReference.person_id_upload;
            editRow.modified_by = objReference.modified_by;
            editRow.modified_dt = objReference.modified_dt;
                
            /*unitOfWork.CorePersonsRepository.TourlistContext.Entry(editRow).State = EntityState.Modified;*/
            unitOfWork.CorePersonsRepository.Update(editRow);
            
            
            unitOfWork.Complete();

            return editRow;
        }

        public tobtab_marketing_agents GetMarketingAgent(string marketing_agent_idx)
        {
            var agent = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.marketing_agent_idx.ToString() == marketing_agent_idx)).FirstOrDefault();
            return agent;
        }

        public core_uploads_freeform_by_persons GetUploadFreeformByDataIdx(string marketing_agent_idx)
        {
            var ma = TourlistUnitOfWork.RefReferencesTypesRepository.Find(x => x.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var ma_ref = TourlistUnitOfWork.RefReferencesRepository.Find(x => x.ref_type == ma.ref_idx && x.ref_code == "MARKETING_AGENT").FirstOrDefault();
            var ma1 = ma_ref != null ? ma_ref.ref_idx : Guid.Empty;

            var agent = unitOfWork.TobtabMarketingAgentRepository.Find(c => (c.marketing_agent_idx.ToString() == marketing_agent_idx)).FirstOrDefault();
            var UploadsFreeFormPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.person_ref == agent.person_ref).Where(x => x.upload_type_ref == ma1).FirstOrDefault();

            return UploadsFreeFormPerson;
        }
        public core_uploads_freeform_by_persons GetUploadFreeform(Guid idx)
        {
            var user = unitOfWork.CoreUsersRepository.Find(i => i.user_idx == idx).FirstOrDefault();
            var UploadsFreeFormPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.person_ref == user.person_ref).FirstOrDefault();

            return UploadsFreeFormPerson;
        }

        public List<TobtabViewModels.tobtab_application> GetTobtabAddBranchStatus(string sAppID)
        {

            List<TourlistDataLayer.DataModel.vw_tobtab_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_tobtab_application>();

            clsApplication = unitOfWork.VwTobtabApplicationRepository.Find(c => (c.apply_idx.ToString() == sAppID)).ToList();

            List<TobtabViewModels.tobtab_application> modelList = new List<TobtabViewModels.tobtab_application>();

            foreach (var app in clsApplication)
            {
                TobtabViewModels.tobtab_application model = new TobtabViewModels.tobtab_application();
                {
                    
                    model.module_name = app.module_name;
                    model.status = app.status;
                    model.apply_idx = app.apply_idx;
                 //   model.m = app.modules_idx;
                    model.status_code = app.ref_code;
                    model.application_no = app.application_no;
                    model.Application_date = app.application_date.ToString();
                    
                }
                modelList.Add(model);
            }

            return modelList;


        }

    }

}
