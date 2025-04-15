using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
public enum GameMode
{
    None,
    OneVsOne,
    OneVsTwo,
    TwoVsTwo
}

public class GameManager : ASingleton<GameManager>
{
    public GameMode Mode;
    [SerializeField] private PlayerController _controller;
    [SerializeField] private CinemachineTargetGroup _group;
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform _playerSpawnPosLeft;
    [SerializeField] private Transform _playerSpawnPosRight;
    [SerializeField] private Transform _playerSpawnPosMiddle;
    [SerializeField] private Transform _enemySpawnPosLeft;
    [SerializeField] private Transform _enemySpawnPosRight;
    [SerializeField] private Transform _enemySpawnPosMiddle;
    public List<Player> CurrentPlayer;
    public List<Enemy> CurrentEnemy;

    public void CheckComplete()
    {
        if (CurrentPlayer.All(player => player.HP <= 0) || CurrentEnemy.All(enemy => enemy.HP <= 0))
        {
            GameStateManager.Instance.SetState(GameStateID.Final);
        }
    }

    public void ClearLevel()
    {
        CurrentPlayer.ForEach(player => Destroy(player.gameObject));
        CurrentEnemy.ForEach(enemy => Destroy(enemy.gameObject));
        CurrentPlayer.Clear();
        CurrentEnemy.Clear();
        Mode = GameMode.None;
    }
    public void Start1V1()
    {
        Mode = GameMode.OneVsOne;
        GameStateManager.Instance.SetState(GameStateID.Progress);
        Player player = Instantiate(_playerPrefab, _playerSpawnPosMiddle.position, Quaternion.Euler(new Vector3(0, 315, 0)));
        Enemy enemy = Instantiate(_enemyPrefab, _enemySpawnPosMiddle.position, Quaternion.Euler(new Vector3(0, 135, 0)));
        enemy.PotentialTargets = new List<Character>() { player };
        _group.m_Targets = new CinemachineTargetGroup.Target[]{
            new CinemachineTargetGroup.Target()
            {
             target = player.transform,
             weight = 1,
             radius = 0
            }
        };
        CurrentPlayer = new List<Player>() { player };
        CurrentEnemy = new List<Enemy>() { enemy };
        _controller.SetUp(CurrentPlayer);
    }
    public void Start1V2()
    {
        Mode = GameMode.OneVsTwo;
        GameStateManager.Instance.SetState(GameStateID.Progress);
        Player player = Instantiate(_playerPrefab, _playerSpawnPosMiddle.position, Quaternion.Euler(new Vector3(0, 315, 0)));
        Enemy enemy1 = Instantiate(_enemyPrefab, _enemySpawnPosLeft.position, Quaternion.Euler(new Vector3(0, 135, 0)));
        Enemy enemy2 = Instantiate(_enemyPrefab, _enemySpawnPosRight.position, Quaternion.Euler(new Vector3(0, 135, 0)));

        enemy1.PotentialTargets = new List<Character>() { player };
        enemy2.PotentialTargets = new List<Character>() { player };

        _group.m_Targets = new CinemachineTargetGroup.Target[]{
            new CinemachineTargetGroup.Target()
            {
             target = player.transform,
             weight = 1,
             radius = 0
            }
        };

        CurrentPlayer = new List<Player>() { player };
        CurrentEnemy = new List<Enemy>() { enemy1, enemy2 };
        _controller.SetUp(CurrentPlayer);
    }
    public void Start2V2()
    {
        Mode = GameMode.TwoVsTwo;
        GameStateManager.Instance.SetState(GameStateID.Progress);
        Player player1 = Instantiate(_playerPrefab, _playerSpawnPosLeft.position, Quaternion.Euler(new Vector3(0, 315, 0)));
        Player player2 = Instantiate(_playerPrefab, _playerSpawnPosRight.position, Quaternion.Euler(new Vector3(0, 315, 0)));

        Enemy enemy1 = Instantiate(_enemyPrefab, _enemySpawnPosLeft.position, Quaternion.Euler(new Vector3(0, 135, 0)));
        Enemy enemy2 = Instantiate(_enemyPrefab, _enemySpawnPosRight.position, Quaternion.Euler(new Vector3(0, 135, 0)));

        enemy1.PotentialTargets = new List<Character>() { player1, player2 };
        enemy2.PotentialTargets = new List<Character>() { player1, player2 };

        _group.m_Targets = new CinemachineTargetGroup.Target[]{
            new CinemachineTargetGroup.Target()
            {
             target = player1.transform,
             weight = 1,
             radius = 0
            },
             new CinemachineTargetGroup.Target()
            {
             target = player2.transform,
             weight = 1,
             radius = 0
            }
        };

        CurrentPlayer = new List<Player>() { player1, player2 };
        CurrentEnemy = new List<Enemy>() { enemy1, enemy2 };
        _controller.SetUp(CurrentPlayer);
    }
}
