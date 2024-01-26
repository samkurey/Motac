using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

//added by samsuri (CR#57258) on 10 jan 2024
namespace TourlistDataLayer.Persistence.Repositories
{
    public class TobtabAddBranchesUpdatedRepository : Repository<tobtab_add_branches_updated>, IRepository<tobtab_add_branches_updated>
    {
        public TobtabAddBranchesUpdatedRepository(TourlistContext context) : base(context)
        {
        }

        public string test(tobtab_add_branches_updated tobtabBranchesUpdated, Guid user_idx)
        {
            //tobtab_add_branches_updated branch = TourlistContext.TobtabBranchesUpdated.Where(c => c.tobtab_add_branches_upd_idx == tobtab_add_branches_upd_idx).FirstOrDefault();
            return "ok";
        }
        public tobtab_add_branches_updated SaveNewTobtabBranch(tobtab_add_branches_updated tobtabBranchesUpdated, Guid user_idx)
        {
            //do update if exist
            tobtab_add_branches_updated branch = TourlistContext.TobtabBranchesUpdated.Where(c => c.tobtab_add_branches_upd_idx == tobtabBranchesUpdated.tobtab_add_branches_upd_idx).FirstOrDefault();

            if (branch != null)
            {
                branch.stub_ref = tobtabBranchesUpdated.stub_ref;
                branch.new_branch_addr_1 = tobtabBranchesUpdated.new_branch_addr_1;
                branch.new_branch_addr_2 = tobtabBranchesUpdated.new_branch_addr_2;
                branch.new_branch_addr_3 = tobtabBranchesUpdated.new_branch_addr_3;
                branch.new_branch_postcode = tobtabBranchesUpdated.new_branch_postcode;
                branch.new_branch_city = tobtabBranchesUpdated.new_branch_city;
                branch.new_branch_state = tobtabBranchesUpdated.new_branch_state;                

                branch.modified_at = DateTime.Now;
                branch.modified_by = user_idx;

                TourlistContext.SaveChanges();
                return branch;

            }

            var new_data = TourlistContext.TobtabBranchesUpdated.Create();

            new_data.tobtab_add_branches_upd_idx = tobtabBranchesUpdated.tobtab_add_branches_upd_idx;//Guid.NewGuid();
            new_data.stub_ref = tobtabBranchesUpdated.stub_ref;
            new_data.Is_change_address = 1;
            new_data.new_branch_addr_1 = tobtabBranchesUpdated.new_branch_addr_1;
            new_data.new_branch_addr_2 = tobtabBranchesUpdated.new_branch_addr_2;
            new_data.new_branch_addr_3 = tobtabBranchesUpdated.new_branch_addr_3;
            new_data.new_branch_postcode = tobtabBranchesUpdated.new_branch_postcode;
            new_data.new_branch_city = tobtabBranchesUpdated.new_branch_city;
            new_data.new_branch_state = tobtabBranchesUpdated.new_branch_state;

            new_data.old_branch_addr_1 = tobtabBranchesUpdated.old_branch_addr_1;
            new_data.old_branch_addr_2 = tobtabBranchesUpdated.old_branch_addr_2;
            new_data.old_branch_addr_3 = tobtabBranchesUpdated.old_branch_addr_3;
            new_data.old_branch_postcode = tobtabBranchesUpdated.old_branch_postcode;
            new_data.old_branch_city = tobtabBranchesUpdated.old_branch_city;
            new_data.old_branch_state = tobtabBranchesUpdated.old_branch_state;
            //new_data.active_status = 1;
            //new_data.organization_ref = ilpBranchesUpdated.organization_ref;
            new_data.created_at = DateTime.Now;
            new_data.created_by = user_idx;
            new_data.modified_at = DateTime.Now;
            new_data.modified_by = user_idx;
            TourlistContext.TobtabBranchesUpdated.Add(new_data);
            TourlistContext.SaveChanges();

            return new_data;
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }

    }
}