using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CsharpScriptLikeVisionPro;

class InvokeBaseImpl : InvokeBase
{
    private Dictionary<string, object> _inputs;

    public override Task Init(Dictionary<string, object> inputs)
    {
        _inputs = inputs;
        return base.Init(inputs);
    }

    public override Task Run(CancellationToken token)
    {
        // #if DEBUG
        // if (Debugger.IsAttached) Debugger.Break();
        // #endif
        throw new NotImplementedException();
    }
}
