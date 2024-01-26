using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

namespace TourlistDataLayer.Persistence.Repositories
{
    public class MM2HAddBranchesRepository : Repository<mm2h_add_branches>, IRepository<mm2h_add_branches>
    {
        public MM2HAddBranchesRepository(TourlistContext context) : base(context)
        {
        }

        //added by samsuri (CR#58741) on 24 jan 2024
        public List<mm2h_add_branches> GetMM2HBranchesByOrgID(Guid org_idx)
        {
            var mm2hBranches = TourlistContext.MM2HBranches.Where(c => c.organization_ref == org_idx).ToList();
            return mm2hBranches;
        }

        //Added by samsuri (CR#58741) on 24 Jan 2024
        public List<mm2h_add_branches> GetMM2HBranchesByBranchIdx(Guid branch_Idx)
        {
            var mm2hBranches = TourlistContext.MM2HBranches.Where(c => c.mm2h_add_branches_idx == branch_Idx).ToList();
            return mm2hBranches;
        }

        //Added by samsuri (CR#57258) on 10 Jan 2024
        public List<mm2h_add_branches_updated> GetMM2HBranchesUpdatedByBranchIdx(Guid branch_Idx)
        {
            var TTBranchesUpdated = TourlistContext.MM2HBranchesUpdated.Where(c => c.mm2h_add_branches_upd_idx == branch_Idx).ToList();
            return TTBranchesUpdated;
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }
    }
}
