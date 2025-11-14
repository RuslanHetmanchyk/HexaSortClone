namespace Core.Services.User.Interfaces
{
    public interface IUserService
    {
        int Level { get; }
        int RerollBonusAmount { get; set; }
        int CoinCurrencyAmount { get; set; }
    }
}