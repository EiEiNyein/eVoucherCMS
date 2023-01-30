using eVoucherCMS.Models;
using eVoucherCMS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace eVoucherCMS.Controllers
{
    public class eVoucherController : Controller
    {
        public eVoucherServices _service;
        public eVoucherController(eVoucherServices service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }


        #region eVoucher 28-Jan-2022
        public IActionResult EvoucherList()
        {
            return View();
        }

        public async  Task<IActionResult> EvoucherNew()
        {
            List<PaymentInfo> payList = await _service.GetPaymentList();
            ViewBag.PaymentList = payList;
            return View();
        }

        public IActionResult EvoucherDetail(string id)
        {
            ViewBag.PaymentList = string.Empty;
            if (!string.IsNullOrEmpty(id)) ViewBag.VoucherNo = id;
            return View();
        }
        #endregion

    }
}
