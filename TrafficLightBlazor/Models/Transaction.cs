namespace TrafficLightBlazor.Models;

public record Transaction(DateOnly Date, string Type, string Description, double Amount);

public enum TransactionType
{
    DEPOSIT,
    WITHDRAW,
    FEE
}