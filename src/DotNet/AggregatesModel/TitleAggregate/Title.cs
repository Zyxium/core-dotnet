using Core.DotNet.AggregatesModel.CommonAggregate;

namespace Core.DotNet.AggregatesModel.TitleAggregate;

public class TitleLocale
{
    public string Code { get; set; }
    public Locale DisplayName { get; set; }
}

public class TitleCode
{
    public string Code { get; set; }
}

public class TitleValue
{
    public string Code { get; set; }
    public string DisplayName { get; set; }
}