using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using _3DMath;

namespace _3DPointRendering
{
    public partial class Form1 : Form
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch renderListwatch = new System.Diagnostics.Stopwatch();
        private List<Polygon> polygons = new List<Polygon>();
        private Dictionary<Polygon, Map.Block> blockRelations = new Dictionary<Polygon, Map.Block>();
        static public Map map = new Map();
        static public TickHandler tickHandler = new TickHandler();
        static public InputHandler inputHandler;
        public Form1()
        {
            inputHandler = new InputHandler(this);
            for (int i = 0; i < map.xSize * map.ySize * map.zSize; i++)
            {
                if (map[i] == null) continue;
                Polygon faceFront = new Polygon();
                faceFront[0] = map[i].pos + new Vector3(map[i].size.x / 2, map[i].size.y / 2, map[i].size.z / 2);
                faceFront[1] = map[i].pos + new Vector3(-map[i].size.x / 2, map[i].size.y / 2, map[i].size.z / 2);
                faceFront[2] = map[i].pos + new Vector3(-map[i].size.x / 2, -map[i].size.y / 2, map[i].size.z / 2);
                faceFront[3] = map[i].pos + new Vector3(map[i].size.x / 2, -map[i].size.y / 2, map[i].size.z / 2);
                polygons.Add(faceFront);
                faceFront.color = Brushes.Blue;


                Polygon faceBack = new Polygon();
                faceBack[3] = map[i].pos + new Vector3(map[i].size.x / 2, map[i].size.y / 2, -map[i].size.z / 2);
                faceBack[2] = map[i].pos + new Vector3(-map[i].size.x / 2, map[i].size.y / 2, -map[i].size.z / 2);
                faceBack[1] = map[i].pos + new Vector3(-map[i].size.x / 2, -map[i].size.y / 2, -map[i].size.z / 2);
                faceBack[0] = map[i].pos + new Vector3(map[i].size.x / 2, -map[i].size.y / 2, -map[i].size.z / 2);
                polygons.Add(faceBack);
                faceBack.color = Brushes.Purple;


                Polygon faceBottom = new Polygon();
                faceBottom[3] = map[i].pos + new Vector3(map[i].size.x / 2, map[i].size.y / 2, map[i].size.z / 2);
                faceBottom[2] = map[i].pos + new Vector3(-map[i].size.x / 2, map[i].size.y / 2, map[i].size.z / 2);
                faceBottom[1] = map[i].pos + new Vector3(-map[i].size.x / 2, map[i].size.y / 2, -map[i].size.z / 2);
                faceBottom[0] = map[i].pos + new Vector3(map[i].size.x / 2, map[i].size.y / 2, -map[i].size.z / 2);
                polygons.Add(faceBottom);
                faceBottom.color = Brushes.Red;


                Polygon faceTop = new Polygon();
                faceTop[0] = map[i].pos + new Vector3(map[i].size.x / 2, -map[i].size.y / 2, map[i].size.z / 2);
                faceTop[1] = map[i].pos + new Vector3(-map[i].size.x / 2, -map[i].size.y / 2, map[i].size.z / 2);
                faceTop[2] = map[i].pos + new Vector3(-map[i].size.x / 2, -map[i].size.y / 2, -map[i].size.z / 2);
                faceTop[3] = map[i].pos + new Vector3(map[i].size.x / 2, -map[i].size.y / 2, -map[i].size.z / 2);
                polygons.Add(faceTop);
                faceTop.color = Brushes.Green;


                Polygon faceRight = new Polygon();
                faceRight[3] = map[i].pos + new Vector3(map[i].size.x / 2, map[i].size.y / 2, map[i].size.z / 2);
                faceRight[2] = map[i].pos + new Vector3(map[i].size.x / 2, map[i].size.y / 2, -map[i].size.z / 2);
                faceRight[1] = map[i].pos + new Vector3(map[i].size.x / 2, -map[i].size.y / 2, -map[i].size.z / 2);
                faceRight[0] = map[i].pos + new Vector3(map[i].size.x / 2, -map[i].size.y / 2, map[i].size.z / 2);
                polygons.Add(faceRight);
                faceRight.color = Brushes.DarkCyan;


                Polygon faceLeft = new Polygon();
                faceLeft[0] = map[i].pos + new Vector3(-map[i].size.x / 2, map[i].size.y / 2, map[i].size.z / 2);
                faceLeft[1] = map[i].pos + new Vector3(-map[i].size.x / 2, map[i].size.y / 2, -map[i].size.z / 2);
                faceLeft[2] = map[i].pos + new Vector3(-map[i].size.x / 2, -map[i].size.y / 2, -map[i].size.z / 2);
                faceLeft[3] = map[i].pos + new Vector3(-map[i].size.x / 2, -map[i].size.y / 2, map[i].size.z / 2);
                polygons.Add(faceLeft);
                faceLeft.color = Brushes.DarkCyan;

                blockRelations.Add(faceFront, map[i]);
                blockRelations.Add(faceBack, map[i]);
                blockRelations.Add(faceTop, map[i]);
                blockRelations.Add(faceBottom, map[i]);
                blockRelations.Add(faceRight, map[i]);
                blockRelations.Add(faceLeft, map[i]);

            }

            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            tickHandler.Start();
            new Camera(Width, Height);
            Invalidated += Form1_Invalidated;
        }

