using UnityEngine;

class GameStateWaitingInput : GameState
{
    CardType NowAction;

    public GameStateWaitingInput(GameController gameController, CardType nowAction) : base(gameController)
    {
        NowAction = nowAction;
        Debug.Log(nowAction);
    }

    public override void OnUnitClicked(Unit unit)
    {
        if (!CardHelper.isBasic(NowAction))
            return;

        if(unit.PlayerNumber.Equals(_gameController.CurrentPlayerNumber))
            _gameController.GameState = new GameStateUnitSelected(_gameController, unit, NowAction); 
    }

    public override void OnCellClicked(Cell cell)
    {
        if (CardHelper.isBasic(NowAction))
            return;


    }
} 
