# Meta Momentum for Umbraco
## Search Engine and Social Media Property Editor


#### A DataType to manage Search engine results (Title, Description & No Follow) /  open Graph (Facebook / LinkedIn) / Twitter cards

Meta Momentum is a powerful SEO and social media management tool for Umbraco CMS that helps content editors optimize their pages for search engines and social media platforms.

It was initially born out of the frustration of content editors leaving SEO Description fields blank. 
MetaMomentum handles that by falling back to relevant information in the content when none is provided. It also helps the content editor visualise what it will look like in a search engine, or Social Post. This takes the guess work out of a post will look.


![Video of Meta Momentum in action](GithubFiles/Images/Newtons_Cradle_In_Action.gif)

## What is Meta Momentum?

Meta Momentum provides:
- **SEO Management**: Control title tags, meta descriptions, and search indexing
- **Social Media Cards**: Manage Open Graph (Facebook/LinkedIn) and Twitter Card previews
- **Visual Previews**: See real-time previews of how content appears in Google Search and social platforms
- **Smart Fallbacks**: Automatically use content from other fields when meta fields are empty
- **Easy Integration**: Simple Tag Helper implementation for developers

## Key Features

- ✅ Google Search preview with accurate title width restrictions
- ✅ Social share previews for Facebook, Twitter/X, and LinkedIn
- ✅ Configurable fallback fields for title, description, and images
- ✅ No-index toggle for search engine visibility control
- ✅ Tag Helper-based rendering for clean, maintainable code
- ✅ Compatible with Umbraco 8.x - 17.x

## Documentation Sections

### For Developers
- [Installation Guide](https://github.com/DigitalMomentum/MetaMomentum/wiki/02-Installation)
- [Configuration](https://github.com/DigitalMomentum/MetaMomentum/wiki/03-Configuration)
- [Implementation Guide](https://github.com/DigitalMomentum/MetaMomentum/wiki/04-Implementation)
- [Tag Helpers Reference](https://github.com/DigitalMomentum/MetaMomentum/wiki/05-Tag-Helpers)
- [Migration from V2.x](https://github.com/DigitalMomentum/MetaMomentum/wiki/07-Migration-Guide)

### For Content Editors
- [Using Meta Momentum](https://github.com/DigitalMomentum/MetaMomentum/wiki/06-Content-Editor-Guide)


### Additional Resources
- [Contributing](https://github.com/DigitalMomentum/MetaMomentum/wiki/08-Contributing.md)

## Quick Start

For developers who want to get started quickly:

1. Install via NuGet: `Install-Package MetaMomentum`
2. Create a Data Type in Umbraco backoffice
3. In `_ViewImports.cshtml`, add:
   1. `@addTagHelper *, MetaMomentum`
   2. `@using MetaMomentum.Models`
4. Use `<meta-momentum meta-values="@(Model.Value<MetaValues>("searchEngine"))"></meta-momentum>` tag in the `<head/>` tags of your layout
5. Create a MetaMomentum Document Type
6. Add the document Type with the alias of `searchEngine` to any content nodes that uses the template

See the [Installation Guide](01 Installation) for detailed instructions.

## Support

- **GitHub Issues**: [Report bugs or request features](https://github.com/DigitalMomentum/MetaMomentum/issues)
- **Version**: Currently supports Umbraco 8 (V1), 9-13 (V2), 14-16 (V3) and 17 (V17)