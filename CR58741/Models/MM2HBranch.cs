using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added by samsuri (CR#58741)  on 24 Jan 2024
namespace TourlistBusinessLayer.Models
{
    public class MM2HBranch
    {
        public Guid mm2h_add_branches_idx;
        public Guid? organization_ref;
        public Guid? stub_ref;
        public String branch_name; 
        public String branch_addr_1;
        public String branch_addr_2;
        public String branch_addr_3;
        public String branch_mobile_no;
        public String branch_postcode;
        public String branch_phone_no;
        public Guid branch_city;
        
        public Guid? active_status;
        public String status;
        public String branch_fax_no;
        public Guid branch_state;
        public String branch_email;
        public String branch_website;
        public Guid? pbt_ref;

        public List<MM2HBranchUpdated> branch_updated; 
        public String fileNameSewaBeliPremis;
        public String fileLocSewaBeliPremis;
        public String fileNamepelanLantai;
        public String fileLocpelanLantai;
        public String fileNameGambarPermis;
        public String fileLocGambarPermis;



    }
}
