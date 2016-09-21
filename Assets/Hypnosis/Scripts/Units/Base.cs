using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Base : GenericUnit
{
    public override List<Cell> GetAvailableDestinations(Dictionary<Vector2, Cell> cellMap)
    {
        return new List<Cell>(); // Base cannot move
    }
}

