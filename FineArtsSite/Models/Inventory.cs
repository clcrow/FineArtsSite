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
    [Table("Inventory")]
    public class Inventory
    {
        [Key, Column("id")]
        [DisplayName("Record ID: ")]
        public int recID { get; set; }
        [DisplayName("Artist")]
        public string ArtistName { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("ItemNum")]
        public string ItemNum { get; set; }

        [DisplayName("Cost")]
        public float Cost { get; set; }

        [DisplayName("Sold")]
        public bool Sold { get; set; }
    }
}
