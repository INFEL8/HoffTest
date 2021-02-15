using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using ServiceReference1;

namespace WebApplication1.ServiceDir
{
    public class Service
    {
        private static DailyInfoSoapClient CreateDailyInfoSoapClient()
        {
            var soapClient = new DailyInfoSoapClient(
                endpointConfiguration: DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap
            );
            return soapClient;
        }

        /// <summary>
        /// ����.
        /// </summary>
        /// <remarks>
        /// �� ���� ��� ����� ��������� ������, �������� �������������� ���������� ������ ��� ��� ���.
        /// �� �� �� �������������.
        /// �� ��� ���� ����������, ����������� � Startup-�.
        /// </remarks>
        public async Task<ValuteCursOnDate[]> GetCursOnDateAsync(DateTime date)
        {
            // �� ������, ����� � �������� ��������. � ��� ������ ���� �������.
            // ��� �� ������ �.�. � �� ����� � ������������,
            // ��� ����� ���������� � �������� � �� ���� ��� �����������.
            date = date.Date;

            using DailyInfoSoapClient soapClient = CreateDailyInfoSoapClient();

            ArrayOfXElement arrayOfXElement = await soapClient.GetCursOnDateAsync(date);

            ValuteCursOnDate[] result = InterpretTables(arrayOfXElement: arrayOfXElement
                , selector: dataRow => new ValuteCursOnDate
                {
                    Vname = dataRow.Field<String>("Vname").Trim(),
                    Vnom = dataRow.Field<Decimal>("Vnom"),
                    Vcurs = dataRow.Field<Decimal>("Vcurs"),
                    Vcode = dataRow.Field<Int32>("Vcode"),
                    VchCode = dataRow.Field<String>("VchCode").Trim(),
                }
            );

            return result;
        }

        /// <summary>
        /// ������������ �����.
        /// </summary>
        /// <remarks>
        /// ����� ����� �� ������������.
        /// ������ ������ ����� �������� ������ ����� �����.
        /// ��� ������������� � try catch �������� ������ �� ������ ����.
        /// � �� ����� � ����� �� ������ � ���� �������� ������. �� �����.
        /// </remarks>
        public async Task<Object[]> EnumValutesAsync()
        {
            using DailyInfoSoapClient soapClient = CreateDailyInfoSoapClient();

            // Seld � ������ -boolean
            // False � �������� ���������� �����
            // True � �������� ����������� �����
            ArrayOfXElement arrayOfXElement = await soapClient.EnumValutesAsync(false);

            Object[] result = InterpretTables(arrayOfXElement: arrayOfXElement
                , selector: dataRow => new
                {
                    Vcode = dataRow.Field<String>("Vcode").Trim(),// � ���������� ��� ������*
                    Vname = dataRow.Field<String>("Vname").Trim(),// � �������� ������
                    VEngname = dataRow.Field<String>("VEngname").Trim(), // � ����. �������� ������
                    Vnom = dataRow.Field<String>("Vnom").Trim(),//  � �������
                    VcommonCode = dataRow.Field<String>("VcommonCode").Trim(),// � ���������� ��� ������, ���������� ��������**
                    VnumCode = dataRow.Field<String>("VnumCode").Trim(),// � �������� ��� ISO
                    VcharCode = dataRow.Field<String>("VcharCode").Trim(),// � 3� ��������� ��� ISO
                } as Object
            );

            return result;
        }



        private static T[] InterpretTables<T>(ArrayOfXElement arrayOfXElement, Func<DataRow, T> selector)
        {
            if (arrayOfXElement is null)
            {
                return Array.Empty<T>();
            }

            DataSet ds = ToDataSet(arrayOfXElement);

            if (!(ds?.Tables.Count > 0))
            {
                return Array.Empty<T>();
            }

            T[] result = ds.Tables[0].AsEnumerable()
                .Select(selector)
                .ToArray();

            return result;
        }

        private static DataSet ToDataSet(ArrayOfXElement data)
        {
            var result = new DataSet();
            var rawXml = new XElement("Root", data.Nodes).ToString();

            using var reader = new StringReader(rawXml);
            result.ReadXml(reader);

            return result;
        }
    }
}