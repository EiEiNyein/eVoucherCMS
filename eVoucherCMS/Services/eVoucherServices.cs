using eVoucherCMS.Helpers;
using eVoucherCMS.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;

namespace eVoucherCMS.Services
{
    public class eVoucherServices
    {
        private readonly HttpClient _client;
        private IHttpContextAccessor _httpContextAccessor;
        public static string BasePath = "api/Authorize/GetAPIKey/1/JD8U3BB2";

        public eVoucherServices(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task GetToken()
        {
            var response = await _client.GetAsync(BasePath);
            Constants.token = await response.ReadContentAsync<string>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.token);
            _httpContextAccessor.HttpContext.Session.SetString("Bearer", Constants.token);
            //return Constants.token;
        }

        public async Task<DataTable> GeteVouchers()
        {
            DataTable dt = new();
            try
            {
                var strToken = _httpContextAccessor.HttpContext.Session.GetString("Bearer");
                try
                {
                    if (string.IsNullOrWhiteSpace(strToken))
                    {
                        await GetToken();
                    }
                }
                catch(SecurityTokenExpiredException ex)
                {
                    await GetToken();
                }
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.token);
                BasePath = "api/eVouchereAPI/GeteVouchers";

                var response = await _client.GetAsync(BasePath);

                if (response.IsSuccessStatusCode == false)
                    throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

                var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                
                var result = System.Text.Json.JsonSerializer.Deserialize<List<eVoucherInfo>>(dataAsString);
                dt = ToDataTable(result);
            }
            catch(Exception ex)
            {
                var test = ex.Message;
            }
            return dt;
        }

        public async Task<List<PaymentInfo>> GetPaymentList()
        {
            List<PaymentInfo> payList = new();
            try
            {
                var strToken = _httpContextAccessor.HttpContext.Session.GetString("Bearer");
                try
                {
                    if (string.IsNullOrWhiteSpace(strToken))
                    {
                        await GetToken();
                    }
                }
                catch (SecurityTokenExpiredException)
                {
                    await GetToken();
                }
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.token);
                BasePath = "api/eVouchereAPI/GetePaymentMethodList";

                var response = await _client.GetAsync(BasePath);

                if (response.IsSuccessStatusCode == false)
                    throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

                var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                payList = System.Text.Json.JsonSerializer.Deserialize<List<PaymentInfo>>(dataAsString);
                
            }
            catch (Exception ex)
            {
                var test = ex.Message;
                
            }
            return payList;
        }

        public DataTable ToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
