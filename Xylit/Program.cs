using System;
using System.Collections.Generic;
using System.Text;
using Xylit;
using Xylit.Playground;
using Xylit.Playground.GameObjects;
using Xylit.UI;
using System.Threading;
using System.Diagnostics;


namespace Xylit
{
    class Program: Game
    {
        static Screen screen;
        static Region region;
        static MoveableObject player;
        static List<MoveableObject> nonfixObject = new List<MoveableObject>();

        static MoveableObject bullet;


        static int moves, oldMoves;
        static int oldScore, newScore;
        static byte health = 15, oldHealth = 15;
        static bool shoot;
        static bool hasPistol;
        static byte bullets, oldBullets;


        static bool Alive = true;


        [STAThread]
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            
            Console.Title = "XYLIT v.a.1.0";
            Game.CursorVisible = false;
            screen = new Screen(120, 60);
            screen.BackColor = ConsoleColor.Black;
            screen.ForeColor = ConsoleColor.White;

            screen.ResetScreen();

            screen.AcceptChanges();

            region = new Region(screen);
            string path = Environment.CurrentDirectory;
            Console.Write("Map config name: ");
            path += @"\Resources\" + Console.ReadLine();
            region.LoadXmlConfig(path, false);
            region.LoadMap();
            screen.AcceptChanges();

            string text = 
            "Legend:\n\n"+
            "To move your\n"+
            "Hero press the\n"+
            "Arrow Keys:\n"+
            "\n← ↑ → ↓\n\n"+
            "The action\n"+
            "button is\n"+
            "Spacebar ████"+
            "\n\n--------------\n\n"+
            "If you hit an\n"+
            "enemy you will\n" +
            "loose -1 HP\n\n" + 
            "In order to \n"+
            "get HP back \n"+
            "collect those \n"+
            "hearts ♥\n\n"+
            "To kill an\n"+
            "enemy shoot at\n"+
            "him with your\n"+
            "pistol ╓\n"+
            "get +5 ammo\n"+
            "with these: ░";


            Window infoWindow = new Window(screen, 101, 1, 18, 33, "LEGEND", text, ConsoleColor.White, ConsoleColor.DarkBlue, ConsoleColor.Yellow, ConsoleColor.White);
            Window status = new Window(screen, 101, 35, 18, 24, "STATUS", "Health: 15\n\nScore: 0\n\nBullets: 0\n\n\n\n\n\n\n\n\n\n\n\n\n--------------\n\nXylit by Marat", ConsoleColor.White, ConsoleColor.DarkBlue, ConsoleColor.Yellow, ConsoleColor.White);

            infoWindow.Show();
            status.Show();
            screen.AcceptChanges();


            player = new MoveableObject(region, ' ', "Player", 2, 2, ConsoleColor.Green, ConsoleColor.Black, new string[] { "Fixed", "Enemy", "ClosedDoor"});
            player.CollisionAction +=new CollisionAction(player_CollisionAction);
            player.StandingTileAction += new StandingAction(player_StandingTileAction);

            bullet = new MoveableObject(region, '°', "bullet", player.Symbol.X, player.Symbol.Y, ConsoleColor.White, ConsoleColor.Black, new string[] { "Fixed", "Enemy", "ClosedDoor", "Barrel" });
            bullet.Name = "MO_projectile" + (nonfixObject.Count - 1).ToString();
            bullet.CollisionAction +=new CollisionAction(projectile_CollisionAction);



