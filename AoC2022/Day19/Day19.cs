using System.Text;
using System.Text.RegularExpressions;

namespace AoC2022.Day19;

public partial class Day19 {
    private record Blueprint(
        int Id,
        Resources OreRobotPrice,
        Resources ClayRobotPrice,
        Resources ObsidianRobotPrice,
        Resources GeodeRobotPrice) {
        private static readonly Regex Regex = new(@"(?<Price>\d+) (?<Type>ore|clay|obsidian)", RegexOptions.Compiled);

        public int MaxOre { get; private init; }
        public int MaxClay { get; private init; }
        public int MaxObsidian { get; private init; }

        public static Blueprint Parse(string[] lines) {
            Resources ParseCost(string line) {
                var raw = line[(line.IndexOf("costs", StringComparison.OrdinalIgnoreCase) + 5)..];
                var matches = Regex.Matches(raw);
                return new Resources(OrePrice(matches, "ore"), OrePrice(matches, "clay"),
                    OrePrice(matches, "obsidian"));
            }

            int OrePrice(MatchCollection matchCollection, string type) {
                return int.Parse(matchCollection.SingleOrDefault(x => x.Groups["Type"].Value == type)?.Groups["Price"]
                    .Value ?? "0");
            }

            var id = int.Parse(lines[0].Replace("Blueprint ", string.Empty).TrimEnd(':'));
            var oreRobotCost = ParseCost(lines[1]);
            var clayRobotCost = ParseCost(lines[2]);
            var obsidianRobotCost = ParseCost(lines[3]);
            var geodeRobotCost = ParseCost(lines[4]);
            var robotsCosts = new[] { oreRobotCost, clayRobotCost, obsidianRobotCost, geodeRobotCost };
            return new Blueprint(id,
                oreRobotCost,
                clayRobotCost,
                obsidianRobotCost,
                geodeRobotCost) {
                MaxOre = robotsCosts.Max(x => x.Ore),
                MaxClay = robotsCosts.Max(x => x.Clay),
                MaxObsidian = robotsCosts.Max(x => x.Obsidian),
            };
        }
    }

    record Resources(int Ore = 0, int Clay = 0, int Obsidian = 0, int Geode = 0) {
        public static readonly Resources OneOre = new(Ore: 1);
        public static readonly Resources OneClay = new(Clay: 1);
        public static readonly Resources OneObsidian = new(Obsidian: 1);
        public static readonly Resources OneGeode = new(Geode: 1);
        public static Resources Zero => new();

        public override string ToString() {
            var sb = new StringBuilder();
            if (Ore > 0)
                sb.AppendFormat("{0} ore", Ore);
            if (Clay > 0)
                sb.AppendFormat("{1}{0} clay", Clay, sb.Length > 0 ? "+" : string.Empty);
            if (Obsidian > 0)
                sb.AppendFormat("{1}{0} obsidian", Obsidian, sb.Length > 0 ? "+" : string.Empty);
            if (Geode > 0)
                sb.AppendFormat("{1}{0} geode", Geode, sb.Length > 0 ? "+" : string.Empty);
            return sb.ToString();
        }

        public static Resources operator +(Resources first, Resources second) => new(
            first.Ore + second.Ore,
            first.Clay + second.Clay,
            first.Obsidian + second.Obsidian,
            first.Geode + second.Geode);

        public static Resources operator -(Resources first, Resources second) => new(
            first.Ore - second.Ore,
            first.Clay - second.Clay,
            first.Obsidian - second.Obsidian,
            first.Geode - second.Geode);

        public static bool operator >=(Resources first, Resources second) {
            return first.Ore >= second.Ore
                   && first.Clay >= second.Clay
                   && first.Obsidian >= second.Obsidian
                   && first.Geode >= second.Geode;
        }

        public static bool operator <=(Resources first, Resources second) {
            return first.Ore <= second.Ore
                   && first.Clay <= second.Clay
                   && first.Obsidian <= second.Obsidian
                   && first.Geode <= second.Geode;
        }
    }

    private record State(Resources Resources, Resources Robots);

    public static void Solve(IEnumerable<string> input) {
        var blueprints = input.SelectMany(s => s.Split(new[] { '.', ':' }))
            .Where(x => !string.IsNullOrEmpty(x))
            .Chunk(5)
            .Select(Blueprint.Parse)
            .ToArray();

        blueprints
            .AsParallel()
            .Sum(b => FindMaxGeode(b, maxTime: 24) * b.Id)
            .Part1();

        blueprints
            .Take(3)
            .AsParallel()
            .Select(b => FindMaxGeode(b, maxTime: 32))
            .Product()
            .Part2();
    }

    private static int FindMaxGeode(Blueprint blueprint, int maxTime) {
        return SearchHelpers.Bfs(item => GetNextStates(item, blueprint),
                maxDistance: maxTime,
                new State(Resources.Zero, Resources.OneOre)
            )
            .Max(x => x.State.Resources.Geode);
    }

    private static IEnumerable<State> GetNextStates(State current, Blueprint blueprint) {
        return NextRobots(current, blueprint)
            .Select(state =>
                new State(
                    Resources: current.Resources - state.SpentResources + current.Robots,
                    Robots: current.Robots + state.Robots)
            );
    }

    private static IEnumerable<(Resources Robots, Resources SpentResources)> NextRobots(State current,
        Blueprint blueprint) {
        var maxOre = blueprint.MaxOre + 1;
        var maxClay = blueprint.MaxClay + 1;
        var maxObsidian = blueprint.MaxObsidian + 1;

        if (current.Resources >= blueprint.GeodeRobotPrice) {
            yield return (Resources.OneGeode, blueprint.GeodeRobotPrice);
            yield break;
        }

        var robotsCount = 0;
        if (current.Resources.Obsidian <= maxObsidian && current.Resources >= blueprint.ObsidianRobotPrice) {
            yield return (Resources.OneObsidian, blueprint.ObsidianRobotPrice);
            robotsCount++;
        }

        if (current.Resources.Clay <= maxClay && current.Resources >= blueprint.ClayRobotPrice) {
            yield return (Resources.OneClay, blueprint.ClayRobotPrice);
            robotsCount++;
        }

        if (current.Resources.Ore <= maxOre && current.Resources >= blueprint.OreRobotPrice) {
            yield return (Resources.OneOre, blueprint.OreRobotPrice);
            robotsCount++;
        }

        if (robotsCount <= 1)
            yield return (Resources.Zero, Resources.Zero);
    }
}