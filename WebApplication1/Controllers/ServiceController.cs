using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using WebApplication1.ServiceDir;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ServiceController : ControllerBase
    {
        private readonly SvcOptions _options;
        //private readonly ILogger<ServiceController> _logger;
        private readonly Service _service;

        public ServiceController(Service service
            , IOptionsSnapshot<SvcOptions> optionsSnapshot
            //, ILogger<ServiceController> logger
            )
        {
            //this._logger = logger;
            this._service = service;

            this._options = optionsSnapshot.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Мне захотелось, чтобы и по простому запросу можно было ещё.
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ActionName(nameof(ValuteCurs))]
        public Task<IActionResult> ValuteCurs2([FromQuery] ValuteCursInput input) => this.ValuteCurs(input);

        [HttpGet("x/{x}/y/{y}")]
        public async Task<IActionResult> ValuteCurs([FromRoute] ValuteCursInput input)
        {
            if (this._options.Radius < 0)
            {
                // 0 можно. (0,0) же попадёт в этот радиус.
                return this.Ok(new SvcOutput
                {
                    Result = null,
                    Message = $"Ошибка: В настройках указан отрицательный радиус: {this._options.Radius}",
                });
            }
            if (this._options.ValuteCode?.Length > 0 != true)
            {
                return this.Ok(new SvcOutput
                {
                    Result = null,
                    Message = $"Ошибка: В настройках не указан код валюты: {this._options.ValuteCode}",
                });
            }

            Double x1 = 0d;
            Double y1 = 0d;
            Double x2 = input.X ?? 0; // проверено по атрибуту [Required] через [ApiController]
            Double y2 = input.X ?? 0; // проверено по атрибуту [Required] через [ApiController]
            var distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

            if (distance > this._options.Radius)
            {
                return this.Ok(new SvcOutput
                {
                    Result = null,
                    Message = $"Ошибка: Точка не попадает в заданный радиус: {this._options.Radius}",
                });
            }

            DateTime date;

            if (input.X > 0 && input.Y > 0)
            {
                // Сегодня.
                date = DateTime.Today;
            }
            else if (input.X > 0 && input.Y < 0)
            {
                // Завтра.
                date = DateTime.Today.AddDays(1);
            }
            else if (input.X < 0 && input.Y < 0)
            {
                // Позавчера.
                date = DateTime.Today.AddDays(-2);
            }
            else
            {
                // (input.X < 0 && input.Y > 0)
                // Вчера.
                date = DateTime.Today.AddDays(-1);
            }

            ValuteCursOnDate[] resultArr = await this._service.GetCursOnDateAsync(
                date: date
                // Не придумал как ещё передать адрес АПИ.
                // Но есть ConnectedService.json, созданный WCF-ом, где настраивается это дело.
                , svcAsmx: this._options.CbRfAsmx ?? "http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx"
                );

            if (resultArr?.Any() != true)
            {
                return this.Ok(new SvcOutput
                {
                    Result = null,
                    Message = "По указанным данным курс не сформирован",
                });
            }


            ValuteCursOnDate result = resultArr
                .FirstOrDefault(x =>
                String.Equals(x.VchCode.Trim(), this._options.ValuteCode.Trim(), StringComparison.OrdinalIgnoreCase));

            if (result == null)
            {
                return this.Ok(new SvcOutput
                {
                    Result = null,
                    Message = $"Курс валюты {this._options.ValuteCode} не найден",
                });
            }

            // Штучка, куда удобно ставить точку.
            { }

            return this.Ok(new SvcOutput
            {
                Result = new
                {
                    Nominal = result.Vnom,
                    Curs = result.Vcurs,
                },
                Message = null,
            });
        }
    }
}
