using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Application.DTOs
{
    public class BookdetailsDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public DateOnly PublishedAt { get; set; }


        public bool IsAvailable { get; set; }


        public string CatalogName { get; set; }
    }
}