            GameObject Temp = null;
            int numb = 0;
            for(int fx = 0; fx < region.Width; fx++)
                for (int fy = 0; fy < region.Height; fy++)
                {
                    Temp = region.Map[fx, fy];
                    if (Temp.ObjectType == "Enemy")
                    {
                        nonfixObject.Add(new MoveableObject(
                            region,
                            Temp.Symbol.Character,
                            Temp.ObjectType,
                            Temp.Symbol.X, Temp.Symbol.Y,
                            Temp.Symbol.ForeColor, Temp.Symbol.BackColor,
                            new string[] { "Player", "Enemy", "Fixed", "ClosedDoor", "Barrel" }));
                        nonfixObject[nonfixObject.Count - 1].Name = "MO_Enemy"+(nonfixObject.Count-1).ToString();
                        nonfixObject[nonfixObject.Count - 1].CollisionAction += new CollisionAction(Enemy_CollisionAction);
                    }
                    else if (Temp.ObjectType == "Barrel")
                    {
                        nonfixObject.Add(new MoveableObject(region, Temp.Symbol, "Barrel", new string[] { "Fixed", "ClosedDoor", "Enemy", "Player", "Barrel" }));
                        region.Map[fx, fy] = new MoveableObject(region, Temp.Symbol, "Barrel", new string[] { "Fixed", "ClosedDoor", "Enemy", "Player", "Barrel" });
                        nonfixObject[nonfixObject.Count - 1].Name = "MO_Barrel"+numb++.ToString();

                    }
                    else if (Temp.ObjectType == "Player")
                    {
                        player.Symbol = Temp.Symbol;
                    }
                }
            nonfixObject.Add(bullet);

            Thread keyPress = new Thread(KeyPressHandler);
            Thread loop = new Thread(loopMethod);

              region.Draw();
            screen.AcceptChanges();

            int width = 34, height = 5;
            int x = (region.Width / 2) - (width / 2), y = (region.Height / 2 - height / 2) - 5;

            
            Window waitingInfo = new Window(screen, x, y, width, height, "Game", "Press any key to start the game");
            waitingInfo.Show();
            Console.ReadKey(true);

            screen.RejectChanges();
            screen.DrawArea(waitingInfo.X, waitingInfo.Y, waitingInfo.Width, waitingInfo.Height);
            screen.AcceptChanges();

            loop.Start();
            keyPress.Priority = ThreadPriority.Highest;
            keyPress.Start();

        }





        static Random random = new Random();
        static void loopMethod()
        {
            int randomNumb = 0;

            Stopwatch w = new Stopwatch();
            w.Start();
            int fps = 60;
            float timePerTick = 1000 / fps;
            float delta = 0;
            long now = 0;
            long last = w.ElapsedMilliseconds;
            long elapsedTime = 0;

            while (Alive)
            {
                lock("LOOP")
                {
                    now = w.ElapsedMilliseconds;
                    delta += (now - last) / timePerTick;
                    elapsedTime += now - last;
                    last = now;

                    for (int i = nonfixObject.Count - 1; i >= 0; i--)
                    {
                        if (delta >= 10)
                            if (nonfixObject[i].Name.StartsWith("MO_Enemy"))
                            {
                                randomNumb = random.Next(0, 4);

                                if (randomNumb == 2)
                                    nonfixObject[i].MoveDraw(Direction.Up);
                                else if (randomNumb == 3)
                                    nonfixObject[i].MoveDraw(Direction.Down);
                                else if (randomNumb == 0)
                                    nonfixObject[i].MoveDraw(Direction.Right);
                                else if (randomNumb == 1)
                                    nonfixObject[i].MoveDraw(Direction.Left);
                            }

                        if (nonfixObject[i].Name.StartsWith("MO_projectile") && shoot && bullets > 0 && hasPistol)
                        {
                            if (delta >= 1)
                            {
                                if (!nonfixObject[i].MoveDraw(nonfixObject[i].Direction))
                                {
                                    region.SetDrawObject(nonfixObject[i].StandingTile);
                                    shoot = false;
                                }
                            }
                        }
                    }
                    if (delta >= 1)
                    {
                        string sElapsedTime = (elapsedTime / 1000).ToString();
                        //screen.Set(sElapsedTime.ToString() + "   ", 109, 52);
                        //screen.DrawArea(109, 52, sElapsedTime.ToString().Length, 1);
                        
                        Console.Title = "XYLIT | Time: " + sElapsedTime;
                    }
                    if (delta >= 10)
                        delta = 0;
                    Thread.Sleep(1);

                }
            }
        }

