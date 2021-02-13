using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ServiceDir
{
    public class ValuteCursInput
    {
        [Required(ErrorMessage = "Координата {0} обязательна")]
        public Double? X { get; set; }

        [Required(ErrorMessage = "Координата {0} обязательна")]
        public Double? Y { get; set; }
    }
}