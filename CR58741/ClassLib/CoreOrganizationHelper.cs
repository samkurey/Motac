using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TourlistDataLayer.DataModel;
using TourlistWebAPI.Models;
using System.Data.Entity;
using System.Configuration;
using System.Globalization;
using TourlistDataLayer.ViewModels.Tobtab;
using TourlistDataLayer.Persistence;

namespace TourlistWebAPI.ClassLib
{
    public class CoreOrganizationHelper : TourlistBaseHelper
    {
        //public TourlistWebAPI.DataModels.MotacEntities motacContext = new DataModels.MotacEntities();
        
        private TourlistUnitOfWork unitOfWork
        {
            get
            {
                return this.TourlistUnitOfWork;
            }
        }

        public CoreOrganizationHelper()
        {
            //this.unitOfWork = new TourlistDataLayer.Persistence.TourlistUnitOfWork(new TourlistDataLayer.Persistence.TourlistContext(ConfigurationManager.ConnectionStrings["TourlistDbContext"].ToString()));
        }

        
        public bool UpdateOrganization(core_organizations org)
        {
            try
            {
                //using (var dbContextTransaction = unitOfWork.CoreOrganizations.TourlistContext.Database.BeginTransaction())
                //{
                    Guid Idx = org.organization_idx;
                   // var upd_org = unitOfWork.core_organizations.Where(c => c.organization_idx == Idx).FirstOrDefault();
                    var upd_org = unitOfWork.CoreOrganizations.Find(c => c.organization_idx == Idx).FirstOrDefault();
                    upd_org.registered_mobile_no = org.registered_mobile_no;
                    upd_org.registered_phone_no = org.registered_phone_no;
                    upd_org.registered_email = org.registered_email;
                    upd_org.registered_fax_no = org.registered_fax_no;
                  
                    
                    upd_org.cosec_mobile_no = org.cosec_mobile_no;
                    upd_org.cosec_phone_no = org.cosec_phone_no;
                    upd_org.cosec_email = org.cosec_email;
                    upd_org.cosec_fax_no = org.cosec_fax_no;
                    upd_org.cosec_name= org.cosec_name;
                    upd_org.cosec_addr_1=org.cosec_addr_1;
                    upd_org.cosec_addr_2=org.cosec_addr_2;
                    upd_org.cosec_addr_3=org.cosec_addr_3;
                    upd_org.cosec_postcode=org.cosec_postcode;
                    upd_org.cosec_city=org.cosec_city;
                    upd_org.cosec_state=org.cosec_state;

                    upd_org.office_mobile_no = org.office_mobile_no;
                    upd_org.office_phone_no = org.office_phone_no;
                    upd_org.office_email = org.office_email;
                    upd_org.office_fax_no = org.office_fax_no;
                    upd_org.pbt_ref=org.pbt_ref;
                   // upd_org.office_addr_1 = org.office_addr_1;
                   // upd_org.office_addr_2= org.office_addr_2;
                  //  upd_org.office_addr_3= org.office_addr_3;
                    upd_org.office_postcode=org.office_postcode;
                    upd_org.office_state=org.office_state;
                    upd_org.office_city = org.office_city;                

                    upd_org.is_has_business_address = org.is_has_business_address;
                    upd_org.website = org.website;
                    upd_org.modified_by = org.modified_by;
                    upd_org.modified_dt = DateTime.Now;
                    unitOfWork.CoreOrganizations.TourlistContext.Entry(upd_org).State = EntityState.Modified;
                    unitOfWork.Complete();

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateOrganizationShareholder(CoreOrganizationModel.core_organizations org)
        {
            try
            {
                //using (var dbContextTransaction = unitOfWork.CoreOrganizations.TourlistContext.Database.BeginTransaction())
                //{
                Guid Idx = org.organization_idx;
                // var upd_org = unitOfWork.core_organizations.Where(c => c.organization_idx == Idx).FirstOrDefault();
                var upd_org = unitOfWork.CoreOrganizations.Find(c => c.organization_idx == Idx).FirstOrDefault();
  

                upd_org.office_mobile_no = org.office_mobile_no;
                if (org.office_phone_no !=null && org.office_phone_no != "")
                {
                    upd_org.office_phone_no = org.office_phone_no;
                }
                //upd_org.office_email = org.office_email;
                //upd_org.office_fax_no = org.office_fax_no;
                upd_org.office_addr_1 = org.office_addr_1;
                upd_org.office_addr_2 = org.office_addr_2;
                upd_org.office_addr_3 = org.office_addr_3;
                upd_org.office_postcode = org.office_postcode;
                var vPostcode_cosec = unitOfWork.VwGeoListRepository.Find(i => (i.postcode_code == org.office_postcode)).FirstOrDefault();
                upd_org.office_state = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.town_idx;
                upd_org.office_city = (vPostcode_cosec == null) ? Guid.Empty : vPostcode_cosec.state_idx;
                //upd_org.office_state = org.office_state;
                //upd_org.office_city = org.office_city;
                upd_org.country_ref=org.country_ref;
                


                upd_org.modified_by = org.modified_by;
                upd_org.modified_dt = DateTime.Now;
                unitOfWork.CoreOrganizations.TourlistContext.Entry(upd_org).State = EntityState.Modified;
                unitOfWork.Complete();

                //}
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }


        }

        public bool UpdatePerson(core_persons person)
        {
            try
            {
                //using (var dbContextTransaction = unitOfWork.CoreOrganizations.TourlistContext.Database.BeginTransaction())
                //{
                    Guid Idx = person.person_idx;
                    //var upd_person = motacContext.core_persons.Where(c => c.person_idx == Idx).FirstOrDefault();
                    var upd_person =unitOfWork.CorePersonsRepository.Find(c => c.person_idx == Idx).FirstOrDefault();
                    upd_person.contact_addr_1 = person.contact_addr_1;
                    upd_person.contact_addr_2 = person.contact_addr_2;
                    upd_person.contact_addr_3 = person.contact_addr_3;
                    upd_person.contact_city = person.contact_city;
                    upd_person.contact_postcode = person.contact_postcode;
                    upd_person.contact_state = person.contact_state;
                    upd_person.person_gender = person.person_gender;
                    upd_person.person_religion = person.person_religion;
                    upd_person.contact_mobile_no = person.contact_mobile_no;
                    upd_person.person_birthday=person.person_birthday;
                    upd_person.person_nationality = person.person_nationality;
                    upd_person.modified_by = person.modified_by;
                    upd_person.modified_dt = DateTime.Now;
                
                    unitOfWork.CorePersonsRepository.TourlistContext.Entry(upd_person).State = EntityState.Modified;
                    unitOfWork.Complete();                 

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool ChangeStatusUpdatePerson(core_persons upd_person, string status_shareholder, decimal number_of_share, string type, string justification, string justificationID)
        {
            try
            {

                Guid Idx = upd_person.person_idx;
                Guid gStatus_shareholder = Guid.Empty;
                if (status_shareholder != "")
                {
                    gStatus_shareholder = Guid.Parse(status_shareholder);
                }


                var person = unitOfWork.CorePersonsUpdatedRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();

                if (person.old_person_name != upd_person.person_name)
                {
                    person.new_person_name = upd_person.person_name;
                }
                else
                {
                    person.new_person_name = null;
                }

                if (person.old_person_identifier != upd_person.person_identifier)
                {
                    person.new_person_identifier = upd_person.person_identifier;
                }
                else
                {
                    person.new_person_identifier = null;
                }

                if (person.old_contact_addr_1 != upd_person.contact_addr_1)
                {
                    person.new_contact_addr_1 = upd_person.contact_addr_1;
                }
                else
                {
                    person.new_contact_addr_1 = null;
                }
                if (person.old_contact_addr_2 != upd_person.contact_addr_2)
                {
                    person.new_contact_addr_2 = upd_person.contact_addr_2;
                }
                else
                {
                    person.new_contact_addr_2 = null;
                }

                if (person.old_contact_addr_3 != upd_person.contact_addr_3)
                {
                    person.new_contact_addr_3 = upd_person.contact_addr_3;
                }
                else
                {
                    person.new_contact_addr_3 = null;
                }
                if (person.old_contact_postcode != upd_person.contact_postcode)
                {
                    person.new_contact_postcode = upd_person.contact_postcode;
                }
                else
                {
                    person.new_contact_postcode = null;
                }
                if (person.old_person_gender != upd_person.person_gender)
                {
                    person.new_person_gender = upd_person.person_gender;
                }
                else
                {
                    person.new_person_gender = null;
                }

                if (person.old_contact_mobile_no != upd_person.contact_mobile_no)
                {
                    person.new_contact_mobile_no = upd_person.contact_mobile_no;
                }
                else
                {
                    person.new_contact_mobile_no = null;
                }
                if (person.old_person_birthday != upd_person.person_birthday)
                {
                    person.new_person_birthday = upd_person.person_birthday;
                }
                else
                {
                    person.new_person_birthday = null;
                }
                if (person.old_person_age != upd_person.person_age)
                {
                    person.new_person_age = upd_person.person_age;
                }
                else
                {
                    person.new_person_age = null;
                }

                if (person.old_person_nationality != upd_person.person_nationality)
                {
                    person.new_person_nationality = upd_person.person_nationality;
                }
                else
                {
                    person.new_person_nationality = null;
                }
                if (type == "Director")
                {
                    if (person.old_contact_phone_no != upd_person.contact_phone_no)
                    {
                        person.new_contact_phone_no = upd_person.contact_phone_no;
                    }
                    else
                    {
                        person.new_contact_phone_no = null;
                    }
                }
                else
                {
                    if (person.old_status_shareholder != gStatus_shareholder)
                    {
                        person.new_status_shareholder = gStatus_shareholder;
                    }
                    else
                    {
                        person.new_status_shareholder = null;
                    }

                    if (person.old_number_of_shares != number_of_share)
                    {
                        person.new_number_of_shares = number_of_share;
                    }
                    else
                    {
                        person.new_number_of_shares = null;
                    }
                    if (person.old_person_religion != upd_person.person_religion)
                    {
                        person.new_person_religion = upd_person.person_religion;
                    }
                    else
                    {
                        person.new_person_religion = null;
                    }
                }

                if (justificationID !="")
                {
                    Guid gJustificationID = Guid.Parse(justificationID);
                    person.new_justification_id = gJustificationID;
                    var justify = unitOfWork.RefReferencesRepository.Find(c => c.ref_idx == gJustificationID).FirstOrDefault();

                    if (justify.ref_code == "LAIN-LAIN")
                    {
                        person.person_status_rec = "KEMASKINI";
                        person.new_justification_name = justification;
                    }
                    else
                    {
                        person.person_status_rec = justify.ref_name;
                        person.new_justification_name = "";
                    }

                }
                else
                {
                    Guid gJustificationID = Guid.Empty;
                    person.new_justification_id = gJustificationID;
                    var justify = unitOfWork.RefReferencesRepository.Find(c => c.ref_idx == gJustificationID).FirstOrDefault();
                    
                    person.person_status_rec = "KEMASKINI";
                    person.new_justification_name = "";                   
                }





                person.modified_by = person.modified_by;
                person.modified_dt = DateTime.Now;

                unitOfWork.CorePersonsUpdatedRepository.TourlistContext.Entry(person).State = EntityState.Modified;
                unitOfWork.Complete();

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool ChangeStatusAddPerson(core_persons upd_person, string status_shareholder, decimal number_of_share, string type)
        {
            try
            {

                Guid Idx = upd_person.person_idx;
                Guid gStatus_shareholder = Guid.Empty;
                if (status_shareholder != "")
                {
                    gStatus_shareholder = Guid.Parse(status_shareholder);
                }


                var person = unitOfWork.CorePersonsUpdatedRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();

                if (person.old_person_name != upd_person.person_name)
                {
                    person.new_person_name = upd_person.person_name;
                }
                else
                {
                    person.new_person_name = null;
                }

                if (person.old_person_identifier != upd_person.person_identifier)
                {
                    person.new_person_identifier = upd_person.person_identifier;
                }
                else
                {
                    person.new_person_identifier = null;
                }

                if (person.old_contact_addr_1 != upd_person.contact_addr_1)
                {
                    person.new_contact_addr_1 = upd_person.contact_addr_1;
                }
                else
                {
                    person.new_contact_addr_1 = null;
                }
                if (person.old_contact_addr_2 != upd_person.contact_addr_2)
                {
                    person.new_contact_addr_2 = upd_person.contact_addr_2;
                }
                else
                {
                    person.new_contact_addr_2 = null;
                }

                if (person.old_contact_addr_3 != upd_person.contact_addr_3)
                {
                    person.new_contact_addr_3 = upd_person.contact_addr_3;
                }
                else
                {
                    person.new_contact_addr_3 = null;
                }
                if (person.old_contact_postcode != upd_person.contact_postcode)
                {
                    person.new_contact_postcode = upd_person.contact_postcode;
                }
                else
                {
                    person.new_contact_postcode = null;
                }
                if (person.old_person_gender != upd_person.person_gender)
                {
                    person.new_person_gender = upd_person.person_gender;
                }
                else
                {
                    person.new_person_gender = null;
                }

                if (person.old_contact_mobile_no != upd_person.contact_mobile_no)
                {
                    person.new_contact_mobile_no = upd_person.contact_mobile_no;
                }
                else
                {
                    person.new_contact_mobile_no = null;
                }
                if (person.old_person_birthday != upd_person.person_birthday)
                {
                    person.new_person_birthday = upd_person.person_birthday;
                }
                else
                {
                    person.new_person_birthday = null;
                }
                if (person.old_person_age != upd_person.person_age)
                {
                    person.new_person_age = upd_person.person_age;
                }
                else
                {
                    person.new_person_age = null;
                }

                if (type == "Director")
                {
                    if (person.old_contact_phone_no != upd_person.contact_phone_no)
                    {
                        person.new_contact_phone_no = upd_person.contact_phone_no;
                    }
                    else
                    {
                        person.new_contact_phone_no = null;
                    }
                    if (person.old_person_nationality != upd_person.person_nationality)
                    {
                        person.new_person_nationality = upd_person.person_nationality;
                    }
                    else
                    {
                        person.new_person_nationality = null;
                    }
                }
                else
                {
                    if (person.new_status_shareholder != gStatus_shareholder)
                    {
                        person.new_status_shareholder = gStatus_shareholder;
                    }
                    else
                    {
                        person.new_status_shareholder = null;
                    }

                    if (person.new_number_of_shares != number_of_share)
                    {
                        person.new_number_of_shares = number_of_share;
                    }
                    else
                    {
                        person.new_number_of_shares = null;
                    }
                    if (person.old_person_religion != upd_person.person_religion)
                    {
                        person.new_person_religion = upd_person.person_religion;
                    }
                    else
                    {
                        person.new_person_religion = null;
                    }


                }


                person.person_status_rec = "Kemaskini";
                person.modified_by = person.modified_by;
                person.modified_dt = DateTime.Now;

                unitOfWork.CorePersonsUpdatedRepository.TourlistContext.Entry(person).State = EntityState.Modified;
                unitOfWork.Complete();

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool ChangeStatusUpdateOrganization(core_organizations upd_org, string status_shareholder, decimal number_of_share, string registered_year, string country_ref, string userID, string justification, string justificationID)
        {
            try
            {

                Guid Idx = upd_org.organization_idx;
                Guid gStatus_shareholder = Guid.Parse(status_shareholder);
                Guid gJustificationID = Guid.Parse(justificationID);

                var org = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => c.organization_upd_idx == Idx).FirstOrDefault();

                if (org.old_organization_name != upd_org.organization_name)
                {
                    org.new_organization_name = upd_org.organization_name;
                }
                else
                {
                    org.new_organization_name = null;
                }

                if (org.old_organization_identifier != upd_org.organization_identifier)
                {
                    org.new_organization_identifier = upd_org.organization_identifier;
                }
                else
                {
                    org.new_organization_identifier = null;
                }

                if (org.old_mobile_no != upd_org.office_mobile_no)
                {
                    org.new_mobile_no = upd_org.office_mobile_no;
                }
                else
                {
                    org.new_mobile_no = null;
                }

                if (org.old_registered_year != registered_year)
                {
                    org.new_registered_year = registered_year;
                }
                else
                {
                    org.new_registered_year = null;
                }
                if (org.old_addr_1 != upd_org.office_addr_1)
                {
                    org.new_addr_1 = upd_org.office_addr_1;
                }
                else
                {
                    org.new_addr_1 = null;
                }
                if (org.old_addr_2 != upd_org.office_addr_2)
                {
                    org.new_addr_2 = upd_org.office_addr_2;
                }
                else
                {
                    org.new_addr_2 = null;
                }
                if (org.old_addr_3 != upd_org.office_addr_3)
                {
                    org.new_addr_3 = upd_org.office_addr_3;
                }
                else
                {
                    org.new_addr_3 = null;
                }

                if (org.old_postcode != upd_org.office_postcode)
                {
                    org.new_postcode = upd_org.office_postcode;
                }
                else
                {
                    org.new_postcode = null;
                }

                if (org.old_state != upd_org.office_state)
                {
                    org.new_state = upd_org.office_state;
                }
                else
                {
                    org.new_state = null;
                }

                if (org.old_status_shareholder != Guid.Parse(status_shareholder))
                {
                    org.new_status_shareholder = Guid.Parse(status_shareholder);
                }
                else
                {
                    org.new_status_shareholder = null;
                }
                if (org.old_country_ref != Guid.Parse(country_ref))
                {
                    org.new_country_ref = Guid.Parse(country_ref);
                }
                else
                {
                    org.new_country_ref = null;
                }
                if (org.old_number_of_shares != number_of_share)
                {
                    org.new_number_of_shares = number_of_share;
                }
                else
                {
                    org.new_number_of_shares = null;
                }

                org.new_justification_id = gJustificationID;
                org.new_justification_name = justification;


                var justify = unitOfWork.RefReferencesRepository.Find(c => c.ref_idx == gJustificationID).FirstOrDefault();
                //modified by awie
                if (justify.ref_description == "LAIN-LAIN")
                {
                    org.organization_status_rec = "KEMASKINI";
                    org.new_justification_name = justification;
                }
                else
                {
                    org.organization_status_rec = justify.ref_name;
                    org.new_justification_name = "";
                }

                org.modified_by = Guid.Parse(userID);
                org.modified_dt = DateTime.Now;

                unitOfWork.CoreOrganizationsUpdatedRepository.TourlistContext.Entry(org).State = EntityState.Modified;
                unitOfWork.Complete();

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool ChangeStatusUpdateOrganization_SaveNew(core_organizations upd_org, string status_shareholder, decimal number_of_share, string registered_year, string country_ref, string userID, Guid stubIDX)
        {
            try
            {

                Guid Idx = Guid.NewGuid();
                Guid gshareholderUpd = Guid.NewGuid();
                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "ACTIVE").FirstOrDefault();

                Guid gStatus_shareholder = Guid.Parse(status_shareholder);

                //  var org = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => c.organization_upd_idx == Idx).FirstOrDefault();
                core_organizations_updated org = new core_organizations_updated();
              //  org.stub_ref = stubIDX;
                org.organization_upd_idx = Idx;
                org.org_shareholder_upd_ref = gStatus_shareholder;
                org.new_organization_name = upd_org.organization_name;
                org.new_organization_identifier = upd_org.organization_identifier;
                org.organization_current_ref = upd_org.organization_idx;
                org.new_mobile_no = upd_org.office_mobile_no;
                org.new_registered_year = registered_year;
                org.new_addr_1 = upd_org.office_addr_1;
                org.new_addr_2 = upd_org.office_addr_2;
                org.new_addr_3 = upd_org.office_addr_3;
                org.new_postcode = upd_org.office_postcode;
                org.new_city = upd_org.office_city;
                org.new_state = upd_org.office_state;
                org.new_status_shareholder = Guid.Parse(status_shareholder);
                org.new_country_ref = Guid.Parse(country_ref);
                org.new_number_of_shares = number_of_share;
                org.organization_status_rec = "BAHARU";
                org.organization_status = upd_active.status_idx;
                org.created_dt = DateTime.Now;
                org.created_by = Guid.Parse(userID);
                org.modified_by = Guid.Parse(userID);
                org.modified_dt = DateTime.Now;
                
                unitOfWork.CoreOrganizationsUpdatedRepository.Add(org);
                unitOfWork.Complete();

                core_org_shareholders_updated shareholderUpd = new core_org_shareholders_updated();

                shareholderUpd.org_shareholder_upd_idx = gshareholderUpd;
                shareholderUpd.stub_ref = stubIDX;
                shareholderUpd.organization_ref = Idx;
                shareholderUpd.active_status = 1;
                shareholderUpd.created_by = Guid.Parse(userID);
                shareholderUpd.modified_by = Guid.Parse(userID);

                shareholderUpd.created_at = DateTime.Now;
                shareholderUpd.modified_at = DateTime.Now;
                unitOfWork.CoreOrgShareholdersUpdatedRepository.Add(shareholderUpd);
                var resultSH = unitOfWork.Complete(); //motacContext.SaveChanges();

                return true;
        }
            catch (Exception ex)
            {
                return false;
            }

}

        public bool ChangeStatusUpdatePerson_SaveNew(core_persons upd_person, string status_shareholder, decimal number_of_share, Guid stubIDX, Guid OrgID, Guid userID, string type,bool file,string shareholderID_add)
        {
            try
            {
                core_persons_updated person = new core_persons_updated();

                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "ACTIVE").FirstOrDefault();

                person.person_upd_idx = upd_person.person_idx;
                Guid gdirectorUpd = Guid.NewGuid();
                Guid gshareholderUpd = Guid.NewGuid();

                if (type == "director")
                {
                    person.org_director_upd_ref = gdirectorUpd;
                    person.new_contact_phone_no = upd_person.contact_phone_no;
                    person.person_type_rec = "director";
                    //  person.new_person_nationality = upd_person.person_nationality;
                }
                else
                {
                    person.person_type_rec = "shareholder";
                    person.org_shareholder_upd_ref = gshareholderUpd;
                    Guid gStatus_shareholder = Guid.Parse(status_shareholder);
                    person.new_status_shareholder = gStatus_shareholder;
                    person.new_number_of_shares = number_of_share;
                    person.new_person_religion = upd_person.person_religion;


                }
                person.new_person_nationality = upd_person.person_nationality;
                person.new_person_name = upd_person.person_name;
                person.new_person_identifier = upd_person.person_identifier;
                person.new_contact_addr_1 = upd_person.contact_addr_1;
                person.new_contact_addr_2 = upd_person.contact_addr_2;
                person.new_contact_addr_3 = upd_person.contact_addr_3;
                person.new_contact_postcode = upd_person.contact_postcode;
                person.new_person_gender = upd_person.person_gender;

                person.new_contact_mobile_no = upd_person.contact_mobile_no;
                person.new_person_birthday = upd_person.person_birthday;
                person.person_status = upd_active.status_idx;
                person.person_status_rec = "BAHARU";
                person.modified_by = userID;
                person.modified_dt = DateTime.Now;
                person.created_by = userID;
                person.created_dt = DateTime.Now;

                unitOfWork.CorePersonsUpdatedRepository.Add(person);
                var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                if (type == "director")
                {
                    core_org_directors_updated directorUpd = new core_org_directors_updated();
                    // Guid gshareholderUpd = Guid.NewGuid();
                    directorUpd.org_director_upd_idx = gdirectorUpd;
                    directorUpd.organization_ref = OrgID;
                    directorUpd.person_ref = upd_person.person_idx;
                    directorUpd.stub_ref = stubIDX;
                    directorUpd.active_status = 1;
                    directorUpd.created_by = userID;
                    directorUpd.modified_by = userID;
                    directorUpd.created_at = DateTime.Now;
                    directorUpd.modified_at = DateTime.Now;
                    unitOfWork.CoreOrgDirectorsUpdatedRepository.Add(directorUpd);
                    var resultSH = unitOfWork.Complete(); //motacContext.SaveChanges();

                    if (file==true)
                    {
                        var shareholderID = Guid.Empty;
                        if (shareholderID_add !="")
                        {
                            shareholderID = Guid.Parse(shareholderID_add);
                        }

                        var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

                        var my_kad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();

                        var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT" && c.ref_type == refType.ref_idx).FirstOrDefault();
                        var pas_pengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN" && c.ref_type == refType.ref_idx).FirstOrDefault();

                        var attach = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == shareholderID && c.upload_type_ref == my_kad.ref_idx && c.active_status == upd_active.status_idx)).FirstOrDefault();
                      
                        if (my_kad == null)
                        {
                            my_kad = passport;
                        }
                        if (attach != null)
                        {

                            core_uploads_freeform_by_persons uploads = new core_uploads_freeform_by_persons();
                            Guid gUploadID = Guid.NewGuid();
                            uploads.uploads_freeform_by_persons_idx = gUploadID;
                            uploads.person_ref = upd_person.person_idx;
                            uploads.upload_type_ref = attach.upload_type_ref;
                            uploads.upload_name = attach.upload_name;
                            uploads.upload_description = attach.upload_description;
                            uploads.upload_path = attach.upload_path;
                            uploads.active_status = upd_active.status_idx;
                            uploads.created_by = userID;
                            uploads.modified_by = userID;
                            uploads.modified_at = DateTime.Now;
                            uploads.created_at = DateTime.Now;

                            unitOfWork.CoreUploadsFreeFormPersonRepository.Add(uploads);
                            var resultattach = unitOfWork.Complete(); //motacContext.SaveChanges();
                        }

                        var attachPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == shareholderID && c.upload_type_ref == pas_pengajian.ref_idx && c.active_status == upd_active.status_idx)).FirstOrDefault();

                        if (attachPasPengajian != null)
                        {
                            core_uploads_freeform_by_persons uploads = new core_uploads_freeform_by_persons();
                            Guid gUploadID = Guid.NewGuid();
                            uploads.uploads_freeform_by_persons_idx = gUploadID;
                            uploads.person_ref = upd_person.person_idx;
                            uploads.upload_type_ref = attach.upload_type_ref;
                            uploads.upload_name = attach.upload_name;
                            uploads.upload_description = attach.upload_description;
                            uploads.upload_path = attach.upload_path;
                            uploads.active_status = upd_active.status_idx;
                            uploads.created_by = userID;
                            uploads.modified_by = userID;
                            uploads.modified_at = DateTime.Now;
                            uploads.created_at = DateTime.Now;

                            unitOfWork.CoreUploadsFreeFormPersonRepository.Add(uploads);
                            var resultattach = unitOfWork.Complete(); //motacContext.SaveChanges();
                        }

                    }


                }
                else
                {
                    core_org_shareholders_updated shareholderUpd = new core_org_shareholders_updated();
                    shareholderUpd.org_shareholder_upd_idx = gshareholderUpd;
                    shareholderUpd.organization_ref = OrgID;
                    shareholderUpd.stub_ref = stubIDX;
                    shareholderUpd.person_ref = upd_person.person_idx;
                    shareholderUpd.active_status = 1;
                    shareholderUpd.created_by = userID;
                    shareholderUpd.modified_by = userID;

                    shareholderUpd.created_at = DateTime.Now;
                    shareholderUpd.modified_at = DateTime.Now;
                    unitOfWork.CoreOrgShareholdersUpdatedRepository.Add(shareholderUpd);
                    var resultSH = unitOfWork.Complete(); //motacContext.SaveChanges();

                }



                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public bool ChangeStatusUpdateShareholder_Delete(string idx)
        {
            try
            {
                Guid shareholderIDX = Guid.Empty;


                Guid Idx = Guid.Parse(idx);
                var del = unitOfWork.CorePersonsUpdatedRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();
                if (del != null)
                {
                    var sh = unitOfWork.vwChangeStatusShareholdersRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();
                    shareholderIDX =  sh.org_shareholder_upd_idx;

                    unitOfWork.CorePersonsUpdatedRepository.Remove(del);
                    unitOfWork.Complete(); 
                                           
                }
                else
                {
                    var delOrg = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => c.organization_upd_idx == Idx).FirstOrDefault();

                    if (delOrg != null)
                    {
                        var sh = unitOfWork.vwChangeStatusShareholdersRepository.Find(c => c.organization_upd_idx == Idx).FirstOrDefault();
                        shareholderIDX = sh.org_shareholder_upd_idx;

                        unitOfWork.CorePersonsUpdatedRepository.Remove(del);
                        unitOfWork.Complete();
                    }
                  
                }

                
                var delShareholderPerson = unitOfWork.CoreOrgShareholdersUpdatedRepository.Find(c => c.org_shareholder_upd_idx == shareholderIDX).FirstOrDefault();
                if (delShareholderPerson != null)
                {
                    unitOfWork.CoreOrgShareholdersUpdatedRepository.Remove(delShareholderPerson);
                    unitOfWork.Complete(); //motacContext.SaveChanges();
                   // return true;
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool ChangeStatusUpdateDirector_Delete(string idx)
        {
            try
            {
                Guid DirectorIDX = Guid.Empty;


                Guid Idx = Guid.Parse(idx);
                var del = unitOfWork.CorePersonsUpdatedRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();
                if (del != null)
                {
                    var sh = unitOfWork.VwChangeStatusDirectorsRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();
                    DirectorIDX = sh.org_director_upd_idx;

                    unitOfWork.CorePersonsUpdatedRepository.Remove(del);
                    unitOfWork.Complete();

                    var delDirector = unitOfWork.CoreOrgDirectorsUpdatedRepository.Find(c => c.org_director_upd_idx == DirectorIDX).FirstOrDefault();
                    if (delDirector != null)
                    {
                        unitOfWork.CoreOrgDirectorsUpdatedRepository.Remove(delDirector);
                        unitOfWork.Complete(); //motacContext.SaveChanges();
                                               // return true;
                    }

                }
                
             


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        //public bool ChangeStatusUpdateDirector_Delete(string idx)
        //{
        //    try
        //    {
        //        Guid directorIDX = Guid.Empty;


        //        Guid Idx = Guid.Parse(idx);
        //        var del = unitOfWork.CorePersonsUpdatedRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();
        //        if (del != null)
        //        {
        //            var sh = unitOfWork.VwChangeStatusDirectorsRepository.Find(c => c.person_upd_idx == Idx).FirstOrDefault();
        //            directorIDX = sh.org_director_upd_idx;

        //            unitOfWork.CorePersonsUpdatedRepository.Remove(del);
        //            unitOfWork.Complete();

        //            var delDirector = unitOfWork.CoreOrgDirectorsUpdatedRepository.Find(c => c.org_director_upd_idx == directorIDX).FirstOrDefault();
        //            if (delDirector != null)
        //            {
        //                unitOfWork.CoreOrgDirectorsUpdatedRepository.Remove(delDirector);
        //                unitOfWork.Complete(); //motacContext.SaveChanges();
        //                                       // return true;
        //            }

        //        }
              

                


        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //}

        public bool UpdateDirector(core_persons person)
        {
            try
            {
                //using (var dbContextTransaction = unitOfWork.CoreOrganizations.TourlistContext.Database.BeginTransaction())
                //{
                Guid Idx = person.person_idx;
                //var upd_person = motacContext.core_persons.Where(c => c.person_idx == Idx).FirstOrDefault();
                var upd_person = unitOfWork.CorePersonsRepository.Find(c => c.person_idx == Idx).FirstOrDefault();

                upd_person.person_gender = person.person_gender;
                upd_person.person_birthday = person.person_birthday;
                upd_person.person_nationality=person.person_nationality;
                upd_person.contact_mobile_no = person.contact_mobile_no;
                upd_person.contact_phone_no = person.contact_phone_no;
                     
                upd_person.modified_by = person.modified_by;
                upd_person.modified_dt = DateTime.Now;
                unitOfWork.CorePersonsRepository.TourlistContext.Entry(upd_person).State = EntityState.Modified;
                unitOfWork.Complete();

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool Acknowledgement_SaveNew(CoreOrganizationModel.CoreAcknowledgement acknowledgement)
        {
            try
            {
                core_acknowledgements new_acknowledgement = new core_acknowledgements(); //motacContext.core_persons.Create();

                Guid gAcknowledgement = Guid.NewGuid();
                new_acknowledgement.acknowledgement_idx = gAcknowledgement;
                new_acknowledgement.stub_ref = acknowledgement.stub_ref;
                new_acknowledgement.acknowledge_person_ref = acknowledgement.acknowledge_person_ref;
                new_acknowledgement.is_acknowledged = 1;
                new_acknowledgement.acknowledge_date=DateTime.Now;
                new_acknowledgement.active_status = 1;
                
                new_acknowledgement.created_at = DateTime.Now;
                new_acknowledgement.modified_at = DateTime.Now;
                new_acknowledgement.created_by = acknowledgement.UserID;
                new_acknowledgement.modified_by = acknowledgement.UserID;
               
                unitOfWork.CoreAcknowledgementsRepository.Add(new_acknowledgement);
                var result = unitOfWork.Complete(); //motacContext.SaveChanges();

               return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool AcknowledgementByname_SaveNew(CoreOrganizationModel.CoreAcknowledgement acknowledgement)
        {
            try
            {
                core_acknowledgements new_acknowledgement = new core_acknowledgements(); //motacContext.core_persons.Create();

                Guid gAcknowledgement = Guid.NewGuid();
                new_acknowledgement.acknowledgement_idx = gAcknowledgement;
                new_acknowledgement.stub_ref = acknowledgement.stub_ref;
                new_acknowledgement.acknowledge_person_name = acknowledgement.name;
                new_acknowledgement.acknowledge_person_icno = acknowledgement.icno;
                new_acknowledgement.acknowledge_position = acknowledgement.position;
                new_acknowledgement.acknowledge_organization_name = acknowledgement.company;
                new_acknowledgement.is_acknowledged = 1;
                new_acknowledgement.acknowledge_date = DateTime.Now;
                new_acknowledgement.active_status = 1;

                new_acknowledgement.created_at = DateTime.Now;
                new_acknowledgement.modified_at = DateTime.Now;
                new_acknowledgement.created_by = acknowledgement.UserID;
                new_acknowledgement.modified_by = acknowledgement.UserID;

                unitOfWork.CoreAcknowledgementsRepository.Add(new_acknowledgement);
                var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool UpdateAcknowledgement(CoreOrganizationModel.CoreAcknowledgement acknowledge)
        {
            try
            {
                Guid Idx = acknowledge.acknowledgement_idx;
                //var upd_person = motacContext.core_persons.Where(c => c.person_idx == Idx).FirstOrDefault();
                var acknowledgement = unitOfWork.CoreAcknowledgementsRepository.Find(c => c.acknowledgement_idx == Idx).FirstOrDefault();
                //core_acknowledgements new_acknowledgement = new core_acknowledgements(); //motacContext.core_persons.Create();

                acknowledgement.acknowledge_person_ref = acknowledge.acknowledge_person_ref;
                acknowledgement.modified_at = DateTime.Now;
                acknowledgement.modified_by = acknowledge.UserID;                
                unitOfWork.CorePersonsRepository.TourlistContext.Entry(acknowledgement).State = EntityState.Modified;
                unitOfWork.Complete();

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateAcknowledgementByName(CoreOrganizationModel.CoreAcknowledgement acknowledge)
        {
            try
            {
                Guid Idx = acknowledge.acknowledgement_idx;             
                var acknowledgement = unitOfWork.CoreAcknowledgementsRepository.Find(c => c.acknowledgement_idx == Idx).FirstOrDefault();
                acknowledgement.acknowledge_person_name = acknowledge.name;
                acknowledgement.acknowledge_person_icno = acknowledge.icno;
                acknowledgement.acknowledge_person_ref = acknowledge.acknowledge_person_ref;
                acknowledgement.acknowledge_position = acknowledge.position;
                acknowledgement.acknowledge_organization_name = acknowledge.company;
                acknowledgement.modified_at = DateTime.Now;
                acknowledgement.modified_by = acknowledge.UserID;
                unitOfWork.CorePersonsRepository.TourlistContext.Entry(acknowledgement).State = EntityState.Modified;
                unitOfWork.Complete();

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public bool updateListing(string item)
        {
            try
            {
                Guid Idx = Guid.Parse(item);
                var upd_item = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == Idx).FirstOrDefault();
                if (upd_item != null)
                {
                    upd_item.bool1 = 1;

                    unitOfWork.CoreChkItemsInstancesRepository.TourlistContext.Entry(upd_item).State = EntityState.Modified;
                    unitOfWork.Complete();
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

        public bool updatePremiseApp(string AppID,string userid)
        {
            try
            {
                Guid Idx = Guid.Parse(AppID);
                Guid userID = Guid.Parse(userid);
                var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "STATUSAWAM").FirstOrDefault();
                var upd_ref_id = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PREMISE_INSPECTION").FirstOrDefault();


                var upd_status = unitOfWork.FlowApplicationStubs.Find(c => c.apply_idx == Idx).FirstOrDefault();
                upd_status.apply_status = upd_ref_id.ref_idx;
                upd_status.modified_by = userID;
                upd_status.modified_dt= DateTime.Now;


                unitOfWork.FlowApplicationStubs.TourlistContext.Entry(upd_status).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool updateTempohPembaharuan(string TempohPembaharuan,string AppID)
        {
            try
            {
                Guid Idx = Guid.Parse(AppID);
                var upd_item = unitOfWork.MM2HLicensesRepository.Find(c => c.stub_ref == Idx).FirstOrDefault();
                upd_item.renewal_duration_years = int.Parse(TempohPembaharuan);

                unitOfWork.MM2HLicensesRepository.TourlistContext.Entry(upd_item).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool updateStatusShareHolder(string status_shareholder, string shareholderID,string registered_year)
        {
            try
            {
                Guid Idx = Guid.Parse(shareholderID);
                var upd_item = unitOfWork.CoreOrganizationShareholders.Find(c => c.organization_shareholder_idx == Idx).FirstOrDefault();
                upd_item.status_shareholder = Guid.Parse(status_shareholder);
                upd_item.registered_year= registered_year;
                unitOfWork.CoreOrganizationShareholders.TourlistContext.Entry(upd_item).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool updateDocumentSokongan(CoreOrganizationModel.CoreDocument doc)
        {
            try
            {
                Guid Idx = Guid.Parse(doc.chkitem_instanceID);
                var upd_item = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == Idx).FirstOrDefault();
                upd_item.date1 = DateTime.Today;
                upd_item.upload_location = doc.UploadLocation;
                upd_item.string1 = doc.FileName;
                upd_item.string2 = doc.UploadFileName;
                
                unitOfWork.CoreChkItemsInstancesRepository.TourlistContext.Entry(upd_item).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool OtherDocument_SaveNew(CoreOrganizationModel.CoreOtherDocument doc)
        {
            try
            {
                core_uploads_freeform_by_modules otherDoc = new core_uploads_freeform_by_modules(); //motacContext.core_persons.Create();

                Guid gdoc = Guid.NewGuid();
                otherDoc.uploads_freeform_by_modules_idx = gdoc;
                otherDoc.document_upload_date= DateTime.Now;
                otherDoc.document_name = doc.document_name;
                otherDoc.document_description = doc.document_description;
                otherDoc.document_upload_path = doc.document_upload_path;
                otherDoc.module_ref = Guid.Parse(doc.module_ref);
                otherDoc.transaction_ref = Guid.Parse(doc.transaction_ref);

                otherDoc.created_at = DateTime.Now;
                otherDoc.modified_at = DateTime.Now;
                otherDoc.created_by = Guid.Parse(doc.UserID);
                otherDoc.modified_by = Guid.Parse(doc.UserID);

                unitOfWork.CoreUploadsFreeformModulesRepository.Add(otherDoc);
                var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool DocumentPPP_SaveNew(CoreOrganizationModel.CoreOtherDocument doc)
        {
            try
            {
                core_uploads_freeform_by_modules otherDoc = new core_uploads_freeform_by_modules(); //motacContext.core_persons.Create();

                Guid gdoc = Guid.NewGuid();
                otherDoc.uploads_freeform_by_modules_idx = gdoc;
                otherDoc.document_upload_date = DateTime.Now;
                otherDoc.document_name = doc.document_name;
                otherDoc.document_description = doc.document_description;
                otherDoc.document_upload_path = doc.document_upload_path;
                otherDoc.module_ref = Guid.Parse(doc.module_ref);
                //otherDoc.transaction_ref = Guid.Parse(doc.transaction_ref);
                otherDoc.ppp_grading_ref = Guid.Parse(doc.ppp_grading_ref);

                otherDoc.created_at = DateTime.Now;
                otherDoc.modified_at = DateTime.Now;
                otherDoc.created_by = Guid.Parse(doc.UserID);
                otherDoc.modified_by = Guid.Parse(doc.UserID);

                unitOfWork.CoreUploadsFreeformModulesRepository.Add(otherDoc);
                var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public bool UpdateOtherDocument(CoreOrganizationModel.CoreOtherDocument doc)
        {
            try
            {
                //core_uploads_freeform_by_modules otherDoc = new core_uploads_freeform_by_modules(); //motacContext.core_persons.Create();

                Guid Idx = Guid.Parse(doc.docID);
                var otherDoc = unitOfWork.CoreUploadsFreeformModulesRepository.Find(c => c.uploads_freeform_by_modules_idx == Idx).FirstOrDefault();
                              
                otherDoc.document_upload_date = DateTime.Now;                
                otherDoc.document_description = doc.document_description;
                otherDoc.document_upload_path = doc.document_upload_path;
                otherDoc.module_ref = Guid.Parse(doc.module_ref);
                otherDoc.transaction_ref = Guid.Parse(doc.transaction_ref);
                otherDoc.modified_at = DateTime.Now;             
                otherDoc.modified_by = Guid.Parse(doc.UserID);

                unitOfWork.CoreUploadsFreeformModulesRepository.TourlistContext.Entry(otherDoc).State = EntityState.Modified;
                unitOfWork.Complete();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdatePPPDocument(CoreOrganizationModel.CoreOtherDocument doc)
        {
            try
            {
                //core_uploads_freeform_by_modules otherDoc = new core_uploads_freeform_by_modules(); //motacContext.core_persons.Create();

                Guid Idx = Guid.Parse(doc.docID);
                var otherDoc = unitOfWork.CoreUploadsFreeformModulesRepository.Find(c => c.uploads_freeform_by_modules_idx == Idx).FirstOrDefault();

                otherDoc.document_upload_date = DateTime.Now;
                otherDoc.document_description = doc.document_description;
                otherDoc.document_upload_path = doc.document_upload_path;
                otherDoc.module_ref = Guid.Parse(doc.module_ref);
                otherDoc.transaction_ref = Guid.Parse(doc.transaction_ref);
                otherDoc.modified_at = DateTime.Now;
                otherDoc.modified_by = Guid.Parse(doc.UserID);

                unitOfWork.CoreUploadsFreeformModulesRepository.TourlistContext.Entry(otherDoc).State = EntityState.Modified;
                unitOfWork.Complete();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteOtherDocument(string idx)
        {
            try
            {
                Guid Idx = Guid.Parse(idx);
                var del = unitOfWork.CoreUploadsFreeformModulesRepository.Find(c => c.uploads_freeform_by_modules_idx == Idx).FirstOrDefault();

                if (del != null)
                {
                    unitOfWork.CoreUploadsFreeformModulesRepository.Remove(del);
                    unitOfWork.Complete(); //motacContext.SaveChanges();
                   
                }

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteDocumentPasPengajian(string idx)
        {
            try
            {
                Guid Idx = Guid.Parse(idx);
                var del = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.uploads_freeform_by_persons_idx == Idx).FirstOrDefault();

                if (del != null)
                {
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Remove(del);
                    unitOfWork.Complete(); //motacContext.SaveChanges();

                }

                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public bool updateDocumentSokonganPerson(CoreOrganizationModel.CoreDocument doc)
        {
            try
            {
                
                string type = doc.type;

                var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

                var upd_type = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == type && c.ref_type==refType.ref_idx).FirstOrDefault();

                Guid typeID = Guid.Empty;

                if (upd_type != null)
                {
                    typeID = upd_type.ref_idx;
                
                }

                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "ACTIVE").FirstOrDefault();

                Guid activeID = Guid.Empty;
                if (upd_active != null)
                {
                    activeID = upd_active.status_idx;
                    
                }


                Guid Idx = Guid.Parse(doc.PersonID);
                var upd_person = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == Idx && c.upload_type_ref== typeID).FirstOrDefault();

                if (upd_person == null)
                {
                    core_uploads_freeform_by_persons upd_item = new core_uploads_freeform_by_persons();
                    upd_item.created_at = DateTime.Now;
                    upd_item.created_by = Guid.Parse(doc.userID);
                    upd_item.modified_at = DateTime.Now;
                    upd_item.modified_by = Guid.Parse(doc.userID);
                    upd_item.upload_path = doc.UploadLocation;
                    upd_item.upload_name = doc.FileName;
                    upd_item.upload_description = doc.UploadFileName;
                    upd_item.person_ref = Guid.Parse(doc.PersonID);
                    upd_item.upload_type_ref = typeID;
                    upd_item.active_status = activeID;
                    
                    Guid Idx1 = Guid.NewGuid();
                    upd_item.uploads_freeform_by_persons_idx = Idx1;
                    
                    unitOfWork.CoreUploadsFreeFormPersonRepository.Add(upd_item);
                    unitOfWork.Complete();

                }
                else
                {                 
                    upd_person.modified_at = DateTime.Now;
                    upd_person.modified_by = Guid.Parse(doc.userID);
                    upd_person.upload_path = doc.UploadLocation;
                    upd_person.upload_name = doc.FileName;
                    upd_person.upload_description = doc.UploadFileName;
                    upd_person.person_ref = Guid.Parse(doc.PersonID);
                    upd_person.upload_type_ref = typeID;
                    upd_person.active_status = activeID;
                    //upd_item.uploads_freeform_by_persons_idx = upd_person.uploads_freeform_by_persons_idx;
                    unitOfWork.CoreUploadsFreeFormPersonRepository.TourlistContext.Entry(upd_person).State = EntityState.Modified;

                    unitOfWork.Complete();
                }
                           
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool updateDocumentSokonganOrganization(CoreOrganizationModel.CoreDocument doc)
        {
            try
            {

                string type = doc.type;

                var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

                var upd_type = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == type && c.ref_type == refType.ref_idx).FirstOrDefault();

                Guid typeID = Guid.Empty;

                if (upd_type != null)
                {
                    typeID = upd_type.ref_idx;

                }

                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "ACTIVE").FirstOrDefault();

                Guid activeID = Guid.Empty;
                if (upd_active != null)
                {
                    activeID = upd_active.status_idx;

                }


                Guid Idx = Guid.Parse(doc.OrgID);
                var upd_org = unitOfWork.CoreUploadsFreeformOrganizationsRepository.Find(c => c.organization_ref == Idx && c.upload_type_ref == typeID).FirstOrDefault();

                if (upd_org == null)
                {
                    core_uploads_freeform_by_organizations upd_item = new core_uploads_freeform_by_organizations();
                    upd_item.created_at = DateTime.Now;
                    upd_item.created_by = Guid.Parse(doc.userID);
                    upd_item.modified_at = DateTime.Now;
                    upd_item.modified_by = Guid.Parse(doc.userID);
                    upd_item.upload_path = doc.UploadLocation;
                    upd_item.upload_name = doc.FileName;
                    upd_item.upload_description = doc.UploadFileName;
                    upd_item.organization_ref = Guid.Parse(doc.OrgID);
                    upd_item.upload_type_ref = typeID;
                    upd_item.active_status = activeID;

                    Guid Idx1 = Guid.NewGuid();
                    upd_item.uploads_freeform_by_organizations_idx = Idx1;

                    unitOfWork.CoreUploadsFreeformOrganizationsRepository.Add(upd_item);
                    unitOfWork.Complete();

                }
                else
                {
                    upd_org.modified_at = DateTime.Now;
                    upd_org.modified_by = Guid.Parse(doc.userID);
                    upd_org.upload_path = doc.UploadLocation;
                    upd_org.upload_name = doc.FileName;
                    upd_org.upload_description = doc.UploadFileName;
                    upd_org.organization_ref = Guid.Parse(doc.OrgID);
                    upd_org.upload_type_ref = typeID;
                    upd_org.active_status = activeID;
                    //upd_item.uploads_freeform_by_persons_idx = upd_person.uploads_freeform_by_persons_idx;
                    unitOfWork.CoreUploadsFreeformOrganizationsRepository.TourlistContext.Entry(upd_org).State = EntityState.Modified;

                    unitOfWork.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public string fileLocation(string PersonID)
        {

            Guid Idx = Guid.Parse(PersonID);
            var upd_person = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == Idx).FirstOrDefault();

            string uploadLocation = "";
            if (upd_person != null)
            {
                uploadLocation=upd_person.upload_path + "/" + upd_person.upload_description;
            }
            return uploadLocation;
        }

        public object GetCompany(string RegistrationNo)
        {

            List<core_organizations> Company = new List<core_organizations>();

            List<vw_geo_list> geo_List = new List<vw_geo_list>();
            List<ref_pbt> pbt_List = new List<ref_pbt>();
            //List<vw_geo_list> geo_StateSec = new List<vw_geo_list>();
            //List<vw_geo_list> geo_CityOff = new List<vw_geo_list>();
            //List<vw_geo_list> geo_StateOff = new List<vw_geo_list>();
            //List<vw_geo_list> geo_CityReg = new List<vw_geo_list>();
            //List<vw_geo_list> geo_StateReg = new List<vw_geo_list>();

            //DataModels.core_organizations model = new DataModels.core_organizations();

            Company = unitOfWork.CoreOrganizations.Find(c => (c.organization_identifier == RegistrationNo)).ToList();
            var CitySec = "";
            var sCitySec = "";
            var sStateSec = "";
            foreach (var citystate in Company)
            {
                CitySec = citystate.cosec_city.ToString();
            }
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.town_idx.ToString() == CitySec))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            foreach (var gCitySec in geo_List)
            {
                sCitySec = gCitySec.town_name;
                sStateSec = gCitySec.state_name;
            }


            var CityOff = "";
            var sCityOff = "";
            var sStateOff = "";
            foreach (var citystate in Company)
            {
                CityOff = citystate.office_city.ToString();
            }
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.town_idx.ToString() == CityOff))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            foreach (var gCityOff in geo_List)
            {
                sCityOff = gCityOff.town_name;
                sStateOff = gCityOff.state_name;
            }


            var CityReg = "";
            var sCityReg = "";
            var sStateReg = "";
            foreach (var citystate in Company)
            {
                CityReg = citystate.registered_city.ToString();
            }
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.town_idx.ToString() == CityReg))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            foreach (var gCityReg in geo_List)
            {
                sCityReg = gCityReg.town_name;
                sStateReg = gCityReg.state_name;
            }

            //pbt_List = unitOfWork.RefPbtRepository
            //    .Find(c => (c.pbt_idx.ToString() == sStateReg))                               
            //    .ToList();
            //var sPbt = "";
            //foreach (var gPBT in pbt_List)
            //{
            //    sPbt = gPBT.pbt_name;
             
            //}



            var clsApp = (from company in Company                         
                          .ToList()
                          select new
                          {
                              organization_idx = company.organization_idx.ToString(),
                              authorized_capital = company.authorized_capital,
                              cosec_addr_1 = company.cosec_addr_1,
                              cosec_addr_2 = company.cosec_addr_2,
                              cosec_addr_3 = company.cosec_addr_3,
                              cosec_city = company.cosec_city,
                              cosec_city_name = sCitySec,
                              cosec_state = company.cosec_state,
                              cosec_state_name = sStateSec,
                              cosec_email = company.cosec_email,
                              cosec_fax_no = company.cosec_fax_no,
                              cosec_mobile_no = company.cosec_mobile_no,
                              cosec_name = company.cosec_name,
                              cosec_phone_no = company.cosec_phone_no,
                              cosec_postcode = company.cosec_postcode,

                              incorporation_date = String.Format("{0:dd/MM/yyyy}", company.incorporation_date),
                              office_addr_1 = company.office_addr_1,
                              office_addr_2 = company.office_addr_2,
                              office_addr_3 = company.office_addr_3,
                              office_city = company.office_city,
                              office_city_name = sCityOff,
                              office_email = company.office_email,
                              office_fax_no = company.office_fax_no,
                              office_mobile_no = company.office_mobile_no,
                              office_phone_no = company.office_phone_no,
                              office_postcode = company.office_postcode,
                              office_state = company.office_state,
                              office_state_name = sStateOff,
                              organization_identifier = company.organization_identifier,
                              country_ref = company.country_ref,
                              organization_name = company.organization_name,
                              organization_status = company.organization_status,
                              organization_type = company.organization_type,
                              paid_capital = company.paid_capital,

                              registered_addr_1 = company.registered_addr_1,
                              registered_addr_2 = company.registered_addr_2,
                              registered_addr_3 = company.registered_addr_3,
                              registered_city = company.registered_city,
                              registered_city_name = sCityReg,
                              registered_email = company.registered_email,
                              registered_fax_no = company.registered_fax_no,
                              registered_mobile_no = company.registered_mobile_no,
                              registered_phone_no = company.registered_phone_no,
                              registered_postcode = company.registered_postcode,
                              registered_state = company.registered_state,
                              nature_of_business = company.nature_of_business,
                              registered_state_name = sStateReg,
                              website = company.website,
                              pbt_ref = company.pbt_ref,
                              is_has_business_address = company.is_has_business_address
                          }).ToList();

            return clsApp;

        }

        public object GetCompanyByID(string OrganizationID)
        {

            List<core_organizations> Company = new List<core_organizations>();

            List<vw_geo_list> geo_List = new List<vw_geo_list>();
           
            Company = unitOfWork.CoreOrganizations.Find(c => (c.organization_idx.ToString() == OrganizationID)).ToList();
            var CitySec = "";
            var sCitySec = "";
            var sStateSec = "";
            foreach (var citystate in Company)
            {
                CitySec = citystate.cosec_city.ToString();
            }
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.town_idx.ToString() == CitySec))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            foreach (var gCitySec in geo_List)
            {
                sCitySec = gCitySec.town_name;
                sStateSec = gCitySec.state_name;
            }


            var CityOff = "";
            var sCityOff = "";
            var sStateOff = "";
            foreach (var citystate in Company)
            {
                CityOff = citystate.office_city.ToString();
            }
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.town_idx.ToString() == CityOff))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            foreach (var gCityOff in geo_List)
            {
                sCityOff = gCityOff.town_name;
                sStateOff = gCityOff.state_name;
            }


            var CityReg = "";
            var sCityReg = "";
            var sStateReg = "";
            foreach (var citystate in Company)
            {
                CityReg = citystate.registered_city.ToString();
            }
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.town_idx.ToString() == CityReg))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            foreach (var gCityReg in geo_List)
            {
                sCityReg = gCityReg.town_name;
                sStateReg = gCityReg.state_name;
            }

            var clsApp = (from company in Company
                          .ToList()
                          select new
                          {
                              organization_idx = company.organization_idx.ToString(),
                              authorized_capital = company.authorized_capital,
                              cosec_addr_1 = company.cosec_addr_1,
                              cosec_addr_2 = company.cosec_addr_2,
                              cosec_addr_3 = company.cosec_addr_3,
                              cosec_city = company.cosec_city,
                              cosec_city_name = sCitySec,
                              cosec_state = company.cosec_state,
                              cosec_state_name = sStateSec,
                              cosec_email = company.cosec_email,
                              cosec_fax_no = company.cosec_fax_no,
                              cosec_mobile_no = company.cosec_mobile_no,
                              cosec_name = company.cosec_name,
                              cosec_phone_no = company.cosec_phone_no,
                              cosec_postcode = company.cosec_postcode,

                              incorporation_date = String.Format("{0:dd/MM/yyyy}", company.incorporation_date),
                              office_addr_1 = company.office_addr_1,
                              office_addr_2 = company.office_addr_2,
                              office_addr_3 = company.office_addr_3,
                              office_city = company.office_city,
                              office_city_name = sCityOff,
                              office_email = company.office_email,
                              office_fax_no = company.office_fax_no,
                              office_mobile_no = company.office_mobile_no,
                              office_phone_no = company.office_phone_no,
                              office_postcode = company.office_postcode,
                              office_state = company.office_state,
                              office_state_name = sStateOff,
                              organization_identifier = company.organization_identifier,
                              country_ref = company.country_ref,
                              organization_name = company.organization_name,
                              organization_status = company.organization_status,
                              organization_type = company.organization_type,
                              paid_capital = company.paid_capital,

                              registered_addr_1 = company.registered_addr_1,
                              registered_addr_2 = company.registered_addr_2,
                              registered_addr_3 = company.registered_addr_3,
                              registered_city = company.registered_city,
                              registered_city_name = sCityReg,
                              registered_email = company.registered_email,
                              registered_fax_no = company.registered_fax_no,
                              registered_mobile_no = company.registered_mobile_no,
                              registered_phone_no = company.registered_phone_no,
                              registered_postcode = company.registered_postcode,
                              registered_state = company.registered_state,
                              registered_state_name = sStateReg,
                              website = company.website,

                          }).ToList();

            return clsApp;

        }

        public Guid GetChkitemInstanceIdx(double orderx, Guid stubRef, string module)
        {

            var coreChklstLists = new core_chklst_lists();
            var coreChkLstItems = new core_chklst_items(); 
            var coreChkitemsInstances = new core_chkitems_instances();
            if (module == "mm2h")
            {
                var mm2hLicense = unitOfWork.MM2HLicensesRepository.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "MM2H_CHANGESTATUS_DOKUMEN")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.orderx == orderx)).FirstOrDefault();
                coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == mm2hLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();

            }else if(module == "tobtab")
            {
                var tobtabLicense = unitOfWork.TobtabLicenses.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "TOBTAB_CHANGE_STATUS_DOCUMENT")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.orderx == orderx)).FirstOrDefault();
                coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == tobtabLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();

            }
            else if (module == "ilp")
            {
                var ilpLicense = unitOfWork.IlpLicenses.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "ILP_TUKAR_STATUS_DOKUMEN")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.orderx == orderx)).FirstOrDefault();
                coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == ilpLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();

            }
            return coreChkitemsInstances.chkitem_instance_idx;

        }

        public Guid GetChkitemInstanceIdxByCode(string code, Guid stubRef, string module)
        {

            var coreChklstLists = new core_chklst_lists();
            var coreChkLstItems = new core_chklst_items();
            var coreChkitemsInstances = new core_chkitems_instances();
            if (module == "mm2h")
            {
                var mm2hLicense = unitOfWork.MM2HLicensesRepository.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "MM2H_CHANGESTATUS_DOKUMEN")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();
                coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == mm2hLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();
                if (coreChkitemsInstances == null) return Guid.Empty; //added by samsuri on 26 jan 2024
            }
            else if (module == "tobtab")
            {
                var tobtabLicense = unitOfWork.TobtabLicenses.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "TOBTAB_CHANGE_STATUS_DOCUMENT")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();
                coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == tobtabLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();
                if (coreChkitemsInstances == null) return Guid.Empty; //added by samsuri on 10 jan 2024
            }
            else if (module == "ilp")
            {
                var ilpLicense = unitOfWork.IlpLicenses.Find(c => (c.stub_ref == stubRef)).FirstOrDefault();
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "ILP_TUKAR_STATUS_DOKUMEN")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();
                coreChkitemsInstances = unitOfWork.CoreChkItemsInstancesRepository.Find(c => (c.chklist_instance_ref == ilpLicense.supporting_document_list && c.chklist_tplt_item_ref == coreChkLstItems.item_idx)).FirstOrDefault();
                if (coreChkitemsInstances == null) return Guid.Empty; //added by samsuri on 10 jan 2024
            }
            return coreChkitemsInstances.chkitem_instance_idx;

        }

        //added by samsuri (CR#57258) (CR#57259) (CR#58741) on 12 jan 2024
        public Guid GetcoreChkitemsInstancesByCode(string code, string module)
        {
            var coreChklstLists = new core_chklst_lists();
            var coreChkLstItems = new core_chklst_items();
            if (module == "mm2h")
            {
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "MM2H_CHANGESTATUS_DOKUMEN")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();
                if (coreChkLstItems == null) return Guid.Empty;
            }
            else if (module == "tobtab")
            {                
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "TOBTAB_CHANGE_STATUS_DOCUMENT")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();                
                if (coreChkLstItems == null) return Guid.Empty; 
            }
            else if (module == "ilp")
            {
                coreChklstLists = unitOfWork.CoreChklstListsRepository.Find(c => (c.chklist_code.ToString() == "ILP_TUKAR_STATUS_DOKUMEN")).FirstOrDefault();
                coreChkLstItems = unitOfWork.CoreChklstItemsRepository.Find(c => (c.chklist_ref == coreChklstLists.chklist_idx && c.descr_string1 == code)).FirstOrDefault();                
                if (coreChkLstItems == null) return Guid.Empty; 
            }
            return coreChkLstItems.item_idx;
        }

        public object GetOrganizationUpdate(Guid AppID, string module)
        {
            List<core_organizations_updated> CompanyUpdate = new List<core_organizations_updated>();
            CompanyUpdate = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => (c.stub_ref == AppID)).ToList();
           
           // var coreChklistInstances = GetChkitemInstanceIdx("Perjanjian Sewa Beli Premis", AppID);
            var coreChklistInstances = unitOfWork.CoreChkListInstancesRepository.Find(c => c.application_ref == AppID).ToList();

            var chkitem_instanceSewaBeliPremis = GetChkitemInstanceIdx(2, AppID, module);//"Perjanjian Sewa Beli Premis"
            var perjanjianSewaBeliPremis = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSewaBeliPremis).FirstOrDefault();
           
            var chkitem_instanceSalinanAsalLesen = GetChkitemInstanceIdx(0, AppID, module);//"Salinan Lesen *"
            var salinanAsalLesen = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSalinanAsalLesen).FirstOrDefault();
          
            var chkitem_instancePelanLantai = GetChkitemInstanceIdx(3, AppID, module);//"Pelan Lantai Premis Perniagaan"
            var pelanLantaiPremisPerniagaan = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instancePelanLantai).FirstOrDefault();

            var chkitem_instanceSSM = GetChkitemInstanceIdx(4, AppID, module);//"Perakuan Pendaftaran Syarikat dan lesen (SSM yang lama)"
            var SSM = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSSM).FirstOrDefault();


            RefGeoHelper geo = new RefGeoHelper();
            var Company = (from company in CompanyUpdate
                         
                          select new
                          {

                              new_organization_name = company.new_organization_name,
                              new_justification_name=company.new_justification_name,
                              new_paid_capital =company.new_paid_capital,
                              new_addr_1=company.new_addr_1,
                              new_addr_2=company.new_addr_2,
                              new_addr_3=company.new_addr_3,
                              new_postcode=company.new_postcode,
                              new_city=company.new_city,
                              new_state=company.new_state,
                              new_mobile_no=company.new_mobile_no,
                              new_phone_no=company.new_phone_no,
                              new_fax_no=company.new_fax_no,
                              new_email=company.new_email,
                              new_website=company.new_website,
                              old_organization_name=company.old_organization_name,
                              old_paid_capital=company.old_paid_capital,
                              old_addr_1=company.old_addr_1,
                              old_addr_2= company.old_addr_2,
                              old_addr_3= company.old_addr_3,
                              old_postcode= company.old_postcode,
                              old_city= geo.GetGuidTownByIdx((Guid) company.old_city),
                              old_state=geo.GetGuidStateByIdx((Guid)company.old_state),
                              old_mobile_no = company.old_mobile_no,
                              old_phone_no= company.old_phone_no,
                              old_fax_no= company.old_fax_no,
                              old_email=company.old_email,
                              old_website=company.old_website,
                              is_change_name=company.is_change_name,
                              is_change_address=company.is_change_address,
                              is_change_capital=company.is_change_capital,
                              is_shareholder=company.is_shareholder,
                              is_directors=company.is_directors,

                              fileNamesalinanAsalLesen = salinanAsalLesen == null ? "" : salinanAsalLesen.string1,
                              fileLocsalinanAsalLesen = salinanAsalLesen == null ? "" : salinanAsalLesen.upload_location,
                              fileNameSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.string1,
                              fileLocSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.upload_location,
                              fileNamepelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.string1,
                              fileLocpelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.upload_location,
                              fileSSM = SSM == null ? "" : SSM.string1,
                              fileLocSSM = SSM == null ? "" : SSM.upload_location,

                              //locationFile = myKad == null ? "" : myKad.upload_path,

                              //fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                              //locationFilePas = paspengajian == null ? "" : paspengajian.upload_path


                          }).ToList();

                         
            return Company;

        }

        public object GetOrganizationUpdateByCode(Guid AppID, string module)
        {
            List<core_organizations_updated> CompanyUpdate = new List<core_organizations_updated>();
            CompanyUpdate = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => (c.stub_ref == AppID)).ToList();

            // var coreChklistInstances = GetChkitemInstanceIdx("Perjanjian Sewa Beli Premis", AppID);
            var coreChklistInstances = unitOfWork.CoreChkListInstancesRepository.Find(c => c.application_ref == AppID).ToList();

            var chkitem_instanceSewaBeliPremis = GetChkitemInstanceIdxByCode("SEWABELI", AppID, module);//"Perjanjian Sewa Beli Premis"
            var perjanjianSewaBeliPremis = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSewaBeliPremis).FirstOrDefault();

            var chkitem_instanceSalinanAsalLesen = GetChkitemInstanceIdxByCode("SALINANLESEN", AppID, module);//"Salinan Lesen *"
            var salinanAsalLesen = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSalinanAsalLesen).FirstOrDefault();

            var chkitem_instancePelanLantai = GetChkitemInstanceIdxByCode("PELANLANTAI", AppID, module);//"Pelan Lantai Premis Perniagaan"
            var pelanLantaiPremisPerniagaan = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instancePelanLantai).FirstOrDefault();

            var chkitem_instanceGambar = GetChkitemInstanceIdxByCode("GAMBAR", AppID, module);//"Gambar Bahagian Dalam dan Luar Pejabat (Berwarna)"
            var gambar = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceGambar).FirstOrDefault();

            //var chkitem_instanceSSM = GetChkitemInstanceIdxByCode("", AppID, module);//"Perakuan Pendaftaran Syarikat dan lesen (SSM yang lama)"
            //var SSM = unitOfWork.CoreChkItemsInstancesRepository.Find(c => c.chkitem_instance_idx == chkitem_instanceSSM).FirstOrDefault();


            RefGeoHelper geo = new RefGeoHelper();
            var Company = (from company in CompanyUpdate

                           select new
                           {

                               new_organization_name = company.new_organization_name,
                               new_justification_name = company.new_justification_name,
                               new_paid_capital = company.new_paid_capital,
                               new_addr_1 = company.new_addr_1,
                               new_addr_2 = company.new_addr_2,
                               new_addr_3 = company.new_addr_3,
                               new_postcode = company.new_postcode,
                               new_city = company.new_city != null ? geo.GetGuidTownByIdx((Guid)company.new_city) : "",
                               new_state = company.new_state != null ? geo.GetGuidStateByIdx((Guid)company.new_state) : "", //hide by Rahimi log 46406 - PROD_TOBTAB_TUKAR STATUS
                               new_mobile_no = company.new_mobile_no,
                               new_phone_no = company.new_phone_no,
                               new_fax_no = company.new_fax_no,
                               new_email = company.new_email,
                               new_website = company.new_website,
                               old_organization_name = company.old_organization_name,
                               old_paid_capital = company.old_paid_capital,
                               old_addr_1 = company.old_addr_1,
                               old_addr_2 = company.old_addr_2,
                               old_addr_3 = company.old_addr_3,
                               old_postcode = company.old_postcode,
                               old_city_idx = company.old_city, //added by samsuri (CR#57259) on 29 Dec 2023
                               old_city = company.old_city != null ? geo.GetGuidTownByIdx((Guid)company.old_city) : "",
                               old_state = company.old_state != null ? geo.GetGuidStateByIdx((Guid)company.old_state) : "",
                               old_mobile_no = company.old_mobile_no,
                               old_phone_no = company.old_phone_no,
                               old_fax_no = company.old_fax_no,
                               old_email = company.old_email,
                               old_website = company.old_website,
                               is_change_name = company.is_change_name,
                               is_change_address = company.is_change_address,
                               is_change_capital = company.is_change_capital,
                               is_shareholder = company.is_shareholder,
                               is_directors = company.is_directors,
                               is_premise_ready=company.is_premise_ready,
                               fileNamesalinanAsalLesen = salinanAsalLesen == null ? "" : salinanAsalLesen.string1,
                               fileLocsalinanAsalLesen = salinanAsalLesen == null ? "" : salinanAsalLesen.upload_location,
                               fileNameSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.string1,
                               fileLocSewaBeliPremis = perjanjianSewaBeliPremis == null ? "" : perjanjianSewaBeliPremis.upload_location,
                               fileNamepelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.string1,
                               fileLocpelanLantai = pelanLantaiPremisPerniagaan == null ? "" : pelanLantaiPremisPerniagaan.upload_location,
                               fileNameGambarPermis = gambar == null ? "" : gambar.string1,
                               FilelocGambarPermis = gambar == null ? "" : gambar.upload_location,

                               //locationFile = myKad == null ? "" : myKad.upload_path,

                               //fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                               //locationFilePas = paspengajian == null ? "" : paspengajian.upload_path


                           }).ToList();


            return Company;

        }

        public List<CoreOrganizationModel.CoreOrg> GetOrgHeader(string sUserID)
        {

            List<core_users> users = new List<core_users>();
            users = unitOfWork.CoreUsersRepository.Find(i => (i.user_idx.ToString() == sUserID)).ToList();

            string sOrg = "";
            foreach (var user in users)
            {
                sOrg=user.person_ref.ToString();
            }


            List<core_organizations> organizations = new List<core_organizations>();
            List<CoreOrganizationModel.CoreOrg> org = new List<CoreOrganizationModel.CoreOrg>();
            organizations = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == sOrg)).ToList();

