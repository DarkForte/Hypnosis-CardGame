using UnityEngine;

class GameStateWaitingInput : GameState
{
    CardType NowAction;

    public GameStateWaitingInput(GameController cellGrid, CardType nowAction) : base(cellGrid)
    {
        NowAction = nowAction;
        Debug.Log(nowAction);
    }

    public override void OnUnitClicked(Unit unit)
    {
        if(unit.PlayerNumber.Equals(_gameController.CurrentPlayerNumber))
            _gameController.GameState = new GameStateUnitSelected(_gameController, unit, NowAction); 
    }
}
