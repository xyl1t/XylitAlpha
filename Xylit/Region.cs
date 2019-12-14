using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using Xylit.Playground.GameObjects;
using System.Xml;
using System.Diagnostics;
using Xylit.UI;

namespace Xylit.Playground
{
    static class GameObjectTypeCollection
    {
        static SortedList gameObjectType = new SortedList();
        public static SortedList GameObjectType { get { return gameObjectType; } }
    }

    class Region
    {
        /* VARIABLES / PROPERTIES */
        public SortedList Tiles = new SortedList();
        public Tile Air;

        string configPath;
        string mapPath;

        int x;
        int y;
        public int X { get { return x; } }
        public int Y { get { return y; } }

        int width;
        int height;
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        GameObject[,] map;
        public GameObject[,] Map { get { return map; } }

        public string[,] mapConfig;

        ConsoleColor foreColor;
        ConsoleColor backColor;
        public ConsoleColor ForeColor { get { return foreColor; } }
        public ConsoleColor BackColor { get { return backColor; } }
        Screen screen;


        public Region(Screen screen) : this(screen, 0, 0) { }
        public Region(Screen screen, int x, int y) : this(screen, x, y, 0, 0) { }
        public Region(Screen screen, int x, int y, int width, int height)
        {
            this.screen = screen;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            this.map = new GameObject[width, height];
        }

        public static Tile GetTile(Tile tile)
        {
            return new Tile(tile.Symbol.Character, tile.ObjectType, tile.Symbol.X, tile.Symbol.Y, tile.Symbol.ForeColor, tile.Symbol.BackColor);
        }

        public static void ChangeTile(ref GameObject oldTile, GameObject newTile)
        {
            ((Tile)(oldTile)).ChangeCharacter(newTile.Symbol.Character);
            ((Tile)(oldTile)).ChangeForeColor(newTile.Symbol.ForeColor);
            ((Tile)(oldTile)).ChangeBackColor(newTile.Symbol.BackColor);
            ((Tile)(oldTile)).ChangeType(newTile.ObjectType);
            ((Tile)(oldTile)).NewLocation(newTile.Symbol.X, newTile.Symbol.Y);
        }

