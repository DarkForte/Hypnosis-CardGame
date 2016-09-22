using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// Base class for all units in the game.
/// </summary>
public abstract class Unit : MonoBehaviour
{
    /// <summary>
    /// UnitClicked event is invoked when user clicks the unit. It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitClicked;
    /// <summary>
    /// UnitSelected event is invoked when user clicks on unit that belongs to him. It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitSelected;
    public event EventHandler UnitDeselected;
    /// <summary>
    /// UnitHighlighted event is invoked when user moves cursor over the unit. It requires a collider on the unit game object to work.
    /// </summary>
    public event EventHandler UnitHighlighted;
    public event EventHandler UnitDehighlighted;
    public event EventHandler<AttackEventArgs> UnitAttacked;
    public event EventHandler<AttackEventArgs> UnitDestroyed;
    public event EventHandler<MovementEventArgs> UnitMoved;

    public UnitState UnitState { get; set; }
    public void SetState(UnitState state)
    {
        UnitState.MakeTransition(state);
    }

    public List<Buff> Buffs { get; private set; }

    public int MaxHP { get; private set; }

    /// <summary>
    /// Cell that the unit is currently occupying.
    /// </summary>
    public Cell Cell { get; set; }

    public int HP { get; protected set; }
    public int DefenceFactor;

    public List<Vector2> Moves { get; protected set; }
    public int Steps;
    public List<Vector2> AttackMoves { get; protected set; }
    public int AttackRange;
    public int AttackPower;


    /// <summary>
    /// Determines speed of movement animation.
    /// </summary>
    public float MovementSpeed;


    /// <summary>
    /// Indicates the player that the unit belongs to. Should correspoond with PlayerNumber variable on Player script.
    /// </summary>
    public int PlayerNumber;

    /// <summary>
    /// Indicates if movement animation is playing.
    /// </summary>
    public bool isMoving { get; set; }

    private static IPathfinding _pathfinder = new BFSPathFinder();

    /// <summary>
    /// Method called after object instantiation to initialize fields etc. 
    /// </summary>
    public virtual void Initialize()
    {
        Buffs = new List<Buff>();
        UnitState = new UnitStateNormal(this);
        HP = MaxHP;

        Moves = new List<Vector2>();

        InitializeMoveAndAttack();
    }
    public abstract void InitializeMoveAndAttack();


    protected virtual void OnMouseDown()
    {
        if (UnitClicked != null)
            UnitClicked.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
            UnitHighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
            UnitDehighlighted.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method is called at the start of each turn.
    /// </summary>
    public virtual void OnTurnStart()
    {
        SetState(new UnitStateMarkedAsFriendly(this));
    }
    /// <summary>
    /// Method is called at the end of each turn.
    /// </summary>
    public virtual void OnTurnEnd()
    {
        Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(this); });
        Buffs.RemoveAll(b => b.Duration == 0);
        Buffs.ForEach(b => { b.Duration--; });

