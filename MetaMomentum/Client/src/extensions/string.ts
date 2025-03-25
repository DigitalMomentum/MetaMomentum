import { html, TemplateResult, unsafeHTML } from "@umbraco-cms/backoffice/external/lit";

declare global {
    interface String {
      truncateAsHtml(length: number): TemplateResult;
      convertUdiToGuid(): string;
    }
  }

String.prototype.truncateAsHtml = function (length: number) :TemplateResult {

    if (!this) {
        return html ``;
      }
      if (this.length <= length) {
        return html `${this}`;
      }
  
      let trimmedText = this.substring(0, length);
  
      trimmedText = trimmedText.substring(0, trimmedText.lastIndexOf(" "));
  
      return  html `${trimmedText} ${unsafeHTML(" &#x2026;")}`;
};


String.prototype.convertUdiToGuid = function () : string {

  if(!this) return this;

     const match = this.match(/umb:\/\/media\/([a-f0-9]{32})/);
     if (!match) {
       //Probably already a guid
       return this.toString();
     }
 
     const rawGuid = match[1];
     return `${rawGuid.slice(0, 8)}-${rawGuid.slice(8, 12)}-${rawGuid.slice(12, 16)}-${rawGuid.slice(16, 20)}-${rawGuid.slice(20)}`;
};