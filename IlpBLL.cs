using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourlistBusinessLayer.Models;
using TourlistDataLayer.Persistence;
using TourlistDataLayer.DataModel;
using System.Globalization;
using System.Data.Entity;
using TourlistDataLayer.ViewModels.Ilp;
using TourlistDataLayer.ViewModels.Tobtab;

namespace TourlistBusinessLayer.BLL
{
    public class IlpBLL : BusinessLayerBaseClass
    {

        private TourlistUnitOfWork unitOfWork
        {
            get
            {
                return this.TourlistUnitOfWork;
            }
        }

        public void CreateLesenBaharu(Guid user_idx, String license_type, String name, String document, String module_name)
        {
            var organization = GetUserOrganization(user_idx);
            var refType = unitOfWork.RefReferencesTypesRepository.GetReferenceTypeByName("STATUSAWAM");
            var status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "DRAFT");
            var module = unitOfWork.CoreModules.GetCoreModuleIdxByName(module_name);
            unitOfWork.RefSequenceRepository.UpdateSequenceNumber("ILP_REF_NO");
            var refSequence = unitOfWork.RefSequenceRepository.GetByModuleName("ILP_REF_NO");
            var applicationStub = unitOfWork.FlowApplicationStubs.SaveNewApplication(status.ref_idx, user_idx, module, (int)refSequence.number);

            //license checklist
            var checklist_idx = unitOfWork.CoreChklstListsRepository.GetChklistIdxByName(name);
            var chklistInstance = unitOfWork.CoreChkListInstancesRepository.SaveNewChecklistInstance(checklist_idx, applicationStub.apply_idx, user_idx);


            var newLicense = unitOfWork.IlpLicenses.SaveNewLicense(applicationStub.apply_idx, user_idx, license_type, organization.organization_idx, chklistInstance.chklist_instance_idx);

            var checklist_items_idx = unitOfWork.CoreChklstItemsRepository.GetChklistItemsIdxByChklistIdx(checklist_idx);
            foreach (var checklist_item in checklist_items_idx)
            {
                unitOfWork.CoreChkItemsInstancesRepository.SaveNewChecklistItemsInstance(checklist_item.item_idx, chklistInstance.chklist_instance_idx, user_idx);
            }

            //supporting document checklist
            var document_checklist_idx = unitOfWork.CoreChklstListsRepository.GetChklistIdxByName(document);
            var document_chklistInstance = unitOfWork.CoreChkListInstancesRepository.SaveNewChecklistInstance(document_checklist_idx, (Guid)newLicense.supporting_document_list, user_idx);

            var document_checklist_items_idx = unitOfWork.CoreChklstItemsRepository.GetChklistItemsIdxByChklistIdx(document_checklist_idx);
            foreach (var document_checklist_item in document_checklist_items_idx)
            {
                unitOfWork.CoreChkItemsInstancesRepository.SaveNewChecklistItemsInstance(document_checklist_item.item_idx, document_chklistInstance.chklist_instance_idx, user_idx);
            }
        }

        public ilp_licenses getIlpLicense(string ilpIdx)
        {
            var ilpLicenseData = unitOfWork.IlpLicenses.Find(i => i.ilp_idx.ToString() == ilpIdx).FirstOrDefault();
            return ilpLicenseData;
        }
        public ILPLicenseLists GetPermohonanBaharu(Guid user_idx, String license_type)
        {
            core_sol_modules solModule = unitOfWork.CoreSolModulesRepository.Find(i => i.module_name == license_type).FirstOrDefault();
            core_users user = unitOfWork.CoreUsersRepository.Find(i => i.user_idx == user_idx).FirstOrDefault();
           // var ilpLicense = unitOfWork.IlpLicenses.GetIlpLicenseByGuid((Guid)user.user_organization, solModule.modules_idx.ToString());
            var applicationStub = unitOfWork.FlowApplicationStubs.Find(x => x.apply_user == user_idx && x.apply_module == solModule.modules_idx).OrderByDescending(x => x.application_date).FirstOrDefault();
            ILPLicenseLists ilpLicenseList = new ILPLicenseLists();
            if (applicationStub != null)
            {
                var ilpLicense = unitOfWork.IlpLicenses.Find(x => x.stub_ref == applicationStub.apply_idx).FirstOrDefault();
                //var applicationStub = unitOfWork.FlowApplicationStubs.GetApplication(ilpLicense.stub_ref);
                var status = unitOfWork.RefReferencesRepository.GetReferenceByIdx(applicationStub.apply_status);
                var module = unitOfWork.CoreModules.GetCoreModuleByIdx(applicationStub.apply_module).module_desc;
                if (ilpLicense != null)
                {
                    ilpLicenseList.ilp_idx = ilpLicense.ilp_idx;
                    ilpLicenseList.stub_ref = ilpLicense.stub_ref;
                    ilpLicenseList.license_ref_code = ilpLicense.license_ref_code;
                    ilpLicenseList.license_type = ilpLicense.license_type;
                    ilpLicenseList.organization_ref = (Guid)ilpLicense.organization_ref;
                    if (ilpLicense.supporting_document_list != null)
                        ilpLicenseList.supporting_document_list = (Guid)ilpLicense.supporting_document_list;
                    if (ilpLicense.active_status != null)
                        ilpLicenseList.active_status = (int)ilpLicense.active_status;
                    ilpLicenseList.created_at = (DateTime)ilpLicense.created_at;
                    ilpLicenseList.application_no = applicationStub.application_no;
                    ilpLicenseList.application_status = status.ref_description;
                    ilpLicenseList.application_status_code = status.ref_code;
                    ilpLicenseList.application_module = module;
                    ilpLicenseList.application_date = (DateTime)applicationStub.application_date;
                }
            }
            return ilpLicenseList;
        }

        public ILPLicenseLists GetAddBranchByStubRef(String stub_ref)
        {
        
            var applicationStub = unitOfWork.FlowApplicationStubs.Find(x => x.apply_idx.ToString() == stub_ref).FirstOrDefault();
            ILPLicenseLists ilpLicenseList = new ILPLicenseLists();
            if (applicationStub != null)
            {
                var ilpLicense = unitOfWork.IlpLicenses.Find(x => x.stub_ref == applicationStub.apply_idx).FirstOrDefault();
                //var applicationStub = unitOfWork.FlowApplicationStubs.GetApplication(ilpLicense.stub_ref);
                var status = unitOfWork.RefReferencesRepository.GetReferenceByIdx(applicationStub.apply_status);
                var module = unitOfWork.CoreModules.GetCoreModuleByIdx(applicationStub.apply_module).module_desc;
                if (ilpLicense != null)
                {
                    ilpLicenseList.ilp_idx = ilpLicense.ilp_idx;
                    ilpLicenseList.stub_ref = ilpLicense.stub_ref;
                    ilpLicenseList.license_ref_code = ilpLicense.license_ref_code;
                    ilpLicenseList.license_type = ilpLicense.license_type;
                    ilpLicenseList.organization_ref = (Guid)ilpLicense.organization_ref;
                    if (ilpLicense.supporting_document_list != null)
                        ilpLicenseList.supporting_document_list = (Guid)ilpLicense.supporting_document_list;

                    ilpLicenseList.active_status = (int)ilpLicense.active_status;
                    ilpLicenseList.created_at = (DateTime)ilpLicense.created_at;
                    ilpLicenseList.application_no = applicationStub.application_no;
                    ilpLicenseList.application_status = status.ref_description;
                    ilpLicenseList.application_status_code = status.ref_code;
                    ilpLicenseList.application_module = module;
                    ilpLicenseList.application_date = (DateTime)applicationStub.application_date;
                }
            }
            return ilpLicenseList;
        }

        public string getLicenseStatus(Guid user_idx, String license_type)
        {
            core_sol_modules solModule = unitOfWork.CoreSolModulesRepository.Find(i => i.module_name == license_type).FirstOrDefault();
            core_users user = unitOfWork.CoreUsersRepository.Find(i => i.user_idx == user_idx).FirstOrDefault();
            /* var ilpLicense = unitOfWork.IlpLicenses.GetIlpLicenseByGuid((Guid)user.user_organization, solModule.modules_idx.ToString());
             var status = "";
             if (ilpLicense != null && ilpLicense.stub_ref != null)
             {
                 var applicationStub = unitOfWork.FlowApplicationStubs.GetApplication(ilpLicense.stub_ref);
                 if (applicationStub != null)
                     status = unitOfWork.RefReferencesRepository.GetReferenceByIdx(applicationStub.apply_status).ref_description;
             }*/
            var applicationStub = unitOfWork.FlowApplicationStubs.Find(x => x.apply_user == user_idx && x.apply_module == solModule.modules_idx).FirstOrDefault();
            var status = "";
            if (applicationStub != null)
                status = unitOfWork.RefReferencesRepository.GetReferenceByIdx(applicationStub.apply_status).ref_description;

            return status;
        }

        public string GetApplicationStatusCode(Guid apply_idx)
        {
            var statusCode = "";
            var application = unitOfWork.FlowApplicationStubs.Find(a => a.apply_idx == apply_idx).FirstOrDefault();
            if (application != null)
            {
                var reference = unitOfWork.RefReferencesRepository.Find(r => r.ref_idx == application.apply_status).FirstOrDefault();
                statusCode = reference.ref_code;
            }

            return statusCode;
        }

        public string GetLicenseStatusCode(Guid apply_idx)
        {
            var statusCode = "";
            var application = unitOfWork.IlpLicenses.Find(a => a.ilp_idx == apply_idx).FirstOrDefault();

            var appl = unitOfWork.FlowApplicationStubs.Find(a => a.apply_idx == application.stub_ref).FirstOrDefault();
            if (appl != null)
            {
                var reference = unitOfWork.RefReferencesRepository.Find(r => r.ref_idx == appl.apply_status).FirstOrDefault();
                statusCode = reference.ref_code;
            }


            return statusCode;
        }

        public ILPLicenseLists GetPermohonanBaharuByIdx(Guid application_ref)
        {
            var ilpLicense = unitOfWork.IlpLicenses.GetIlpLicenseByIdx(application_ref);
            ILPLicenseLists ilpLicenseList = new ILPLicenseLists();
            if (ilpLicense != null)
            {
                ilpLicenseList.ilp_idx = ilpLicense.ilp_idx;
                ilpLicenseList.stub_ref = ilpLicense.stub_ref;
                ilpLicenseList.license_ref_code = ilpLicense.license_ref_code;
                ilpLicenseList.license_type = ilpLicense.license_type;
                ilpLicenseList.organization_ref = (Guid)ilpLicense.organization_ref;
                ilpLicenseList.supporting_document_list = (Guid)ilpLicense.supporting_document_list;
                ilpLicenseList.active_status = (int)ilpLicense.active_status;
                ilpLicenseList.created_at = (DateTime)ilpLicense.created_at;
            }
            return ilpLicenseList;
        }

        public ILPLicenseLists GetRenewalDuration(Guid license_ref)
        {
            var ilpLicense = unitOfWork.IlpLicenses.GetIlpLicenseByIdx(license_ref);
            ILPLicenseLists ilpLicenseList = new ILPLicenseLists();
            if (ilpLicense != null)
            {
                ilpLicenseList.renewal_duration = ilpLicense.renewal_duration != null ? (int)ilpLicense.renewal_duration : 0;
            }
            return ilpLicenseList;
        }

        public ILPTerminateLicense GetTerminateLicense(Guid license_ref)
        {
            var terminateLicense = unitOfWork.IlpTerminateLicenses.GetTerminateLicenseByLicenseRef(license_ref);
            ILPTerminateLicense ilpTerminateLicense = new ILPTerminateLicense();
            ilpTerminateLicense.terminate_license_idx = terminateLicense.terminate_license_idx;
            ilpTerminateLicense.license_ref = terminateLicense.license_ref;
            ilpTerminateLicense.terminate_type = terminateLicense.terminate_type;
            ilpTerminateLicense.terminate_reason = (Guid)terminateLicense.terminate_reason;
            ilpTerminateLicense.terminate_date = (DateTime)terminateLicense.terminate_date;
            return ilpTerminateLicense;
        }

        public List<ILPTerminateBranch> GetTerminateBranch(Guid terminate_license_ref)
        {
            var branches = unitOfWork.IlpTerminateBranches.GetTerminateBranchesByTerminateLicenseRef(terminate_license_ref);
            var ilpTerminateBranches = new List<ILPTerminateBranch>();
            foreach (var branch in branches)
            {
                ILPTerminateBranch ilpTerminateBranch = new ILPTerminateBranch();
                ilpTerminateBranch.terminate_branch_idx = branch.terminate_branch_idx;
                ilpTerminateBranch.terminate_license_ref = branch.terminate_license_ref;
                ilpTerminateBranch.branch_ref = branch.branch_ref;
                ilpTerminateBranches.Add(ilpTerminateBranch);
            }
            return ilpTerminateBranches;
        }

