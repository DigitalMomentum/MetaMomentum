import { html, customElement, property, state, nothing, ifDefined, css } from "@umbraco-cms/backoffice/external/lit";
import { UmbPropertyEditorConfigCollection, UmbPropertyEditorUiElement, UmbPropertyValueChangeEvent } from "@umbraco-cms/backoffice/property-editor";
import { MetaValue } from "../../types/metaValue";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT, UmbDocumentWorkspaceContext } from "@umbraco-cms/backoffice/document";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import "../../extensions/string"
import { UmbImagingRepository } from "@umbraco-cms/backoffice/imaging";
import { ImageCropModeModel } from "@umbraco-cms/backoffice/external/backend-api";
import { UMB_PROPERTY_DATASET_CONTEXT } from "@umbraco-cms/backoffice/property";
import { RtePropertyValue } from "../../types/rtePropertyValue";
import { UmbMediaPickerPropertyValue } from "@umbraco-cms/backoffice/media";
import { MetaEditorConfig } from "../../types/metaEditorConfig";

@customElement('dm-meta-momentum')
export default class MetaMomentumPropertyEditorUIElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	private workspaceContext?: UmbDocumentWorkspaceContext;
	private imagingRepository = new UmbImagingRepository(this);


	private fallbackValues: {
		[alias: string]:
		| string
		| Array<string>
		| Array<UmbMediaPickerPropertyValue>;
	} = {};


	@property({ type: Object })
	public value: MetaValue = {} as MetaValue;

	@state()
	private _showSearchCustomiser: boolean = false;

	@state()
	private _host: string = window.location.host;

	@state()
	private _nodeUrl: string | undefined;

	@state()
	private _activeSharePreview: string = "facebook";
	@state()
	private _showSocialCustomiser: boolean = false;
	@state()
	private _facebookImgUrl: string | undefined;

	@state()
	private _twitterImgUrl: string | undefined;

	@state()
	private _linkedinImgUrl: string | undefined;


	#config: MetaEditorConfig | undefined;

	@property({ attribute: false })
	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#config = {
			fallbackTitleFields: [
				...(config.getValueByAlias<string>("fallbackTitleFields")?.split(",") ??
					[]),
			],
			fallbackDescriptionFields: config
				.getValueByAlias<string>("fallbackDescriptionFields")
				?.split(","),
			fallbackImageFields: config
				.getValueByAlias<string>("fallbackImageFields")
				?.split(","),
		};
	}

	constructor() {
		super();

		this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (_instance) => {

			this.workspaceContext = _instance;


			this.initValue();
			this.initNodeUrl();

		});


		this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, async (context) => {
			const stringFields = [
				...(this.#config?.fallbackTitleFields ?? []),
				...(this.#config?.fallbackDescriptionFields ?? []),
			];

			this.#config?.fallbackTitleFields?.push("name");

			const imageFields = [...(this.#config?.fallbackImageFields ?? [])];

			// Keep track of the name field separately
			this.observe(context.name, (name) => {
				console.log("Setting fallbackValue: name to " + name);
				this.fallbackValues["name"] = name ?? "";

				// Update the fallback values
				// this.#updateSearchTitle();
				// this.#updateShareTitle();
			});

			// Loop though all fields to observe
			stringFields.forEach(async (field) => {


				//Keep track of the fields that we need to check and subscribe to changes
				context?.propertyValueByAlias<string | RtePropertyValue>(field).then((property) => {
					property?.subscribe((propertyValue) => {


						if (typeof propertyValue === "string") {
							this.fallbackValues[field] = propertyValue ?? "";
							console.log("Setting fallbackValue: " + field + " to " + propertyValue);

						} else if (typeof propertyValue === "object" && propertyValue !== null && "markup" in propertyValue) {
							this.fallbackValues[field] = (propertyValue as RtePropertyValue).markup ?? "";
							console.log("Setting fallbackValue: " + field + " to " + (propertyValue as RtePropertyValue).markup);

						}

						this.#updatePreviewValues();

					});

				});
			});


			// keep track of image property value changes
			imageFields.forEach(async (field) => {
				console.log("Observing changes for " + field);
				context.propertyValueByAlias<Array<UmbMediaPickerPropertyValue>>(field).then((property) => {
					property?.subscribe((propertyValue) => {
						this.fallbackValues[field] = propertyValue ?? [];

						console.log("Setting fallbackValue: " + field + " to " + propertyValue?.map((item) => item.mediaKey));

						// Update fallback values
						this.#updateShareImage();

						this.#updatePreviewValues();
					}
					);
				});

			});

		});



	}

	/**
   * @description Init the value if require, could also migrate from older versions here.
   */
	initValue() {


		if (!this.value) {
			this.value = {
				title: "",
				description: "",
				noIndex: false,
				shareTitle: "",
				shareDescription: "",
				shareImage: undefined,
				default: {
					title: "",
					description: "",
				},
				share: {
					title: "",
					description: "",
					image: undefined,
				},
			};

		}

		// This is for backwards compatibility. Previous versions stored the media as a UDI string, and we now just use the guid
		//TODO: come back and check backwards compatibility
		//this.value = { ...this.value, ...{shareImage: this.value.shareImage?.convertUdiToGuid() } };


		this.#updateShareImageUrls();

		this.#updatePreviewValues();
	}


	// Updates the preview values based on the current value and fallback values
	#updatePreviewValues() {

		let title = this.#getFallbackStringValue(this.#config?.fallbackTitleFields, this.value.default.title);
		let description = this.#getFallbackStringValue(this.#config?.fallbackDescriptionFields, this.value.default.description);


		let shareTitle = this.#getFallbackStringValue(this.#config?.fallbackTitleFields, this.value.share.title, this.value.default.title);
		let shareDescription = this.#getFallbackStringValue(this.#config?.fallbackDescriptionFields, this.value.share.description, this.value.default.description);

		this.value = { ...this.value, ...{ title: title, description: description, shareTitle: shareTitle, shareDescription: shareDescription } };



		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	// Works out what value to use based on custom value, custom fallback value and then loop though the field aliases to find a value
	#getFallbackStringValue(fieldAliases: Array<string> | undefined, customValue: string | undefined, customFallbackValue: string | null = null): string {
		let result: string = "";

		if (customValue !== undefined && customValue !== "") {
			return customValue;
		} else if (customFallbackValue !== null && customFallbackValue !== "") {
			return customFallbackValue;
		} else {
			for (let i = 0; i < (fieldAliases?.length ?? 0); i++) {

				let field = fieldAliases![i];
				if (!field) continue;
				const fallbackValue = this.fallbackValues[field];
				if (fallbackValue !== undefined && fallbackValue !== "") {
					//this.value = { ...this.value, ...{ title: fallbackValue as string } };
					result = fallbackValue as string;


					break;
				}
			};
		}


		return result;

	}

	#toggleSearchCustomiser() {
		this._showSearchCustomiser = !this._showSearchCustomiser;
		if (this._showSearchCustomiser) {
			document.querySelector("#SearchPreview")?.scrollIntoView({
				behavior: "smooth",
			});
		}
	}

	#toggleSocialCustomiser() {
		this._showSocialCustomiser = !this._showSocialCustomiser;
		if (this._showSocialCustomiser) {
			document.querySelector("#SocialPreview")?.scrollIntoView({
				behavior: "smooth",
			});
		}
	}


	initNodeUrl() {
		if (this.workspaceContext != undefined) {
			if (this.workspaceContext.urls != undefined) {
				this.workspaceContext.urls.subscribe((_urls) => {
					if ((_urls?.length ?? 0) == 0 || _urls[0].url == undefined) {
						return;
					}
					if (_urls[0].url.startsWith("/")) {
						this._nodeUrl = _urls[0].url;
					} else if (_urls[0].url.startsWith("http")) {
						const endOfDomain = _urls[0].url.split("/", 3).join("/").length;
						this._host = _urls[0].url.substring(0, endOfDomain);
						this._nodeUrl = _urls[0].url.substring(endOfDomain);
					}

					if (this._nodeUrl) {
						this._nodeUrl = this._nodeUrl.replace(/\/$/g, "").replace(/\//g, " > ");
					}
				});
			}
		}
	}


	// Called when the custom title or description fields are updated
	#onUpdateMetaValue(e: InputEvent) {
		let source = (e.target as HTMLInputElement);
		let property: string = source.dataset.bind!;

		// Update default.title or default.description
		var defaults = { ...this.value.default, ...{ [property]: source.value } };
		this.value = { ...this.value, ...{ default: defaults } };

		this.#updatePreviewValues();

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	// Called when the custom share title or description fields are updated
	#onUpdateShareValue(e: InputEvent) {
		let source = (e.target as HTMLInputElement);
		let property: string = source.dataset.bind!;

		// Update default.title or default.description
		var share = { ...this.value.share, ...{ [property]: source.value } };
		this.value = { ...this.value, ...{ share: share } };

		this.#updatePreviewValues();

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#onUpdateMetaCheckedInverse(e: InputEvent) {
		let source = (e.target as HTMLInputElement);
		let property: string = source.dataset.bind!;
		console.log(source.checked);

		this.value = { ...this.value, ...{ [property]: !source.checked } };
		console.log(this.value);
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}


	/**
   * @description Sets the new value for Share Image and triggers an update on Share Image to
   * cause the updated values to be set and internal url state variables to be set.
   */
	async #onShareImageChanged(newShareImageValue: string | undefined) {

		let newValue: MetaValue = {
			...this.value,
			...{
				share: {
					...this.value.share,
					image: newShareImageValue
				}
			}
		};


		newValue = await this.#updateShareImage(newValue);
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}



	/**
  * @description Updates the share image with the overridden value or fallback value
  */
	async #updateShareImage(value?: MetaValue) {

		if (this.value == undefined) {
			return this.value;
		}
		if (value == undefined) {
			value = this.value;
		}

		var result = await this.imagingRepository.requestThumbnailUrls(
			[value.share!.image!], 0, 0
		);

		console.log(result)


		if (value?.share?.image !== undefined && value.share.image !== null) {
			this.value = {
				...value,
				...{
					shareImage: value.share.image,
				},
			};
		}

		if (value?.shareImage === undefined) {
			this.value = {
				...value,
				...{
					shareImage: undefined //this.#getImageFallbackValue(),
				},
			};
		}
		console.log(this.value)
		this.#updateShareImageUrls();
		return value;
	}

	/**
	* @description Updates internal component state with the selected image urls
	*/
	async #updateShareImageUrls() {

		const unique = this.value?.shareImage !== undefined ? this.value?.shareImage : undefined;


		console.log(unique)
		if (unique == undefined || unique == "") {
			return;
		}

		let imageData = (
			await this.imagingRepository.requestThumbnailUrls(
				[unique],
				281,
				540,
				ImageCropModeModel.CROP
			)
		).data;
		this._facebookImgUrl = imageData?.[0]?.url ?? "";

		imageData = (
			await this.imagingRepository.requestThumbnailUrls(
				[unique],
				220,
				438,
				ImageCropModeModel.CROP
			)
		).data;
		this._twitterImgUrl = imageData?.[0]?.url ?? "";

		imageData = (
			await this.imagingRepository.requestThumbnailUrls(
				[unique],
				263,
				520,
				ImageCropModeModel.CROP
			)
		).data;
		this._linkedinImgUrl = imageData?.[0]?.url ?? "";
	}







	render() {
		return html`

        <uui-icon-registry-essentials>
          <uui-box
            id="#SearchPreview"
            headline="Search Engine Appearance"
            headline-variant="h5"
            style="margin-bottom:20px;"
          >
            <uui-toggle
              slot="header-actions"
              value=${this._showSearchCustomiser}
              label="Customise"
              @change=${this.#toggleSearchCustomiser}
            ></uui-toggle>
			



            ${this.renderSearchPreview()} 
            ${this.renderSearchCustomiser()}
            ${this.renderSearchIndexingSetting()}
          </uui-box>
    
      <uui-box
        id="#SocialPreview"
        headline="Social Share Appearance"
        headline-variant="h5"
        style="min-width:620px; padding-bottom:20px;"
      >
        <uui-toggle
          slot="header-actions"
          value=${this._showSocialCustomiser}
          label="Customise"
          @change=${this.#toggleSocialCustomiser}
        ></uui-toggle>
        <uui-tab-group style="margin-top:20px; margin-bottom:10px">
          <uui-tab
            label="Facebook"
            .active="${this._activeSharePreview === "facebook"}"
            @click="${() => {
				this._activeSharePreview = "facebook";
				this.requestUpdate();
			}}"
          >
            <uui-icon slot="icon" name="icon-facebook"></uui-icon>
          </uui-tab>
          <uui-tab
            label="X"
            .active="${this._activeSharePreview === "twitter"}"
            @click="${() => {
				this._activeSharePreview = "twitter";
				this.requestUpdate();
			}}"
          >
            <uui-icon slot="icon" name="icon-twitter-x"></uui-icon>
          </uui-tab>
          <uui-tab
            label="Linked In"
            .active="${this._activeSharePreview === "linkedin"}"
            @click="${() => {
				this._activeSharePreview = "linkedin";
				this.requestUpdate();
			}}"
          >
            <uui-icon slot="icon" name="icon-linkedin"></uui-icon>
          </uui-tab>
        </uui-tab-group>

        ${this.#renderSocialPreview()} 
        ${this.#renderSocialCustomiser()}
      </uui-box>
      </uui-icon-registry-essentials>


      //	<pre>${JSON.stringify(this.value, null, 2)}</pre>
      //<pre>${JSON.stringify(this.fallbackValues, null, 2)}</pre>




          `;
	}

	renderSearchPreview() {
		if (this.value != undefined) {
			return html`
         
            <div
              style="color:#202124;font-size:14px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;font-family:Arial, sans-serif"
            >
              ${this._host}<span style="color: #4d5156;">${this._nodeUrl}</span>
            </div>
            <div
              style="width:570px;color:#1a0dab;font-size:20px; line-height:21px; margin: 6px 0; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;word-wrap:break-word; font-family:Arial, sans-serif"
            >
              ${this.value.title}
            </div>
            <div style="color:#4d5156;font-size:14px;font-family:Arial, sans-serif">
              ${this.value.description?.truncateAsHtml(160)}
            </div>
          `;
		} else {
			return nothing;
		}
	}


	renderSearchCustomiser() {
		if (this._showSearchCustomiser === false || this.value == undefined) {
			return nothing;
		}
		return html`
          <umb-stack look="compact">
            <umb-property-layout
              label="Title"
              description="The title will appear in the browser tab and as the first line in search results."
            >
              <div
                slot="editor"
                style="font-size:14px;text-align:right;"
                class="umb-textstring ${this.value?.title?.length &&
				this.value.title.length >= 60
				? "danger"
				: this.value?.title?.length && this.value.title.length >= 45
					? "warning"
					: ""}"
              >
                ${this.value?.title?.length ?? 0}/ 60
              </div>
              <uui-input
                slot="editor"
                .value=${this.value.default.title ?? ""}
                .placeholder=${this.value.title ?? ""}
                @input=${this.#onUpdateMetaValue}
                data-bind="title"
                type="text"
                style="width:100%;"
              ></uui-input>
              <div
                slot="editor"
                style="font-size:14px;color:#202124;text-align:right;"
                class="umb-textstring"
              >
                <a href="https://moz.com/learn/seo/title-tag" target="_blank"
                  ><i
                    >How to write a good title
                    <i
                      class="icon-out"
                      style="font-size:16px;display:inline-block;transform:translateY(2px)"
                    ></i></i
                ></a>
              </div>
            </umb-property-layout>
            <umb-property-layout
              label="Description"
              description="The description provides a brief summary of a web page and is shown under the title in search results."
            >
              <div
                slot="editor"
                style="font-size:14px;text-align:right;"
                class="umb-textstring ${this.value?.description?.length &&
				this.value.description.length >= 160
				? "danger"
				: this.value?.description?.length &&
					this.value.description.length >= 150
					? "warning"
					: ""}"
              >
                ${this.value?.description?.length ?? 0} / 160
              </div>
              <uui-textarea
                slot="editor"
                .value=${this.value.default.description}
                .placeholder=${this.value.description}
                @input=${this.#onUpdateMetaValue}
                data-bind="description"
                rows="2"
                style="width:100%;"
              ></uui-textarea>
              <div
                slot="editor"
                style="font-size:14px;color:#202124;text-align:right;"
                class="umb-textstring"
              >
                <a href="https://moz.com/learn/seo/meta-description" target="_blank"
                  ><i
                    >Write a compelling description
                    <i
                      class="icon-out"
                      style="font-size:16px;display:inline-block;transform:translateY(2px)"
                    ></i></i
                ></a>
              </div>
            </umb-property-layout>
          </umb-stack>
        `;
	}

	renderSearchIndexingSetting() {
		if (this.value == undefined) {
			return nothing;
		}
		return html`
          <umb-property-layout label="Search Engine Visibility">
            <umb-input-toggle
              slot="editor"
              .checked=${!this.value.noIndex}
              
              labelOn="Allow search engines to index this page"
              labelOff="Discourage search engines from indexing this page"
              ?showlabels=${true}
              @change=${this.#onUpdateMetaCheckedInverse}
              data-bind="noIndex"
              style="--uui-color-selected: var(--uui-color-positive); --uui-color-selected-emphasis: var(--uui-color-positive-emphasis)"
            ></umb-input-toggle>
          </umb-property-layout>

        `;
	}

	#renderSocialPreview() {
		if (this.value == undefined) {
			return nothing;
		}
		if (this._activeSharePreview === "facebook") {
			return html`
            <div
              style="width:527px;font-family:Helvetica, Arial, sans-serif;position:relative;"
            >
                <!-- 
                    It'd be nice to use <umb-imaging-thumbnail> here but it doesn't refresh when unique="" changes
                    So we work around this by storign the urls in state
                -->
              <img
                src="${ifDefined(this._facebookImgUrl)}"
                style="border:1px solid #dddfe2;width:calc(100% - 2px);"
              />
              <div
                style="border:1px solid #dddfe2;background-color:#e9ebee;padding:10px 12px;"
              >
                <div
                  style="color:#606770;text-transform:uppercase; line-height:11px;font-size:12px"
                >
                  ${this._host}
                </div>
                <div
                  style="margin-top:6px;margin-bottom: 1px;color:#1d2129;font-size:16px;font-weight:600;line-height:20px;overflow:hidden;white-space:nowrap;overflow-wrap:break-word;text-overflow:ellipsis"
                  ng-bind-html="updateShareTitle()"
                >
                  ${this.value.shareTitle}
                </div>

                <div
                  style="color:#4d5156;font-size:14px;overflow:hidden;white-space:nowrap;overflow-wrap:break-word;text-overflow:ellipsis"
                >
                  ${this.value.shareDescription}
                </div>
              </div>
            </div>
          `;
		} else if (this._activeSharePreview == "twitter") {
			return html`
            <div
              style="width:438px;font-family:'Helvetica Neue' ,Helvetica,Arial,sans-serif;font-size:14px;position:relative;line-height:1.3em;border-radius:.85714em;overflow:hidden;border:1px solid #dddfe2;"
            >
              <img
                src="${ifDefined(this._twitterImgUrl)}"
                style="border:1px solid #dddfe2;width:calc(100%);"
              />
              <div style="padding:10px 12px;">
                <div
                  style="max-height:1.3em; white-space:nowrap;overflow:hidden;text-overflow:ellipsis;font-weight:700;margin:0 0 .15em;color:#000;"
                >
                  ${this.value.shareTitle}
                </div>

                <div
                  style="max-height:2.6em; overflow:hidden;margin:0;margin-top:.32333em;color:#000;"
                >
                  ${this.value.shareDescription}
                </div>
                <div
                  style="text-transform:lowercase; color:#8899A6;max-height:1.3em;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;margin-top:.32333em;"
                >
                  ${this._host}
                </div>
              </div>
            </div>
          `;
		} else if (this._activeSharePreview == "linkedin") {
			return html`
            <div
              style="width:438px;font-family:-apple-system,system-ui,BlinkMacSystemFont,Segoe UI,Roboto,Helvetica Neue,F;font-size:14px;position:relative;line-height:1.3em;border-radius:.85714em;overflow:hidden;border:1px solid #dddfe2;"
            >
              <img
                src="${ifDefined(this._linkedinImgUrl)}"
                style="border:1px solid #dddfe2;width:calc(100%);"
              />
              <div style="padding: 8px 12px; background-color: #E7EAEC">
                <div
                  style="max-height:40px; white-space:nowrap;overflow:hidden;text-overflow:ellipsis;font-weight:600;margin:0 0 .15em;color:rgba(0,0,0,0.9);"
                >
                  ${this.value.shareTitle}
                </div>

                <div
                  style="line-height:1.33;text-transform:lowercase; color:rgba(0,0,0,0.6);max-height:16px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;margin-top:8px;white-space:nowrap;font-weight:400"
                >
                  ${this._host}
                </div>
              </div>
            </div>
          `;
		}

		return nothing;
	}

	#renderSocialCustomiser() {
		if (!this._showSocialCustomiser || this.value == undefined) {
			return nothing;
		}

		return html`
          <umb-stack look="compact">
            <umb-property-layout
              label="Title"
              description="The title will appear as the title when shared on social media."
            >
              <uui-input
                slot="editor"
                .value=${this.value?.share.title ?? ""}
                @input=${this.#onUpdateShareValue}
                data-bind="title"
                type="text"
                style="width:100%;"
              ></uui-input>
            </umb-property-layout>
            <umb-property-layout
              label="Description"
              description="The share description shown under the title when shared."
            >
              <uui-textarea
                slot="editor"
                .value=${this.value.share.description}
                @input=${this.#onUpdateShareValue}
                data-bind="description"
                rows="2"
                style="width:100%;"
              ></uui-textarea>
            </umb-property-layout>
            <umb-property-layout label="Image">
              <umb-input-media
                slot="editor"
                max="1"
                value=${ifDefined(this.value?.shareImage)}
                data-bind=""
                @change=${async (e: any) => {
				await this.#onShareImageChanged(e.target.value);
			}}
              ></umb-input-media>
            </umb-property-layout>
          </umb-stack>
        `;
	}

	static styles = [
		css`
          .danger {
            color: var(--uui-color-danger-standalone);
          }
          .warning {
            color: var(--uui-color-warning-standalone);
          }
        `,
	];

}

declare global {
	interface HTMLElementTagNameMap {
		'dm-meta-momentum': MetaMomentumPropertyEditorUIElement;
	}
}