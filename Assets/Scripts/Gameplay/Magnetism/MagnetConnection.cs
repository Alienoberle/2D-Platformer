using System;
using UnityEngine;

public partial class ForceVisualisation
{
    public class MagnetConnection
    {
        public MagneticForce MagneticForce;
        public Tuple<Magnet, Magnet> Tuple;
        public GameObject Beam;
        public LineRenderer LineRenderer;
        public MagnetConnection(MagneticForce magneticForce , Tuple<Magnet, Magnet> tuple, GameObject beam)
        {
            MagneticForce = magneticForce;
            Tuple = tuple;
            Beam = beam;
            LineRenderer = Beam.GetComponent<LineRenderer>();
        }
    }
}
