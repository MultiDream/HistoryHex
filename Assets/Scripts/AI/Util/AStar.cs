using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

    private Dictionary<GameObject, List<GameObject>> adjacencyMap;
    private List<GameObject> vertexEntities;

    // Get all neighbors of a vertex
    public List<GameObject> GetNeighbors(GameObject vertex) {
        if (adjacencyMap.ContainsKey(vertex))
            return adjacencyMap[vertex];
        return new List<GameObject>(); //return empty list
    }

    // Back-reconstruct the A* path
    private List<GameObject> ReconstructPath(Dictionary<GameObject, GameObject> cameFrom, GameObject current) {
        List<GameObject> totalPath = new List<GameObject>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    // Return dictionary key with minimum fScore from open list.
    // TODO: Faster way to do this
    private GameObject MinFScore(Dictionary<GameObject, float> fScore, List<GameObject> open) {
        float min = float.MaxValue;
        GameObject result = null;
        foreach (GameObject ky in open) {
            if (fScore[ky] < min) {
                min = fScore[ky];
                result = ky;
            }
        }
        return result;
    }

    public List<GameObject> CreatePath(GameObject root, GameObject target, int maxAttempts = 100) {
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
        int attempts = 0;
        while (open.Count != 0) {
            if (attempts > maxAttempts)
                break;
            GameObject current = MinFScore(fScore, open);
            HexEntity currentEntity = current.GetComponent<HexEntity>();
            if (current == target)
                return ReconstructPath(cameFrom, current);
            List<GameObject> neighbors = GetNeighbors(current);
            if (neighbors.Contains(target)) {
                cameFrom[target] = current;
                current = target;
                return ReconstructPath(cameFrom, current);
            }
            open.Remove(current);
            closed.Remove(current);
            foreach (GameObject neighbor in neighbors) {
                if (!gScore.ContainsKey(neighbor))
                    gScore[neighbor] = Mathf.Infinity;
                if (!fScore.ContainsKey(neighbor))
                    fScore[neighbor] = Mathf.Infinity;
                HexEntity neighborEnt = neighbor.GetComponent<HexEntity>();
                if (closed.Contains(neighbor))
                    continue;
                float tentativeGscore = gScore[current] + 1;//currentEntity.Distance(neighborEnt);
                if (tentativeGscore <= gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGscore;
                    float h = Vector3.Distance(neighbor.transform.position, target.transform.position);
                    fScore[neighbor] = gScore[neighbor] + h;
                    if (!open.Contains(neighbor))
                        open.Add(neighbor);
                }
            }
            attempts += 1;
        }
        return open;
    }
}
