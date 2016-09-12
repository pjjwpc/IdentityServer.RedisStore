using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RigoFunc.IdentityServer.UnitTest.Common {
    public static class TestLogger {
        public static ILogger<T> Create<T>() {
            return new LoggerFactory().CreateLogger<T>();
        }
    }
}