        SetState(new UnitStateNormal(this));
    }
    /// <summary>
    /// Method is called when units HP drops below 1.
    /// </summary>
    protected virtual void OnDestroyed()
    {
        Cell.IsTaken = false;
        MarkAsDestroyed();
        Destroy(gameObject);
    }

    /// <summary>
    /// Method is called when unit is selected.
    /// </summary>
    public virtual void OnUnitSelected()
    {
        SetState(new UnitStateMarkedAsSelected(this));
        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
    }
    /// <summary>
    /// Method is called when unit is deselected.
    /// </summary>
    public virtual void OnUnitDeselected()
    {
        SetState(new UnitStateMarkedAsFriendly(this));
        if (UnitDeselected != null)
            UnitDeselected.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method indicates if it is possible to attack unit given as parameter, from cell given as second parameter.
    /// </summary>
    public virtual bool IsUnitAttackable(Unit other, Cell sourceCell)
    {
        if (sourceCell.GetDistance(other.Cell) <= AttackRange)
            return true;

        return false;
    }

    public virtual List<Unit> GetEnemiesInRange(List<Unit> units)
    {
        List<Unit> ret = new List<Unit>();
        List<Unit> enemies = units.FindAll(u => u.PlayerNumber != PlayerNumber);

        foreach (var currentUnit in enemies)
        {
            if (Cell.GetDistance(currentUnit.Cell) <= AttackRange)
            {
                ret.Add(currentUnit);
            }
        }
        return ret;
    }

    /// <summary>
    /// Method deals damage to unit given as parameter.
    /// </summary>
    public virtual void DealDamage(Unit other)
    {
        if (isMoving)
            return;

        if (!IsUnitAttackable(other, Cell))
            return;

        MarkAsAttacking(other);
        other.Defend(this, AttackPower);

    }
    /// <summary>
    /// Attacking unit calls Defend method on defending unit. 
    /// </summary>
    protected virtual void Defend(Unit other, int damage)
    {
        MarkAsDefending(other);
        HP -= Mathf.Clamp(damage - DefenceFactor, 1, damage);  //Damage is calculated by subtracting attack factor of attacker and defence factor of defender. If result is below 1, it is set to 1.
                                                               //This behaviour can be overridden in derived classes.
        if (UnitAttacked != null)
            UnitAttacked.Invoke(this, new AttackEventArgs(other, this, damage));

        if (HP <= 0)
        {
            if (UnitDestroyed != null)
                UnitDestroyed.Invoke(this, new AttackEventArgs(other, this, damage));
            OnDestroyed();
        }
    }

    public virtual void Move(Cell destinationCell, List<Cell> path)
    {
        if (isMoving)
            return;

        var totalMovementCost = path.Sum(h => h.MovementCost);

        Cell.IsTaken = false;
        Cell = destinationCell;
        destinationCell.IsTaken = true;

        if (MovementSpeed > 0)
            StartCoroutine(MovementAnimation(path));
        else
            transform.position = Cell.transform.position;

        if (UnitMoved != null)
            UnitMoved.Invoke(this, new MovementEventArgs(Cell, destinationCell, path));
    }
    protected virtual IEnumerator MovementAnimation(List<Cell> path)
    {
        isMoving = true;

        path.Reverse();
        foreach (var cell in path)
        {
            while (new Vector2(transform.position.x, transform.position.y) != new Vector2(cell.transform.position.x, cell.transform.position.y))
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(cell.transform.position.x, cell.transform.position.y, transform.position.z), Time.deltaTime * MovementSpeed);
                yield return 0;
            }
        }

        isMoving = false;
    }

    ///<summary>
    /// Method indicates if unit is capable of moving to cell given as parameter.
    /// </summary>
    public virtual bool IsCellMovableTo(Cell cell)
    {
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method indicates if unit is capable of moving through cell given as parameter.
    /// </summary>
    public virtual bool IsCellTraversable(Cell cell)
    {
        return !cell.IsTaken;
    }
    /// <summary>
    /// Method returns all cells that the unit is capable of moving to.
    /// </summary>
    public virtual List<Cell> GetAvailableDestinations(Dictionary<Vector2, Cell> cellMap)
    {
        return BFSDestinationFinder.FindCellsWithinSteps(cellMap, Cell, Moves, Steps);
    }

    public List<Cell> FindPath(Dictionary<Vector2, Cell> cellMap, Cell destination)
    {
        return _pathfinder.FindPath(GetGraphEdges(cellMap, Moves), Cell, destination);
    }
    /// <summary>
    /// Method returns graph representation of cell grid for pathfinding.
    /// </summary>
    protected Dictionary<Cell, Dictionary<Cell, int>> GetGraphEdges(Dictionary<Vector2, Cell> cellMap, List<Vector2> movement)
    {
        Dictionary<Cell, Dictionary<Cell, int>> ret = new Dictionary<Cell, Dictionary<Cell, int>>();
        foreach (var cell in cellMap.Values)
        {
            Vector2 offset = cell.OffsetCoord;
            ret[cell] = new Dictionary<Cell, int>();
            foreach (var nowDirection in Moves)
            {
                Vector2 nowCoord = offset + nowDirection;
                if (cellMap.ContainsKey(nowCoord) && cellMap[nowCoord].IsTaken == false)
                {
                    Cell targetCell = cellMap[nowCoord];
                    ret[cell][targetCell] = targetCell.MovementCost;
                }
            }
        }
        return ret;
    }
    /// <summary>
    /// Gives visual indication that the unit is under attack.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsDefending(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is attacking.
    /// </summary>
    /// <param name="other"></param>
    public abstract void MarkAsAttacking(Unit other);
    /// <summary>
    /// Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
    /// destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    /// </summary>
    public abstract void MarkAsDestroyed();

    /// <summary>
    /// Method marks unit as current players unit.
    /// </summary>
    public abstract void MarkAsFriendly();
    /// <summary>
    /// Method mark units to indicate user that the unit is in range and can be attacked.
    /// </summary>
    public abstract void MarkAsReachableEnemy();
    /// <summary>
    /// Method marks unit as currently selected, to distinguish it from other units.
    /// </summary>
    public abstract void MarkAsSelected();
    /// <summary>
    /// Method marks unit to indicate user that he can't do anything more with it this turn.
    /// </summary>
    public abstract void MarkAsFinished();
    /// <summary>
    /// Method returns the unit to its base appearance
    /// </summary>
    public abstract void UnMark();
}

public class MovementEventArgs : EventArgs
{
    public Cell OriginCell;
    public Cell DestinationCell;
    public List<Cell> Path;

    public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path)
    {
        OriginCell = sourceCell;
        DestinationCell = destinationCell;
        Path = path;
    }
}
public class AttackEventArgs : EventArgs
{
    public Unit Attacker;
    public Unit Defender;

    public int Damage;

    public AttackEventArgs(Unit attacker, Unit defender, int damage)
    {
        Attacker = attacker;
        Defender = defender;

        Damage = damage;
    }
}

public class CommonMovement
{
    public static List<Vector2> dir4 = new List<Vector2>();
    public static List<Vector2> dir8 = new List<Vector2>();

    static CommonMovement()
    {
        Vector2[] d4 = { new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(-1, 0) };
        Vector2[] d8 = { new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1) };

        foreach (var d in d4)
        {
            dir4.Add(d);
            dir8.Add(d);
        }
        foreach (var d in d8)
        {
            dir8.Add(d);
        }
    }
}