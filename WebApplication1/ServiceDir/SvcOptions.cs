using System;

namespace WebApplication1.ServiceDir
{
    public class SvcOptions
    {
        /// <summary>
        /// Ссылка на API.
        /// </summary>
        public String CbRfAsmx { get; set; }

        /// <summary>
        /// Радиус окружности, куда должна попадать точка.
        /// </summary>
        public Double Radius { get; set; }

        /// <summary>
        /// Код валюты.
        /// </summary>
        public String ValuteCode { get; set; }
    }
}
