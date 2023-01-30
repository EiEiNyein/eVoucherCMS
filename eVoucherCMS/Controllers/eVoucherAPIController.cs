using eVoucherCMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace eVoucherCMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class eVoucherAPIController : ControllerBase
    {
        public eVoucherServices _service;
        public eVoucherAPIController(eVoucherServices service)
        {
            _service = service;
        }

        [HttpGet]
        [ActionName("GeteVouchers")]
        public async Task<IActionResult> GeteVouchers()
        {
            DataTable dt = await _service.GeteVouchers();
            return Ok(dt);
        }
    }
}