        static bool player_CollisionAction(MoveableObject sender, CollisionEventArgs e)
        {
            bool allow = true;
            if (e.CollidingObject.ObjectType == "Barrel")
            {
                (e.CollidingObject as MoveableObject).Direction = e.Direction;
                if (!(e.CollidingObject as MoveableObject).CheckCollision())
                    allow = false;
                (e.CollidingObject as MoveableObject).MoveDraw(e.Direction);
            }
            else if (e.CollidingObject.ObjectType == "Enemy")
            {
                if (health > 1)
                {
                    health--;
                    UpdateHealth();
                }
                else
                {
                    health--;
                    Alive = false;
                    UpdateHealth();
                    int width = 13, height = 5;
                    int x = region.Width / 2 - width / 2, y = (region.Height / 2 - height / 2) - 5;

                    Window gameOver = new Window(screen, x, y, width, height, "Game", "YOU LOOSE", ConsoleColor.White, ConsoleColor.Black, ConsoleColor.DarkRed, ConsoleColor.Red);
                    gameOver.Show();
                    Console.ReadKey(true);
                }
            }
            return allow;
        }
        static bool Enemy_CollisionAction(MoveableObject sender, CollisionEventArgs e)
        {
            bool allow = true;

            if (e.CollidingObject.ObjectType == "Player")
            {
                if (health > 1)
                {
                    health--;
                    UpdateHealth();

                }
                else
                {
                    health--;
                    Alive = false;
                    UpdateHealth();
                    int width = 13, height = 5;
                    int x = region.Width / 2 - width, y = (region.Height / 2 - height / 2) - 5;

                    Window gameOver = new Window(screen, x, y, width, height, "Game", "YOU LOOSE", ConsoleColor.White, ConsoleColor.Black, ConsoleColor.Yellow, ConsoleColor.Red);
                    gameOver.Show();
                    Console.ReadKey(true);
                }
                allow = false;
            }
            else if (e.CollidingObject.ObjectType == "projectile")
            {
                MoveableObject currentEnemy = sender as MoveableObject;
                int index = Int32.Parse(currentEnemy.Name.Split('y')[1]);
                currentEnemy.ChangeBackColor(ProChar.GetInvertedColor(e.CollidingObject.Symbol.ForeColor));
                currentEnemy.ChangeBackColor(ProChar.GetInvertedColor(e.CollidingObject.Symbol.BackColor));

                region.SetDrawObject(currentEnemy.StandingTile);
                nonfixObject[index].Name = "";
                allow = false;
            }

            return allow;
        }
        static bool projectile_CollisionAction(MoveableObject sender, CollisionEventArgs e)
        {
            bool allow = true;
            try
            {
                if (e.CollidingObject.Name.StartsWith("MO_Enemy"))
                {
                    int index = Int32.Parse(e.CollidingObject.Name.Split('y')[1]);
                    e.CollidingObject.ChangeBackColor(ProChar.GetInvertedColor(e.CollidingObject.Symbol.ForeColor));
                    e.CollidingObject.ChangeBackColor(ProChar.GetInvertedColor(e.CollidingObject.Symbol.BackColor));

                    region.SetDrawObject(((MoveableObject)e.CollidingObject).StandingTile);
                    nonfixObject[index].Name = "";
                    allow = false;
                }
            }
            catch { return allow; }
            return allow;
        }

        static void player_StandingTileAction(MoveableObject sender, StandingEventArgs e)
        {
            if (e.StandingTile.ObjectType == "Pistol")
            {
                shoot = false;
                hasPistol = true;
                bullets += 5;

                UpdateBullets();

                player.Collect();
            }
            else if (e.StandingTile.ObjectType == "Amunition" && hasPistol)
            {
                shoot = false;
                bullets += 5;

                UpdateBullets();

                player.Collect();
            }
            else if (e.StandingTile.ObjectType == "Coin")
            {
                newScore++;

                UpdateScore();

                player.Collect();
            }
            else if (e.StandingTile.ObjectType == "SuperCoin")
            {
                newScore += 5;

                UpdateScore();

                player.Collect();
            }
            else if (e.StandingTile.ObjectType == "Health")
            {
                if(health < 15)
                {
                    health++;
                    player.Collect();
                }

                UpdateHealth();

            }
            else if (e.StandingTile.ObjectType == "Goal")
            {
                Alive = false;

                int width = 13, height = 5;
                int x = region.Width / 2 - width / 2, y = (region.Height / 2 - height / 2) - 5;

                Window youWin = new Window(screen, x, y, width, height, "Game", "YOU WIN", ConsoleColor.White, ConsoleColor.Black, ConsoleColor.DarkGreen, ConsoleColor.White);
                youWin.Show();
                Console.ReadKey(true);
            }
            //moves++;
            //UpdateMoves();
            // /-| X... |-\
        }


