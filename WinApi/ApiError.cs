using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkAdapterRouteControl.WinApi
{
    public static class ApiError
    {
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_BUFFER_OVERFLOW = 111;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int ERROR_NOT_FOUND = 1168;
        public const int ERROR_CANCELLED = 1223;
        public const int ERROR_OBJECT_ALREADY_EXISTS = 5010;
    }
}