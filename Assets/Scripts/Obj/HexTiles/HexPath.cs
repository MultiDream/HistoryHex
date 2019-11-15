using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
ClassName:      HexPath
Author:         Brad Baker
Creation Date:  10032019
Inherits:       MonoBehavior ?
Abstract:       No
Brief:          An object for creating paths of Hex tiles. Convenient storage of references to hex tiles,
                    as well as additional properties and convenient methods for checking breaks,
                    and finding shortest paths.
Constructor:    None
*/
public class HexPath : MonoBehaviour
{
    #region Properties
    // Internal variables
    private List<GameObject> hexEntities; // list of tiles
    private AdjacencyMap adjacency;
    private List<GameObject> lines;
    [SerializeField] private Material pathMaterial;
    [SerializeField] private Material pathEndMaterial;
	//Prefabs

	//Public variables
    public string Name { get; set; }
	public ArmyEntity army;

	#endregion

	#region Monobehaivor Methods
	// Start is called before the first frame update
	void Start()
    {
        Draw();
    }

    // Update is called once per frame.
    void Update()
    {

    }

	private void OnDestroy() {
		foreach (GameObject line in lines) {
			Destroy(line);
		}
		UnRegisterOrder(); //Decouple from the supply line.
	}
	#endregion

	//If Activated, run the extended activation methods.
	private void ActiveUpdate()
    {

    }

    private void WireSelectionInterface()
    {

    }

    //Start Delegation
    public void Initialize()
    {
        Debug.Log("Initializing!");
        Name = "EmptyPath";
        hexEntities = new List<GameObject>();
        lines = new List<GameObject>();
        //adjacency = new AdjacencyMap();
    }

    // ContainsHex - wraps the list Contains function.
    public bool ContainsHex(GameObject hex)
    {
        return hexEntities.Contains(hex);
    }

    public bool AddHex(GameObject hex)
    {
        if (!ContainsHex(hex))
        {
            hexEntities.Add(hex);
            //adjacency.AddVertex(hex);
        }
        return ContainsHex(hex);
    }

    public void AddHexes(List<GameObject> hexes)
    {
        foreach (GameObject hex in hexes)
            AddHex(hex);
    }

    public void Refresh(List<GameObject> hexes) {
        foreach (GameObject line in lines) {
			Destroy(line);
		}
        hexEntities = hexes;
        if (!IsConnected()){
            Debug.Log("Not connected!");
            Destroy(gameObject);
        }
        else {
            Draw();
        }
    }

    public bool IsConnected(){
        if (hexEntities.Count < 2)
            return false;
        for (int i=1; i<hexEntities.Count;i++){
            HexEntity last = hexEntities[i-1].GetComponent<HexEntity>();
            HexEntity next = hexEntities[i].GetComponent<HexEntity>();            
            if (!last.Adjacent(next)){
                return false;
            }
        }
        return true;
    }

    public void DrawCircle(float radius, float lineWidth, float x, float y)
    {
        GameObject container = new GameObject();
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.material = pathEndMaterial;
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius + x, 0.1f, Mathf.Cos(rad) * radius + y);
        }

        line.SetPositions(points);
        lines.Add(container);
    }

    private void DrawSegment(Vector3 start, Vector3 end, Color color, string textToWrite = "")
    {
        start += new Vector3(0, 0.1f, 0);
        end += new Vector3(0, 0.1f, 0);
        DrawCircle(0.1f, 0.1f, start.x, start.z);
        DrawCircle(0.1f, 0.1f, end.x, end.z);
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent<LineRenderer>();
        lr.material = pathMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lines.Add(myLine);
        //GameObject.Destroy(myLine, duration);
    }

    private void Draw()
    {
        GameObject hex1, hex2;
        Color pathColor = UnityEngine.Random.ColorHSV();
        for (int i = 0; i < hexEntities.Count - 1; i++)
        {
            hex1 = hexEntities[i];
            if (i + 1 >= hexEntities.Count)
            {
                break;
            }
            hex2 = hexEntities[i + 1];
            DrawSegment(hex1.transform.position, hex2.transform.position, pathColor, "" + i);
        }
    }

	/// <summary>
	/// Interface for an entity to request food from the hex path entity.
	/// TODO: Hook this up to the base of food production.
	/// It may be possible that this should instead be on another component...
	/// </summary>
	/// <param name="amountRequested">Amount of food requested</param>
	/// <returns>Amount of food transported.</returns>
	public int FoodRequest(int amountRequested) {
		if (amountRequested <= 0)
		{
			return 0;
		}
		int finalIndex = hexEntities.Count - 1;
        if (finalIndex < 0)
            return 0;
		GameObject baseTile = hexEntities[finalIndex];
		HexEntity entity = baseTile.GetComponent<HexEntity>();
		return entity.FoodRequest(amountRequested,this.Length());
	}

	public void RegisterOrder(){
		int finalIndex = hexEntities.Count - 1;
		GameObject baseTile = hexEntities[finalIndex];
		HexEntity entity = baseTile.GetComponent<HexEntity>();
		entity.CreateOrder(this);
	}

	public void UnRegisterOrder() {
		int finalIndex = hexEntities.Count - 1;
		GameObject baseTile = hexEntities[finalIndex];
		HexEntity entity = baseTile.GetComponent<HexEntity>();
		entity.DeleteOrder(this);
	}

	public GameObject GetHex(int i){
        if (hexEntities.Count > i)
            return hexEntities[i];
        return null;
    }

    public int Length(){
        return hexEntities.Count;
    }
}