angular.module("umbraco").controller("WatermarkerController",
//inject umbracos assetsService
function ($scope, assetsService, dialogService) {
   

    assetsService
        .load([

            //Ladda pluginet
            //"/App_Plugins/Redactor/lib/.js"
        ]).then(function () {





            //assetsService.loadCss("/App_Plugins/redactor/lib/.css");


    });
});
