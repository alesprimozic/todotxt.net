using System;
using System.IO;

namespace Client
{
    class ObserverChangeFile: IDisposable
    {
        private string _filename = "";
        private FileSystemWatcher _watcher;
        private bool _disposed;

        public ObserverChangeFile()
        {
        }

        public void ViewOnFile(string filename)
        {
            if (User.Default.AutoRefresh == false)
            {
                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = false;
                    _watcher.Dispose();
                    _watcher = null;
                    _filename = "";
                }
                return;
            }

            if (_filename != filename)
            {
                _filename = filename;
                if (_watcher != null)
                {
                    _watcher.Dispose();
                }
                _watcher = new FileSystemWatcher();
                _watcher.Path = System.IO.Path.GetDirectoryName(filename);
                _watcher.Filter = System.IO.Path.GetFileName(filename);
                _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;

                // Add event handlers.
                _watcher.Changed += FileChange;

                // Begin watching.
                _watcher.EnableRaisingEvents = true;
            }
            else if (User.Default.AutoRefresh == false && _watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
            }
        }

        public delegate void FileTaskListChange();

        public event FileTaskListChange OnFileTaskListChange;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (this._watcher != null)
                    {
                        this._watcher.Dispose();
                        this._watcher = null;
                    }
                }

                _disposed = true;
            }
        }

        private void FileChange(object source, FileSystemEventArgs e)
        {
            OnFileTaskListChange();
        }        
    }
}
