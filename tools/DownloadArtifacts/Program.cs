// Minimal standalone tool to download BC Service Tier DLLs via HTTP range requests.
// Used by the MSBuild pre-build target when DLLs aren't present locally.
// This is a standalone project so it can be built and run without the BC DLL references.

using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: DownloadArtifacts <bc-version> <output-dir>");
    return 1;
}

var version = args[0];
var outputDir = args[1];
var artifactUrl = $"https://bcartifacts-exdbf9fwegejdqak.b02.azurefd.net/onprem/{version}/platform";

Directory.CreateDirectory(outputDir);

using var http = new HttpClient();
http.Timeout = TimeSpan.FromMinutes(5);

// Step 1: HEAD request for total size
Console.Error.WriteLine($"Resolving artifact size for BC {version}...");
using var headReq = new HttpRequestMessage(HttpMethod.Head, artifactUrl);
using var headResp = http.Send(headReq);
headResp.EnsureSuccessStatusCode();
var totalSize = headResp.Content.Headers.ContentLength ?? 0;
if (totalSize == 0) { Console.Error.WriteLine("Error: unknown size"); return 1; }
Console.Error.WriteLine($"Platform artifact: {totalSize / 1048576} MB");

// Step 2: Download last 64KB for EOCD
Console.Error.WriteLine("Downloading ZIP directory...");
var tail = DownloadRange(http, artifactUrl, totalSize - 65536, totalSize - 1);

int eocdPos = -1;
for (int i = tail.Length - 22; i >= 0; i--)
    if (tail[i] == 0x50 && tail[i + 1] == 0x4b && tail[i + 2] == 0x05 && tail[i + 3] == 0x06)
    { eocdPos = i; break; }
if (eocdPos < 0) { Console.Error.WriteLine("Error: EOCD not found"); return 1; }

int entryCount = BitConverter.ToUInt16(tail, eocdPos + 10);
uint cdOffset = BitConverter.ToUInt32(tail, eocdPos + 16);

// Step 3: Central directory
byte[] cdData;
int cdStart;
long cdInTail = tail.Length - (totalSize - cdOffset);
if (cdInTail >= 0) { cdData = tail; cdStart = (int)cdInTail; }
else
{
    Console.Error.WriteLine("Downloading central directory...");
    cdData = DownloadRange(http, artifactUrl, cdOffset, totalSize - 1);
    cdStart = 0;
}

// Step 4: Parse entries and find Nav DLLs
var matching = new List<(string Name, int Method, long CompSize, long Offset)>();
int pos = cdStart;
for (int i = 0; i < entryCount && pos + 46 <= cdData.Length; i++)
{
    if (cdData[pos] != 0x50 || cdData[pos + 1] != 0x4b || cdData[pos + 2] != 0x01 || cdData[pos + 3] != 0x02)
        break;
    int cm = BitConverter.ToUInt16(cdData, pos + 10);
    uint cs = BitConverter.ToUInt32(cdData, pos + 20);
    int nl = BitConverter.ToUInt16(cdData, pos + 28);
    int el = BitConverter.ToUInt16(cdData, pos + 30);
    int cl = BitConverter.ToUInt16(cdData, pos + 32);
    uint lo = BitConverter.ToUInt32(cdData, pos + 42);
    if (pos + 46 + nl > cdData.Length) break;
    var name = Encoding.UTF8.GetString(cdData, pos + 46, nl).Replace('\\', '/');
    var lower = name.ToLowerInvariant();
    var bn = Path.GetFileName(lower);
    if (lower.Contains("servicetier/") && lower.Contains("/service/") &&
        bn.StartsWith("microsoft.dynamics.nav.") && bn.EndsWith(".dll") && cs > 0 &&
        !lower.Split("/service/").Last().Contains('/'))
    {
        matching.Add((name, cm, cs, lo));
    }
    pos += 46 + nl + el + cl;
}

if (matching.Count == 0) { Console.Error.WriteLine("Error: no Nav DLLs found"); return 1; }
Console.Error.WriteLine($"Found {matching.Count} Nav DLLs");

// Step 5: Single range download
matching.Sort((a, b) => a.Offset.CompareTo(b.Offset));
long firstOffset = matching[0].Offset;
var last = matching[^1];
long rangeEnd = Math.Min(last.Offset + 30 + last.Name.Length + 512 + last.CompSize, totalSize - 1);
Console.Error.WriteLine($"Downloading {(rangeEnd - firstOffset) / 1048576} MB ({(int)((1.0 - (double)(rangeEnd - firstOffset) / totalSize) * 100)}% savings)...");
var data = DownloadRange(http, artifactUrl, firstOffset, rangeEnd);

// Step 6: Extract
int extracted = 0;
foreach (var (name, method, compSize, offset) in matching)
{
    int p = (int)(offset - firstOffset);
    if (p < 0 || p + 30 > data.Length || data[p] != 0x50 || data[p + 1] != 0x4b || data[p + 2] != 0x03 || data[p + 3] != 0x04)
        continue;
    int nl2 = BitConverter.ToUInt16(data, p + 26);
    int el2 = BitConverter.ToUInt16(data, p + 28);
    int ds = p + 30 + nl2 + el2;
    if (ds + compSize > data.Length) continue;

    byte[] fileData;
    if (method == 0)
    {
        fileData = new byte[compSize];
        Array.Copy(data, ds, fileData, 0, (int)compSize);
    }
    else if (method == 8)
    {
        using var cs2 = new MemoryStream(data, ds, (int)compSize);
        using var df = new DeflateStream(cs2, CompressionMode.Decompress);
        using var o = new MemoryStream();
        df.CopyTo(o);
        fileData = o.ToArray();
    }
    else continue;

    File.WriteAllBytes(Path.Combine(outputDir, Path.GetFileName(name)), fileData);
    extracted++;
}

Console.Error.WriteLine($"Downloaded {extracted} DLLs to {outputDir}");
return 0;

static byte[] DownloadRange(HttpClient http, string url, long from, long to)
{
    using var req = new HttpRequestMessage(HttpMethod.Get, url);
    req.Headers.Range = new RangeHeaderValue(from, to);
    using var resp = http.Send(req);
    resp.EnsureSuccessStatusCode();
    using var ms = new MemoryStream();
    resp.Content.ReadAsStream().CopyTo(ms);
    return ms.ToArray();
}
