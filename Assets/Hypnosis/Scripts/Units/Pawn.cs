using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Pawn : GenericUnit
{
    public override List<Cell> GetAvailableDestinations(Dictionary<Vector2, Cell> cellMap)
    {
        List<Cell> ret = new List<Cell>();
        Vector2[] direction = { new Vector2(0, 1), new Vector2(-1, 1), new Vector2(1, 1) };
        foreach(Vector2 dir in direction)
        {
            Vector2 realDir;
            if (PlayerNumber == 1)
                realDir = dir * -1;
            else
                realDir = dir;

            Vector2 nowCoord = Cell.OffsetCoord + realDir;

            if (cellMap.ContainsKey(nowCoord) && cellMap[nowCoord].IsTaken == false)
                ret.Add(cellMap[nowCoord]);
        }
        return ret;
    }
}