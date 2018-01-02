using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimplePlatformer
{
    public static class CoordinateHelper
    {

        public const float unitToPixel = 100.0f;
        public const float pixelToUnit = 1 / unitToPixel;

        public static Vector2 ToScreen(Vector2 worldCoordinates)
        {
            return worldCoordinates * unitToPixel;
        }

        public static Vector2 ToWorld(Vector2 screenCoordinates)
        {
            return screenCoordinates * pixelToUnit;
        }
    }
}
