angular.module("umbraco")
    .controller("DM.MetaMomentum",
        function ($scope, $filter, editorState, contentEditingHelper, editorService) {
			$scope.sharePreviewType = null;
			$scope.showSearchPreview = false;
			$scope.showSocialPreview = false;


            if ($scope.model.config.showSocialPreviewFacebook == 1) {
                $scope.sharePreviewType = "facebook";
            } else if ($scope.model.config.showSocialPreviewtwitter == 1) {
                $scope.sharePreviewType = "twitter";
            } else if ($scope.model.config.showSocialPreviewLinkedIn== 1) {
                $scope.sharePreviewType = "linkedIn";
            }
         //   $scope.showShare = false;
            if (typeof $scope.model.contentTypeId !== "undefined") {
                //rendering in doctype editor, so return false so as not to break anything
                return;
            }
            var fallbackTitles = $scope.model.config.fallbackTitleFields?.split(',');
            var fallbackDescriptions = $scope.model.config.fallbackDescriptionFields?.split(',');
            if ($scope.model.value === "") {
                $scope.model.value = {
                    title: "",
                    description: "",
                    shareTitle: "",
                    shareDescription: "",
                    shareImage: null,
                    default: {
                        title: "",
                        description: ""
                    },
                    share: {
                        title: "",
                        description: "",
                        image: null
                    }
                };
            } else {
                if (typeof $scope.model.value.share === 'undefined') {
                    $scope.model.value.share = {
                        title: "",
                        description: "",
                        image: null
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


                if (typeof editorState.current.urls !== "undefined") {
                    $scope.searchHost = window.location.host;
                    $scope.searchUrl = editorState.current.urls[0].text;

                    $scope.searchUrl = $scope.searchUrl.replace(/\/$/g, "").replace(/\//g, " › ")

                    //for (var i = 0; i < editorState.current.urls.length; i++) {
                    //    console.log(editorState.current.urls[i].text)
                    //    if (editorState.current.urls[i].text.substring(0, 4) === "http") {
                    //        $scope.searchUrl = editorState.current.urls[i].text;
                    //    }
                    //}
                }
            }

          

           // $rootscope.watch("")


            // console.log($scope.model.config);
            
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

            $scope.mediaItems = [];
            $scope.chooseMedia = function () {
                var mediaPickerOptions = {
                    multiPicker: false,
                    submit: function (imgmodel) {
                        editorService.close();
                        console.log(JSON.stringify(imgmodel)); // Do something with selected image data
                        $scope.model.value.share.image = imgmodel.selection[0];
                        $scope.model.value.shareImage = imgmodel.selection[0].udi;
                        $scope.mediaItems.push(imgmodel.selection[0])
                    },
                    close: function () {
                        editorService.close();
                    }
                };

                editorService.mediaPicker(mediaPickerOptions);
            };

            $scope.removeMedia = function () {
                $scope.model.value.share.image = null;
                $scope.model.value.shareImage = null;
			}



			$scope.toggleSearchPreview = function () {
				$scope.showSearchPreview = !$scope.showSearchPreview ;
				document.querySelector('#SearchPreview').scrollIntoView({
					behavior: 'smooth'
				});
			}

			$scope.toggleSocialPreview = function () {
				$scope.showSocialPreview = !$scope.showSocialPreview;
				document.querySelector('#SocialPreview').scrollIntoView({
					behavior: 'smooth'
				});
			}

            //$scope.showSharefields = function(){
            //    $scope.showShare = !$scope.showShare;
            //}

            init();
        });