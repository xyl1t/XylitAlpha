using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xylit.Playground.GameObjects;
using Xylit.Playground;
using System.Diagnostics;
using System.Threading;
using Xylit.UI;

// Include moveby in future versions...

namespace Xylit.Playground.GameObjects
{
    enum ActionProperty
    {
        Leave,
        Delete
    }

    delegate void StandingAction(MoveableObject sender, StandingEventArgs e);
    delegate bool CollisionAction(MoveableObject sender, CollisionEventArgs e);

    class MoveableObject: Transformable
    {
        /* for debug */
        int moves;

        /* Fields */
        protected GameObject oldStandingTile;
        protected GameObject standingTile;

        protected string[] obstacle;

        /* Properties */
        public ActionProperty Action = ActionProperty.Leave;
        public string[] Obstacle { get { return obstacle; } }
        public GameObject StandingTile { get { return standingTile; } set { standingTile = value; } }

        /* Events */
        public event StandingAction StandingTileAction;
        public event CollisionAction CollisionAction;

        /* Constructors */
        #region Consructor

        // Constructor 3
        public MoveableObject(
            Region region,
            char character,
            string objectType,
            string[] obstacles)
            : this(region, character, objectType, 0, 0, obstacles) { }

        // Constructor 2
        public MoveableObject(
            Region region,
            char character,
            string objectType,
            int x, int y,
            string[] obstacles)
            : this(region, character, objectType, x, y, ConsoleColor.White, ConsoleColor.Black, obstacles) { }

        // Constructor 1
        public MoveableObject(
            Region region,
            char character,
            string objectType,
            int x, int y,
            ConsoleColor forecolor, ConsoleColor backcolor,
            string[] obstacles)
        {
            standingTile = new Tile(
                Char.Parse(region.mapConfig[0, 4]),
                region.mapConfig[0, 7],
                x, y,
                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((region.mapConfig[0, 5] == "None") ? region.ForeColor.ToString() : region.mapConfig[0, 5])),
                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((region.mapConfig[0, 6] == "None") ? region.BackColor.ToString() : region.mapConfig[0, 7])));

            oldStandingTile = Region.GetTile((Tile)standingTile);

