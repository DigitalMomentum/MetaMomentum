# Meta Momentum for Umbraco

## Note: This is an alpha version for Umbraco 15.0+ and is not yet ready for production use. Please use the Umbraco 8.x version for production sites.

#### Completed Features:
- [X] Supports V15+
- [x] Search and Socials previews (although, I'll need to come back and review these to perfect the latest changes in the platforms)
- [x] Support Migration from V2.x to V3.0, so no data migration needs to occur
- [x] Replace the partial view for rendering tags with TagHelpers. `<meta-momentum meta-values="@(Model.Value<MetaValues>("MetaMomentum"))"></meta-momentum>`
- [x] Add additional tag helpers to allow for customisation of tags (see [TagHelpers](https://github.com/DigitalMomentum/MetaMomentum/tree/V3/3.0.0?tab=readme-ov-file#tag-helpers) in the V3 Readme
- [x] Remove MetaMomentum.Core - As this is now a RazorClass Library as a DLL, there are no front end files to mess up our class projects.
- [x] Remove FacebookAppId: Facebook no longer uses this
- [x] Move OGSiteName and TwitterName into the data type config
- [x] Remove services.AddMetaMomentum(). No longer needed
- [ ] UI previews to handle fall backs 
- [ ] Move fall-back logic to server side, so that programmatic changes to the content node will automatically be reflected on the front end


#### A DataType to manage Search engine results (Title, Description & No Follow) /  open Graph (Facebook / LinkedIn) / Twitter cards

Meta Momentum is an Umbraco Data Type editor to manage SEO Title & description tags and Open Graph / Twitter Cards, 
with a visual representation of how it will look in a Google Search / Social Platform. 

![Video of Meta Momentum in action](GithubFiles/Images/Newtons_Cradle_In_Action.gif)

## SEO Search display features
The search display follows googles search restrictions in terms of titles being restricted by width, instead of a character
count. This gives the user a good idea on how the title / description will show in an actual google search.

Most importantly, you can specify fall backs to other properties on the same page/node, so that the title for instance will
fall back to a "Page Title" property if no title tag is supplied by the user. We all know editors are terrible at keeping
these fields up to date, so you can make sure its handled for them!

## Share Features
The Social Share preview will give an accurate preview of the Facebook, Twitter and Linked in share previews. 
The content editor can switch between the 3 previews to see how they will look when shared.

You can specify fall backs to other fields for the share title and description. 

## Editor Screenshot
![Google  Editor previews](GithubFiles/Images/Edit_SEO_Screenshot.png)
![Facebook Editor previews](GithubFiles/Images/Edit_SocialShare_Screenshot.png)


## Installation
Installation of the package is done via NuGet package manager.
```bash
Install-Package MetaMomentum
```


After installation, you can create and configure a new MetaMomentum Data Type **Umbraco -> Settings -> Data Types -> New Data Type -> Select Meta Momentum**. 

Install stable releases via Nuget; development releases are available via MyGet.

| Package Name                   | Release (NuGet) | Nightly (MyGet) |
|--------------------------------|-----------------|-----------------|
| `MetaMomentum`         | [![NuGet](https://img.shields.io/nuget/v/MetaMomentum.svg)](https://www.nuget.org/packages/MetaMomentum/) | [![MyGet](https://img.shields.io/myget/digital-momentum/vpre/MetaMomentum.svg)](https://www.myget.org/feed/digital-momentum/package/nuget/MetaMomentum) |
| `MetaMomentum.Core`         | [![NuGet](https://img.shields.io/nuget/v/MetaMomentum.Core.svg)](https://www.nuget.org/packages/MetaMomentum.Core/) | [![MyGet](https://img.shields.io/myget/digital-momentum/vpre/MetaMomentum.Core.svg)](https://www.myget.org/feed/digital-momentum/package/nuget/MetaMomentum.Core) |

Previous Versions can be used for earlier versions of Umbraco.

| Umbraco Version | MetaMomentum Version |
|-----------------|----------------------|
| 8.x - 13.x      | 1.x  - 2.x           |
| 15.x            | 3.x                  |


## Configuration
Configuration can be done via the following Data Type setting fields: 

- *Title / Description fields for search:* Turn this on to allow the content editor to edit the title and description tags. 
If turned off the option to set the meta title and description will be removed from the editor.

- *Google Search Preview:* Turn on to show the google search preview to the content editor. This will show a preview of how the Google search entry will look

- *Fallback Title Fields:* You can enter a comma separated list of DataType aliases, so that if the title is not filled in by the user, it will fall back to each alias in the list. 
If the alias is not filled in or does not exist in the DocType, then it will fall back to the next, until finally, it will use the page name as the title. 
The fall backs only support fields in the form of a string, and does not currently support complex data types like the grid or nested content.

- *Fallback Description Fields:* This is the same as the fall back Title fields setting, however, if it finds no value, the description will be empty. 

- *Fallback Image Fields:* Same as fall back Title Fields, but for images. You can specify media picker or upload fields as fall backs. *Note: Upload fields have Limitations: When the image falls back to an upload field, the first time an image is selected, it needs to be saved and published twice (once to upload the image, and then a second time for it to be saved into the social media image).*

- *Social Share fields:* Turn this on to allow the content editor to edit the title and description tags for the share meta. 
If turned off the content editor will not be able to edit the title and description and will be shown the text from the default fall backs.

- *Facebook, Twitter, LinkedIn share preview*: These 3 options will allow you to turn on / off the different share previews available to the content editor. 


## Usage

### V3.0+ (Umbraco 15+) - Tag Helpers


The easiest way to use MetaMomentum is though the help of Tag Helpers.

In your _ViewImports.cshtml file add the following line:
```c#
@addTagHelper *, MetaMomentum
```

Then in your header you can add the following tag helper to do the heavy lifting and add all the meta tags for you:
```html
<meta-momentum meta-values="@(Model.Value<MetaValues>("MetaMomentumPropertyAlias"))"></meta-momentum>
```

If you would like to add the tags manually, you can use individual tags helpers or use the MetaMomentum model to output the values manually.


### V2.0+ (Umbraco 8.x+ to 15+)

If you would like manual control over the meta tags, you can create the following partial view to output the tags and adjust as necessary.

`@Html.Partial("MetaMomentum/RenderMetaTags", Model.Value("metaMomentum"))`

```html
@inherits UmbracoViewPage<MetaValues>
@using MetaMomentum.Models;

@{	if (Model == null) { return; } }
@if (!string.IsNullOrWhiteSpace(Model.Title)){
	<title>@(Model.Title)</title>
}

<meta name="description" content="@Model.Description">

<meta property="og:title" content='@Model.ShareTitle' />
<meta property="og:description" content="@Model.ShareDescription" />
@if (Model.ShareImageUrl != null)
{
    <meta property="og:image:width" content="1200" />
    <meta property="og:image:height" content="630" />
	<meta property="og:image" content="@Model.GetAbsoluteShareImageUrl(Context.Request)?width=1200&height=630&bgcolor=white" />
	<meta name="twitter:image" content="@Model.GetAbsoluteShareImageUrl(Context.Request)?width=1200&height=600&bgcolor=white">
}

@if (Model.NoIndex){
	<meta name="robots" content="noindex">
}

<meta property="og:type" content="website" />
@if (Model.OGSiteName != null){
	<meta property="og:site_name" content="@Model.OGSiteName" />
}
@if (Model.TwitterName != null){
	<meta name="twitter:site" content="@Html.Raw(Model.TwitterName)">
}

<meta name="twitter:card" content="summary_large_image">
<meta name="twitter:title" content="@Model.ShareTitle">
<meta name="twitter:description" content="@Model.ShareDescription">

```

## Migrating from V2.x to V3.0

1. AppSettings config has been moved to the datatype setings. If you have a `MetaMomentum` section in your app.config, you will need to copy these across to the settings in the Data Type editor for the following Fields:
 - **OGSiteName:** copy `Site Name`
 - **TwitterName:** copy `Twitter Name`
 - **FacebookId:** Facebook no longer uses this, so it can be safely removed
2. If you have created settings in the `startup.cs` or `program.cs` file, you can remove the `services.AddMetaMomentum()` and move any of the settings over to the Data Type settings as per point 1.
3. If you have used the `@Html.Partial("MetaMomentum/RenderMetaTags", Model.Value("metaMomentum"))` HTML Partial, you can remove this and replace it with the `<meta-momentum meta-values="@(Model.Value<MetaValues>("MetaMomentum"))"></meta-momentum` Tag Helper. Make sure you add the `@addTagHelper *, MetaMomentum` to your _ViewImports.cshtml file.
  


# Tag Helpers

## \<meta-momentum>

The `<meta-momentum>` tag helper is used to output all the meta tags for the page.

Attributes:
- `meta-values` - The MetaMomentum model to use for the meta tags.
- `og-site-name` - Override the site name for Open Graph tags.
- `twitter-name` - Override the Twitter name for Twitter Card tags.
- `no-index` - Override the No Index setting for the page.

Example:
``` html
<meta-momentum meta-values="@(Model.Value<MetaValues>("MetaMomentum"))"></meta-momentum>``
```


## \<meta-open-graph>
Outputs the Open Graph meta tags required for website sharing.
Attributes:
- `meta-values` - The MetaMomentum model to use for the meta tags. If this is supplied, all other attributes are ignored.
- `title` - The title to show in the share
- `description` - The description to show in the share
- `ShareImageUrl` - The full URL of an image to show in the share.
- `OGSiteName` - This is the name of your website/brand/company.

## \<meta-twitter-card>
Outputs the Open Graph meta tags required for website sharing.
Attributes:
- `meta-values` - The MetaMomentum model to use for the meta tags. If this is supplied, all other attributes are ignored.
- `title` - The title to show in the share
- `description` - The description to show in the share
- `ShareImageUrl` - The full URL of an image to show in the share.
- `TwitterName` - This is the name of your twitter / X account (e.g. `@digitalmomentum`).



## Version History


##### V3.0:
- Update UI to be fully compatible with Umbraco 15 web components
- move from HTML Partial to Tag Helpers
- Remove Facebook AppId tag as its no longer used by Facebook - https://yoast.com/help/fb-app-id-warnings/
- AppSettings config moved to DataType Editor


##### V2.2:
- Update UI to use the new Umbraco UI Library
- RTE / Html fallbacks now strip the HTML tags
- Bug Fixes and performance inprovements
- 
##### V2.1.1:
- Bugfix: Umbraco displays "Discard Changes?" when changing bewteen social previews
- Featue: Allow hiding the "No Index" toggle via settings

##### V2.1:
- Split Compiled DLL into separate MetaMomentum.Core project

##### V2.0:
 - Full Umbraco 9.0+ Support
 - Bug fix: Fallbacks to the Image Cropper field threw an error

##### V1.2:
 - Added mediapicker3 support for Umbraco 8.14+

##### V1.1:
 - Added support for fallback images.
 - Added **og:title** tag (defaulting to website) to the RenderMetaTags.cshtml partial.

##### V1.0:
 - Initial release.



## Pre-Release Versions

If you can't wait for a feature to be released, you can install the latest pre-release version via my MyGet feed. 
These versions are quickly tested, but may at time cause 


## Contributing

To report a new bug, create an issue on the GitHub repository. 

To fix a problem or add features:

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

I recommend creating an issue on the issue tracker before adding to discuss new features to make sure that we can include them. Any contributions you make are greatly appreciated. 


### Running the project

#### Requirements
* Node LTS Version 20.17.0+
* Umbraco 15+ project to test the changes

#### Steps
* Include the MetaMomentum project in an Umbraco 15+ project. Add a reference to the MetaMomentum project in the Umbraco project.
* Open a terminal inside the `\Client` folder
* Run `npm install` to install all the dependencies
* Run `npm run build` to build the project

#### File Watching
* Add this Razor Class Library Project as a project reference to an Umbraco Website project
* From the `\Client` folder run the command `npm run watch` this will monitor the changes to the *.ts files and rebuild the project
* With the Umbraco website project running the Razor Class Library Project will refresh the browser when the build is complete