using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gestion_de_fichier_Form
{
    public partial class FileGestion : Form
    {
        User currentUser;
        Directory currentDir;
        List<int> history;
        Form auForm;
        ListViewItem selectedItem;
        ListViewItem toCopyItem;
        bool cut;
        bool dragTreeView;


        private TextBox txt = new TextBox { BorderStyle = BorderStyle.FixedSingle, Visible = false };
        /*
         * fix context curse position //
         * fix copy in item ///
         * fix copy in listView  ///
         * add recursofoty to document for delete ///
         * add recursivity for copy ///
         * add inner listview drag   
         * add animation to list view drag
         * (optional) add the bottom bar ùù ùù
         * add the selectAll reverseSelect unSelectAll functions and button ùù
         * fix edit text position for all 'affichage'
         * fix language to french //
         * robbin button disabling ///
             */

        public FileGestion(int ID, Form form)
        {
            auForm = form;
            currentUser = new User(ID);
            currentDir = Directory.ROOT;

            InitializeComponent();
            pastToolStripMenuItem.Enabled = false;
            pastToolStripMenuItem1.Enabled = false;
            copierRibbonButton.Enabled = false;
            collerRibbonButton2.Enabled = false;
            couperRibbonButton.Enabled = false;
            supprimerRibbonButton5.Enabled = false;
            renameRibbonButton.Enabled = false;
            ribbonButton1.Enabled = false;
            ribbonButton4.Enabled = false;
            listViewFile.Controls.Add(txt);
            txt.Leave += new EventHandler(mouseTextLeave);
            history = new List<int>();
            history.Add(1);
            cut = true;
        }

        //Initialisation Methodes
        #region InitialisasionMethodes
        private void FileGestion_Load(object sender, EventArgs e)
        {
            listViewFile.AllowDrop = true;

            fillListView();
            fillTreeView();
        }
        private void fillTreeView()
        {

            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.Add("1", "Root", 1);
            fillDirectory(Directory.ROOT, treeView.Nodes[0]);
            treeView.EndUpdate();
            treeView.ExpandAll();

        }        
        private void fillListView()
        {


            pathTextBox.Text = currentDir.Path;
            listViewFile.Items.Clear();
            var listD = currentDir.ListDirectory();
            var listFile = currentDir.ListFile();
            if (listD != null)
            {
                foreach (var dir in listD)
                {
                    int typeIcon = (dir.Empty()) ? 0 : 1;
                    ListViewItem dirItem = new ListViewItem(dir.Name, typeIcon);
                    dirItem.SubItems.Add("Directory");
                    dirItem.SubItems.Add(dir.getSize().ToString() + " Ko");
                    dirItem.SubItems.Add("perm");
                    dirItem.SubItems.Add(dir.ID.ToString());
                    listViewFile.Items.Add(dirItem);
                }
            }

            if (listFile != null)
            {
                foreach (var file in listFile)
                {
                    int typeIcon;
                    switch (file.exe)
                    {
                        case "png":
                            typeIcon = 2;
                            break;
                        case "txt":
                            typeIcon = 4;
                            break;
                        case "exe":
                            typeIcon = 3;
                            break;
                        default:
                            typeIcon = 5;
                            break;
                    }
                    ListViewItem fileItem = new ListViewItem(file.name + "." + file.exe, typeIcon);
                    fileItem.SubItems.Add(file.type);
                    fileItem.SubItems.Add(file.size.ToString() + " Ko");
                    fileItem.SubItems.Add("perm");
                    fileItem.SubItems.Add(file.ID.ToString());
                    listViewFile.Items.Add(fileItem);
                }
            }
        }

        #endregion

        // BUTTON METHODES
        #region ButtonMethodes
        private void ReButton_Click(object sender, EventArgs e)
        {

            if (!(history.Count == 1))
            {
                history.RemoveAt(history.Count - 1);
            }

            currentDir = Db.getDirById(history[history.Count - 1]);

            fillListView();

        }
        private void AccederButton_Click(object sender, EventArgs e)
        {
            int id = currentDir.IdByPath(pathTextBox.Text);
            if (id != -1)
            {
                currentDir = Db.getDirById(id);
                history.Add(currentDir.ID);
                fillListView();
            }
            else
            {
                pathTextBox.Text = currentDir.Path;
            }
        }
        private void formClosed(object sender, FormClosedEventArgs e)
        {
            auForm.Close();
        }
        private void NewDirButton_Click(object sender, EventArgs e)
        {
            var item = new ListViewItem("New Folder", 0);
            var dir = currentDir.creatChildDirectory(item.SubItems[0].Text + "*", currentDir.Path + "/" + item.SubItems[0].Text);
            item.SubItems.Add("Directory");
            item.SubItems.Add(dir.getSize().ToString() + " Ko");
            item.SubItems.Add("perm");
            item.SubItems.Add(dir.ID.ToString());
            listViewFile.Items.Add(item);
            foreach (ListViewItem item1 in listViewFile.SelectedItems)
            {
                item1.Selected = false;
            }
            listViewFile.Items[listViewFile.Items.Count - 1].Selected = true;
            //ListViewHitTestInfo hit = listViewFile.HitTest(e.Location);

            //////
            /// here to set tex box
            /////

            Rectangle rowBounds = listViewFile.Items[listViewFile.Items.Count - 1].SubItems[0].Bounds;
            Rectangle labelBounds = listViewFile.Items[listViewFile.Items.Count - 1].GetBounds(ItemBoundsPortion.Label);
            int leftMargin = labelBounds.Left - 1;
            txt.Bounds = new Rectangle(rowBounds.Left + leftMargin, rowBounds.Top, listViewFile.Columns[0].Width - 17, rowBounds.Height);
            txt.Text = listViewFile.Items[listViewFile.Items.Count - 1].SubItems[0].Text;
            txt.SelectAll();
            txt.Visible = true;
            txt.Focus();

        }
        #endregion

        //Mouse event
        #region MouseEvent
        private void listView_mouseClick(object sender, MouseEventArgs e)
        {

            if (listViewFile.SelectedItems.Count == 1)
            {
                if (listViewFile.SelectedItems[0].SubItems[1].Text == "Directory")
                {
                    int id = Convert.ToInt32(listViewFile.SelectedItems[0].SubItems[4].Text);
                    currentDir = Db.getDirById(id);
                    history.Add(currentDir.ID);
                    fillListView();
                }
            }
        }
        private void listViewClicked(object sender, MouseEventArgs e)
        {
            /*ListViewHitTestInfo hit = listViewFile.HitTest(e.Location);
            Rectangle rowBounds = hit.SubItem.Bounds;
            Rectangle labelBounds = hit.Item.GetBounds(ItemBoundsPortion.Label);
            int leftMargin = labelBounds.Left - 1;
            txt.Bounds = new Rectangle(rowBounds.Left + leftMargin, rowBounds.Top, listViewFile.Columns[0].Width - 17, rowBounds.Height);
            txt.Text = hit.SubItem.Text;
            txt.SelectAll();
            txt.Visible = true;
            txt.Focus();*/
        }
        private void nodeDoubleCllicked(object sender, TreeNodeMouseClickEventArgs e)
        {
            int id = Convert.ToInt32(e.Node.Name);

            if (e.Node.ImageIndex == 1 || e.Node.ImageIndex == 0)
            {
                currentDir = Db.getDirById(id);
                history.Add(id);
                fillListView();
            }
        }
        private void mouseTextLeave(object sender, EventArgs e)
        {
            var subitem = listViewFile.SelectedItems[0].SubItems;
            txt.Visible = false;
            subitem[0].Text = changeName(Convert.ToInt32(subitem[4].Text), txt.Text);
            fillTreeView();
            fillListView();
        }
        private void listView_mouseDown(object sender, MouseEventArgs e)
        {
            var hit = listViewFile.HitTest(e.X, e.Y);
            Point loc = new Point(e.X, e.Y);
            loc.Offset(splitContainer1.Location);
            if (e.Button == MouseButtons.Right)
            {
                if (hit.Item != null)
                {
                    listViewItemContextMenuStrip.Show(this,loc.X + splitContainer1.SplitterDistance, loc.Y);
                    selectedItem = hit.Item;
                }
                else
                {
                    listViewContextMenuStrip.Show(this, loc);
                }
            }else if(e.Button == MouseButtons.Left)
            {
                if (hit.Item != null)
                {
                    hit.Item.Selected = true;
                    selectedItem = hit.Item;
                    copierRibbonButton.Enabled = true;
                    couperRibbonButton.Enabled = true;
                    supprimerRibbonButton5.Enabled = true;
                    renameRibbonButton.Enabled = true;
                    ribbonButton1.Enabled = true;
                    ribbonButton4.Enabled = true;
                }
                else
                {
                    copierRibbonButton.Enabled = false;
                    couperRibbonButton.Enabled = false;
                    supprimerRibbonButton5.Enabled = false;
                    renameRibbonButton.Enabled = false;
                    ribbonButton1.Enabled = false;
                    ribbonButton4.Enabled = false;
                }

            }
        }

        private void treeView_mouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var hit = treeView.HitTest(e.X, e.Y);
                Point loc = new Point(e.X, e.Y);
                ListViewItem item;

                loc.Offset(splitContainer1.Location);
                if (hit.Node != null)
                {
                    listViewItemContextMenuStrip.Show(this, loc);
                    if (hit.Node.ImageIndex == 0 || hit.Node.ImageIndex == 1)
                    {
                        var dir = Db.getDirById(Convert.ToInt32(hit.Node.Name));
                        item = new ListViewItem(dir.Name, hit.Node.ImageIndex);
                        item.SubItems.Add("Directory");
                        item.SubItems.Add(dir.getSize().ToString() + " Ko");
                        item.SubItems.Add("perm");
                        item.SubItems.Add(dir.ID.ToString());                      
                    }
                    else
                    {
                        var file = Db.getFileById(Convert.ToInt32(hit.Node.Name));
                        item = new ListViewItem(file.name, hit.Node.ImageIndex);
                        item.SubItems.Add(file.type);
                        item.SubItems.Add( file.size.ToString() + " Ko");
                        item.SubItems.Add("perm");
                        item.SubItems.Add(file.ID.ToString());
                    }
                    selectedItem = item;
                }
            }
        }
        #endregion

        // Drag button
        #region ButtonDragEvent
        //Drag from treeView to ListView
        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            dragTreeView = true;
            DoDragDrop(e.Item, DragDropEffects.Move);
            
        }
        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            
        }

        private void dropListView(object sender, DragEventArgs e)
        {
            if (dragTreeView)
            {
                bool trans = true;
                var NewNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                var loc = new Point(e.X, e.Y);
                ListViewItem item;
                loc.Offset(listViewFile.Location);
                if (e.Data.GetDataPresent(typeof(List<ListViewItem>)))
                {
                    var items = (List<ListViewItem>)e.Data.GetData(typeof(List<ListViewItem>));
                    if (items[0].SubItems[1].Text == "Directory")
                    {
                        var parent = Db.getDirById(Convert.ToInt32(items[0].SubItems[4].Text));
                        if (NewNode.ImageIndex == 1 || NewNode.ImageIndex == 0)
                        {
                            Directory dir = Db.getDirById(Convert.ToInt32(NewNode.Name));

                            if (dir.ParentID == parent.ID || dir.ID == parent.ID)
                            {
                                return;
                            }

                            if (isParent(dir.ID, parent.ID))
                            {
                                MessageBox.Show("EROR : la destination est un fils de dossier ");
                            }
                            else
                            {
                                if (parent.ChildNameExist(dir.Name))
                                {
                                    DialogResult res = MessageBox.Show("Nom deja exist , rennomer", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                    if (res == DialogResult.Cancel)
                                    {
                                        trans = false;
                                    }
                                }

                                if (trans)
                                {
                                    parent.Transform(dir.ID, dir.Name, false);
                                    changeName(dir.ID, dir.Name);
                                }

                            }

                        }
                        else
                        {
                            File file = Db.getFileById(Convert.ToInt32(NewNode.Name));

                            if (parent.ChildNameExist(file.name, true, file.ID, file.exe))
                            {
                                DialogResult res = MessageBox.Show("Nom deja exist , rennomer", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                if (res == DialogResult.Cancel)
                                {
                                    trans = false;
                                }
                            }

                            if (trans)
                            {
                                parent.Transform(file.ID, file.name, true);
                                changeName(file.ID, file.name, true);
                                fillTreeView();
                                fillListView();
                            }

                        }
                    }
                }
                else
                {
                    if (NewNode.ImageIndex == 1 || NewNode.ImageIndex == 0)
                    {
                        Directory dir = Db.getDirById(Convert.ToInt32(NewNode.Name));

                        if (dir.ParentID == currentDir.ID || dir.ID == currentDir.ID)
                        {
                            return;
                        }

                        if (isParent(dir.ID, currentDir.ID))
                        {
                            MessageBox.Show("EROR : the destination is a child to the directory");
                        }
                        else
                        {
                            item = new ListViewItem(NewNode.Text, NewNode.ImageIndex);
                            item.SubItems.Add("Directory");
                            item.SubItems.Add(dir.getSize().ToString() + " Ko");
                            item.SubItems.Add("perm");
                            item.SubItems.Add(dir.ID.ToString());


                            if (currentDir.ChildNameExist(dir.Name))
                            {
                                DialogResult res = MessageBox.Show("Name already exist ,Rename it", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                if (res == DialogResult.Cancel)
                                {
                                    trans = false;
                                }
                            }

                            if (trans)
                            {
                                currentDir.Transform(dir.ID, dir.Name, false);
                                item.SubItems[0].Text = changeName(dir.ID, dir.Name);
                                //listViewFile.Items.Add(item);
                                fillTreeView();
                                fillListView();
                            }

                        }

                    }
                    else
                    {
                        File file = Db.getFileById(Convert.ToInt32(NewNode.Name));
                        if (file.parentID == currentDir.ID)
                        {
                            return;
                        }
                        item = new ListViewItem(NewNode.Text + "." + file.exe, NewNode.ImageIndex);
                        item.SubItems.Add(file.type);
                        item.SubItems.Add(file.size.ToString() + " Ko");
                        item.SubItems.Add("perm");
                        item.SubItems.Add(file.ID.ToString());
                        if (currentDir.ChildNameExist(file.name, true, file.ID, file.exe))
                        {
                            DialogResult res = MessageBox.Show("Name already exist ,Rename it", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (res == DialogResult.Cancel)
                            {
                                trans = false;
                            }
                        }

                        if (trans)
                        {
                            currentDir.Transform(file.ID, file.name, true);
                            item.SubItems[0].Text = changeName(file.ID, file.name, true)+"."+file.exe;
                            //listViewFile.Items.Add(item);
                            fillTreeView();
                            fillListView();
                        }

                    }
                }




            }
            else
            {
                bool trans = true;
                var newItem = (ListViewItem)e.Data.GetData("System.Windows.Forms.ListViewItem");
                var destHit = listViewFile.HitTest(e.X, e.Y);
                var loc = new Point(e.X, e.Y);
                loc.Offset(listViewFile.Location);


                if ((newItem.ImageIndex == 1 || newItem.ImageIndex == 0) && destHit.Item != null)
                {
                    if (destHit.Item.SubItems[1].Text == "Directory")
                    {
                        var parent = Db.getDirById(Convert.ToInt32(destHit.Item.SubItems[4].Text));
                        Directory dir = Db.getDirById(Convert.ToInt32(newItem.SubItems[4].Text));
                        if (dir.ParentID == parent.ID || dir.ID == parent.ID)
                        {
                            return;
                        }

                        if (isParent(dir.ID, parent.ID))
                        {
                            MessageBox.Show("EROR : the destination is a child to the directory");
                        }
                        else
                        {
                            if (parent.ChildNameExist(dir.Name))
                            {
                                DialogResult res = MessageBox.Show("Name already exist ,Rename it", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                if (res == DialogResult.Cancel)
                                {
                                    trans = false;
                                }
                            }

                            if (trans)
                            {
                                parent.Transform(dir.ID, dir.Name, false);
                                changeName(dir.ID, dir.Name);
                            }
                        }



                    }
                }
                else if(destHit.Item != null)
                {
                    if(destHit.Item.SubItems[1].Text == "Directory")
                    {
                        var parent = Db.getDirById(Convert.ToInt32(destHit.Item.SubItems[4].Text));
                        File file = Db.getFileById(Convert.ToInt32(newItem.SubItems[4].Text));
                        if (file.parentID == parent.ID || file.ID == parent.ID)
                        {
                            return;
                        }

                            if (parent.ChildNameExist(file.name,true,0,file.exe))
                            {
                                DialogResult res = MessageBox.Show("Name already exist ,Rename it", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                                if (res == DialogResult.Cancel)
                                {
                                    trans = false;
                                }
                            }

                            if (trans)
                            {
                                parent.Transform(file.ID, file.name, true);
                                changeName(file.ID, file.name,true);
                            }
                        }
                }

                fillTreeView();
                fillListView();

            }
        }
        private void listViewFile_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

        }
        //Drag from listView to ListViewItem
        private void listView_itemDrag(object sender, ItemDragEventArgs e)
        {
            dragTreeView = false;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        #endregion

        // Algorithmique methode
        #region Algo
        private string getPathNode(TreeNode tree)
        {
            if (Convert.ToInt32(tree.Name) != 1)
                return getPathNode(tree.Parent) + "/" + tree.Text;
            else
                return "";
        }
        private string changeName(int ID, string newName, bool isFile = false, int i = 0)
        {
            string path;
            string finalName = newName;
            if (!isFile)
            {
                Directory dir = Db.getDirById(ID);

                if (!Db.getDirById(dir.ParentID).ChildNameExist(newName, isFile, dir.ID))
                {
                    var dirParent = Db.getDirById(dir.ParentID);
                    path = dirParent.Path;
                    Db.Update(Db._DirTable, dir.ParentID, path, newName, ID);
                }
                else
                {
                    finalName = changeName(ID, newName + "(" + (++i) + ")", false, i);
                }
            }
            else
            {
                File file = new File(ID);
                if (!Db.getDirById(file.parentID).ChildNameExist(newName, true, file.ID, file.exe))
                {
                    var dirParent = Db.getDirById(file.parentID);
                    path = dirParent.Path;
                    Db.Update(Db._FileTable, file.parentID, path, newName, ID);
                }
                else
                {
                    newName = newName.Split('(')[0];
                    finalName = changeName(ID, newName + "(" + (++i) + ")", true, i);
                }
            }
            return finalName;

        }
        private void fillDirectory(Directory dir, TreeNode treeNode)
        {
            var listD = dir.ListDirectory();
            var listF = dir.ListFile();
            if (listD != null)
            {
                foreach (var dire in listD)
                {
                    int empty = (dire.Empty()) ? 0 : 1;
                    treeNode.Nodes.Add(dire.ID.ToString(), dire.Name, empty, empty);
                    fillDirectory(dire, treeNode.Nodes[treeNode.Nodes.Count - 1]);
                }
            }
            if (listF != null)
            {
                foreach (var file in listF)
                {
                    int typeIcon;
                    switch (file.exe)
                    {
                        case "png":
                            typeIcon = 2;
                            break;
                        case "txt":
                            typeIcon = 4;
                            break;
                        case "exe":
                            typeIcon = 3;
                            break;
                        default:
                            typeIcon = 5;
                            break;
                    }
                    treeNode.Nodes.Add(file.ID.ToString(), file.name, typeIcon, typeIcon);
                }
            }
        }
        private bool isParent(int toTransformId, int destID)
        {
            foreach (var id in Db.getDirById(toTransformId).allDirDecendent())
            {
                if (id == destID)
                    return true;
            }
            return false;
        }
        #endregion

        //Context Menu Strip Methodes
        #region ContextStripMethodes
        private void LargeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewFile.View = View.LargeIcon;
        }
        private void SmallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewFile.View = View.SmallIcon;
        }
        private void DetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewFile.View = View.Details;
        }
        private void ListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewFile.View = View.List;
        }
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool rmv;
            if(selectedItem.ImageIndex != 0 && selectedItem.ImageIndex != 1)
            {
                var filetoremove = Db.getFileById(Convert.ToInt32(selectedItem.SubItems[4].Text));
                rmv = filetoremove.remove();
                if (rmv == true)
                {
                    fillTreeView();
                    fillListView();
                }
                else
                {
                    MessageBox.Show("EROR");
                }
            }
            else
            {
                var toremove = Db.getDirById(Convert.ToInt32(selectedItem.SubItems[4].Text));
                rmv = toremove.remove();
                if (rmv == true)
                {
                    fillTreeView();
                    fillListView();
                }
                else
                {
                    MessageBox.Show("EROR");
                }
            }

        }
        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rectangle rowBounds = selectedItem.SubItems[0].Bounds;
            Rectangle labelBounds = selectedItem.GetBounds(ItemBoundsPortion.Label);
            int leftMargin = labelBounds.Left - 1;
            txt.Bounds = new Rectangle(rowBounds.Left + leftMargin, rowBounds.Top, listViewFile.Columns[0].Width - 17, rowBounds.Height);
            txt.Text = listViewFile.Items[listViewFile.SelectedItems[0].Index].SubItems[0].Text;
            txt.SelectAll();
            txt.Visible = true;
            txt.Focus();
        }
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cut = true;
            pastToolStripMenuItem.Enabled = true;
            pastToolStripMenuItem1.Enabled = true;
            collerRibbonButton2.Enabled = true;
            toCopyItem = selectedItem;
        }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cut = false;
            pastToolStripMenuItem.Enabled = true;
            pastToolStripMenuItem1.Enabled = true;
            collerRibbonButton2.Enabled = true;
            toCopyItem = selectedItem;

        }

        private void copierVerButtonRoot(object sender,EventArgs e)
        {
            var temp = toCopyItem;
            toCopyItem = selectedItem;
            if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) //is directory
            {
                copyDir(Directory.ROOT);
            }
            else
            {
                copyFile(Directory.ROOT);
            }
            fillListView();
            fillTreeView();
            toCopyItem = temp;
        }
        private void copierVerButtonDocument(object sender,EventArgs e)
        {
            var temp = toCopyItem;
            toCopyItem = selectedItem;
            if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) //is directory
            {
                copyDir(Db.getDirById(5));
            }
            else
            {
                copyFile(Db.getDirById(5));
            }
            fillListView();
            fillTreeView();
            toCopyItem = temp;
        }
        private void deplacerRoot(object sender, EventArgs e)
        {
            var temp = toCopyItem;
            toCopyItem = selectedItem;
            if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) //is directory
            {
                cutDir(Directory.ROOT);
            }
            else
            {
                cutFile(Directory.ROOT);
            }
            fillListView();
            fillTreeView();
            toCopyItem = temp;

        }
        private void deplacerDocument(object sender,EventArgs e)
        {
            var temp = toCopyItem;
            toCopyItem = selectedItem;
            if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) //is directory
            {
                cutDir(Db.getDirById(5));
            }
            else
            {
                cutFile(Db.getDirById(5));
            }
            fillListView();
            fillTreeView();
            toCopyItem = temp;
        }
        //in item "PAST"
        private void PastToolStripMenuItem_Click(object sender , EventArgs e)
        {
            Directory parent = Db.getDirById(Convert.ToInt32(selectedItem.SubItems[4].Text));
            if (cut) // is cut
            {
                if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) //is directory
                {
                    cutDir(parent);
                }
                else
                {
                    cutFile(parent);
                }
            }
            else
            {
                if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) // is directory
                {
                    copyDir(parent);
                }
                else
                {
                    copyFile(parent) ;
                }
            }
            fillListView();
            fillTreeView();
        }
        //in list view "PAST"
        private void PastToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (cut) // is cut
            {
                if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) //is directory
                {
                    cutDir();
                }
                else
                {
                    cutFile();
                }
            }
            else
            {
                if (toCopyItem.ImageIndex == 0 || toCopyItem.ImageIndex == 1) // is directory
                {
                    copyDir();
                }
                else
                {
                    copyFile();
                }
            }
            fillListView();
            fillTreeView();
        }
        private void cutDir(Directory dest = null)
        {
            var curentD = (dest == null) ? currentDir : dest;
            Directory dir = Db.getDirById(Convert.ToInt32(toCopyItem.SubItems[4].Text));
            bool trans = true;
            if (dir.ParentID == curentD.ID || dir.ID == curentD.ID)
            {
                return;
            }

            if (isParent(dir.ID, curentD.ID))
            {
                MessageBox.Show("EROR : the destination is a child to the directory");
            }
            else
            {

                if (curentD.ChildNameExist(dir.Name))
                {
                    DialogResult res = MessageBox.Show("Name already exist ,Rename it", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (res == DialogResult.Cancel)
                    {
                        trans = false;
                    }
                }
                if (trans)
                {
                    curentD.Transform(dir.ID, dir.Name, false);
                    toCopyItem.SubItems[0].Text = changeName(dir.ID, dir.Name);
                    pastToolStripMenuItem.Enabled = false;
                    pastToolStripMenuItem1.Enabled = false;
                    collerRibbonButton2.Enabled = false;

                }
            }
        }
        private void cutFile(Directory dest = null)
        {
            dest = (dest == null) ? currentDir : dest;
            File file = Db.getFileById(Convert.ToInt32(toCopyItem.SubItems[4].Text));
            bool trans = true;

            if (dest.ChildNameExist(file.name))
            {
                DialogResult res = MessageBox.Show("Name already exist ,Rename it", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel)
                {
                    trans = false;
                }
            }
            if (trans)
            {
                dest.Transform(file.ID, file.name, true);
                toCopyItem.SubItems[0].Text = changeName(file.ID, file.name, true);
                pastToolStripMenuItem.Enabled = false;
                pastToolStripMenuItem1.Enabled = false;
                collerRibbonButton2.Enabled = false;
            }
        }
        private void copyDir(Directory dest = null)
        {
            dest = (dest == null) ? currentDir : dest;
            Directory dir = Db.getDirById(Convert.ToInt32(toCopyItem.SubItems[4].Text));
            bool trans = true;

            if (isParent(dir.ID, dest.ID))
            {
                MessageBox.Show("EROR : the destination is a child to the directory");
                return;
            }

            if (dest.ChildNameExist(dir.Name))
            {
                DialogResult res = MessageBox.Show("Name already exist ,Rename it", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (res == DialogResult.Cancel)
                {
                    trans = false;
                }
            }
            if (trans)
            {
                copyDirRec(dir, dest);
            }

        }
        private void copyFile(Directory destination = null ,File fileOrig = null)
        {
            var file = (fileOrig == null) ? Db.getFileById(Convert.ToInt32(toCopyItem.SubItems[4].Text)) : fileOrig;
            var dest = (destination == null) ? currentDir : destination;
            var newFile = new File(0, file.name, dest.Path, dest.ID, file.type, file.exe, file.size);
            toCopyItem.SubItems[0].Text = changeName(newFile.ID, newFile.name, true);

        }
        private void copyDirRec(Directory DirOrig,Directory destination)
        {
            string path = (destination.Path == "/") ? destination.Path : destination.Path + "/" + DirOrig.Name;
            //var dirCopy = destination.creatChildDirectory(DirOrig.Name, path);
            var dirCopy = new Directory(0, DirOrig.Name, path, 2111, true);
            var listD = DirOrig.ListDirectory();
            var listF = DirOrig.ListFile();
            if(listD != null)
            {
                foreach (var dir in listD)
                {
                    copyDirRec(dir, dirCopy);
                }
            }
            if(listF != null)
            {
                foreach(var file in listF)
                {
                    copyFile(dirCopy, file);
                }
            }
            destination.Transform(dirCopy.ID, dirCopy.Name, false);
            changeName(dirCopy.ID, dirCopy.Name);
        }



        #endregion

    }

}
