using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ChooseFileApp
{
    public class FilePickerEventArgs
    {
        public byte[] FileByte { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public FilePickerEventArgs()
        {

        }

        public FilePickerEventArgs(byte[] fileByte)
        {
            FileByte = fileByte;
        }

        public FilePickerEventArgs(byte[] fileByte, string fileName)
            : this(fileByte)
        {
            FileName = fileName;
        }

        public FilePickerEventArgs(byte[] fileByte, string fileName, string filePath)
            : this(fileByte, fileName)
        {
            FilePath = filePath;
        }
    }
}