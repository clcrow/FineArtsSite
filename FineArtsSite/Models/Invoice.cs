using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace FineArtsSite.Models
{
    public class Invoice
    {
        [Key, Column("id")]
        [DisplayName("Record ID: ")]
        public int recID { get; set; }
        [DisplayName("Pieces Sold: ")]
        public string PiecesSold { get; set; }

        [DisplayName("Total Cost: ")]
        public string TotalCost { get; set; }

        [DisplayName("Payment Type: ")]
        public string PaymentType { get; set; }
    }
}
