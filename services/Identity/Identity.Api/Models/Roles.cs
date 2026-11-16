namespace Identity.Api.Models;

public static class Roles
{
    public const string Seller = "Seller";
    public const string Broker = "Broker";
    public const string Buyer = "Buyer";

    public static readonly string[] All = [Seller, Broker, Buyer];
}
