namespace Core.DotNet.AggregatesModel.CommonAggregate;

public class Phone
{
    public PhoneType PhoneType { get; set; }
    public string PhoneCode { get; set; }
    public string PhoneNumber { get; set; }
}
public enum PhoneType
{
    Unknown,
    Mobile,
    Phone,
    Fax
}