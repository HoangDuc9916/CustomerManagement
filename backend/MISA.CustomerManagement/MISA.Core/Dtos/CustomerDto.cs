using MISA.Core.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Dtos
{
    public class CustomerDto
    {
        [MISARequired("Full name is required.")]
        public string FullName { get; set; }

        [MISARequired("Phone number is required.")]
        public string Phone { get; set; }

        [MISARequired("Email is required.")]
        public string Email { get; set; }

        public string CustomerType { get; set; }
        public string TaxCode { get; set; }

        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }

        public string PurchasedSummary { get; set; }
        public string LastPurchasedItem { get; set; }
        public DateTime? LastPurchaseDate { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
