angular.module('umbraco').controller('Codery.TextCountController', function ($scope, $element, contentTypeResource, assetsService, localizationService, $timeout, angularHelper) {
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

            $scope.$watch('wrappedProperty.value', function (newVal) {
                // update property value with new value of wrapped editor
				$scope.model.value = newVal;
				init();
			});

			init();
		});

	function init() {
		var counterLabel = '';
		var counterLabelPlural = '';
		var errorLabel = '';
		var valueSeparator = '';
		var countFunction = function (v) { return 0; };
		var errorTooMany = '';
		console.log($scope.model)
		switch ($scope.model.config.countType) {
			case 'characters':
				localizationService.localize('textcount_labelCharacter').then(function (value) {
					counterLabel = ' ' + value;
				});

				localizationService.localize('textcount_labelCharacters').then(function (value) {
					counterLabelPlural = ' ' + value;
				});

				localizationService.localize('textcount_errorCharacters').then(function (value) {
					errorLabel = value;
				});

				valueSeparator = '';
				countFunction = function (v) {
					if (v !== undefined) {
						return v.length;
					} else {
						return 0;
					}
				};
				break;
			case 'words':
				localizationService.localize('textcount_labelWord').then(function (value) {
					counterLabel = ' ' + value;
				});

				localizationService.localize('textcount_labelWords').then(function (value) {
					counterLabelPlural = ' ' + value;
				});

				localizationService.localize('textcount_errorWords').then(function (value) {
					errorLabel = value;
				});

				valueSeparator = ' ';
				countFunction = function (v) {
					if (v !== undefined) {
						return v.trim().split(/\s+/).filter(function (a) { return a && a.length > 0; }).length;
					} else {
						return 0;
					}
				};
				break;
		}

		localizationService.localize('textcount_errorTooMany').then(function (value) {
				errorTooMany = value;
			});

			function getValue($el) {
				// get element value and process it if required
				var val = $el.val();

				var isTinyMCE = $el.parent().hasClass('umb-rte');
				var isMarkdown = $el.hasClass('wmd-input');

				var html = null;

				if (isTinyMCE) {
					// get HTML value
					html = $(val);
				}

				if (isMarkdown && typeof Markdown !== 'undefined') {
					// get parsed Markdown value as HTML
					var converter = new Markdown.Converter();

					html = $(converter.makeHtml(val));
				}

				if (html !== null) {
					// strip value from HTML tags for counting (ignoring any empty elements or line breaks)
					val = html
							.map(function (h) {
								var txt = $.trim(this.innerText);

								if (txt !== '') {
									return txt;
								} else {
									return null;
								}
							})
							.toArray()
							.join(valueSeparator);
				}

				// return sanitised value
				return val;
			};

			var inputs = [];

			function initCounters() {
				var elem = $element;
				var isMultipleTextbox = (elem.find('.umb-multiple-textbox').length > 0);

				// find any non-initialised inputs
				var newInputs = elem.find('input[type="text"], textarea').not('.codery__text-input').toArray();

				newInputs.forEach(function (el) {
					// append counter
					var counter = $('<div class="codery__text-counter"></div>');

					var $el = $(el);

					if (isMultipleTextbox) {
						// append after the remove button
						$el.next('a').after(counter);
					} else {
						// append after the input
						$el.after(counter);
					}

					// set class to mark it as initialised
					$el.addClass('codery__text-input');
				});

				inputs = inputs.concat(newInputs);
			};

			function updateCounters() {
				// update each input counter based on current value
				inputs.forEach(function (el) {
					var $el = $(el);
					var val = getValue($el);
					var count = countFunction(val);

					var counter = $el.nextAll('.codery__text-counter:first');

					var showError = false;
					var showErrorMsg = false;
					var showWarning = false;

					if ($scope.model.config.limit && $scope.model.config.limitType !== 'none') {
						if (count > $scope.model.config.limit) {
							showError = true;
						}
						else if (count > ($scope.model.config.limit - 5)) {
							showWarning = true;
						}
						
						if ($scope.model.config.limitType === 'hard') {
							console.log(showError, $scope.model.config.limitType);
							// show error message when limit is exceeded
							var tmp = angularHelper.getCurrentForm($scope).$setValidity('textcount', !showError);
							console.log("TMP", tmp);
							showErrorMsg = showError;
						}

						// update counter style
						counter.toggleClass('codery__text-counter--error', showError);
						counter.toggleClass('codery__text-counter--warning', showWarning);
					}

					counter.html(count + (count === 1 ? counterLabel : counterLabelPlural) + (showErrorMsg ? ' - ' + errorTooMany + ' ' + errorLabel : ''));
				});
			};

			function refreshCounters() {
				// refreshes the counters in the next digest
				$timeout(function () {
					initCounters();
					updateCounters();
				});
			};

			// initialize when editor content has been loaded
			$scope.$on('$includeContentLoaded', function() {
				// track any text changes
				$scope.$watch('model.value', refreshCounters, true);
			});
	}

	assetsService.loadCss('/App_Plugins/Codery.TextCount/textcount.css');

});
