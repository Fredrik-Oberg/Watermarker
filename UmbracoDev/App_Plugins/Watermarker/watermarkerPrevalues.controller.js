angular.module("umbraco").controller("WatermarkerPrevaluesController",
    function ($scope, $http, $timeout) {
        //http://shazwazza.com/post/Uploading-files-and-JSON-data-in-the-same-request-with-Angular-JS
        $scope.files = [];
        if (!$scope.model.value) {
            $scope.model.value = [];
        } else {
            $scope.file = $scope.model.value;
        }
        $scope.$on("filesSelected", function (event, args) {

        $scope.$apply(function () {
            $scope.files.push(args.files[0]);
        });
            //Show a loader 
            $http({
                    method: 'POST',
                    url: "Api/WatermarkUpload/UploadWatermark",

                    headers: { 'Content-Type': false },

                    transformRequest: function(data) {
                        var formData = new FormData();
                        formData.append("model", angular.toJson(data.model));
                        formData.append("file", data.files[0]);
                        return formData;
                    },
                    data: {
                        model: $scope.model,
                        files: $scope.files
                    }
                }).
                success(function(data, status, headers, config) {

                    $scope.model.value = data.filelink;
                    $scope.file = $scope.model.value;
                    $scope.hasError = false;
                    return;
                }).error(function(data, status, headers, config) {
                    $scope.hasError = true;

                });
    }); 
});