            if(organizations.Count == 0)
            {
                core_persons core_persons = unitOfWork.CorePersonsRepository.Find(x => x.person_idx.ToString() == sOrg).FirstOrDefault();
                org.Add(new CoreOrganizationModel.CoreOrg
                {
                    NamaSyarikat = core_persons.person_name,
                    NoPendaftaranSyarikat = core_persons.person_identifier,
                    OrganizationID = core_persons.person_idx.ToString(),
                    Email = core_persons.contact_email,

                });
            }
            else
            {
                var clsOrg = (from user in users
                              from organization in organizations
                              .Where(d => d.organization_idx.ToString() == user.person_ref.ToString())
                              .DefaultIfEmpty() // <== makes join left join  

                              select new
                              {
                                  NamaSyarikat = organization.organization_name,
                                  NoPendaftaranSyarikat = organization.organization_identifier,
                                  OrganizationID = organization.organization_idx,
                                  Email = organization.office_email//,
                                                                     //organization.authorized_capital
                              }).ToList();


                foreach (var app in clsOrg)
                {
                    org.Add(new CoreOrganizationModel.CoreOrg
                    {
                        NamaSyarikat = app.NamaSyarikat,
                        NoPendaftaranSyarikat = app.NoPendaftaranSyarikat,
                        OrganizationID = app.OrganizationID.ToString(),
                        Email = app.Email,

                    });
                }
            }
            return org;


        }

