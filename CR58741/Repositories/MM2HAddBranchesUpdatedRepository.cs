using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

//added by samsuri (CR#57258) on 10 jan 2024
namespace TourlistDataLayer.Persistence.Repositories
{
    public class MM2HAddBranchesUpdatedRepository : Repository<mm2h_add_branches_updated>, IRepository<mm2h_add_branches_updated>
    {
        public MM2HAddBranchesUpdatedRepository(TourlistContext context) : base(context)
        {
        }

        public mm2h_add_branches_updated SaveNewMM2HBranch(mm2h_add_branches_updated mm2hBranchesUpdated, Guid user_idx)
        {
            //do update if exist
            mm2h_add_branches_updated branch = TourlistContext.MM2HBranchesUpdated.Where(c => c.mm2h_add_branches_upd_idx == mm2hBranchesUpdated.mm2h_add_branches_upd_idx).FirstOrDefault();

            if (branch != null)
            {
                branch.stub_ref = mm2hBranchesUpdated.stub_ref;
                branch.new_branch_addr_1 = mm2hBranchesUpdated.new_branch_addr_1;
                branch.new_branch_addr_2 = mm2hBranchesUpdated.new_branch_addr_2;
                branch.new_branch_addr_3 = mm2hBranchesUpdated.new_branch_addr_3;
                branch.new_branch_postcode = mm2hBranchesUpdated.new_branch_postcode;
                branch.new_branch_city = mm2hBranchesUpdated.new_branch_city;
                branch.new_branch_state = mm2hBranchesUpdated.new_branch_state;                

                branch.modified_at = DateTime.Now;
                branch.modified_by = user_idx;

                TourlistContext.SaveChanges();
                return branch;

            }

            var new_data = TourlistContext.MM2HBranchesUpdated.Create();

            new_data.mm2h_add_branches_upd_idx = mm2hBranchesUpdated.mm2h_add_branches_upd_idx;//Guid.NewGuid();
            new_data.stub_ref = mm2hBranchesUpdated.stub_ref;
            new_data.Is_change_address = 1;
            new_data.new_branch_addr_1 = mm2hBranchesUpdated.new_branch_addr_1;
            new_data.new_branch_addr_2 = mm2hBranchesUpdated.new_branch_addr_2;
            new_data.new_branch_addr_3 = mm2hBranchesUpdated.new_branch_addr_3;
            new_data.new_branch_postcode = mm2hBranchesUpdated.new_branch_postcode;
            new_data.new_branch_city = mm2hBranchesUpdated.new_branch_city;
            new_data.new_branch_state = mm2hBranchesUpdated.new_branch_state;

            new_data.old_branch_addr_1 = mm2hBranchesUpdated.old_branch_addr_1;
            new_data.old_branch_addr_2 = mm2hBranchesUpdated.old_branch_addr_2;
            new_data.old_branch_addr_3 = mm2hBranchesUpdated.old_branch_addr_3;
            new_data.old_branch_postcode = mm2hBranchesUpdated.old_branch_postcode;
            new_data.old_branch_city = mm2hBranchesUpdated.old_branch_city;
            new_data.old_branch_state = mm2hBranchesUpdated.old_branch_state;
            //new_data.active_status = 1;
            //new_data.organization_ref = ilpBranchesUpdated.organization_ref;
            new_data.created_at = DateTime.Now;
            new_data.created_by = user_idx;
            new_data.modified_at = DateTime.Now;
            new_data.modified_by = user_idx;
            TourlistContext.MM2HBranchesUpdated.Add(new_data);
            TourlistContext.SaveChanges();

            return new_data;
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }

    }
}