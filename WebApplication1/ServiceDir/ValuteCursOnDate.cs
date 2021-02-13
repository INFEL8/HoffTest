using System;

namespace WebApplication1.ServiceDir
{
    public class ValuteCursOnDate
    {
        /// <summary>
        /// Название валюты.
        /// </summary>
        public String Vname { get; set; }

        /// <summary>
        /// Номинал.
        /// </summary>
        public Decimal Vnom { get; set; }

        /// <summary>
        /// Курс.
        /// </summary>
        public Decimal Vcurs { get; set; }

        /// <summary>
        /// ISO Цифровой код валюты.
        /// </summary>
        public Int32 Vcode { get; set; }

        /// <summary>
        /// ISO Символьный код валюты.
        /// </summary>
        public String VchCode { get; set; }
    }
}