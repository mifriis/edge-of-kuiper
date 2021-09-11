using System.Collections.Generic;
using System.Drawing;

namespace kuiper.Domain.CelestialBodies
{
    public interface ICelestialBody
    {
         string Name {get;}

         /// <summary>
         /// Mean distance to parent body which will become orbit distance.
         /// </summary>
         /// <value>Radius in AU.</value>
         float OrbitRadius {get;}

         /// <summary>
         /// Linear velocity for orbit.
         /// </summary>
         /// <value>Velocity in km/s.</value>
         float Velocity {get;}

         /// <summary>
         /// The degrees value the body was, on its orbit path, on the 1/1/2078
         /// </summary>
         /// <value>Start position in degrees.</value>
         float OriginDegrees {get;}
         PointF Position {get;}

         /// <summary>
         /// This is the body this body orbits around.
         /// </summary>
         /// <value>Reference to another body.</value>
         CelestialBody Parent {get;}

         /// <summary>
         /// The collection of orbiting satellites.
         /// </summary>
         /// <value>Enumerable of other bodies.</value>
         IEnumerable<CelestialBody> Satellites {get;}
    }
}