import { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/property-editor';

const propertyEditors: Array<ManifestPropertyEditorUi> = [
    {
        type: 'propertyEditorUi',
        alias: 'DM.MetaMomentum',
        name: 'Meta Momentum',
        element: () => import('./meta-momentum/meta-momentum.element'),
        meta: {
            label: "Meta Momentum",
            propertyEditorSchemaAlias: "Umbraco.Plain.Json",
            icon: "icon-code",
            group: "seo",
            settings: {
                properties: [
                    {
						label: "Title / Description fields for search",
						description: "Show the Title and Description fields to the editor",
						alias: "showSearchFields",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle",
					},
					{
						label: "Google search preview",
						description: "Show a preview of how the Google search entry will look",
						alias: "showSearchPreviewGoogle",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
					},
					{
						label: "Show Index Option",
						description: "Show the Search Engine Visibility toggle. This toggle allows the user to select a page not to be indexed by google (NoIndex)",
						alias: "showNoIndexdOption",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
					},
					{
						label: "Fallback Title Fields",
						description: "enter a csv of fallback property editors aliases",
						alias: "fallbackTitleFields",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox",
					},
					{
						label: "Fallback Description Fields",
						description: "Enter a csv of fallback property editors aliases",
						alias: "fallbackDescriptionFields",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox",
					},
					{
						label: "Fallback Image Fields",
						description: "Enter a csv of fallback property editors aliases",
						alias: "fallbackImageFields",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox",
					},
					{
						label: "Social Share fields",
						description: "Show the Title and Description and share image fields for Open Graph/Twitter to the editor",
						alias: "showSocialFields",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
					},
					{
						label: "Facebook share preview",
						description: "Show a preview of how the Facebook link preview will look",
						alias: "showSocialPreviewFacebook",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
					},
					{
						label: "Twitter share preview",
						description: "Show a preview of how the Twitter link preview will look",
						alias: "showSocialPreviewTwitter",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
					},
					{
						label: "LinkedIn share preview",
						description: "Show a preview of how the LinkedIn link preview will look",
						alias: "showSocialPreviewLinkedIn",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.Toggle"
					},
					{
						label: "X (aka Twitter) Name",
						description: "A twitter handle related to your company or site.\n\r e.g. `@digitalmomentum`",
						alias: "twitterName",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox"
					},
					{
						label: "Site Name",
						description: "This is the name of your website/brand/company.\n\r e,g, `Digital Momentum`",
						alias: "siteName",
						propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox"
					}
                ],
                defaultData: [
                    {
                        alias: "showSearchFields",
                        value: true,
                    },
                    {
                        alias: "showSearchPreviewGoogle",
                        value: true,
                    },
                    {
                        alias: "showNoIndexdOption",
                        value: true,
                    },
                    {
                        alias: "showSocialFields",
                        value: true,
                    },
                    {
                        alias: "showSocialPreviewFacebook",
                        value: true,
                    },
                    {
                        alias: "showSocialPreviewTwitter",
                        value: true,
                    },
                    {
                        alias: "showSocialPreviewLinkedIn",
                        value: true,
                    },
                    {
                        alias: "fallbackTitleFields",
                        value: "title,pageTitle,heroHeader,productName,name",
                    },
                    {
                        alias: "fallbackDescriptionFields",
						value: "description,heroDescription,excerpt,seoMetaDescription",
                    },
                    {
                        alias: "fallbackImageFields",
                        value: "featureImage,heroImage,mainImage,photo,photos,HeroBackgroundImage",
                    },
                ]
            }
        },
    }
];

export const manifests = [...propertyEditors];