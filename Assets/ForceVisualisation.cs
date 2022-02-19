using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForceVisualisation : MonoBehaviour
{
    [SerializeField] private MagnetismController magnetismController;
    [SerializeField] private GameObject beam;

    private HashSet<MagneticForce> activeForces = new HashSet<MagneticForce>();
    private HashSet<MagneticForce> existingForces = new HashSet<MagneticForce>();
    private Dictionary<Tuple<Magnet, Magnet>, MagneticForce> connections = new Dictionary<Tuple<Magnet, Magnet>, MagneticForce>();
    public Dictionary<MagneticForce, GameObject> beams = new Dictionary<MagneticForce, GameObject>();
    public List<Connection> connections2 = new List<Connection>();


    void FixedUpdate()
    {
        UpdateConnections();        
        UpdateConnectionsVisuals();
    }

    public void UpdateConnections()
    {
        activeForces.Clear();
        activeForces.UnionWith(magnetismController.activeConnections);
        foreach (MagneticForce force in activeForces)
        {
            var connection = new Tuple<Magnet, Magnet>(force._magnet1, force._magnet2);
            var invertedConnection = new Tuple<Magnet, Magnet>(force._magnet2, force._magnet1);


            var existingConnections = from con in connections2
                                      where con.tuple == connection || con.tuple == invertedConnection
                                      select con;

            foreach (Connection c in existingConnections)
            {
                c._magneticForce = force;
            }
            
            var newConnections = from con in connections2
                                 where con.tuple != connection || con.tuple != invertedConnection
                                 select con;

            foreach (Connection c in existingConnections)
            {
                connections2.Add(c);
                CreateNewConnections(force);
            }


            //if (connections.ContainsKey(connection)) 
            //{
            //    connections[connection] = force; // does not seem to update the points
            //    return;
            //}
            //if ( connections.ContainsKey(invertedConnection))
            //{
            //    connections[invertedConnection] = force;
            //    return;
            //}

            //connections.Add(connection, force);
            //CreateNewConnections(force);
        }

        existingForces.ExceptWith(activeForces);
        foreach (MagneticForce force in existingForces)
        {
            var connection = new Tuple<Magnet, Magnet>(force._magnet1, force._magnet2);
            var invertedConnection = new Tuple<Magnet, Magnet>(force._magnet2, force._magnet1);

            if (connections.ContainsKey(connection))
            {
                RemoveConnections(force);
                connections.Remove(connection);
            }


            if (connections.ContainsKey(invertedConnection))
            {
                RemoveConnections(force);
                connections.Remove(invertedConnection);
            }
        }
        existingForces.Clear();
        existingForces.UnionWith(activeForces);

        // debug print
        if (connections.Count > 0)
        {
            print(connections.Count);
            foreach (var connection in connections)
            {
                print($"Connection between " + connection.Key.Item1 + " and " + connection.Key.Item2);
            }
        }
    }
    private void UpdateConnectionsVisuals()
    {
        foreach (var beam in beams)
        {
            var lineRenderer = beam.Value.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, beam.Key._magnet1Point);
            lineRenderer.SetPosition(1, beam.Key._magnet2Point);
        }
    }
    private void CreateNewConnections(MagneticForce forceToVisualise)
    {
        var newBeam = Instantiate(beam, Vector2.zero, Quaternion.identity);
        beams.Add(forceToVisualise, newBeam);

    }
    private void RemoveConnections(MagneticForce forceToVisualise)
    {
        GameObject beamToDestroy = null;
        beamToDestroy = (beams.TryGetValue(forceToVisualise, out beamToDestroy))? beamToDestroy : null;
        Destroy(beamToDestroy);
        beams.Remove(forceToVisualise);
    }

    public class Connection
    {
        public Tuple<Magnet, Magnet> tuple;
        public MagneticForce _magneticForce;
        public GameObject beam;
    }
}