            this.region = region;
            this.Symbol.Character = character;
            this.objectType = objectType;
            this.Symbol.X = x;
            this.Symbol.Y = y;
            this.Symbol.ForeColor = forecolor;
            this.Symbol.BackColor = backcolor;
            this.obstacle = obstacles;
        }
        public MoveableObject(Region region, ProChar symbol, string objectType, string[] obstacles)
        {
            this.region = region;
            this.Symbol = symbol;
            this.objectType = objectType;
            this.obstacle = obstacles;

            standingTile = new Tile(
                Char.Parse(region.mapConfig[0, 4]),
                region.mapConfig[0, 7],
                symbol.X, symbol.Y,
                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((region.mapConfig[0, 5] == "None") ? region.ForeColor.ToString() : region.mapConfig[0, 5])),
                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((region.mapConfig[0, 6] == "None") ? region.BackColor.ToString() : region.mapConfig[0, 7])));

            oldStandingTile = Region.GetTile((Tile)standingTile);

        }

        #endregion

        /// <summary>
        /// Moves the object in given direction, but does NOT draw it.
        /// </summary>
        /// <param name="direction">Direction to move, e.g.: If you put 'Direction.Up' the object will move by 1 up </param>
        private void Move(Direction direction)
        {
            /* Set Direction */
            this.direction = direction;

            if (CollisionAction != null)
                CollisionAction(this, new CollisionEventArgs(direction, GetObjectFromDirection(direction)));
            else
                DefaultCollisionAction();

            /* Check for obstacles */
            if (!CheckCollision()) return;

            /* Set New Player Position */
            if (direction == Direction.Up) this.Symbol.Y -= 1;
            else if (direction == Direction.Down) this.Symbol.Y += 1;
            else if (direction == Direction.Left) this.Symbol.X -= 1;
            else if (direction == Direction.Right) this.Symbol.X += 1;

            /* Draw on player standingtile */
            oldStandingTile = Region.GetTile((Tile)standingTile);
            region.SetObject(oldStandingTile);

            /* Set New StandingTile */
            standingTile = region.Map[Symbol.X, Symbol.Y];
            if (StandingTileAction != null)
                StandingTileAction(this, new StandingEventArgs(standingTile));
            else
                DefaultAction();

            region.SetObject(this);

            Debug.Print(this.objectType + " | [X = " + Symbol.X + " | Y = " + Symbol.Y + "] | Standing Tile = " + standingTile.ObjectType.ToString());
            moves++;
            Debug.Print("Movements: " + moves);

        }

        /// <summary>
        /// Moves the object in given direction and draws it on the region.
        /// </summary>
        /// <param name="direction">Direction to move, e.g.: If you put 'Direction.Up' the object will move by 1 up </param>
        public override bool MoveDraw(Direction direction)
        {
            bool returncase;
            Monitor.Enter("MOVEOBJECT");
            try
            {
                /* Set Direction */
                this.direction = direction;

                if (CollisionAction != null)
                {
                    if (!CollisionAction(this, new CollisionEventArgs(this.direction, GetObjectFromDirection(direction))))
                        return false;
                }
                else
                    DefaultCollisionAction();

                /* Check for obstacles */
                if (!CheckCollision()) return false;

                /* Set New Player Position */
                if (Direction == Direction.Up) this.Symbol.Y -= 1;
                else if (Direction == Direction.Down) this.Symbol.Y += 1;
                else if (Direction == Direction.Left) this.Symbol.X -= 1;
                else if (Direction == Direction.Right) this.Symbol.X += 1;

                /* Draw on player standingtile */
                region.SetDrawObject(StandingTile);


                /* Set New StandingTile */
                standingTile = region.Map[Symbol.X, Symbol.Y];
                if (StandingTileAction != null)
                    StandingTileAction(this, new StandingEventArgs(standingTile));
                else
                    DefaultAction();

                /* Draw Object */

                region.SetDrawObject(this);

                Debug.Print(this.objectType + " | [X = " + Symbol.X + " | Y = " + Symbol.Y + "] | Standing Tile = " + standingTile.ObjectType.ToString());
                moves++;
                Debug.Print("Movements: " + moves);

                returncase = true;
            }
            finally { Monitor.Exit("MOVEOBJECT"); }
            return returncase;
        }

        /// <summary>
        /// Compare the standing tile of the object with your 'string' type
        /// </summary>
        /// <param name="gameobjectToCompare">The object type that will be compared with.</param>
        /// <returns>Retruns true if current standing tile is equal to the compared one.</returns>
        public bool IsStandingTile(string gameobjectToCompare)
        {
            if (this.standingTile.ObjectType == gameobjectToCompare)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if the object can move in the direction that was set on.
        /// </summary>
        /// <returns>Returns true if collision is detected.</returns>
        public bool CheckCollision()
        {
            foreach (string o in Obstacle)
            {
                if (Direction == Direction.Up)
                    if (region.Map[Symbol.X, Symbol.Y - 1].ObjectType == o) return false;
                if (Direction == Direction.Down)
                    if (region.Map[Symbol.X, Symbol.Y + 1].ObjectType == o) return false;
                if (Direction == Direction.Right)
                    if (region.Map[Symbol.X + 1, Symbol.Y].ObjectType == o) return false;
                if (Direction == Direction.Left)
                    if (region.Map[Symbol.X - 1, Symbol.Y].ObjectType == o) return false;
            }

            return true;
        }

        /// <summary>
        /// Removes the standing tile (Usualy collects it).
        /// </summary>
        public void Collect()
        {
            this.standingTile = Region.GetTile(region.Air);
            this.standingTile.NewLocation(this.Symbol.X, this.Symbol.Y);
        }

        /// <summary>
        /// Removes the standing tile (Usualy collects it).
        /// </summary>
        public void Collect(Tile replaceTile)
        {
            this.standingTile = Region.GetTile(replaceTile);
            this.standingTile.NewLocation(this.Symbol.X, this.Symbol.Y);
        }

        /// <summary>
        /// Does default action if the user didn't set up the 'StandingTileAction' event.
        /// </summary>
        protected virtual void DefaultAction()
        {

        }
        /// <summary>
        /// Does default collision action if the user didn't set up the 'CollisionAction' event.
        /// </summary>
        protected virtual bool DefaultCollisionAction()
        {
            return true;
        }

        /// <summary>
        /// Changes the character to a new one.
        /// </summary>
        /// <param name="newCharacter">The new character to be set.</param>
        public void ChangeCharacter(char newCharacter)
        {
            this.Symbol.Character = newCharacter;
        }
        /// <summary>
        /// Changes the character to a new one.
        /// </summary>
        /// <param name="newCharacter">The new character to be set.</param>
        /// <param name="foreground">Foreground of the new character.</param>
        /// <param name="background">Background of the new character.</param>
        public void ChangeCharacter(char newCharacter, ConsoleColor foreground, ConsoleColor background)
        {
            this.Symbol.ForeColor = foreground;
            this.Symbol.BackColor = background;

            this.Symbol.Character = newCharacter;
        }

        /// <summary>
        /// Set new location by X, Y. Doesn't draw the character.
        /// </summary>
        /// <param name="X">New X location.</param>
        /// <param name="Y">New Y location.</param>
        public override void NewLocation(int X, int Y)
        {
            standingTile = new Tile(
                Char.Parse(region.mapConfig[0, 4]),
                region.mapConfig[0, 7],
                X, Y,
                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((region.mapConfig[0, 5] == "None") ? region.ForeColor.ToString() : region.mapConfig[0, 5])),
                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((region.mapConfig[0, 6] == "None") ? region.BackColor.ToString() : region.mapConfig[0, 6])));

            base.NewLocation(X, Y);
        }


        GameObject GetObjectFromDirection(Direction dir)
        {
            GameObject gameObject = null;

            switch (dir)
            {
                case Direction.Up: 
                    gameObject = region.Map[this.Symbol.X, this.Symbol.Y - 1]; break;
                case Direction.Down: 
                    gameObject = region.Map[this.Symbol.X, this.Symbol.Y + 1]; break;
                case Direction.Right: 
                    gameObject = region.Map[this.Symbol.X + 1, this.Symbol.Y]; break;
                case Direction.Left: 
                    gameObject = region.Map[this.Symbol.X - 1, this.Symbol.Y]; break;

            }

            return gameObject;
        }

        public GameObject Next(Direction direction)
        {
            GameObject nextStanding = null;

            switch (direction)
            {
                case Direction.Up: 
                    nextStanding = region.Map[this.Symbol.X, this.Symbol.Y - 1]; 
                break;
                case Direction.Down: 
                    nextStanding = (region.Map[this.Symbol.X, this.Symbol.Y + 1]);
                break;
                case Direction.Right:
                    nextStanding = (region.Map[this.Symbol.X + 1, this.Symbol.Y]);
                break;
                case Direction.Left:
                    nextStanding = (region.Map[this.Symbol.X - 1, this.Symbol.Y]); 
                break;
            }

            return nextStanding;
        }
        public GameObject Next(Direction direction, ref int x, ref int y)
        {
            GameObject nextStanding = null;
            
            switch (direction)
            {
                case Direction.Up:
                    nextStanding = region.Map[this.Symbol.X, this.Symbol.Y - 1];
                    y = -1;
                    x = 0;
                    break;
                case Direction.Down:
                    nextStanding = (region.Map[this.Symbol.X, this.Symbol.Y + 1]);
                    y = 1;
                    x = 0;
                    break;
                case Direction.Right:
                    nextStanding = (region.Map[this.Symbol.X + 1, this.Symbol.Y]);
                    y = 0;
                    x = 1;
                    break;
                case Direction.Left:
                    nextStanding = (region.Map[this.Symbol.X - 1, this.Symbol.Y]);
                    x = -1;
                    y = 0;
                    break;
            }

            return nextStanding;
        }
    }
}