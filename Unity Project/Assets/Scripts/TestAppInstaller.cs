using System;
using System.Collections.Generic;
using TestApp.Map;
using TestApp.Mesh;
using TestApp.Player;
using Zenject;

public class TestAppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<MapGenerator>().ToSinglePrefabResource("Map Generator");
        Container.Bind<MeshGenerator>().ToSinglePrefabResource("Map Generator");
        Container.Bind<Player>().ToSinglePrefabResource("Player");
        Container.Bind<Menu>().ToSinglePrefabResource("Canvas");


        Container.Bind<IInitializable>().ToSinglePrefabResource<MapGenerator>("Map Generator");
        Container.Bind<IInitializable>().ToSinglePrefabResource<Player>("Player");
        Container.Bind<IInitializable>().ToSinglePrefabResource<Menu>("Canvas");

        Container.Install<ExecutionOrderInstaller>(
            new List<Type>()
            {
                            typeof(MapGenerator),
                            typeof(Player),
                            typeof(Menu)
            });
    }
}
