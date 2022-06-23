/**
 * @description
 * removes html tags from HTML string to produce only text. Additional "stripLineBreaks" parameter will remove all line breaks as well
 */

(function () {
	'use strict';

	function momentumStripHtml() {
		return function (htmlString, stripLineBreaks) {

			if (!Utilities.isString(htmlString)) {
				return "";
			}

			if (Utilities.isUndefined(htmlString)) {
				return "";
			}


			var retVal = String(htmlString).replace(/<[^>]+>/gm, '');
			if (stripLineBreaks) {
				retVal = retVal.replace(/\r?\n|\r/g, ' '); //replace line break with a space
			}

			return retVal;
		};
	}

	angular.module('umbraco.filters').filter('momentumStripHtml', momentumStripHtml);

})();