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
        private static DailyInfoSoapClient CreateDailyInfoSoapClient(String svcAsmx)
        {
            var soapClient = new DailyInfoSoapClient(
                endpointConfiguration: DailyInfoSoapClient.EndpointConfiguration.DailyInfoSoap
                //, "http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx"
                , remoteAddress: svcAsmx
            );
            return soapClient;
        }

        /// <summary>
        /// ����.
        /// </summary>
        public async Task<ValuteCursOnDate[]> GetCursOnDateAsync(DateTime date, String svcAsmx)
        {
            // �� ������, ����� � �������� ��������. � ��� ������ ���� �������.
            date = date.Date;

            using DailyInfoSoapClient soapClient = CreateDailyInfoSoapClient(svcAsmx);

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
        public async Task<Object[]> EnumValutesAsync(String svcAsmx)
        {
            using DailyInfoSoapClient soapClient = CreateDailyInfoSoapClient(svcAsmx);

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
            DataSet ds = ToDataSet(arrayOfXElement);

            if (ds.Tables.Count == 0)
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