        public List<ILPMultiSelect> GetMultiSelect(Guid parent_ref)
        {
            var selects = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef(parent_ref);
            var ilpMultiSelects = new List<ILPMultiSelect>();
            foreach (var select in selects)
            {
                ILPMultiSelect ilpMultiSelect = new ILPMultiSelect();
                ilpMultiSelect.multi_select_idx = select.multi_select_idx;
                ilpMultiSelect.parent_ref = select.parent_ref;
                ilpMultiSelect.details_ref = select.details_ref;
                ilpMultiSelects.Add(ilpMultiSelect);
            }
            return ilpMultiSelects;
        }

        public List<ChecklistLists> GetIlpChecklist(Guid application_ref, string chklistCode, string status_code)
        {
            var checklist_items_instances = new List<ChecklistLists>();
            core_chklst_lists chklst = unitOfWork.CoreChklstListsRepository.Find(i => i.chklist_code == chklistCode).FirstOrDefault();
            if (chklst != null)
            {
                var checklist_idx = unitOfWork.CoreChkListInstancesRepository.GetChecklistInstanceIdxByAppRef(application_ref, chklst.chklist_idx);
                var datas = unitOfWork.CoreChkItemsInstancesRepository.GetChklistItemsInstancesByChecklistIdx(checklist_idx);
                foreach (var data in datas)
                {
                    var chklstItems = unitOfWork.CoreChklstItemsRepository.GetChklistItemsByTpltItemRef(data.chklist_tplt_item_ref);
                    if (chklstItems != null)
                    {
                        ChecklistLists checklistLists = new ChecklistLists();
                        checklistLists.chkitem_instance_idx = data.chkitem_instance_idx;
                        checklistLists.chklist_tplt_item_ref = data.chklist_tplt_item_ref;
                        checklistLists.chklist_instance_ref = data.chklist_instance_ref;
                        checklistLists.bool1 = (int)data.bool1;
                        checklistLists.order = chklstItems.orderx;
                        checklistLists.description = chklstItems.descr_bool1;
                        checklistLists.page = chklstItems.descr_string1;
                        checklistLists.modal = chklstItems.descr_string2;
                        checklistLists.string1 = data.string1;
                        if (data.date1.HasValue) checklistLists.date1 = (DateTime)data.date1;
                        checklistLists.upload_location = data.upload_location;
                        checklist_items_instances.Add(checklistLists);

                        if (checklistLists.description == "Dokumen Sokongan" && checklistLists.bool1 == 1 && status_code == "PREMISE_PREPARE")
                        {
                            updateListingFalse(checklistLists.chkitem_instance_idx);
                            checklistLists.bool1 = 0;
                        }

                    }
                }
            }
            return checklist_items_instances;
        }

        public List<ChecklistLists> GetIlpChecklistDokumen(Guid checklist_idx)
        {
            //var checklist_idx = unitOfWork.CoreChkListInstancesRepository.GetChecklistInstanceIdxByAppRef(application_ref);
            var checklist_items_instances = new List<ChecklistLists>();
            var datasfff = unitOfWork.CoreChkItemsInstancesRepository.GetChklistItemsInstancesByChecklistIdx(checklist_idx);
            int numbering = 1;
            var datas = (from chkInstance in unitOfWork.CoreChkItemsInstancesRepository.Find(i => i.chklist_instance_ref == checklist_idx)
                         join chkItem in unitOfWork.CoreChklstItemsRepository.GetAll()
                               on chkInstance.chklist_tplt_item_ref equals chkItem.item_idx
                         where chkInstance.chklist_instance_ref == checklist_idx
                         orderby chkItem.orderx ascending
                         select new core_chkitems_instances()
                         {
                             chkitem_instance_idx = chkInstance.chkitem_instance_idx,
                             chklist_tplt_item_ref = chkInstance.chklist_tplt_item_ref,
                             chklist_instance_ref = chkInstance.chklist_instance_ref,
                             string1 = chkInstance.string1,
                             upload_location = chkInstance.upload_location,
                             bool1 = chkInstance.bool1,
                             date1 = chkInstance.date1,
                         }).ToList();
            foreach (var data in datas)
            {
                var chklstItems = unitOfWork.CoreChklstItemsRepository.GetChklistItemsByTpltItemRef(data.chklist_tplt_item_ref);
                if (chklstItems != null)
                {
                    ChecklistLists checklistLists = new ChecklistLists();
                    checklistLists.chkitem_instance_idx = data.chkitem_instance_idx;
                    checklistLists.chklist_tplt_item_ref = data.chklist_tplt_item_ref;
                    checklistLists.chklist_instance_ref = data.chklist_instance_ref;
                    checklistLists.bool1 = (int)data.bool1;
                    checklistLists.descr_bool2 = chklstItems.descr_bool2;
                    checklistLists.order = chklstItems.orderx;
                    checklistLists.number = numbering;
                    checklistLists.description = chklstItems.descr_bool1;
                    checklistLists.page = chklstItems.descr_string1;
                    checklistLists.modal = chklstItems.descr_string2;
                    checklistLists.string1 = data.string1;
                    if (data.date1.HasValue) checklistLists.date1 = (DateTime)data.date1;
                    checklistLists.upload_location = data.upload_location;
                    checklist_items_instances.Add(checklistLists);
                    numbering = numbering + 1;

                }
            }
            return checklist_items_instances;
        }

