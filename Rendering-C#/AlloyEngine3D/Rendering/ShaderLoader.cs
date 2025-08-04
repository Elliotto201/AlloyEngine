using System.Runtime.InteropServices;

namespace AlloyRendering
{
    public static partial class ShaderLoader
    {
        [LibraryImport("AlloyRendering.dll", StringMarshalling = StringMarshalling.Utf8)]
        public static partial ShaderInfo loadModelShader(string vertexSource, string fragmentSource);
    }
}
