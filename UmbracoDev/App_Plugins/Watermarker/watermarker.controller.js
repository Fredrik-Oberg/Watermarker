angular.module("umbraco").controller("WatermarkerController",
//inject umbracos assetsService
    function($scope, assetsService, imageHelper, dialogService, mediaHelper, umbRequestHelper) {
        
        $scope.sliderOpacity =
        {
            min: 0.01,
            max: 1,
            step: 0.01,
            value: 1,
        };
        $scope.sliderSize =
        {
            min: 1,
            max: 100,
            step: 1,
            value: 50,
            cssClass: "vert",

        };
        $scope.img = $scope.model.value != "" ? parseImgData($scope.model.value) : "";
        
        $scope.watermarkPicker = function () {
   
            dialogService.mediaPicker({
                onlyImages: true,
                callback: function(data) {
                    if (data) {
                        $scope.img = data.image;
                        $scope.sliderSize.max = data.originalWidth;
                        $scope.sliderSize.value = data.originalWidth;
                    }
                }
            });
        };
        function parseImgData(data) {
            
            $scope.sliderOpacity.value = data.opacity;
            $scope.sliderSize.value = data.size;
            $scope.sliderSize.max = data.orgSize;

            return data.value;
        }
        $scope.$on("formSubmitting", function (ev, args) {
            debugger;
            var watermarker = {
                value: $scope.img,
                opacity: $scope.sliderOpacity.value,
                size: $scope.sliderSize.value,
                orgSize: $scope.sliderSize.max
            };
            //var imgValue = $scope.img;
            //var opacity = $scope.sliderOpacity.value;
            //var size = $scope.sliderSize.value;
            //var orgSize = $scope.sliderSize.max;

            $scope.model.value = watermarker;
        });


//assetsService.load([
        //    //Ladda pluginet
        //    //"/App_Plugins/Watermarker/lib/.js"
        //]).then(function () {


//assetsService.loadCss("/App_Plugins/Watermarker/lib/.css");


        //});
    });