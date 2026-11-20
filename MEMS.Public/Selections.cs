using MEMS.Shared;

namespace MEMS.Public;

// Maintained only in the WASM client. IDs from an old slate are reconciled with today's directory.
public sealed class Selections(IReadOnlyList<Position> offices, IReadOnlyList<Candidate> candidates)
{
    public HashSet<int> Guide { get; } = [];
    public HashSet<int> Comparison { get; } = [];

    public void Restore(IEnumerable<int> guide, IEnumerable<int> comparison)
    {
        Guide.Clear();
        Comparison.Clear();
        foreach (var id in guide.Distinct())
            if (candidates.FirstOrDefault(c => c.Id == id) is { } c) ToggleGuide(c);
        foreach (var id in comparison.Distinct().Where(id => candidates.Any(c => c.Id == id)).Take(3))
            Comparison.Add(id);
    }

    public string? ToggleGuide(Candidate candidate)
    {
        if (Guide.Remove(candidate.Id)) return null;
        var office = offices.First(p => p.Id == candidate.PositionId);
        if (candidates.Count(c => c.PositionId == office.Id && Guide.Contains(c.Id)) >= office.SeatsAvailable)
            return $"You can choose up to {office.SeatsAvailable} for {office.Name}. Remove a selection first.";
        Guide.Add(candidate.Id);
        return null;
    }

    public string? ToggleComparison(int id)
    {
        if (Comparison.Remove(id)) return null;
        if (Comparison.Count == 3) return "Compare up to three candidates. Remove a profile first.";
        Comparison.Add(id);
        return null;
    }
}
