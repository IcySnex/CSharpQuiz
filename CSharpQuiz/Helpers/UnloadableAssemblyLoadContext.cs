using System.Reflection;
using System.Runtime.Loader;

namespace CSharpQuiz.Helpers;

sealed class UnloadableAssemblyLoadContext() : AssemblyLoadContext(true)
{
    protected override Assembly Load(
        AssemblyName assemblyName) =>
        default!;
}