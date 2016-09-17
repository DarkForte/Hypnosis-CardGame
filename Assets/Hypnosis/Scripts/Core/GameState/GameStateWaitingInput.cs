class GameStateWaitingInput : GameState
{
    public GameStateWaitingInput(GameController cellGrid) : base(cellGrid)
    {
    }

    public override void OnUnitClicked(Unit unit)
    {
        if(unit.PlayerNumber.Equals(_gameController.CurrentPlayerNumber))
            _gameController.GameState = new GameStateUnitSelected(_gameController, unit); 
    }
}
