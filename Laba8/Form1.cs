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
using System.IO;



namespace Laba8 {
    public partial class Form1 : Form {
        Instrument instrument;
        DrawableObject example;
        Tree tree;

        List<DrawableObject> nowS = new List<DrawableObject>();

        private void stickChanged(object sender, EventArgs e) {
            
            if (sender as DrawableObject == null) return;
            if (e as MyEventArgs == null) return;

            MyEventArgs ee = e as MyEventArgs;
            DrawableObject obj = sender as DrawableObject;

            nowS.Add(obj);
            foreach (DrawableObject a in Info.storage) {

                if (a == obj) {//Если сам себя двигает
                    goto endP;
                }

                if (a as DGroup != null) {
                    if (((DGroup)a).isContain(obj)) {
                        Console.WriteLine("Group");
                        nowS.Add(a);
                        goto endP;
                    }
                }


                foreach (DrawableObject b in nowS) {//Если уже подвигали
                    Console.WriteLine("Same");
                    if (a == b) goto endP;
                }

                bool goEnd = false;
                foreach (DrawableObject b in Info.selectedObj) {//Если уже подвигали
                    Console.WriteLine("Selected");
                    if (a == b) {
                        nowS.Add(a);
                        goEnd = true;
                        //goto endP;     
                    }
                    //if (obj == b) goto endP;
                }

                if (goEnd == true) goto endP;

                if (obj.checkContact(a)) {//Двигаем
                    nowS.Add(a);
                    a.move(ee.x, ee.y);
                    a.checkExitByMove(panel1.ClientRectangle);
                }
            endP:;
            }



            //Console.WriteLine((e as MyEventArgs).x + "   " + (e as MyEventArgs).y);
            //
            //needToStickSender.add((sender as DrawableObject), needToStickSender.elementCount);
            //if (needToStickE == null) needToStickE = e as MyEventArgs;
            //needToStickE = e as MyEventArgs;

        }

        private void stopStickThing() {
            nowS.Clear();
            //needToStickSender.clear();
        }
        
        private void ddlChanged(object sender, EventArgs e) {

            comboBox1.Items.Clear();
            comboBox1.ResetText();
            //Console.WriteLine("ТУТ");
 

            foreach (var a in Info.thinhCanCreate) {
                comboBox1.Items.Add(a);
            }

        }

