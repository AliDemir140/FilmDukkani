using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CategoryDTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string CategoryName { get; set; }
    }
}
