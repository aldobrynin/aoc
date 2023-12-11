namespace AoC2020.Day16;

public partial class Day16 {
    public static IEnumerable<SampleInput> GetSamples() {
        yield return SampleInput.ForInput("""
                                          class: 0-1 or 4-19
                                          row: 0-5 or 8-19
                                          seat: 0-13 or 16-19

                                          your ticket:
                                          11,12,13

                                          nearby tickets:
                                          3,9,18
                                          15,1,5
                                          5,14,9
                                          """)
            .WithPartOneAnswer("0")
            .WithPartTwoAnswer("1716");
    }
}