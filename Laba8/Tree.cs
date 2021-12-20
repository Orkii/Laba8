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
    class Tree {
        StorageListT storage;
        StorageListT selectedStorage;
        public TreeView tree;
        static Color selectColor = Color.Blue;
        static Color standartColor = Color.White;
        public Tree(StorageListT stor, StorageListT selec) {
            storage = stor;
            selectedStorage = selec;

            storage.smthChanegd += new EventHandler(smthChanged);
            selectedStorage.smthChanegd += new EventHandler(smthChangedInSelect);
        }
        public void smthChanged(object sender, EventArgs e){
            draw(tree);
        }
        public void smthChangedInSelect(object sender, EventArgs e) {
            select();
        }
        public void select() {
            
            foreach (TreeNode a in tree.Nodes) {
                colorRec(a);
            }
        }
        private void colorRec(TreeNode node) {
            node.BackColor = standartColor;
            foreach (DrawableObject b in selectedStorage) {
                DrawableObject temp = b;

                DGroup d = null;
                do {
                    d = null;
                    d = (temp as DGroup);
                    if (d != null) {

                        if (d.selected != null) {
                            //Console.WriteLine("SMTH");
                            temp = d.selected;
                        }
                        else {
                            break;
                        }
                    }
                    //Console.WriteLine(d);
                } while (d != null);
                d = null;

                if (temp == node.Tag) {
                    //Console.WriteLine(temp);
                    node.BackColor = selectColor;
                    foreach (TreeNode c in node.Nodes) {
                        c.BackColor = selectColor;
                    }
                }
            }

            foreach (TreeNode a in node.Nodes) {
                colorRec(a);  
            }
        }
        public void draw(TreeView tree = null) {

            if (tree == null) tree = this.tree;

            tree.Nodes.Clear();

            

            foreach (DrawableObject a in storage) {
                TreeNode node = tree.Nodes.Add("");
                a.fillMyNode(node);

            }    
        }
    
    
        public void choose() {
            if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift) {
                selectedStorage.clear();
            }
            bool needAdd = true;
            DrawableObject ob = (DrawableObject)(tree.SelectedNode.Tag);
            foreach (DrawableObject a in selectedStorage) {
                if (a == ob) needAdd = false;
            }
            if (needAdd == true) {
                selectedStorage.add((DrawableObject)(tree.SelectedNode.Tag));
            }


        }
    }
}
