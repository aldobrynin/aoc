namespace AoC2022.Day24;

public partial class Day24
{
    private record State(int Move, V Position);

    private record Blizzard(V Pos, V Dir);

    public static void Solve(IEnumerable<string> input) {
        var map = Map.From(input);
        var start = FindNonWallCoord(map, 0);
        var end = FindNonWallCoord(map, map.LastRowIndex);

        var blizzards = LoadBlizzards(map);
        var blizzardsPerMove = new Dictionary<int, HashSet<V>>(1_000);

        var fromStartToEnd = Search(map, start, end, 0, BlizzardsForecast).Move.Part1();
        var fromEndToStart = Search(map, end, start, fromStartToEnd, BlizzardsForecast);
        Search(map, start, end, fromEndToStart.Move, BlizzardsForecast).Move.Part2();

        IReadOnlySet<V> BlizzardsForecast(int move)
        {
            if (blizzardsPerMove.TryGetValue(move, out var nextMoveBlizzards))
                return nextMoveBlizzards;
            return blizzardsPerMove[move] = blizzards
                .Select(b => GetBlizzardPosition(b, map.SizeX, map.SizeY, move))
                .ToHashSet();
        }
    }

    private static State Search(Map<char> map,
        V from,
        V to,
        int initialMove,
        Func<int, IReadOnlySet<V>> getBlizzardsForecast)
    {
        var initialState = new State(initialMove, from);
        return SearchHelpers.Bfs(getNextState: prevState => {
                var nextMove = prevState.Move + 1;
                var nextMoveBlizzards = getBlizzardsForecast(nextMove);
                return prevState.Position.Area5()
                    .Where(v => !nextMoveBlizzards.Contains(v))
                    .Where(v => v.IsInRange(map) && map[v] != '#')
                    .Select(nextPos => new State(nextMove, nextPos));
            }, maxDistance: null, initialState)
            .Select(x => x.State)
            .First(s => s.Position == to);
    }

    private static Blizzard[] LoadBlizzards(Map<char> map)
    {
        return map.Coordinates()
            .Select(v => {
                var result = map[v] switch {
                    '^' => V.Up,
                    'v' => V.Down,
                    '>' => V.Right,
                    '<' => V.Left,
                    _ => null,
                };
                return result == null ? null : new Blizzard(v, result);
            })
            .OfType<Blizzard>()
            .ToArray();
    }

    private static V GetBlizzardPosition(Blizzard blizzard, int xLength, int yLength, int move) {
        var (pos, dir) = blizzard;
        var nextPos = pos + dir * move - new V(1, 1);
        return nextPos.Mod(yLength - 2, xLength - 2) + new V(1, 1);
    }

    private static V FindNonWallCoord(Map<char> map, int y) {
        return map.Coordinates().Where(v => v.Y == y).First(v => map[v] == '.');
    }
}