        private void Form1_Invalidated(object sender, InvalidateEventArgs e)
        {
            if (isPainting) return;
            renderListwatch.Restart();
            _poly = polygons.OrderByDescending(p => p.Dist(Camera.camPos));
            renderListwatch.Stop();
        }

        public void SortPoly()
        {
            //_poly = _poly.OrderByDescending(p => p.Dist(Camera.camPos));
        }
        public void MergePoly()
        {

        }

        Point[] p = { new Point(), new Point(), new Point() };
        IOrderedEnumerable<Polygon> _poly;
        public static bool isPainting { private set; get; }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_poly == null) return;
            if (isPainting) return;
            isPainting = true;

            stopwatch.Restart();
            double lx = Math.Sin(Camera.camRot.y * (Math.PI / 180)) * Math.Cos(Camera.camRot.x * (Math.PI / 180));
            double lz = Math.Cos(Camera.camRot.y * (Math.PI / 180)) * Math.Cos(Camera.camRot.x * (Math.PI / 180));
            double ly = Math.Sin(Camera.camRot.x * (Math.PI / 180));


            int limit = 2200;

            Graphics g = e.Graphics;
            int polyDrawn = 0;
            foreach (var polygon in _poly)
            {
                if (polygon == null) continue;

                Point pM = Math3D.ProjectPointTo2D(polygon.GetMiddel());
                if ((pM.X > limit || pM.X < -limit) || (pM.Y > limit || pM.Y < -limit))
                {
                    continue;
                }

                Vector3 dir = (new Vector3(lx, ly, lz) + Camera.camPos) - polygon.GetMiddel();
                double a1 = Math3D.AngleBetweenVectors3D(polygon.GetNormal(), dir) * (180 / Math.PI);
                if (a1 > 95) continue; //Backface culling
                                       //Console.WriteLine(a1);

                if (blockRelations.ContainsKey(polygon))
                {
                    Vector3 pm1 = Map.WorldCordToMapCord(polygon.GetMiddel());
                    Map.Block block = map[pm1 + polygon.GetNormal().Normalized()];
                    if (block != null && block != map[pm1]) continue;
                }

                for (int v = 1; v < polygon.verticeCount; v += 2)
                {
                    p[0] = Math3D.ProjectPointTo2D(polygon[v - 1]);
                    p[1] = Math3D.ProjectPointTo2D(polygon[v]);
                    p[2] = Math3D.ProjectPointTo2D(polygon[v >= polygon.verticeCount - 1 ? 0 : v + 1]);

                    if ((p[0].X > limit || p[0].X < -limit) ||
                        (p[1].X > limit || p[1].X < -limit) ||
                        (p[2].X > limit || p[2].X < -limit) ||
                        (p[0].Y > limit || p[0].Y < -limit) ||
                        (p[1].Y > limit || p[1].Y < -limit) ||
                        (p[2].Y > limit || p[2].Y < -limit))
                    {
                        continue;
                    }

                    if (p[0].X > Width && p[1].X > Width && p[2].X > Width) continue;
                    if (p[0].X < 0 && p[1].X < 0 && p[2].X < 0) continue;
                    if (p[0].Y > Height && p[1].Y > Height && p[2].Y > Height) continue;
                    if (p[0].Y < 0 && p[1].Y < 0 && p[2].Y < 0) continue;

                    g.FillPolygon(polygon.color, p);
                    g.DrawLine(Pens.Black, p[0], p[1]);
                    g.DrawLine(Pens.Black, p[1], p[2]);
                    polyDrawn++;
                }
            }

            #region old
            /*
            for (int i = 0; i < _poly.Count; i++)
            {
                if (_poly[i] == null) continue;

                Point pM = Math3D.ProjectPointTo2D(_poly[i].GetMiddel());
                if ((pM.X > limit || pM.X < -limit) || (pM.Y > limit || pM.Y < -limit))
                {
                    continue;
                }

                Vector3 dir = (new Vector3(lx, ly, lz) + Camera.camPos) - _poly[i].GetMiddel();
                double a1 = Math3D.AngleBetweenVectors3D(_poly[i].GetNormal(), dir) * (180 / Math.PI);
                if (a1 > 95) continue; //Backface culling
                                       //Console.WriteLine(a1);

                if (blockRelations.ContainsKey(_poly[i]))
                {
                    Vector3 pm1 = Map.WorldCordToMapCord(_poly[i].GetMiddel());
                    Map.Block block = map[pm1 + _poly[i].GetNormal().Normalized()];
                    if (block != null && block != map[pm1]) continue;
                }

                for (int v = 1; v < _poly[i].verticeCount; v += 2)
                {
                    p[0] = Math3D.ProjectPointTo2D(_poly[i][v - 1]);
                    p[1] = Math3D.ProjectPointTo2D(_poly[i][v]);
                    p[2] = Math3D.ProjectPointTo2D(_poly[i][v >= _poly[i].verticeCount - 1 ? 0 : v + 1]);

                    if ((p[0].X > limit || p[0].X < -limit) ||
                        (p[1].X > limit || p[1].X < -limit) ||
                        (p[2].X > limit || p[2].X < -limit) ||
                        (p[0].Y > limit || p[0].Y < -limit) ||
                        (p[1].Y > limit || p[1].Y < -limit) ||
                        (p[2].Y > limit || p[2].Y < -limit))
                    {
                        continue;
                    }

                    if (p[0].X > Width && p[1].X > Width && p[2].X > Width) continue;
                    if (p[0].X < 0 && p[1].X < 0 && p[2].X < 0) continue;
                    if (p[0].Y > Height && p[1].Y > Height && p[2].Y > Height) continue;
                    if (p[0].Y < 0 && p[1].Y < 0 && p[2].Y < 0) continue;

                    g.FillPolygon(_poly[i].color, p);
                    g.DrawLine(Pens.Black, p[0], p[1]);
                    g.DrawLine(Pens.Black, p[1], p[2]);
                    polyDrawn++;
                }
            }
            */
            #endregion
            stopwatch.Stop();
            string debugText = $"X: {Camera.camRot.x}, Y: {Camera.camRot.y}, Z: {Camera.camRot.z} \n" +
                               $"X: {Camera.camPos.x}, Y: {Camera.camPos.y}, Z: {Camera.camPos.z} \n" +
                               $"Time: {stopwatch.ElapsedMilliseconds} {renderListwatch.ElapsedMilliseconds} : " +
                               $"{stopwatch.ElapsedMilliseconds + renderListwatch.ElapsedMilliseconds }\n" +
                               $"{polyDrawn} / {_poly.Count()}";

            g.DrawString(debugText, Font, Brushes.Black, 0, 0);
            isPainting = false;
        }

        int Clamp(int value, int min, int max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
    }

    public class TickHandler
    {
        public static Action onTick;
        public int tickSpeed = 16;

        public void Start()
        {
            Task.Run(Run);
        }

        public void Run()
        {
            Console.WriteLine("Tick handler started!");
            while (Form1.inputHandler != null)
            {
                onTick?.Invoke();
                if (Form.ActiveForm != null && !Form1.isPainting) Form.ActiveForm.Invalidate();
                Thread.Sleep(tickSpeed);
            }
            Console.WriteLine("Tick handler stopped!");
        }
    }
    public class InputHandler
    {
        public double moveSpeed = 0.1f;
        public Form handleTarget;

        Dictionary<Keys, Action> inputAction;

        List<Action> tickActions = new List<Action>();

        public InputHandler(Form target)
        {
            inputAction = new Dictionary<Keys, Action>()
            {
                {Keys.W, () => { Camera.MoveWithRotation(0,0,moveSpeed); } },
                {Keys.A, () => { Camera.MoveWithRotation(-moveSpeed,0,0); } },
                {Keys.S, () => { Camera.MoveWithRotation(0,0,-moveSpeed); } },
                {Keys.D, () => { Camera.MoveWithRotation(moveSpeed,0,0); } },
                {Keys.Space, () => { Camera.MoveWithRotation(0,-moveSpeed,0); } },
                {Keys.ControlKey, () => { Camera.MoveWithRotation(0,moveSpeed,0); } },


                {Keys.Down, () => {Camera.Rotate(-1,0,0); } },
                {Keys.Up, () => {Camera.Rotate(1,0,0); } },
                {Keys.Left, () => {Camera.Rotate(0,-1,0); } },
                {Keys.Right, () => {Camera.Rotate(0,1,0); } },
            };

            handleTarget = target;
            handleTarget.KeyDown += HandleTarget_KeyDown;
            handleTarget.KeyUp += HandleTarget_KeyUp;
            TickHandler.onTick += () =>
            {
                for (int i = 0; i < tickActions.Count; i++)
                {
                    tickActions[i].Invoke();
                }
            };


        }

        private void HandleTarget_KeyUp(object sender, KeyEventArgs e)
        {
            if (!inputAction.ContainsKey(e.KeyCode)) return;
            if (!tickActions.Contains(inputAction[e.KeyCode])) return;
            tickActions.Remove(inputAction[e.KeyCode]);
        }

        private void HandleTarget_KeyDown(object sender, KeyEventArgs e)
        {
            if (!inputAction.ContainsKey(e.KeyCode)) return;
            if (tickActions.Contains(inputAction[e.KeyCode])) return;
            tickActions.Add(inputAction[e.KeyCode]);
        }
    }

    public class Camera
    {
        public static Vector3 camPos = new Vector3(0, -5, 0);
        public static Vector3 camRot = new Vector3(0, 0, 0);
        public static Vector3 displaySurface;
        public Camera(float width, float height)
        {
            displaySurface = new Vector3(width / 2, height / 2, width);
            Task.Run(CamPhysics);
        }

        public static void MoveWithRotation(double x, double y, double z)
        {
            camPos.x += z * Math.Sin(camRot.y * (Math.PI / 180));
            camPos.z += z * Math.Cos(camRot.y * (Math.PI / 180));

            camPos.x += x * Math.Cos(camRot.y * (Math.PI / 180));
            camPos.z -= x * Math.Sin(camRot.y * (Math.PI / 180));

            camPos.y += y;
        }
        public static void Rotate(double x, double y, double z)
        {
            camRot += new Vector3(x, y, z);
        }

        double fallSpeed = 0.1f;
        double height = 1;

        void CamPhysics()
        {
            while (Form.ActiveForm != null)
            {
                Vector3 newMapPos = camPos + new Vector3(0, fallSpeed + height, 0);
                newMapPos.y = Math.Ceiling(newMapPos.y);
                newMapPos = Map.WorldCordToMapCord(newMapPos);
                //Console.WriteLine(newMapPos.ToString());
                if (Form1.map[newMapPos] != null) continue;
                camPos.y += fallSpeed;
                Thread.Sleep(16);
            }
        }
    }
    public class Map
    {
        public int xSize = 5;
        public int ySize = 5;
        public int zSize = 5;

        public Vector3 worldOffset = new Vector3(0, -3, 0);

        static Map instance;

        private Block[,,] blocks = new Block[5, 5, 5];
        public Block this[int x, int y, int z]
        {
            get { return blocks[x, y, z]; }
            set { blocks[x, y, z] = value; }
        }
        public Block this[Vector3 cord]
        {
            get
            { 
                return blocks[(int)Math3D.Clamp(cord.x, 0, xSize -1), (int)Math3D.Clamp(cord.y, 0, ySize - 1), (int)Math3D.Clamp(cord.z, 0, zSize - 1)]; 
            }
            set { blocks[(int)cord.x, (int)cord.y, (int)cord.z] = value; }
        }
        public Block this[int i]
        {
            get
            {
                int z = i / (xSize * ySize);
                i -= (z * xSize * ySize);
                int y = i / xSize;
                int x = i % xSize;
                return blocks[x, y, z];
            }
            set
            {
                int z = i / (xSize * ySize);
                i -= (z * xSize * ySize);
                int y = i / xSize;
                int x = i % xSize;
                blocks[x, y, z] = value;
            }
        }

        public Map()
        {
            worldOffset.x = -xSize / 2;
            blocks = new Block[xSize, ySize, zSize];
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 2; y < ySize; y++)
                {
                    for (int z = 0; z < zSize; z++)
                    {
                        blocks[x, y, z] = new Block(new Vector3(x, y, z));
                    }
                }
            }
            instance = this;
        }

        public class Block
        {
            public Vector3 _pos;
            public Vector3 pos
            {
                get
                {
                    return _pos + instance.worldOffset;
                }
                set
                {
                    _pos = value;
                }
            }
            public Vector3 size = new Vector3(1, 1, 1);

            public Block(Vector3 _pos)
            {
                pos = _pos;
            }
        }


        public static Vector3 WorldCordToMapCord(Vector3 coords)
        {
            Vector3 convertedCoords = coords - instance.worldOffset;
            convertedCoords.x = Math3D.Clamp(Math.Round(convertedCoords.x), 0, instance.xSize -1);
            convertedCoords.y = Math3D.Clamp(Math.Round(convertedCoords.y), 0, instance.ySize -1);
            convertedCoords.z = Math3D.Clamp(Math.Round(convertedCoords.z), 0, instance.zSize -1);

            return convertedCoords;
        }

        public int to1D(int x, int y, int z)
        {
            return (z * xSize * ySize) + (y * xSize) + x;
        }
        public int to1D(Vector3 pos)
        {
            return ((int)pos.z * xSize * ySize) + ((int)pos.y * xSize) + (int)pos.x;
        }
    }
    

}
namespace _3DMath
{