        public void SetNewLocation(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetNewDimensions(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void Remove(GameObject obj)
        {
            map[obj.Symbol.X, obj.Symbol.Y] = null;
        }

        public void Replace(GameObject obj1, GameObject obj2)
        {
            /* update objects */
            int x1 = obj1.Symbol.X, y1 = obj1.Symbol.Y;
            int x2 = obj2.Symbol.X, y2 = obj2.Symbol.Y;

            obj1.NewLocation(x2, y2);
            obj2.NewLocation(x1, y1);

            /* update region */
            map[x1, y1] = obj2;
            map[x2, y2] = obj1;
        }

        public static void Replace(Region region, GameObject obj1, GameObject obj2)
        {
            /* update region */
            region.Replace(obj1, obj2);
        }

        public void SetObject(GameObject obj)
        {
            map[obj.Symbol.X, obj.Symbol.Y] = obj;
        }
        public void SetDrawObject(GameObject obj)
        {
            map[obj.Symbol.X, obj.Symbol.Y] = obj;
            screen.Set(obj.Symbol);
            obj.Symbol.Draw(this.X, this.Y);
        }

        public virtual void LoadXmlConfig(string configPath, bool showInfo)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configPath);

            foreach (XmlNode node in xmlDoc.DocumentElement)
            {
                if (node.Name == "Config")
                {
                    XmlNode Coordinates = node["Coordinates"];
                    XmlNode DefaultColors = node["DefaultColors"];
                    XmlNode MapPath = node["Path"];

                    this.x = Int32.Parse(Coordinates.Attributes["X"].InnerText);
                    this.y = Int32.Parse(Coordinates.Attributes["Y"].InnerText);
                    this.mapPath = MapPath.Attributes["MapPath"].InnerText;

                    this.foreColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), DefaultColors.Attributes["ForeColor"].InnerText);
                    this.backColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), DefaultColors.Attributes["BackColor"].InnerText);
                }
                else if (node.Name == "Tiles")
                {
                    XmlNode Name;
                    XmlNode Color;
                    XmlNode TileColor;
                    XmlNode TileType;
                    XmlNode BlockPlayer;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;

                    mapConfig = new string[(node.ChildNodes.Count / 2), 9];


                    int index = 0;
                    foreach (XmlNode Tile in node.ChildNodes)
                    {
                        if (Tile.NodeType != XmlNodeType.Comment)
                        {
                            Name = Tile["Name"];
                            TileColor = Tile["TileColor"];
                            TileType = Tile["TileType"];
                            BlockPlayer = Tile["BlockPlayer"];

                            if (showInfo)
                            {
                                Console.WriteLine(Name.InnerText);
                                Console.WriteLine(Tile.Attributes["character"].InnerText);
                            }
                            Color = Tile["CheckColor"];
                            if (showInfo)
                            {
                                foreach (XmlNode ColorRGB in Color.ChildNodes)
                                    Console.Write(ColorRGB.InnerText + " ");
                                Console.WriteLine();
                            }

                            if (showInfo)
                            {
                                Console.WriteLine(TileColor.Attributes["ForeColor"].InnerText + " | " + TileColor.Attributes["BackColor"].InnerText);
                                Console.WriteLine(TileType.Attributes["Type"].InnerText);
                                Console.WriteLine(BlockPlayer.Attributes["Block"].InnerText);

                                Console.WriteLine();
                            }

                            mapConfig[index, 0] = Name.InnerText;
                            mapConfig[index, 1] = Color["R"].InnerText;
                            mapConfig[index, 2] = Color["G"].InnerText;
                            mapConfig[index, 3] = Color["B"].InnerText;

                            mapConfig[index, 4] = Tile.Attributes["character"].InnerText;
                            mapConfig[index, 5] = TileColor.Attributes["ForeColor"].InnerText;
                            mapConfig[index, 6] = TileColor.Attributes["BackColor"].InnerText;
                            mapConfig[index, 7] = TileType.Attributes["Type"].InnerText;

                            GameObjectTypeCollection.GameObjectType[mapConfig[index, 0]] = mapConfig[index, 7];

                            if (mapConfig[index, 7] == "Air" && Air == null)
                                Air = new Tile(
                                    Char.Parse(mapConfig[index, 4]),
                                     mapConfig[index, 7],
                                    0,
                                    0,
                                    (ConsoleColor)Enum.Parse(typeof(ConsoleColor), (mapConfig[index, 5] == "None") ? foreColor.ToString() : mapConfig[index, 5]),
                                    (ConsoleColor)Enum.Parse(typeof(ConsoleColor), (mapConfig[index, 6] == "None") ? backColor.ToString() : mapConfig[index, 6]));

                            
                                Tiles.Add(mapConfig[index, 0], new Tile(
                                    Char.Parse(mapConfig[index, 4]),
                                    mapConfig[index, 7],
                                    0,
                                    0,
                                    (ConsoleColor)Enum.Parse(typeof(ConsoleColor), (mapConfig[index, 5] == "None") ? foreColor.ToString() : mapConfig[index, 5]),
                                    (ConsoleColor)Enum.Parse(typeof(ConsoleColor), (mapConfig[index, 6] == "None") ? backColor.ToString() : mapConfig[index, 6])));
                            
                           
                            index++;

                        }
                    }
                }
            }
        }

        void LoadConfig(string configPath)
        {
            // UNDONE: EXCEPTION HANDELING on normal load config method

            this.configPath = configPath;

            using (StreamReader reader = new StreamReader(configPath))
            {
                string all = reader.ReadToEnd().Replace("\r\n", "");

                string[] OverInfo = all.Split('~');
                string[] CoordinateInfo = OverInfo[0].Split(',');
                string[] ColorInfo = OverInfo[1].Split(';');
                string[] MainInfo = OverInfo[2].Split(';');

                #region set coordinates

                this.x = Int32.Parse(CoordinateInfo[0]);
                this.y = Int32.Parse(CoordinateInfo[1]);

                #endregion

                #region set default colors

                for (int i = 0; i < ColorInfo.Length; i++)
                {
                    if (ColorInfo[i].Contains("Foreground"))
                        this.foreColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ColorInfo[i].Split('=')[1]);
                    else if (ColorInfo[i].Contains("Background"))
                        this.backColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ColorInfo[i].Split('=')[1]);

                }

                #endregion

                #region set main information

                mapConfig = new string[MainInfo.Length, 8];

                for (int i = 0; i < MainInfo.Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                        mapConfig[i, j] = MainInfo[i].Split(new string[] { ",", "=", ";" }, StringSplitOptions.None)[j];
                }

                #endregion
            }

        }

        public virtual void LoadMap(string mapPath)
        {
            if (string.IsNullOrEmpty(this.mapPath))
                this.mapPath = mapPath;
            Bitmap loadedMap = (Bitmap)Bitmap.FromFile(this.mapPath);
            Color pixel;

            this.width = loadedMap.Width;
            this.height = loadedMap.Height;
            map = new GameObject[Width, Height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int i = 0; i <= mapConfig.GetUpperBound(0); i++)
                    {
                        pixel = loadedMap.GetPixel(x, y);

                        if (pixel == Color.FromArgb(
                            Int32.Parse(mapConfig[i, 1]),   /* RED */
                            Int32.Parse(mapConfig[i, 2]),   /* GREEN */
                            Int32.Parse(mapConfig[i, 3])))  /* BLUE */
                        {
                            map[x, y] = new Tile(Char.Parse(mapConfig[i, 4]),
                                mapConfig[i, 7],
                                x, y,
                                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((mapConfig[i, 5] == "None") ? ForeColor.ToString() : mapConfig[i, 5])),
                                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((mapConfig[i, 6] == "None") ? BackColor.ToString() : mapConfig[i, 6])));

                        }
                        else if (i == mapConfig.Length / 7 - 1 && map[x, y] == null)
                            map[x, y] = new Tile('?', "Air", x, y, ConsoleColor.Black, ConsoleColor.White);

                    }
                }
            }
        }
        public virtual void LoadMap()
        {
            string path = Environment.CurrentDirectory;
            path += @"\Resources\" + mapPath;
            Bitmap loadedMap = (Bitmap)Bitmap.FromFile(path);
            Color pixel;

            this.width = loadedMap.Width;
            this.height = loadedMap.Height;
            map = new GameObject[Width, Height];
            ProChar[,] tScreen = new ProChar[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int i = 0; i <= mapConfig.GetUpperBound(0); i++)
                    {
                        pixel = loadedMap.GetPixel(x, y);

                        if (pixel == Color.FromArgb(
                            Int32.Parse(mapConfig[i, 1]),   /* RED */
                            Int32.Parse(mapConfig[i, 2]),   /* GREEN */
                            Int32.Parse(mapConfig[i, 3])))  /* BLUE */
                        {
                            map[x, y] = new Tile(Char.Parse(mapConfig[i, 4]),
                                mapConfig[i, 7],
                                x, y,
                                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((mapConfig[i, 5] == "None") ? ForeColor.ToString() : mapConfig[i, 5])),
                                (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ((mapConfig[i, 6] == "None") ? BackColor.ToString() : mapConfig[i, 6])));

                        }
                        else if (i == mapConfig.Length / 7 - 1 && map[x, y] == null)
                            map[x, y] = new Tile('?', "Air", x, y, ConsoleColor.Black, ConsoleColor.White);


                    }
                    tScreen[x, y] = map[x, y].Symbol;
                }
            }
            screen.Set(tScreen);
        }

        public void Draw()
        {
            screen.DrawArea(this.x, this.y, this.width, this.height);
            screen.ResetScreen();
        }

        public void Draw(int startingX, int startingY, int width, int height)
        {
            if (startingX < 0) startingX = 0;
            if (startingY < 0) startingY = 0;
            if (width + startingX >= this.Width) width = this.Width - startingX;
            if (height + startingY >= this.Height) height = this.Height -startingY;

            screen.DrawArea(startingX, startingY, width, height);
            screen.ResetScreen();
        }
    }
}