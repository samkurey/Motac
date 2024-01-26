using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

namespace TourlistDataLayer.Persistence.Repositories
{
    public class TobtabAddBranchesRepository : Repository<tobtab_add_branches>, IRepository<tobtab_add_branches>
    {
        public TobtabAddBranchesRepository(TourlistContext context) : base(context)
        {
        }

        //added by samsuri (CR#57258) on 10 jan 2024
        public List<tobtab_add_branches> GetTobtabBranchesByOrgID(Guid org_idx)
        {
            var TobTabBranches = TourlistContext.TobtabBranches.Where(c => c.organization_ref == org_idx).ToList();
            return TobTabBranches;
        }

        //Added by samsuri (CR#57258) on 10 Jan 2024
        public List<tobtab_add_branches> GetTobtabBranchesByBranchIdx(Guid branch_Idx)
        {
            var TobTabBranches = TourlistContext.TobtabBranches.Where(c => c.tobtab_add_branches_idx == branch_Idx).ToList();
            return TobTabBranches;
        }

        //Added by samsuri (CR#57258) on 10 Jan 2024
        public List<tobtab_add_branches_updated> GetTobtabBranchesUpdatedByBranchIdx(Guid branch_Idx)
        {
            var TTBranchesUpdated = TourlistContext.TobtabBranchesUpdated.Where(c => c.tobtab_add_branches_upd_idx == branch_Idx).ToList();
            return TTBranchesUpdated;
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }

    }
}