using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SsisUnit.Design
{
    public partial class ConnectionStringBuilder : Form
    {
        private DataTable _providerList = DbProviderFactories.GetFactoryClasses();

        private DbConnectionStringBuilder _connBuilder = new DbConnectionStringBuilder();

        public string ConnectionString
        {
            get { return _connBuilder.ConnectionString; }
            set { _connBuilder.ConnectionString = value; }
        }

        public ConnectionStringBuilder()
        {
            InitializeComponent();
            LoadProviders();
            propConnection.SelectedObject = _connBuilder;
        }

        private void LoadProviders()
        {
            foreach (DataRow dr in _providerList.Rows)
            {
                cboProvider.Items.Add(dr["Name"].ToString());
            }
        }

        private void ConnectionStringBuilder_Load(object sender, EventArgs e)
        {
        }

        private void cboProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(_providerList.Rows[cboProvider.SelectedIndex]);
            _connBuilder = factory.CreateConnectionStringBuilder();
            propConnection.SelectedObject = _connBuilder;
        }
    }
}