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

        public static DrawablePhysicsObject findClosestHook(List<DrawablePhysicsObject> hooklist, Body playerBody)
        {
            List<double> distances = new List<double>();
            foreach(var i in hooklist)
            {
                distances.Add(Vector2.Distance(i.body.Position, playerBody.Position));
                Console.Write(Vector2.Distance(i.body.Position, playerBody.Position).ToString() + " , ");
            }
            Console.WriteLine("");
            double minVal = distances.Min();
            Console.WriteLine("Index at " + distances.IndexOf(minVal).ToString());
            return hooklist[distances.IndexOf(minVal)];
        }
    }
}
