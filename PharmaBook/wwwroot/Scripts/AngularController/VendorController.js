var appA = angular.module("ProductModule", []);
var app = angular.module("MyModuleA", []);

app.factory('loadvndor', ['$http', '$rootScope', function ($http, $rootScope) {
    $rootScope.UserName = '';
    var fac = {};
    fac.getvndr = function () {
        $http.get('/Vendor/GetAllVendor').then(function (res) {
            $rootScope.VendorList = res.data;
        }, function (error) {
        }
        )
    }
    fac.getprdct = function () {
        $http.get('/Product/GetAllMedicine').then(function (res) {
            $rootScope.ProductList = res.data;
        }, function (error) {
        }
        )
    }
    return fac;
}])
app.controller('MyController', function ($scope, $http,loadvndor) {
    loadvndor.getvndr();
    $scope.divhide1 = false;
    $scope.divhide2 = true;
    $scope.savebtn = false;
    $scope.editbtn = true;
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
        var obj = {
            'vendorName': $scope.Item.Name,
            'vendorAddress': $scope.Item.Address,
            'vendorMobile': $scope.Item.Mobile,
            'vendorCompnay': $scope.Item.Company,
            'cusUserName': $scope.Item.UserName
        };
        $http({
            method: 'post',
            url: "/Vendor/VendorCreate",
            data: JSON.stringify(obj),
            dataType: "json"
           // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {       
            loadvndor.getvndr();
        }, function (error) {
            
        })        
    }
    var counter = 0;
    $scope.showdata = function (item, inc) {
        $scope.savebtn = true;
        $scope.editbtn = false;
        $scope.cClass = item.id;        
    }
    $scope.Updtdata = function (item) {
        var obj = { 'Id': item.id, 'vendorName': item.vendorName, 'vendorAddress': item.vendorAddress, 'vendorMobile': item.vendorMobile, 'vendorCompnay': item.vendorCompnay, 'cusUserName': item.cusUserName };
        $http({
            method: 'post',
            url: "/Vendor/UpdtVendor",
            data: obj,
            datatype: "json",
            // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            loadvndor.getvndr();
        }, function (error) {

            })
        $scope.cClass = false;
        $scope.savebtn = false;
        $scope.editbtn = true;
    }
    $scope.delitem = function (id) {
        if (confirm("Do you want to continue?")) {
            $http.post('/Vendor/VendorDlt/?id=' + id).then(
                function (res) {
                    loadvndor.getvndr();
                },
                function (err) {

                });
        }
    }
    $scope.bcktoinbx = function (type) {
        if (type == "create") {
            $scope.divhide1 = false;
            $scope.divhide2 = true;
        }
        if (type == "inbox") {
            $scope.divhide1 = true;
            $scope.divhide2 = false;
        }
    }
})
app.controller('ProductController', function ($scope, $http, $location, loadvndor, $window) {
    debugger 
    loadvndor.getvndr();
    loadvndor.getprdct();   
    $scope.divhide1 = false;
    $scope.divhide2 = true;
    $scope.savebtn = false;
    $scope.editbtn = true;
    $scope.MediProdct = {
        MedicineName: '',
        batchNo: '',
        openingStock: '',
        ExpDt: '',
        CompanyName: '',
        MRP: '',
        vendorID: ''
    }
    $scope.AddMedcin = function () {
        debugger
        //$scope.Item.Date = $filter('date')($scope.Item.Date, "yyyy-MM-dd");
        var obj = {
            'name': $scope.MediProdct.MedicineName,
            'batchNo': $scope.MediProdct.batchNo,
            'openingStock': $scope.MediProdct.openingStock,
            'expDate': $scope.MediProdct.ExpDt,
            'companyName': $scope.MediProdct.CompanyName,
            'MRP': $scope.MediProdct.MRP,
            'vendorID': $scope.MediProdct.vendorID
        };
        $http({
            method: 'post',
            url: "/Product/Create",
            data: JSON.stringify(obj),
            dataType: "json"
            // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            $window.location.href = "http://localhost:49440/Product/Index";
        }, function (error) {

        })
    }
    var counter = 0;
    $scope.showdata = function (item, inc) {
        $scope.savebtn = true;
        $scope.cClass = item.id;
        $scope.editbtn = false;      
    }
    $scope.Updtdata = function (item) {       
        var obj = {
            'Id': item.id,
            'name': item.name,
            'batchNo': item.batchNo,            
            'expDate': item.expDate,
            'companyName': item.companyName,
            'MRP': item.mrp,
            'openingStock': item.openingStock,
            'vendorID': item.vendorID
        };
        $http({
            method: 'post',
            url: "/Product/UpdateMedicn",
            data: obj,
            datatype: "json",            
        }).then(function (response) {
            loadvndor.getprdct();
        }, function (error) {

            })       
        $scope.cClass = false;
        $scope.savebtn = false;
        $scope.editbtn = true;
    }
    $scope.delitem = function (id) {
        if (confirm("Do you want to continue?")) {
            $http.post('/Product/DeleteMedicine/?id=' + id).then(
                function (res) {
                    loadvndor.getprdct();
                },
                function (err) {

                });
        }
    }
    $scope.bcktoinbx = function (type)
    {
        if (type == "create") {
            $scope.divhide1 = false;
            $scope.divhide2 = true;
        }
        if (type == "inbox") {
            $scope.divhide1 = true;
            $scope.divhide2 = false;
        }
    }
})