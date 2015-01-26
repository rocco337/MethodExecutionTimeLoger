using PostSharp.Aspects;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MethodExecutionTimeLoger
{
    [Serializable]
    [DebuggerStepThrough]
    public class MethodExecutionTimeLoger : OnMethodBoundaryAspect
    {
        public int Threshold { get; set; }

        public MethodExecutionTimeLoger()
        {
            Threshold = 1500;
        }

        private string methodName;

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            if (method.DeclaringType != null)
            {
                methodName = method.DeclaringType.FullName + "." + method.Name;
            }
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            args.MethodExecutionTag = Stopwatch.StartNew();
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Stopwatch sw = (Stopwatch)args.MethodExecutionTag;
            sw.Stop();

#if DEBUG
            StringBuilder info = new StringBuilder();
            info.AppendLine(string.Format("{0}", methodName));
            info.AppendLine(string.Format("Ticks: {0}; Milliseconds: {1}", sw.ElapsedTicks, sw.ElapsedMilliseconds));
            Logger.Get().Warn(info);
#endif

            if (sw.ElapsedMilliseconds > Threshold)
            {
                Logger.Get().Warn(string.Format("Method [{0}] was expected to finish within [{1}] milliseconds, but took [{2}] instead!",
                    methodName,
                    Threshold,
                    sw.ElapsedMilliseconds));
            }
        }
    }
}
