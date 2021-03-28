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
    public partial class Aithentication : Form
    {
        public Aithentication()
        {
            InitializeComponent();
        }

        private void Aithentication_Load(object sender, EventArgs e)
        {
            nameTextBox.Text = "Guest";
        }

        private void VerifierButton_Click(object sender, EventArgs e)
        {
            int? ID = Db.Login(nameTextBox.Text.ToLower(), passwordTextBox.Text.ToLower());
            if (ID != null)
            {
                FileGestion fileGestion = new FileGestion((int)ID,this);
                fileGestion.Show();
                Hide();
            }
            else
            {
                MessageBox.Show("Utulisateur introuvable");
            }
        }
    }
}