    public class Vector3
    {
        public double x;
        public double y;
        public double z;

        public Vector3(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public Vector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public static Vector3 operator *(Vector3 v1, double num)
        {
            return new Vector3(v1.x * num, v1.y * num, v1.z * num);
        }
        public static Vector3 operator /(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }
        public static Vector3 operator /(Vector3 v1, double num)
        {
            return new Vector3(v1.x / num, v1.y / num, v1.z / num);
        }
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public Vector3 Normalized()
        {
            double d = Math.Sqrt((x * x) + (y * y) + (z * z));
            return new Vector3(x / d, y / d, z / d);
        }
    }
    public class Vector2
    {
        public double x;
        public double y;

        public Vector2(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public Vector2()
        {
            x = 0;
            y = 0;
        }
    }

    public class Polygon
    {
        public const int MAX_VERTS = 8;
        private Vector3[] vertices = new Vector3[MAX_VERTS];
        public Vector3 this[int i]
        {
            get
            {
                return vertices[i]; 
            }
            set
            {
                vertices[i] = value;
                for (int v = 0; v < vertices.Length; v++)
                {
                    verticeCount = v;
                    if (vertices[v] == null) break;
                }
            }
        }
        public int verticeCount;
        public Brush color;

        public double Dist(Vector3 camPos)
        {
            double dst = 0;
            for (int i = 0; i < verticeCount; i++)
            {
                dst += Math3D.FindDst(camPos, vertices[i]);
            }
            dst = dst / verticeCount;
            return dst;
        }
        public Vector3 GetMiddel()
        {
            Vector3 mid = new Vector3();
            for (int i = 0; i < verticeCount; i++)
            {
                mid += vertices[i];
            }
            return mid / verticeCount;
        }
        public Vector3 GetNormal()
        {
            Vector3 cross = new Vector3();
            for (int i = 1; i < verticeCount; i++)
            {
                cross += Math3D.CrossProduct(vertices[i - 1], vertices[i]);
            }
            cross += Math3D.CrossProduct(vertices[verticeCount - 1], vertices[0]);
            return cross.Normalized();
        }
    }
    public class Math3D
    {
        public static bool IsInFront(Vector3 a, Vector3 b, Vector3 dir)
        {
            double product = (a.x - b.x) * dir.x
                             + (a.y - b.y) * dir.y
                             + (a.z - b.z) * dir.z;
            return (product > 0.0);
        }


