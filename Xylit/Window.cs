using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xylit.UI
{
    class Window
    {
        protected int x, y;
        // UNDONE: Seter for "x" and "y" !
        public int X { get { return x; } }
        public int Y { get { return y; } }
        protected int originalWidth, originalHeight;
        protected int editedWidth, editedHeight;
        public int Width 
        { 
            get { return editedWidth; }

            set 
            {
                editedWidth = originalWidth = (value < 12) ? 12 : value;
            }
        }
        public int Height
        {
            get { return editedHeight; }

            set 
            {
                editedHeight = originalHeight = (value < 5) ? 12 : value;
            }
        }
        
        protected string finalTitle;
        public string Title 
        { 
            get { return finalTitle; } 
            set 
            { 
                finalTitle = value;

                CompressTitle(CompressedTitle);
                Warptext(WarpText);
            }
        }
        protected string originalTitle;
        protected string originalText;
        protected string[] text;
        public string Text
        {
            get { return originalText; }
            set 
            {
                originalText = value;

                CompressTitle(CompressedTitle);
                Warptext(WarpText);
            }
        }
        protected ConsoleColor borderColor = ConsoleColor.White;
        protected ConsoleColor titleColor = ConsoleColor.White;
        protected ConsoleColor textColor = ConsoleColor.White;
        protected ConsoleColor backColor = ConsoleColor.Black;

        protected Screen screen;

        protected ProChar[,] window;            
        
        /*  0 1 2 3 4 5 6 7 8 9 10 */
        /*  ─ │ ┌ ┐ └ ┘ ├ ┤ ┬ ┴ ┼  */
        /*  ═ ║ ╔ ╗ ╚ ╝ ╠ ╣ ╦ ╩ ╬  */
        #region borders
        protected ProChar[] Borders = { 
            new ProChar('─'), new ProChar('│'), new ProChar('┌'), new ProChar('┐'), 
            new ProChar('└'), new ProChar('┘'), new ProChar('├'), new ProChar('┤'), 
            new ProChar('┬'), new ProChar('┴'), new ProChar('┼'), 

            new ProChar('═'), new ProChar('║'), new ProChar('╔'), new ProChar('╗'),
            new ProChar('╚'), new ProChar('╝'), new ProChar('╠'), new ProChar('╣'),
            new ProChar('╦'), new ProChar('╩'), new ProChar('╬'), 
            
            new ProChar(' ')};
        #endregion

        bool warpText = false;
        bool compressedTitle = true;
        public bool WarpText
        {
            get { return warpText; }
            set
            {
                warpText = value;

                Warptext(value);
                CompressTitle(compressedTitle);

                loadBorder();
                loadTitle();
                loadText();
            }
        }
        public bool CompressedTitle
        {
            get { return compressedTitle; }
            set 
            {
                compressedTitle = value;
                CompressTitle(value);
                Warptext(warpText);

                loadBorder();
                loadTitle();
                loadText();
            }
        }

        // /-| X... |-\
        public Window() { }
        public Window(Screen screen, int x, int y, int width, int height, string title, string text) 
            : this(screen, x, y, width, height, title, text, screen.ForeColor, screen.BackColor, screen.ForeColor, screen.ForeColor) { }
        public Window(Screen screen, int x, int y, int width, int height, string title, string text, ConsoleColor BorderColor, ConsoleColor BackColor, ConsoleColor TitleColor, ConsoleColor TextColor)
        {
            this.screen = screen;
            this.x = x;
            this.y = y;

            this.borderColor = BorderColor;
            this.titleColor = TitleColor;
            this.textColor = TextColor;
            this.backColor = BackColor;

            this.finalTitle = title;
            this.originalTitle = title;
            this.originalText = text;

            this.originalWidth = this.editedWidth = (width < 12) ? 12 : width;
            this.originalHeight = this.editedHeight = (height < 5) ? 5 : height;

            Warptext(warpText);
            CompressTitle(compressedTitle);
            
            loadBorder();
            loadTitle();
            loadText();
        }


        protected void Warptext(bool value)
        {
            string editedText = originalText;

            if (value)
            {
                for (int i = 1; i <= editedText.Length - 1; i++)
                {
                    if (i % (this.originalWidth - 4)  == 0)
                        editedText = editedText.Insert(i + ((i / (originalWidth - 4)) - 1), "\n");
                }

            }

            this.text = editedText.Split(new string[] { "\n" }, StringSplitOptions.None);

            int t_longestLine = 0;

            for (int i = 0; i < this.text.Length; i++)
                if (t_longestLine < this.text[i].Length && this.editedWidth < this.text[i].Length + 4)
                {
                    t_longestLine = this.text[i].Length;
                    this.editedWidth = t_longestLine + 4;
                }

            if (this.editedHeight < this.text.Length + 4)
                this.editedHeight = this.text.Length + 4;


            window = new ProChar[editedWidth, editedHeight];
        }
        protected void CompressTitle(bool value)
        {
            if (value)
            {
                if (originalTitle.Length + 8 > editedWidth)
                    this.finalTitle = this.finalTitle.Substring(0, editedWidth - 11) +"...";
            }
            else
            {
                this.finalTitle = originalTitle;
                if (originalTitle.Length + 8 > editedWidth)
                    this.editedWidth = 8 + finalTitle.Length;
            }

            window = new ProChar[this.editedWidth, this.editedHeight];
        }


        protected void loadText()
        {
            for (int i = 0; i < this.text.Length; i++)
            {
                for (int j = 0; j < this.text[i].Length; j++)
                {
                    window[2 + j, 2 + i].CopySymbol(new ProChar(this.text[i][j], textColor, backColor));
                    window[2 + j, 2 + i].ForeColor = textColor;
                }
            }
        }

        protected void loadTitle()
        {
            int leftMagrin = 2;
            for (int fx = leftMagrin; fx < this.finalTitle.Length  + leftMagrin + 4; fx++)
            {
                window[fx, 0].CopySymbol(Borders[22]);
                if (fx > 3  && fx < finalTitle.Length - 1 + leftMagrin + 3)
                {
                    window[fx, 0].CopySymbol(new ProChar(Char.Parse(finalTitle[fx - leftMagrin - 2].ToString()), ConsoleColor.White, ConsoleColor.Black));
                    window[fx, 0].ForeColor = titleColor;
                    window[fx, 0].BackColor = ProChar.GetInvertedColor(titleColor);
                }
                if (fx == 0 + leftMagrin)
                {
                    window[fx, 0].CopySymbol(Borders[7 + 11]);

                }
                if (fx == finalTitle.Length -1  + leftMagrin + 4)
                {
                    window[fx, 0].CopySymbol(Borders[6 + 11]);
                }
            }
        }

        protected void loadBorder()
        {
            for (int fx = 0; fx < this.editedWidth; fx++)
            {
                for (int fy = 0; fy < this.editedHeight; fy++)
                {
                    window[fx, fy].X = fx + this.x;
                    window[fx, fy].Y = fy + this.y;
                    window[fx, fy].ForeColor = borderColor;

                    if (fx == 0 && fy == 0)
                        window[fx, fy].CopySymbol(Borders[2 + 11]);
                    else if (fx == editedWidth - 1 && fy == 0)
                        window[fx, fy].CopySymbol(Borders[3 + 11]);
                    else if (fx == 0 && fy == editedHeight - 1)
                        window[fx, fy].CopySymbol(Borders[4 + 11]);
                    else if (fx == editedWidth - 1 && fy == editedHeight - 1)
                        window[fx, fy].CopySymbol(Borders[5 + 11]);

                    else if (fx > 0 && fx < editedWidth - 1&& (fy == 0 || fy == editedHeight - 1))
                        window[fx, fy].CopySymbol(Borders[0 + 11]);
                    else if (fy > 0 && fy < editedHeight - 1  && (fx == 0 || fx == editedWidth -1))
                        window[fx, fy].CopySymbol(Borders[1 + 11]);

                    else
                        window[fx, fy].BackColor = backColor;
                }
            }
        }


        public void Show()
        {
            screen.Set(window, this.x, this.y);
            screen.DrawArea(x, y, this.editedWidth , this.editedHeight);
        }
    }
}
