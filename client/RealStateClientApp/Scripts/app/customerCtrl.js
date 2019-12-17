
(function () {
    'use strict';
    //create angularjs controller
    var app = angular.module('app', []);//set and get the angular module
    app.controller('customerController', ['$scope', '$http', customerController]);

    //angularjs controller method
    function customerController($scope, $http) {

        //declare variable for mainain ajax load and entry or edit mode
        $scope.loading = true;
        $scope.addMode = false;

        //get all customer information
        //$http.get('/api/Customer/').success(function (data) {
        //    $scope.customers = [];
        //    $scope.loading = false;
        //})
        //.error(function () {
        //    $scope.error = "An Error has occured while loading posts!";
        //    $scope.loading = false;
        //});

        $http.get('http://initvent.biz/GoogleAPISvc/AdministrationService.svc/UpdateSalesObjects').then(function (data) {
            if (data.data.length > 0) {
                for (var i = 0; i < data.data.length; i++) {
                    var id = data.data[i].id;                   
                    var url1 = 'http://maps.googleapis.com/maps/api/geocode/json?latlng=' + data.data[i].lattitude + ',' + data.data[i].longitude + '&sensor=true';
                    $http.get(url1, {
                    }).then(function (data1) {
                        var placeID = data1.data.results[0].place_id;
                        var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/updateSalesObj?id=' + id + '&placeId=' + placeID;
                        $http.get(url, {
                        }).then(function (data) {
                            $scope.SalesObjectfind();
                        }, function () {
                            alert('Error get marker'); //shows error if connection lost or error occurs
                        });

                    }, function () {
                        alert('Error get marker'); //shows error if connection lost or error occurs
                    });

                   
                }
            }
            else {
                $scope.mapObjData();
            }

        }, function () {
            alert('Error get Location');
        });



        $scope.SalesObjectfind = function () {
            $http.get('http://initvent.biz/GoogleAPISvc/AdministrationService.svc/UpdateSalesObjects').then(function (data) {
                if (data.data.length > 0) {
                    for (var i = 0; i < data.data.length; i++) {
                        var id = data.data[i].id;
                        var url1 = 'http://maps.googleapis.com/maps/api/geocode/json?latlng=' + data.data[i].lattitude + ',' + data.data[i].longitude + '&sensor=true';
                        $http.get(url1, {
                        }).then(function (data1) {
                            var placeID = data1.data.results[0].place_id;
                            var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/updateSalesObj?id=' + id + '&placeId=' + placeID;
                            $http.get(url, {
                            }).then(function (data) {
                                $scope.SalesObjectfind();
                            }, function () {
                                alert('Error get marker'); //shows error if connection lost or error occurs
                            });

                        }, function () {
                            alert('Error get marker'); //shows error if connection lost or error occurs
                        });
                    }
                }
                else {
                    $scope.mapObjData();
                }

            }, function () {
                alert('Error get Location');
            });

        }

        $scope.mapObjData = function () {
            $http.get('http://initvent.biz/GoogleAPISvc/AdministrationService.svc/MapObjects').then(function (data) {
                $scope.sources = data.data;
                $scope.loading = false;
            }, function () {
                alert('Error get Location');
            });
        }


        $scope.PopulateSearchData = function (salesObject, MissList) {
            $scope.customers = salesObject;
            $scope.missList = MissList;
        }

        $scope.searchSalesObject = function () {
            var Source_id = $("#salesobject").val();
            var TravelType = $("#travelType").val();
            var Distance = $("#Distance").val();
            var objType = $("#objectType").val();
            var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/searchSalesObjComparedMapObject?Source_id=' + Source_id + '&TravelType=' + TravelType + '&Distance=' + Distance + '&objType=' + objType
                $http.get(url, {
                }).then(function (data) {
                    $scope.customers = data.data;
                    //$scope.latlng = [data.data.Lat, data.data.Long];
                }, function () {
                    alert('Error get marker'); //shows error if connection lost or error occurs
                });
            
        }

        //by pressing toggleEdit button ng-click in html, this method will be hit
        $scope.toggleEdit = function () {
            getSearchedDestinition();
            this.customer.editMode = !this.customer.editMode;
        };

        //by pressing toggleAdd button ng-click in html, this method will be hit
        $scope.toggleAdd = function () {
            $scope.addMode = !$scope.addMode;
        };

        //Inser Customer
        $scope.add = function () {
            $scope.loading = true;
            $http.post('/api/Customer/', this.newcustomer).success(function (data) {
                alert("Added Successfully!!");
                $scope.addMode = false;
                $scope.customers.push(data);
                $scope.loading = false;
            }).error(function (data) {
                $scope.error = "An Error has occured while Adding Customer! " + data;
                $scope.loading = false;
            });
        };

        //Edit Customer
        $scope.save = function () {
            alert("Edit");
            $scope.loading = true;
            var frien = this.customer;
            alert(frien);
            $http.put('/api/Customer/' + frien.Id, frien).success(function (data) {
                alert("Saved Successfully!!");
                frien.editMode = false;
                $scope.loading = false;
            }).error(function (data) {
                $scope.error = "An Error has occured while Saving customer! " + data;
                $scope.loading = false;
            });
        };

        //Delete Customer
        $scope.deletecustomer = function () {
            $scope.loading = true;
            var Id = this.customer.Id;
            $http.delete('/api/Customer/' + Id).success(function (data) {
                alert("Deleted Successfully!!");
                $.each($scope.customers, function (i) {
                    if ($scope.customers[i].Id === Id) {
                        $scope.customers.splice(i, 1);
                        return false;
                    }
                });
                $scope.loading = false;
            }).error(function (data) {
                $scope.error = "An Error has occured while Saving Customer! " + data;
                $scope.loading = false;
            });
        };
    }
})();