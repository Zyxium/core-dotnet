using Core.DotNet.AggregatesModel.TitleAggregate;

namespace Core.DotNet.Extensions.AggregatesModel.TitleAggregate;

public static class TitleExtensions
{
    public static TitleValue ToTitleValue(this TitleLocale titleLocal)
    {
        return new TitleValue
        {
            Code = titleLocal.Code,
            DisplayName = titleLocal.DisplayName.ToString()
        };
    }
}