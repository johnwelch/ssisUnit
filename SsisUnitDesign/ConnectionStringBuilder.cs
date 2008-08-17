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
            set
            {
                _connBuilder.ConnectionString = value;
                if (value.ToUpper().Contains("PROVIDER"))
                {
                    cboProvider.SelectedIndex = _providerList.Rows.IndexOf(_providerList.Rows.Find("System.Data.OleDb"));
                }
            }
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
            string connectionString = _connBuilder.ConnectionString;
            DbProviderFactory factory = DbProviderFactories.GetFactory(_providerList.Rows[cboProvider.SelectedIndex]);
            _connBuilder = factory.CreateConnectionStringBuilder();
            _connBuilder.ConnectionString = connectionString;
            propConnection.SelectedObject = _connBuilder;
        }
    }
}