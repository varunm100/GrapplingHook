using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePlatformer
{
    class findHook
    {
        public findHook()
        {

        }

        public static bool stopNull = false;

        public static DrawablePhysicsObject findClosestHook(List<DrawablePhysicsObject> hooklist, Body playerBody)
        {
            if (!stopNull)
            {
                List<double> distances = new List<double>();
                foreach (var i in hooklist)
                {
                    distances.Add(Vector2.Distance(i.body.Position, playerBody.Position));
                }
                double minVal = distances.Min();
                return hooklist[distances.IndexOf(minVal)];
            } else
            {
                stopNull = false;
                return hooklist[0];
            }
        }

        public static double getAngle(Vector2 point1, Vector2 point2)
        {
            double xDiff = point2.X - point1.X;
            double yDiff = point2.Y - point1.Y;
            Console.WriteLine(Math.Atan2(yDiff, xDiff) * (180 / Math.PI));
            return Math.Atan2(yDiff, xDiff);
        }
    }
}
