﻿using System.Text;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;

namespace MagicVilla_Web.Services
{
	public class BaseService : IBaseService
	{
		public APIResponse responseModel { get; set; }
		public IHttpClientFactory httpCLient { get; set; }
		public BaseService(IHttpClientFactory httpCLient)
		{
			this.responseModel = new();
			this.httpCLient = httpCLient;
		}
		public async Task<T> SendAsync<T>(APIRequest apiRequest)
		{
			try
			{
				var client = httpCLient.CreateClient("MagicAPI");
				HttpRequestMessage message = new HttpRequestMessage();
				message.Headers.Add("Accept", "application/json");
				message.RequestUri = new Uri(apiRequest.Url);
				if (apiRequest.Data != null)
				{
					message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
						Encoding.UTF8, "application/json");
				}
				switch (apiRequest.ApiType)
				{
					case SD.ApiType.POST:
						message.Method = HttpMethod.Post;
						break;
					case SD.ApiType.PUT:
						message.Method = HttpMethod.Put;
						break;
					case SD.ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}
				HttpResponseMessage apiResponse = null;
				apiResponse = await client.SendAsync(message);



				var apiContent = await apiResponse.Content.ReadAsStringAsync();
				try
				{
                    APIResponse ApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
					if(apiResponse.StatusCode==System.Net.HttpStatusCode.BadRequest 
						|| apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
					{
                        ApiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        ApiResponse.IsSuccess = false;

                        var res = JsonConvert.SerializeObject(ApiResponse);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
				catch (Exception e)
				{
                    var ExceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
					return ExceptionResponse;
                }
                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
				return APIResponse;
            }
			catch(Exception e)
			{
				var dto = new APIResponse
				{
					ErrorMessages = new List<string> { Convert.ToString(e.Message) },
					IsSuccess = false
				};
				var res = JsonConvert.SerializeObject(dto);
				var APIResponse = JsonConvert.DeserializeObject<T>(res);
				return APIResponse;
			}
		}
	}
}