        public List<CoreOrganizationModel.core_org_shareholder> GetShareHolderList(string OrgID)
        {
            /*var clsDoc = unitOfWork.RefPersonIdentifierTypesRepository
               .Find(x => x.person_identifier_name.ToString() == "MYKAD")
               .FirstOrDefault();*/

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();


           // var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD").FirstOrDefault();

            var clsshareholder = unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_ref.ToString() == OrgID && x.active_status == 1)
               .ToList(); //&& (x.upload_type_ref == mykad.ref_idx || x.upload_type_ref == null)

            decimal sumNumberOfShare = clsshareholder.Sum(d => d.number_of_shares);

            List<CoreOrganizationModel.core_org_shareholder> modelList = new List<CoreOrganizationModel.core_org_shareholder>();

            foreach (var shareholder in clsshareholder)
            {
                CoreOrganizationModel.core_org_shareholder model = new CoreOrganizationModel.core_org_shareholder();
                {
                    model.organization_shareholder_idx = shareholder.organization_shareholder_idx;
                    model.organization_name = shareholder.organization_name;
                    model.status_pegangan = shareholder.status_pegangan;
                    decimal share = (shareholder.number_of_shares/ sumNumberOfShare * 100);
                    model.number_of_shares = shareholder.number_of_shares;
                    model.share_percentage = share.ToString("n2");
                    if (shareholder.person_identifier != null)
                    {
                        model.person_name = shareholder.person_name;
                        model.person_identifier = shareholder.person_identifier;
                        model.organization_identifier = shareholder.organization_identifier;
                        model.type = "Person";
                    }
                    else
                    {
                        core_organizations coreOrg = unitOfWork.CoreOrganizations.Find(i => i.organization_idx == shareholder.organization_idx).FirstOrDefault();
                        model.person_name = coreOrg.organization_name;
                        model.person_identifier = coreOrg.organization_identifier;
                        model.organization_identifier = shareholder.organization_identifier;
                        model.type = "Organization";
                    }
                    if (shareholder.active_status == 1)
                    {
                        model.active_status = "Active";
                    }
                    else
                    {
                        model.active_status = "Inactive";
                    }
                        
                     
                }
                modelList.Add(model);
            }

