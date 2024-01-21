namespace Core.DotNet.AggregatesModel.ExceptionAggregate;

public class ErrorRow
{
    public int Row { get; set; }
    public string Error { get; set; }

    public ErrorRow(int row)
    {
        Row = row;
    }

    public ErrorRow(int row, string error) : this(row)
    {
        Error = error;
    }
}