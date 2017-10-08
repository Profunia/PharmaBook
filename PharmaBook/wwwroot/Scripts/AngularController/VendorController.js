﻿var app = angular.module("MyModuleA", []);
app.constant('ServiceBaseURL', 'http://localhost:64321')
app.controller('MyController', function ($scope, $http, ServiceBaseURL) {
    getvendor()
    $scope.searchbox = '';
    $scope.Item = {
        Name: '',
        Address: '',
        Mobile: '',
        Company: '',
        UserName:''
    }
    debugger
    $scope.AddValues = function () {
        debugger
        //$scope.Item.Date = $filter('date')($scope.Item.Date, "yyyy-MM-dd");
        var obj = { 'vedorName': $scope.Item.Name, 'vedorAddress': $scope.Item.Address, 'vedorMobile': $scope.Item.Mobile, 'vedorCompnay': $scope.Item.Company, 'cusUserName': $scope.Item.UserName};
        $http({
            method: 'post',
            url: "/Vendor/VendorCreate",
            data: JSON.stringify(obj),
            dataType: "json"
           // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {       
            getvendor();
        }, function (error) {
            
        })        
    }
    function getvendor() {
        $http.get('/Vendor/GetAllVendor').then(function (res) {
            $scope.VendorList = res.data;
        }, function (error) {

        })         
    }
    $scope.showdata = function (item) {
        $scope.cClass = item.id;
    }
    $scope.Updtdata = function (item) {
        var obj = { 'Id': item.id, 'vedorName': item.vedorName, 'vedorAddress': item.vedorAddress, 'vedorMobile': item.vedorMobile, 'vedorCompnay': item.vedorCompnay, 'cusUserName': item.cusUserName };
        $http({
            method: 'post',
            url: "/Vendor/UpdtVendor",
            data: obj,
            datatype: "json",
            // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            getvendor();
        }, function (error) {

        })
    }
    $scope.delitem = function (id) {
        $http.post('/Vendor/VendorDlt/?id=' + id).then(
            function (res) {
                getvendor();
            },
            function (err) {

            });
    }
})