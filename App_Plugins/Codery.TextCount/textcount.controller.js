angular.module('umbraco').controller('Codery.TextCountController', function ($scope, assetsService) {

    if ($scope.model.value === undefined || $scope.model.value === null) {
        $scope.model.value = '';
    }

    $scope.wrappedProperty = $scope.model.config.wrappedProperty;

    // set some missing values
    $scope.wrappedProperty.id = $scope.model.id;
    $scope.wrappedProperty.alias = $scope.model.alias;
    $scope.wrappedProperty.value = $scope.model.value;

    // hide the label of the wrapped data type
    $scope.wrappedProperty.hideLabel = true;

    $scope.$watch('wrappedProperty.value',
        function (newVal) {
            // update property value with new value of wrapped editor
            $scope.model.value = newVal;
        });

    assetsService.loadCss('/App_Plugins/Codery.TextCount/textcount.css');

});
