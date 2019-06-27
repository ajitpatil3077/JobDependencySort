using System;
using System.Collections.Generic;
using System.Linq;

namespace ResolveJobDependency
{
    class Program
    {
        public static void Main(string[] args)
        {
            //            //a =>
            //            //b => c
            //            //c => f
            //            //d => a
            //            //e => b
            //            //f =>

            Graph<string> graph = new Graph<string>();

            //Test Case - Empty Input
            graph.AddVertex("");
            System.Diagnostics.Debug.Assert(graph.SortedJobList().Equals(""));
            Console.WriteLine($" { graph.SortedJobList() }Is empty");
            graph.Clear();

            //Test Case - Single Input
            graph.AddVertex("a");
            System.Diagnostics.Debug.Assert(graph.SortedJobList().Equals("a"));
            Console.WriteLine(graph.SortedJobList());
            graph.Clear();


            //Test Case - No dependencies
            graph.AddVertex("a", "b", "c");
            graph.AddEdge("a", null);
            graph.AddEdge("b", null);
            graph.AddEdge("c", null);
            System.Diagnostics.Debug.Assert(graph.SortedJobList().Equals("a b c"));
            Console.WriteLine(graph.SortedJobList());
            graph.Clear();

            //Test Case - Job dependencies and its output
            graph.AddVertex("a", "b", "c", "d", "e", "f");
            graph.AddEdge("a", null);
            graph.AddEdge("b", "c");
            graph.AddEdge("c", "f");
            graph.AddEdge("d", "a");
            graph.AddEdge("e", "b");
            graph.AddEdge("f", null);
            System.Diagnostics.Debug.Assert(graph.SortedJobList().Equals("d e a b c f"));
            Console.WriteLine(graph.SortedJobList());
            graph.Clear();

            //Test Case - Job dependencies and few without link dependency
            graph.AddVertex("a", "b", "c");
            graph.AddEdge("a", null);
            graph.AddEdge("b", "c");
            graph.AddEdge("c", null);
            System.Diagnostics.Debug.Assert(graph.SortedJobList().Equals("a b c"));
            Console.WriteLine(graph.SortedJobList());
            graph.Clear();


            //Test Case - Self dependency
            graph.AddVertex("a", "b", "c");
            graph.AddEdge("a", null);
            graph.AddEdge("b", null);
            graph.AddEdge("c", "c");

            System.Diagnostics.Debug.Assert(graph.SortedJobList().Equals("Having circular dependency"));
            Console.WriteLine(graph.SortedJobList());
            graph.Clear();

            //Test Case - Having circular dependency
            graph.AddVertex("a", "b", "c", "d", "e", "f");
            graph.AddEdge("a", null);
            graph.AddEdge("b", "c");
            graph.AddEdge("c", "f");
            graph.AddEdge("d", "a");
            graph.AddEdge("e", null);
            graph.AddEdge("f", "b");
            System.Diagnostics.Debug.Assert(graph.SortedJobList().Equals("Having circular dependency"));
            Console.WriteLine(graph.SortedJobList());
            graph.Clear();

            Console.ReadKey();

        }
    }
    
}


/// <summary>
/// This is a Node of a graph having details about its value and outward vertex and Indegree information
/// </summary>
/// <typeparam name="T"></typeparam>
public class Node<T>
{
    public T Value { get; set; }
    public HashSet<T> OutwardVertex { get; set; }
    public int Indegree { get; set; }

    public Node(T value)
    {
        Value = value;
        OutwardVertex = new HashSet<T>();
        Indegree = 0;
    }

    /// <summary>
    /// Compares the node value
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public override bool Equals(object o)
    {
        Node<T> graphNode = o as Node<T>;
        return this.Value.Equals(graphNode.Value);
    }

    /// <summary>
    /// Gets the Hash code
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int hash = 11;
        hash = (hash * 9) + Value.GetHashCode();
        return hash;
    }
}

public class Graph<T>
{
    private Dictionary<T, Node<T>> AdjacencyDictionary { get; set; }

    public Graph()
    {
        AdjacencyDictionary = new Dictionary<T, Node<T>>();
    }

    /// <summary>
    /// Validates if node exists with given source and destination.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool IsNodeLinked(T source, T destination)
    {
        Node<T> node = AdjacencyDictionary[source];
        return node.OutwardVertex.Contains(destination);
    }

    /// <summary>
    /// Add vertex to the list available in graph.
    /// </summary>
    /// <param name="sources"></param>
    public void AddVertex(params T[] sources)
    {
        foreach (var s in sources)
        {
            AdjacencyDictionary.Add(s, new Node<T>(s));
        }
    }

    /// <summary>
    /// Clear the values of graph nodes.
    /// </summary>
    public void Clear()
    {
        AdjacencyDictionary.Clear();
     }

    /// <summary>
    /// Add edge to the graph
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    public void AddEdge(T source, T destination)
    {
        Node<T> node = AdjacencyDictionary[source];

        if (destination != null)
        {
            node.OutwardVertex.Add(destination);
            AdjacencyDictionary[destination].Indegree++;
        }

    }


    public string SortedJobList()
    {
        List<T> output = TopologicalSort();
        string jobList = output != null? string.Join(" ", output.ToArray()) : "Having circular dependency";

        return jobList;
    }


    /// <summary>
    /// This method does returns sorted list jobs based upon job dependencies. 
    /// </summary>
    /// <returns></returns>
    private List<T> TopologicalSort()
    {
        // Queue for maintaining zero outgoing node
        Queue<T> zeroIndegreeQueue = new Queue<T>();

        //Created sort list 
        HashSet<T> sortedList = new HashSet<T>();

        //create dictionary to maintain nodes having incoming vertex > 0
        Dictionary<T, int> inDegreeDictionary = new Dictionary<T, int>();


        //loop through all the available nodes. 
        foreach (KeyValuePair<T, Node<T>> adjacencyKeyValue in AdjacencyDictionary)
        {
            int indegree = adjacencyKeyValue.Value.Indegree;

            if (indegree == 0) 
                // put node into queue which are not having incoming vertex.
                zeroIndegreeQueue.Enqueue(adjacencyKeyValue.Key);
            else
                //update dictionary which are having one or more incoming vertex.
                inDegreeDictionary[adjacencyKeyValue.Key] = indegree;
        }


        //Run through queue until empty.
        while (zeroIndegreeQueue.Count != 0)
        {
            //Remove node from queue.
            T graphNode = zeroIndegreeQueue.Dequeue();

            //Add zero indegree node to the sorted list 
            sortedList.Add(graphNode);

            //traverse through outwardVertex of select vertex.  
            foreach (T node in AdjacencyDictionary[graphNode].OutwardVertex)
            {
                // find node in inDegreeDictionary - contains node having indegree > 0.
                if (inDegreeDictionary.ContainsKey(node))
                { 
                    int inDegree = inDegreeDictionary[node] - 1;

                    if (inDegree == 0)
                    {
                        //add node to queue and remove it from inDegreeDictionary. 
                        zeroIndegreeQueue.Enqueue(node);
                        inDegreeDictionary.Remove(node);
                    }
                    else
                    {
                        // update inDegree value for existing node in inDegreeDictionary.
                        inDegreeDictionary[node] = inDegree;
                    }
                }
            }
        }

        if (inDegreeDictionary.Any())
        {
            return null; // Having circular dependency.
         }

        // return the sorted job list.
        return sortedList.ToList();
    }
}