            return modelList;
        }

        public List<CoreOrganizationModel.core_org_shareholder> GetShareHolderMM2HList(string OrgID)
        {
            /*var clsDoc = unitOfWork.RefPersonIdentifierTypesRepository
               .Find(x => x.person_identifier_name.ToString() == "MYKAD")
               .FirstOrDefault();*/

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();


            // var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD").FirstOrDefault();

            var clsshareholder = unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_ref.ToString() == OrgID && (x.upload_type_ref == mykad.ref_idx || x.upload_type_ref == null) && x.active_status == 1)
               .ToList();

            decimal sumNumberOfShare = clsshareholder.Sum(d => d.number_of_shares);

            List<CoreOrganizationModel.core_org_shareholder> modelList = new List<CoreOrganizationModel.core_org_shareholder>();

            foreach (var shareholder in clsshareholder)
            {
                CoreOrganizationModel.core_org_shareholder model = new CoreOrganizationModel.core_org_shareholder();
                {
                    model.organization_shareholder_idx = shareholder.organization_shareholder_idx;
                    model.organization_name = shareholder.organization_name;
                    model.status_pegangan = shareholder.status_pegangan;
                    decimal share = (shareholder.number_of_shares / sumNumberOfShare * 100);
                    model.number_of_shares = shareholder.number_of_shares;
                    model.share_percentage = share.ToString("n2");
                    if (shareholder.person_identifier != null)
                    {
                        model.person_name = shareholder.person_name;
                        model.person_identifier = shareholder.person_identifier;
                        model.organization_identifier = shareholder.organization_identifier;
                        model.type = "Person";
                    }
                    else
                    {
                        core_organizations coreOrg = unitOfWork.CoreOrganizations.Find(i => i.organization_idx == shareholder.organization_idx).FirstOrDefault();
                        //model.person_name = coreOrg.organization_name;
                        //model.person_identifier = coreOrg.organization_identifier;
                        model.organization_name = coreOrg.organization_name;
                        model.organization_identifier = coreOrg.organization_identifier;
                        model.organization_idx = shareholder.organization_idx;
                        model.type = "Organization";
                    }
                    if (shareholder.active_status == 1)
                    {
                        model.active_status = "Active";
                    }
                    else
                    {
                        model.active_status = "Inactive";
                    }


                }
                modelList.Add(model);
            }

            return modelList;
        }

        public List<vw_change_status_shareholders> GetChangeStatusShareHolder(string AppID)
        {
            return unitOfWork.vwChangeStatusShareholdersRepository
               .Find(x => x.apply_idx.ToString() == AppID).OrderByDescending(x => x.shareholder_number_of_shares)
               .ToList();

        }

        public List<vw_change_status_directors> GetChangeStatusDirector(string AppID)
        {
            return unitOfWork.VwChangeStatusDirectorsRepository
               .Find(x => x.apply_idx.ToString() == AppID).OrderBy(i => i.old_person_name)
               .ToList();

        }
        public List<vw_core_org_shareholder> GetShareHolder(string OrgID)
        {
            return unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_ref.ToString() == OrgID).OrderByDescending(x => x.number_of_shares)
               .ToList();

        }

        public List<vw_core_org_shareholder> GetShareHolderPerson(string OrgID)
        {
            return unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_ref.ToString() == OrgID && x.person_name != null)
               .ToList();

        }

        public List<core_persons_updated> GetPersonChangeStatusDetail(Guid idx)
        {
            return unitOfWork.CorePersonsUpdatedRepository
          .Find(x => x.person_upd_idx == idx)
          .ToList();


        }
        public List<core_organizations_updated> GetOrgChangeStatusDetail(Guid idx)
        {
            return unitOfWork.CoreOrganizationsUpdatedRepository
          .Find(x => x.organization_upd_idx == idx)
          .ToList();


        }

        public List<mm2h_licenses> GetRenewYear(string AppID)
        {
            return unitOfWork.MM2HLicensesRepository
               .Find(x => x.stub_ref.ToString() == AppID)
               .ToList();

        }

        public int CheckShareHolder(string OrgID)
        {
            var shareholders= unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_ref.ToString() == OrgID && x.status_shareholder == null )
               .ToList();

            return shareholders.Count;

        }

        public int CheckDirector(string OrgID)
        {
            var director = unitOfWork.vwCoreOrgDirectorRepository
               .Find(x => x.organization_ref.ToString() == OrgID && x.nationality == null)
               .ToList();

            return director.Count;

        }

       
        public List<vw_core_org_shareholder> GetShareHolderDetail(string identifier)
        {
            return unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_identifier.ToString().Contains(identifier) || x.person_identifier== identifier)
               .ToList();
           
        }

        public List<vw_core_org_shareholder> GetShareHolderDetailByID(string organization_shareholder_idx)
        {
            return unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_shareholder_idx.ToString()== organization_shareholder_idx)
               .ToList();

        }


        public List<vw_core_org_director> GetDirector(string sOrganization)
        {
            var clsDirector= unitOfWork.vwCoreOrgDirectorRepository
               .Find(x => x.organization_ref.ToString() == sOrganization && x.active_status==1)
               .ToList();

            return clsDirector;
        }

        public List<CoreOrganizationModel.CorePersonAcknowledge> GetDirectorPerakuan(string sOrganization)
        {
            List<TourlistDataLayer.DataModel.core_organizations> organizations = new List<TourlistDataLayer.DataModel.core_organizations>();
            organizations = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == sOrganization)).ToList();

            List<TourlistDataLayer.DataModel.core_organization_directors> director = new List<TourlistDataLayer.DataModel.core_organization_directors>();
            director = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => (i.organization_ref.ToString() == sOrganization && i.active_status == 1)).ToList();

            //Data migrate from SPIP, all director record = inactive
            if(director.Count == 0)
            {
                var directorList = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => i.organization_ref.ToString() == sOrganization).ToList();
                foreach (var app in directorList)
                {
                    core_organization_directors director_inactive = app;
                    director_inactive.active_status = 1;
                    unitOfWork.CoreOrganizationDirectorsRepository.Update(director_inactive);
                }
                unitOfWork.Complete();
            }

           if (director.Count>0)
            {
                /*var clsDirector = (from org in organizations
                                   from dir in director

                                   .Where(d => d.organization_ref == org.organization_idx)
                                   .DefaultIfEmpty() // <== makes join left join  

                                   from person in persons
                                   .Where(p => p.person_idx == dir.person_ref)

                                   select new
                                   {
                                       name = person.person_name,
                                       indentifier = person.person_identifier,
                                       position = person.person_employ_position,
                                       acknowledge_person_ref = person.person_idx,
                                       person_idx = person.person_idx,
                                   }).ToList();*/

                List<CoreOrganizationModel.CorePersonAcknowledge> modelList = new List<CoreOrganizationModel.CorePersonAcknowledge>();

                foreach (var dashboard in director)
                {
                    CoreOrganizationModel.CorePersonAcknowledge model = new CoreOrganizationModel.CorePersonAcknowledge();
                    {
                        core_persons person = unitOfWork.Persons.Find(i => i.person_idx == dashboard.person_ref).FirstOrDefault();
                        model.person_name = person.person_name;
                        model.person_identifier = person.person_identifier;
                        model.person_employ_position = person.person_employ_position;
                        model.person_idx = person.person_idx;

                    }
                    modelList.Add(model);
                }

                return modelList;
            }
            
           return null;

        }

        public List<CoreOrganizationModel.CorePersonAcknowledge> GetDirectorPerakuanDetail(string personID)
        {

            List<TourlistDataLayer.DataModel.core_persons> persons = new List<TourlistDataLayer.DataModel.core_persons>();
            persons = unitOfWork.CorePersonsRepository.Find(i => (i.person_idx.ToString() == personID)).ToList();


            List<CoreOrganizationModel.CorePersonAcknowledge> modelList = new List<CoreOrganizationModel.CorePersonAcknowledge>();

            foreach (var dashboard in persons)
            {
                CoreOrganizationModel.CorePersonAcknowledge model = new CoreOrganizationModel.CorePersonAcknowledge();
                {
                    //model.person_name = dashboard.name;
                    model.person_identifier = dashboard.person_identifier;
                    model.person_employ_position = dashboard.person_employ_position;
                    // model.person_idx = dashboard.person_id;
                }
                modelList.Add(model);
            }

            return modelList;


        }

        public List<TourlistDataLayer.DataModel.core_acknowledgements> GetDirectorPerakuanDetailByAcknowledeID(string acknowledgeID)
        {

            List<TourlistDataLayer.DataModel.core_acknowledgements> persons = new List<TourlistDataLayer.DataModel.core_acknowledgements>();
            persons = unitOfWork.CoreAcknowledgementsRepository.Find(i => (i.acknowledgement_idx.ToString() == acknowledgeID)).ToList();

            List<TourlistDataLayer.DataModel.core_acknowledgements> modelList = new List<TourlistDataLayer.DataModel.core_acknowledgements>();

            foreach (var dashboard in persons)
            {
                TourlistDataLayer.DataModel.core_acknowledgements model = new TourlistDataLayer.DataModel.core_acknowledgements();
                {
                    model.acknowledge_person_name = dashboard.acknowledge_person_name;
                    model.acknowledge_person_icno = dashboard.acknowledge_person_icno;
                    model.acknowledge_position = dashboard.acknowledge_position;

                    // model.person_idx = dashboard.person_id;
                }
                modelList.Add(model);
            }

            return modelList;


        }

        public List<CoreOrganizationModel.CorePersonDoc> GetShareholderDocAttachment(string sOrganization)
        {
            List<TourlistDataLayer.DataModel.core_organizations> organizations = new List<TourlistDataLayer.DataModel.core_organizations>();
            organizations = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == sOrganization)).ToList();

           /* List<TourlistDataLayer.DataModel.core_persons> persons = new List<TourlistDataLayer.DataModel.core_persons>();
            persons = unitOfWork.CorePersonsRepository.GetAll().ToList();*/

            List<TourlistDataLayer.DataModel.core_organization_shareholders> shareholder = new List<TourlistDataLayer.DataModel.core_organization_shareholders>();
            shareholder = unitOfWork.CoreOrganizationShareholders.Find(i => (i.organization_ref.ToString() == sOrganization)).ToList();

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type==refType.ref_idx).FirstOrDefault();
            var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT" && c.ref_type == refType.ref_idx).FirstOrDefault();
            var pasPengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN" && c.ref_type == refType.ref_idx).FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.upload_type_ref == mykad.ref_idx || c.upload_type_ref == passport.ref_idx)).ToList();


            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.upload_type_ref == pasPengajian.ref_idx)).ToList();


            if (shareholder.Count > 0)
            {
                var clsDirector = (from org in organizations
                                   from dir in shareholder

                                   .Where(d => d.organization_ref == org.organization_idx)
                                   .DefaultIfEmpty() // <== makes join left join  

                                  /* from person in persons
                                   .Where(p => p.person_idx == dir.shareholder_person_ref)
                                   .DefaultIfEmpty()*/

                                   from upload in clsUploadPerson
                                   .Where(d => d.person_ref == dir.shareholder_person_ref)
                                   .DefaultIfEmpty() // <== makes join left join
                                                     // 
                                   from paspengajian in clsUploadPasPengajian
                                    .Where(p => p.person_ref == dir.shareholder_person_ref)
                                    .DefaultIfEmpty() // <== makes join left join  

                                   select new
                                   {
                                       // name = person.person_name,
                                       // indentifier = person.person_identifier,
                                       person_idx = dir.shareholder_person_ref,
                                       fileName = upload == null ? "" : upload.upload_name,
                                       locationFile = upload == null ? "" : upload.upload_path,

                                       fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                                       locationFilePas = paspengajian == null ? "" : paspengajian.upload_path
                                       

                                   }).ToList();



                List<CoreOrganizationModel.CorePersonDoc> modelList = new List<CoreOrganizationModel.CorePersonDoc>();

                foreach (var dashboard in clsDirector)
                {
                    CoreOrganizationModel.CorePersonDoc model = new CoreOrganizationModel.CorePersonDoc();
                    {
                        core_persons person = unitOfWork.Persons.Find(i => i.person_idx == dashboard.person_idx).FirstOrDefault();
                        model.fileName = dashboard.fileName;
                        model.indentifier = person.person_identifier;
                        model.locationFile = dashboard.locationFile;
                        model.name = person.person_identifier;
                        model.locationFilePas = dashboard.locationFilePas;
                        model.fileNamePas = dashboard.fileNamePas;

                    }
                    modelList.Add(model);

                }
                if (modelList.Count > 0)
                {
                    return modelList;
                }
                else
                {
                    List<CoreOrganizationModel.CorePersonDoc> modelList1 = new List<CoreOrganizationModel.CorePersonDoc>();

                    return modelList1;
                }

            }

            return null;


        }


        public List<CoreOrganizationModel.CorePersonDoc> GetDirectorAttachment(string sOrganization)
        {
            List<TourlistDataLayer.DataModel.core_organizations> organizations = new List<TourlistDataLayer.DataModel.core_organizations>();
            organizations = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == sOrganization)).ToList();

           /* List<TourlistDataLayer.DataModel.core_persons> persons = new List<TourlistDataLayer.DataModel.core_persons>();
            persons = unitOfWork.CorePersonsRepository.GetAll().ToList();*/

            List<TourlistDataLayer.DataModel.core_organization_directors> director = new List<TourlistDataLayer.DataModel.core_organization_directors>();
            director = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => (i.organization_ref.ToString() == sOrganization && i.active_status==1)).ToList();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD").FirstOrDefault();
            var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT").FirstOrDefault();
            var pasPengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN").FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.upload_type_ref == mykad.ref_idx || c.upload_type_ref == passport.ref_idx)).ToList();


            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.upload_type_ref == pasPengajian.ref_idx)).ToList();


            if (director.Count>0)
            {
                var clsDirector = (from org in organizations
                                   from dir in director

                                   .Where(d => d.organization_ref == org.organization_idx)
                                   .DefaultIfEmpty() // <== makes join left join  

                                  /* from person in persons
                                   .Where(p => p.person_idx == dir.person_ref)
                                   .DefaultIfEmpty()*/

                                   from upload in clsUploadPerson
                                   .Where(d => d.person_ref == dir.person_ref)
                                   .DefaultIfEmpty() // <== makes join left join
                                                     // 
                                   from paspengajian in clsUploadPasPengajian
                                    .Where(p => p.person_ref == dir.person_ref)
                                    .DefaultIfEmpty() // <== makes join left join  

                                   select new
                                   {
                                      /* name = person.person_name,
                                       indentifier = person.person_identifier,*/
                                      person_idx = dir.person_ref,
                                       fileName = upload == null ? "" : upload.upload_name,
                                       locationFile = upload == null ? "" : upload.upload_path,

                                       fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                                       locationFilePas = paspengajian == null ? "" : paspengajian.upload_path

                                   }).ToList();



                List<CoreOrganizationModel.CorePersonDoc> modelList = new List<CoreOrganizationModel.CorePersonDoc>();

                foreach (var dashboard in clsDirector)
                {
                    CoreOrganizationModel.CorePersonDoc model = new CoreOrganizationModel.CorePersonDoc();
                    {
                        core_persons person = unitOfWork.Persons.Find(i => i.person_idx == dashboard.person_idx).FirstOrDefault();
                        model.fileName = dashboard.fileName;
                        model.indentifier = person.person_identifier;
                        model.locationFile = dashboard.locationFile;
                        model.name = person.person_name;
                        model.locationFilePas=dashboard.locationFilePas;
                        model.fileNamePas = dashboard.fileNamePas;

                    }
                    modelList.Add(model);
                  
                }
                if (modelList.Count > 0)
                {
                    return modelList;
                }
                else
                {
                    List<CoreOrganizationModel.CorePersonDoc> modelList1 = new List<CoreOrganizationModel.CorePersonDoc>();

                    return modelList1;
                }

            }
            
            return null;


        }

        public List<CoreOrganizationModel.CorePersonDoc> GetShareHolderAttachment(string sOrganization)
        {

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var clsDoc = unitOfWork.RefReferencesRepository
               .Find(x => x.ref_code.ToString() == "MYKAD" && x.ref_type == refType.ref_idx)
               .FirstOrDefault();

            List<vw_core_org_shareholder> shareholders = new List<vw_core_org_shareholder>();

            shareholders= unitOfWork.VwCoreOrgShareholderRepository
               .Find(x => x.organization_ref.ToString() == sOrganization && (x.upload_type_ref==clsDoc.ref_idx || x.upload_type_ref==null))
               .ToList();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.GetAll().ToList();


            var clsDirector = (from shareholder in shareholders
                               from upload in clsUploadPerson

                               .Where(d => d.person_ref == shareholder.person_idx && d.upload_type_ref==clsDoc.ref_idx)
                               .DefaultIfEmpty() // <== makes join left join  

                               select new
                               {
                                   name = shareholder.person_name == null ? "" : shareholder.person_name,
                                   orgName = shareholder.organization_name == null ? "" : shareholder.organization_name,
                                   indentifier = shareholder.person_identifier,
                                   orgIdentifier = shareholder.organization_identifier == null ? "" : shareholder.organization_identifier,
                                   fileName = upload == null ? "-" : upload.upload_name,
                                   locationFile = upload == null ? "-" : upload.upload_path
                               }).ToList();

            List<CoreOrganizationModel.CorePersonDoc> modelList = new List<CoreOrganizationModel.CorePersonDoc>();

            foreach (var dashboard in clsDirector)
            {
                CoreOrganizationModel.CorePersonDoc model = new CoreOrganizationModel.CorePersonDoc();
                {
                    model.fileName = dashboard.fileName;
                    model.indentifier = dashboard.indentifier;
                    model.locationFile = dashboard.locationFile;
                    model.name = dashboard.name;
                    if(dashboard.name == "") { 
                        model.name = dashboard.orgName;
                        model.indentifier = dashboard.orgIdentifier;
                    }
                }
                modelList.Add(model);
            }

            return modelList;


        }

        //Change Status
        public List<CoreOrganizationModel.CorePersonDoc> GetDirectorChangeStatusAttachment(string AppID)
        {

            //List<TourlistDataLayer.DataModel.core_persons_updated> persons = new List<TourlistDataLayer.DataModel.core_persons_updated>();
            //persons = unitOfWork.CorePersonsUpdatedRepository.GetAll().ToList();

            List<TourlistDataLayer.DataModel.vw_change_status_directors> persons = new List<TourlistDataLayer.DataModel.vw_change_status_directors>();
            persons = unitOfWork.VwChangeStatusDirectorsRepository.Find(i => (i.apply_idx.ToString() == AppID)).ToList();


            List<TourlistDataLayer.DataModel.core_org_directors_updated> director = new List<TourlistDataLayer.DataModel.core_org_directors_updated>();
            director = unitOfWork.CoreOrgDirectorsUpdatedRepository.Find(i => (i.stub_ref.ToString() == AppID)).ToList();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD").FirstOrDefault();
            var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT").FirstOrDefault();
            var pasPengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN").FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.upload_type_ref == mykad.ref_idx || c.upload_type_ref == passport.ref_idx)).ToList();


            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.upload_type_ref == pasPengajian.ref_idx)).ToList();


            if (director.Count > 0)
            {
                var clsDirector = (from dir in director
                                   
                                   from person in persons
                                   .Where(p => p.person_upd_idx == dir.person_ref)
                                   .DefaultIfEmpty()

                                   from upload in clsUploadPerson
                                   .Where(d => d.person_ref == person.person_upd_idx)
                                   .DefaultIfEmpty() // <== makes join left join
                                                     // 
                                   from paspengajian in clsUploadPasPengajian
                                    .Where(p => p.person_ref == person.person_upd_idx)
                                    .DefaultIfEmpty() // <== makes join left join  

                                   select new
                                   {
                                       name = person.old_person_name,
                                       nameNew = person.new_person_name,
                                       indentifier = person.old_person_identifier,
                                       indentifierNew = person.new_person_identifier,
                                       fileName = upload == null ? "" : upload.upload_name,
                                       locationFile = upload == null ? "" : upload.upload_path,
                                       fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                                       locationFilePas = paspengajian == null ? "" : paspengajian.upload_path

                                   }).ToList();



                List<CoreOrganizationModel.CorePersonDoc> modelList = new List<CoreOrganizationModel.CorePersonDoc>();

                foreach (var dashboard in clsDirector)
                {
                    CoreOrganizationModel.CorePersonDoc model = new CoreOrganizationModel.CorePersonDoc();
                    {
                        if (dashboard.name == null)
                        {                           
                            model.name = dashboard.nameNew;
                        }
                        else
                        {
                            model.name = dashboard.name;
                        }
                        if (dashboard.indentifier == null)
                        {
                            model.indentifier = dashboard.indentifierNew;
                        }
                        else
                        {
                            model.indentifier = dashboard.indentifier;
                        }

                        model.fileName = dashboard.fileName;
                       // model.indentifier = dashboard.indentifier;
                        model.locationFile = dashboard.locationFile;
                        model.locationFilePas = dashboard.locationFilePas;
                        model.fileNamePas = dashboard.fileNamePas;
                        if (model.fileName == "")
                        {
                            var coreP = unitOfWork.VwChangeStatusDirectorsRepository.Find(i => i.new_person_identifier == model.indentifier).FirstOrDefault();
                            if(coreP != null)
                            {
                                core_uploads_freeform_by_persons upload = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(i => i.person_ref == coreP.person_upd_idx).FirstOrDefault();
                                if (upload != null)
                                {
                                    model.fileName = upload == null ? "" : upload.upload_name;
                                    model.locationFile = upload == null ? "" : upload.upload_path;
                                }
                                core_uploads_freeform_by_persons uploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => c.person_ref == coreP.person_upd_idx
                                && (c.upload_type_ref == pasPengajian.ref_idx)).FirstOrDefault();
                                if (uploadPasPengajian != null)
                                {
                                    model.fileName = uploadPasPengajian == null ? "" : uploadPasPengajian.upload_name;
                                    model.locationFile = uploadPasPengajian == null ? "" : uploadPasPengajian.upload_path;
                                }
                            }
                        }
                    }
                    modelList.Add(model);

                }
                if (modelList.Count > 0)
                {
                    return modelList;
                }
                else
                {
                    List<CoreOrganizationModel.CorePersonDoc> modelList1 = new List<CoreOrganizationModel.CorePersonDoc>();

                    return modelList1;
                }

            }

            return null;


        }

        public List<CoreOrganizationModel.CorePersonDoc> GetShareHolderChangeStatusAttachment(string AppID)
        {

            List<CoreOrganizationModel.CorePersonDoc> listShareHolder = new List<CoreOrganizationModel.CorePersonDoc>();

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var DocType = unitOfWork.RefReferencesRepository
               .Find(x => x.ref_code.ToString() == "MYKAD" && x.ref_type == refType.ref_idx)
               .FirstOrDefault();
            Guid gAppid = Guid.Parse(AppID);

            listShareHolder = unitOfWork.vwChangeStatusShareholdersRepository.Table.DefaultIfEmpty()
                        // .Join(unitOfWork.CorePersonsUpdatedRepository.Table, p => p.person_upd_idx, vw => vw.person_ref, (vw, p) => new { vw, p })
                        .Join(unitOfWork.CoreUploadsFreeFormPersonRepository.Table.DefaultIfEmpty(), vw => vw.person_upd_idx, c => c.person_ref, (vw, c) => new
                        {
                            name = vw.old_person_name,
                            nameNew=vw.new_person_name,
                            identifier = vw.old_person_identifier,
                            identifierNew=vw.new_person_identifier,
                            upload_name =c.upload_name,
                            upload_path=c.upload_path,
                            apply_idx = vw.apply_idx,
                            fileType=c.upload_type_ref
                        })                        
                        .Where(x => x.apply_idx== gAppid && x.fileType == DocType.ref_idx)
                        .Select(x => new CoreOrganizationModel.CorePersonDoc
                        {
                            name = x.name,
                            nameNew=x.nameNew,
                            indentifier = x.identifier,
                            indentifierNew=x.identifierNew,
                            fileName=x.upload_name,
                            locationFile = x.upload_path

                        }).ToList();
            List<CoreOrganizationModel.CorePersonDoc> modelList = new List<CoreOrganizationModel.CorePersonDoc>();

            foreach (var dashboard in listShareHolder)
            {
                CoreOrganizationModel.CorePersonDoc model = new CoreOrganizationModel.CorePersonDoc();
                {
                    if (dashboard.name == null)
                    {
                        model.name = dashboard.nameNew;
                    }
                    else
                    {
                        model.name = dashboard.name;
                    }
                    if (dashboard.indentifier == null)
                    {
                        model.indentifier = dashboard.indentifierNew;
                    }
                    else
                    {
                        model.indentifier = dashboard.indentifier;
                    }

                    model.fileName = dashboard.fileName;
                    //model.indentifier = dashboard.indentifier;
                    model.locationFile = dashboard.locationFile;
                    //model.name = dashboard.name;
                    //model.nameNew = dashboard.nameNew;
                    model.locationFilePas = dashboard.locationFilePas;
                    model.fileNamePas = dashboard.fileNamePas;

                }
                modelList.Add(model);

            }


            return modelList;


        }
        //Change Status

        public List<CoreOrganizationModel.CoreAcknowledgement> getAcknowledge(string appID)
        {
            
            List<core_acknowledgements> core_acknowledgement = new List<core_acknowledgements>();
            var gAppID= Guid.Parse(appID);
            core_acknowledgement = unitOfWork.CoreAcknowledgementsRepository.Find(c => c.stub_ref == gAppID).ToList();

            List<CoreOrganizationModel.CoreAcknowledgement> modelList = new List<CoreOrganizationModel.CoreAcknowledgement>();

            foreach(var item in core_acknowledgement)
            {
                CoreOrganizationModel.CoreAcknowledgement model = new CoreOrganizationModel.CoreAcknowledgement();
                {
                    model.acknowledge_person_ref = (Guid)item.acknowledge_person_ref;
                    model.acknowledgement_idx = (Guid)item.acknowledgement_idx;
                    //model.locationFile = dashboard.locationFile;
                    //model.name = dashboard.name;

                }
                modelList.Add(model);
            }

            return modelList;

            //if (acknowledge != null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

        }

        public List<CoreOrganizationModel.CoreAcknowledgement> getAcknowledgePPP(string appID)
        {

            List<core_acknowledgements> core_acknowledgement = new List<core_acknowledgements>();
            var gAppID = Guid.Parse(appID);
            core_acknowledgement = unitOfWork.CoreAcknowledgementsRepository.Find(c => c.stub_ref == gAppID).ToList();

            List<CoreOrganizationModel.CoreAcknowledgement> modelList = new List<CoreOrganizationModel.CoreAcknowledgement>();

            foreach (var item in core_acknowledgement)
            {
                CoreOrganizationModel.CoreAcknowledgement model = new CoreOrganizationModel.CoreAcknowledgement();
                {
                    model.name = item.acknowledge_person_name;
                    model.company = item.acknowledge_organization_name;
                    model.position = item.acknowledge_position;
                    model.icno = item.acknowledge_person_icno;
                    model.acknowledgement_idx = (Guid)item.acknowledgement_idx;
    

                }
                modelList.Add(model);
            }

            return modelList;


        }


        public object GetDirectorDetail(string Identifier)
        {
            List<TourlistDataLayer.DataModel.core_persons> clsPerson = new List<TourlistDataLayer.DataModel.core_persons>();
            clsPerson = unitOfWork.CorePersonsRepository.Find(c => (c.person_identifier == Identifier)).ToList();

            Guid gPersonIDX = Guid.Empty;
            foreach (var item in clsPerson)
            {
                gPersonIDX = item.person_idx;
            }
            var postcode = "";
            foreach (var item in clsPerson)
            {
                postcode = item.contact_postcode;
            }



            //List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            //clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gPersonIDX)).ToList();

            //List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            //clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gPersonIDX)).ToList();

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var pasPengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN" && c.ref_type == refType.ref_idx).FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons > clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gPersonIDX && c.upload_type_ref== mykad.ref_idx || c.upload_type_ref==passport.ref_idx)).ToList();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gPersonIDX && c.upload_type_ref == pasPengajian.ref_idx)).ToList();


            List<vw_geo_list> geo_List = new List<vw_geo_list>();
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.postcode_code.ToString() == postcode))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            var CityReg = "";
            var sCityReg = "";
            var sStateReg = "";
            foreach (var gCityReg in geo_List)
            {
                sCityReg = gCityReg.town_name;
                sStateReg = gCityReg.state_name;
            }


            var clsApp = (from person in clsPerson
                          from myKad in clsUploadPerson

                        .Where(d => d.person_ref == person.person_idx)
                        .DefaultIfEmpty() // <== makes join left join  

                        from paspengajian in clsUploadPasPengajian
                            .Where(p => p.person_ref == person.person_idx)
                            .DefaultIfEmpty() // <== makes join left join  


                          select new
                          {
                              person_name = person.person_name,
                              person_identifier = person.person_identifier,
                              contact_addr_1 = person.contact_addr_1,
                              contact_addr_2 = person.contact_addr_2,
                              contact_addr_3 = person.contact_addr_3,
                              contact_postcode = person.contact_postcode,
                              contact_state=person.contact_state,
                              contact_city=person.contact_city,
                              contact_city_name= sCityReg,
                              contact_state_name= sStateReg,
                              person_gender = person.person_gender,
                              sperson_birthday = person.sperson_birthday,
                              sperson_age = person.sperson_age,
                              contact_mobile_no = person.contact_mobile_no,
                              contact_phone_no = person.contact_phone_no,
                              mobile_no = person.personal_mobile_no,
                              phone_no = person.residential_phone_no,
                              person_nationality = person.person_nationality,
                              person_idx = person.person_idx,

                              fileName = myKad == null ? "" : myKad.upload_name,
                              locationFile = myKad == null ? "" : myKad.upload_path,

                              fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                              locationFilePas = paspengajian == null ? "" : paspengajian.upload_path
                             

                          }).ToList();




            return clsApp;
        }

        public object GetDirectorDetailByDirectorID(string organization_director_idx)
        {
            var clsDirector = unitOfWork.vwCoreOrgDirectorRepository.Find(c => (c.organization_director_idx.ToString() == organization_director_idx)).FirstOrDefault();
            List<TourlistDataLayer.DataModel.core_persons> clsPerson = new List<TourlistDataLayer.DataModel.core_persons>();
            clsPerson = unitOfWork.CorePersonsRepository.Find(c => (c.person_idx == clsDirector.person_ref)).ToList();
            Guid gPersonIDX = Guid.Empty;
            foreach (var item in clsPerson)
            {
                gPersonIDX = item.person_idx;
            }
            var postcode = "";
            foreach (var item in clsPerson)
            {
                postcode = item.contact_postcode;
            }

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var pasPengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN" && c.ref_type == refType.ref_idx).FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gPersonIDX && c.upload_type_ref == mykad.ref_idx || c.upload_type_ref == passport.ref_idx)).ToList();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gPersonIDX && c.upload_type_ref == pasPengajian.ref_idx)).ToList();


            List<vw_geo_list> geo_List = new List<vw_geo_list>();
            geo_List = unitOfWork.VwGeoListRepository
                .Find(c => (c.postcode_code.ToString() == postcode))
                .GroupBy(d => d.town_idx)
                .Select(g => g.FirstOrDefault())
                .ToList();

            var CityReg = "";
            var sCityReg = "";
            var sStateReg = "";
            foreach (var gCityReg in geo_List)
            {
                sCityReg = gCityReg.town_name;
                sStateReg = gCityReg.state_name;
            }


            var clsApp = (from person in clsPerson
                          from myKad in clsUploadPerson

                        .Where(d => d.person_ref == person.person_idx)
                        .DefaultIfEmpty() // <== makes join left join  

                          from paspengajian in clsUploadPasPengajian
                              .Where(p => p.person_ref == person.person_idx)
                              .DefaultIfEmpty() // <== makes join left join  


                          select new
                          {
                              person_name = person.person_name,
                              person_identifier = person.person_identifier,
                              contact_addr_1 = person.contact_addr_1,
                              contact_addr_2 = person.contact_addr_2,
                              contact_addr_3 = person.contact_addr_3,
                              contact_postcode = person.contact_postcode,
                              contact_state = person.contact_state,
                              contact_city = person.contact_city,
                              contact_city_name = sCityReg,
                              contact_state_name = sStateReg,
                              person_gender = person.person_gender,
                              sperson_birthday = person.sperson_birthday,
                              sperson_age = person.sperson_age,
                              contact_mobile_no = person.contact_mobile_no,
                              contact_phone_no = person.contact_phone_no,
                              mobile_no = person.personal_mobile_no,
                              phone_no = person.residential_phone_no,
                              person_nationality = person.person_nationality,
                              person_idx = person.person_idx,

                              fileName = myKad == null ? "" : myKad.upload_name,
                              locationFile = myKad == null ? "" : myKad.upload_path,

                              fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                              locationFilePas = paspengajian == null ? "" : paspengajian.upload_path


                          }).ToList();




            return clsApp;
        }


        public object GetChangeStatusDirectorDetail(string DirectorIdx)
        {
            Guid gDirectorIdx = Guid.Parse(DirectorIdx);
            List<TourlistDataLayer.DataModel.core_persons_updated> clsPerson = new List<TourlistDataLayer.DataModel.core_persons_updated>();
            clsPerson = unitOfWork.CorePersonsUpdatedRepository.Find(c => (c.person_upd_idx == gDirectorIdx)).ToList();

            //Guid gPersonIDX = Guid.Empty;
            //foreach (var item in clsPerson)
            //{
            //    gPersonIDX = item.person_idx;
            //}
            //var postcode = "";
            //foreach (var item in clsPerson)
            //{
            //    postcode = item.contact_postcode;
            //}

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var pasPengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var justfikasi = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "JUSTIFICATION" && c.ref_type == refType.ref_idx).FirstOrDefault();


            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gDirectorIdx && (c.upload_type_ref == mykad.ref_idx || c.upload_type_ref == passport.ref_idx))).ToList();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPasPengajian = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gDirectorIdx && c.upload_type_ref == pasPengajian.ref_idx)).ToList();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadJustification = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadJustification = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gDirectorIdx && c.upload_type_ref == justfikasi.ref_idx)).ToList();

            //List<vw_geo_list> geo_List = new List<vw_geo_list>();
            //geo_List = unitOfWork.VwGeoListRepository
            //    .Find(c => (c.postcode_code.ToString() == postcode))
            //    .GroupBy(d => d.town_idx)
            //    .Select(g => g.FirstOrDefault())
            //    .ToList();


            RefGeoHelper refGeo = new RefGeoHelper();
            string New_person_name = "";
            string Old_person_name = "";
            string New_person_identifier = "";
            string Old_person_identifier = "";
            string New_contact_addr_1 = "";
            string Old_contact_addr_1 = "";
            string New_contact_addr_2 = "";
            string Old_contact_addr_2 = "";
            string New_contact_addr_3 = "";
            string Old_contact_addr_3 = "";
            string New_contact_postcode = "";
            string Old_contact_postcode = "";
            string New_contact_mobile_no = "";
            string Old_contact_mobile_no = "";
            string New_contact_phone_no = "";
            string Old_contact_phone_no = "";

            Guid New_contact_state = Guid.Empty;
            Guid Old_contact_state = Guid.Empty;
            Guid New_contact_city = Guid.Empty;
            Guid Old_contact_city = Guid.Empty;
            Guid New_person_gender = Guid.Empty;
            Guid Old_person_gender = Guid.Empty;
            Guid New_person_nationality = Guid.Empty;
            Guid Old_person_nationality = Guid.Empty;
            Guid New_justification_id = Guid.Empty;
            string New_justification_name="";
            string New_person_birthday = "";
            string Old_person_birthday = "";
            string Old_person_age = "";
            string New_person_age = "";
            foreach (var item in clsPerson)
            {

                New_person_name = item.new_person_name;
                Old_person_name = item.old_person_name;
                New_person_identifier = item.new_person_identifier;
                Old_person_identifier = item.old_person_identifier;
                New_contact_addr_1 = item.new_contact_addr_1;
                Old_contact_addr_1 = item.old_contact_addr_1;
                New_contact_addr_2 = item.new_contact_addr_2;
                Old_contact_addr_2 = item.old_contact_addr_2;
                New_contact_addr_3 = item.new_contact_addr_3;
                Old_contact_addr_3 = item.old_contact_addr_3;
                New_contact_postcode = item.new_contact_postcode;
                Old_contact_postcode = item.old_contact_postcode;
                New_contact_state = (item.new_contact_state == null) ? Guid.Empty : (Guid)item.new_contact_state;
                Old_contact_state = (item.old_contact_state == null) ? Guid.Empty : (Guid)item.old_contact_state;
                New_contact_city = (item.new_contact_city == null) ? Guid.Empty : (Guid)item.new_contact_city;
                Old_contact_city = (item.old_contact_city == null) ? Guid.Empty : (Guid)item.old_contact_city;
                New_person_gender = (item.new_person_gender == null) ? Guid.Empty : (Guid)item.new_person_gender;
                Old_person_gender = (item.old_person_gender == null) ? Guid.Empty : (Guid)item.old_person_gender;
                New_person_nationality = (item.new_person_nationality == null) ? Guid.Empty : (Guid)item.new_person_nationality;
                Old_person_nationality = (item.old_person_nationality == null) ? Guid.Empty : (Guid)item.old_person_nationality;
                New_contact_mobile_no = item.new_contact_mobile_no;              
                Old_contact_mobile_no = item.old_contact_mobile_no;
                New_contact_phone_no = item.new_contact_phone_no;
                Old_contact_phone_no = item.old_contact_phone_no;
                New_person_birthday = String.Format("{0:yyyy-MM-dd}", item.new_person_birthday);
                Old_person_birthday = String.Format("{0:yyyy-MM-dd}", item.old_person_birthday);
                if (New_person_birthday != "")
                {
                    New_person_age = (DateTime.Now.Year - int.Parse(New_person_birthday.Substring(0, 4))).ToString();
                }
                if (Old_person_birthday != "")
                {
                    Old_person_age = (DateTime.Now.Year - int.Parse(Old_person_birthday.Substring(0, 4))).ToString();
                }

                New_justification_id = (item.new_justification_id == null) ? Guid.Empty : (Guid)item.new_justification_id;
                New_justification_name = item.new_justification_name;

            }

            var clsApp = (from person in clsPerson
                          from myKad in clsUploadPerson

                        .Where(d => d.person_ref == gDirectorIdx)
                        .DefaultIfEmpty() // <== makes join left join  

                          from paspengajian in clsUploadPasPengajian
                              .Where(p => p.person_ref == gDirectorIdx)
                              .DefaultIfEmpty() // <== makes join left join  

                          from justifikasi in clsUploadJustification
                          .Where(p => p.person_ref == gDirectorIdx)
                          .DefaultIfEmpty() // <== makes join left join  
                          select new
                          {
                              new_person_name = New_person_name,
                              old_person_name = Old_person_name,
                              new_person_identifier = New_person_identifier,
                              old_person_identifier = Old_person_identifier,
                              new_contact_addr_1 = New_contact_addr_1,
                              old_contact_addr_1 = Old_contact_addr_1,
                              new_contact_addr_2 = New_contact_addr_2,
                              old_contact_addr_2 = Old_contact_addr_2,
                              new_contact_addr_3 = New_contact_addr_3,
                              old_contact_addr_3 = Old_contact_addr_3,
                              new_contact_postcode = New_contact_postcode,
                              old_contact_postcode = Old_contact_postcode,
                              new_contact_state = (Guid)New_contact_state,
                              old_contact_state = (Guid)Old_contact_state,
                              new_contact_city = (Guid)New_contact_city,
                              old_contact_city = (Guid)Old_contact_city,
                              new_person_gender = (Guid)New_person_gender,
                              old_person_gender = (Guid)Old_person_gender,
                              new_contact_mobile_no = New_contact_mobile_no,
                              old_contact_mobile_no = Old_contact_mobile_no,
                              new_contact_phone_no = New_contact_phone_no,
                              old_contact_phone_no = Old_contact_phone_no,
                              new_person_nationality = (Guid)New_person_nationality,
                              old_person_nationality = (Guid)Old_person_nationality,
                              new_justification_id= New_justification_id,
                              new_justification_name=New_justification_name,

                              sNew_person_birthday = New_person_birthday,
                              sOld_person_birthday = Old_person_birthday,
                              sOld_person_age= Old_person_age,
                              sNew_person_age= New_person_age,


                              fileName = myKad == null ? "" : myKad.upload_name,
                              locationFile = myKad == null ? "" : myKad.upload_path,

                              fileNamePas = paspengajian == null ? "" : paspengajian.upload_name,
                              locationFilePas = paspengajian == null ? "" : paspengajian.upload_path,

                              fileNameDirector = justifikasi == null ? "" : justifikasi.upload_name,
                              locationFileDirector = justifikasi == null ? "" : justifikasi.upload_path

                          });

            


            return clsApp;
        }

        public object GetChangeStatusShareholderDetail(string ShareholderIdx)
        {
            Guid gShareholderIdx = Guid.Parse(ShareholderIdx);
            List<TourlistDataLayer.DataModel.core_persons_updated> clsPerson = new List<TourlistDataLayer.DataModel.core_persons_updated>();
            clsPerson = unitOfWork.CorePersonsUpdatedRepository.Find(c => (c.person_upd_idx == gShareholderIdx)).ToList();

           
        

            RefGeoHelper refGeo = new RefGeoHelper();
            string New_person_name = "";
            string Old_person_name = "";
            string New_person_identifier = "";
            string Old_person_identifier = "";
            string New_contact_addr_1 = "";
            string Old_contact_addr_1 = "";
            string New_contact_addr_2 = "";
            string Old_contact_addr_2 = "";
            string New_contact_addr_3 = "";
            string Old_contact_addr_3 = "";
            string New_contact_postcode = "";
            string Old_contact_postcode = "";
            string New_contact_mobile_no = "";
            string Old_contact_mobile_no = "";
            string New_contact_phone_no = "";
            string Old_contact_phone_no = "";

            Guid New_contact_state = Guid.Empty;
            Guid Old_contact_state = Guid.Empty;
            Guid New_contact_city = Guid.Empty;
            Guid Old_contact_city = Guid.Empty;
            Guid New_person_gender = Guid.Empty;
            Guid Old_person_gender = Guid.Empty;
            Guid New_person_nationality = Guid.Empty;
            Guid Old_person_nationality = Guid.Empty;
            Guid New_justification_id = Guid.Empty;
            string New_justification_name = "";
            string New_person_birthday = "";
            string Old_person_birthday = "";
            string Old_person_age = "";
            string New_person_age = "";
            string New_number_of_shares = "";
            string Old_number_of_shares = "";
            Guid New_person_religion = Guid.Empty;
            Guid Old_person_religion = Guid.Empty;

            Guid New_status_shareholder = Guid.Empty;
            Guid Old_status_shareholder = Guid.Empty;

            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var mykad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();

            var refTypeJust = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();
            var myfileJust = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "JUSTIFICATION" && c.ref_type == refType.ref_idx).FirstOrDefault();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPerson = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPerson = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gShareholderIdx && (c.upload_type_ref == mykad.ref_idx))).ToList();

            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons> clsUploadPersonJust = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_persons>();
            clsUploadPersonJust = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == gShareholderIdx && (c.upload_type_ref == myfileJust.ref_idx))).ToList();

            foreach (var item in clsPerson)
            {

                New_person_name = item.new_person_name;
                Old_person_name = item.old_person_name;
                New_person_identifier = item.new_person_identifier;
                Old_person_identifier = item.old_person_identifier;
                New_contact_addr_1 = item.new_contact_addr_1;
                Old_contact_addr_1 = item.old_contact_addr_1;
                New_contact_addr_2 = item.new_contact_addr_2;
                Old_contact_addr_2 = item.old_contact_addr_2;
                New_contact_addr_3 = item.new_contact_addr_3;
                Old_contact_addr_3 = item.old_contact_addr_3;
                New_contact_postcode = item.new_contact_postcode;
                Old_contact_postcode = item.old_contact_postcode;
                New_contact_state = (item.new_contact_state == null) ? Guid.Empty : (Guid)item.new_contact_state;
                Old_contact_state = (item.old_contact_state == null) ? Guid.Empty : (Guid)item.old_contact_state;
                New_contact_city = (item.new_contact_city == null) ? Guid.Empty : (Guid)item.new_contact_city;
                Old_contact_city = (item.old_contact_city == null) ? Guid.Empty : (Guid)item.old_contact_city;
                New_person_gender = (item.new_person_gender == null) ? Guid.Empty : (Guid)item.new_person_gender;
                Old_person_gender = (item.old_person_gender == null) ? Guid.Empty : (Guid)item.old_person_gender;
                New_person_nationality = (item.new_person_nationality == null) ? Guid.Empty : (Guid)item.new_person_nationality;
                Old_person_nationality = (item.old_person_nationality == null) ? Guid.Empty : (Guid)item.old_person_nationality;
                New_contact_mobile_no = item.new_contact_mobile_no;
                Old_contact_mobile_no = item.old_contact_mobile_no;
                New_contact_phone_no = item.new_contact_phone_no;
                Old_contact_phone_no = item.old_contact_phone_no;
                New_number_of_shares = item.new_number_of_shares.ToString();
                Old_number_of_shares = item.old_number_of_shares.ToString();
                New_person_religion = (item.new_person_religion == null) ? Guid.Empty : (Guid)item.new_person_religion;
                Old_person_religion = (item.old_person_religion == null) ? Guid.Empty : (Guid)item.old_person_religion;
                New_status_shareholder = (item.new_status_shareholder == null) ? Guid.Empty : (Guid)item.new_status_shareholder;
                Old_status_shareholder = (item.old_status_shareholder == null) ? Guid.Empty : (Guid)item.old_status_shareholder;

                New_person_birthday = String.Format("{0:yyyy-MM-dd}", item.new_person_birthday);
                Old_person_birthday = String.Format("{0:yyyy-MM-dd}", item.old_person_birthday);
                if (New_person_birthday != "")
                {
                    New_person_age = (DateTime.Now.Year - int.Parse(New_person_birthday.Substring(0, 4))).ToString();
                }
                if (Old_person_birthday != "")
                {
                    Old_person_age = (DateTime.Now.Year - int.Parse(Old_person_birthday.Substring(0, 4))).ToString();
                }

                New_justification_id = (item.new_justification_id == null) ? Guid.Empty : (Guid)item.new_justification_id;
                New_justification_name = item.new_justification_name;


                
            }

            var clsApp = (from person in clsPerson
                          from myKad in clsUploadPerson                         

                      .Where(d => d.person_ref == gShareholderIdx)
                        .DefaultIfEmpty() // <== makes join left join  
                          from myFileJust in clsUploadPersonJust
                           .Where(e => e.person_ref == gShareholderIdx)
                        .DefaultIfEmpty() // <== makes join left join  
                          select new
                          {
                              new_person_name = New_person_name,
                              old_person_name = Old_person_name,
                              new_person_identifier = New_person_identifier,
                              old_person_identifier = Old_person_identifier,
                              new_contact_addr_1 = New_contact_addr_1,
                              old_contact_addr_1 = Old_contact_addr_1,
                              new_contact_addr_2 = New_contact_addr_2,
                              old_contact_addr_2 = Old_contact_addr_2,
                              new_contact_addr_3 = New_contact_addr_3,
                              old_contact_addr_3 = Old_contact_addr_3,
                              new_contact_postcode = New_contact_postcode,
                              old_contact_postcode = Old_contact_postcode,
                              new_contact_state = (Guid)New_contact_state,
                              old_contact_state = (Guid)Old_contact_state,
                              new_contact_city = (Guid)New_contact_city,
                              old_contact_city = (Guid)Old_contact_city,
                              new_person_gender = (Guid)New_person_gender,
                              old_person_gender = (Guid)Old_person_gender,
                              new_contact_mobile_no = New_contact_mobile_no,
                              old_contact_mobile_no = Old_contact_mobile_no,
                              new_contact_phone_no = New_contact_phone_no,
                              old_contact_phone_no = Old_contact_phone_no,
                              new_person_nationality = (Guid)New_person_nationality,
                              old_person_nationality = (Guid)Old_person_nationality,
                              new_justification_id = New_justification_id,
                              new_justification_name = New_justification_name,
                              new_person_religion = New_person_religion,
                              old_person_religion = Old_person_religion,
                              new_number_of_shares = New_number_of_shares,
                              old_number_of_shares = Old_number_of_shares,
                              new_status_shareholder = New_status_shareholder,
                              old_status_shareholder = Old_status_shareholder,


                              sNew_person_birthday = New_person_birthday,
                              sOld_person_birthday = Old_person_birthday,
                              sOld_person_age = Old_person_age,
                              sNew_person_age = New_person_age,


                              fileName = myKad == null ? "" : myKad.upload_name,
                              locationFile = myKad == null ? "" : myKad.upload_path,

                              fileNameJust = myFileJust == null ? "" : myFileJust.upload_name,
                              locationFileJust = myFileJust == null ? "" : myFileJust.upload_path,


                          });




            return clsApp;
        }

        public object GetChangeStatusShareholderOrgDetail(string ShareholderIdx)
        {
            Guid gShareholderIdx = Guid.Parse(ShareholderIdx);
             
            List<TourlistDataLayer.DataModel.core_organizations_updated> clsOrg = new List<TourlistDataLayer.DataModel.core_organizations_updated>();

            clsOrg = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => (c.organization_upd_idx == gShareholderIdx)).ToList();

         
            RefGeoHelper refGeo = new RefGeoHelper();
            string New_organization_name = "";
            string Old_organization_name = "";
            string New_organization_identifier = "";
            string Old_organization_identifier = "";
            string New_addr_1 = "";
            string Old_addr_1 = "";
            string New_addr_2 = "";
            string Old_addr_2 = "";
            string New_addr_3 = "";
            string Old_addr_3 = "";
            string New_postcode = "";
            string Old_postcode = "";
            string New_number_of_shares = "";
            string Old_number_of_shares = "";
            string New_registered_year = "";
            string Old_registered_year = "";
            string New_mobile_no = "";
            string Old_mobile_no = "";
            string New_justification_name = "";

            Guid New_city = Guid.Empty;
            Guid Old_city = Guid.Empty;
            Guid Newstate = Guid.Empty;
            Guid Old_state = Guid.Empty;
            Guid New_status_shareholder = Guid.Empty;
            Guid Old_status_shareholder = Guid.Empty;            
           
            Guid Org_shareholder_upd_ref = Guid.Empty;

            Guid New_justification_id = Guid.Empty;


            var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();
           
            var myfileJust = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "JUSTIFICATION" && c.ref_type == refType.ref_idx).FirstOrDefault();
                       
            List<TourlistDataLayer.DataModel.core_uploads_freeform_by_organizations> clsUploadOrgJust = new List<TourlistDataLayer.DataModel.core_uploads_freeform_by_organizations>();
            clsUploadOrgJust = unitOfWork.CoreUploadsFreeformOrganizationsRepository.Find(c => (c.uploads_freeform_by_organizations_idx == gShareholderIdx && (c.upload_type_ref == myfileJust.ref_idx))).ToList();

            foreach (var item in clsOrg)
            {

                New_organization_name = item.new_organization_name;
                Old_organization_name = item.old_organization_name;
                New_organization_identifier = item.new_organization_identifier;
                Old_organization_identifier = item.old_organization_identifier;
                New_addr_1 = item.new_addr_1;
                Old_addr_1 = item.old_addr_1;
                New_addr_2 = item.new_addr_2;
                Old_addr_2 = item.old_addr_2;
                New_addr_3 = item.new_addr_3;
                Old_addr_3 = item.old_addr_3;
                New_postcode = item.new_postcode;
                Old_postcode = item.old_postcode;
                Org_shareholder_upd_ref = item.org_shareholder_upd_ref;
                New_city = (item.new_city == null) ? Guid.Empty : (Guid)item.new_city;
                Old_city = (item.old_city == null) ? Guid.Empty : (Guid)item.old_city;
             
                New_mobile_no = item.new_mobile_no;
                Old_mobile_no = item.old_mobile_no;
                New_registered_year = item.new_registered_year;
                Old_registered_year = item.old_registered_year;                
                New_number_of_shares = item.new_number_of_shares.ToString();
                Old_number_of_shares = item.old_number_of_shares.ToString();
                New_status_shareholder = (item.new_status_shareholder == null) ? Guid.Empty : (Guid)item.new_status_shareholder;
                Old_status_shareholder = (item.old_status_shareholder == null) ? Guid.Empty : (Guid)item.old_status_shareholder;

                New_justification_id = (item.new_justification_id == null) ? Guid.Empty : (Guid)item.new_justification_id;
                New_justification_name = item.new_justification_name;

            }

            var clsApp = (from org in clsOrg
                          from myFileJust in clsUploadOrgJust

                      .Where(d => d.organization_ref == gShareholderIdx)
                        .DefaultIfEmpty() // <== makes join left join  
                       
                          select new
                          {
                              new_organization_name = New_organization_name,
                              old_organization_name = Old_organization_name,
                              new_organization_identifier = New_organization_identifier,
                              old_organization_identifier = Old_organization_identifier,
                              new_addr_1 = New_addr_1,
                              old_addr_1 = Old_addr_1,
                              new_addr_2 = New_addr_2,
                              old_addr_2 = Old_addr_2,
                              new_addr_3 = New_addr_3,
                              old_addr_3 = Old_addr_3,
                              new_postcode = New_postcode,
                              old_postcode = Old_postcode,
                              org_shareholder_upd_ref = Org_shareholder_upd_ref,
                              new_city = (Guid)New_city,
                              old_city = (Guid)Old_city,
                              new_status_shareholder = (Guid)New_status_shareholder,
                              old_status_shareholder = (Guid)Old_status_shareholder,
                              new_mobile_no = New_mobile_no,
                              old_mobile_no = Old_mobile_no,
                              new_registered_year = New_registered_year,
                              old_registered_year = Old_registered_year,
                              new_number_of_shares = New_number_of_shares,
                              old_number_of_shares = Old_number_of_shares,
                              
                              new_justification_id = New_justification_id,
                              new_justification_name = New_justification_name,
                             
                              fileNameJust = myFileJust == null ? "" : myFileJust.upload_name,
                              locationFileJust = myFileJust == null ? "" : myFileJust.upload_path,


                          });




            return clsApp;
        }


        public List<core_uploads_freeform_by_modules> GetOtherDocument(string appID, Guid? user_idx = null)
        {
            var clsDocument = user_idx == null ? unitOfWork.CoreUploadsFreeformModulesRepository
               .Find(x => x.transaction_ref.ToString() == appID)
               .ToList() 
               : 
               unitOfWork.CoreUploadsFreeformModulesRepository
               .Find(x => x.transaction_ref.ToString() == appID && x.created_by == user_idx)
               .ToList();

            return clsDocument;
        }

        public List<vw_ref_references_by_ref_types> GetRefListByType(string refType)
        {
            var datalist = unitOfWork.VwRefReferencesRepository
                .Find(x => x.ref_type_name == refType).OrderBy(i=>i.numeric1_field)
                .ToList();

            return datalist;
        }

        public TourlistDataLayer.DataModel.core_license GetCoreLicens(string organizationIdx)
        {
            var coreLicense = unitOfWork.CoreLicenseRepository.Find(i => (i.core_organization_ref.ToString() == organizationIdx.ToString())).FirstOrDefault();
            return coreLicense;
        }

        public List<TourlistDataLayer.DataModel.core_license> GetCoreLicenseNo(string sOrganizationID, string solSolution)
        {
            if(solSolution == "TOBTAB")
            {
                core_sol_solutions core_Sol_Solutions = unitOfWork.CoreSolSolutionsRepository.Find(i => i.solution_name.ToString() == solSolution).FirstOrDefault();
                solSolution = core_Sol_Solutions.solutions_idx.ToString();
            }else if (solSolution == "ILP")
            {
                core_sol_solutions core_Sol_Solutions = unitOfWork.CoreSolSolutionsRepository.Find(i => i.solution_name.ToString() == solSolution).FirstOrDefault();
                solSolution = core_Sol_Solutions.solutions_idx.ToString();
            }else if (solSolution == "MM2H")
            {
                core_sol_solutions core_Sol_Solutions = unitOfWork.CoreSolSolutionsRepository.Find(i => i.solution_name.ToString() == solSolution).FirstOrDefault();
                solSolution = core_Sol_Solutions.solutions_idx.ToString();
            }
            Guid gOrgID = Guid.Parse(sOrganizationID);
            List<TourlistDataLayer.DataModel.core_license> clsLicense = new List<TourlistDataLayer.DataModel.core_license>();
            clsLicense = unitOfWork.CoreLicenseRepository.Find(c => c.core_organization_ref == gOrgID && c.core_sol_solution_ref.ToString() == solSolution).ToList();

            return clsLicense;

        }

        public List<TourlistDataLayer.DataModel.core_license> GetCoreLicenseNo(string sOrganizationID)
        {
            Guid gOrgID = Guid.Parse(sOrganizationID);
            List<TourlistDataLayer.DataModel.core_license> clsLicense = new List<TourlistDataLayer.DataModel.core_license>();
            clsLicense = unitOfWork.CoreLicenseRepository.Find(c => (c.core_organization_ref == gOrgID)).ToList();

            return clsLicense;

        }
        public List<TourlistDataLayer.DataModel.core_license> GetCoreLicenseNoTG(Guid person_ref)
        {
            List<TourlistDataLayer.DataModel.core_license> clsLicense = new List<TourlistDataLayer.DataModel.core_license>();
            clsLicense = unitOfWork.CoreLicenseRepository.Find(c => (c.core_organization_ref == person_ref || c.core_persons_ref == person_ref)).ToList();

            return clsLicense;

        }
        public bool updateTobtabLicenseRenewal(Guid OrganizationIdx, Guid stubIdx, Guid userID)
        {
            try
            {
                core_sol_solutions core_Sol_Solutions = unitOfWork.CoreSolSolutionsRepository.Find(i => i.solution_name.ToString() == "TOBTAB").FirstOrDefault();
                string solSolution = core_Sol_Solutions.solutions_idx.ToString();

                var coreLicense = unitOfWork.CoreLicenseRepository.Find(i => i.core_organization_ref == OrganizationIdx && i.core_sol_solution_ref.ToString() == solSolution).FirstOrDefault();
                if(coreLicense != null)
                {
                    var tobtabLicense = unitOfWork.TobtabLicenses.Find(i => i.stub_ref == stubIdx).FirstOrDefault();
                    tobtabLicense.inbound = (byte)coreLicense.inbound;
                    tobtabLicense.outbound = (byte)coreLicense.outbound;
                    tobtabLicense.ticketing = (byte)coreLicense.ticketing;
                    tobtabLicense.umrah = (byte)coreLicense.umrah;
                    unitOfWork.TobtabLicenses.Update(tobtabLicense);
                    
                    //update existing record to active_status=0
                    var directorList = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => i.organization_ref == OrganizationIdx).ToList();
                    foreach (var app in directorList)
                    {
                        core_organization_directors director = app;
                        director.active_status = 0;
                        unitOfWork.CoreOrganizationDirectorsRepository.Update(director);
                    }
                    var shareholderList = unitOfWork.CoreOrganizationShareholders.Find(i => i.organization_ref == OrganizationIdx).ToList();
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

        public bool updateMM2HLicenseRenewal(Guid OrganizationIdx, Guid stubIdx, Guid userID)
        {
            try
            {
                //core_sol_solutions core_Sol_Solutions = unitOfWork.CoreSolSolutionsRepository.Find(i => i.solution_name.ToString() == "MM2H").FirstOrDefault();
                //string solSolution = core_Sol_Solutions.solutions_idx.ToString();

                //var coreLicense = unitOfWork.CoreLicenseRepository.Find(i => i.core_organization_ref == OrganizationIdx && i.core_sol_solution_ref.ToString() == solSolution).FirstOrDefault();
                //if (coreLicense != null)
                //{                  
                    //update existing record to active_status=0
                    var directorList = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => i.organization_ref == OrganizationIdx).ToList();
                    foreach (var app in directorList)
                    {
                        core_organization_directors director = app;
                        director.active_status = 0;
                        unitOfWork.CoreOrganizationDirectorsRepository.Update(director);
                    }
                    var shareholderList = unitOfWork.CoreOrganizationShareholders.Find(i => i.organization_ref == OrganizationIdx).ToList();
                    foreach (var app in shareholderList)
                    {
                        core_organization_shareholders shareholder = app;
                        shareholder.active_status = 0;
                        unitOfWork.CoreOrganizationShareholders.Update(shareholder);
                    }
                    unitOfWork.Complete();
                //}

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ChangeStatusOrganization_SaveNew(Guid OrganizationIdx,Guid stubIDX, Guid userID)
        {
            try
            {
                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "ACTIVE").FirstOrDefault();

                
                // List<core_organizations> clsOrganization = new List<core_organizations>();
                var clsOrganization = unitOfWork.CoreOrganizations.Find(c => (c.organization_idx == OrganizationIdx && c.organization_status== upd_active.status_idx)).FirstOrDefault();
                

                core_organizations_updated organizationsUpdated = new core_organizations_updated(); //motacContext.core_persons.Create();

                Guid gUpdate = Guid.NewGuid();
                organizationsUpdated.stub_ref= stubIDX;
                organizationsUpdated.organization_upd_idx = gUpdate;
                organizationsUpdated.organization_current_ref = OrganizationIdx;
                organizationsUpdated.old_organization_name=clsOrganization.organization_name;
                organizationsUpdated.old_addr_1 = clsOrganization.office_addr_1;
                organizationsUpdated.old_addr_2 = clsOrganization.office_addr_2;
                organizationsUpdated.old_addr_3=clsOrganization.office_addr_3;
                organizationsUpdated.old_postcode=clsOrganization.office_postcode;
                organizationsUpdated.old_city=clsOrganization.office_city;
                organizationsUpdated.old_state=clsOrganization.office_state;
                if (clsOrganization.paid_capital == null)
                {
                    var clsOrganizationFinancial = unitOfWork.CoreOrganizationFinancialInfoRepository.Find(x => x.organization_ref == OrganizationIdx).FirstOrDefault();
                    organizationsUpdated.old_paid_capital = clsOrganizationFinancial.share_capital;
                }
                else
                {
                    organizationsUpdated.old_paid_capital = clsOrganization.paid_capital;
                }
                organizationsUpdated.organization_status = upd_active.status_idx;
                organizationsUpdated.created_by = userID;
                organizationsUpdated.modified_by = userID;
                organizationsUpdated.created_dt = DateTime.Now;
                organizationsUpdated.modified_dt = DateTime.Now;

                unitOfWork.CoreOrganizationsUpdatedRepository.Add(organizationsUpdated);
                var result = unitOfWork.Complete(); //motacContext.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ChangeStatusShareHolder_SaveNew(Guid OrganizationIdx, Guid stubIDX, Guid userID)
        {
            try
            {
                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "ACTIVE").FirstOrDefault();

                var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE").FirstOrDefault();

                var my_kad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();



                List<core_organization_shareholders> shareHolders = new List<core_organization_shareholders>();
                shareHolders = unitOfWork.CoreOrganizationShareholders.Find(c => (c.organization_ref == OrganizationIdx && c.shareholder_person_ref != null && c.active_status == 1)).ToList();

                //core_persons_updated PersonUpdated = new core_persons_updated();


                foreach (var shareholder in shareHolders)
                {
                    core_persons_updated PersonUpdated = new core_persons_updated();

                    var person = unitOfWork.CorePersonsRepository.Find(c => (c.person_idx == shareholder.shareholder_person_ref && c.person_status == upd_active.status_idx)).FirstOrDefault();
                    Guid gUpdatePerson = Guid.NewGuid();
                    Guid gshareholderUpd = Guid.NewGuid();
                    PersonUpdated.person_upd_idx = gUpdatePerson;
                    PersonUpdated.org_shareholder_upd_ref = gshareholderUpd;
                    PersonUpdated.person_status_rec = "ASAL";
                    PersonUpdated.person_type_rec = "Shareholder";
                    PersonUpdated.person_ref = person.person_idx;
                    PersonUpdated.old_person_name = person.person_name;
                    PersonUpdated.old_person_identifier = person.person_identifier;
                    PersonUpdated.old_contact_mobile_no = person.contact_mobile_no;
                    PersonUpdated.old_person_birthday = person.person_birthday;
                    PersonUpdated.old_contact_addr_1 = person.contact_addr_1;
                    PersonUpdated.old_contact_addr_2 = person.contact_addr_2;
                    PersonUpdated.old_contact_addr_3 = person.contact_addr_3;
                    PersonUpdated.old_contact_postcode = person.contact_postcode;
                    PersonUpdated.old_contact_city = person.contact_city;
                    PersonUpdated.old_contact_state = person.contact_state;
                    PersonUpdated.old_person_gender = person.person_gender;
                    PersonUpdated.old_person_religion = person.person_religion;
                    PersonUpdated.old_person_nationality = person.person_nationality;
                    PersonUpdated.old_status_shareholder = shareholder.status_shareholder;
                    PersonUpdated.old_number_of_shares = shareholder.number_of_shares;
                    PersonUpdated.person_status = upd_active.status_idx;
                    PersonUpdated.created_by = userID;
                    PersonUpdated.modified_by = userID;
                    PersonUpdated.created_dt = DateTime.Now;
                    PersonUpdated.modified_dt = DateTime.Now;

                    unitOfWork.CorePersonsUpdatedRepository.Add(PersonUpdated);
                    var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                    var attach = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx && c.upload_type_ref== my_kad.ref_idx && c.active_status == upd_active.status_idx)).FirstOrDefault();

                    if (attach != null)
                    {
                        core_uploads_freeform_by_persons uploads = new core_uploads_freeform_by_persons();
                        Guid gUploadID = Guid.NewGuid();
                        uploads.uploads_freeform_by_persons_idx = gUploadID;
                        uploads.person_ref = gUpdatePerson;
                        uploads.upload_type_ref = attach.upload_type_ref;
                        uploads.upload_name = attach.upload_name;
                        uploads.upload_description = attach.upload_description;
                        uploads.upload_path = attach.upload_path;
                        uploads.active_status = upd_active.status_idx;
                        uploads.created_by = userID;
                        uploads.modified_by = userID;
                        uploads.modified_at = DateTime.Now;
                        uploads.created_at = DateTime.Now;

                        unitOfWork.CoreUploadsFreeFormPersonRepository.Add(uploads);
                        var resultattach = unitOfWork.Complete(); //motacContext.SaveChanges();
                    }
                   

                    core_org_shareholders_updated shareholderUpd = new core_org_shareholders_updated();
                    shareholderUpd.org_shareholder_upd_idx = gshareholderUpd;
                    shareholderUpd.stub_ref = stubIDX;
                    shareholderUpd.person_ref = gUpdatePerson;
                    shareholderUpd.active_status = 1;
                    shareholderUpd.created_by = userID;
                    shareholderUpd.modified_by = userID;
                    shareholderUpd.created_at = DateTime.Now;
                    shareholderUpd.modified_at = DateTime.Now;
                    unitOfWork.CoreOrgShareholdersUpdatedRepository.Add(shareholderUpd);
                    var resultSH = unitOfWork.Complete(); //motacContext.SaveChanges();

                }

                List<core_organization_shareholders> shareHolderComs = new List<core_organization_shareholders>();
                shareHolderComs = unitOfWork.CoreOrganizationShareholders.Find(c => (c.organization_ref == OrganizationIdx && c.shareholder_organization_ref != null && c.active_status == 1)).ToList();
                foreach (var shareHolderCom in shareHolderComs)
                {
                    var com = unitOfWork.CoreOrganizations.Find(c => (c.organization_idx == shareHolderCom.shareholder_organization_ref && c.organization_status == upd_active.status_idx)).FirstOrDefault();

                    core_organizations_updated organizationsUpdated = new core_organizations_updated(); //motacContext.core_persons.Create();
                    Guid gshareholderUpd = Guid.NewGuid();
                    Guid gUpdate = Guid.NewGuid();
                    organizationsUpdated.stub_ref = stubIDX;
                    organizationsUpdated.org_shareholder_upd_ref = gshareholderUpd;
                    organizationsUpdated.organization_status_rec = "ASAL";
                    organizationsUpdated.organization_type_rec = "Shareholder";
                    organizationsUpdated.organization_upd_idx = gUpdate;
                    organizationsUpdated.organization_current_ref = OrganizationIdx;
                    organizationsUpdated.old_organization_name = com.organization_name;
                    organizationsUpdated.old_organization_identifier = com.organization_identifier;
                    organizationsUpdated.old_addr_1 = com.office_addr_1;
                    organizationsUpdated.old_addr_2 = com.office_addr_2;
                    organizationsUpdated.old_addr_3 = com.office_addr_3;
                    organizationsUpdated.old_postcode = com.office_postcode;
                    organizationsUpdated.old_city = com.office_city;
                    organizationsUpdated.old_state = com.office_state;
                    organizationsUpdated.old_mobile_no = com.office_mobile_no;
                    organizationsUpdated.organization_status = upd_active.status_idx;
                    organizationsUpdated.old_registered_year = shareHolderCom.registered_year;
                    organizationsUpdated.old_status_shareholder = shareHolderCom.status_shareholder;

                    organizationsUpdated.old_number_of_shares = shareHolderCom.number_of_shares;
                    organizationsUpdated.created_by = userID;
                    organizationsUpdated.modified_by = userID;
                    organizationsUpdated.created_dt = DateTime.Now;
                    organizationsUpdated.modified_dt = DateTime.Now;

                    unitOfWork.CoreOrganizationsUpdatedRepository.Add(organizationsUpdated);
                    var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                    core_org_shareholders_updated shareholderUpd = new core_org_shareholders_updated();
                    // Guid gshareholderUpd = Guid.NewGuid();
                    shareholderUpd.org_shareholder_upd_idx = gshareholderUpd;
                    shareholderUpd.organization_ref = gUpdate;
                    shareholderUpd.stub_ref = stubIDX;
                    shareholderUpd.active_status = 1;
                    shareholderUpd.created_by = userID;
                    shareholderUpd.modified_by = userID;
                    shareholderUpd.created_at = DateTime.Now;
                    shareholderUpd.modified_at = DateTime.Now;
                    unitOfWork.CoreOrgShareholdersUpdatedRepository.Add(shareholderUpd);
                    var resultSH = unitOfWork.Complete(); //motacContext.SaveChanges();

                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ChangeStatusDirector_SaveNew(Guid OrganizationIdx, Guid stubIDX, Guid userID)
        {
            try
            {
                var upd_active = unitOfWork.RefStatusRecordRepository.Find(c => c.status_name == "ACTIVE").FirstOrDefault();

                List<core_organization_directors> directors = new List<core_organization_directors>();
                directors = unitOfWork.CoreOrganizationDirectorsRepository.Find(c => (c.organization_ref == OrganizationIdx && c.active_status == 1)).ToList();
                var refType = unitOfWork.RefReferencesTypesRepository.Find(c => c.ref_type_name == "UPLOADTYPE" ).FirstOrDefault();

                var my_kad = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "MYKAD" && c.ref_type == refType.ref_idx).FirstOrDefault();

                var passport = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PASSPORT" && c.ref_type == refType.ref_idx).FirstOrDefault();

                if (my_kad == null)
                {
                    my_kad = passport;
                }                   


                var pas_pengajian = unitOfWork.RefReferencesRepository.Find(c => c.ref_code == "PAS_PENGGAJIAN" && c.ref_type == refType.ref_idx).FirstOrDefault();

                List<core_persons> core_persons = new List<core_persons>();

                foreach (var director in directors)
                {
                    core_persons_updated PersonUpdated = new core_persons_updated();
                    var person = unitOfWork.CorePersonsRepository.Find(c => (c.person_idx == director.person_ref && c.person_status == upd_active.status_idx)).FirstOrDefault();
                    Guid gUpdatePerson = Guid.NewGuid();
                    Guid gdirectorUpd = Guid.NewGuid();
                    PersonUpdated.person_upd_idx = gUpdatePerson;
                    PersonUpdated.org_director_upd_ref = gdirectorUpd;
                    // PersonUpdated.stub_ref = stubIDX;
                    PersonUpdated.person_status_rec = "ASAL";
                    PersonUpdated.person_type_rec = "Director";
                    PersonUpdated.person_ref = person.person_idx;
                    PersonUpdated.old_person_name = person.person_name;
                    PersonUpdated.old_person_identifier = person.person_identifier;
                    PersonUpdated.old_contact_mobile_no = person.contact_mobile_no;
                    PersonUpdated.old_contact_phone_no=person.contact_phone_no;
                    PersonUpdated.old_person_birthday = person.person_birthday;
                    PersonUpdated.old_contact_addr_1 = person.contact_addr_1;
                    PersonUpdated.old_contact_addr_2 = person.contact_addr_2;
                    PersonUpdated.old_contact_addr_3 = person.contact_addr_3;
                    PersonUpdated.old_contact_postcode = person.contact_postcode;
                    PersonUpdated.old_contact_city = person.contact_city;
                    PersonUpdated.old_contact_state = person.contact_state;
                    PersonUpdated.old_person_gender = person.person_gender;
                    PersonUpdated.old_person_religion = person.person_religion;
                    PersonUpdated.old_person_nationality = person.person_nationality;
                    //PersonUpdated.old_status_shareholder = director.status_shareholder;
                    //PersonUpdated.old_number_of_shares = director.number_of_shares;
                    PersonUpdated.person_status = upd_active.status_idx;
                    PersonUpdated.created_by = userID;
                    PersonUpdated.modified_by = userID;
                    PersonUpdated.created_dt = DateTime.Now;
                    PersonUpdated.modified_dt = DateTime.Now;
                    unitOfWork.CorePersonsUpdatedRepository.Add(PersonUpdated);
                    var result = unitOfWork.Complete(); //motacContext.SaveChanges();

                    var attach = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx && c.upload_type_ref == my_kad.ref_idx && c.active_status == upd_active.status_idx)).FirstOrDefault();

                    if (attach != null)
                    {
                        core_uploads_freeform_by_persons uploads = new core_uploads_freeform_by_persons();
                        Guid gUploadID = Guid.NewGuid();
                        uploads.uploads_freeform_by_persons_idx = gUploadID;
                        uploads.person_ref = gUpdatePerson;
                        uploads.upload_type_ref = attach.upload_type_ref;
                        uploads.upload_name = attach.upload_name;
                        uploads.upload_description = attach.upload_description;
                        uploads.upload_path = attach.upload_path;
                        uploads.active_status = upd_active.status_idx;
                        uploads.created_by = userID;
                        uploads.modified_by = userID;
                        uploads.modified_at = DateTime.Now;
                        uploads.created_at = DateTime.Now;

                        unitOfWork.CoreUploadsFreeFormPersonRepository.Add(uploads);
                        var resultattach = unitOfWork.Complete(); //motacContext.SaveChanges();
                    }

                    var attachPasPengajian = unitOfWork.CoreUploadsFreeFormPersonRepository.Find(c => (c.person_ref == person.person_idx && c.upload_type_ref == pas_pengajian.ref_idx && c.active_status == upd_active.status_idx)).FirstOrDefault();

                    if (attachPasPengajian != null)
                    {
                        core_uploads_freeform_by_persons uploads = new core_uploads_freeform_by_persons();
                        Guid gUploadID = Guid.NewGuid();
                        uploads.uploads_freeform_by_persons_idx = gUploadID;
                        uploads.person_ref = gUpdatePerson;
                        uploads.upload_type_ref = attach.upload_type_ref;
                        uploads.upload_name = attach.upload_name;
                        uploads.upload_description = attach.upload_description;
                        uploads.upload_path = attach.upload_path;
                        uploads.active_status = upd_active.status_idx;
                        uploads.created_by = userID;
                        uploads.modified_by = userID;
                        uploads.modified_at = DateTime.Now;
                        uploads.created_at = DateTime.Now;

                        unitOfWork.CoreUploadsFreeFormPersonRepository.Add(uploads);
                        var resultattach = unitOfWork.Complete(); //motacContext.SaveChanges();
                    }


                    core_org_directors_updated directorUpd = new core_org_directors_updated();
                    // Guid gshareholderUpd = Guid.NewGuid();
                    directorUpd.org_director_upd_idx = gdirectorUpd;
                    directorUpd.organization_ref = OrganizationIdx;
                    directorUpd.person_ref = gUpdatePerson;
                    directorUpd.stub_ref = stubIDX;
                    directorUpd.active_status = 1;
                    directorUpd.created_by = userID;
                    directorUpd.modified_by = userID;
                    directorUpd.created_at = DateTime.Now;
                    directorUpd.modified_at = DateTime.Now;
                    unitOfWork.CoreOrgDirectorsUpdatedRepository.Add(directorUpd);
                    var resultSH = unitOfWork.Complete(); //motacContext.SaveChanges();
                }

               
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public bool UpdateChangeStatusOrg(core_organizations_updated org_updated, Guid userID)
        {
            try
            {
               var organizationsUpdated = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => c.stub_ref == org_updated.stub_ref).FirstOrDefault();

                //core_organizations_updated organizationsUpdated = new core_organizations_updated(); //motacContext.core_persons.Create();

                organizationsUpdated.new_organization_name = org_updated.new_organization_name;
                organizationsUpdated.new_justification_name=org_updated.new_justification_name;
                organizationsUpdated.new_addr_1 = org_updated.new_addr_1;
                organizationsUpdated.new_addr_2 = org_updated.new_addr_2;
                organizationsUpdated.new_addr_3 = org_updated.new_addr_3;
                organizationsUpdated.new_postcode = org_updated.new_postcode;
                organizationsUpdated.new_city = org_updated.new_city;
                organizationsUpdated.new_state = org_updated.new_state;
                organizationsUpdated.new_paid_capital = org_updated.new_paid_capital;
                organizationsUpdated.is_change_address=org_updated.is_change_address;
                organizationsUpdated.is_change_capital = org_updated.is_change_capital;
                organizationsUpdated.is_change_name=org_updated.is_change_name;
                organizationsUpdated.is_shareholder = org_updated.is_shareholder;
                organizationsUpdated.is_directors = org_updated.is_directors;
                organizationsUpdated.is_premise_ready = org_updated.is_premise_ready;
                organizationsUpdated.created_by = userID;
                organizationsUpdated.modified_by = userID;
                organizationsUpdated.created_dt = DateTime.Now;
                organizationsUpdated.modified_dt = DateTime.Now;
                               
                unitOfWork.CoreOrganizationsUpdatedRepository.TourlistContext.Entry(organizationsUpdated).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //added by samsuri (CR#57259) on 5 jan 2024
        public bool UpdateChangeStatusOrgforBranchInd(core_organizations_updated org_updated, Guid userID)
        {
            try
            {
                var organizationsUpdated = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => c.stub_ref == org_updated.stub_ref).FirstOrDefault();

                organizationsUpdated.is_change_address = org_updated.is_change_address;
                //organizationsUpdated.is_premise_ready = org_updated.is_premise_ready;

                unitOfWork.CoreOrganizationsUpdatedRepository.TourlistContext.Entry(organizationsUpdated).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateChangeStatusIS(Guid userID, string isUpdate,Guid AppId)
        {
            try
            {
                var organizationsUpdated = unitOfWork.CoreOrganizationsUpdatedRepository.Find(c => c.stub_ref == AppId).FirstOrDefault();

                if (isUpdate=="director")               
                    organizationsUpdated.is_directors = 1;

                if (isUpdate == "shareholder")
                    organizationsUpdated.is_shareholder = 1;


                organizationsUpdated.created_by = userID;
                organizationsUpdated.modified_by = userID;
                organizationsUpdated.created_dt = DateTime.Now;
                organizationsUpdated.modified_dt = DateTime.Now;

                unitOfWork.CoreOrganizationsUpdatedRepository.TourlistContext.Entry(organizationsUpdated).State = EntityState.Modified;
                unitOfWork.Complete();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string UpdateCompanyDataSSM(string module_id, string userID, string component_id, TobtabViewModels.tobtab_ssm_organization model_org, string module)
        {

            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            var chklist = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.chklist_instance_idx.ToString() == chklistitems.chklist_instance_ref.ToString())).First();
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == chklist.application_ref.ToString())).First();
            var user = new core_users();
            user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == userID)).First();
            if (module == "ILP")
            {
                var license = unitOfWork.IlpLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();                
            }
            else if (module == "TOBTAB")
            {
                var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
            }
            else if (module == "BPKSP")
            {
                var license = unitOfWork.BPKSPLicensesRepository.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
            }
            var person = unitOfWork.Persons.Find(i => (i.person_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            CultureInfo provider = CultureInfo.InvariantCulture;

            RefHelper refHelper = new RefHelper();
            List<ref_references> refCompOrigin = refHelper.GetRefReferencesByType("SSM_ORIGIN");
            List<ref_references> refCompSts = refHelper.GetRefReferencesByType("SSM_STATUS_SYARIKAT");

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
                data_org.old_organization_identifier = model_org.company_regno;
                data_org.nature_of_business = model_org.nature_of_business;

                if (!DateTime.ParseExact(model_org.date_of_change, "dd/MM/yyyy", null).ToString("dd/MM/yyyy").Equals("01/01/0001"))
                    data_org.date_of_change = DateTime.ParseExact(model_org.date_of_change, "dd/MM/yyyy", null);

                if (model_org.status != null && refCompSts.Find(x => x.ref_code == model_org.status.Trim()) != null)
                    data_org.company_category = (from z in refCompSts where z.ref_code == model_org.status.Trim() select z.ref_description.Trim()).FirstOrDefault();
                if (model_org.origin != null && refCompOrigin.Find(x => x.ref_code == model_org.origin.Trim()) != null)
                    data_org.country_ref = (from z in refCompOrigin where z.ref_code == model_org.origin.Trim() select z.ref_idx).FirstOrDefault();

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
                //data_org.paid_capital = Decimal.Parse(model_org.share_capital);

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

                data_FI.capital_ordinary_cash = Decimal.Parse(model_org.capital_ordinary_cash);
                data_FI.capital_ordinary_otherwise = Decimal.Parse(model_org.capital_ordinary_otherwise);
                data_FI.capital_others_cash = Decimal.Parse(model_org.capital_others_cash);
                data_FI.capital_others_otherwise = Decimal.Parse(model_org.capital_others_otherwise);
                data_FI.capital_preference_cash = decimal.Parse(model_org.capital_preference_cash);
                data_FI.capital_preference_otherwise = decimal.Parse(model_org.capital_preference_otherwise);

                unitOfWork.CoreOrganizationFinancialInfoRepository.Add(data_FI);

            }
            if (person == null && organization != null)
            {
                organization.organization_name = model_org.company_name;
                organization.old_organization_name = model_org.company_oldname;
                organization.organization_identifier = model_org.company_newregno;
                organization.old_organization_identifier = model_org.company_regno;

                if (!DateTime.ParseExact(model_org.date_of_change, "dd/MM/yyyy", null).ToString("dd/MM/yyyy").Equals("01/01/0001"))
                    organization.date_of_change = DateTime.ParseExact(model_org.date_of_change, "dd/MM/yyyy", null);               

                if (model_org.status != null && refCompSts.Find(x => x.ref_code == model_org.status.Trim()) != null)
                    organization.company_category = (from z in refCompSts where z.ref_code == model_org.status.Trim() select z.ref_description.Trim()).FirstOrDefault();
                if (model_org.origin != null && refCompOrigin.Find(x => x.ref_code == model_org.origin.Trim()) != null)
                    organization.country_ref = (from z in refCompOrigin where z.ref_code == model_org.origin.Trim() select z.ref_idx).FirstOrDefault();

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
                organization.paid_capital = Decimal.Parse(model_org.share_capital);
                if(model_org.share_capital == "0")
                {
                    organization.paid_capital = Decimal.Parse(model_org.capital_ordinary_cash);
                }
                organization.registered_addr_1 = model_org.companyr_addr1;
                organization.registered_addr_2 = model_org.companyr_addr2;
                organization.registered_addr_3 = model_org.companyr_addr3;
                organization.registered_postcode = model_org.companyr_postcode;

                var vStateReg = unitOfWork.RefGeoStatesRepository.Find(i => (i.ssm_code.Contains(model_org.companyr_state))).FirstOrDefault();

                organization.registered_state = (vStateReg == null) ? Guid.Empty : vStateReg.state_idx;

                if (vStateReg != null)
                {
                    var vPostcode_Reg = unitOfWork.VwGeoListRepository.Find(i => (i.town_name == model_org.companyr_town && i.state_code == vStateReg.state_code)).FirstOrDefault();
                    organization.registered_city = (vPostcode_Reg == null) ? Guid.Empty : vPostcode_Reg.town_idx;
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
                //    organization.cosec_city = (vPostcode_Sec == null) ? Guid.Empty : vPostcode_Sec.town_idx;
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

                    data_FI.capital_ordinary_cash = Decimal.Parse(model_org.capital_ordinary_cash);
                    data_FI.capital_ordinary_otherwise = Decimal.Parse(model_org.capital_ordinary_otherwise);
                    data_FI.capital_others_cash = Decimal.Parse(model_org.capital_others_cash);
                    data_FI.capital_others_otherwise = Decimal.Parse(model_org.capital_others_otherwise);
                    data_FI.capital_preference_cash = decimal.Parse(model_org.capital_preference_cash);
                    data_FI.capital_preference_otherwise = decimal.Parse(model_org.capital_preference_otherwise);

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
                    if (model_org.share_capital == "0")
                    {
                        financialInfo.share_capital = Decimal.Parse(model_org.capital_ordinary_cash);
                    }
                    financialInfo.reserve = Decimal.Parse(model_org.reserve);
                    financialInfo.retain_earning = Decimal.Parse(model_org.retain_earning);
                    financialInfo.bal_minority_interest = Decimal.Parse(model_org.bal_minority_interest);
                    financialInfo.revenue = Decimal.Parse(model_org.revenue);
                    financialInfo.profit_lost_before_tax = Decimal.Parse(model_org.profit_lost_before_tax);
                    financialInfo.profit_lost_after_tax = Decimal.Parse(model_org.profit_lost_after_tax);
                    financialInfo.net_dividend = Decimal.Parse(model_org.net_dividend);
                    financialInfo.income_minority_interest = Decimal.Parse(model_org.income_minority_interest);

                    financialInfo.capital_ordinary_cash = Decimal.Parse(model_org.capital_ordinary_cash);
                    financialInfo.capital_ordinary_otherwise = Decimal.Parse(model_org.capital_ordinary_otherwise);
                    financialInfo.capital_others_cash = Decimal.Parse(model_org.capital_others_cash);
                    financialInfo.capital_others_otherwise = Decimal.Parse(model_org.capital_others_otherwise);
                    financialInfo.capital_preference_cash = decimal.Parse(model_org.capital_preference_cash);
                    financialInfo.capital_preference_otherwise = decimal.Parse(model_org.capital_preference_otherwise);

                    financialInfo.active_status = 1;

                    financialInfo.modified_at = DateTime.Now;
                    financialInfo.modified_by = user.user_idx;
                    unitOfWork.CoreOrganizationFinancialInfoRepository.TourlistContext.Entry(financialInfo).State = EntityState.Modified;
                }
                unitOfWork.CoreOrganizations.Update(organization);
            }


            //chklistitems.bool1 = status;
            unitOfWork.Complete();
            return "success";
        }
        public string ajaxUpdateCompanyDirectorsSSM(string module_id, string component_id, string userID, TourlistDataLayer.ViewModels.Tobtab.TobtabViewModels.tobtab_ssm_directors model_org, string module)
        {

            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            var chklist = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.chklist_instance_idx.ToString() == chklistitems.chklist_instance_ref.ToString())).First();
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == chklist.application_ref.ToString())).First();
           
            var user = new core_users();
            if (module == "ILP")
            {
                var license = unitOfWork.IlpLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
                user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == userID)).First();
            }
            else if (module == "TOBTAB")
            {
                var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
                user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == userID)).First();
            }

            var person = unitOfWork.Persons.Find(i => (i.person_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            

            var genderMale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "MALE")).FirstOrDefault();
            var genderFemale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "FEMALE")).FirstOrDefault();

            var dir_person = unitOfWork.Persons.Find(i => (i.person_identifier.ToString() == model_org.director_docno.ToString())).FirstOrDefault();
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
                data_org.date_appointed = DateTime.ParseExact(model_org.director_date_appointed, "dd/MM/yyyy", null);
                data_org.is_executive = (short)((model_org.director_designation != "") ? 1 : 0);
                data_org.active_status = 1;
                data_org.created_at = DateTime.Now;
                data_org.created_by = user.user_idx;
                data_org.modified_at = DateTime.Now;
                data_org.modified_by = user.user_idx;

                unitOfWork.Persons.Add(data_person);
                unitOfWork.CoreOrganizationDirectorsRepository.Add(data_org);

            }else if (dir_person != null)
            {
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

                var director = unitOfWork.CoreOrganizationDirectorsRepository.Find(i => (i.organization_ref == organization.organization_idx && i.person_ref.ToString() == dir_person.person_idx.ToString())).FirstOrDefault();
                 if (director == null)
                {
                    var data_org = new TourlistDataLayer.DataModel.core_organization_directors();
                    data_org.organization_director_idx = Guid.NewGuid();
                    data_org.organization_ref = organization.organization_idx;
                    data_org.person_ref = dir_person.person_idx;
                    if (model_org.director_date_appointed == "01/01/0001")
                    {
                        data_org.date_appointed = null;
                    }
                    else
                    {
                        data_org.date_appointed = DateTime.ParseExact(model_org.director_date_appointed, "dd/MM/yyyy", null);
                    }
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
                    if(model_org.director_date_appointed == "01/01/0001")
                    {
                        director.date_appointed = null;
                    }
                    else
                    {
                        director.date_appointed = DateTime.ParseExact(model_org.director_date_appointed, "dd/MM/yyyy", null);
                    }
                    director.is_executive = (short)((model_org.director_designation != "") ? 1 : 0);
                    director.active_status = 1;
                    director.created_at = DateTime.Now;
                    director.created_by = user.user_idx;
                    director.modified_at = DateTime.Now;
                    director.modified_by = user.user_idx;
                    unitOfWork.CoreOrganizationDirectorsRepository.Update(director);
                }
                unitOfWork.Persons.Update(dir_person);
            }

            unitOfWork.Complete();
            return "success";
        }

        public string ajaxUpdateCompanyShareholdersSSM(string module_id, string component_id, string userID, TourlistDataLayer.ViewModels.Tobtab.TobtabViewModels.tobtab_ssm_shareholders model_org, string module)
        {
            var chklistitems = unitOfWork.CoreChkItemsInstancesRepository.Find(i => (i.chkitem_instance_idx.ToString() == component_id)).First();
            var chklist = unitOfWork.CoreChkListInstancesRepository.Find(i => (i.chklist_instance_idx.ToString() == chklistitems.chklist_instance_ref.ToString())).First();
            var application = unitOfWork.FlowApplicationStubs.Find(i => (i.apply_idx.ToString() == chklist.application_ref.ToString())).First();
            var user = new core_users();
            if (module == "ILP")
            {
                var license = unitOfWork.IlpLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
                user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == userID)).First();
            }
            else if (module == "TOBTAB")
            {
                var license = unitOfWork.TobtabLicenses.Find(i => (i.stub_ref.ToString() == application.apply_idx.ToString())).First();
                user = unitOfWork.Users.Find(i => (i.user_idx.ToString() == userID)).First();
            }

            var person = unitOfWork.Persons.Find(i => (i.person_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();
            var organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_idx.ToString() == user.person_ref.ToString())).FirstOrDefault();

            var sh_person = unitOfWork.Persons.Find(i => (i.person_identifier.ToString() == model_org.shareholder_docno.ToString())).FirstOrDefault();
            if (model_org.shareholder_docno.ToString() == "-")
            {
                sh_person = unitOfWork.Persons.Find(i => (i.person_identifier.ToString() == model_org.shareholder_name.ToString())).FirstOrDefault();
            }
            var sh_organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_identifier.ToString() == model_org.shareholder_docno.ToString())).FirstOrDefault();
            if (model_org.shareholder_docno.ToString() == "-")
            {
                sh_organization = unitOfWork.CoreOrganizations.Find(i => (i.organization_identifier.ToString() == model_org.shareholder_docno.ToString() && i.organization_name.ToString() == model_org.shareholder_name.ToString())).FirstOrDefault();
            }
            var genderMale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "MALE")).FirstOrDefault();
            var genderFemale = unitOfWork.RefReferencesRepository.Find(i => (i.ref_code.ToString() == "FEMALE")).FirstOrDefault();


            if (model_org.shareholder_idType == "C" || model_org.shareholder_idType == "XLZ" 
                || model_org.shareholder_idType == "XAK" || model_org.shareholder_idType == "XLS")
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

                   // var sh_org = unitOfWork.CoreOrganizations.Find(i => (i.organization_identifier.ToString() == model_org.shareholder_docno.ToString())).FirstOrDefault();
                    var data_orgShare = new TourlistDataLayer.DataModel.core_organization_shareholders();

                    data_orgShare.organization_shareholder_idx = Guid.NewGuid();
                    data_orgShare.organization_ref = organization.organization_idx;
                    data_orgShare.shareholder_organization_ref = data_org.organization_idx;
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
                    var shareholder = unitOfWork.CoreOrganizationShareholders.Find(i => 
                    i.shareholder_organization_ref.ToString() == sh_organization.organization_idx.ToString() &&
                    i.organization_ref == organization.organization_idx).FirstOrDefault();

                    /*organization.organization_identifier = model_org.shareholder_docno;
                    organization.organization_name = model_org.shareholder_name;

                    organization.organization_status = Guid.Parse("6F217B31-A584-4883-B5E5-65E00FBF98E7");
                    organization.created_dt = DateTime.Now;
                    organization.created_by = user.user_idx;
                    organization.modified_dt = DateTime.Now;
                    organization.modified_by = user.user_idx;*/
                    if (shareholder == null)
                    {
                        var data_org = new TourlistDataLayer.DataModel.core_organization_shareholders();

                        data_org.organization_shareholder_idx = Guid.NewGuid();
                        data_org.organization_ref = organization.organization_idx;
                        data_org.shareholder_organization_ref = sh_organization.organization_idx;
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

        public string ajaxUpdateCompanyChargersSSM(string organization_id, string charge_num, Models.MM2HModels.mm2h_ssm_charges model_org, Guid user_id)
        {
            var charges = unitOfWork.CoreOrganizationChargesRepository.Find(i => (i.organization_ref.ToString() == organization_id && i.charge_num == charge_num)).FirstOrDefault();

            if (charges != null)
            {
                charges.organization_ref = Guid.Parse(organization_id);
                charges.chargee_name = model_org.chargee_name;
                charges.charge_num = model_org.charge_num;
                charges.charge_status = model_org.charge_status;
                charges.total_charge = model_org.total_charge;
                charges.date_creation = model_org.date_creation;
                charges.active_status = 1;
                charges.created_at = DateTime.Now;
                charges.created_by = user_id;
                charges.modified_at = DateTime.Now;
                charges.modified_by = user_id;
                unitOfWork.CoreOrganizationChargesRepository.TourlistContext.Entry(charges).State = EntityState.Modified;

            }
            else
            {
                var data_Chargers = new TourlistDataLayer.DataModel.core_organization_charges();
                data_Chargers.organization_charges_idx = Guid.NewGuid();
                data_Chargers.organization_ref = Guid.Parse(organization_id);
                data_Chargers.chargee_name = model_org.chargee_name;
                data_Chargers.charge_num = model_org.charge_num;
                data_Chargers.charge_status = model_org.charge_status;
                data_Chargers.total_charge = model_org.total_charge;
                data_Chargers.date_creation = model_org.date_creation;
                data_Chargers.active_status = 1;
                data_Chargers.created_at = DateTime.Now;
                data_Chargers.created_by = user_id;
                data_Chargers.modified_at = DateTime.Now;
                data_Chargers.modified_by = user_id;
                unitOfWork.CoreOrganizationChargesRepository.Add(data_Chargers);
            }

            unitOfWork.Complete();
            return "success";
        }

        public List<TourlistDataLayer.DataModel.ref_pbt> GetRefPBT(string negeri_id)
        {
            List<TourlistDataLayer.DataModel.ref_pbt> clsPBT = new List<TourlistDataLayer.DataModel.ref_pbt>();
            if (!string.IsNullOrEmpty(negeri_id))
            {
                Guid negeri_idx = Guid.Parse(negeri_id);
                clsPBT = unitOfWork.RefPbtRepository.Find(c => (c.state_ref == negeri_idx)).ToList();
            }
            return clsPBT;

        }


    }
}
