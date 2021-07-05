using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Models
{
    public abstract class Viewable : BaseEntity
    {
        public Viewable()
        {
            Views = new List<View>();
        }

        public List<View> Views { get; }
    }
}
