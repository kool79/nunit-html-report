window.NUnitTestResult = require('../TestResult.json');

var angular = require('angular');

var application = angular.module('NUnitReport', [
    require('angular-route'),
    require('angular-animate'),
    'ui.bootstrap',
    'angular.backtop'
]);

application.factory('Report', require('./services/ReportFactory'));
application.service('ReportAdapter', require('./services/ReportAdapter'));
application.controller('DashboardController', require('./controllers/DashboardController'));
application.controller('TestcaseController', require('./controllers/TestcaseController'));

application.config(['$routeProvider',
    function($routeProvider) {
        $routeProvider.when('/dashboard', {
            templateUrl: 'views/DashboardView.html',
            controller: 'DashboardController'
        }).when('/testcase/:id', {
            templateUrl: 'views/TestCaseView.html',
            controller: 'TestcaseController'
        }).otherwise({
            redirectTo: '/dashboard'
        });
    }
]);

require('angular-ui-bootstrap/ui-bootstrap-tpls.min.js');
require('angular-backtop');