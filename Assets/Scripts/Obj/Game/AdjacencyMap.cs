using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
ClassName:      AdjacencyMap
Author:         Brad Baker
Creation Date:  10052019
Inherits:       Object
Abstract:       No
Brief:          Contains all logic for adjacency mapping of GameObjects. Can be used for paths or map-level tracking of adjacency.
Constructor:    None
*/

public class AdjacencyMap
{
    /* 
        Maintain a list of adjacent vertices for each vertex. 
    */
    private Dictionary<GameObject, List<GameObject>> adjacencyMap;
    private List<GameObject> vertexEntities;

    // Constructor for AdjacencyMap
    //  Create list, map, initialize null list.
    public AdjacencyMap()
    {
        vertexEntities = new List<GameObject>();
        adjacencyMap = new Dictionary<GameObject, List<GameObject>>();
    }

    // Update adjacency 
    private void UpdateAdjacency(GameObject vertex1, GameObject vertex2)
    {
        HexEntity vertexEnt1 = vertex1.GetComponent<HexEntity>();
        HexEntity vertexEnt2 = vertex2.GetComponent<HexEntity>();
        if (vertexEnt1.Adjacent(vertexEnt2))
        {
            if (!adjacencyMap.ContainsKey(vertex1))
                adjacencyMap[vertex1] = new List<GameObject>();
            if (!adjacencyMap.ContainsKey(vertex2))
                adjacencyMap[vertex2] = new List<GameObject>();
            adjacencyMap[vertex1].Add(vertex2);
            adjacencyMap[vertex2].Add(vertex1);
        }
    }

    // Update adjacency matrix with existing vertexes
    private void UpdateAdjacency(GameObject vertex1)
    {
        foreach (GameObject vertex2 in vertexEntities)
        {
            if (vertex1 == vertex2)
                continue;
            UpdateAdjacency(vertex1, vertex2);
        }
    }

    // Update all entries in adjacency matrix (AVOID USING)
    private void UpdateAllAdjacency()
    {
        foreach (GameObject vertex1 in vertexEntities)
        {
            UpdateAdjacency(vertex1);
        }
    }

    // Check if adjacent
    public bool Adjacent(GameObject vertex1, GameObject vertex2)
    {
        bool has1 = adjacencyMap.ContainsKey(vertex1);
        bool has2 = adjacencyMap.ContainsKey(vertex2);
        if (!(has1 && has2))
            return false;
        return adjacencyMap[vertex1].Contains(vertex2);
    }

    // Add a vertex to the graph
    public void AddVertex(GameObject vertex)
    {
        vertexEntities.Add(vertex);
        UpdateAdjacency(vertex);
    }

    // Get all neighbors of a vertex
    public List<GameObject> GetNeighbors(GameObject vertex)
    {
        if (adjacencyMap.ContainsKey(vertex))
            return adjacencyMap[vertex];
        return new List<GameObject>(); //return empty list
    }

    // Remove vertex from AdjacencyMap and update
    public void RemoveVertex(GameObject vertex)
    {
        List<GameObject> neighbors = null;
        if (vertexEntities.Contains(vertex))
            vertexEntities.Remove(vertex);
        if (adjacencyMap.ContainsKey(vertex))
        {
            neighbors = adjacencyMap[vertex];
            adjacencyMap.Remove(vertex);
        }
        else
            return;
        if (neighbors == null) // uneccessary (?) safeguard.
            return;
        foreach (GameObject otherVertex in neighbors)
        {
            if (adjacencyMap[otherVertex].Contains(vertex))
                adjacencyMap[otherVertex].Remove(vertex);
        }
    }

    public bool ContainsVertex(GameObject vertex)
    {
        return vertexEntities.Contains(vertex);
    }

