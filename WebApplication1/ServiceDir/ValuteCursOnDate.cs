using System;

namespace WebApplication1.ServiceDir
{
    public class ValuteCursOnDate
    {
        /// <summary>
        /// �������� ������.
        /// </summary>
        public String Vname { get; set; }

        /// <summary>
        /// �������.
        /// </summary>
        public Decimal Vnom { get; set; }

        /// <summary>
        /// ����.
        /// </summary>
        public Decimal Vcurs { get; set; }

        /// <summary>
        /// ISO �������� ��� ������.
        /// </summary>
        public Int32 Vcode { get; set; }

        /// <summary>
        /// ISO ���������� ��� ������.
        /// </summary>
        public String VchCode { get; set; }
    }
}