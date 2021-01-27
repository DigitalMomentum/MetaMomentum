angular.module("umbraco")
    .controller("DM.MetaMomentum",
        function ($scope, $filter, editorState, contentEditingHelper) {

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
                    default: {
                        title: "",
                        description: ""
                    }
                };
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

            function getAllProperties() {
                var activeVarients = $filter('filter')(editorState.current.variants, { 'active': true });
                if (typeof activeVarients !== "undefined") {
                    return contentEditingHelper.getAllProps($filter('filter')(editorState.current.variants, { 'active': true })[0]);
                }

                return [];

            }


            $scope.truncate = function (value, length) {
                if (typeof value === "undefined") {
                    return "";
                }
                if (value.length <= length) {
                    return value;
                }

                return value.substring(0, length) + "&#x2026;"
            }

            //$scope.getSearchUrl = function () {

            //}

            init();
        });