using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StandardUnitGenerator : MonoBehaviour, IUnitGenerator
{
    public GameObject basePrefab;
    public GameObject pawnPrefab;
    public Transform unitParent;

    public List<Unit> SpawnUnits(Dictionary<Vector2, Cell> cells)
    {
        foreach(Transform child in unitParent)
        {
            Destroy(child.gameObject);
        }

        List<Unit> ret = new List<Unit>();

        SpawnIndividualUnit(basePrefab, 0, new Vector2(2, 0), cells, ret);
        SpawnIndividualUnit(basePrefab, 1, new Vector2(2, 5), cells, ret);

        SpawnIndividualUnit(pawnPrefab, 0, new Vector2(1, 1), cells, ret);
        SpawnIndividualUnit(pawnPrefab, 0, new Vector2(3, 1), cells, ret);
        SpawnIndividualUnit(pawnPrefab, 1, new Vector2(1, 4), cells, ret);
        SpawnIndividualUnit(pawnPrefab, 1, new Vector2(3, 4), cells, ret);

        return ret;
    }

    Unit SpawnIndividualUnit(GameObject prefab, int player, Vector2 pos, Dictionary<Vector2, Cell> cellMap, List<Unit> ret)
    {
        GameObject nowUnit = Instantiate(prefab);
        nowUnit.transform.parent = unitParent;

        Unit unit = nowUnit.GetComponent<Unit>();
        unit.Cell = cellMap[pos];
        unit.Cell.OccupyingUnit = unit;
        unit.PlayerNumber = player;
        unit.transform.position = unit.Cell.transform.position;

        unit.Initialize();
        unit.InitializeHealthBar(player == 0);

        ret.Add(unit);

        return unit;
    }
}

