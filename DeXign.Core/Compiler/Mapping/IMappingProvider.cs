using DeXign.Core.Logic;
using System;

namespace DeXign.Core.Compiler
{
    public interface IMappingProvider
    {
        string GetNullString();

        string GetFunctionLine(PFunction pFunc, params string[] parameters);

        string GetEventName(PTrigger trigger);

        string GetEventCallbackName(PTrigger trigger);

        string GetValueLine(object obj, bool isInline = false);

        string GetMappingCode(object obj);

        string Casting(Type castingType, string value);
    }
}
