namespace Core.DotNet.AggregatesModel.CommonAggregate;

public class DeleteResponse : IServiceResponse
{
    public Guid Id { get; set; }
}