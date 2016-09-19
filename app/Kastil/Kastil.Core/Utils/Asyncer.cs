using System;
using System.Threading.Tasks;

namespace Kastil.Core.Utils
{
    public static class Asyncer
    {
        public static Task Async(Action action)
        {
            return Task.Factory.StartNew(action);
        }

        public static Task<T> Async<T>(Func<T> func)
        {
            return Task.Factory.StartNew(func);
        }

        public static Task DoNothing()
        {
            return Task.Factory.StartNew(() => { });
        }
    }
}