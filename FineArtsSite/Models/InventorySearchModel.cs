using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FineArtsSite.Models
{
    public class InventorySearchModel
    {
        [DisplayName("Artist Name: ")]
        public IEnumerable<SelectListItem> ArtistName { get; set; }
        public string _artistName { get; set; }

        public DataTable results { get; set; }
    }
}
