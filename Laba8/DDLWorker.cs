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
using System.Reflection;

namespace Laba8 {
    static class DDLWorker {

        public static EventHandler smthChanged;
        public static void loadDDL(string path) {



            Assembly q;
            try {
                q = Assembly.LoadFile(path);
            }
            catch {
                Console.WriteLine("Неверный файл DLL");
                return;

            }
            List<TypeInfo> c = q.DefinedTypes.ToList();
            foreach (var t in c) {
                Console.WriteLine("C = " + t);
            }
            Console.WriteLine("Q = " + q.DefinedTypes);
            if (q != null) Console.WriteLine("Good1");

            Docker o = (Docker)q.CreateInstance("DDocker"); //TestCreator // StandartCreator

            ((GroupCreator)Info.creator).addCreator(o.getCreator());

            //Info.storage.clear();
            //Info.storage.load(Info.fileName, Info.creator);

            string[] a = Info.creator.allowToCreate();

            //string[] thigCanCreate = new string[a.Length];
            Info.thinhCanCreate.Clear();
            foreach (string s in a) {
                Info.thinhCanCreate.Add(s);
            }
            //Info.thinhCanCreate.Clear();


            if (smthChanged!= null) smthChanged.Invoke(null, null);

        }
    }
}


