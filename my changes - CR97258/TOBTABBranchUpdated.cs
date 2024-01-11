using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added by samsuri (CR#57258)  on 10 Jan 2024
namespace TourlistBusinessLayer.Models
{
    public class TOBTABBranchUpdated
    {
        public Guid tobtab_add_branches_upd_idx;
        public Guid stub_ref;        
        public Int16 Is_change_address;
        
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
        public Guid? old_branch_city;
        public Guid old_branch_state;

        public Int16? active_status;
    }
}
