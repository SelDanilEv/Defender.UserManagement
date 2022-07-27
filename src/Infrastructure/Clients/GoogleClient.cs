﻿using Defender.UserManagement.Application.Common.Exceptions;
using Defender.UserManagement.Application.Models.Google;
using Defender.UserManagement.Domain.Entities.User;
using Defender.UserManagement.Infrastructure.Clients.Interfaces;
using Newtonsoft.Json;

namespace Defender.UserManagement.Infrastructure.Clients;

public partial class GoogleClient : IGoogleClient
{
    private HttpClient _httpClient;

    public GoogleClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GoogleUser> GetTokenInfo(string token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        var url = $"oauth2/v1/userinfo?alt=json&access_token={token}";

        var client = _httpClient;
        try
        {
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await client.SendAsync(request);

                if ((int)response.StatusCode == 200)
                {
                    var responseText = await response.Content.ReadAsStringAsync();

                    try
                    {
                        return JsonConvert.DeserializeObject<GoogleUser>(responseText);
                    }
                    catch (JsonSerializationException exception)
                    {
                        var message = "Could not deserialize the response body string as " + typeof(User).FullName + ".";
                        throw new InvalidCastException(message);
                    }
                }

                throw new GoogleClientException();
            }
        }
        finally
        {
            client.Dispose();
        }
    }

}
