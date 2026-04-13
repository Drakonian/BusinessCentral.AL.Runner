namespace AlRunner.Runtime;

using Microsoft.Dynamics.Nav.Runtime;
using Microsoft.Dynamics.Nav.Types;

/// <summary>
/// Minimal file-dialog replacement. Standalone mode has no client picker, so
/// upload requests fail closed and leave the target stream empty.
/// </summary>
public static class MockFile
{
    public static bool ALUploadIntoStream(DataError errorLevel, string filter, ByRef<MockInStream> inStream)
    {
        inStream.Value.Clear();
        return false;
    }

    public static bool ALUploadIntoStream(DataError errorLevel, string filter, ByRef<MockInStream> inStream, System.Guid uploadId)
    {
        inStream.Value.Clear();
        return false;
    }
}
