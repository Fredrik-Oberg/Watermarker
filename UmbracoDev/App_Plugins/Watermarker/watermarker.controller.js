angular.module("umbraco").controller("WatermarkerController",
//inject umbracos assetsService
    function($scope, assetsService, dialogService) {
        debugger;
        $scope.slider =
        {
            min: 0,
            max: 10,
            step: 0.1
        };
        $scope.sliderVert =
        {
            min: 0,
            max: 10,
            step: 0.1,
            cssClass: "vert",

        };
        assetsService.load([
                
                
             
                
            //Ladda pluginet
            //"/App_Plugins/Redactor/lib/.js"
        ]).then(function () {





            //assetsService.loadCss("/App_Plugins/redactor/lib/.css");


    });
});
