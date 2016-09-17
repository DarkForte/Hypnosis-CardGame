public class CellGridStateAiTurn : GameState
{
    public CellGridStateAiTurn(GameController cellGrid) : base(cellGrid)
    {      
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        foreach (var cell in _gameController.Cells)
        {
            cell.UnMark();
        }
    }
}