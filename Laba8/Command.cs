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
    public abstract class Command {
        public abstract void execute();
        public abstract void unexecute();
        public abstract Command clone();
    }


    public class CCreate : Command {
        Point point1;
        Point point2; 
        Color color;
        string creatorState;
        DrawableObject obj;
        public CCreate(Point point1_, Point point2_, Color color_) {
            point1 = point1_;
            point2 = point2_;
            color = color_;
            creatorState = Info.creator.stateS;
        }

        public override void execute() {
            obj = Info.creator.create(point1, point2, color, creatorState);
            Info.storage.add(obj);
        }

        public override void unexecute() {
            if (obj == null) return;
            Info.storage.remove(obj);
            obj = null;
        }
        public override Command clone() {
            return null;    
        }
    }



    public class CMove : Command {
        Point point1;
        Point point2;
        DrawableObject obj;

        public CMove(Point point1_, Point point2_, DrawableObject obj_) {
            point1 = point1_;
            point2 = point2_;
            obj = obj_;
        }

        public override void execute() {
            if (obj != null) {

                obj.move(point2.X - point1.X, point2.Y - point1.Y);
            }
        }
        public override void unexecute() {
            if (obj != null) {
                obj.move(point1.X - point2.X, point1.Y - point2.Y);
            }
        }
        public override Command clone() {
            return new CMove(point1, point2, obj);
        }
    }
}
