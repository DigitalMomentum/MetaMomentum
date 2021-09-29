var bob = null
angular.module("umbraco")
	.controller("DM.MetaMomentum",
		function ($scope, $filter, editorState, contentEditingHelper, editorService, mediaHelper, entityResource, $interval) {
			$scope.sharePreviewType = null;
			$scope.showEditSearch = false;
			$scope.showEditSocial = false;


			if ($scope.model.config.showSocialPreviewFacebook == 1) {
				$scope.sharePreviewType = "facebook";
			} else if ($scope.model.config.showSocialPreviewtwitter == 1) {
				$scope.sharePreviewType = "twitter";
			} else if ($scope.model.config.showSocialPreviewLinkedIn == 1) {
				$scope.sharePreviewType = "linkedIn";
			}
			if (typeof $scope.model.contentTypeId !== "undefined") {
				//rendering in doctype editor, so return false so as not to break anything
				return;
			}
			var fallbackTitles = $scope.model.config.fallbackTitleFields?.split(',');
			var fallbackDescriptions = $scope.model.config.fallbackDescriptionFields?.split(',');
			var fallbackImages = $scope.model.config.fallbackImageFields?.split(',');
			if ($scope.model.value === "") {
				$scope.model.value = {
					title: "",
					description: "",
					noIndex: false,
					shareTitle: "",
					shareDescription: "",
					shareImage: null,
					shareImageUrl: "",
					default: {
						title: "",
						description: ""
					},
					share: {
						title: "",
						description: "",
						image: null,
						imageUrl: null
					}
				};
			} else {

				if (typeof $scope.model.value.default === 'undefined') {
					//possible upgrade from Seo Meta
					$scope.model.value.default = {
						title: $scope.model.value.title,
						description: $scope.model.value.description
					}
				}

				if (typeof $scope.model.value.share === 'undefined') {
					$scope.model.value.share = {
						title: "",
						description: "",
						image: null,
						imageUrl: null,
					}
				}

				if (typeof $scope.model.value.shareTitle === 'undefined') {
					$scope.model.value.shareTitle = "";
				}
				if (typeof $scope.model.value.shareDescription === 'undefined') {
					$scope.model.value.shareDescription = "";
				}
			}

			function init() {
				$scope.updateSearchTitle();
				$scope.updateSearchDescription();
				$scope.updateShareImage();


				if (typeof editorState.current.urls !== "undefined") {
					$scope.searchHost = window.location.host;
					if (editorState.current.urls[0].text.startsWith("/")) {
						//No domain name specified
						$scope.searchUrl = editorState.current.urls[0].text;


					} else if (editorState.current.urls[0].text.startsWith("http")) {
						var endOfDomain = getPositionInString(editorState.current.urls[0].text, "/", 3);
						$scope.searchHost = editorState.current.urls[0].text.substring(0, endOfDomain);
						$scope.searchUrl = editorState.current.urls[0].text.substring(endOfDomain);
					}

					if ($scope.searchUrl) {
						$scope.searchUrl = $scope.searchUrl.replace(/\/$/g, "").replace(/\//g, " › ");
					}
				}

				//Todo: Need to find a better way of checking if a fallback property has changed. Works for text but not images
				$interval($scope.updateShareImage, 3000);
			}

			function getPositionInString(string, subString, index) {
				return string.split(subString, index).join(subString).length;
			}

			$scope.updateSearchTitle = function () {
				$scope.model.value.title = "";
				if ($scope.model.value.default.title != "") {
					$scope.model.value.title = $scope.model.value.default.title;
				} else {

					var properties = getAllProperties();

					if (fallbackTitles != null) {
						for (var i = 0; i < fallbackTitles.length; i++) {
							for (var p = 0; p < properties.length; p++) {
								if (typeof properties[p] !== "undefined" && properties[p].alias === fallbackTitles[i]) {
									if (typeof properties[p].value !== "undefined" && properties[p].value !== "" && properties[p].value !== null)
										//Found a fallback property value
										$scope.model.value.title = properties[p].value;
								}
							}
						}
					}
				}

				if ($scope.model.value.title == "") {

					for (var i = 0; i < editorState.current.variants.length; i++) {
						if (editorState.current.variants[i].active) {

							//Fall back to page name
							$scope.model.value.title = editorState.current.variants[i].name;
						}
					}
				}
				return $scope.model.value.title;
			}


			$scope.updateSearchDescription = function () {
				$scope.model.value.description = "";
				if ($scope.model.value.default.description != "") {
					$scope.model.value.description = $scope.model.value.default.description;
				} else {

					var properties = getAllProperties();

					if (fallbackDescriptions != null) {
						for (var i = 0; i < fallbackDescriptions.length; i++) {
							for (var p = 0; p < properties.length; p++) {
								if (typeof properties[p] !== "undefined" && properties[p].alias === fallbackDescriptions[i]) {
									if (typeof properties[p].value !== "undefined" && properties[p].value !== "" && properties[p].value !== null)
										//Found a fallback property value
										$scope.model.value.description = properties[p].value;
								}
							}
						}
					}
				}

				return $scope.model.value.description;

			}







			$scope.updateShareTitle = function () {
				$scope.model.value.shareTitle = "";
				if ($scope.model.value.share.title != "") {
					$scope.model.value.shareTitle = $scope.model.value.share.title;
				} else {
					//fallback to default title
					$scope.model.value.shareTitle = $scope.model.value.title;

				}


				return $scope.model.value.shareTitle;
			}


			$scope.updateShareDescription = function () {
				$scope.model.value.shareDescription = "";

				if ($scope.model.value.share.description != "") {
					$scope.model.value.shareDescription = $scope.model.value.share.description;
				} else {
					//fallback to default title
					$scope.model.value.shareDescription = $scope.model.value.description;
				}

				return $scope.model.value.shareDescription;

			}



			$scope.updateShareImage = function () {
				$scope.model.value.shareImage = null;


				//check if there is one already set.
				if ($scope.model.value.share.imageUrl != null) {
					$scope.model.value.shareImageUrl = $scope.model.value.share.imageUrl;


					if ($scope.model.value.share.image != null) {
						$scope.model.value.shareImage = $scope.model.value.share.image.udi
					}

					return $scope.model.value.shareImageUrl;
				}

				//For backwards comatibility - If share.image is set, then we need to update share.ImageUrl property
				if ($scope.model.value.share.image != null) {
					$scope.model.value.shareImage = $scope.model.value.share.image;
					$scope.model.value.share.ImageUrl = $scope.model.value.share.image.image;
					$scope.model.value.shareImageUrl = $scope.model.value.share.image.image;
					return $scope.model.value.shareImageUrl;
				}

				//Noting Set, Time to fall back
				var properties = getAllProperties();

				if (fallbackImages != null) {
					//rootLoop:
					for (var i = 0; i < fallbackImages.length; i++) {
						for (var p = 0; p < properties.length; p++) {
							if (typeof properties[p] !== "undefined" && properties[p].alias === fallbackImages[i]) {
								if (typeof properties[p].value !== "undefined" && properties[p].value !== "" && properties[p].value !== null) {
									//Found a fallback property value
									//console.log("fallback", properties[p])
									$scope.model.value.shareImage = properties[p].value;


									if (properties[p].view == "mediapicker" && $scope.model.value.shareImage != null && $scope.model.value.shareImage.startsWith("umb://")) {
										//The fallback is a media picker
										entityResource.getById($scope.model.value.shareImage, "Media")
											.then(function (mediaEntity) {
												$scope.model.value.shareImageUrl = mediaEntity.metaData.MediaPath;
											});
									} else if (properties[p].view == "mediapicker3" && Array.isArray(properties[p].value) && properties[p].value.length > 0) {
										//The fallback is a media picker
										
										$scope.model.value.shareImage = properties[p].value[0].mediaKey
										entityResource.getById($scope.model.value.shareImage, "Media")
											.then(function (mediaEntity) {
												
												$scope.model.value.shareImageUrl = mediaEntity.metaData.MediaPath;
											});
									} else if (!Array.isArray(properties[p].value) && typeof $scope.model.value.shareImage.src !== "undefined" && $scope.model.value.shareImage.src.startsWith("/")) {
										//Probably an upload field for V9. Could be another random property too, so be careful
										$scope.model.value.shareImageUrl = $scope.model.value.shareImage.src;
										

									} else if (!Array.isArray(properties[p].value) && typeof $scope.model.value.shareImage.src === "undefined" && $scope.model.value.shareImage.startsWith("/")) {
										//Probably an upload field for V8. Could be another random property too, so be careful
										$scope.model.value.shareImageUrl = $scope.model.value.shareImage;
										
									} else {
										
										//Not a property with a valid source image, or upload that has not yet been saved
										$scope.model.value.shareImageUrl = null;
									
									}
									return $scope.model.value.shareImageUrl;
								}
								//break rootLoop;
							}
						}
					}
				}


				//If we got this far, then we havent found an image
				$scope.model.value.shareImageUrl = null;


				return null;
			}




			function getAllProperties() {
				var activeVarients = $filter('filter')(editorState.current.variants, { 'active': true });
				if (typeof activeVarients !== "undefined") {
					return contentEditingHelper.getAllProps($filter('filter')(editorState.current.variants, { 'active': true })[0]);
				}

				return [];

			}


			$scope.truncate = function (value, length) {
				if (typeof value === "undefined" || value == null) {
					return "";
				}
				if (value.length <= length) {
					return value;
				}

				var trimmedText = value.substring(0, length);

				trimmedText = trimmedText.substring(0, trimmedText.lastIndexOf(" "));

				return trimmedText + " &#x2026;"
			}


			$scope.chooseMedia = function () {
				var mediaPickerOptions = {
					multiPicker: false,
					submit: function (imgmodel) {
						editorService.close();
						console.log(imgmodel.selection[0])
						$scope.model.value.share.image = imgmodel.selection[0];
						$scope.model.value.share.imageUrl = imgmodel.selection[0].image;

						$scope.updateShareImage();
					},
					close: function () {
						editorService.close();
					}
				};

				editorService.mediaPicker(mediaPickerOptions);
			};

			$scope.removeMedia = function () {
				$scope.model.value.share.image = null;
				$scope.model.value.share.imageUrl = null;
				$scope.updateShareImage();
			}



			$scope.toggleSearchPreview = function () {
				$scope.showEditSearch = !$scope.showEditSearch;
				document.querySelector('#SearchPreview').scrollIntoView({
					behavior: 'smooth'
				});
			}

			$scope.toggleSocialPreview = function () {
				$scope.showEditSocial = !$scope.showEditSocial;
				document.querySelector('#SocialPreview').scrollIntoView({
					behavior: 'smooth'
				});
			}

			init();
		});