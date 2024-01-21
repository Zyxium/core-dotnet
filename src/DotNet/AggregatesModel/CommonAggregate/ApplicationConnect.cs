namespace Core.DotNet.AggregatesModel.CommonAggregate;

public enum ApplicationConnect
{
    Admin,
    InHouse,
    Partner,
    Customer
}

public static class EnumApplicationConnectExtension
{
    public static string GetName(this ApplicationConnect enumTarget) => enumTarget.ToString();
}