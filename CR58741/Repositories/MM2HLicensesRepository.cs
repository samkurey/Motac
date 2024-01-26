using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

namespace TourlistDataLayer.Persistence.Repositories
{
    public class MM2HLicensesRepository : Repository<mm2h_licenses>, IRepository<mm2h_licenses>
    {
        public MM2HLicensesRepository(TourlistContext context) : base(context)
        {
        }

        //Added by samsuri (CR#58741)  on 26 Jan 2024
        public mm2h_licenses GetMM2HLicenseByStubRef(Guid Idx)
        {
            var data = TourlistContext.MM2HLicenses.Where(c => c.stub_ref == Idx).First();
            return data;
        }
        //added by samsuri (CR#58741) on 26 jan 2024
        public Guid UpdateSupportingDocList(Guid Idx)
        {
            var data = TourlistContext.MM2HLicenses.Where(c => c.mm2h_idx == Idx).First();
            if (data != null)
            {
                data.supporting_document_list = Guid.NewGuid();
                TourlistContext.SaveChanges();
            }
            return (Guid)data.supporting_document_list;
        }


        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }
    }
}
