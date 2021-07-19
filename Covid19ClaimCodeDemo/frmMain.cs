using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System.IO;
using System.Diagnostics;
using DevExpress.XtraReports.UI;

namespace Covid19ClaimCodeDemo
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        private MySqlConnection conn = new MySqlConnection("server=192.168.0.2;userid=root;password=boom123;charset=tis620;");
        private DataTable dt = new DataTable();

        public frmMain()
        {
            InitializeComponent();
        }

        private void txtHn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string HN = "";
                if (txtHn.Text.Trim().Length == 1)
                {
                    HN = "000000" + txtHn.Text.Trim();
                }
                if (txtHn.Text.Trim().Length == 2)
                {
                    HN = "00000" + txtHn.Text.Trim();
                }
                if (txtHn.Text.Trim().Length == 3)
                {
                    HN = "0000" + txtHn.Text.Trim();
                }
                if (txtHn.Text.Trim().Length == 4)
                {
                    HN = "000" + txtHn.Text.Trim();
                }
                if (txtHn.Text.Trim().Length == 5)
                {
                    HN = "00" + txtHn.Text.Trim();
                }
                if (txtHn.Text.Trim().Length == 6)
                {
                    HN = "0" + txtHn.Text.Trim();
                }
                string sql = string.Format("select pt.pt.hn, concat(pt.pt.pttitle, pt.pt.ptfname, ' ', pt.pt.ptlname) as name from pt.pt where hn = '{0}'", HN);
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    dataGridView.DataSource = dt;
                    txtCovidCode.Focus();
                }
                else
                {
                    XtraMessageBox.Show("Not Found!!!");
                    txtHn.Focus();
                    return;
                }
            }
        }

        private void txtCovidCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave_Click(sender, e);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // delete
            string sqlDelete = "delete from human_resource.covidcode";
            MySqlCommand cmd1 = new MySqlCommand(sqlDelete, conn);
            conn.Open();
            cmd1.ExecuteNonQuery();
            conn.Close();

            //insert
            string sqlInsert = string.Format("insert into human_resource.covidcode (hn, name, code) values ('{0}','{1}','{2}')", txtHn.Text.Trim(), dt.Rows[0][1].ToString(), txtCovidCode.Text.Trim());
            MySqlCommand cmd2 = new MySqlCommand(sqlInsert, conn);
            conn.Open();
            cmd2.ExecuteNonQuery();
            conn.Close();
            //XtraMessageBox.Show("Successfully Saved!!!");
            txtHn.Focus();

            // export pdf and print
            DataSet ds = new DataSet();
            string sqlReport = "select hn, name, code from human_resource.covidcode";
            MySqlDataAdapter da = new MySqlDataAdapter(sqlReport, conn);
            da.Fill(ds, ds.Tables[0].TableName);
            XtraReport report = new XtraReport();
            report.RequestParameters = false;
            report.DataSource = ds;

            // create directory
            Directory.CreateDirectory("C:\\Temp\\EXPORT");
            string path = "C:\\Temp\\EXPORT\\printCovidCode.pdf";
            report.Print();

            //report.ExportToPdf(path);
            //Process.Start(@path);
        }

        //private void PrintReport(IReportDataV2 reportData)
        //{
        //    XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
        //    ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);
        //    if (reportsModule != null && reportsModule.ReportsDataSourceHelper != null)
        //    {
        //        reportsModule.ReportsDataSourceHelper.SetupBeforePrint(report);
        //        report.PrintDialog();
        //    }
        //}
    }
}