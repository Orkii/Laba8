using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba8 {
    static class Info {
        public static Stack<Command> commands = new Stack<Command>();
        public static StorageListT storage = new StorageListT();
        public static StorageListT selectedObj = new StorageListT();
        public static StorageListT stickObj = new StorageListT();
        public static Creator creator = new GroupCreator();// = new TestCreator();
        public static string standartDLL = @"C:\Users\Orkii\Desktop\OOP\Laba7\def\def\bin\Debug\def.dll";

        public static ColorDialog color = new ColorDialog();
        public static List<string> thinhCanCreate = new List<string>();

        public static EventHandler DDLChanged;

        public static string fileName = "Data.txt";


        public static List<string> needDDLMesage = new List<string>();
    }
}
