using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SsisUnit.Design
{
    public partial class DatasetBrowser : Form
    {
        public DatasetBrowser()
        {
            InitializeComponent();
        }

        public DataTable ResultsDatatable { get; set; }
        public DataTable OriginalResultsDatatable { get; set; }
        public Dataset OpenedDataset { get; set; }
        public bool FormIsResultsStored { get; set; }

        private void btnLoadDataset_Click(object sender, EventArgs e)
        {
            dgvResults.DataSource = OriginalResultsDatatable;
            using (var stringWriter = new StringWriter())
            {
                OriginalResultsDatatable.WriteXml(stringWriter, XmlWriteMode.WriteSchema, true);
                txtSerializedDataTable.Text = stringWriter.ToString();
            }
        }

        private void DatasetBrowser_Load(object sender, EventArgs e)
        {
            this.Text = "Dataset: " + OpenedDataset.Name + " | IsResultsStored: " + FormIsResultsStored.ToString();

            dgvResults.DataSource = ResultsDatatable;
            dgvOriginalResults.DataSource = OriginalResultsDatatable;

            using (var stringWriter = new StringWriter())
            {
                ResultsDatatable.WriteXml(stringWriter, XmlWriteMode.WriteSchema, true);
                txtSerializedDataTable.Text = stringWriter.ToString();
            }
        }

        private void dgvResults_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            using (var stringWriter = new StringWriter())
            {
                ResultsDatatable.WriteXml(stringWriter, XmlWriteMode.WriteSchema, true);
                txtSerializedDataTable.Text = stringWriter.ToString();
            }
        }
    }
}
