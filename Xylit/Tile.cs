using System;
using System.Collections;
using Xylit.Playground.GameObjects;
using System.Data;

namespace Xylit
{
    namespace Playground
    {
        namespace GameObjects
        {


            class Tile : GameObject
            {
                public Tile(char character, string tileType, int x, int y) : this(character, tileType, x, y, ConsoleColor.White, ConsoleColor.Black) { }
                public Tile(char character, string tileType, int x, int y, ConsoleColor forecolor, ConsoleColor backcolor)
                {
                    this.Symbol.Character = character;
                    this.objectType = tileType;
                    this.Symbol.X = x;
                    this.Symbol.Y = y;
                    this.Symbol.ForeColor = forecolor;
                    this.Symbol.BackColor = backcolor;
                }

                public void ChangeType(string newType)
                {
                    this.objectType = newType;
                }

                public  void ChangeCharacter(char newCharacter)
                {
                    this.Symbol.Character = newCharacter;
                }
            }
        }
    }
}