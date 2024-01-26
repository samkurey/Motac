using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

namespace TourlistDataLayer.Persistence.Repositories
{
    public class IlpLicensesRepository : Repository<ilp_licenses>, IRepository<ilp_licenses>
    {
        public IlpLicensesRepository(TourlistContext context) : base(context)
        {
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }

        public ilp_licenses GetIlpLicenseByStubRef(Guid stubRef)
        {
            var data = TourlistContext.IlpLicenses.Where(c => c.stub_ref == stubRef).FirstOrDefault(); ;
            return data;
        }

        public ilp_licenses GetIlpLicenseByGuid(Guid user_idx, String license_type)
        {
            var data = TourlistContext.IlpLicenses.Where(c => c.organization_ref == user_idx && c.license_type == license_type).OrderByDescending(x => x.created_at).FirstOrDefault();
            return data;
        }

        public ilp_licenses GetIlpLicenseByIdx(Guid Idx)
        {
            var data = TourlistContext.IlpLicenses.Where(c => c.ilp_idx == Idx).FirstOrDefault(); ;
            return data;
        }

        public ilp_licenses SaveNewLicense(Guid stub_ref, Guid user_idx, String license_type, Guid orgIdx, Guid chklist_instance_idx) {
            var new_data = TourlistContext.IlpLicenses.Create();

            new_data.ilp_idx = Guid.NewGuid();
            new_data.stub_ref = stub_ref;
            new_data.license_type = license_type;
            new_data.organization_ref = orgIdx;
            new_data.supporting_document_list = chklist_instance_idx;
            new_data.active_status = 1;
            new_data.created_at = DateTime.Now;
            new_data.created_by = user_idx;
            new_data.modified_at = DateTime.Now;
            new_data.modified_by = user_idx;

            TourlistContext.IlpLicenses.Add(new_data);
            TourlistContext.SaveChanges();

            return new_data;
        }

        //added by samsuri (CR#57259) on 11 jan 2024
        public Guid UpdateSupportingDocList(Guid Idx)
        {
            var data = TourlistContext.IlpLicenses.Where(c => c.ilp_idx == Idx).First();
            if (data != null)
            {
                data.supporting_document_list = Guid.NewGuid();
                TourlistContext.SaveChanges();
            }
            return (Guid)data.supporting_document_list;
        }

        private string GenerateReferenceNumber()
        {
            return "ILP-" + GetRandomText(5).ToUpperInvariant();
        }

        private string GetRandomText(int Length)
        {
            StringBuilder randomText = new StringBuilder();
            string alphabets = "012345679ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyz";
            Random r = new Random();
            for (int j = 0; j <= Length; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }
            return randomText.ToString();
        }

        public bool UpdateRenewalDuration(Guid license_ref, int renewal_duration)
        {
            ilp_licenses license = TourlistContext.IlpLicenses.Where(c => c.ilp_idx == license_ref).FirstOrDefault();

            if (license != null)
            {
                license.renewal_duration = renewal_duration;
                TourlistContext.SaveChanges();
                return true;
            }

            return false;
        }

        public bool UpdateLicenseStatusByIdx(Guid ilp_idx)
        {
            ilp_licenses license = TourlistContext.IlpLicenses.Where(c => c.ilp_idx == ilp_idx).FirstOrDefault();

            if (license != null)
            {
                license.active_status = 2;
                TourlistContext.SaveChanges();
                return true;
            }

            return false;
        }

        public List<ilp_licenses> GetIlpLicensesByTypeAndUserId(String license_type, Guid user_idx)
        {
            var datas = TourlistContext.IlpLicenses.Where(c => c.license_type == license_type && c.created_by == user_idx).ToList();
            return datas;
        }
    }
}