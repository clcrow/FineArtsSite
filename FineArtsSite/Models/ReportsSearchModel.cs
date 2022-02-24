using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FineArtsSite.Models
{
    public class ReportsSearchModel
    {
        [DisplayName("Artist Name: ")]
        public IEnumerable<SelectListItem> ArtistName { get; set; }
        public string _artistName { get; set; }

        public string custTake { get; set; }

        public string churchTake { get; set; }

        public string total { get; set; }

        public DataTable results { get; set; }
    }
}
