namespace Core.Services.User.Interfaces
{
    public interface IUserService
    {
        int Level { get; set; }
        int RerollBonusAmount { get; set; }
        int CoinCurrencyAmount { get; set; }
    }
}