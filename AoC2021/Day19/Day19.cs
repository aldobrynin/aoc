namespace AoC2021.Day19;

public partial class Day19
{
    private record Scanner3D(string Id, V3<int>[] Beacons)
    {
        public static Scanner3D Parse(string s)
        {
            var lines = s.Split('\n');
            var id = lines[0].Replace('-', '\0')
                .Replace(' ', '\0')
                .Replace("scanner", string.Empty);

            var beacons = lines.Skip(1).Select(V3<int>.Parse).ToArray();

            return new Scanner3D(id, beacons);
        }

        public override string ToString() => $"[Scanner {Id}, Beacons: {Beacons.StringJoin()}]";
    }

    public static void Solve(IEnumerable<string> input)
    {
        var data = input.StringJoin(Environment.NewLine)
            .Split("\n\n")
            .Select(Scanner3D.Parse)
            .ToArray();
        var scanners = new List<V3<int>>
        {
            V3<int>.Zero,
        };
        var locatedScanners = data.Take(1).Select(x => x.Id).ToHashSet();
        var knownBeacons = data.Take(1).SelectMany(x => x.Beacons).ToHashSet();

        while (locatedScanners.Count != data.Length)
        {
            foreach (var (id, beacons) in data.Where(x => !locatedScanners.Contains(x.Id)))
            {
                var transform = FindTransform(beacons, knownBeacons);
                if (transform == null) continue;

                scanners.Add(transform.Value.Offset);
                knownBeacons.UnionWith(transform.Value.Beacons);
                locatedScanners.Add(id);
            }
        }

        knownBeacons.Count.Part1();

        scanners.SelectMany(first => scanners.Select(first.DistTo))
            .Max()
            .Part2();

    }

    private static (V3<int> Offset, V3<int>[] Beacons)? FindTransform(V3<int>[] beacons, IReadOnlySet<V3<int>> knownBeacons)
    {
        for (var rotation = 0; rotation < 24; rotation++)
        {
            var rotated = beacons.Select(b => Rotate(b, rotation)).ToArray();
            var offset = rotated.SelectMany(r => knownBeacons.Select(b => b - r))
                .GroupBy(x => x)
                .Where(x => x.Count() >= 12)
                .Select(x => x.Key)
                .SingleOrDefault();
            if (offset == null) continue;
            var translated = rotated.Select(r => r + offset).ToArray();
            return (offset, translated);
        }

        return null;
    }

    private static V3<int> Rotate(V3<int> v, int direction)
    {
        return direction switch
        {
            0 => new(v.X, v.Y, v.Z),
            1 => new(v.Y, v.Z, v.X),
            2 => new(-v.Y, v.X, v.Z),
            3 => new(-v.X, -v.Y, v.Z),
            4 => new(v.Y, -v.X, v.Z),
            5 => new(v.Z, v.Y, -v.X),
            6 => new(v.Z, v.X, v.Y),
            7 => new(v.Z, -v.Y, v.X),
            8 => new(v.Z, -v.X, -v.Y),
            9 => new(-v.X, v.Y, -v.Z),
            10 => new(v.Y, v.X, -v.Z),
            11 => new(v.X, -v.Y, -v.Z),
            12 => new(-v.Y, -v.X, -v.Z),
            13 => new(-v.Z, v.Y, v.X),
            14 => new(-v.Z, v.X, -v.Y),
            15 => new(-v.Z, -v.Y, -v.X),
            16 => new(-v.Z, -v.X, v.Y),
            17 => new(v.X, -v.Z, v.Y),
            18 => new(-v.Y, -v.Z, v.X),
            19 => new(-v.X, -v.Z, -v.Y),
            20 => new(v.Y, -v.Z, -v.X),
            21 => new(v.X, v.Z, -v.Y),
            22 => new(-v.Y, v.Z, -v.X),
            23 => new(-v.X, v.Z, v.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}