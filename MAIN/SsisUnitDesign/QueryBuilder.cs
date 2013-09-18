using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SsisUnit.Design
{
    public partial class QueryBuilder : Form
    {
        public QueryBuilder()
        {
            InitializeComponent();
        }

        public string Query
        {
            get { return txtQuery.Text; }
            set { txtQuery.Text = value; }
        }

        private void QueryBuilder_Load(object sender, EventArgs e)
        {

        }
    }
}
