using Amazon.Lambda.Core;
using System.Text;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string FunctionHandler(string input, ILambdaContext context)
    {
        // return input.ToUpper();
        context.Logger.LogInformation($"The Function Handler received: {input}");

        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        string curPayload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";
        
        var client = new HttpClient();
        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(curPayload, Encoding.UTF8, "application/json")
        };
    
        var res = client.Send(webRequest);
        using var reader = new StreamReader(res.Content.ReadAsStream());
            
        return reader.ReadToEnd();
    }
}
