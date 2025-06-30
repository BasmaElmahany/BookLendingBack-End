using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Domain.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public DateOnly PublishedAt { get; set; }


        public bool IsAvailable { get; set; }

        [ForeignKey("catalog")]
        public int CatalogId { get; set; }


        public Catalog? catalog { get; set; }
    }
}
