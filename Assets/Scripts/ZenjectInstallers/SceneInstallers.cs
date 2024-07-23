using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class SceneInstallers : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<ScoreView>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container 
            .Bind<Piece>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<Board>()
            .FromComponentInHierarchy()
            .AsSingle();
        
        Container
            .Bind<Tilemap>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}
