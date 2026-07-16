using System.Collections.Generic;

namespace BloodBankNetwork.Models
{
    public class DashboardViewModel
    {
        public int TotalDonors { get; set; }
        public int TotalBanks { get; set; }
        public int TotalUnits { get; set; }
        public int TotalRequests { get; set; }
        public int TotalDonations { get; set; }

        // Collections for Dashboard Activity Feeds
        public List<DonationRecord> RecentDonations { get; set; }
        public List<BloodRequest> RecentRequests { get; set; }
    }
}