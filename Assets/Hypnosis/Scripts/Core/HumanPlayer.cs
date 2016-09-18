class HumanPlayer : Player
{
    public override void Play(GameController gamecontroller)
    {
        gamecontroller.GameState = new GameStateWaitingInput(gamecontroller);
    }
}