using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Domain.Entities
{
    public class Catalog
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Book>? Books { get; set; }   //allows null due to the catalog may be exisited and not has any books 

    }
}
