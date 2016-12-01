using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class BFSDestinationFinder
{
    public static List<Cell> FindCellsWithinSteps(Dictionary<Vector2, Cell> cellMap, Cell start, List<Vector2> movements, int steps, int playerNum, bool pierceFriend = false, bool pierceEnemy = false, bool includeTakenCell=false)
    {
        List<Cell> ret = new List<Cell>();

        Queue< KeyValuePair<Vector2, int> > q = new Queue< KeyValuePair<Vector2, int> >();
        HashSet<Vector2> used = new HashSet<Vector2>();

        q.Enqueue( new KeyValuePair<Vector2, int>( start.OffsetCoord, 0) );
        used.Add(start.OffsetCoord);

        while(q.Count > 0)
        {
            KeyValuePair<Vector2, int> nowPair = q.Dequeue();
            Vector2 nowCoord = nowPair.Key;
            int nowStep = nowPair.Value;
            if(nowCoord != start.OffsetCoord)
            {
                if(cellMap[nowCoord].IsTaken==false || includeTakenCell)
                    ret.Add(cellMap[nowCoord]);
            }

            foreach (var nowMove in movements)
            {
                Vector2 nextCoord = nowCoord + nowMove;
                if(cellMap.ContainsKey(nextCoord) && !used.Contains(nextCoord) && nowStep + cellMap[nextCoord].MovementCost <= steps)
                {
                    bool add = false;

                    if(cellMap[nextCoord].IsTaken==true)
                    {
                        Unit takingunit = cellMap[nextCoord].OccupyingUnit;
                        if(takingunit.PlayerNumber == playerNum && pierceFriend)
                        {
                            add = true;
                        }
                        else if(takingunit.PlayerNumber != playerNum && pierceEnemy)
                        {
                            add = true;
                        }
                    }
                    else
                    {
                        add = true;
                    }

                    if(add)
                    {
                        q.Enqueue(new KeyValuePair<Vector2, int>(nextCoord, nowStep + cellMap[nextCoord].MovementCost));
                        used.Add(nextCoord);
                    }
                }
            }
        }

        return ret;
    }
}
