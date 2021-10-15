using Assets.Scripts.SERVER;
using Assets.Scripts.SERVER.Processors;

using hololensMultiplayer;

using Lidgren.Network;

using UnityEngine;
using Zenject;

public class DependencyInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<NetClient>().FromFactory<NetClientFactory>().AsSingle().NonLazy();
        Container.Bind<NetServer>().FromFactory<NetServerFactory>().AsSingle().NonLazy();        
        Container.Bind<DataManager>().AsSingle().NonLazy();

        Container.Bind<Server>().AsSingle().NonLazy();

        //Server processors
        Container.Bind<ServerPlayTransProcessor>().AsSingle().NonLazy();
        Container.Bind<ServerWelcomeProcessor>().AsSingle().NonLazy();
        Container.Bind<ServerDisconnectProcessor>().AsSingle().NonLazy();

        //Client processors
        Container.Bind<ClientPlayTransProcessor>().AsSingle().NonLazy();
        Container.Bind<ClientWelcomeProcessor>().AsSingle().NonLazy();
        Container.Bind<ClientDisconnectProcessor>().AsSingle().NonLazy();


        Container.BindFactory<string, NetworkPlayer, NetworkPlayer.Factory>().FromFactory<PrefabResourceFactory<NetworkPlayer>>();
    }

    class NetClientFactory : IFactory<NetClient>
    {
        public NetClient Create()
        {
            var config = new NetPeerConfiguration("HRM");
            var client = new NetClient(config);
            client.Start();
            return client;
        }
    }


    class NetServerFactory : IFactory<NetServer>
    {
        public NetServer Create()
        {
            var config = new NetPeerConfiguration("HRM") { Port = 12345 };
            return new NetServer(config);
        }
    }
}