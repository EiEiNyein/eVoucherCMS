using eVoucherCMS.Models;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace eVoucherCMS.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task<string> ReadContentAsync<T>(this HttpResponseMessage response)
        {
           // var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.token);

            if (response.IsSuccessStatusCode == false)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //var result = JsonSerializer.Deserialize<string>(
            //    dataAsString, new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    });

            return dataAsString;
        }

        public static async Task<DataTable> ReadContentListAsync<T>(this HttpResponseMessage response)
        {
            // var client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.token);
            DataTable dt = new();
            if (response.IsSuccessStatusCode == false)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<List<T>>(
                dataAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            // dt = (DataTable)JsonSerializer.Deserialize(dataAsString, (typeof(DataTable)));
            //var result = JsonSerializer.Deserialize<List<object>>(dataAsString);
            // List<object> list = result;
            //  dt = ToDataTable(list);
            foreach (var data in result)
            {
                dt.Rows.Add(data);
            }
            return dt;
            //return dt;
        }


        public static async Task<T> PostContentAsync<T>(object requestInfo)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.token);
            var data = JsonSerializer.Serialize(requestInfo);
            HttpContent searchContent = new StringContent(data);
            searchContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Constants.urlAddress + "/api/eVoucher/SaveEvoucher/", searchContent);

            if (response.IsSuccessStatusCode == false)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false); 
            var result = JsonSerializer.Deserialize<T>(
                dataAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return result;
        }


        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(data);
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