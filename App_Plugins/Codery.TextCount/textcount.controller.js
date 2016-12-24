angular.module('umbraco').controller('Codery.TextCountController', function ($scope, contentTypeResource, assetsService) {

    if ($scope.model.value === undefined || $scope.model.value === null) {
        $scope.model.value = '';
    }

    contentTypeResource.getPropertyTypeScaffold($scope.model.config.dataType)
        .then(function (dataType) {
            // set some missing values
            dataType.id = $scope.model.id;
            dataType.alias = $scope.model.alias;
            dataType.value = $scope.model.value;

            // hide the label of the wrapped data type
            dataType.hideLabel = true;

            $scope.wrappedProperty = dataType;

            $scope.$watch('wrappedProperty.value',
                function (newVal) {
                    // update property value with new value of wrapped editor
                    $scope.model.value = newVal;
                });
        });

    assetsService.loadCss('/App_Plugins/Codery.TextCount/textcount.css');

});
