using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class ForceVisualisation : MonoBehaviour
{
    [SerializeField] private MagnetismController _magnetismController;
    [SerializeField] private GameObject _beam;

    private HashSet<MagneticForce> _activeForces = new HashSet<MagneticForce>();
    public List<MagnetConnection> Connections = new List<MagnetConnection>();
    private Tuple<Magnet, Magnet> _connection, _invertedConnection;
    private HashSet<MagnetConnection> _connectionsToRemove = new HashSet<MagnetConnection>();

    void FixedUpdate()
    {
        UpdateConnections();        
        UpdateConnectionsVisuals();
    }

    public void UpdateConnections()
    {
        _activeForces.Clear();
        _activeForces.UnionWith(_magnetismController.activeForces);
        foreach (MagneticForce force in _activeForces)
        {
            _connection = new Tuple<Magnet, Magnet>(force._magnet1, force._magnet2);
            _invertedConnection = new Tuple<Magnet, Magnet>(force._magnet2, force._magnet1);

            var existingConnection = Connections.Find(c => ((c.Tuple.Item1 == _connection.Item1 && c.Tuple.Item2 == _connection.Item2) || (c.Tuple.Item1 == _invertedConnection.Item1 && c.Tuple.Item2 == _invertedConnection.Item2)));
            if (existingConnection is null)
            {
                CreateNewConnections(force, _connection);
            }
            else
            {
                existingConnection.MagneticForce = force;
            }
        }
        foreach(var connection in Connections)
        {
            _connection = new Tuple<Magnet, Magnet>(connection.MagneticForce._magnet1, connection.MagneticForce._magnet2);
            _invertedConnection = new Tuple<Magnet, Magnet>(connection.MagneticForce._magnet2, connection.MagneticForce._magnet1);

            if (!_activeForces.Any(magneticForce => (magneticForce._magnet1 == _connection.Item1 && magneticForce._magnet2 == _connection.Item2) || (magneticForce._magnet1 == _invertedConnection.Item1 && magneticForce._magnet2 == _invertedConnection.Item2)))
            {
                _connectionsToRemove.Add(connection);
            }
        }              
        foreach(var connection in _connectionsToRemove)
        {
            RemoveConnections(connection);
        }
    }
    private void UpdateConnectionsVisuals()
    {
        foreach (var connection in Connections)
        {
            connection.LineRenderer.SetPosition(0, connection.MagneticForce._magnet1Point);
            connection.LineRenderer.SetPosition(1, connection.MagneticForce._magnet2Point);
        }
    }

    private void CreateNewConnections(MagneticForce forceToVisualise, Tuple<Magnet, Magnet> tuple)
    {
        var newBeam = Instantiate(_beam, Vector2.zero, Quaternion.identity);
        var newConnection = new MagnetConnection(forceToVisualise, tuple, newBeam);
        Connections.Add(newConnection);
    }
    private void RemoveConnections(MagnetConnection connection)
    {
        Destroy(connection.Beam);
        Connections.Remove(connection);
    }
}
