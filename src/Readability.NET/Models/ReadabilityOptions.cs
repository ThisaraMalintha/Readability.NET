namespace Readability.NET.Models;

/// <summary>
/// Options to be passed in to the Mozilla readability parser.
/// <para>
/// <see href="https://github.com/mozilla/readability?tab=readme-ov-file#api-reference"/>
/// </para>
/// </summary>
public class ReadabilityOptions
{
    /// <summary>
    /// Whether to enable logging.
    /// </summary>
    [JsonPropertyName("debug")]
    public bool Debug { get; set; }

    /// <summary>
    /// The maximum number of elements to parse. Default is 0 (no limit).
    /// </summary>
    [JsonPropertyName("maxElemsToParse")]
    public int? MaxElemsToParse { get; set; }

    /// <summary>
    /// The number of top candidates to consider when analyzing how tight the competition is among candidates.
    /// </summary>
    [JsonPropertyName("nbTopCandidates")]
    public int? NbTopCandidates { get; set; }

    /// <summary>
    /// The number of characters an article must have in order to return a result.
    /// </summary>
    [JsonPropertyName("charThreshold")]
    public int? CharThreshold { get; set; }

    /// <summary>
    /// A set of classes to preserve on HTML elements when the keepClasses options is set to false.
    /// </summary>
    [JsonPropertyName("classesToPreserve")]
    public List<string>? ClassesToPreserve { get; set; }

    /// <summary>
    /// Whether to preserve all classes on HTML elements. When set to false only classes specified in the classesToPreserve array are kept.
    /// </summary>
    [JsonPropertyName("keepClasses")]
    public bool KeepClasses { get; set; }

    /// <summary>
    /// When extracting page metadata, Readability gives precedence to Schema.org fields specified in the JSON-LD format. 
    /// Set this option to true to skip JSON-LD parsing.
    /// </summary>
    [JsonPropertyName("disableJSONLD")]
    public bool DisableJSONLD { get; set; }

    /// <summary>
    /// Controls how the content property returned by the parse() method is produced from the root DOM element.
    /// </summary>
    [JsonIgnore]
    public Func<object, string> Serializer => throw new NotSupportedException();

    /// <summary>
    /// A regular expression that matches video URLs that should be allowed to be included in the article content.
    /// </summary>
    [JsonPropertyName("allowedVideoRegex")]
    public string? AllowedVideoRegex { get; set; }

    /// <summary>
    /// A number that is added to the base link density threshold during the shadiness checks.
    /// This can be used to penalize nodes with a high link density or vice versa.
    /// </summary>
    [JsonPropertyName("linkDensityModifier")]
    public double? LinkDensityModifier { get; set; }

    public static ReadabilityOptions Empty() => new();
}
