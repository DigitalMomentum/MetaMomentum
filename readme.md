# Meta Momentum for Umbraco 8

#### A DataType to manage Search engine results, open Graph and Twitter cards

Meta Momentum is an Umbraco Data Type editor to manage SEO Title & description tags and Open Graph / Twitter Cards, 
with a visual representation of how it will look in a Google Search / Social Platform. 

The search display follows googles search restrictions in terms of titles being restricted by width, instead of a character count. 
This gives the user a good idea on how the title / description will show in an actual google search.

Additionally, you can specify fallbacks to other text properties in the document, so that the title for instance will fall back to a "Page Title" property 
if no title tag is supplied by the user.

In the future, it will also allow the user to maintain Open Graph and Twitter cards, with previews. 

This plugin was inspired by [Seo Meta by Ryan Lewis](https://github.com/ryanlewis/seo-metadata) which I've used for Umbraco 7 for many years and 
was no longer being maintained and not avaliable for Umbraco 8.

## Installation
The easiest way to install the plugin is though Nuget.

`Install-Package MetaMomentum`

After installation, you can create and configure a new MetaMomentum Data Type **Umbraco -> Settings -> Data Types -> New Data Type**. 

## Configuration
Canfiguration can be done via the following Data Type setting fields: 

- *Fallback Title Fields:* You can enter a comma seperated list of DataType aliases, so that if the title is not filled in by the user, it will fall back to each alias in the list. 
If the alias is not filled in or does not exist in the DocType, then it will fall back to the next, until finally, it will use the page name as the title. 
The fallbacks only support fields in the form of a string, and does not support complex data types like the grid or nested content.

- *Fallback Description Fields:* This is the same as the Fallback Title fields setting, however, if it finds no value, the description will be empty. 

## Usage
There is a razor file that can be found under `/Views/Partials/Metamomentum/RenderMetaTags.cshtml` which will write out the specific tags. 
This can be included in the head of each page like the following (where `metaMomentum` is the DataType alias):

```c#
 @Html.Partial("MetaMomentum/RenderMetaTags", Model.Value("metaMomentum"))
```

Alternativly, you can access each of the properties using the following syntax:

```c#
Title:         @Model.Value<MetaMomentum.Models.MetaValues>.Title
Description:   @Model.Value<MetaMomentum.Models.MetaValues>.Description
```

Or with Models Builder, strongly typed models:

```c#
Title:         @Model.MetaMomentum.Title
Description:   @Model.MetaMomentum.Description
```




## Contributing

To report a new bug, create an issue on the github repository. 

To fix a problem or add features:

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

I reccomend creating an issue on the issue tracker before adding to discuss new features to make sure that we can include them. Any contributions you make are greatly appreciated. 


### Running the project

A working umbraco installation is setup under MetaMomentumPageMeta.Umbraco which contains the App_plugins folder and the partial view. 

You can login to the back office with

email: david@digitalmomentum.com.au
password: password99