        static Vector3 D(Vector3 p)
        {
            Vector3 rot = _3DPointRendering.Camera.camRot * (Math.PI / 180);
            Vector3 d = new Vector3();
            double x = p.x - _3DPointRendering.Camera.camPos.x;
            double y = p.y - _3DPointRendering.Camera.camPos.y;
            double z = p.z - _3DPointRendering.Camera.camPos.z;


            d.x = Math.Cos(rot.y) * (Math.Sin(rot.z) * y + Math.Cos(rot.z) * x) - Math.Sin(rot.y) * z;
            d.y = Math.Sin(rot.x) * (Math.Cos(rot.y) * z + Math.Sin(rot.y) * (Math.Sin(rot.z) * y + Math.Cos(rot.z) * x)) + Math.Cos(rot.x) * (Math.Cos(rot.z) * y - Math.Sin(rot.z) * x);
            d.z = Math.Cos(rot.x) * (Math.Cos(rot.y) * z + Math.Sin(rot.y) * (Math.Sin(rot.z) * y + Math.Cos(rot.z) * x)) - Math.Sin(rot.x) * (Math.Cos(rot.z) * y - Math.Sin(rot.z) * x);

            return d;
        }

        public static Point ProjectPointTo2D(Vector3 p)
        {
            Point point = new Point();

            Vector3 d = D(p);
            point.X = (int)((_3DPointRendering.Camera.displaySurface.z / d.z) * d.x + _3DPointRendering.Camera.displaySurface.x);
            point.Y = (int)((_3DPointRendering.Camera.displaySurface.z / d.z) * d.y + _3DPointRendering.Camera.displaySurface.y);



            return point;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        public static double Clamp(double value, double min, double max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        public static double FindDst(Vector3 p1, Vector3 p2)
        {
            double x = Math.Pow(p2.x - p1.x, 2);
            double y = Math.Pow(p2.y - p1.y, 2);
            double z = Math.Pow(p2.z - p1.z, 2);

            return Math.Sqrt(x + y + z);
        }
        public static double FindDst(Vector3 p1)
        {
            double x = Math.Pow(p1.x, 2);
            double y = Math.Pow(p1.y, 2);
            double z = Math.Pow(p1.z, 2);

            return Math.Sqrt(x + y + z);
        }

        public static Vector3 CrossProduct(Vector3 v1, Vector3 v2)
        {
            double x = v1.y * v2.z - v1.z * v2.y;
            double y = v1.z * v2.x - v1.x * v2.z;
            double z = v1.x * v2.y - v1.y * v2.x;
            return new Vector3(x, y, z);
        }


        public static double AngleBetweenVectors3D(Vector3 a, Vector3 b)
        {
            double a_dot_b = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
            double a_Length = Math.Sqrt((a.x * a.x) + (a.y * a.y) + (a.z * a.z));
            double b_Length = Math.Sqrt((b.x * b.x) + (b.y * b.y) + (b.z * b.z));
            double angle = Math.Acos(a_dot_b / (a_Length * b_Length));

            return angle;
        }
    }
}
