using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ssisUnitTestRunnerUI
{
    class UIHelper
    {
        private const string FILE_FILTER_SSISUNIT = "Test Files(*.ssisUnit)|*.ssisUnit|All files (*.*)|*.*";
        private const string FILE_FILTER_DTSX = "Packages(*.dtsx)|*.dtsx|All files (*.*)|*.*";
        private const string FILE_FILTER_CSV = "Comma Seperated Values(*.csv)|*.csv|All files (*.*)|*.*";
        private const string FILE_FILTER_ALL = "All files (*.*)|*.*";


        static public DialogResult ShowSaveAs(ref string fileName, FileFilter filter, bool checkFileExists)
        {
            SaveFileDialog dlgSaveFile = new SaveFileDialog();
            DialogResult returnValue = DialogResult.Cancel;

            dlgSaveFile.Filter = ConvertFileFilterEnumToString(filter);
            dlgSaveFile.CheckFileExists = checkFileExists;
            dlgSaveFile.FileName = fileName;
            returnValue = dlgSaveFile.ShowDialog();
            if (returnValue== DialogResult.OK)
            {
                fileName = dlgSaveFile.FileName;
            }
            
            return returnValue;
        }

        static public DialogResult ShowOpen(ref string fileName, FileFilter filter, bool checkFileExists)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            DialogResult returnValue = DialogResult.Cancel;

            dlgOpenFile.Filter = ConvertFileFilterEnumToString(filter);
            dlgOpenFile.CheckFileExists = checkFileExists;
            dlgOpenFile.FileName = fileName;
            returnValue = dlgOpenFile.ShowDialog();
            if (returnValue == DialogResult.OK)
            {
                fileName = dlgOpenFile.FileName;
            }
            return returnValue;
        }

        private static string ConvertFileFilterEnumToString(FileFilter fileFilter)
        {
            switch (fileFilter)
            {
                case FileFilter.SsisUnit:
                    return FILE_FILTER_SSISUNIT;
                case FileFilter.DTSX:
                    return FILE_FILTER_DTSX;
                case FileFilter.CSV:
                    return FILE_FILTER_CSV;
                default:
                    return FILE_FILTER_ALL;
            }
        }

        public enum FileFilter
        {
            SsisUnit = 0,
            DTSX = 1,
            CSV = 2
        }
    }
}
