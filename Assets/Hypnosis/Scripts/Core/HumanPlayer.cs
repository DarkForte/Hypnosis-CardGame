class HumanPlayer : Player
{
    public override void Play(GameController cellGrid)
    {
        cellGrid.GameState = new GameStateWaitingInput(cellGrid);
    }
}