        public Form1() {
            InitializeComponent();


            button1.BackColor = Color.Red;

            //Info.creator = new StandartCreator();

            DDLWorker.smthChanged += new EventHandler(ddlChanged);

            ((GroupCreator)Info.creator).addCreator(new PlugCreator());

            DDLWorker.loadDDL(Info.standartDLL);

            //Info.storage.load(Info.fileName, Info.creator);

            if (Info.needDDLMesage.Count != 0) {
                string mes = "Не хватает Следующих DDL:\n";

                foreach (string a in Info.needDDLMesage) {
                    mes += a + "\n";
                }
                MessageBox.Show(mes);
                Info.needDDLMesage.Clear();
            }


            tree = new Tree(Info.storage, Info.selectedObj);
            tree.tree = treeView1;
            //tree.draw(treeView1);


            typeof(Panel).InvokeMember("DoubleBuffered", // Двойная буферизация, убирает мерцание
                            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, panel1, new object[] { true });
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {
            foreach (DrawableObject a in Info.storage) {
                a.draw(sender, e);
                //Point[,] b = a.getBound();
                //for(int i = 0; i < b.GetLength(0); i++) {
                //    e.Graphics.DrawLine(new Pen(Color.Red, 5), b[i, 0], b[i, 1]);
                //}
            }
            foreach (DrawableObject a in Info.selectedObj) {
                a.draw(sender, e);
                a.drawChooseLine(sender, e);
                a.drawResizeDot(sender, e);
                //Point[,] b = a.getBound();
                //for (int i = 0; i < b.GetLength(0); i++) {
                //    e.Graphics.DrawLine(new Pen(Color.Red, 5), b[i, 0], b[i, 1]);
                //}
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e) {
            instrument = new InsResize();
            Console.WriteLine("///////////////////");
            if (instrument.down(sender, e)) {
                return;
            }



            instrument = new InsMove();
            if (instrument.down(sender, e)) {
                return;
            }
            instrument = null;

            Console.WriteLine("Select");
            foreach (DrawableObject a in Info.storage) {//Select
                if (a.click(sender, e) == true) {
                    if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift) {
                        Info.selectedObj.clear();
                    }

                    Info.selectedObj.add(a);
                    panel1.Invalidate();
                    return;
                }
            }

            Console.WriteLine("Create");
            instrument = new InsCreate();
            if (instrument.down(sender, e) == false) {
                instrument = null;
            }

            

        }
        private void panel1_MouseMove(object sender, MouseEventArgs e) {
            Console.WriteLine("///////////");
            
            if (instrument != null) {
                Console.WriteLine("Move");
                instrument.move(sender, e);
                panel1.Invalidate();
            }
            stopStickThing();
            //doStickThing();
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e) {
            if (instrument != null) {
                instrument.up(sender, e);
            }
            if (createStick == true) {
                if (instrument as InsCreate != null) {
                    
                    if (Info.selectedObj.get(0) != null) {
                        Info.stickObj.add(Info.selectedObj.get(0));
                        Info.selectedObj.get(0).onBeforeMoved += new EventHandler(stickChanged);
                    }
                }
            }
            instrument = null;

            bool t = false;


            if (Info.selectedObj.elementCount != 0) {
                //Console.WriteLine("Diam = " + Info.selectedObj.get(0).rectangle.Width);

                foreach (DrawableObject a in Info.storage) {
                    if (Info.selectedObj.get(0).checkContact(a) == true) t = true;
                }
            }

            if (t == true) Console.WriteLine("Touched");
            else Console.WriteLine("Not touched");

            panel1.Invalidate();
        }

        private void group_Button_Click(object sender, EventArgs e) {

            if (Info.selectedObj.elementCount <= 1) return;
            foreach(DrawableObject a in Info.selectedObj) {
                Info.storage.remove(a);
            }
            DrawableObject group = new DGroup(Info.selectedObj.ToArray());
            Info.selectedObj.clear();
            Info.storage.add(group);
            Info.selectedObj.add(group);
            Info.storage.save(Info.fileName);
            panel1.Invalidate();
        }

        private void clear_Button_Click(object sender, EventArgs e) {
            Info.selectedObj.clear();
            Info.storage.clear();
            Info.stickObj.clear();
            Info.storage.save(Info.fileName);
            panel1.Invalidate();
        }
        
        private void Ungroup_Button_Click(object sender, EventArgs e) {

            foreach (DrawableObject a in Info.selectedObj) {
                try {
                    DrawableObject[] mas = ((DGroup)a).ungroup();
                    Info.storage.remove(a);
                    Info.selectedObj.remove(a);
                    Info.stickObj.remove(a);
                    foreach (DrawableObject b in mas) {
                        Info.storage.add(b);
                        Info.selectedObj.add(b);
                        Info.stickObj.add(b);
                    }
                }
                catch {

                }
            }
            Info.storage.save(Info.fileName);
            panel1.Invalidate();

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                foreach(DrawableObject a in Info.selectedObj) {
                    Info.storage.remove(a);
                    Info.stickObj.remove(a);
                }
                Info.selectedObj.clear();
                Info.storage.save(Info.fileName);
                panel1.Invalidate();
            }
            else if ((e.KeyCode == Keys.A) && (e.Modifiers == Keys.Control)) {
                Info.selectedObj.clear();
                foreach(DrawableObject a in Info.storage) {
                    Info.selectedObj.add(a);
                    //Console.WriteLine(a);
                }
                panel1.Invalidate();
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            Info.creator.setState(comboBox1.SelectedItem.ToString());
            setExample();
        }

        private void setExample() {
            example = Info.creator.create(new Point(5, 5),
                new Point(panel3.Size.Width - 5, panel3.Size.Height - 5), Info.color.Color);
            panel3.Invalidate();
        }


        private void dLLToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "DLL|*.DLL";
            fd.ShowDialog();

            string file = fd.FileName;

            Console.WriteLine(file);

            DDLWorker.loadDDL(file);

            panel1.Invalidate();
        }


        private void Form1_Load(object sender, EventArgs e) {

        }

        private void color_button_Click(object sender, EventArgs e) {
            Info.color.ShowDialog();
            color_button.BackColor = Info.color.Color;

            foreach (DrawableObject a in Info.selectedObj) {
                a.color = Info.color.Color;
            }
            panel1.Invalidate();
            setExample();
            panel3.Invalidate();
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e) {
            foreach (DrawableObject a in Info.storage) {//Select
                if (a.click(sender, e) == true) {
                    
                    Info.selectedObj.clear();
                    

                    Info.selectedObj.add(a);
                    panel1.Invalidate();
                    return;
                }
            }
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e) {
            Console.WriteLine("Text");
        }

        private void panel3_Paint(object sender, PaintEventArgs e) {
            if (example != null) {
                example.draw(sender, e);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "txt|*.txt";
            fd.ShowDialog();

            string file = fd.FileName;

            Console.WriteLine("File = " + file);

            Info.storage.save(file);

            try {
                File.AppendAllText(file, this.Width + ";" + this.Height);
            }
            catch {

            }

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "txt|*.txt";
            fd.ShowDialog();

            string file = fd.FileName;
            Console.WriteLine("File = " + file);

            Info.selectedObj.clear();
            Info.storage.clear();

            Info.storage.load(file, Info.creator);

            if (file == null) return;

            string[] a = File.ReadAllText(file).Split('\n');
            string b = a[a.Length - 1];
            a = b.Split(';');

            this.Size = new Size(Convert.ToInt32(a[0]), Convert.ToInt32(a[1]));

            panel1.Invalidate();
        }



        private void Form1_ResizeEnd(object sender, EventArgs e) {
            foreach(DrawableObject a in Info.storage) {
                a.checkExitByMove(panel1.ClientRectangle);
            }
            panel1.Invalidate();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {

            tree.choose();
            panel1.Invalidate();
        }

        bool createStick = false;
        private void button1_Click(object sender, EventArgs e) {
            if (createStick == false) {
                button1.BackColor = Color.Green;
                createStick = true;
            }
            else {
                button1.BackColor = Color.Red;
                createStick = false;
            }
        }
    }
}
