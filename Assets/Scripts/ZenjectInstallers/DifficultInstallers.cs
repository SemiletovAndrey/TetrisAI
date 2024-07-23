using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DifficultInstallers : MonoInstaller
{
    [SerializeField] private GameDifficultyManager gameDifficultyManager;
    [SerializeField] private DifficultyData difficultyData;

    public override void InstallBindings()
    {
        Container
            .BindInstance<DifficultyData>(difficultyData)
            .AsSingle();

        Container
            .Bind<GameDifficultyManager>()
            .FromNew()
            .AsSingle();
    }
}
