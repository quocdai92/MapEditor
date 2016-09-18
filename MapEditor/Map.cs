using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapEditor
{
    public partial class Map : Form
    {
        public static int CellSize = 20;
        public static int height = 0;
        public static int width = 0;
        public static List<Cell> GridMaps = new List<Cell>();
        public Stack<List<Cell>> UndoStack = new Stack<List<Cell>>();
        public Stack<List<Cell>> RedoStack = new Stack<List<Cell>>();

        private BufferedGraphics myBuffer;
        private BufferedGraphicsContext currentContext;
        private bool isDrawMap = false;
        private bool isDragging = false;
        private Point startPosition;
        private Point endPosition;
        private DisplayArea CurrentArea;
        public Map()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            currentContext = BufferedGraphicsManager.Current;
            panelMap.Width = width * CellSize;
            panelMap.Height = height * CellSize;
            ResetGridPanel(CellSize,width,height);
            if (myBuffer != null)
                myBuffer.Dispose();
            myBuffer = currentContext.Allocate(this.panelMap.CreateGraphics(),
                                               this.panelMap.DisplayRectangle);
    //        typeof(Panel).InvokeMember("DoubleBuffered",
    //BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
    //null, panelMap, new object[] { true });
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UndoStack.Count > 0)
            {
                var listGrid = UndoStack.Pop();
                RedoStack.Push(listGrid);
                CurrentArea.ListGrid = new List<Cell>();
                CurrentArea.ListGrid.AddRange(listGrid);
                panelMap.Invalidate();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RedoStack.Count > 0)
            {
                var listGrid = RedoStack.Pop();
                UndoStack.Push(listGrid);
                CurrentArea.ListGrid = new List<Cell>();
                CurrentArea.ListGrid.AddRange(listGrid);
                panelMap.Invalidate();
            }
        }

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            if (myBuffer != null)
                myBuffer.Dispose();
            myBuffer = currentContext.Allocate(this.panelMap.CreateGraphics(),
                                               this.panelMap.DisplayRectangle);
            SolidBrush brush = new SolidBrush(Color.FromArgb(128, Color.Red));
            if (chkDoubleBuffer.Checked)
            {
                Graphics g = panelMap.CreateGraphics();
                g.Clear(Color.Black);
                Bitmap drawing = null;
                drawing = new Bitmap(this.Width, this.Height, e.Graphics);
                g = Graphics.FromImage(drawing);

                if (GridMaps != null)
                {
                    foreach (var cell in GridMaps)
                    {
                        g.FillRectangle(new SolidBrush(Color.SlateGray),
                            cell.StartPosition.X, cell.StartPosition.Y, 1, 1);
                    }
                }
                if (CurrentArea != null && CurrentArea.ListGrid != null && CurrentArea.ListGrid.Count > 0)
                {
                    brush = new SolidBrush(CurrentArea.Color);
                    foreach (var cell in CurrentArea.ListGrid)
                    {
                        g.FillRectangle(brush, cell.StartPosition.X - CellSize / 4,
                            cell.StartPosition.Y - CellSize / 4, CellSize/2, CellSize/2);
                    }
                }

                e.Graphics.DrawImageUnscaled(drawing, 0, 0);
                g.Dispose();
            }
            else
            {
                Pen p = new Pen(Color.Black);
                myBuffer.Graphics.Clear(Color.Black);
                if (GridMaps != null)
                {
                    foreach (var cell in GridMaps)
                    {
                        myBuffer.Graphics.FillRectangle(new SolidBrush(Color.SlateGray),
                            cell.StartPosition.X, cell.StartPosition.Y, 1, 1);
                    }
                }
                if (CurrentArea != null && CurrentArea.ListGrid != null && CurrentArea.ListGrid.Count > 0)
                {
                    brush = new SolidBrush(CurrentArea.Color);
                    foreach (var cell in CurrentArea.ListGrid)
                    {
                        myBuffer.Graphics.FillRectangle(brush, cell.StartPosition.X - CellSize / 4,
                            cell.StartPosition.Y - CellSize / 4, CellSize/2, CellSize/2);
                    }
                }
                myBuffer.Render(this.panelMap.CreateGraphics());
            }

        }
        public void ResetGridPanel(int cellSize,int w, int h)
        {
            panelMap.Width = w*cellSize;
            panelMap.Height = h*cellSize;
            GridMaps = new List<Cell>();
            for (int y = 0; y < h; ++y)
            {
                for (int k = 0; k < w; k++)
                {
                    var cell = new Cell()
                    {
                        Size = cellSize,
                        StartPosition = new Point(k * cellSize, y * cellSize),
                        X = k,
                        Y = y
                    };
                    GridMaps.Add(cell);
                }
            }
        }
        private void UpdateListGrid(List<Cell> listGrid)
        {
            foreach (var cell in listGrid)
            {
                cell.Size = CellSize;
                cell.StartPosition = new Point(cell.X * CellSize, cell.Y * CellSize);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            isDrawMap = true;
            CurrentArea = new DisplayArea()
            {
                Color = Color.Red
            };
            MessageBox.Show(@"Drag to create map.");
        }

        private void panelMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isDrawMap)
            {
                startPosition = e.Location;
                isDragging = true;
            }
        }

        private void panelMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                endPosition = e.Location;
                var width = Math.Abs(endPosition.X - startPosition.X);
                var height = Math.Abs(endPosition.Y - startPosition.Y);
                var x = startPosition.X;
                var y = startPosition.Y;
                if (endPosition.X < startPosition.X && endPosition.Y < startPosition.Y)
                {
                    x = endPosition.X;
                    y = endPosition.Y;
                }
                else if (endPosition.X < startPosition.X && endPosition.Y > startPosition.Y)
                {
                    x = endPosition.X;
                    y = startPosition.Y;
                }
                else if (endPosition.X > startPosition.X && endPosition.Y < startPosition.Y)
                {
                    x = startPosition.X;
                    y = endPosition.Y;
                }
                if (CurrentArea != null)
                {
                    var area = new Rectangle(x, y, width, height);
                    var listCellInArea = GridMaps.Where(cell => IsGridInTheDisplayArea(cell, area)).ToList();
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        foreach (var cell in listCellInArea)
                        {
                            if (!CurrentArea.ListGrid.Contains(cell))
                            {
                                CurrentArea.ListGrid.Add(cell);
                            }
                        }
                        UndoStack.Push(CurrentArea.ListGrid);
                    }
                    else
                    {
                        CurrentArea.ListGrid = listCellInArea;
                        UndoStack.Push(CurrentArea.ListGrid);
                    }
                }

                panelMap.Invalidate();
            }
        }

        private void panelMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                endPosition = e.Location;
                isDragging = false;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isDrawMap = false;
        }
        private bool IsGridInTheDisplayArea(Cell grid, Rectangle displayRectangle)
        {
            if (IsPointInTheRectangle(grid.StartPosition.X, grid.StartPosition.Y, displayRectangle)
                )
                return true;
            return false;
        }
        private bool IsPointInTheRectangle(int x, int y, Rectangle displayRectangle)
        {
            if (displayRectangle.Width == 0 || displayRectangle.Height == 0)
            {
                return false;
            }
            if (x >= displayRectangle.X && x <= displayRectangle.X + displayRectangle.Width
                && y >= displayRectangle.Y && y <= displayRectangle.Y + displayRectangle.Height)
            {
                return true;
            }
            return false;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            CellSize = trackBar1.Value;
            ResetGridPanel(CellSize,width,height);
            if (CurrentArea != null)
            {
                UpdateListGrid(CurrentArea.ListGrid);
            }
            panelMap.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapConfig config = new MapConfig();
            config.ShowDialog();
            ResetGridPanel(CellSize,width,height);
            panelMap.Invalidate();
        }
    }
}
