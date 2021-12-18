using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba8 {
    
    public class DGroup : DrawableObject {

        public override Point[,] getBound() {
            throw new NotImplementedException();
        }

        //public Group(Point point1_, Point point2_, Color color_) : base(point1_, point2_, color_) {
        //}
        DateTime lastClick = DateTime.Now;
        bool insideSelect = false;
        DrawableObject[] objs;
        public DrawableObject selected {protected set; get; }
        DrawableObject prevClick;


        public override Color color {
            get { return myColor; }
            set {
                foreach(DrawableObject a in objs) {
                    a.color = value;
                    myColor = value;
                }
            }
        }
        public DGroup(params DrawableObject[] mas) {
            name = "DGroup";
            objs = new DrawableObject[mas.Length];
            for (int i = 0; i < mas.Length; i++) {
                objs[i] = mas[i];
            }
            checkBound();
        }
        private void checkBound() {
            if (objs.Length == 0) return;
            point1 = new Point(objs[0].point1.X, objs[0].point1.Y);
            point2 = new Point(objs[0].point2.X, objs[0].point2.Y);

            int maxX = 0;
            int minX = int.MaxValue;
            int maxY = 0;
            int minY = int.MaxValue;


            foreach (DrawableObject a in objs) {

                if (a as DGroup != null) ((DGroup)a).checkBound();

                if (a != null) {
                    if (a.point1.X > maxX) {
                        maxX = a.point1.X;
                    }
                    if (a.point1.X < minX) {
                        minX = a.point1.X;
                    }

                    if (a.point2.X > maxX) {
                        maxX = a.point2.X;
                    }
                    if (a.point2.X < minX) {
                        minX = a.point2.X;
                    }

                    if (a.point1.Y > maxY) {
                        maxY = a.point1.Y;
                    }
                    if (a.point1.Y < minY) {
                        minY = a.point1.Y;
                    }

                    if (a.point2.Y > maxY) {
                        maxY = a.point2.Y;
                    }
                    if (a.point2.Y < minY) {
                        minY = a.point2.Y;
                    }


                }
            }
            point1 = new Point(minX, minY);
            point2 = new Point(maxX, maxY);
            Console.WriteLine("checkBound");
        }
        public override bool click(object sender, MouseEventArgs e) {

   


            if (selected != null) {
                if (selected.click(sender, e) == true) {
                    return true;
                }
            }
            selected = null;
            if ((DateTime.Now - lastClick).TotalMilliseconds < SystemInformation.DoubleClickTime) {
                insideSelect = true;
            }
            else {
                insideSelect = false;
            }
            lastClick = DateTime.Now;
            //bool result = false;



            foreach (DrawableObject a in objs) {

                if (a.click(sender, e) == true) {

                    //result = true;



                    if (insideSelect == true) {

                        //if (prevClick == a) {
                        selected = a;
                        try {
                            while (true) {
                                selected = ((DGroup)selected).selected;
                            }
                        }
                        catch {   }
                        if (selected == null) selected = a;
                        //}

                        //prevClick = a;
                        lastClick = DateTime.Now;
                        return true;
                    }
                    selected = null;
                    //prevClick = a;
                    
                    return true;
                }
            }

            selected = null;
            //lastClick = DateTime.Now;
            return false;
        }
        public override void draw(object sender, PaintEventArgs e) {

            if (selected != null) {
                selected.drawChooseLine(sender, e);
                selected.drawResizeDot(sender, e);
            }

            foreach (DrawableObject a in objs) {
                if (a != null) {
                    a.draw(sender, e);
                }
            }



            //drawChooseLine(sender, e);
        }
        public override void drawResizeDot(object sender, PaintEventArgs e) {
            if (selected != null) {
                selected.drawResizeDot(sender, e);
                return;
            }
            base.drawResizeDot(sender, e);
        }
        public override void drawChooseLine(object sender, PaintEventArgs e) {
            if (selected != null) {
                selected.drawChooseLine(sender, e);
                
                return;
            }
            Pen pen = new Pen(Color.White, 3);
            e.Graphics.DrawRectangle(pen, rectangle);
            pen.DashPattern = dashValue;
            pen.Color = Color.Blue;
            e.Graphics.DrawRectangle(pen, rectangle);
        }
        public override void move(int moveX, int moveY) {

            if (selected != null) {
                selected.move(moveX, moveY);
                checkBound();
                Console.WriteLine("Here");
                return;
            }

            foreach (DrawableObject a in objs) {
                a.move(moveX, moveY);
            }
            base.move(moveX, moveY);
            //checkBound();
        }

        float[] positionsXF;
        float[] positionsYF;
        int [] positionsX;
        int [] positionsY;
        Point[] objsPoints;
        int startSizeX;
        int startSizeY;

        public override bool startResize(object sender, MouseEventArgs e) {
            if (selected != null) {
                return (selected.startResize(sender, e));
            }

            if (base.startResize(sender, e) == false) {
                return false;
            }

            Point pos = point1;///////////////////
            

            positionsXF = new float[objs.Length];
            positionsYF = new float[objs.Length];

            positionsX = new int[objs.Length];
            positionsY = new int[objs.Length];

            objsPoints = new Point[objs.Length];

            startSizeX = point2.X - point1.X;
            startSizeY = point2.Y - point1.Y;

            for (int i = 0; i < objs.Length; i++) {
                Point oPos = objs[i].position;
                positionsX[i] = oPos.X - pos.X;
                positionsY[i] = oPos.Y - pos.Y;


                Point a = objs[i].startResizeProcent();
                a.X = a.X - pos.X;
                a.Y = a.Y - pos.Y;
                objsPoints[i] = a;
            }

            return true;
        }
        public override void resize(object sender, MouseEventArgs e) {
            if (selected != null) {
                selected.resize(sender, e);
                return;  
            }
            base.resize(sender, e);

            checkExitByResize(((Control)sender).ClientRectangle);

            float xP = ((float)(point2.X - point1.X) / (float)startSizeX);
            float yP = ((float)(point2.Y - point1.Y) / (float)startSizeY);

            Point pos = point1;///////////////////
            //Point pos = position;
            //Console.WriteLine("xP = " + xP + " yP = " + yP);

            for (int i = 0; i < objs.Length; i++) {
                objs[i].resizeProcent(xP, yP);

                objs[i].setPosition((int)(objsPoints[i].X * xP) + pos.X, 
                                    (int)(objsPoints[i].Y * yP) + pos.Y); 
            }
            
        }
        public override Point startResizeProcent() {
            base.startResizeProcent();

            Point pos = point1;///////////////////


            positionsXF = new float[objs.Length];
            positionsYF = new float[objs.Length];

            positionsX = new int[objs.Length];
            positionsY = new int[objs.Length];

            objsPoints = new Point[objs.Length];

            startSizeX = point2.X - point1.X;
            startSizeY = point2.Y - point1.Y;

            for (int i = 0; i < objs.Length; i++) {
                Point oPos = objs[i].position;
                positionsX[i] = oPos.X - pos.X;
                positionsY[i] = oPos.Y - pos.Y;


                Point a = objs[i].startResizeProcent();
                a.X = a.X - pos.X;
                a.Y = a.Y - pos.Y;
                objsPoints[i] = a;
            }

            return point1;



            /*
            base.startResizeProcent();

            Point pos = position;

            positionsXF = new float[objs.Length];
            positionsYF = new float[objs.Length];

            positionsX = new int[objs.Length];
            positionsY = new int[objs.Length];

            startSizeX = point2.X - point1.X;
            startSizeY = point2.Y - point1.Y;

            for (int i = 0; i < objs.Length; i++) {
                Point oPos = objs[i].position;
                positionsX[i] = oPos.X - pos.X;
                positionsY[i] = oPos.Y - pos.Y;

                objs[i].startResizeProcent();
            }

            return point1;
            */
        }
        public override void resizeProcent(float xP, float yP) {
            base.resizeProcent(xP, yP);
            Point pos = point1;
            for (int i = 0; i < objs.Length; i++) {


                objs[i].resizeProcent(xP, yP);

                objs[i].setPosition((int)(objsPoints[i].X * xP) + pos.X,
                                    (int)(objsPoints[i].Y * yP) + pos.Y);
            }
        }
        public override void setPosition(int x, int y) {
            base.setPosition(x, y);

            int xDif = point1.X - x;
            int yDif = point1.Y - y;

            base.setPosition(x, y);

            for (int i = 0; i < objs.Length; i++) {

                objs[i].move(xDif, yDif);

                //objs[i].setPosition(point1.X + xDif[i],
                //                    point1.Y + yDif[i]);
            }
            //checkBound();
        }
        public DrawableObject[] ungroup() {
            return objs;
        }
        public override string ToString() {
            string res = name + " {\n";
            foreach (DrawableObject a in objs) {
                res += a + "\n";
            }
            res += "}";
            return res;
        }


        public override void fillMyNode(TreeNode node) {
            base.fillMyNode(node);
            foreach (DrawableObject a in objs) {
                a.fillMyNode(node.Nodes.Add(""));
            }
        }

    }

}
