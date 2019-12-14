using System;
using Xylit;
using Xylit.Playground;

namespace Xylit
{
    namespace Playground
    {
        namespace GameObjects
        {
            enum Direction
            {
                Up,
                Down,
                Left,
                Right,
            }


            abstract class Transformable: GameObject
            {
                protected Region region;
                protected Direction direction;
                public Direction Direction { get { return direction; } set { direction = value; } }

                public virtual void Move(Direction direction)
                {
                    this.direction = direction;

                    if (direction == Direction.Up)
                        this.Symbol.Y -= 1;
                    else if (direction == Direction.Down)
                        this.Symbol.Y += 1;
                    else if (direction == Direction.Left)
                        this.Symbol.X -= 1;
                    else if (direction == Direction.Right)
                        this.Symbol.X += 1;


                }
                public virtual bool MoveDraw(Direction direction)
                {
                    this.direction = direction;

                    if (direction == Direction.Up)
                        this.Symbol.Y -= 1;
                    else if (direction == Direction.Down)
                        this.Symbol.Y += 1;
                    else if (direction == Direction.Left)
                        this.Symbol.X -= 1;
                    else if (direction == Direction.Right)
                        this.Symbol.X += 1;

                    region.SetDrawObject(this);
                    return true;
                }

                public static void ReplaceInMap(Region region, GameObject obj1, GameObject obj2)
                {
                    /* update region */
                    region.Replace(obj1, obj2);
                }
            }

        }
    }
}