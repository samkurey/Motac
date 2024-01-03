using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;
using TourlistDataLayer.ViewModels;

namespace TourlistDataLayer.Persistence.Repositories
{
    public class IlpBranchesUpdatedRepository : Repository<ilp_branches_updated>, IRepository<ilp_branches_updated>
    {
        public IlpBranchesUpdatedRepository(TourlistContext context) : base(context)
        {
        }

        public ilp_branches_updated SaveNewIlpBranch(ilp_branches_updated ilpBranchesUpdated, Guid user_idx)
        {
            ilp_branches_updated branch = TourlistContext.IlpBranchesUpdated.Where(c => c.ilp_branches_idx == ilpBranchesUpdated.ilp_branches_idx).FirstOrDefault();

            if (branch != null)
            {
                branch.ilp_license_idx = ilpBranchesUpdated.ilp_license_idx;
                branch.new_branch_addr_1 = ilpBranchesUpdated.new_branch_addr_1;
                branch.new_branch_addr_2 = ilpBranchesUpdated.new_branch_addr_2;
                branch.new_branch_addr_3 = ilpBranchesUpdated.new_branch_addr_3;
                branch.new_branch_postcode = ilpBranchesUpdated.new_branch_postcode;
                branch.new_branch_city = ilpBranchesUpdated.new_branch_city;
                branch.new_branch_state = ilpBranchesUpdated.new_branch_state;
                branch.organization_ref = ilpBranchesUpdated.organization_ref;
                                
                branch.modified_dt = DateTime.Now;
                branch.modified_by = user_idx;

                TourlistContext.SaveChanges();
            return branch;

        }

        var new_data = TourlistContext.IlpBranchesUpdated.Create();

            new_data.ilp_branches_idx = ilpBranchesUpdated.ilp_branches_idx;//Guid.NewGuid();
            new_data.ilp_license_idx = ilpBranchesUpdated.ilp_license_idx;
            new_data.ls_change_address = 1;
            new_data.new_branch_addr_1 = ilpBranchesUpdated.new_branch_addr_1;
            new_data.new_branch_addr_2 = ilpBranchesUpdated.new_branch_addr_2;
            new_data.new_branch_addr_3 = ilpBranchesUpdated.new_branch_addr_3;
            new_data.new_branch_postcode = ilpBranchesUpdated.new_branch_postcode;            
            new_data.new_branch_city = ilpBranchesUpdated.new_branch_city;
            new_data.new_branch_state = ilpBranchesUpdated.new_branch_state;

            new_data.old_branch_addr_1 = ilpBranchesUpdated.old_branch_addr_1;
            new_data.old_branch_addr_2 = ilpBranchesUpdated.old_branch_addr_2;
            new_data.old_branch_addr_3 = ilpBranchesUpdated.old_branch_addr_3;
            new_data.old_branch_postcode = ilpBranchesUpdated.old_branch_postcode;
            new_data.old_branch_city = ilpBranchesUpdated.old_branch_city;
            new_data.old_branch_state = ilpBranchesUpdated.old_branch_state;

            new_data.organization_ref = ilpBranchesUpdated.organization_ref;
            new_data.created_dt = DateTime.Now;
            new_data.created_by = user_idx;
            new_data.modified_dt = DateTime.Now;
            new_data.modified_by = user_idx;
            TourlistContext.IlpBranchesUpdated.Add(new_data);
            TourlistContext.SaveChanges();

            return new_data;
        }

        public ilp_branches_updated UpdateIlpBranch(ilp_branches_updated ilpBranchesUpdated)
        {
            ilp_branches_updated branch = TourlistContext.IlpBranchesUpdated.Where(c => c.ilp_branches_idx == ilpBranchesUpdated.ilp_branches_idx).FirstOrDefault();

            if (branch != null)
            {
                branch.new_branch_addr_1 = ilpBranchesUpdated.new_branch_addr_1;
                branch.new_branch_addr_2 = ilpBranchesUpdated.new_branch_addr_2;
                branch.new_branch_addr_3 = ilpBranchesUpdated.new_branch_addr_3;                
                branch.new_branch_postcode = ilpBranchesUpdated.new_branch_postcode;                
                branch.new_branch_city = ilpBranchesUpdated.new_branch_city;                
                branch.new_branch_state = ilpBranchesUpdated.new_branch_state;                                
                branch.organization_ref = ilpBranchesUpdated.organization_ref;

                TourlistContext.SaveChanges();
                return branch;
            }

            return branch;
        }

        public List<ilp_branches_updated> GetIlpBranchesByApplicationRef(Guid application_ref)
        {
            var ilpBranchesUpdated = TourlistContext.IlpBranchesUpdated.Where(c => c.ilp_license_idx == application_ref).ToList();
            return ilpBranchesUpdated;
        }

        public List<ilp_branches_updated> GetIlpBranchesByUserId(Guid user_idx)
        {
            var user = TourlistContext.Users.Where(u => u.user_idx == user_idx).FirstOrDefault();
            var org = TourlistContext.CoreOrganizations.Where(o => o.organization_idx == user.user_organization).FirstOrDefault();
            var ilpBranchesUpdated = TourlistContext.IlpBranchesUpdated.Where(c => c.organization_ref == org.organization_idx).ToList();
            return ilpBranchesUpdated;
        }

        public List<ilp_branches_updated> GetIlpBranchesByOrgID(Guid org_idx)
        {            
            //var org = TourlistContext.CoreOrganizations.Where(o => o.organization_idx == user.user_organization).FirstOrDefault();
            var ilpBranchesUpdated = TourlistContext.IlpBranchesUpdated.Where(c => c.organization_ref == org_idx).ToList();
            return ilpBranchesUpdated;
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }
    }
}
