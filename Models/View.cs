using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Models
{
    public class View : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime Created { get; set; }
    }
}