        public bool updateListingFalse(Guid Idx)
        {
            try
            {
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

        public List<core_uploads_freeform_by_modules> GetIlpUploads(Guid application_ref, String type)
        {
            var ilp_uploads = new List<core_uploads_freeform_by_modules>();
            var ilpLicense = unitOfWork.IlpLicenses.GetIlpLicenseByIdx(application_ref);
            var datas = unitOfWork.CoreUploadsFreeformModulesRepository.GetUploadsByTransactionRef(ilpLicense.stub_ref);
            ilp_uploads = datas;
            return ilp_uploads;
        }

        public List<ChecklistLists> GetDocumentChecklist(String name)
        {
            var chklist_idx = unitOfWork.CoreChklstListsRepository.GetChklistIdxByName(name);
            var checklists = new List<ChecklistLists>();
            var datas = unitOfWork.CoreChklstItemsRepository.GetChklistItemsIdxByChklistIdx(chklist_idx);
            foreach (var data in datas)
            {
                ChecklistLists list = new ChecklistLists();
                list.order = data.orderx;
                list.description = data.descr_bool1;
                list.title = data.descr_bool2;
                checklists.Add(list);
            }
            return checklists;
        }

        public void CreateOrganization(Organization organization, String organization_identifier)
        {
            var user = new core_users();
            core_organizations organizations = new core_organizations();
            organizations.incorporation_date = organization.incorporation_date;
            organizations.office_addr_1 = organization.office_addr_1;
            organizations.office_addr_2 = organization.office_addr_2;
            organizations.office_addr_3 = organization.office_addr_3;
            organizations.office_postcode = organization.office_postcode;
            var vPostcode_Office = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == organization.office_postcode)).FirstOrDefault();
            organizations.office_city = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.town_idx;
            organizations.office_state = (vPostcode_Office == null) ? Guid.Empty : vPostcode_Office.state_idx;

            organizations.registered_addr_1 = organization.registered_addr_1;
            organizations.registered_addr_2 = organization.registered_addr_2;
            organizations.registered_addr_3 = organization.registered_addr_3;
            organizations.registered_postcode = organization.registered_postcode;
            var vPostcode_reg = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == organization.registered_postcode)).FirstOrDefault();
            organizations.registered_city = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.town_idx;
            organizations.registered_state = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.state_idx;

            organizations.cosec_addr_1 = organization.cosec_addr_1;
            organizations.cosec_addr_2 = organization.cosec_addr_2;
            organizations.cosec_addr_3 = organization.cosec_addr_3;
            organizations.cosec_postcode = organization.cosec_postcode;
            organizations.cosec_name = organization.cosec_name;
            var vPostcode_cosec = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == organization.cosec_postcode)).FirstOrDefault();
            organizations.cosec_city = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.town_idx;
            organizations.cosec_state = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.state_idx;
            unitOfWork.CoreOrganizations.UpdateNewOrganization(organizations, organization_identifier, organization.company_newregno);
        }

        public void UpdateOrganization(Organization organization)
        {
            core_organizations organizations = new core_organizations();
            organizations.organization_identifier = organization.organization_identifier;
            organizations.organization_name = organization.organization_name;
            organizations.incorporation_date = organization.incorporation_date;
            organizations.paid_capital = organization.paid_capital;
            organizations.authorized_capital = organization.authorized_capital;
            organizations.is_has_business_address = organization.is_has_business_address;
            organizations.office_addr_1 = organization.office_addr_1;
            organizations.office_addr_2 = organization.office_addr_2;
            organizations.office_addr_3 = organization.office_addr_3;
            organizations.office_postcode = organization.office_postcode;
            organizations.office_city = organization.office_city;
            organizations.office_state = organization.office_state;
            organizations.office_mobile_no = organization.office_mobile_no;
            organizations.office_phone_no = organization.office_phone_no;
            organizations.office_fax_no = organization.office_fax_no;
            organizations.office_email = organization.office_email;
            organizations.office_website = organization.office_website;
            organizations.office_size = organization.office_size;
            organizations.registered_addr_1 = organization.registered_addr_1;
            organizations.registered_addr_2 = organization.registered_addr_2;
            organizations.registered_addr_3 = organization.registered_addr_3;
            organizations.registered_postcode = organization.registered_postcode;
            organizations.registered_city = organization.registered_city;
            organizations.registered_state = organization.registered_state;
            organizations.registered_mobile_no = organization.registered_mobile_no;
            organizations.registered_phone_no = organization.registered_phone_no;
            organizations.registered_fax_no = organization.registered_fax_no;
            organizations.registered_email = organization.registered_email;
            organizations.cosec_name = organization.cosec_name;
            organizations.cosec_addr_1 = organization.cosec_addr_1;
            organizations.cosec_addr_2 = organization.cosec_addr_2;
            organizations.cosec_addr_3 = organization.cosec_addr_3;
            organizations.cosec_postcode = organization.cosec_postcode;
            organizations.cosec_city = organization.cosec_city;
            organizations.cosec_state = organization.cosec_state;
            organizations.cosec_mobile_no = organization.cosec_mobile_no;
            organizations.cosec_phone_no = organization.cosec_phone_no;
            organizations.cosec_fax_no = organization.cosec_fax_no;
            organizations.cosec_email = organization.cosec_email;
            organizations.pbt_ref = organization.pbt_ref;
            unitOfWork.CoreOrganizations.UpdateOrganization(organizations);
        }

        public TourlistDataLayer.DataModel.core_organizations GetOrganization(string userID)
        {
            var user = unitOfWork.CoreUsersRepository.Find(i => (i.user_idx.ToString() == userID.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            return organization;
        }
        public void StoreShareholder(Shareholder shareholder, Guid user_idx)
        {
            var newPerson = unitOfWork.Persons.Find(i => i.person_identifier == shareholder.person_identifier).FirstOrDefault();
            if (newPerson == null)
            {
                core_persons person = new core_persons();
                person.person_name = shareholder.person_name;
                person.person_identifier = shareholder.person_identifier;
                newPerson = unitOfWork.CorePersonsRepository.StoreNewPerson(person, user_idx);
            }
            var organization = unitOfWork.CoreOrganizations.GetOrganization(Guid.Parse(shareholder.organization_idx));
            core_organization_shareholders shareholders = new core_organization_shareholders();
            shareholders.organization_ref = organization.organization_idx;
            shareholders.shareholder_person_ref = newPerson.person_idx;
            shareholders.number_of_shares = shareholder.number_of_shares;
            shareholders.active_status = 1;
            unitOfWork.CoreOrganizationShareholders.StoreNewOrganizationShareholder(shareholders, user_idx);
        }

        public core_organization_shareholders UpdateShareholder(Shareholder shareholder, string isPerson)
        {
            if (isPerson == "true")
            {
                core_persons person = new core_persons();
                person.contact_mobile_no = shareholder.person_mobile_no;
                person.contact_addr_1 = shareholder.person_addr1;
                person.contact_addr_2 = shareholder.person_addr2;
                person.contact_addr_3 = shareholder.person_addr3;
                person.person_age = shareholder.person_age;
                person.person_birthday = shareholder.person_birthday;
                person.person_religion = shareholder.person_religion;
                person.person_gender = shareholder.person_gender;
                person.person_nationality = shareholder.person_nationality;
                person.contact_postcode = shareholder.person_postcode;
                person.contact_city = shareholder.person_city;
                person.contact_state = shareholder.person_state;
                person.person_idx = shareholder.shareholder_person_ref;
                person.person_id_upload = shareholder.person_id_upload;
                UpdateShareholderPerson(person);
            }

            else
            {
                core_organizations organization = new core_organizations();
                //organization.organization_identifier = unitOfWork.CoreOrganizations.Find(x => x.organization_idx == organization.organization_idx).Select(x => x.organization_identifier).FirstOrDefault();
                organization.organization_idx = shareholder.shareholder_organization_ref;
                organization.office_mobile_no = shareholder.organization_mobile_no;
                organization.incorporation_date = shareholder.organization_incorporation_date;
                organization.office_addr_1 = shareholder.organization_addr1;
                organization.office_addr_2 = shareholder.organization_addr2;
                organization.office_addr_3 = shareholder.organization_addr3;
                organization.office_postcode = shareholder.organization_postcode;
                organization.office_city = shareholder.organization_city;
                organization.office_state = shareholder.organization_state;
                organization.country_ref = shareholder.organization_country;
                UpdateShareholderOrganization(organization);
            }


            core_organization_shareholders shareholders = new core_organization_shareholders();
            shareholders.organization_shareholder_idx = shareholder.organization_shareholder_idx;
            shareholders.status_shareholder = shareholder.status_shareholder;
            return unitOfWork.CoreOrganizationShareholders.UpdateShareholder(shareholders);
        }

        public void StoreDirector(Director director, Guid user_idx)
        {
            var newPerson = unitOfWork.Persons.Find(i => i.person_identifier == director.person_identifier).FirstOrDefault();
            if (newPerson == null)
            {
                core_persons person = new core_persons();
                person.person_name = director.person_name;
                person.person_identifier = director.person_identifier;
                person.contact_addr_1 = director.person_addr1;
                person.contact_addr_2 = director.person_addr2;
                person.contact_addr_3 = director.person_addr3;
                person.contact_postcode = director.person_postcode;
                var vPostcode_reg = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == director.person_postcode)).FirstOrDefault();
                person.contact_city = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.town_idx;
                person.contact_state = (vPostcode_reg == null) ? Guid.Empty : vPostcode_reg.state_idx;
                newPerson = unitOfWork.CorePersonsRepository.StoreNewPerson(person, user_idx);
                /*var organization = unitOfWork.CoreOrganizations.GetOrganization(Guid.Parse(director.organization_idx));
                core_organization_directors directors = new core_organization_directors();
                directors.organization_ref = organization.organization_idx;
                directors.person_ref = newPerson.person_idx;
                directors.is_executive = 1;
                directors.active_status = 1;
                unitOfWork.CoreOrganizationDirectorsRepository.StoreNewOrganizationDirector(directors, user_idx);*/
            }
            var organization = unitOfWork.CoreOrganizations.GetOrganization(Guid.Parse(director.organization_idx));
            core_organization_directors directors = new core_organization_directors();
            directors.organization_ref = organization.organization_idx;
            directors.person_ref = newPerson.person_idx;
            directors.is_executive = 1;
            directors.active_status = 1;
            unitOfWork.CoreOrganizationDirectorsRepository.StoreNewOrganizationDirector(directors, user_idx);
        }

        public void UpdateDirector(Director director)
        {
            core_persons person = new core_persons();
            person.contact_mobile_no = director.person_mobile_no;
            person.contact_phone_no = director.person_phone_no;
            person.person_age = director.person_age;
            person.person_birthday = director.person_birthday;
            person.person_nationality = director.person_nationality;
            person.contact_addr_1 = director.person_addr1;
            person.contact_addr_2 = director.person_addr2;
            person.contact_addr_3 = director.person_addr3;
            person.contact_postcode = director.person_postcode;
            person.contact_city = director.person_city;
            person.contact_state = director.person_state;
            person.person_gender = director.person_gender;
            person.person_idx = director.person_ref;
            if (director.person_id_upload != null) person.person_id_upload = director.person_id_upload;
            if (director.person_cert_upload != null) person.person_cert_upload = director.person_cert_upload;
            unitOfWork.CorePersonsRepository.UpdatePerson(person);
        }
        public ref_geo_postcodes GetPostcodeByCode(String code)
        {
            var postcode = unitOfWork.RefGeoPostcodesRepository.GetPostcodeByCode(code);
            if (postcode == null)
            {
                return null;
            }
            return postcode;
        }
        public ref_geo_postcodes GetPostcodeByIdx(Guid postcode_idx)
        {
            var postcode = unitOfWork.RefGeoPostcodesRepository.GetPostcodeByIdx(postcode_idx);
            if (postcode == null)
            {
                return null;
            }
            return postcode;
        }

        public Towns GetTown(String code)
        {
            var postcode = unitOfWork.RefGeoPostcodesRepository.GetPostcodeByCode(code);
            var data = unitOfWork.RefGeoTownsRepository.GetTownByTownRef(postcode.town_ref);
            Towns town = new Towns();
            town.town_idx = data.town_idx;
            town.town_name = data.town_name;
            town.district_ref = data.district_ref;
            return town;
        }

        public States GetState(Guid district_ref)
        {
            var district = unitOfWork.RefGeoDistrictsRepository.GetDistrictByDistrictRef(district_ref);
            var data = unitOfWork.RefGeoStatesRepository.GetStateByStateRef(district.state_ref);
            States state = new States();
            state.state_idx = data.state_idx;
            state.state_name = data.state_name;
            return state;
        }

        public string UpdateCompanyDetails(core_organizations orgData, Guid user_idx)
        {
            var user = unitOfWork.CoreUsersRepository.Find(x => x.user_idx == user_idx).FirstOrDefault();
            var org = unitOfWork.CoreOrganizations.Find(x => x.organization_idx == user.user_organization).FirstOrDefault();

            org.registered_mobile_no = orgData.registered_mobile_no;
            org.registered_phone_no = orgData.registered_phone_no;
            org.registered_fax_no = orgData.registered_fax_no;
            org.registered_email = orgData.registered_email;
            org.cosec_mobile_no = orgData.cosec_mobile_no;
            org.cosec_phone_no = orgData.cosec_phone_no;
            org.cosec_fax_no = orgData.cosec_fax_no;
            org.cosec_email = orgData.cosec_email;
            org.office_mobile_no = orgData.office_mobile_no;
            org.office_phone_no = orgData.office_phone_no;
            org.office_fax_no = orgData.office_fax_no;
            org.office_email = orgData.office_email;
            org.office_size = orgData.office_size;
            org.office_website = orgData.office_website;
            org.nature_of_business = orgData.nature_of_business;
            org.pbt_ref = orgData.pbt_ref;
            unitOfWork.Complete();

            return "success";
        }

        public bool UpdateChecklistStatus(Guid chkitem_instance_idx)
        {
            var status = unitOfWork.CoreChkItemsInstancesRepository.UpdateChklistItemsInstancesByIdx(chkitem_instance_idx);
            return true;
        }


        public bool insertAuditTrailApplication(Guid stub_ref, Guid userid)
        {
            var stubs = unitOfWork.FlowApplicationStubs.Find(i => i.apply_idx == stub_ref).FirstOrDefault();
            #region Audit Trail Application
            common_audit_trail_application commonAuditTrailApp = new common_audit_trail_application();
            commonAuditTrailApp.audit_trail_apps_idx = Guid.NewGuid();
            commonAuditTrailApp.stub_ref = stubs.apply_idx;
            commonAuditTrailApp.module_ref = stubs.apply_module;
            commonAuditTrailApp.status_ref = stubs.apply_status;
            //commonAuditTrailApp.context_ref = ---> for common only
            commonAuditTrailApp.active_status = 1;
            commonAuditTrailApp.created_at = DateTime.Now;
            commonAuditTrailApp.created_by = userid;
            unitOfWork.CommonAuditTrailApplicationRepository.Add(commonAuditTrailApp);
            #endregion

            unitOfWork.Complete();
            return true;
        }

        public bool UpdatePermohonanStatus(Guid stub_ref)
        {
            var refType = unitOfWork.RefReferencesTypesRepository.GetReferenceTypeByName("STATUSAWAM");
            var new_status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "PENDING_PAY_PROCESS");
            var draft_status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "DRAFT");
            unitOfWork.FlowApplicationStubs.UpdateApplyStatus(stub_ref, new_status.ref_idx, draft_status.ref_idx);
            return true;
        }
        public bool UpdatePermohonanInprocess(Guid stub_ref)
        {
            var refType = unitOfWork.RefReferencesTypesRepository.GetReferenceTypeByName("STATUSAWAM");
            var new_status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "IN_PROCESS");
            var draft_status = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "DRAFT");
            unitOfWork.FlowApplicationStubs.UpdateApplyStatus(stub_ref, new_status.ref_idx, draft_status.ref_idx);
            return true;
        }

        public bool UpdateRenewalDuration(Guid license_ref, int renewal_duration)
        {
            var license = unitOfWork.IlpLicenses.UpdateRenewalDuration(license_ref, renewal_duration);
            return true;
        }

        public void StoreAcknowledgement(Acknowledgement acknowledgement, Guid user_idx)
        {
            var module_idx = unitOfWork.CoreModules.GetCoreModuleIdxByName("ILP");
            core_acknowledgements acknowledgements = new core_acknowledgements();
            acknowledgements.module_ref = module_idx;
            acknowledgements.license_type_ref = acknowledgement.license_type_ref;
            acknowledgements.stub_ref = acknowledgement.stub_ref;
            acknowledgements.acknowledge_person_name = acknowledgement.acknowledge_person_name;
            acknowledgements.acknowledge_person_icno = acknowledgement.acknowledge_person_icno;
            acknowledgements.acknowledge_position = acknowledgement.acknowledge_position;
            acknowledgements.acknowledge_organization_name = acknowledgement.acknowledge_organization_name;
            acknowledgements.is_acknowledged = acknowledgement.is_acknowledged;
            acknowledgements.created_at = DateTime.Now;
            acknowledgements.created_by = user_idx;
            acknowledgements.modified_at = DateTime.Now;
            acknowledgements.modified_by = user_idx;
            unitOfWork.CoreAcknowledgementsRepository.StoreNewAcknowledgement(acknowledgements);
        }

        public ilp_terminate_licenses StoreTerminateLicense(ILPTerminateLicense terminateLicense, Guid user_idx)
        {
            ilp_terminate_licenses terminate_license = new ilp_terminate_licenses();
            terminate_license.license_ref = terminateLicense.license_ref;
            terminate_license.terminate_type = terminateLicense.terminate_type;
            terminate_license.terminate_reason = terminateLicense.terminate_reason;
            terminate_license.terminate_date = terminateLicense.terminate_date;
            var data = unitOfWork.IlpTerminateLicenses.SaveNewTerminateLicense(terminate_license, user_idx);

            return data;
        }

        public void DestroyTerminateBranches(Guid parent_ref)
        {
            unitOfWork.IlpTerminateBranches.DestroyTerminateBranches(parent_ref);
        }

        public ilp_terminate_licenses UpdateTerminateLicense(ILPTerminateLicense terminateLicense)
        {
            ilp_terminate_licenses terminate_license = new ilp_terminate_licenses();
            terminate_license.terminate_license_idx = terminateLicense.terminate_license_idx;
            terminate_license.terminate_type = terminateLicense.terminate_type;
            terminate_license.terminate_reason = terminateLicense.terminate_reason;
            terminate_license.terminate_date = terminateLicense.terminate_date;
            var data = unitOfWork.IlpTerminateLicenses.UpdateTerminateLicense(terminate_license);

            return data;
        }

        public void StoreTerminateBranch(ILPTerminateBranch terminateBranch, Guid user_idx)
        {
            ilp_terminate_branches terminate_branch = new ilp_terminate_branches();
            terminate_branch.terminate_license_ref = terminateBranch.terminate_license_ref;
            terminate_branch.branch_ref = terminateBranch.branch_ref;
            var data = unitOfWork.IlpTerminateBranches.SaveNewTerminateBranch(terminate_branch, user_idx);
        }

        public void StoreMultiSelect(ILPMultiSelect multiSelect, Guid user_idx)
        {
            ilp_multi_selects multi_select = new ilp_multi_selects();
            multi_select.parent_ref = multiSelect.parent_ref;
            multi_select.details_ref = multiSelect.details_ref;
            var data = unitOfWork.IlpMultiSelects.SaveNewMultiSelect(multi_select, user_idx);
        }

        public void DestroyMultiSelect(Guid parent_ref)
        {
            unitOfWork.IlpMultiSelects.DestroyMultiSelect(parent_ref);
        }

        public Organization GetOrganization(Guid user_idx)
        {
            var user = unitOfWork.Users.GetUser(user_idx);
            var data = unitOfWork.CoreOrganizations.GetOrganizationByUser((Guid)user.person_ref);
            Organization organization = new Organization();
            organization.organization_idx = data.organization_idx;
            organization.organization_identifier = data.organization_identifier;
            organization.organization_name = data.organization_name;
            if (data.parent_org_idx != null) organization.parent_org_idx = (Guid)data.parent_org_idx;
            if (data.incorporation_date != null) organization.incorporation_date = (DateTime)data.incorporation_date;
            if (data.paid_capital != null) organization.paid_capital = (decimal)data.paid_capital;
            if (data.authorized_capital != null) organization.authorized_capital = (decimal)data.authorized_capital;
            if (data.is_has_business_address != null) organization.is_has_business_address = (short)data.is_has_business_address;
            organization.office_addr_1 = data.office_addr_1;
            organization.office_addr_2 = data.office_addr_2;
            organization.office_addr_3 = data.office_addr_3;
            organization.office_postcode = data.office_postcode;
            if (data.office_city != null) organization.office_city = (Guid)data.office_city;
            if (data.office_state != null) organization.office_state = (Guid)data.office_state;
            organization.office_mobile_no = data.office_mobile_no;
            organization.office_phone_no = data.office_phone_no;
            organization.office_fax_no = data.office_fax_no;
            organization.office_email = data.office_email;
            organization.office_website = data.office_website;
            if (data.office_size != null) organization.office_size = (int)data.office_size;
            organization.nature_of_business = data.nature_of_business;
            organization.registered_addr_1 = data.registered_addr_1;
            organization.registered_addr_2 = data.registered_addr_2;
            organization.registered_addr_3 = data.registered_addr_3;
            organization.registered_postcode = data.registered_postcode;
            if (data.registered_city != null) organization.registered_city = (Guid)data.registered_city;
            if (data.registered_state != null) organization.registered_state = (Guid)data.registered_state;
            organization.registered_mobile_no = data.registered_mobile_no;
            organization.registered_phone_no = data.registered_phone_no;
            organization.registered_fax_no = data.registered_fax_no;
            organization.registered_email = data.registered_email;
            organization.cosec_name = data.cosec_name;
            organization.cosec_addr_1 = data.cosec_addr_1;
            organization.cosec_addr_2 = data.cosec_addr_2;
            organization.cosec_addr_3 = data.cosec_addr_3;
            organization.cosec_postcode = data.cosec_postcode;
            if (data.cosec_city != null) organization.cosec_city = (Guid)data.cosec_city;
            if (data.cosec_state != null) organization.cosec_state = (Guid)data.cosec_state;
            organization.cosec_mobile_no = data.cosec_mobile_no;
            organization.cosec_phone_no = data.cosec_phone_no;
            organization.cosec_fax_no = data.cosec_fax_no;
            organization.cosec_email = data.cosec_email;          
            if (data.pbt_ref != null) organization.pbt_ref = (Guid)data.pbt_ref;
            return organization;
        }

        public Organization GetUserOrganization(Guid user_idx)
        {
            var user = unitOfWork.Users.GetUser(user_idx);
            var data = unitOfWork.CoreOrganizations.GetOrganizationByUser((Guid)user.person_ref);
            Organization organization = new Organization();
            organization.organization_idx = data.organization_idx;
            organization.organization_identifier = data.organization_identifier;
            organization.organization_name = data.organization_name;
            organization.office_addr_1 = data.office_addr_1;
            organization.office_addr_2 = data.office_addr_2;
            organization.office_addr_3 = data.office_addr_3;
            organization.office_postcode = data.office_postcode;
            if (data.office_city.HasValue) organization.office_city = (Guid)data.office_city;
            if (data.office_state.HasValue) organization.office_state = (Guid)data.office_state;
            if (data.paid_capital.HasValue) organization.paid_capital = (decimal)data.paid_capital;

            return organization;
        }

        public ilp_branches StoreILPBranch(ILPBranch ilp_branch, Guid user_idx)
        {
            ilp_branches branch = new ilp_branches();
            branch.ilp_license_idx = ilp_branch.ilp_license_idx;
            branch.paid_capital = ilp_branch.paid_capital;
            branch.authorized_capital = ilp_branch.authorized_capital;
            branch.branch_addr_1 = ilp_branch.branch_addr_1;
            branch.branch_addr_2 = ilp_branch.branch_addr_2;
            branch.branch_addr_3 = ilp_branch.branch_addr_3;
            branch.branch_postcode = ilp_branch.branch_postcode;
            branch.branch_city = ilp_branch.branch_city;
            branch.branch_state = ilp_branch.branch_state;
            branch.branch_mobile_no = ilp_branch.branch_mobile_no;
            branch.branch_phone_no = ilp_branch.branch_phone_no;
            branch.branch_fax_no = ilp_branch.branch_fax_no;
            branch.branch_email = ilp_branch.branch_email;
            branch.branch_website = ilp_branch.branch_website;
            branch.branch_size = ilp_branch.branch_size;
            branch.utility_others = ilp_branch.utility_others;
            branch.organization_ref = ilp_branch.organization_ref;
            branch.pbt_ref=ilp_branch.pbt_ref;
            branch.active_status = Guid.Parse("B35B2A37-79B1-470C-A4D4-FC6EA3AF2436");


            return unitOfWork.IlpBranches.SaveNewIlpBranch(branch, user_idx);
        }

        public ilp_branches UpdateILPBranch(ILPBranch ilp_branch)
        {
            ilp_branches branch = new ilp_branches();
            branch.ilp_branches_idx = ilp_branch.ilp_branch_idx;
            branch.ilp_license_idx = ilp_branch.ilp_license_idx;
            branch.paid_capital = ilp_branch.paid_capital;
            branch.authorized_capital = ilp_branch.authorized_capital;
            branch.branch_addr_1 = ilp_branch.branch_addr_1;
            branch.branch_addr_2 = ilp_branch.branch_addr_2;
            branch.branch_addr_3 = ilp_branch.branch_addr_3;
            branch.branch_postcode = ilp_branch.branch_postcode;
            branch.branch_city = ilp_branch.branch_city;
            branch.branch_state = ilp_branch.branch_state;
            branch.branch_mobile_no = ilp_branch.branch_mobile_no;
            branch.branch_phone_no = ilp_branch.branch_phone_no;
            branch.branch_fax_no = ilp_branch.branch_fax_no;
            branch.branch_email = ilp_branch.branch_email;
            branch.branch_website = ilp_branch.branch_website;
            branch.branch_size = ilp_branch.branch_size;
            branch.utility = ilp_branch.utility;
            branch.utility_others = ilp_branch.utility_others;
            branch.organization_ref = ilp_branch.organization_ref;
            branch.pbt_ref=ilp_branch.pbt_ref;
            branch.active_status = Guid.Parse("B35B2A37-79B1-470C-A4D4-FC6EA3AF2436");

            return unitOfWork.IlpBranches.UpdateIlpBranch(branch);
        }

        public List<ILPBranch> GetIlpBranches(Guid application_ref)
        {
            var ilpBranches = new List<ILPBranch>();
            var datas = unitOfWork.IlpBranches.GetIlpBranchesByApplicationRef(application_ref);
            foreach (var data in datas)
            {
                ILPBranch ilpBranch = new ILPBranch();
                ilpBranch.ilp_branch_idx = data.ilp_branches_idx;
                ilpBranch.branch_name = data.branch_name;
                ilpBranch.branch_addr_1 = data.branch_addr_1;
                ilpBranch.branch_addr_2 = data.branch_addr_2;
                ilpBranch.branch_addr_3 = data.branch_addr_3;
                ilpBranch.branch_postcode = data.branch_postcode;
                ilpBranch.branch_phone_no = data.branch_phone_no;
                ilpBranch.branch_mobile_no = data.branch_mobile_no;
                ilpBranch.branch_state = (Guid)data.branch_state;
                ilpBranch.branch_city = (Guid)data.branch_city;
                ilpBranch.branch_email = data.branch_email;
                ilpBranch.branch_fax_no = data.branch_fax_no;
                ilpBranch.branch_size = data.branch_size;
                ilpBranch.branch_email = data.branch_email;
                ilpBranch.utility = (Guid)data.utility;
                ilpBranch.utility_others = data.utility_others;
                ilpBranch.branch_website = data.branch_website;
                ilpBranch.authorized_capital = (Decimal)data.authorized_capital;
                ilpBranch.paid_capital = (Decimal)data.paid_capital;
                ilpBranch.ilp_license_idx = data.ilp_license_idx;
                ilpBranch.pbt_ref = data.pbt_ref;
                
                var utilities = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.utility);
                var multiUtilities = new List<ILPMultiSelect>();
                foreach (var utility in utilities)
                {
                    ILPMultiSelect multiUtility = new ILPMultiSelect();
                    multiUtility.parent_ref = utility.parent_ref;
                    multiUtility.details_ref = utility.details_ref;
                    multiUtility.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)utility.details_ref).ref_description;
                    multiUtilities.Add(multiUtility);
                }
                ilpBranch.multi_utility = multiUtilities;
                ilpBranches.Add(ilpBranch);
            }
            return ilpBranches;
        }

        //added by samsuri (CR#57259)  on 2 jan 2024
        public List<ILPBranch> GetIlpBranchesActive(Guid Org_ref)
        {
            var ilpBranches = new List<ILPBranch>();
            var datas = unitOfWork.IlpBranches.GetIlpBranchesByOrgID(Org_ref);

            foreach (var data in datas)
            {
                ILPBranch ilpBranch = new ILPBranch();
                ilpBranch.ilp_branch_idx = data.ilp_branches_idx;
                ilpBranch.branch_name = data.branch_name;
                ilpBranch.branch_addr_1 = data.branch_addr_1;
                ilpBranch.branch_addr_2 = data.branch_addr_2;
                ilpBranch.branch_addr_3 = data.branch_addr_3;
                ilpBranch.branch_postcode = data.branch_postcode;
                ilpBranch.branch_phone_no = data.branch_phone_no;
                ilpBranch.branch_mobile_no = data.branch_mobile_no;
                ilpBranch.branch_state = (Guid)data.branch_state;
                ilpBranch.branch_city = (Guid)data.branch_city;
                ilpBranch.branch_email = data.branch_email;
                ilpBranch.branch_fax_no = data.branch_fax_no;
                ilpBranch.branch_size = data.branch_size;
                ilpBranch.branch_email = data.branch_email;
                ilpBranch.utility = data.utility != null ? (Guid)data.utility : Guid.Empty; //modified b samsuri on 8 jan 2024
                ilpBranch.utility_others = data.utility_others;
                ilpBranch.branch_website = data.branch_website;
                ilpBranch.authorized_capital = data.authorized_capital != null ? (Decimal)data.authorized_capital : 0; //modified b samsuri on 8 jan 2024
                ilpBranch.paid_capital = data.paid_capital != null ? (Decimal)data.paid_capital : 0; //modified b samsuri on 8 jan 2024
                ilpBranch.ilp_license_idx = data.ilp_license_idx;
                ilpBranch.pbt_ref = data.pbt_ref;

                ref_status_record statusRecord = unitOfWork.RefStatusRecordRepository.Find(x => x.status_idx == data.active_status).FirstOrDefault();
                var guidActive = statusRecord != null ? statusRecord.status_name : "";
                if (guidActive == "ACTIVE") ilpBranch.active_status = data.active_status;

                //modified b samsuri on 8 jan 2024
                if (ilpBranch.utility != Guid.Empty)
                {
                    var utilities = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.utility);
                    var multiUtilities = new List<ILPMultiSelect>();
                    foreach (var utility in utilities)
                    {
                        ILPMultiSelect multiUtility = new ILPMultiSelect();
                        multiUtility.parent_ref = utility.parent_ref;
                        multiUtility.details_ref = utility.details_ref;
                        multiUtility.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)utility.details_ref).ref_description;
                        multiUtilities.Add(multiUtility);
                    }
                    ilpBranch.multi_utility = multiUtilities;
                }

                ilpBranches.Add(ilpBranch);
            }
            return ilpBranches;
        }
        //added by samsuri (CR#57259) on 10 jan 2024
        private Guid GetILPBranchUploadedDoc(string code, Guid stubRef)
        {
            var ilpLicense = unitOfWork.IlpLicenses.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
            var coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "ILP_TUKAR_STATUS_DOKUMEN")).FirstOrDefault();
            var coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();
            var coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == ilpLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();
            if (coreChkitemsInstances == null) return Guid.Empty;
            return coreChkitemsInstances.chkitem_instance_idx;
        }

        //Added by samsuri (CR#57259)  on 2 Jan 2024
        public List<ILPBranch> GetIlpBranchesbyBranchIdx(Guid Branch_Idx)
        {
            var ilpBranches = new List<ILPBranch>();
            var datas = unitOfWork.IlpBranches.GetIlpBranchesByBranchIdx(Branch_Idx);

            foreach (var data in datas)
            {
                ILPBranch ilpBranch = new ILPBranch();
                ilpBranch.ilp_branch_idx = data.ilp_branches_idx;
                ilpBranch.branch_name = data.branch_name;
                ilpBranch.branch_addr_1 = data.branch_addr_1;
                ilpBranch.branch_addr_2 = data.branch_addr_2;
                ilpBranch.branch_addr_3 = data.branch_addr_3;
                ilpBranch.branch_postcode = data.branch_postcode;
                ilpBranch.branch_phone_no = data.branch_phone_no;
                ilpBranch.branch_mobile_no = data.branch_mobile_no;
                ilpBranch.branch_state = (Guid)data.branch_state;
                ilpBranch.branch_city = (Guid)data.branch_city;
                ilpBranch.branch_email = data.branch_email;
                ilpBranch.branch_fax_no = data.branch_fax_no;
                ilpBranch.branch_size = data.branch_size;
                ilpBranch.branch_email = data.branch_email;
                ilpBranch.utility = data.utility != null ? (Guid)data.utility : Guid.Empty; //modified b samsuri on 8 jan 2024
                ilpBranch.utility_others = data.utility_others;
                ilpBranch.branch_website = data.branch_website;
                ilpBranch.authorized_capital = data.authorized_capital != null ? (Decimal)data.authorized_capital : 0; //modified b samsuri on 8 jan 2024
                ilpBranch.paid_capital = data.paid_capital != null ? (Decimal)data.paid_capital : 0; //modified b samsuri on 8 jan 2024
                var ilpBranchLic = unitOfWork.IlpLicenses.GetIlpLicenseByIdx(data.ilp_license_idx);
                ilpBranch.ilp_license_idx = ilpBranchLic.stub_ref; //use for upload docs
                ilpBranch.pbt_ref = data.pbt_ref;

                var chkitem_instanceSewaBeliPremis = GetILPBranchUploadedDoc("SEWABELI", ilpBranchLic.stub_ref);//"Perjanjian Sewa Beli Premis"
                var perjanjianSewaBeliPremis = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSewaBeliPremis).FirstOrDefault();
                ilpBranch.fileNameSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.string1;
                ilpBranch.fileLocSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.upload_location;

                var chkitem_instancePelanLantai = GetILPBranchUploadedDoc("PELANLANTAI", ilpBranchLic.stub_ref);//"Pelan Lantai Premis Perniagaan"
                var pelanLantaiPremisPerniagaan = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instancePelanLantai).FirstOrDefault();
                ilpBranch.fileNamepelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.string1;
                ilpBranch.fileLocpelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.upload_location;

                var chkitem_instanceGambar = GetILPBranchUploadedDoc("GAMBAR", ilpBranchLic.stub_ref);//"Gambar Bahagian Dalam dan Luar Pejabat (Berwarna)"
                var gambar = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceGambar).FirstOrDefault();
                ilpBranch.fileNameGambarPermis = gambar == null ? "" : gambar.string1;
                ilpBranch.fileLocGambarPermis = gambar == null ? "" : gambar.upload_location;

                //modified b samsuri on 8 jan 2024
                if (ilpBranch.utility != Guid.Empty)
                {
                    var utilities = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.utility);
                    var multiUtilities = new List<ILPMultiSelect>();
                    foreach (var utility in utilities)
                    {
                        ILPMultiSelect multiUtility = new ILPMultiSelect();
                        multiUtility.parent_ref = utility.parent_ref;
                        multiUtility.details_ref = utility.details_ref;
                        multiUtility.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)utility.details_ref).ref_description;
                        multiUtilities.Add(multiUtility);
                    }
                    ilpBranch.multi_utility = multiUtilities;
                }

                var datasUpdated = unitOfWork.IlpBranches.GetIlpBranchesUpdatedByBranchIdx(Branch_Idx);
                var ilpBranchUpdated = new List<ILPBranchUpdated>();
                foreach (var dataUpd in datasUpdated)
                {
                    ILPBranchUpdated ILPBranchUpd = new ILPBranchUpdated();
                    ILPBranchUpd.new_branch_addr_1 = dataUpd.new_branch_addr_1;
                    ILPBranchUpd.new_branch_addr_2 = dataUpd.new_branch_addr_2;
                    ILPBranchUpd.new_branch_addr_3 = dataUpd.new_branch_addr_3;
                    ILPBranchUpd.new_branch_postcode = dataUpd.new_branch_postcode;
                    ILPBranchUpd.new_branch_city = (Guid)dataUpd.new_branch_city;
                    ILPBranchUpd.new_branch_state = (Guid)dataUpd.new_branch_state;

                    ILPBranchUpd.old_branch_addr_1 = dataUpd.old_branch_addr_1;
                    ILPBranchUpd.old_branch_addr_2 = dataUpd.old_branch_addr_2;
                    ILPBranchUpd.old_branch_addr_3 = dataUpd.old_branch_addr_3;
                    ILPBranchUpd.old_branch_postcode = dataUpd.old_branch_postcode;
                    ILPBranchUpd.old_branch_city = (Guid)dataUpd.old_branch_city;
                    ILPBranchUpd.old_branch_state = (Guid)dataUpd.old_branch_state;

                    ilpBranchUpdated.Add(ILPBranchUpd);
                }
                ilpBranch.branch_updated = ilpBranchUpdated;

                ilpBranches.Add(ilpBranch);
            }
            return ilpBranches;
        }

        public List<ILPBranch> GetIlpBranchesByUser(Guid user_idx)
        {
            var ilpBranches = new List<ILPBranch>();
            var datas = unitOfWork.IlpBranches.GetIlpBranchesByUserId(user_idx);
            foreach (var data in datas)
            {
                ILPBranch ilpBranch = new ILPBranch();
                ilpBranch.ilp_branch_idx = data.ilp_branches_idx;
                ilpBranch.branch_addr_1 = data.branch_addr_1;
                ilpBranch.branch_addr_2 = data.branch_addr_2;
                ilpBranch.branch_addr_3 = data.branch_addr_3;
                ilpBranch.branch_postcode = data.branch_postcode;
                ilpBranch.branch_phone_no = data.branch_phone_no;
                ilpBranch.branch_mobile_no = data.branch_mobile_no;
                ilpBranch.branch_state = (Guid)data.branch_state;
                ilpBranch.branch_city = (Guid)data.branch_city;
                ilpBranch.branch_email = data.branch_email;
                ilpBranch.branch_fax_no = data.branch_fax_no;
                ilpBranch.branch_size = data.branch_size;
                ilpBranch.branch_email = data.branch_email;
                if (data.utility != null)
                {
                    ilpBranch.utility = (Guid)data.utility;
                }
                ilpBranch.utility_others = data.utility_others;
                ilpBranch.branch_website = data.branch_website;
                ilpBranch.active_status = data.active_status;
                if (ilpBranch.active_status != null)
                {
                    var activeID = unitOfWork.RefReferencesRepository.Find(c => c.ref_idx == ilpBranch.active_status).FirstOrDefault();
                    ilpBranch.status = activeID.ref_description;
                }
                else
                {
                    ilpBranch.status = "-";
                }
                if (data.authorized_capital != null) { 
                    ilpBranch.authorized_capital = (Decimal)data.authorized_capital;
                }
                if (data.paid_capital != null)
                {
                    ilpBranch.paid_capital = (Decimal)data.paid_capital;
                }
                ilpBranch.ilp_license_idx = data.ilp_license_idx;
                if (data.utility != null)
                {
                    var utilities = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.utility);
                    var multiUtilities = new List<ILPMultiSelect>();
                    foreach (var utility in utilities)
                    {
                        ILPMultiSelect multiUtility = new ILPMultiSelect();
                        multiUtility.parent_ref = utility.parent_ref;
                        multiUtility.details_ref = utility.details_ref;
                        multiUtility.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)utility.details_ref).ref_description;
                        multiUtilities.Add(multiUtility);
                    }
                    ilpBranch.multi_utility = multiUtilities;
                }
                ilpBranches.Add(ilpBranch);
            }
            return ilpBranches;
        }

        public string DeleteIlpBranch(Guid ilp_branch_idx)
        {
            ilp_branches ilp_branch = unitOfWork.IlpBranches.Find(i => i.ilp_branches_idx == ilp_branch_idx).FirstOrDefault();
            unitOfWork.IlpBranches.Remove(ilp_branch);
            unitOfWork.Complete();
            return "success";
        }
        public string DeletePersonQualification(Guid qualification_idx)
        {
            core_person_qualifications qualification = unitOfWork.CorePersonQualificationsRepository.Find(q => q.qualification_idx == qualification_idx).FirstOrDefault();
            unitOfWork.CorePersonQualificationsRepository.Remove(qualification);
            unitOfWork.Complete();
            return "success";
        }
        public string DeletePersonExperience(Guid experience_idx)
        {
            core_person_work_experiences experience = unitOfWork.CorePersonWorkExperiencesRepository.Find(e => e.experience_idx == experience_idx).FirstOrDefault();
            unitOfWork.CorePersonWorkExperiencesRepository.Remove(experience);
            unitOfWork.Complete();
            return "success";
        }
        public string DeletePersonCourse(Guid person_course_idx)
        {
            core_person_courses personCourse = unitOfWork.CorePersonCoursesRepository.Find(c => c.person_course_idx == person_course_idx).FirstOrDefault();
            unitOfWork.CorePersonCoursesRepository.Remove(personCourse);
            unitOfWork.Complete();
            return "success";
        }
        public string DeleteApplication(string appID)
        {
            var licenses = unitOfWork.IlpLicenses.SingleOrDefault(i => (i.ilp_idx.ToString() == appID.ToString()));
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == licenses.stub_ref.ToString())).FirstOrDefault();
            var chklist_Instances = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.application_ref.ToString() == application.apply_idx.ToString())).ToList();
            foreach (var app in chklist_Instances)
            {
                var chklistitems_Instances = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chklist_instance_ref.ToString() == app.chklist_instance_idx.ToString())).ToList();

                unitOfWork.CoreChkItemsInstancesRepository.RemoveRange(chklistitems_Instances);
            }

            unitOfWork.IlpLicenses.Remove(licenses);
            unitOfWork.FlowApplicationStubs.Remove(application);
            unitOfWork.CoreChkListInstancesRepository.RemoveRange(chklist_Instances);
            unitOfWork.Complete();

            return "Success";
        }

        public core_sol_modules GetSolModule(Guid moduleIdx)
        {
            var solModules = unitOfWork.CoreSolModulesRepository.Find(i => i.modules_idx == moduleIdx).FirstOrDefault();
            return solModules;
        }

        public flow_application_stubs GetApplicationStubs(Guid stubRef)
        {
            var applicationStub = unitOfWork.FlowApplicationStubs.GetApplication(stubRef);
            return applicationStub;
        }
        public ilp_licenses GeIlpLicense(Guid stubRef)
        {
            var ilpLicense = unitOfWork.IlpLicenses.GetIlpLicenseByStubRef(stubRef);
            return ilpLicense;
        }
        public List<ILPLicenseLists> GetIlpLicenses(String license_type, Guid user_idx)
        {
            var licenses = new List<ILPLicenseLists>();
            core_sol_modules solModule = unitOfWork.CoreSolModulesRepository.Find(i => i.module_name == license_type).FirstOrDefault();
            var datas = unitOfWork.IlpLicenses.GetIlpLicensesByTypeAndUserId(solModule.modules_idx.ToString(), user_idx);
            foreach (var data in datas)
            {
                var applicationStub = unitOfWork.FlowApplicationStubs.GetApplication(data.stub_ref);
                if (applicationStub != null)
                {
                    var status = unitOfWork.RefReferencesRepository.GetReferenceByIdx(applicationStub.apply_status);
                    var module = unitOfWork.CoreModules.GetCoreModuleByIdx(applicationStub.apply_module).module_desc;

                    ILPLicenseLists license = new ILPLicenseLists();
                    license.ilp_idx = data.ilp_idx;
                    license.stub_ref = data.stub_ref;
                    license.license_type = data.license_type;
                    license.license_ref_code = data.license_ref_code;
                    license.created_at = (DateTime)data.created_at;
                    license.application_no = applicationStub.application_no;
                    license.application_module = module;
                    license.application_status = status.ref_description;
                    license.application_status_code = status.ref_code;

                    licenses.Add(license);
                }
            }
            return licenses;
        }

        public Acknowledgement GetAcknowledgement(Guid license_type_ref)
        {
            var data = unitOfWork.CoreAcknowledgementsRepository.GetAcknowledgement(license_type_ref);
            Acknowledgement acknowledgement = new Acknowledgement();
            if (data != null)
            {
                acknowledgement.acknowledge_person_name = data.acknowledge_person_name;
                acknowledgement.acknowledge_person_icno = data.acknowledge_person_icno;
                acknowledgement.acknowledge_position = data.acknowledge_position;
                acknowledgement.acknowledge_organization_name = data.acknowledge_organization_name;
                acknowledgement.is_acknowledged = (short)data.is_acknowledged;
            }

            return acknowledgement;
        }

        public core_persons CreatePerson(ILPPerson ilpPerson, Guid user_idx)
        {
            core_persons persons = new core_persons();
            persons.person_name = ilpPerson.person_name;
            persons.person_gender = ilpPerson.person_gender;
            persons.person_identifier = ilpPerson.person_identifier;
            persons.person_birthday = ilpPerson.person_birthday;
            persons.contact_addr_1 = ilpPerson.residential_addr_1;
            persons.person_birthplace = ilpPerson.person_birthplace;
            persons.contact_addr_2 = ilpPerson.residential_addr_2;
            persons.person_nationality = ilpPerson.person_nationality;
            persons.contact_addr_3 = ilpPerson.residential_addr_3;
            persons.contact_mobile_no = ilpPerson.contact_mobile_no;
            persons.contact_postcode = ilpPerson.residential_postcode;
            persons.contact_phone_no = ilpPerson.contact_phone_no;
            persons.contact_city = ilpPerson.residential_city;
            persons.person_employ_permit_no = ilpPerson.person_employ_permit_no;
            persons.contact_state = ilpPerson.residential_state;
            persons.person_employ_permit_released_place = ilpPerson.person_employ_permit_released_place;
            persons.person_employ_date_start = ilpPerson.person_employ_date_start;
            persons.person_employ_date_end = ilpPerson.person_employ_date_end;
            persons.person_cert_upload = ilpPerson.person_cert_upload;
            var newPerson = unitOfWork.CorePersonsRepository.StoreNewPerson(persons, user_idx);

            return newPerson;
        }

        public core_persons UpdatePersonUpload(Guid person_idx, Guid upload_idx)
        {
            core_persons persons = new core_persons();
            persons.person_idx = person_idx;
            persons.person_cert_upload = upload_idx.ToString();
            var newPerson = unitOfWork.CorePersonsRepository.UpdatePersonUpload(persons);

            return newPerson;
        }

        public ilp_instructor_courses UpdateInstructorUpload(Guid person_idx, Guid upload_idx)
        {
            ilp_instructor_courses instructor = new ilp_instructor_courses();
            instructor.person_ref = person_idx;
            instructor.course_details_upload = upload_idx.ToString();
            var newPerson = unitOfWork.IlpInstructorCourses.UpdateInstructorUpload(instructor);

            return newPerson;
        }

        public void UpdatePengajarPemohon(ILPPerson ilpPerson)
        {
            core_persons persons = new core_persons();
            persons.person_idx = ilpPerson.person_idx;
            persons.person_name = ilpPerson.person_name;
            persons.person_gender = ilpPerson.person_gender;
            persons.person_identifier = ilpPerson.person_identifier;
            persons.person_birthday = ilpPerson.person_birthday;
            persons.contact_addr_1 = ilpPerson.residential_addr_1;
            persons.person_birthplace = ilpPerson.person_birthplace;
            persons.contact_addr_2 = ilpPerson.residential_addr_2;
            persons.person_nationality = ilpPerson.person_nationality;
            persons.contact_addr_3 = ilpPerson.residential_addr_3;
            persons.contact_mobile_no = ilpPerson.contact_mobile_no;
            persons.contact_postcode = ilpPerson.residential_postcode;
            persons.contact_phone_no = ilpPerson.contact_phone_no;
            persons.contact_city = ilpPerson.residential_city;
            persons.person_employ_permit_no = ilpPerson.person_employ_permit_no;
            persons.contact_state = ilpPerson.residential_state;
            persons.person_employ_permit_released_place = ilpPerson.person_employ_permit_released_place;
            persons.person_employ_date_start = ilpPerson.person_employ_date_start;
            persons.person_employ_date_end = ilpPerson.person_employ_date_end;
            persons.person_cert_upload = ilpPerson.person_cert_upload;

            unitOfWork.CorePersonsRepository.UpdatePengajarPerson(persons);
        }

        public core_person_qualifications CreatePersonQualification(PersonQualification personQualification, Guid user_idx)
        {
            core_person_qualifications qualitification = new core_person_qualifications();

            qualitification.person_idx = personQualification.person_idx;
            qualitification.qualification_institution_name = personQualification.qualification_institution_name;
            qualitification.qualification_name = personQualification.qualification_name;
            qualitification.qualification_date_start = personQualification.qualification_date_start;
            qualitification.qualification_upload = personQualification.qualification_upload;

            return unitOfWork.CorePersonQualificationsRepository.StoreNewPersonQualification(qualitification, user_idx);
        }

        public core_person_qualifications UpdatePersonQualification(PersonQualification personQualification)
        {
            core_person_qualifications qualitification = new core_person_qualifications();

            qualitification.qualification_idx = personQualification.qualification_idx;
            qualitification.qualification_institution_name = personQualification.qualification_institution_name;
            qualitification.qualification_name = personQualification.qualification_name;
            qualitification.qualification_date_start = personQualification.qualification_date_start;
            qualitification.qualification_upload = personQualification.qualification_upload;

            return unitOfWork.CorePersonQualificationsRepository.UpdatePersonQualification(qualitification);
        }

        public void CreateILPPermit(Guid person_idx, Guid ilp_license_ref, Guid user_idx)
        {
            ilp_permits permit = new ilp_permits();
            permit.ilp_license_ref = ilp_license_ref;
            permit.person_ref = person_idx;
            unitOfWork.IlpPermits.SaveNewPermit(permit, user_idx);
        }

        public List<Countries> GetCountries()
        {
            var countries = new List<Countries>();
            var datas = unitOfWork.RefGeoCountriesRepository.GetCountries();
            foreach (var data in datas)
            {
                Countries country = new Countries();
                country.country_idx = data.country_idx;
                country.country_name = data.country_name;
                country.is_asean = data.is_asean;
                countries.Add(country);
            }
            return countries;
        }

        public ILPPerson GetPerson(Guid ilp_license_ref)
        {
            var permit = unitOfWork.IlpPermits.GetPermitByLicenseRef(ilp_license_ref);
            var data = unitOfWork.CorePersonsRepository.GetCorePersonByGuid(permit.person_ref);
            var refType = unitOfWork.RefReferencesTypesRepository.GetReferenceTypeByName("GENDER");
            var genderIdx = unitOfWork.RefReferencesRepository.GetReferenceByTypeAndCode(refType.ref_idx, "MALE").ref_idx;
            ILPPerson person = new ILPPerson();
            person.person_idx = data.person_idx;
            person.person_name = data.person_name;
            person.person_gender = (Guid)data.person_gender;
            person.person_identifier = data.person_identifier;
            person.person_birthday = (DateTime)data.person_birthday;
            person.residential_addr_1 = data.contact_addr_1;
            person.person_birthplace = data.person_birthplace;
            person.residential_addr_2 = data.contact_addr_2;
            person.person_nationality = (Guid)data.person_nationality;
            person.residential_addr_3 = data.contact_addr_3;
            person.contact_mobile_no = data.contact_mobile_no;
            person.residential_postcode = data.contact_postcode;
            person.contact_phone_no = data.contact_phone_no;
            person.residential_city = (Guid)data.contact_city;
            person.person_employ_permit_no = data.person_employ_permit_no;
            person.residential_state = (Guid)data.contact_state;
            person.person_employ_permit_released_place = data.person_employ_permit_released_place;
            person.person_employ_date_start = data.person_employ_date_start;
            person.person_employ_date_end = data.person_employ_date_end;
            if (data.person_cert_upload != "" && data.person_cert_upload != null)
            {
                person.upload_cert = GetILPUpload(Guid.Parse(data.person_cert_upload));
            }

            return person;
        }

        public List<PersonQualification> GetIlpPersonQualifications(Guid application_ref)
        {
            var personQualifications = new List<PersonQualification>();
            var permit = unitOfWork.IlpPermits.GetPermitByLicenseRef(application_ref);
            if (permit != null)
            {
                var datas = unitOfWork.CorePersonQualificationsRepository.GetPersonQualificationsByPersonIdx(permit.person_ref);
                foreach (var data in datas)
                {
                    PersonQualification personQualification = new PersonQualification();
                    personQualification.qualification_idx = data.qualification_idx;
                    personQualification.person_idx = data.person_idx;
                    personQualification.qualification_institution_name = data.qualification_institution_name;
                    personQualification.qualification_name = data.qualification_name;
                    personQualification.qualification_date_start = (DateTime)data.qualification_date_start;
                    personQualification.qualification_upload = data.qualification_upload;
                    var qualifications = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.qualification_level_idx);
                    var multiQualifications = new List<ILPMultiSelect>();
                    foreach (var qualification in qualifications)
                    {
                        ILPMultiSelect multiQualification = new ILPMultiSelect();
                        multiQualification.parent_ref = qualification.parent_ref;
                        multiQualification.details_ref = qualification.details_ref;
                        multiQualification.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)qualification.details_ref).ref_description;
                        multiQualifications.Add(multiQualification);
                    }
                    personQualification.multi_qualification_level = multiQualifications;
                    if (data.qualification_upload != "" && data.qualification_upload != null)
                    {
                        personQualification.upload_doc = GetILPUpload(Guid.Parse(data.qualification_upload));
                    }
                    personQualifications.Add(personQualification);
                }
            }
            return personQualifications;
        }

        public void CreatePersonExperience(PersonExperience personExperience, Guid user_idx)
        {
            core_person_work_experiences experience = new core_person_work_experiences();

            experience.person_idx = personExperience.person_idx;
            experience.stub_ref = personExperience.stub_ref;
            experience.experience_employer = personExperience.experience_employer;
            experience.experience_position = personExperience.experience_position;
            experience.experience_date_start = personExperience.experience_date_start;
            experience.experience_date_end = personExperience.experience_date_end;
            experience.experience_employer_address = personExperience.experience_employer_address;
            experience.experience_employer_address2 = personExperience.experience_employer_address2;
            experience.experience_employer_address3 = personExperience.experience_employer_address3;
            experience.experience_employer_poscode = personExperience.experience_employer_poscode;
            experience.experience_employer_city = personExperience.experience_employer_city;
            experience.experience_employer_state = personExperience.experience_employer_state;

            unitOfWork.CorePersonWorkExperiencesRepository.StoreNewPersonExperience(experience, user_idx);
        }

        public void UpdatePersonExperience(PersonExperience personExperience)
        {
            core_person_work_experiences experience = new core_person_work_experiences();

            experience.experience_idx = personExperience.experience_idx;
            experience.experience_employer = personExperience.experience_employer;
            experience.experience_position = personExperience.experience_position;
            experience.experience_date_start = personExperience.experience_date_start;
            experience.experience_date_end = personExperience.experience_date_end;
            experience.experience_employer_address = personExperience.experience_employer_address;
            experience.experience_employer_address2 = personExperience.experience_employer_address2;
            experience.experience_employer_address3 = personExperience.experience_employer_address3;
            experience.experience_employer_poscode = personExperience.experience_employer_poscode;
            experience.experience_employer_city = personExperience.experience_employer_city;
            experience.experience_employer_state = personExperience.experience_employer_state;

            unitOfWork.CorePersonWorkExperiencesRepository.UpdatePersonExperience(experience);
        }

        public List<PersonExperience> GetIlpPersonExperiences(Guid application_ref)
        {
            var personExperiences = new List<PersonExperience>();
            var permit = unitOfWork.IlpPermits.GetPermitByLicenseRef(application_ref);
            var datas = unitOfWork.CorePersonWorkExperiencesRepository.GetPersonExperiencesByPersonIdx(permit.person_ref);
            foreach (var data in datas)
            {
                PersonExperience personExperience = new PersonExperience();
                personExperience.experience_idx = data.experience_idx;
                personExperience.experience_employer = data.experience_employer;
                personExperience.experience_position = data.experience_position;
                personExperience.experience_date_start = (DateTime)data.experience_date_start;
                personExperience.experience_date_end = (DateTime)data.experience_date_end;
                personExperience.experience_employer_address = data.experience_employer_address;
                personExperience.experience_employer_address2 = data.experience_employer_address2;
                personExperience.experience_employer_address3 = data.experience_employer_address3;
                personExperience.experience_employer_poscode = data.experience_employer_poscode;
                personExperience.experience_employer_city = data.experience_employer_city;
                personExperience.experience_employer_state = data.experience_employer_state;
                personExperiences.Add(personExperience);
            }
            return personExperiences;
        }

        public core_person_courses CreatePersonCourse(PersonCourse personCourse, Guid user_idx)
        {
            core_person_courses course = new core_person_courses();

            course.person_idx = personCourse.person_idx;
            course.stub_ref = personCourse.stub_ref;
            course.course_name = personCourse.course_name;

            return unitOfWork.CorePersonCoursesRepository.StoreNewPersonCourse(course, user_idx);
        }

        public core_person_courses UpdatePersonCourse(PersonCourse personCourse)
        {
            core_person_courses course = new core_person_courses();

            course.person_course_idx = personCourse.person_course_idx;
            course.course_name = personCourse.course_name;

            return unitOfWork.CorePersonCoursesRepository.UpdatePersonCourse(course);
        }

        public List<PersonCourse> GetIlpPersonCourses(Guid application_ref)
        {
            var personCourses = new List<PersonCourse>();
            var permit = unitOfWork.IlpPermits.GetPermitByLicenseRef(application_ref);
            var datas = unitOfWork.CorePersonCoursesRepository.GetPersonCoursesByPersonIdx(permit.person_ref);
            foreach (var data in datas)
            {
                PersonCourse personCourse = new PersonCourse();
                personCourse.person_course_idx = data.person_course_idx;
                personCourse.course_name = data.course_name;
                personCourse.course_subject_idx = (Guid)data.course_subject_idx;
                var subjects = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.course_subject_idx);
                var multiSubjects = new List<ILPMultiSelect>();
                foreach (var subject in subjects)
                {
                    ILPMultiSelect multiSubject = new ILPMultiSelect();
                    multiSubject.parent_ref = subject.parent_ref;
                    multiSubject.details_ref = subject.details_ref;
                    multiSubject.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)subject.details_ref).ref_description;
                    multiSubjects.Add(multiSubject);
                }
                personCourse.multi_subject = multiSubjects;
                personCourses.Add(personCourse);
            }
            return personCourses;
        }

        public List<Reference> GetReferences(String reference_name)
        {
            var references = new List<Reference>();
            var referenceType = unitOfWork.RefReferencesTypesRepository.GetReferenceTypeByName(reference_name);
            var datas = unitOfWork.RefReferencesRepository.GetReferencesByType(referenceType.ref_idx);
            foreach (var data in datas)
            {
                Reference reference = new Reference();
                reference.ref_idx = data.ref_idx;
                reference.ref_type = data.ref_type;
                reference.ref_name = data.ref_name;
                reference.ref_code = data.ref_code;
                reference.ref_description = data.ref_description;
                references.Add(reference);
            }
            return references;
        }

        public List<Shareholder> GetShareholders(Guid user_idx)
        {
            var shareholders = new List<Shareholder>();
            var user = unitOfWork.Users.GetUser(user_idx);
            var datas = unitOfWork.CoreOrganizationShareholders.GetOrganizationShareholdersByUser((Guid)user.person_ref);

            decimal sumNumberOfShare = datas.Sum(d => d.number_of_shares);
            foreach (var data in datas)
            {
                Shareholder shareholder = new Shareholder();

                if (data.shareholder_person_ref != null)
                {
                    var person = unitOfWork.CorePersonsRepository.GetCorePersonByGuid((Guid)data.shareholder_person_ref);
                    shareholder.person_name = person.person_name;
                    shareholder.name = person.person_name;
                    shareholder.person_mobile_no = person.contact_mobile_no;
                    shareholder.person_identifier = person.person_identifier;
                    if (person.person_birthday.HasValue) shareholder.person_birthday = (DateTime)person.person_birthday;
                    shareholder.person_addr1 = person.contact_addr_1;
                    shareholder.person_addr2 = person.contact_addr_2;
                    shareholder.person_addr3 = person.contact_addr_3;
                    if (person.person_age.HasValue) shareholder.person_age = (int)person.person_age;
                    if (person.person_religion.HasValue) shareholder.person_religion = (Guid)person.person_religion;
                    if (person.person_gender.HasValue) shareholder.person_gender = (Guid)person.person_gender;
                    if (person.person_nationality.HasValue) shareholder.person_nationality = (Guid)person.person_nationality;
                    shareholder.person_postcode = person.contact_postcode;
                    if (person.contact_city.HasValue) shareholder.person_city = (Guid)person.contact_city;
                    if (person.contact_state.HasValue) shareholder.person_state = (Guid)person.contact_state;
                    if (person.person_nationality.HasValue) shareholder.person_nationality = (Guid)person.person_nationality;
                    shareholder.person_id_upload = person.person_id_upload;
                    shareholder.identifier = person.person_identifier;
                    shareholder.shareholder_person_ref = person.person_idx;
                    if (person.person_id_upload != "" && person.person_id_upload != null)
                    {
                        shareholder.upload_id = GetILPUpload(Guid.Parse(person.person_id_upload));
                    }
                }

                if (data.shareholder_organization_ref != null)
                {
                    var organization = unitOfWork.CoreOrganizations.GetOrganization((Guid)data.shareholder_organization_ref);
                    shareholder.organization_identifier = organization.organization_identifier;
                    shareholder.name = organization.organization_name;
                    shareholder.identifier = organization.organization_identifier;
                    shareholder.organization_name = organization.organization_name;
                    shareholder.organization_mobile_no = organization.office_mobile_no;
                    if(organization.incorporation_date!=null)
                    shareholder.organization_incorporation_date = (DateTime)organization.incorporation_date;


                    shareholder.organization_addr1 = organization.office_addr_1;
                    shareholder.organization_addr2 = organization.office_addr_2;
                    shareholder.organization_addr3 = organization.office_addr_3;
                    shareholder.organization_postcode = organization.office_postcode;
                    if (organization.office_city != null)
                        shareholder.organization_city = (Guid)organization.office_city;

                    if (organization.office_state != null)
                        shareholder.organization_state = (Guid)organization.office_state;

                    if (organization.country_ref != null)
                        shareholder.organization_country = (Guid)organization.country_ref;

                    shareholder.shareholder_organization_ref = (Guid)data.shareholder_organization_ref;
                }

                shareholder.organization_shareholder_idx = data.organization_shareholder_idx;
                shareholder.organization_ref = data.organization_ref;
                if (data.status_shareholder.HasValue)
                {
                    shareholder.status_shareholder = (Guid)data.status_shareholder;
                    var reference = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)data.status_shareholder);
                    shareholder.status = reference.ref_description;
                }

                shareholder.number_of_shares = data.number_of_shares;
                decimal share = (shareholder.number_of_shares / sumNumberOfShare * 100);
                //shareholder.number_of_shares = shareholder.number_of_shares;
                shareholder.share_percentage = share.ToString("n2");

                shareholders.Add(shareholder);
            }
            return shareholders;
        }

        public bool GetShareholdersByOrgRef(Guid org_ref)
        {
            var shareholder = unitOfWork.CoreOrganizationShareholders.GetOrganizationShareholdersByOrgRef(org_ref);
            return shareholder;
        }

        public List<Director> GetDirectors(Guid user_idx)
        {
            var directors = new List<Director>();
            var user = unitOfWork.Users.GetUser(user_idx);
            var datas = unitOfWork.CoreOrganizationDirectorsRepository.GetOrganizationDirectorsByOrgRef((Guid)user.person_ref);
            foreach (var data in datas)
            {
                var organization = unitOfWork.CoreOrganizations.GetOrganization(data.organization_ref);
                var person = unitOfWork.CorePersonsRepository.GetCorePersonByGuid((Guid)data.person_ref);
                Director director = new Director();
                director.organization_director_idx = data.organization_director_idx;
                director.organization_ref = data.organization_ref;
                director.person_ref = (Guid)data.person_ref;
                director.person_name = person.person_name;
                director.organization_identifier = organization.organization_identifier;
                director.person_mobile_no = person.contact_mobile_no;
                director.person_phone_no = person.contact_phone_no;
                director.person_identifier = person.person_identifier;
                if (person.person_birthday != null) director.person_birthday = (DateTime)person.person_birthday;
                director.person_addr1 = person.contact_addr_1;
                director.person_addr2 = person.contact_addr_2;
                director.person_addr3 = person.contact_addr_3;
                director.person_age = (person.person_age != null) ? (int)person.person_age : 0;
                if (person.person_gender.HasValue)
                {
                    director.person_gender = (Guid)person.person_gender;
                    var reference = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)person.person_gender);
                    director.person_gender_name = reference.ref_description;
                }
                if (person.person_nationality.HasValue)
                {
                    director.person_nationality = (Guid)person.person_nationality;
                    var country = unitOfWork.RefGeoCountriesRepository.GetCountry((Guid)person.person_nationality);
                    director.person_nationality_name = country.country_name;
                }
                if (person.person_id_upload != "" && person.person_id_upload != null)
                {
                    director.upload_id = GetILPUpload(Guid.Parse(person.person_id_upload));
                }
                if (person.person_cert_upload != "" && person.person_cert_upload != null)
                {
                    director.upload_cert = GetILPUpload(Guid.Parse(person.person_cert_upload));
                }
                director.person_postcode = person.contact_postcode;
                directors.Add(director);
            }
            return directors;
        }

        public bool GetDirectorsByOrgRef(Guid org_ref)
        {
            var director = unitOfWork.CoreOrganizationDirectorsRepository.GetOrganizationDirectorByOrgRef(org_ref);
            return director;
        }

        public void CreatePersonReference(ILPPersonReference personReference, Guid user_idx, Guid license_ref)
        {
            var permit = unitOfWork.IlpPermits.GetPermitByLicenseRef(license_ref);

            core_persons person = new core_persons();
            person.person_identifier = "0";
            person.person_name = personReference.person_name;
            person.contact_addr_1 = personReference.person_addr1;
            person.contact_addr_2 = personReference.person_addr2;
            person.contact_addr_3 = personReference.person_addr3;
            person.contact_postcode = personReference.person_postcode;
            person.contact_city = personReference.person_city;
            person.contact_state = personReference.person_state;
            person.contact_email = personReference.person_email;
            person.contact_mobile_no = personReference.person_mobile_no;
            var newPerson = unitOfWork.CorePersonsRepository.StoreNewPerson(person, user_idx);

            ilp_person_references personRef = new ilp_person_references();

            personRef.permit_ref = permit.ilp_permits_idx;
            personRef.person_ref = newPerson.person_idx;
            personRef.person_occupation = personReference.person_occupation;
            personRef.person_known_duration = personReference.person_known_duration;

            unitOfWork.IlpPersonReferences.SaveNewPersonReference(personRef, user_idx);
        }

        public void UpdatePersonReference(ILPPersonReference personReference)
        {
            core_persons person = new core_persons();
            person.person_idx = personReference.person_ref;
            person.person_name = personReference.person_name;
            person.contact_addr_1 = personReference.person_addr1;
            person.contact_addr_2 = personReference.person_addr2;
            person.contact_addr_3 = personReference.person_addr3;
            person.contact_postcode = personReference.person_postcode;
            person.contact_city = personReference.person_city;
            person.contact_state = personReference.person_state;
            person.contact_email = personReference.person_email;
            person.contact_mobile_no = personReference.person_mobile_no;
            unitOfWork.CorePersonsRepository.UpdatePerson(person);

            ilp_person_references personRef = new ilp_person_references();
            personRef.person_reference_idx = personReference.person_reference_idx;
            personRef.person_occupation = personReference.person_occupation;
            personRef.person_known_duration = personReference.person_known_duration;

            unitOfWork.IlpPersonReferences.UpdatePersonReference(personRef);
        }

        public List<ILPPersonReference> GetPersonReference(Guid ilp_license_ref)
        {
            var permit = unitOfWork.IlpPermits.GetPermitByLicenseRef(ilp_license_ref);
            var datas = unitOfWork.IlpPersonReferences.GetPersonReferencesByPermitRef(permit.ilp_permits_idx);
            var personReferences = new List<ILPPersonReference>();
            foreach (var data in datas)
            {
                var person = unitOfWork.CorePersonsRepository.GetCorePersonByGuid((Guid)data.person_ref);

                ILPPersonReference personReference = new ILPPersonReference();
                personReference.person_reference_idx = data.person_reference_idx;
                personReference.permit_ref = data.permit_ref;
                personReference.person_ref = data.person_ref;
                personReference.person_name = person.person_name;
                personReference.person_addr1 = person.contact_addr_1;
                personReference.person_addr2 = person.contact_addr_2;
                personReference.person_addr3 = person.contact_addr_3;
                personReference.person_mobile_no = person.contact_mobile_no;
                personReference.person_email = person.contact_email;
                personReference.person_postcode = person.contact_postcode;
                personReference.person_city = (Guid)person.contact_city;
                personReference.person_state = (Guid)person.contact_state;
                personReference.person_occupation = data.person_occupation;
                personReference.person_known_duration = (int)data.person_known_duration;
                personReferences.Add(personReference);
            }

            return personReferences;
        }

        public ilp_instructor_courses CreateInstructorCourse(ILPInstructorCourse instructor, Guid user_idx)
        {
            core_persons person = new core_persons();
            person.person_identifier = instructor.person_identifier;
            person.person_name = instructor.person_name;
            person.contact_mobile_no = instructor.person_mobile_no;
            person.contact_phone_no = instructor.person_phone_no;
            person.person_nationality = instructor.person_country;
            var newPerson = unitOfWork.CorePersonsRepository.StoreNewPerson(person, user_idx);

            ilp_instructor_courses instructorCourse = new ilp_instructor_courses();

            instructorCourse.license_ref = instructor.license_ref;
            instructorCourse.person_ref = newPerson.person_idx;
            instructorCourse.course_details_others = instructor.course_details_others;
            instructorCourse.facility_details_others = instructor.facility_details_others;
            instructorCourse.course_details_upload = instructor.course_details_upload;
            instructorCourse.premise_is_shared = instructor.premise_is_shared;

            return unitOfWork.IlpInstructorCourses.SaveNewInstructorCourse(instructorCourse, user_idx);
        }

        public ilp_instructor_courses UpdateInstructorCourse(ILPInstructorCourse instructor)
        {
            core_persons person = new core_persons();
            person.person_idx = instructor.person_ref;
            person.person_identifier = instructor.person_identifier;
            person.person_name = instructor.person_name;
            person.contact_mobile_no = instructor.person_mobile_no;
            person.contact_phone_no = instructor.person_phone_no;
            person.person_nationality = instructor.person_country;
            var newPerson = unitOfWork.CorePersonsRepository.UpdateInstructor(person);

            ilp_instructor_courses instructorCourse = new ilp_instructor_courses();

            instructorCourse.instructor_courses_idx = instructor.instructor_courses_idx;
            instructorCourse.course_details_others = instructor.course_details_others;
            instructorCourse.facility_details_others = instructor.facility_details_others;
            instructorCourse.course_details_upload = instructor.course_details_upload;
            instructorCourse.premise_is_shared = instructor.premise_is_shared;

            return unitOfWork.IlpInstructorCourses.UpdateInstructorCourse(instructorCourse);
        }

        public List<ILPInstructorCourse> GetInstructorCourses(Guid ilp_license_ref, Guid user_idx)
        {
            //var datas = unitOfWork.IlpInstructorCourses.GetInstructorCoursesByLicenseRef(ilp_license_ref);
            var datas = (from ins in unitOfWork.IlpInstructorCourses.Table
                         join ilp in unitOfWork.IlpLicenses.Table on ins.license_ref equals ilp.ilp_idx
                         join o in unitOfWork.CoreOrganizations.Table on ilp.organization_ref equals o.organization_idx
                         join u in unitOfWork.CoreUsersRepository.Table on o.organization_idx equals u.user_organization
                         where u.user_idx == user_idx
                         select ins).ToList();

            var instructorCourses = new List<ILPInstructorCourse>();
            foreach (var data in datas)
            {
                var person = unitOfWork.CorePersonsRepository.GetCorePersonByGuid((Guid)data.person_ref);

                ILPInstructorCourse instructorCourse = new ILPInstructorCourse();
                instructorCourse.instructor_courses_idx = data.instructor_courses_idx;
                instructorCourse.license_ref = data.license_ref;
                instructorCourse.person_ref = data.person_ref;
                instructorCourse.course_details = (Guid)data.course_details;
                instructorCourse.course_details_others = data.course_details_others;
                instructorCourse.facility_details = data.facility_details;
                instructorCourse.facility_details_others = data.facility_details_others;
                instructorCourse.course_details_upload = data.course_details_upload;
                instructorCourse.premise_is_shared = data.premise_is_shared;

                instructorCourse.person_name = person.person_name;
                instructorCourse.person_identifier = person.person_identifier;
                instructorCourse.person_mobile_no = person.contact_mobile_no;
                instructorCourse.person_phone_no = person.contact_phone_no;
                instructorCourse.person_country = (Guid)person.person_nationality;

                var courseDetails = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.course_details);
                var multiCourseDetails = new List<ILPMultiSelect>();
                foreach (var courseDetail in courseDetails)
                {
                    ILPMultiSelect multiCourseDetail = new ILPMultiSelect();
                    multiCourseDetail.parent_ref = courseDetail.parent_ref;
                    multiCourseDetail.details_ref = courseDetail.details_ref;
                    multiCourseDetail.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)courseDetail.details_ref).ref_description;
                    multiCourseDetails.Add(multiCourseDetail);
                }
                instructorCourse.multi_course_details = multiCourseDetails;

                var facilityDetails = unitOfWork.IlpMultiSelects.GetMultiSelectsByParentRef((Guid)data.facility_details);
                var multiFacilityDetails = new List<ILPMultiSelect>();
                foreach (var facilityDetail in facilityDetails)
                {
                    ILPMultiSelect multiFacilityDetail = new ILPMultiSelect();
                    multiFacilityDetail.parent_ref = facilityDetail.parent_ref;
                    multiFacilityDetail.details_ref = facilityDetail.details_ref;
                    multiFacilityDetail.details_name = unitOfWork.RefReferencesRepository.GetReferenceByIdx((Guid)facilityDetail.details_ref).ref_description;

                    multiFacilityDetails.Add(multiFacilityDetail);
                }
                instructorCourse.multi_facility_details = multiFacilityDetails;
                if (data.course_details_upload != "" && data.course_details_upload != null)
                {
                    instructorCourse.upload_doc = GetILPUpload(Guid.Parse(data.course_details_upload));
                }

                instructorCourses.Add(instructorCourse);
            }

            return instructorCourses;
        }

        public void UpdateSupportDoc(Guid id, String org_file_name, String file_name, String path)
        {
            unitOfWork.CoreChkItemsInstancesRepository.UpdateSupportDocByIdx(id, org_file_name, file_name, path);
        }

        public string UpdateChecklistStatus(string module_id, string component_id, short status)
        {

            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            chklistitems.bool1 = status;
            unitOfWork.Complete();
            return "success";
        }
        public core_uploads_freeform_by_persons StoreILPUploads(Guid person_ref, String upload_name, String upload_decription, String upload_path, Guid user_idx, String type)
        {
            try
            {
                //var upload_type = unitOfWork.RefPersonIdentifierTypesRepository.GetIdentifierTypeByName(type);
                var upload_type = unitOfWork.RefReferencesRepository.Find(i => i.ref_code == type).FirstOrDefault();
                var status = unitOfWork.RefStatusRecordRepository.GetStatusByName("ACTIVE");
                return unitOfWork.CoreUploadsFreeFormPersonRepository.SaveNewUpload(person_ref, upload_type.ref_idx, upload_name, upload_decription, upload_path, status.status_idx, user_idx);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e.Message);
            }
            return null;
        }

        public core_uploads_freeform_by_modules StoreAdditionalDoc(Guid stub_ref, String document_name, String document_decription, String document_upload_path, Guid user_idx)
        {
            var applicationStub = unitOfWork.FlowApplicationStubs.GetApplication(stub_ref);
            return unitOfWork.CoreUploadsFreeformModulesRepository.SaveAdditionalDoc(applicationStub.apply_module, applicationStub.apply_idx, document_name, document_decription, document_upload_path, user_idx);
        }

        public void UpdateILPUploads(Guid idx, String org_file_name, String file_name, String path)
        {
            unitOfWork.IlpUploads.UpdateNewUpload(idx, org_file_name, file_name, path);
        }

        public core_uploads_freeform_by_persons GetILPUpload(Guid idx)
        {
            return unitOfWork.CoreUploadsFreeFormPersonRepository.GetUploadByIdx(idx);
        }

        public void DestroyILPUpload(Guid idx)
        {
            unitOfWork.CoreUploadsFreeformModulesRepository.DestroyUpload(idx);
        }

        public void DestroyILPInstructorCourse(Guid idx)
        {
            unitOfWork.IlpInstructorCourses.DestroyInstructorCourse(idx);
        }


        public List<IlpViewModels.ilp_application> GetApplicationStatusMain1(string sUserID, string module)
        {
            Guid gUserID = Guid.Parse(sUserID);
            List<TourlistDataLayer.DataModel.vw_ilp_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_ilp_application>();
            if (module == null)
            {
                clsApplication = unitOfWork.VwIlpApplicationRepository.Find(c => (c.status == "Deraf") && c.apply_user == gUserID).OrderByDescending(i => i.module_name).ToList();

                if (clsApplication.Count == 0)
                {
                    var appActive = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_user == gUserID)).OrderByDescending(i => i.application_date).FirstOrDefault();
                    if (appActive != null)
                    {
                        clsApplication = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_idx == appActive.apply_idx)).ToList();
                    }
                }
            }
            else
            {
                //var appActive = unitOfWork.VWPPPAplicationRepository.Find(c => (c.apply_user == gUserID && c.status == "Selesai" && c.module_name == module)).OrderByDescending(i => i.application_date).FirstOrDefault();
                var appActive = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_user == gUserID && c.module_name == module)).OrderByDescending(i => i.application_date).FirstOrDefault();
                if (appActive != null)
                {
                    clsApplication = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_idx == appActive.apply_idx)).ToList();
                }
            }
            List<IlpViewModels.ilp_application> modelList = new List<IlpViewModels.ilp_application>();
            foreach (var app in clsApplication)
            {
                IlpViewModels.ilp_application model = new IlpViewModels.ilp_application();
                {
                    model.apply_idx = app.apply_idx;
                    model.ilp_idx = app.ilp_idx;
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

        public List<IlpViewModels.ilp_application> GetApplicationStatusMain(string sUserID, string module, Guid organization_ID)
        {
            Guid gUserID = Guid.Parse(sUserID);
            List<TourlistDataLayer.DataModel.vw_ilp_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_ilp_application>();
            if (module == null)
            {
                
                var appActive = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_user == gUserID)).OrderByDescending(i => i.application_date).FirstOrDefault();
                if (appActive != null)
                {
                    clsApplication = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_idx == appActive.apply_idx)).ToList();
                }
               
            }
            else
            {
               
                if (module == "ILP_ADDBRANCH")
                {
                    var appActive = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_user == gUserID && c.module_name == module)).OrderByDescending(i => i.application_date).ToList();
                    if (appActive != null)
                    {
                        clsApplication = unitOfWork.VwIlpApplicationRepository.Find(c => (c.organization_ref == organization_ID && c.module_name == module)).OrderByDescending(i => i.application_date).ToList();
                    }
                }
                else
                {
                    var appActive = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_user == gUserID && c.module_name == module)).OrderByDescending(i => i.application_date).FirstOrDefault();
                    if (appActive != null)
                    {
                        clsApplication = unitOfWork.VwIlpApplicationRepository.Find(c => (c.apply_idx == appActive.apply_idx)).ToList();
                    }
                }



            }
            List<IlpViewModels.ilp_application> modelList = new List<IlpViewModels.ilp_application>();
            foreach (var app in clsApplication)
            {
                IlpViewModels.ilp_application model = new IlpViewModels.ilp_application();
                {
                    model.apply_idx = app.apply_idx;
                    model.ilp_idx = app.ilp_idx;
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

        public int checkDokumenSokongan(string AppID, string Module, string userID)
        {

            List<TourlistDataLayer.DataModel.ilp_licenses> licenses = new List<TourlistDataLayer.DataModel.ilp_licenses>();
            licenses = unitOfWork.IlpLicenses.Find(i => (i.stub_ref.ToString() == AppID)).ToList();

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
            List<IlpViewModels.ilp_supporting_documents> modelList = new List<IlpViewModels.ilp_supporting_documents>();

            return clsList.Count;

        }

        public bool UpdateShareholderOrganization(core_organizations organizations)
        {
            core_organizations core_organization = unitOfWork.CoreOrganizations.Find(c => c.organization_idx == organizations.organization_idx).FirstOrDefault();

            bool returnResult = false;
            int returnVal = 0;

            if (core_organization != null)
            {
                core_organization.office_addr_1 = organizations.office_addr_1;
                if (organizations.office_addr_1 == "")
                    core_organization.office_addr_1 = null;

                core_organization.office_addr_2 = organizations.office_addr_2;
                if (organizations.office_addr_2 == "")
                    core_organization.office_addr_2 = null;

                core_organization.office_addr_3 = organizations.office_addr_3;
                if (organizations.office_addr_3 == "")
                    core_organization.office_addr_3 = null;

                core_organization.office_mobile_no = organizations.office_mobile_no;
                core_organization.office_postcode = organizations.office_postcode;
                core_organization.office_city = organizations.office_city;
                core_organization.office_state = organizations.office_state;
                core_organization.incorporation_date = organizations.incorporation_date;
                core_organization.country_ref = organizations.country_ref;

                unitOfWork.CoreOrganizations.Update(core_organization);

                returnVal = unitOfWork.Complete();

                if (returnVal > 0)
                {
                    returnResult = true;
                }
            }
            return returnResult;
        }

        public bool UpdateShareholderPerson(core_persons persons)
        {
            core_persons core_person = unitOfWork.CorePersonsRepository.Find(c => c.person_idx == persons.person_idx).FirstOrDefault();

            bool returnResult = false;
            int returnVal = 0;

            if (core_person != null)
            {
                core_person.contact_mobile_no = persons.contact_mobile_no;

                core_person.contact_addr_1 = persons.contact_addr_1;
                if (persons.contact_addr_1 == "")
                    core_person.contact_addr_1 = null;

                core_person.contact_addr_2 = persons.contact_addr_2;
                if (persons.contact_addr_2 == "")
                    core_person.contact_addr_2 = null;

                core_person.contact_addr_3 = persons.contact_addr_3;
                if (persons.contact_addr_3 == "")
                    core_person.contact_addr_3 = null;

                core_person.person_age = persons.person_age;
                core_person.person_birthday = persons.person_birthday;
                core_person.person_religion = persons.person_religion;
                core_person.person_gender = persons.person_gender;
                core_person.person_nationality = persons.person_nationality;
                core_person.contact_postcode = persons.contact_postcode;
                core_person.contact_city = persons.contact_city;
                core_person.contact_state = persons.contact_state;
                core_person.person_idx = persons.person_idx;
                core_person.person_id_upload = persons.person_id_upload;

                unitOfWork.CorePersonsRepository.Update(core_person);

                returnVal = unitOfWork.Complete();

                if (returnVal > 0)
                {
                    returnResult = true;
                }
            }
            return returnResult;
        }

        public int GetILPStatusModul(string sOrganizationID, string sModule)
        {
            Guid gOrgID = Guid.Parse(sOrganizationID);
            List<TourlistDataLayer.DataModel.vw_ilp_application> clsApplication = new List<TourlistDataLayer.DataModel.vw_ilp_application>();

            clsApplication = unitOfWork.VwIlpApplicationRepository.Find(c => (c.organization_ref == gOrgID && c.module_name == sModule)).ToList();

            return clsApplication.Count;
        }
    }
}
