using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace PdfInvoice
{

    public static class OrdService
    {

        public static async Task<TrxnDocV> GetDocVByDN(HttpClient http, string docNum)
        {
            var response = await http.GetAsync($"https://wcabr.app/API/GetDocV/ByDN/{docNum}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return default;
                }
                return await response.Content.ReadFromJsonAsync<TrxnDocV>();
            }
            // Refine exception handling
            var message = await response.Content.ReadAsStringAsync();
            throw new Exception($"Http Status code: {response.StatusCode} Message: {message}");
        }


    }

}