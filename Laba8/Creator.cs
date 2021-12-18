using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Laba8 {

    class GroupCreator : Creator {
        List<Creator> creators;// = new List<Creator>();

        public GroupCreator() {
            creators = new List<Creator>();
            allow = new string[0];
        }
        public void addCreator(Creator cr) {
            //creators.Add(cr);

            List<Creator> a = new List<Creator>();
            a.Add(cr);
            foreach(Creator b in creators) {
                a.Add(b);
            }

            creators = a;

            string[] al;

            if (cr.allowToCreate() == null) return;
            try {

                al = new string[allow.Length + cr.allowToCreate().Length];
            }
            catch {
                return;
            }

            int i = 0;
            for (; i < allow.Length; i++) {
                al[i] = allow[i];
            }
            string[] tmp = cr.allowToCreate();
            for (; i < allow.Length + tmp.Length; i++) {
                al[i] = tmp[i - allow.Length];
            }
            allow = al;

        }
        public void clear() {
            creators.Clear();
        }

        public override string[] allowToCreate() {
            List<string> ret = new List<string>();
            foreach(Creator a in creators) {
                foreach (string b in a.allowToCreate()) {
                    ret.Add(b);
                }
            }
            return ret.ToArray();
        }

        public override DrawableObject create(Point point1_, Point point2_, Color color_) {
            DrawableObject res;
            foreach (Creator a in creators) {
                res = a.create(point1_, point2_, color_);
                if (res != null) return res;
            }
            return null;
        }

        public override DrawableObject create(Point point1_, Point point2_, Color color_, string state_) {
            DrawableObject res;
            foreach (Creator a in creators) {
                res = a.create(point1_, point2_, color_, state_);
                if (res != null) return res;
            }
            return null;
        }

        public override DrawableObject fromString(List<string> str) {
            DrawableObject res;
            foreach (Creator a in creators) {
                if (str.Count == 0) return null;
                res = a.fromString(str);
                if (res != null) return res;
            }
            return null;
        }

        public override bool setState(string str) {
            foreach(Creator a in creators) {
                if (a.setState(str) == true) {
                    return true;
                }
            }
            return false;
        }

    }

    class PlugCreator : Creator {
        public override string[] allowToCreate() {
            return new string[0];
        }

        public override DrawableObject create(Point point1_, Point point2_, Color color_) {
            return null;
        }

        public override DrawableObject create(Point point1_, Point point2_, Color color_, string state_) {
            return null;
        }

        public override DrawableObject fromString(List<string> str) {
            
            if (str[0].Contains("DGroup")) {
                List<DrawableObject> list = new List<DrawableObject>();
                str.RemoveAt(0);
                while (!str[0].Contains('}')) {
                    DrawableObject a = Info.creator.fromString(str);
                    if (a != null) {
                        list.Add(a);
                    }
                }
                str.RemoveAt(0);
                return new DGroup(list.ToArray());
            }
            

            string tmp = str[0].Split(' ')[0];
            foreach (string a in Info.needDDLMesage) {
                if (a == tmp) {
                    str.RemoveAt(0);
                    return null;
                }
            }
            Info.needDDLMesage.Add(tmp);
            str.RemoveAt(0);

            

            return null;
        }
        public override bool setState(string str) {
            return false;
        }
    }
}
