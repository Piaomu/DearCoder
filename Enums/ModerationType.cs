using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DearCoder.Enums
{
    public enum ModerationType
    {
        [Display(Name = "none")]
        none,
        [Display(Name = "Political Propaganda")]
        PoliticalPropaganda,
        [Display(Name = "Offensive Language")]
        Language,
        [Display(Name = "Druge References")]
        Drugs,
        [Display(Name = "Threatening Speech")]
        Threatening,
        [Display(Name = "Sexual Content")]
        Sexual,
        [Display(Name = "Targeted Shaming")]
        HateSpeech,
        [Display(Name = "Fraud")]
        Fraud
    }
}
