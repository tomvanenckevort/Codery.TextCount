angular.module('umbraco.directives').directive('coderyTextcount', function ($timeout) {

    return {
        restrict: 'A',
        require: 'ngModel',
        scope: {
            countType: '@coderyTextcountCountType',
            limit: '@coderyTextcountLimit',
            limitType: '@coderyTextcountLimitType'
        },
        link: function (scope, elem, attrs, ctrl) {

            // initialise counter settings based on chosen options
            var counterLabel = '';
            var counterLabelPlural = '';
            var errorLabel = '';
            var valueSeparator = '';
            var countFunction = function (v) { return 0; };

            switch (scope.countType) {
                case 'characters':
                    counterLabel = ' char.';
                    counterLabelPlural = ' chars.';
                    errorLabel = 'characters';
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
                    counterLabel = ' word';
                    counterLabelPlural = ' words';
                    errorLabel = 'words';
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

            function getValue($el) {
                // get element value and process it if required
                var val = $el.val();

                var isTinyMCE = $el.prev().hasClass('mce-tinymce');
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

                    if (scope.limit && scope.limitType !== 'none') {
                        if (count > scope.limit) {
                            showError = true;
                        }
                        else if (count > (scope.limit - 5)) {
                            showWarning = true;
                        }

                        if (scope.limitType === 'hard') {
                            // show error message when limit is exceeded
                            ctrl.$setValidity('textcount', !showError);
                            showErrorMsg = showError;
                        }

                        // update counter style
                        counter.toggleClass('codery__text-counter--error', showError);
                        counter.toggleClass('codery__text-counter--warning', showWarning);
                    }
                    
                    counter.html(count + (count === 1 ? counterLabel : counterLabelPlural) + (showErrorMsg ? ' - Too many ' + errorLabel : ''));
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
            scope.$on('$includeContentLoaded', function() {
                // track any text changes
                scope.$watch('model.value', refreshCounters, true);
            });

        }
    };

});
