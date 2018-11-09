using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Graph g = new Graph(6);
        g.addEdge(1, 2);
        g.addEdge(3, 4);
        g.addEdge(2, 4);
        Debug.LogError(g);

        DepthFirstSearch dfs = new DepthFirstSearch(g, 2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
