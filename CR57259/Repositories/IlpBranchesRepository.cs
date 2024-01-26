using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourlistDataLayer.Core.Repositories;
using TourlistDataLayer.DataModel;

namespace TourlistDataLayer.Persistence.Repositories
{
    public class IlpBranchesRepository : Repository<ilp_branches>, IRepository<ilp_branches>
    {
        public IlpBranchesRepository(TourlistContext context) : base(context)
        {
        }

        public ilp_branches SaveNewIlpBranch(ilp_branches ilpBranches, Guid user_idx)
        {
            var new_data = TourlistContext.IlpBranches.Create();

            new_data.ilp_branches_idx = Guid.NewGuid();
            new_data.ilp_license_idx = ilpBranches.ilp_license_idx;
            new_data.branch_addr_1 = ilpBranches.branch_addr_1;
            new_data.branch_addr_2 = ilpBranches.branch_addr_2;
            new_data.branch_addr_3 = ilpBranches.branch_addr_3;
            new_data.branch_mobile_no = ilpBranches.branch_mobile_no;
            new_data.branch_postcode = ilpBranches.branch_postcode;
            new_data.branch_phone_no = ilpBranches.branch_phone_no;
            new_data.branch_city = ilpBranches.branch_city;
            new_data.branch_fax_no = ilpBranches.branch_fax_no;
            new_data.branch_state = ilpBranches.branch_state;
            new_data.branch_email = ilpBranches.branch_email;
            new_data.branch_website = ilpBranches.branch_website;
            new_data.branch_size = ilpBranches.branch_size;
            new_data.authorized_capital = ilpBranches.authorized_capital;
            new_data.paid_capital = ilpBranches.paid_capital;
            new_data.utility = Guid.NewGuid();
            new_data.utility_others = ilpBranches.utility_others;
            new_data.organization_ref = ilpBranches.organization_ref;
            new_data.active_status = ilpBranches.active_status;
            new_data.pbt_ref = ilpBranches.pbt_ref;
            new_data.created_dt = DateTime.Now;
            new_data.created_by = user_idx;
            new_data.modified_dt = DateTime.Now;
            new_data.modified_by = user_idx;
            TourlistContext.IlpBranches.Add(new_data);
            TourlistContext.SaveChanges();

            return new_data;
        }

        public ilp_branches UpdateIlpBranch(ilp_branches ilpBranches)
        {
            ilp_branches branch = TourlistContext.IlpBranches.Where(c => c.ilp_branches_idx == ilpBranches.ilp_branches_idx).FirstOrDefault();

            if (branch != null)
            {
                branch.branch_addr_1 = ilpBranches.branch_addr_1;
                branch.branch_addr_2 = ilpBranches.branch_addr_2;
                branch.branch_addr_3 = ilpBranches.branch_addr_3;
                branch.branch_mobile_no = ilpBranches.branch_mobile_no;
                branch.branch_postcode = ilpBranches.branch_postcode;
                branch.branch_phone_no = ilpBranches.branch_phone_no;
                branch.branch_city = ilpBranches.branch_city;
                branch.branch_fax_no = ilpBranches.branch_fax_no;
                branch.branch_state = ilpBranches.branch_state;
                branch.branch_email = ilpBranches.branch_email;
                branch.branch_website = ilpBranches.branch_website;
                branch.branch_size = ilpBranches.branch_size;
                branch.authorized_capital = ilpBranches.authorized_capital;
                branch.paid_capital = ilpBranches.paid_capital;
                branch.utility_others = ilpBranches.utility_others;
                branch.organization_ref = ilpBranches.organization_ref;
                branch.active_status = ilpBranches.active_status;
                branch.pbt_ref = ilpBranches.pbt_ref;

                TourlistContext.SaveChanges();
                return branch;
            }

            return branch;
        }

        public List<ilp_branches> GetIlpBranchesByApplicationRef(Guid application_ref)
        {
            var ilpBranches = TourlistContext.IlpBranches.Where(c => c.ilp_license_idx == application_ref).ToList();
            return ilpBranches;
        }

        //Added by samsuri (CR#57259) on 2 Jan 2024
        public List<ilp_branches> GetIlpBranchesByBranchIdx(Guid branch_Idx)
        {
            var ilpBranches = TourlistContext.IlpBranches.Where(c => c.ilp_branches_idx == branch_Idx).ToList();
            return ilpBranches;
        }

        //Added by samsuri (CR#57259) on 3 Jan 2024
        public List<ilp_branches_updated> GetIlpBranchesUpdatedByBranchIdx(Guid branch_Idx)
        {
            var ilpBranchesUpdated = TourlistContext.IlpBranchesUpdated.Where(c => c.ilp_add_branches_upd_idx == branch_Idx).ToList();
            return ilpBranchesUpdated;
        }

        public List<ilp_branches> GetIlpBranchesByUserId(Guid user_idx)
        {
            var user = TourlistContext.Users.Where(u => u.user_idx == user_idx).FirstOrDefault();
            var org = TourlistContext.CoreOrganizations.Where(o => o.organization_idx == user.user_organization).FirstOrDefault();
            var ilpBranches = TourlistContext.IlpBranches.Where(c => c.organization_ref == org.organization_idx).ToList();
            return ilpBranches;
        }

        public List<ilp_branches> GetIlpBranchesByOrgID(Guid org_idx)
        {            
            //var org = TourlistContext.CoreOrganizations.Where(o => o.organization_idx == user.user_organization).FirstOrDefault();
            var ilpBranches = TourlistContext.IlpBranches.Where(c => c.organization_ref == org_idx).ToList();
            return ilpBranches;
        }

        public TourlistContext TourlistContext
        {
            get { return Context as TourlistContext; }
        }
    }
}
