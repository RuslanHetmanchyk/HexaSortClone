using Core.Services.User.Interfaces;

namespace Core.Services.User.Implementation
{
    public class UserService : IUserService
    {
        public int Level { get; }
        public int RerollBonusAmount { get; set; }
        public int CoinCurrencyAmount { get; set; }

        public UserService()
        {
            Level = 1;
            RerollBonusAmount = 67;
            CoinCurrencyAmount = 100;
        }
    }
}