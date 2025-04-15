using UnityEngine;

public enum GameStateID
{
    Home,
    Progress,
    Final,
}
public class GameStateManager : ASingleton<GameStateManager>
{
    public GameStateID CurrentState;
    [SerializeField] private GameObject _menu;
    [SerializeField] private Joystick _control;
    [SerializeField] private GameObject _confetti;
    public void SetState(GameStateID newState)
    {
        CurrentState = newState;
        switch (newState)
        {
            case GameStateID.Home:
                {
                    _menu.SetActive(true);
                    _control.gameObject.SetActive(false);
                    break;
                }
            case GameStateID.Progress:
                {
                    _menu.SetActive(false);
                    _control.gameObject.SetActive(true);
                    break;
                }
            case GameStateID.Final:
                {
                    _control.OnPointerUp(null);
                    _control.gameObject.SetActive(false);
                    _confetti.SetActive(true);
                    StartCoroutine(Common.Delay(8.0f, () =>
                    {
                        GameManager.Instance.ClearLevel();
                        _confetti.SetActive(false);
                        _menu.SetActive(true);
                    }));
                    break;
                }
        }
    }

}
