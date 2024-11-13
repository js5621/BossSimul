using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterSequence : MonoBehaviour
{
    private async UniTaskVoid Start()
    {
        var boss = FindAnyObjectByType<BossMovement>();
        var player = FindAnyObjectByType<PlayerScript>();

        player.LockMovement();
        await boss.PlayEncounterEffectAsync();
        player.UnlockMovement();
    }

   async void update()
    {
        var player = FindAnyObjectByType<PlayerScript>();
        var boss = FindAnyObjectByType<BossMovement>();
        Debug.Log("중재테스트");


    }

}