        static void KeyPressHandler()
        {
            ConsoleKeyInfo input = new ConsoleKeyInfo();
            while (Alive)
            {
                input = Game.GetKeyPress(false);

                lock("MOVING")
                {
                    if (input.Key == ConsoleKey.UpArrow)
                    {
                        player.MoveDraw(Direction.Up);
                    }
                    else if (input.Key == ConsoleKey.DownArrow)
                    {
                        player.MoveDraw(Direction.Down);
                    }
                    else if (input.Key == ConsoleKey.RightArrow)
                    {
                        player.MoveDraw(Direction.Right);
                    }
                    else if (input.Key == ConsoleKey.LeftArrow)
                    {
                        player.MoveDraw(Direction.Left);
                    }

                    else if (input.Key == ConsoleKey.Spacebar)
                    {
                        int tempX = 0, tempY = 0;
                        if (player.Next(player.Direction, ref tempX, ref tempY).ObjectType == "ClosedDoor")
                        {
                            ((Tile)region.Map[player.Symbol.X + tempX, player.Symbol.Y + tempY]).ChangeType("OpenedDoor");
                            ((Tile)region.Map[player.Symbol.X + tempX, player.Symbol.Y + tempY]).ChangeCharacter('/');
                            ((Tile)region.Map[player.Symbol.X + tempX, player.Symbol.Y + tempY]).Symbol.Draw();


                        }
                        else if (player.Next(player.Direction, ref tempX, ref tempY).ObjectType == "OpenedDoor")
                        {
                            ((Tile)region.Map[player.Symbol.X + tempX, player.Symbol.Y + tempY]).ChangeType("ClosedDoor");
                            ((Tile)region.Map[player.Symbol.X + tempX, player.Symbol.Y + tempY]).ChangeCharacter('%');
                            ((Tile)region.Map[player.Symbol.X + tempX, player.Symbol.Y + tempY]).Symbol.Draw();
                        }
                        else 
                        {
                            bullet.Symbol.X = player.Symbol.X;
                            bullet.Symbol.Y = player.Symbol.Y;
                            bullet.Direction = player.Direction;
                            
                            if (bullets > 0) bullets--;
                            
                            UpdateBullets();
                            shoot = true;
                        }
                    }
                }
            }
        }

        #region Update Status
        static void UpdateHealth()
        {
            lock ("UPDATEHEALTH")
            {
                screen.Set(health.ToString() + "   ", 111, 37, ConsoleColor.White, ConsoleColor.DarkBlue);

                for (int x = 111; x < 115; x++)
                    screen.screen[x, 37].Draw();
            }
        }

        static void UpdateScore()
        {
            lock ("UPDATESCORE")
            {
                screen.Set(newScore.ToString() + "   ", 110, 39, ConsoleColor.White, ConsoleColor.DarkBlue);

                for (int x = 110; x < 114; x++)
                    screen.screen[x, 39].Draw();
            }
        }

        static void UpdateBullets()
        {
            lock ("UPDATEBULLETS")
            {
                screen.Set(bullets.ToString() + "   ", 112, 41, ConsoleColor.White, ConsoleColor.DarkBlue);

                for (int x = 112; x < 114; x++)
                    screen.screen[x, 41].Draw();
            }
        }

        static void UpdateMoves()
        {
            lock ("UPDATEMOVES")
            {
                screen.Set(moves.ToString(), 110, 43, ConsoleColor.White, ConsoleColor.DarkBlue);

                // IMPROVE make better method for drawing one line
                for (int x = 110; x < 110 + moves.ToString().Length; x++)
                    screen.screen[x, 43].Draw();

                //Console.SetCursorPosition(110, 43);
                //Console.Write(moves);
            }
        }
        #endregion



        
    }
}
