using DeXign.Logic.Collections;
using System;

namespace DeXign.Logic
{
    //
    //                                       +---------+
    //                          +--->(Input) | IBinder | (Output)...
    //                          |            +---------+
    //                          |
    //   +---------+            |            +---------+
    //   | IBinder | (Outputs)==+--->(Input) | IBinder | (Output)...
    //   +---------+            |            +---------+
    //                          |
    //                          |            +---------+
    //                          +--->(Input) | IBinder | (Output)...
    //                                       +---------+
    //
    public interface IBinder : IBinderProvider
    {
        event EventHandler<BinderBindedEventArgs> Binded;
        event EventHandler<BinderReleasedEventArgs> Released;

        BinderCollection Inputs { get; } // inputs
        BinderCollection Parameters { get; } // input parameters

        BinderCollection Outputs { get; }

        void SetProvider(IBinderProvider provider);
    }
}
