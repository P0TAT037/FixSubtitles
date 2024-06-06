using System.Text;

Console.WriteLine("Path?");

var path = Console.ReadLine();
if (!Path.Exists(path) || !Path.GetFileName(path).EndsWith(".srt", StringComparison.CurrentCultureIgnoreCase))
{
    Console.WriteLine("invalid srt file path");
    Console.ReadKey();
    return 1;
}

Console.Write("Delay in seconds (negative values are valid) : ");
if (!int.TryParse(Console.ReadLine(), out var delay))
{
    Console.WriteLine("only numeric values are valid");
    Console.ReadKey();
    return 1;
};

var str = File.ReadAllText(path);
var strings = str.Split("\r\n\r\n");
var newSrtBuilder = new StringBuilder();

foreach (var s in strings)
{
    if(string.IsNullOrWhiteSpace(s)) continue;
    var lines = s.Split("\r\n");
    var time = lines[1].Split(" --> ");
    var minMsStart = time[0].Replace(",", ".");
    var minMsEnd = time[1].Replace(",", ".");
    TimeOnly start;
    TimeOnly end;
    TimeOnly.TryParse(minMsStart, out start);
    TimeOnly.TryParse(minMsEnd, out end);
    var span = end - start;
    start = start.Add(TimeSpan.FromSeconds(delay));
    var timeStartStr = $"{start.Hour}:{start.Minute}:{start.Second},{start.Millisecond}";
    end = start.Add(span);
    var timeEndstr = $"{end.Hour}:{end.Minute}:{end.Second},{end.Millisecond}";
    lines[1] = $"{timeStartStr} --> {timeEndstr}";
    newSrtBuilder.Append(string.Join("\r\n", lines));
    newSrtBuilder.Append("\r\n\r\n");
}

var result = newSrtBuilder.ToString();
File.WriteAllText(@"./FixedSubs.srt", result, Encoding.UTF8);
Console.WriteLine("Done");
Console.ReadKey();
return 0;
