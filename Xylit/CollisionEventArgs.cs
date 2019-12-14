using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xylit.Playground.GameObjects;

namespace Xylit
{
    class CollisionEventArgs
    {
        Direction direction;
        GameObject collidingObject;

        public Direction Direction { get { return direction; } }
        public GameObject CollidingObject { get { return collidingObject; } }

        public CollisionEventArgs(Direction direction, GameObject CollidingObject)
        {
            this.direction = direction;
            this.collidingObject = CollidingObject;
        }
    }
}
