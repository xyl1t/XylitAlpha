using System;
using System.Collections;
using Xylit.UI;

namespace Xylit
{
    namespace Playground
    {
        namespace GameObjects
        {
            class GameObject
            {
                public ProChar Symbol;

                protected string name;
                public string Name { get { return name; } set { name = value; } }

                protected string objectType;
                public string ObjectType { get { return objectType; } }

                public virtual void NewLocation(int X, int Y)
                {
                    this.Symbol.X = X;
                    this.Symbol.Y = Y;
                }


                public static void Draw(GameObject gameObject)
                {
                    Console.SetCursorPosition(gameObject.Symbol.X, gameObject.Symbol.Y);
                    Console.ForegroundColor = gameObject.Symbol.ForeColor;
                    Console.BackgroundColor = gameObject.Symbol.BackColor;
                    Console.Write((char)gameObject.Symbol.Character);
                }
                public static void Draw(GameObject gameObject, int offsetX, int offsetY)
                {
                    Console.SetCursorPosition(gameObject.Symbol.X + offsetX, gameObject.Symbol.Y + offsetY);
                    Console.ForegroundColor = gameObject.Symbol.ForeColor;
                    Console.BackgroundColor = gameObject.Symbol.BackColor;
                    Console.Write((char)gameObject.Symbol.Character);
                }
                public static void Draw(char Character, int X, int Y, ConsoleColor ForeColor, ConsoleColor BackColor)
                {
                    Console.SetCursorPosition(X, Y);
                    Console.ForegroundColor = ForeColor;
                    Console.BackgroundColor = BackColor;
                    Console.Write((char)Character);
                }

                public ConsoleColor ChangeForeColor(ConsoleColor ForeColor)
                {
                    return this.Symbol.ForeColor = ForeColor;
                }

                public ConsoleColor ChangeBackColor(ConsoleColor BackColor)
                {
                    return this.Symbol.BackColor = BackColor;
                }
            }
        }
    }
}