namespace Common;

public record Range3d(Range<int> X, Range<int> Y, Range<int> Z) {
    public IEnumerable<V3<int>> All() =>
        Z.SelectMany(z => Y.SelectMany(y => X.Select(x => new V3<int>(x, y, z))));

    public Range3d Grow(int n) => new(X.Grow(n), Y.Grow(n), Z.Grow(n));
    public bool Contains(V3<int> v) => X.Contains(v.X) && Y.Contains(v.Y) && Z.Contains(v.Z);
    public bool Contains(Range3d r) => X.Contains(r.X) && Y.Contains(r.Y) && Z.Contains(r.Z);
    public bool IsEmpty() => X.IsEmpty() || Y.IsEmpty() || Z.IsEmpty();

    public Range3d? Intersect(Range3d other) {
        var intersectionX = X.Intersect(other.X);
        var intersectionY = Y.Intersect(other.Y);
        var intersectionZ = Z.Intersect(other.Z);
        if (intersectionX != null && intersectionY != null && intersectionZ != null)
            return new Range3d(intersectionX, intersectionY, intersectionZ);
        return null;
    }

    public bool Intersects(Range3d other) =>
        X.HasIntersection(other.X) && Y.HasIntersection(other.Y) && Z.HasIntersection(other.Z);
    
    public bool HasXYIntersection(Range3d other) => Y.HasIntersection(other.Y) && X.HasIntersection(other.X);

    public Range3d[] Subtract(Range3d other) {
        if (other.Contains(this))
            return Array.Empty<Range3d>();

        var intersect = Intersect(other);
        if (intersect == null)
            return new[] { this };

        return new[] {
                this with { Z = Z with { To = intersect.Z.From } },
                this with { Z = Z with { From = intersect.Z.To } },
                new Range3d(X, Y with { To = intersect.Y.From }, intersect.Z),
                new Range3d(X, Y with { From = intersect.Y.To }, intersect.Z),
                intersect with { X = X with { To = intersect.X.From } },
                intersect with { X = X with { From = intersect.X.To } },
            }
            .Where(x => !x.IsEmpty())
            .ToArray();
    }

    public static Range3d FromMinMax(V3<int> min, V3<int> max) {
        return new Range3d(Range.FromStartAndEndInclusive(min.X, max.X),
            Range.FromStartAndEndInclusive(min.Y, max.Y),
            Range.FromStartAndEndInclusive(min.Z, max.Z));
    }
}