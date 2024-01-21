using Google.Apis.Auth.OAuth2;

namespace Core.DotNet.Utilities.Auth;

public static class CredentialHelper
{
    public static GoogleCredential CreateGoogleCredential(string jsonSingleLine)
    {
        var credentialString = jsonSingleLine
            .Replace("<double-quote>", "\"")
            .Replace("<comma>", ",")
            .Replace("<newline>", "\n");

        return GoogleCredential.FromJson(credentialString);
    }
}