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
        ///  урс.
        /// </summary>
        /// <remarks>
        /// ѕо идее тут могут выскочить ошибки, например преобразовани€ пришедшего ответа или что ещЄ.
        /// Ќо всЄ не предусмотришь.
        /// Ќа это есть обработчик, настроенный в Startup-е.
        /// </remarks>
        public async Task<ValuteCursOnDate[]> GetCursOnDateAsync(DateTime date)
        {
            // Ќа вс€кий, вдруг с временем передали. ј мне только дату хочетс€.
            // Ёто не лишнее т.к. € не нашЄл в документации,
            // что можно передавать с временем и от него нет зависимости.
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
        /// ѕеречисление валют.
        /// </summary>
        /// <remarks>
        /// ћетод здесь не используетс€.
        /// —оздан просто чтобы получить список кодов валют.
        /// ѕри использовании в try catch обернуть только не забыть надо.
        /// ј то может и ответ не прийти и поле помен€ть формат. Ќу вдруг.
        /// </remarks>
        public async Task<Object[]> EnumValutesAsync()
        {
            using DailyInfoSoapClient soapClient = CreateDailyInfoSoapClient();

            // Seld Ч формат -boolean
            // False Ч перечень ежедневных валют
            // True Ч перечень ежемес€чных валют
            ArrayOfXElement arrayOfXElement = await soapClient.EnumValutesAsync(false);

            Object[] result = InterpretTables(arrayOfXElement: arrayOfXElement
                , selector: dataRow => new
                {
                    Vcode = dataRow.Field<String>("Vcode").Trim(),// Ч ¬нутренний код валюты*
                    Vname = dataRow.Field<String>("Vname").Trim(),// Ч Ќазвание валюты
                    VEngname = dataRow.Field<String>("VEngname").Trim(), // Ч јнгл. название валюты
                    Vnom = dataRow.Field<String>("Vnom").Trim(),//  Ч Ќоминал
                    VcommonCode = dataRow.Field<String>("VcommonCode").Trim(),// Ч ¬нутренний код валюты, €вл€ющейс€ ТбазовойТ**
                    VnumCode = dataRow.Field<String>("VnumCode").Trim(),// Ч цифровой код ISO
                    VcharCode = dataRow.Field<String>("VcharCode").Trim(),// Ч 3х буквенный код ISO
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