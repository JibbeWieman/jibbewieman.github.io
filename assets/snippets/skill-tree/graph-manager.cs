using System.Collections.Generic;
using UnityEngine;

public class GraphManager : Manager
{
    #region VARIABLES
    public Dictionary<int, Node> Nodes = new();  // Dictionary to store skill nodes with their IDs

    [SerializeField]
    private List<Edge> edges;  // List of edges where you manually assign the nodes in the Inspector

    [SerializeField]
    private SceneTypeObject edgeCollection;   // Serialize the GameObject references
    #endregion

    public override void Start()
    {
        base.Start();
        InitialiseEdges();
    }

    /// <summary>
    /// Initialise all line renderers for edges.
    /// </summary>
    private void InitialiseEdges()
    {
        foreach (Edge edge in edges)
        {
            edge.InitialiseLineRenderer(edgeCollection.Objects[0]);
        }
    }

    /// <summary>
    /// Adds a skill node to the dictionary if it doesn't already exist.
    /// </summary>
    /// <param name="id">The ID of the skill node.</param>
    /// <param name="skillNode">The skill node to add.</param>
    public void AddSkillNode(int id, Node skillNode)
    {
        if (!Nodes.ContainsKey(id))
        {
            Nodes.Add(id, skillNode);
        }
        else
        {
            Debug.LogWarning($"Node with ID {id} already exists in the graph.");
        }
    }

    public List<Node> GetPrerequisites(int nodeId)
    {
        List<Node> prerequisites = new();

        foreach (Edge edge in edges)
        {
            if (edge.toNodeId == nodeId && Nodes.TryGetValue(edge.fromNodeId, out Node prerequisiteNode))
            {
                prerequisites.Add(prerequisiteNode);
            }
        }

        return prerequisites;
    }
}
