using Microsoft.CodeAnalysis;

namespace Dnf.SourceGenerators
{
    public class AdditionalFilesOptions
    {
        public string? Type { get; set; }

        public AdditionalText? AdditionalText { get; set; }
    }
}