    // "Ragged Walk". Tries to get on the same file as a target piece, with some heuristics and search once that's complete.
    // Extremely fast, because once we reach the file, we're basically just rule following.
    // TODO: implement search when file is not obvious. Using either wall clinging, or something different.
    public List<GameObject> RaggedWalk(GameObject root, GameObject target, Dictionary<Vector3Int, GameObject> map, int maxSteps = 1000)
    {
        List<GameObject> path = new List<GameObject>();
        HexEntity targetEntity = target.GetComponent<HexEntity>();
        int steps = 0;
        path.Add(root);
        while (!path.Contains(target))
        {
            if (steps > maxSteps)
                break;
            HexEntity rootEntity = root.GetComponent<HexEntity>();
            Vector3Int coordDist = rootEntity.Position - targetEntity.Position;
            Vector3Int nextPos = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
            List<GameObject> neighbors = GetNeighbors(root);
            if (neighbors.Contains(target))
            {
                path.Add(target);
                return path;
            }
            if (coordDist.x == 0)
            {
                if (coordDist.y > 0)
                {
                    // Move north west
                    Debug.Log("On-file NW");
                    nextPos = rootEntity.Position + new Vector3Int(0, 1, -1) * -1;
                }
                else
                {
                    // Move south east
                    Debug.Log("On-file SE");
                    nextPos = rootEntity.Position + new Vector3Int(0, -1, 1) * -1;
                }
            }
            else if (coordDist.y == 0)
            {
                if (coordDist.x > 0)
                {
                    Debug.Log("On-file NE");
                    // Move north east
                    nextPos = rootEntity.Position + new Vector3Int(1, 0, -1) * -1;
                }
                else
                {
                    // Move south west
                    Debug.Log("On-file SW");
                    nextPos = rootEntity.Position + new Vector3Int(-1, 0, 1) * -1;
                }
            }
            else if (coordDist.z == 0)
            {
                if (coordDist.y > 0)
                {
                    // Move left
                    Debug.Log("On-file E");
                    nextPos = rootEntity.Position + new Vector3Int(-1, 1, 0) * -1;
                }
                else
                {
                    Debug.Log("On-file W");
                    nextPos = rootEntity.Position + new Vector3Int(1, -1, 0) * -1;
                    // Move right
                }
            }
            else
            {
                if (coordDist.y > 0 && coordDist.z > 0)
                {
                    // Move southwest
                    Debug.Log("Off-file SW");
                    nextPos = rootEntity.Position + new Vector3Int(-1, 0, 1) * -1;
                }
                else if (coordDist.y > 0 && coordDist.z < 0)
                {
                    // Move northwest
                    Debug.Log("Off-file NW");
                    nextPos = rootEntity.Position + new Vector3Int(0, 1, -1) * -1;
                }
                else if (coordDist.x > 0 && coordDist.z > 0)
                {
                    // Move southeast
                    Debug.Log("Off-file SE");
                    nextPos = rootEntity.Position + new Vector3Int(0, -1, 1) * -1;

                }
                else if (coordDist.x > 0 && coordDist.z < 0)
                {
                    // Move northeast
                    Debug.Log("Off-file NE");
                    nextPos = rootEntity.Position + new Vector3Int(1, 0, -1) * -1;

                }
                else if (coordDist.z > 0 && coordDist.y < 0 && coordDist.x < 0)
                {
                    // Move northeast?
                    Debug.Log("Off-file SW");
                    nextPos = rootEntity.Position + new Vector3Int(-1, 0, 1) * -1;
                }
                else if (coordDist.z < 0 && coordDist.y > 0 && coordDist.x > 0)
                {
                    // Move south?
                    Debug.Log("Off-file NE");
                    nextPos = rootEntity.Position + new Vector3Int(1, 0, -1) * -1;
                }
            }
            Debug.Log("From position " + rootEntity.Position + " looking at target " + targetEntity.Position + " distance was " + (rootEntity.Position - targetEntity.Position) + " nextPos is " + nextPos);
            if (map.ContainsKey(nextPos))
            {
                root = map[nextPos];
                path.Add(root);
            }
            else
            {
                int index = (int)Random.Range(0, neighbors.Count);
                // TODO add heuristic
                path.Add(neighbors[index]);
            }
            steps++;
        }
        return path;

    }

