using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class GameStateWaitingInput : GameState
{
    CardType NowAction;
    List<Cell> availableSummonCells = new List<Cell>();

    public override void OnStateEnter()
    {
        if(!CardHelper.isBasic(NowAction))
        {
            availableSummonCells = GetSummonCells(_gameController.CurrentPlayerNumber);
        }
        availableSummonCells.ForEach(cell => cell.MarkAsReachable());
    }

    public override void OnStateExit()
    {
        availableSummonCells.ForEach(cell => cell.UnMark());
    }

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
        {
            if(NowAction == CardType.SPECIAL)
            {
                unit.SpecialMove(_gameController);
            }
            else
                _gameController.GameState = new GameStateUnitSelected(_gameController, unit, NowAction);

        }
    }

    //Deal with summon
    public override void OnCellClicked(Cell cell)
    {
        if (CardHelper.isBasic(NowAction))
            return;
        if (!availableSummonCells.Contains(cell))
            return;

        _gameController.SummonPrefab(NowAction, cell);
        _gameController.EndTurn();
    }

    public override void OnCellSelected(Cell cell)
    {
        base.OnCellSelected(cell);
        if(availableSummonCells.Contains(cell))
        {
            cell.MarkAsPath();
        }
    }

    public override void OnCellDeselected(Cell cell)
    {
        base.OnCellDeselected(cell);
        if(availableSummonCells.Contains(cell))
        {
            cell.MarkAsReachable();
        }
    }

    private List<Cell> GetSummonCells(int playerNumber)
    {
        List<Cell> ret = new List<Cell>();

        Vector2 start = _gameController.Units.Find(unit => unit.UnitName == "Base" && unit.PlayerNumber == playerNumber).Cell.OffsetCoord;
        HashSet<Vector2> used = new HashSet<Vector2>();
        Queue<Vector2> q = new Queue<Vector2>();
        Dictionary<Vector2, Cell> cellMap = _gameController.CellMap;
        used.Add(start);
        q.Enqueue(start);

        while(q.Count>0)
        {
            Vector2 nowCoord = q.Dequeue();
            foreach(var d in CommonMovement.dir4)
            {
                Vector2 nextCoord = nowCoord + d;
                if(cellMap.ContainsKey(nextCoord) && !used.Contains(nextCoord))
                {
                    Cell nextCell = cellMap[nextCoord];
                    if(nextCell.IsTaken==true && nextCell.OccupyingUnit.PlayerNumber == playerNumber) //Friend unit
                    {
                        q.Enqueue(nextCoord);
                    }
                    else if(nextCell.IsTaken == false)
                    {
                        ret.Add(nextCell);
                    }
                    used.Add(nextCoord);
                }
            }
        }

        return ret;
    }

} 
