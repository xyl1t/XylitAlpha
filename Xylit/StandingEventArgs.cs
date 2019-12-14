using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xylit.Playground.GameObjects;

namespace Xylit
{
    class StandingEventArgs
    {
        GameObject standingTile;
        public GameObject StandingTile { get { return standingTile; } }
        public StandingEventArgs(GameObject standingTile)
        {
            this.standingTile = standingTile;
        }
    }
}
