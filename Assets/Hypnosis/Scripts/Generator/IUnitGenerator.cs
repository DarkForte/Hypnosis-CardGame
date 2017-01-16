using System.Collections.Generic;
using UnityEngine;

public interface IUnitGenerator
{
     List<Unit> SpawnUnits(Dictionary<Vector2, Cell> cells);
}

