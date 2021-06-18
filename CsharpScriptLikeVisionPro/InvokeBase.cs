using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CsharpScriptLikeVisionPro
{
    public abstract class InvokeBase
    {
        public virtual Task Init(Dictionary<string, object> inputs)
        {
            return Task.CompletedTask;
        }

        public abstract Task Run(CancellationToken token);
    }
}
