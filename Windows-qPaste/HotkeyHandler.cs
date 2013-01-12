using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_qPaste
{
    /// <summary>
    /// Handles all kind of uploading that is initiated through the hotkey.
    /// </summary>
    class HotkeyHandler
    {
        bool isUploading = false;
        public HotkeyHandler()
        {
            //Hook to global keyboard for qPaste Hotkey
            KeyboardHook hook = new KeyboardHook();
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(Windows_qPaste.ModifierKeys.Control | Windows_qPaste.ModifierKeys.Shift, Keys.V);
        }
        
        /// <summary>
        /// Handle upload of data. Should be run in different thread to avoid UI-blocking.
        /// </summary>
        private void HandleUpload()
        {
            if (isUploading)
                return;
            Debug.WriteLine("Handling upload");
            isUploading = true;
            if (Clipboard.ContainsFileDropList())
            {
                StringCollection files = Clipboard.GetFileDropList();
                foreach (string file in files)
                {
                    Token token = UploadHelper.getToken();
                    ClipboardHelper.Paste(token.link);
                    UploadHelper.Upload(file, token);
                }
            }
            else if (Clipboard.ContainsImage())
            {

            }
            else if (Clipboard.ContainsText())
            {

            }
            isUploading = false;
        }

        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(HandleUpload));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }
}
