using System;
using System.Linq;
using System.Text;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

namespace TourlistDataLayer.Persistence.Repositories
{
    public class TobtabLicensesRepository : Repository<tobtab_licenses>, IRepository<tobtab_licenses>
    {
        public TobtabLicensesRepository(TourlistContext context) : base(context)
        {
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }

        public tobtab_licenses GetTobtabLicenseByGuid(Guid Idx)
        {
            var data = TourlistContext.TobtabLicenses.Where(c => c.tobtab_idx == Idx).First();
            return data;
        }

        //Added by samsuri (CR#57258)  on 10 Jan 2024
        public tobtab_licenses GetTobtabLicenseByStubRef(Guid Idx)
        {
            var data = TourlistContext.TobtabLicenses.Where(c => c.stub_ref == Idx).First();
            return data;
        }

        public tobtab_licenses SaveNewLicense()
        {
            var new_data = TourlistContext.TobtabLicenses.Create();

            new_data.tobtab_idx = Guid.NewGuid();
            new_data.stub_ref = Guid.NewGuid();
            new_data.license_ref_code = GenerateReferenceNumber();
            new_data.license_type_list = Guid.NewGuid();
            new_data.organization_ref = Guid.NewGuid();
            new_data.supporting_document_list = Guid.NewGuid();
            new_data.created_by = Guid.NewGuid();
            new_data.active_status = 1;
            new_data.created_at = DateTime.Now;

            TourlistContext.TobtabLicenses.Add(new_data);
            TourlistContext.SaveChanges();

            return new_data;
        }

        //added by samsuri (CR#57258) on 11 jan 2024
        public Guid UpdateSupportingDocList(Guid Idx)
        {
            var data = TourlistContext.TobtabLicenses.Where(c => c.tobtab_idx == Idx).First();
            if(data != null)
            {
                data.supporting_document_list = Guid.NewGuid();
                TourlistContext.SaveChanges();
            }
            return (Guid)data.supporting_document_list;
        }

        private string GenerateReferenceNumber()
        {
            return "TTB-" + GetRandomText(5).ToUpperInvariant();
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
    }
}