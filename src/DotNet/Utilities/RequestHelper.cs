using Core.DotNet.AggregatesModel.CommonAggregate;

namespace Core.DotNet.Utilities;

public class RequestHelper
{
    public static bool IsLanguageThai()
    {
        return Thread.CurrentThread.CurrentUICulture.Name.ToLower() == LanguageCode.TH ||
               Thread.CurrentThread.CurrentUICulture.Name.ToLower() == "th-th";
    }
}