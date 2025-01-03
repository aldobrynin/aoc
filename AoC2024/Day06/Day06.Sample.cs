namespace AoC2024.Day06;

public static partial class Day06 {
    public static IEnumerable<SampleInput> GetSamples() {
        yield return SampleInput.ForInput("""
                                          ....#.....
                                          .........#
                                          ..........
                                          ..#.......
                                          .......#..
                                          ..........
                                          .#..^.....
                                          ........#.
                                          #.........
                                          ......#...
                                          """)
            .WithPartOneAnswer(41)
            .WithPartTwoAnswer(6);
    }
}
