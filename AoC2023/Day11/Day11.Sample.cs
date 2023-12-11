namespace AoC2023.Day11;

public partial class Day11 {
    public static IEnumerable<SampleInput> GetSamples() {
        yield return SampleInput.ForInput("""
                                          ...#......
                                          .......#..
                                          #.........
                                          ..........
                                          ......#...
                                          .#........
                                          .........#
                                          ..........
                                          .......#..
                                          #...#.....
                                          """)
            .WithPartOneAnswer("374")
            .WithPartTwoAnswer("1030");
    }
}