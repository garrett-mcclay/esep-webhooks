using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    /*
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string FunctionHandler(string input, ILambdaContext context)
    {
        return input.ToUpper();
    }
    */
    public async Task FunctionHandler(JObject input, ILambdaContext context) {
        var issueUrl = input["issue"]?["html_url"]?.ToString();
        // Send issue URL to Slack via the webhook
        if (issueUrl != null) {
            await PostToSlack(issueUrl);
        }
    }

    private async Task PostToSlack(string issueUrl) {
        var slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");
        using (var client = new HttpClient()) {
            var payload = new { text = $"New GitHub issue created: {issueUrl}" };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            await client.PostAsync(slackUrl, content);
        }
    }
}