    // Depth-First search for a target. Often fails.
    public List<GameObject> DFS(GameObject root, GameObject target, List<GameObject> path = null)
    {
        if (path == null)
            path = new List<GameObject>();
        path.Add(root);
        List<GameObject> neighbors = GetNeighbors(root);
        if (neighbors.Contains(target))
        {
            path.Add(target);
            return path;
        }
        foreach (GameObject neighbor in neighbors)
        {
            if (!path.Contains(neighbor))
            {
                path = DFS(neighbor, target, path);
                if (path.Contains(target))
                    return path;
            }
        }
        return path;
    }

    // Random walk through graph to find a target.
    public List<GameObject> RandomWalk(GameObject root, GameObject target, int maxAttempts = 1000)
    {
        List<GameObject> neighbors = GetNeighbors(root);
        List<GameObject> visited = new List<GameObject>();
        visited.Add(root);
        List<GameObject> attempted = new List<GameObject>();
        if (neighbors.Count == 0)
            return visited;
        int length = 0;
        int attempts = 0;
        GameObject next = root;
        while (next != target)
        {
            int index = (int)Random.Range(0, neighbors.Count);
            next = neighbors[index];
            if (!attempted.Contains(next))
                attempted.Add(next);
            if (!visited.Contains(next))
            {
                visited.Add(next);
                root = next;
                neighbors = GetNeighbors(root);
                length += 1;
            }
            else if (attempted.Count == neighbors.Count)
            {
                length -= 1;
                visited.Remove(root);
                root = visited[length];
                neighbors = GetNeighbors(root);
                attempted = new List<GameObject>();
            }
            attempts++;
            if (attempts > maxAttempts)
            {
                Debug.Log("Path not found!");
                break;
            }

        }
        return visited;
    }

    // Back-reconstruct the A* path
    private List<GameObject> _reconstructAstarPath(Dictionary<GameObject, GameObject> cameFrom, GameObject current)
    {
        List<GameObject> totalPath = new List<GameObject>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    // Return dictionary key with minimum fScore from open list.
    // TODO: Faster way to do this
    private GameObject _minFScore(Dictionary<GameObject, float> fScore, List<GameObject> open)
    {
        float min = float.MaxValue;
        GameObject result = null;
        foreach (GameObject ky in open)
        {
            if (fScore[ky] < min)
            {
                min = fScore[ky];
                result = ky;
            }
        }
        return result;
    }

    // A* search for finding shortest path.
    public List<GameObject> NearestAstar(GameObject root, GameObject target)
    {
        HexEntity rootEntity = root.GetComponent<HexEntity>();
        HexEntity targetEntity = target.GetComponent<HexEntity>();
        List<GameObject> closed = new List<GameObject>();
        List<GameObject> open = new List<GameObject>();
        Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();
        open.Add(root);
        Dictionary<GameObject, float> gScore = new Dictionary<GameObject, float>();
        Dictionary<GameObject, float> fScore = new Dictionary<GameObject, float>();
        gScore[root] = 0;
        fScore[root] = rootEntity.Distance(targetEntity);
        while (open.Count != 0)
        {
            GameObject current = _minFScore(fScore, open);
            HexEntity currentEntity = current.GetComponent<HexEntity>();
            if (current == target)
                return _reconstructAstarPath(cameFrom, current);
            List<GameObject> neighbors = GetNeighbors(current);
            if (neighbors.Contains(target))
            {
                cameFrom[target] = current;
                current = target;
                return _reconstructAstarPath(cameFrom, current);
            }
            open.Remove(current);
            closed.Remove(current);
            foreach (GameObject neighbor in neighbors)
            {
                if (!gScore.ContainsKey(neighbor))
                    gScore[neighbor] = Mathf.Infinity;
                if (!fScore.ContainsKey(neighbor))
                    fScore[neighbor] = Mathf.Infinity;
                HexEntity neighborEnt = neighbor.GetComponent<HexEntity>();
                if (closed.Contains(neighbor))
                    continue;
                float tentativeGscore = gScore[current] + 1;//currentEntity.Distance(neighborEnt);
                if (tentativeGscore <= gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGscore;
                    float h = Vector3.Distance(neighbor.transform.position, target.transform.position);
                    fScore[neighbor] = gScore[neighbor] + h;
                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }
        }
        return open;
    }

}