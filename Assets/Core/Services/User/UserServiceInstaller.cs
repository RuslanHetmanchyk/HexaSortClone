using Core.Services.SaveLoad.Implementation;
using Core.Services.SaveLoad.Interfaces;
using Core.Services.User.Implementation;
using Core.Services.User.Interfaces;
using Zenject;

namespace Core.Services.User
{
    public class UserServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IUserService>()
                .To<UserService>()
                .FromNew()
                .AsSingle();
        }
    }
}