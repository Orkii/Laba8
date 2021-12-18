using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace Laba8 {
    public class StorageListT {

        public EventHandler smthChanegd;
        private Element firstElement;
        public int elementCount { get; private set; }
        public StorageListT(DrawableObject element) {
            firstElement = new Element(element);
            elementCount = 1;
        }
        public StorageListT() {
            elementCount = 0;
        }
        public DrawableObject get(int index) {
            if ((index >= elementCount) || (index < 0)) return default(DrawableObject);
            Element result = firstElement;
            for (int i = 0; i < index - 1; i++) {
                result = result.nextElement;
            }

            if (smthChanegd != null) smthChanegd.Invoke(this, null);
            return result.thisElement;
        }
        public DrawableObject getRemove(int index) {
            
            if ((index >= elementCount) || (index < 0)) return default(DrawableObject);
            Element prev = null;
            Element result = firstElement;
            for (int i = 0; i < index - 1; i++) {
                prev = result.nextElement;
                result = result.nextElement;
            }
            prev.nextElement = result.nextElement;

            if (smthChanegd != null) smthChanegd.Invoke(this, null);
            return result.thisElement;
        }

        public void show() {
            Console.WriteLine("///////////");
            foreach(DrawableObject a in this) {
                Console.WriteLine(a);
            }
            Console.WriteLine("///////////");
        }

        public void add(DrawableObject element, int index = 0) {
            
            if ((index > elementCount) || (index < 0)) {
                Console.WriteLine("Error. StorageListT. Get. Incorect position.");
                return;
            }

            if (index == 0) {
                firstElement = new Element(element, firstElement);
                elementCount++;
                if (smthChanegd != null) smthChanegd.Invoke(this, null);
                return;
            }
            Element result = firstElement;
            for (int i = 0; i < index - 1; i++) {
                result = result.nextElement;

            }
            result.nextElement = new Element(element, result.nextElement);
            elementCount++;
            if (smthChanegd != null) smthChanegd.Invoke(this, null);
        }

        public void addEnd(DrawableObject element) {
            add(element, elementCount);

        }
        private Element iteratorElement;
        public void remove(int index) {
            
            if (index == 0) {
                firstElement = firstElement.nextElement;
                elementCount--;
                return;
            }

            Element a = getT(index);
            a.nextElement = a.nextElement.nextElement;
            elementCount--;

            if (smthChanegd != null) smthChanegd.Invoke(this, null);
            return;
        }
        public void remove(DrawableObject obj) {
            
            Console.WriteLine("remove");
            if (obj == null) return;
            if (firstElement == null) return;


            if (firstElement.thisElement.Equals(obj)) {
                Console.WriteLine("remove first");
                firstElement = firstElement.nextElement;
                elementCount--;
                return;
            }
            Element now = firstElement;

            Element prev = null;
            while (!now.thisElement.Equals(obj)) {

                prev = now;
                now = now.nextElement;
                if (now == null) return;
            }
            Console.WriteLine("remove smth");
            prev.nextElement = now.nextElement;
            elementCount--;
            if (smthChanegd != null) smthChanegd.Invoke(this, null);
            return;
        }
        private Element getT(int index) {
            Element nowEl = firstElement;
            for (int i = 0; i < index - 1; i++) {
                nowEl = nowEl.nextElement;
            }
            return nowEl;
        }
        public int length() {
            Element nowEl = firstElement;
            int i = 0;
            while (nowEl != null) {
                i++;
                nowEl = nowEl.nextElement;
            }
            return i;
        }
        public void clear() {
            if (smthChanegd != null) smthChanegd.Invoke(this, null);
            firstElement = null;
            elementCount = 0;
            if (smthChanegd != null) smthChanegd.Invoke(this, null);
        }


        Stack<Element> stackOfEL = new Stack<Element>();
        public void first() {
            stackOfEL.Push(iteratorElement);
            iteratorElement = firstElement;
        }
        public bool eol() {
            if (iteratorElement == null) {
                iteratorElement = stackOfEL.Pop();
                return true;
            }
            return false;
        }
        public void next() {
            iteratorElement = iteratorElement.nextElement;

        }
        public int getIndex(DrawableObject elem) {
            Element nowEl = firstElement;
            int i = 0;
            while (nowEl != null) {
                i++;
                if (nowEl.thisElement.Equals(elem) == true) {
                    return i - 1;
                }
                nowEl = nowEl.nextElement;
            }
            return (i - 1);
        }
        public DrawableObject[] ToArray() {
            DrawableObject[] mas = new DrawableObject[elementCount];
            int i = 0;
            foreach(DrawableObject a in this) {
                mas[i] = a;
                i++;
            }
            return mas;
        }
        public DrawableObject iteratorGet() {
            return iteratorElement.thisElement;
        }

        public IEnumerator<DrawableObject> GetEnumerator() {
            for (first(); !eol(); next())
                yield return iteratorElement.thisElement;
        }
        class Element {
            public DrawableObject thisElement;
            public Element nextElement;
            public Element(DrawableObject thisElement_, Element nextElement_ = null) {
                thisElement = thisElement_;
                nextElement = nextElement_;
            }
            public DrawableObject next() {
                return nextElement.thisElement;
            }
        }



        public void save(string fileName) {
            StreamWriter sw;
            try {
                File.Delete(fileName);
                File.Create(fileName).Close();

                sw = new StreamWriter(fileName);
            }
            catch {
                return;
            }
            foreach (DrawableObject a in this) {
                sw.WriteLine(a);
            }
            sw.Close();
        }
        public void load(string filePath, Creator creator) {
            StreamReader sr;
            try {
                sr = new StreamReader(filePath);
            }
            catch {
                return;
            }
            List<string> str = sr.ReadToEnd().Split('\n').ToList();
            sr.Close();
            clear();

            while (str.Count > 0) {
                DrawableObject obj = (DrawableObject)creator.fromString(str);
                if (obj != null) {
                    add(obj);
                }
            }
            if (smthChanegd != null) smthChanegd.Invoke(this, null);
        }



    }
}
