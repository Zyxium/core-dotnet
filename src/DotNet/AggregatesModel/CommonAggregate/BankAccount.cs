namespace Core.DotNet.AggregatesModel.CommonAggregate;

public class BankAccount
{
    public string BankCode { get; set; }
    public string AccountType { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string AccountBranch { get; set; }
    public bool IsDefault { get; set; }
}

public enum BankAccountType
{
    SavingsAccount,
    FixedDepositAccount,
    CurrentAccount
}