using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Path Finder using Breadth first search
/// </summary>
class BFSPathFinder : IPathfinding
{
    public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, int>> edges, T originNode, T destinationNode)
    {
        Queue<T> frontier = new Queue<T>();
        frontier.Enqueue(originNode);

        Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        cameFrom.Add(originNode, default(T));

        while(frontier.Count!=0)
        {
            var current = frontier.Dequeue();
            if (current.Equals(destinationNode)) break;

            var neighbours = GetNeigbours(edges, current);
            foreach(var neighbour in neighbours)
            {
                if(!cameFrom.ContainsKey(neighbour))
                {
                    cameFrom.Add(neighbour, current);
                    frontier.Enqueue(neighbour);
                }
            }       
        }

        List<T> path = new List<T>();
        if (!cameFrom.ContainsKey(destinationNode))
            return path;

        path.Add(destinationNode);
        var temp = destinationNode;
        
        while(!cameFrom[temp].Equals(originNode))
        {
            var currentPathElement = cameFrom[temp];
            path.Add(currentPathElement);

            temp = currentPathElement;
        }

        return path;

    }

    public static List<Cell> FindCellPath(Dictionary<Vector2, Cell> cellMap, List<Vector2> moves, Vector2 from, Vector2 to, bool pierceFriend, bool pierceEnemy, int ID)
    {
        List<Cell> ret = new List<Cell>();
        Queue<Vector2> q = new Queue<Vector2>();
        HashSet<Vector2> used = new HashSet<Vector2>();
        Dictionary<Vector2, Vector2> comeFrom = new Dictionary<Vector2, Vector2>();

        q.Enqueue(from);
        used.Add(from);
        while(q.Count > 0)
        {
            Vector2 nowPoint = q.Dequeue();

            if (nowPoint == to)
                break;

            foreach(var move in moves)
            {
                Vector2 nextPoint = nowPoint + move;
                if(cellMap.ContainsKey(nextPoint) && !used.Contains(nextPoint))
                {
                    bool ok = false;
                    Cell nextCell = cellMap[nextPoint];
                    if (nextCell.IsTaken == false)
                        ok = true;
                    else if (nextCell.OccupyingUnit.PlayerNumber == ID && pierceFriend)
                        ok = true;
                    else if (nextCell.OccupyingUnit.PlayerNumber != ID && pierceEnemy)
                        ok = true;

                    if(ok)
                    {
                        q.Enqueue(nextPoint);
                        used.Add(nextPoint);
                        comeFrom[nextPoint] = nowPoint;
                    }
                }
            }
        }

        Vector2 now = to;
        while(now!= from)
        {
            ret.Add(cellMap[now]);
            now = comeFrom[now];
        }
        ret.Add(cellMap[from]);

        return ret;
    }
}

