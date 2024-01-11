using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added by samsuri (CR#57259)  on 2 Jan 2024
namespace TourlistBusinessLayer.Models
{
    public class ILPBranchUpdated
    {
        public Guid ilp_add_branches_upd_idx;
        public Guid stub_ref;
        //public Guid? organization_ref;
        public Int16 Is_change_address;
        //public String new_branch_name; //added by samsuri (CR#57259)  on 27 Dec 2023
        public String new_branch_addr_1;
        public String new_branch_addr_2;
        public String new_branch_addr_3;        
        public String new_branch_postcode;   
        public Guid new_branch_city;
        public Guid new_branch_state;

        public String old_branch_addr_1;
        public String old_branch_addr_2;
        public String old_branch_addr_3;
        public String old_branch_postcode;
        public Guid old_branch_city;
        public Guid old_branch_state;

        public Int16? active_status;
        //public String status;
        //public String branch_fax_no;
        //public Guid branch_state;
        //public String branch_email;
        //public String branch_website;
        //public Guid utility;
        //public String utility_others;
        //public int branch_size;
        //public Decimal authorized_capital;
        //public Decimal paid_capital;
        //public List<ILPMultiSelect> multi_utility;
        //public Guid? pbt_ref;
    }
}
