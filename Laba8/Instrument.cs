using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;


namespace Laba8 {
    abstract class Instrument {
        public abstract bool down(object sender, MouseEventArgs e);
        public abstract void move(object sender, MouseEventArgs e);
        public abstract void up(object sender, MouseEventArgs e);
    }

    class InsCreate : Instrument {

        public override bool down(object sender, MouseEventArgs e) {
            //Console.WriteLine("Create down");

            Info.selectedObj.clear();


            List<DrawableObject> st = new List<DrawableObject>();

            foreach(DrawableObject dr in Info.storage) {
                
                st.Add(dr);
            }

            DrawableObject obj  = Info.creator.create(e.Location, e.Location, Info.color.Color);

            if (obj == null) return false;
            Info.selectedObj.add(obj);

            Info.storage.add(obj);

            return true;
        }
        public override void move( object sender, MouseEventArgs e) {

            Info.selectedObj.get(0).point2 = e.Location;
            Info.selectedObj.get(0).checkExitByResize(((Control)sender).ClientRectangle);
        }
        public override void up(object sender, MouseEventArgs e) {
            DrawableObject obj = Info.selectedObj.get(0);
            if (obj.point1 == obj.point2) {
                Info.selectedObj.remove(obj);
                Info.storage.remove(0);
            }

        }
    }

    class InsResize : Instrument {
        DrawableObject obj;
        public override bool down(object sender, MouseEventArgs e) {
            //Console.WriteLine("Resize down");

            foreach (DrawableObject a in Info.selectedObj) {
                if (a.startResize(sender, e) == true) {
                    obj = a;
                    return true;
                }
            }
            return false;
        }
        public override void move(object sender, MouseEventArgs e) {
            obj.resize(sender, e);
            Info.selectedObj.get(0).checkExitByResize(((Control)sender).ClientRectangle);
        }
        public override void up(object sender, MouseEventArgs e) {
            

        }
    }

    class InsMove : Instrument {
        DrawableObject obj;
        Point startPoint;
        Point prevPoint;
        public override bool down(object sender, MouseEventArgs e) {
            //Console.WriteLine("Move down");
            startPoint = e.Location;
            prevPoint = e.Location;
            foreach (DrawableObject a in Info.selectedObj) {
                if (a.click(sender, e) == true) {
                    obj = a;
                    return true;
                }
            }
            return false;
        }
        public override void move(object sender, MouseEventArgs e) {
            foreach (DrawableObject a in Info.selectedObj) {
                a.move(e.Location.X - prevPoint.X, e.Location.Y - prevPoint.Y);
            }
            prevPoint = e.Location;
        }
        public override void up(object sender, MouseEventArgs e) {
            
            foreach (DrawableObject a in Info.selectedObj) {
                //Info.selectedObj.get(0).
                a.checkExitByMove(((Control)sender).ClientRectangle);
            }
        }
    }


}
