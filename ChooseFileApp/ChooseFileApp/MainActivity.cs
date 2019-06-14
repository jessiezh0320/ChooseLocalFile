using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;

namespace ChooseFileApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {  //https://github.com/syncfusion/Xamarin-FileFormat-Demos/blob/master/SampleBrowser/SampleBrowser.Droid/FilePickerActivity.cs
        public  string TAG
        {
            get
            {
                return "123456";
            }
        }
        static readonly int REQUEST_CHOOSER = 0x001;
        static readonly int REQUEST_File = 0x002;
        Button selectBtn;

        /**
     	* Root of the layout of this Activity.
     	*/
        View layout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            selectBtn = FindViewById<Button>(Resource.Id.selectBtn);

            layout = FindViewById(Resource.Id.sample_main_layout);

            selectBtn.Click += SelectBtn_Click;
        }

        private void SelectBtn_Click(object sender, System.EventArgs e)
        {
            getStoreFile();
        }

        void pickFile() {
            Intent intent = null;
            intent = new Intent(Intent.ActionGetContent);
            //if (Build.VERSION.SdkInt < Build.VERSION_CODES.Kitkat)
            //{
            //    intent = new Intent(Intent.ActionGetContent);
            //}
            //else
            //{
            //    intent = new Intent(Intent.ActionOpenDocument);
            //}

            // The MIME data type filter
            //intent.SetType("audio/*");
            intent.SetType("*/*");
            intent.AddCategory(Intent.CategoryOpenable);


            //intent.SetAction(Intent.ActionGetContent);
            //intent.SetType("audio/*");
            //intent.PutExtra(Intent.ExtraLocalOnly, true);
            StartActivityForResult(Intent.CreateChooser(intent, "Select ,Music"), REQUEST_CHOOSER);

        }


        public void getStoreFile()
        {
            Log.Info(TAG, " ReadExternalStorage button pressed. Checking permission.");

            // Check if the Camera permission is already available.
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {

                // Camera permission has not been granted
                RequestReadStoragePermission();
            }
            else
            {
                // Camera permissions is already available, show the camera preview.
                Log.Info(TAG, "ReadExternalStorage permission has already been granted. Displaying camera preview.");
                //ShowCameraPreview();

                pickFile();
            }
        }

        private void RequestReadStoragePermission()
        {
            Log.Info(TAG, "ReadExternalStorage  permission has NOT been granted. Requesting permission.");

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage))
            {
                // Provide an additional rationale to the user if the permission was not granted
                // and the user would benefit from additional context for the use of the permission.
                // For example if the user has previously denied the permission.
                Log.Info(TAG, " ReadExternalStorage permission rationale to provide additional context.");

                Snackbar.Make(layout, Resource.String.permission_access_storage_rationale,
                    Snackbar.LengthIndefinite).SetAction(Resource.String.ok, new Action<View>(delegate (View obj) {
                        ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, REQUEST_File);
                    })).Show();
            }
            else
            {
                // Camera permission has not been granted yet. Request it directly.
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, REQUEST_File);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_File)
            {
                // Received permission result for camera permission.
                Log.Info(TAG, "Received response for Camera permission request.");

                // Check if the only required permission has been granted
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    // Camera permission has been granted, preview can be displayed
                    Log.Info(TAG, "ReadExternalStorage permission has now been granted. Showing preview.");
                    Snackbar.Make(layout, Resource.String.permission_available_access_storage, Snackbar.LengthShort).Show();

                    pickFile();
                }
                else
                {
                    Log.Info(TAG, "ReadExternalStorage permission was NOT granted.");
                    Snackbar.Make(layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
                }
            }

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
                if (resultCode == Result.Canceled)
                {
                    Finish();
                }
                else
                {
                    try
                    {
                    var _uri = data.Data;
                    var filePath = IOUtil.getPath(this, _uri);
                    System.Diagnostics.Debug.Write("123456  ******* filePath =  " + filePath);

                    if (string.IsNullOrEmpty(filePath))
                        filePath = _uri.Path;

                    var file = IOUtil.readFile(filePath);
                    System.Diagnostics.Debug.Write("123456  ******* filePath =  " + filePath);

                    //var fileName = GetFileName(this, _uri);

                    //OnFilePicked(new FilePickerEventArgs(file, fileName, filePath));
                }
                    catch (Exception readEx)
                    {
                        // Notify user file picking failed.
                        //OnFilePickCancelled();
                        System.Diagnostics.Debug.Write(readEx);
                    }
                    finally
                    {
                        Finish();
                    }
            }
        }

        string GetFileName(Context ctx, Android.Net.Uri uri)
        {

            string[] projection = { MediaStore.MediaColumns.DisplayName };

            var cr = ctx.ContentResolver;
            var name = "";
            var metaCursor = cr.Query(uri, projection, null, null, null);

            if (metaCursor != null)
            {
                try
                {
                    if (metaCursor.MoveToFirst())
                    {
                        name = metaCursor.GetString(0);
                    }
                }
                finally
                {
                    metaCursor.Close();
                }
            }
            return name;
        }

        internal static event EventHandler<FilePickerEventArgs> FilePicked;
        internal static event EventHandler<EventArgs> FilePickCancelled;

        private static void OnFilePickCancelled()
        {
            FilePickCancelled?.Invoke(null, null);
        }

        private static void OnFilePicked(FilePickerEventArgs e)
        {
            var picked = FilePicked;

            if (picked != null)
                picked(null, e);
        }


        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}
    }
}