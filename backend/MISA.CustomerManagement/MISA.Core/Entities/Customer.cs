using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; set; }               // customer_id
        public string? CustomerCode { get; set; }          // customer_code KHyyyyMMxxxxxx

        public string? CustomerType { get; set; }          // customer_type
        public string FullName { get; set; }               // full_name <= 128 chars

        public string? TaxCode { get; set; }               // tax_code
        public string? ShippingAddress { get; set; }      // shipping_address
        public string? BillingAddress { get; set; }       // billing_address

        public string Phone { get; set; }                 // phone unique
        public string Email { get; set; }                 // email unique

        public DateTime? LastPurchaseDate { get; set; }   // last_purchase_date
        public string? PurchasedSummary { get; set; }    // purchased_summary
        public string? LastPurchasedItem { get; set; }   // last_purchased_item

        public DateTime CreatedAt { get; set; }           // created_at
        public DateTime UpdatedAt { get; set; }           // updated_at

        public string IsDelete { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
