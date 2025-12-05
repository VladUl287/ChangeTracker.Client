using System.Text;

namespace Tracker.AspNet.Utils;

public sealed class CacheControlBuilder
{
    private readonly List<string> _directives;
    private int? _maxAge;

    public CacheControlBuilder() => _directives = [];
    public CacheControlBuilder(int capacitiy) => _directives = new(capacitiy);

    public CacheControlBuilder WithDirective(string directive)
    {
        _directives.Add(directive);
        return this;
    }

    public CacheControlBuilder WithMaxAge(TimeSpan duration)
    {
        _maxAge = (int)duration.TotalSeconds;
        return this;
    }

    private void AppendNumericDirectives(StringBuilder sb)
    {
        if (_maxAge.HasValue)
            sb.Append($"max-age={_maxAge.Value}");
    }

    private void AppendCustomDirectives(StringBuilder sb)
    {
        foreach (var directive in _directives)
            sb.Append(directive);
    }

    public string Build()
    {
        var sb = new StringBuilder("Cache-Control: ");

        AppendNumericDirectives(sb);
        AppendCustomDirectives(sb);

        return sb.ToString();
    }
}
