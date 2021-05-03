using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Enums
{
    public enum PublishState
    {
        [Display(Name = "Production Ready")]
        ProductionReady,
        [Display(Name = "Preview Ready")]
        PreviewReady,
        [Display(Name = "Not Ready")]
        NotReady
    }
}
