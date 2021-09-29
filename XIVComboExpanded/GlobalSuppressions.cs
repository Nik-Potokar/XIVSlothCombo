// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// General
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Preventing long lines", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "I like regions", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1123:Do not place regions within elements", Justification = "I like regions in elements too", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:Braces should not be omitted", Justification = "This is annoying", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1512:Single-line comments should not be followed by blank line", Justification = "I like this better", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:Single-line comment should be preceded by blank line", Justification = "I like this better", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1127:Generic type constraints should be on their own line", Justification = "I like this better", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "We don't do those yet")]

[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Self explanatory.", Scope = "type", Target = "~T:XIVComboExpandedPlugin.CustomComboPreset")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Self explanatory.", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin.Combos")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Group jobs together.", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin.Combos")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1132:Do not combine fields", Justification = "Not necessary.", Scope = "namespaceanddescendants", Target = "~N:XIVComboExpandedPlugin.Combos")]
