using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

namespace NetworkAdapterRouteControl
{

    public class SingleProgramInstance : IDisposable
    {

        private Mutex _processSync;
        private bool _owned = false;


        public SingleProgramInstance()
        {

            _processSync = new Mutex(
                true,
                Assembly.GetExecutingAssembly().GetName().Name,
                out _owned);
        }

        public SingleProgramInstance(string identifier)
        {
            _processSync = new Mutex(
                true,
                Assembly.GetExecutingAssembly().GetName().Name + " " + identifier,
                out _owned);
        }

        ~SingleProgramInstance()
        {
            Release();
        }

        public bool IsSingleInstance
        {
            get { return _owned; }
        }

        private void Release()
        {
            if (_owned)
            {
                _processSync.ReleaseMutex();
                _owned = false;
            }
        }

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }
    }
}
