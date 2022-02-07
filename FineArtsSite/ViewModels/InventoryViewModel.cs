using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FineArtsSite.Models;

namespace FineArtsSite.ViewModels
{
    public class InventoryViewModel
    {
        public Inventory invItem { get; set; }

        [DisplayName("Artist: ")]
        public string ArtistName { get; set; }
    }
}
