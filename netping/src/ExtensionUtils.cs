using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace netping {
    public static class ExtensionUtils {

        public static T WaitResult<T>(this Task<T> task) {
            task.Wait();
            return task.Result;
        }

        public static T WaitResult<T>(this ValueTask<T> task) {
            var t = task.AsTask();
            t.Wait();
            return t.Result;
        }


    }
}
