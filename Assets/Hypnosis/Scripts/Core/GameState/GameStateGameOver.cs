public class GameStateGameOver : GameState
{
    public GameStateGameOver(GameController gameController) : base(gameController)
    {
    }

    public override void OnStateEnter()
    {
        _gameController.uiController.HideInGameUI();
        _gameController.uiController.ShowConnectUI();
    }
}