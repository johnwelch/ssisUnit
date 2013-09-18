using System;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace SsisUnit.Design
{
    public partial class ConnectionStringBuilder : Form
    {
        private readonly DataTable _providerList = DbProviderFactories.GetFactoryClasses();

        private DbConnectionStringBuilder _connBuilder = new DbConnectionStringBuilder();
        
        public string ConnectionString
        {
            get { return _connBuilder.ConnectionString; }
            set
            {
                _connBuilder.ConnectionString = value;
                if (value != null && value.ToUpper().Contains("PROVIDER"))
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

        private void ConnectionStringBuilderLoad(object sender, EventArgs e)
        {
        }

        private void CboProviderSelectedIndexChanged(object sender, EventArgs e)
        {
            string connectionString = _connBuilder.ConnectionString;
            DbProviderFactory factory = DbProviderFactories.GetFactory(_providerList.Rows[cboProvider.SelectedIndex]);
            _connBuilder = factory.CreateConnectionStringBuilder();

            if (_connBuilder == null)
                return;

            _connBuilder.ConnectionString = connectionString;
            
            propConnection.SelectedObject = _connBuilder;
        }
    }
}