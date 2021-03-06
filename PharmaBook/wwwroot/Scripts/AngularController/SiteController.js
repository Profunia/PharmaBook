﻿// var appA = angular.module("ProductModule", []);
var app = angular.module("MyModuleA", []);

function setMedicines(val) {
    sessionStorage.setItem("medicines", JSON.stringify(val));
};
function getMedicines() {
    return sessionStorage.getItem("medicines");
};
function setVendors(val) {
    sessionStorage.setItem("vendors", JSON.stringify(val));
};
function getVendors() {
    return sessionStorage.getItem("vendors");
};
function setSessionUser(val) {
    sessionStorage.setItem("user", JSON.stringify(val));
};
function getSessionUser() {
    return sessionStorage.getItem("user");
};
app.factory('loadvndor', ['$http', '$rootScope', function ($http, $rootScope) {
    $rootScope.UserName = '';
    var fac = {};
    fac.getvndr = function () {
        $rootScope.isLoadingScreenActive = true;

        var vendorSession = getVendors();
        if (vendorSession) {
            console.log('--vendor from session--');
            $rootScope.VendorList = JSON.parse(vendorSession);
            $rootScope.isLoadingScreenActive = false;
        }
        else {
            console.log('--vendor from API--');
            $http.get('/Vendor/GetAllVendor').then(function (res) {
                $rootScope.VendorList = res.data;
                setVendors(res.data);
                $rootScope.isLoadingScreenActive = false;
            }, function (error) {
                $rootScope.isLoadingScreenActive = false;
            }
            )
        }

    }
    fac.getprdct = function () {
        $rootScope.isLoadingScreenActive = true;
        var sessionMedi = getMedicines();
        if (sessionMedi) {
            console.log('--medicine getting from session');
            $rootScope.ProductList = JSON.parse(sessionMedi);
            $rootScope.isLoadingScreenActive = false;
        }
        else {
            $http.get('/Product/GetAllMedicine').then(function (res) {
                setMedicines(res.data);
                $rootScope.ProductList = res.data;
                console.log('--medicine getting from API');
                $rootScope.isLoadingScreenActive = false;
            }, function (error) {
                $rootScope.isLoadingScreenActive = false;
            }
            )
        }

    }

    fac.getVendorbyID = function (id) {
        $rootScope.isLoadingScreenActive = true;
        $http.get('/Vendor/GetVendorByID/' + id).then(function (res) {
            $rootScope.selectedVendor = res.data;
            console.log($rootScope.selectedVendor);
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;
        }
        )
    };

    fac.currentUser = function () {
        $rootScope.isLoadingScreenActive = true;
        var user = getSessionUser();
        if (user) {
            $rootScope.currentUser = JSON.parse(user);
            console.log("--user info from sesssion--");
            $rootScope.isLoadingScreenActive = false;
        }
        else {
            $http.get('/Home/CurUser/').then(function (res) {
                $rootScope.currentUser = res.data;
                setSessionUser(res.data);
                console.log("--user info from API--");
            }, function (error) {
            })
            $rootScope.isLoadingScreenActive = false;
        }
    }

        return fac;
    
}]);
app.controller('MyController', function ($scope, $http, loadvndor, $rootScope) {
    loadvndor.getvndr();
    $scope.divhide1 = false;
    $scope.divhide2 = true;
    $scope.editbtn = true;
    $scope.savebtn = false;
    $scope.isPreview = false;
    $scope.Item = {

        Name: '',
        Address: '',
        Mobile: '',
        Company: '',
    }

    $scope.AddValues = function () {
        if (!$scope.Item.Name || !$scope.Item.Mobile || !$scope.Item.Address) {
            $scope.errMsg = "Please provide required fields";
            return false;
        }
        $rootScope.isLoadingScreenActive = true
        var obj = {
            'vendorName': $scope.Item.Name,
            'vendorAddress': $scope.Item.Address,
            'vendorMobile': $scope.Item.Mobile,
            'vendorCompnay': $scope.Item.Company
        };

        $http({
            method: 'post',
            url: "/Vendor/VendorCreate",
            data: JSON.stringify(obj),
            dataType: "json"
            // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            sessionStorage.clear();
            loadvndor.getvndr();
            $scope.isPreview = true;
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $scope.isPreview = false;
            $rootScope.isLoadingScreenActive = false;
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
        $rootScope.isLoadingScreenActive = true;
        $http({
            method: 'post',
            url: "/Vendor/UpdtVendor",
            data: obj,
            datatype: "json",
            // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
        }).then(function (response) {
            sessionStorage.clear();
            loadvndor.getvndr();
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;
        })
        $scope.cClass = false;
        $scope.savebtn = false;
        $scope.editbtn = true;
    }
    $scope.delitem = function (id) {
        if (confirm("Do you want to continue?")) {
            $rootScope.isLoadingScreenActive = true;
            $http.post('/Vendor/VendorDlt/?id=' + id).then(
                function (res) {
                    sessionStorage.clear();
                    loadvndor.getvndr();
                    $rootScope.isLoadingScreenActive = false;
                },
                function (err) {
                    $rootScope.isLoadingScreenActive = false;
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
            $scope.isPreview = false;
            $scope.Item = {

                Name: '',
                Address: '',
                Mobile: '',
                Company: '',
            }
        }
    }
    $scope.purchasedhistry = [];
    $http.get('/Product/PurchsdHstryInbx/').then(function (res) {
        $scope.purchasedhistry = res.data;
        for (var i = 0; i < $rootScope.VendorList.length; i++) {
            var vendorid = $rootScope.VendorList[i].id;
            for (var ln = 0; ln < $scope.purchasedhistry.length; ln++) {
                if ($scope.purchasedhistry[ln].vendorID == vendorid) {
                    $scope.purchasedhistry[ln].vendorname = $rootScope.VendorList[i].vendorName;
                    $scope.purchasedhistry[ln].vendorcompany = $rootScope.VendorList[i].vendorCompnay;
                    $scope.purchasedhistry[ln].vendoradres = $rootScope.VendorList[i].vendorAddress;
                }
            }
        }
    }, function (error) {
    })
})
app.controller('ProductController', function ($scope, $http, $location, $rootScope, loadvndor, $window) {
    $scope.vendorDDL = '';
  //  setuser();
    $scope.getSelectedVendor = function () {
        if ($scope.vendorDDL != '') {
            $scope.searchbox = $scope.vendorDDL;
        }
        else {
            $scope.searchbox = '';
        }
        $scope.medicineName = 'name';
    }
    $scope.medicineName = 'name';
    $scope.ItemNameMedicine = '';
    $scope.medicineselect = function (item) {
        if (item != '') {
            $scope.searchbox = item.name;
            $scope.medicineName = item.name;
        }
        else {
            $scope.searchbox = '';
            $scope.medicineName = 'name';
        }
        $scope.vendorDDL = '';
    }


    $scope.isStefActive = false;
    $scope.stef = '';
    $scope.tablets = '';
    $scope.StefPrice = '';

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
        vendorID: '',
        Remarks: ''
    }
    //$scope.purchasedhistry = {
    //    name: '',
    //    batchno: '',
    //    expdate: '',
    //    mfg: '',
    //    mrp: '',
    //    qty: '',
    //    purchaseddated: '',
    //    vendrname:''
    //}
    $scope.AddMedcin = function () {

        if ($scope.MediProdct.MedicineName && $scope.MediProdct.batchNo
            && $scope.tablets && $scope.MediProdct.CompanyName
            && $scope.MediProdct.ExpDt && $scope.stef && $scope.StefPrice) {
            $rootScope.isLoadingScreenActive = true;
            var obj = {
                'name': $scope.MediProdct.MedicineName,
                'batchNo': $scope.MediProdct.batchNo,
                'expDate': $scope.MediProdct.ExpDt,
                'companyName': $scope.MediProdct.CompanyName,
                'vendorID': $scope.MediProdct.vendorID,
                'Remarks': $scope.MediProdct.Remarks,
                'stef': $scope.stef,
                'tabletsCapsule': $scope.tablets,
                'eachStefPrice': $scope.StefPrice
            };
            $http({
                method: 'post',
                url: "/Product/Create",
                data: JSON.stringify(obj),
                dataType: "json"
                // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function (response) {
                sessionStorage.clear();
                $window.location.href = "/Product/Create";
                $rootScope.isLoadingScreenActive = false;
            }, function (error) {
                $scope.errMsg = error.data;
                $rootScope.isLoadingScreenActive = false;
            })
        }
        else {
            $scope.rqtErr = "please provide required fields";
        }
    }
    var counter = 0;
    $scope.showdata = function (item, inc) {
        $scope.savebtn = true;
        $scope.cClass = item.id;
        $scope.editbtn = false;
    }
    $scope.Updtdata = function (item) {
        $rootScope.isLoadingScreenActive = true;
        var obj = {
            'Id': item.id,
            'name': item.name,
            'batchNo': item.batchNo,
            'expDate': item.expDate,
            'companyName': item.companyName,
            'stef': item.stef,
            'tabletsCapsule': item.tabletsCapsule,
            'eachStefPrice': item.eachStefPrice,
            'vendorID': item.vendorID,
            'cusUserName': item.cusUserName
        };

        $http({
            method: 'post',
            url: "/Product/UpdateMedicn",
            data: obj,
            datatype: "json",
        }).then(function (response) {
            sessionStorage.clear();
            loadvndor.getprdct();
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;

        })
        $scope.cClass = false;
        $scope.savebtn = false;
        $scope.editbtn = true;
    }
    $scope.delitem = function (id) {

        if (confirm("Do you want to continue?")) {
            $rootScope.isLoadingScreenActive = true;
            $http.post('/Product/DeleteMedicine/?id=' + id).then(
                function (res) {
                    sessionStorage.clear();
                    loadvndor.getprdct();
                    $rootScope.isLoadingScreenActive = false;
                },
                function (err) {
                    $rootScope.isLoadingScreenActive = false;
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
    $scope.bulkupldlink = function (value) {
        if (value == "successlst") {
            $scope.successlst = true;
            $scope.duplictlst = false;
            $scope.producterrlst = false;
        }
        if (value == "duplictlst") {
            $scope.duplictlst = true;
            $scope.successlst = false;
            $scope.producterrlst = false;
        }
        if (value == "producterrlst") {
            $scope.producterrlst = true;
            $scope.duplictlst = false;
            $scope.successlst = false;
        }
    }
    $scope.purchasedhistry = [];
    $http.get('/Product/PurchsdHstryInbx/').then(function (res) {
        $scope.purchasedhistry = res.data;
        //for (var i = 0; i < $rootScope.VendorList.length; i++)
        //{
        //    var vendorid = $rootScope.VendorList[i].id;
        //    for (var ln=0; ln < $scope.purchasedhistry.length; ln++) {
        //        if ($scope.purchasedhistry[ln].vendorID == vendorid) {
        //            $scope.purchasedhistry[ln].vendorname = $rootScope.VendorList[i].vendorName;
        //            $scope.purchasedhistry[ln].vendorcompany = $rootScope.VendorList[i].vendorCompnay;
        //            $scope.purchasedhistry[ln].vendoradres = $rootScope.VendorList[i].vendorAddress;
        //        }
        //    }
        //    //$scope.purchasedhistry.name.push(value);
        //}
        //$scope.purchasedhistry = res.data;        

    }, function (error) {
    })
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    function setuser() {
        
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser) {
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }
    //-----------------------------------XX----------------------------------
})

app.controller('SalesController', function ($scope, $http, $rootScope, loadvndor) {
    var total = 0;
    function initialSetup() {
        $scope.medicineName = 'Name';
        $scope.isPreview = false;
        final = 0;
        total = 0;
        $scope.unitprice = '';
        $scope.totalprice = '';
        $scope.grandtotalprice = '';
        $scope.discountAmt = '';
        $scope.cartlists = [];
        loadvndor.getprdct()
        $scope.PrdId = '',
            $scope.Qty = '';
        $scope.master = {
            Id: '',
            InvId: '',
            PatientName: '',
            PatientAdres: '',
            DrName: '',
            RegNo: '',
            Discount: ''
        }
        $scope.child = {
            Id: '',
            Name: '',
            Description: '',
            Mrg: '',
            BatchNo: '',
            ExpDt: '',
            Amount: '',
            Qty: ''
        }
    }
    initialSetup();
    $scope.isCreatedInvoice = false;
    $scope.switchToInvoicePage = function () {
        initialSetup();
        $scope.isCreatedInvoice = false;
    }

    $scope.AddChildInvc = function () {
        $scope.ErrMsg = '';
        if ($scope.child.Qty && $scope.child.Description) {
            var price = $scope.child.Amount;
            var qty = $scope.child.Qty;
            var total = (qty * price);
            final += total;
            $scope.unitprice = total;
            $scope.totalprice = final;
            $scope.cartlists.push({
                'Mfg': $scope.child.Mfg,
                'PrdId': $scope.PrdId,
                'Qty': $scope.child.Qty,
                'ExpDt': $scope.child.ExpDt,
                'unitprice': price,
                'Amount': total,
                'Description': $scope.child.Description,
                'BatchNo': $scope.child.BatchNo
            });
            $scope.grandtotalprice = $scope.totalprice;
            $scope.child = {
                Id: '',
                Name: '',
                Description: '',
                Mrg: '',
                BatchNo: '',
                ExpDt: '',
                Amount: '',
                Qty: ''
            }
        }
        else {
            $scope.ErrMsg = "please fill required fields";
        }

    }
    $scope.showMe = true;
    $scope.fnShowMe = function () {
        $scope.showMe = !$scope.showMe;
    }


    $scope.DelCrtIitem = function (index) {
        total = $scope.child.Amount;
        final -= total;
        $scope.totalprice = final;
        $scope.cartlists.splice(index, 1);
    }
    $scope.medicineselect = function (item) {
        $http.get('/Product/GetMedicnById/?id=' + item.id).then(function (res) {
            $scope.PrdId = item.id;
            $scope.medicineName = res.data.name;
            $scope.child = {
                Mfg: res.data.companyName,
                Description: res.data.name,
                BatchNo: res.data.batchNo,
                ExpDt: res.data.expDate,
                Amount: res.data.mrp,
                Availstk: res.data.openingStock
            };
        }, function (error) {
        })

    }

    $scope.SaveInvc = function () {
        $rootScope.isLoadingScreenActive = true;
        if ($scope.master.PatientName == '') {
            $scope.master.PatientName = 'Guest'
        }
        var obj1 = {
            'PatientName': $scope.master.PatientName,
            'PatientAdres': $scope.master.PatientAdres,
            'DrName': $scope.master.DrName,
            'RegNo': $scope.master.RegNo,
            'Discount': $scope.master.discountAmt
        };
        var obj2 = $scope.cartlists;
        var obj = { childinvc: obj2, masterinvc: obj1 }
        things = JSON.stringify({ 'things': obj1 });
        $http({
            method: 'post',
            url: "/Sales/AddMasterInvc",
            data: JSON.stringify(obj),
            dataType: "json"
        }).then(function (response) {
            $scope.isCreatedInvoice = true;
            $scope.successMessage = "Invoice has been successfully created..!";
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;
        })
    }
    $scope.PrintInvoice = function () {
        window.open("/Sales/Invoice", "_blank");
        // window.location="/Sales/Invoice";
    }
    $scope.qtychk = function (qty, avlstk) {
        if (qty > avlstk) {
            confirm("Given quantity is greater than available quantity")
        }
        if (qty == 0) {
            alert("Given quantity is not Available")
        }
    }
})

app.controller('StockController', function ($scope, $http, loadvndor, $rootScope) {
    sessionStorage.clear();
    setuser()
    loadvndor.getprdct();
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser) {
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }
    function setuser() {
        
        
    }
    //-----------------------------------XX----------------------------------
});

app.controller('PurchasedController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    loadvndor.getvndr();
    loadvndor.getprdct();
    $scope.isPreview = false;
    $scope.previewPO = function () {



        if (!$scope.vendorID) {
            $scope.vendorErrMsg = "- please select vendor.";
        }
        else {
            $scope.vendorErrMsg = '';
        }
        if ($scope.PreCreatePO.length < 1) {
            $scope.poErrMsg = "- please select at least one medicine";
        }
        else {
            $scope.poErrMsg = '';
        }

        if ($scope.vendorID && $scope.PreCreatePO.length > 0) {
            $scope.isPreview = !$scope.isPreview;
        }
    }
    $scope.isChecked = function (item) {
        for (var i = $scope.PreCreatePO.length - 1; i >= 0; i--) {
            if ($scope.PreCreatePO[i].ProdID == item.id) {
                return true;
                break;
            }
        }
        return false;
    }

    $scope.PreCreatePO = [];
    $scope.toggleItemIndex = function ($event, item) {
        $event.stopPropagation();
        //  var ProdList = $filter('orderBy')($rootScope.ProductList, 'openingStock');    


        if ($event.target.checked) {
            var temp = item;
            var CreateObj = {
                ProdID: temp.id,
                AvlStock: temp.openingStock,
                MedicineName: temp.name,
                Mfg: temp.companyName,
                VendorID: $scope.vendorID,
                Remarks: '',
                stef: item.stef,
                tabletsCapsule: item.tabletsCapsule
            }
            $scope.PreCreatePO.push(CreateObj)
        }
        else {
            for (var i = $scope.PreCreatePO.length - 1; i >= 0; i--) {
                if ($scope.PreCreatePO[i].ProdID == item.id) {
                    $scope.PreCreatePO.splice(i, 1);
                }
            }

        }
    }

    $scope.isVendorSelected = false;
    $scope.vendorID;
    $scope.getSelectedVendor = function () {
        loadvndor.getVendorbyID($scope.vendorID);
        $scope.isVendorSelected = true;
    }

    $scope.createPoOrder = function () {
        $scope.errEntry = "";
        $scope.isReadyToCallAPI = false;
        for (var i = 0; i < $scope.PreCreatePO.length; i++) {
            if ($scope.PreCreatePO[i].stef) {
                $scope.isReadyToCallAPI = true;

            }
            else {
                $scope.errEntry = "please provide Stef.";
                $scope.isReadyToCallAPI = false;
                return false;
            }
        }

        if ($scope.isReadyToCallAPI) {
            $rootScope.isLoadingScreenActive = true;
            $scope.isSucessDB = false;
            var obj = $scope.PreCreatePO;
            $http({
                method: 'post',
                url: "/Purchased/CreatePO",
                data: JSON.stringify(obj),
                dataType: "json"
            }).then(function (res) {
                $scope.isSucessDB = true;
                $rootScope.isLoadingScreenActive = false;

            }, function (error) {
                $scope.isSucessDB = false;
                $rootScope.isLoadingScreenActive = false;
            })

        }
    }


});

app.controller('PurchasedInboxController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    $scope.masterPo = [];
    $scope.childPoView = false;

    function getPurchasedInbox() {
        $rootScope.isLoadingScreenActive = true;
        $http.get('/Purchased/InboxPO').then(function (res) {

            $scope.masterPo = res.data.masterPo;
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;
        })

    }
    getPurchasedInbox();
    $scope.backtoPurchased = function () {
        $scope.childPoView = !$scope.childPoView;

    }
    $scope.backToMail = function () {
        $scope.purchasedEntryView = !$scope.purchasedEntryView;
        $scope.childPoView = !$scope.childPoView;
    }
    $scope.purchasedEntryView = false;
    $scope.CreatePurchased = function () {
        $scope.Prepurchased = [];

        for (var i = 0; i < $scope.childPO.cpoList.length; i++) {
            var model = {
                ProductID: $scope.childPO.cpoList[i].productID,
                Name: $scope.childPO.cpoList[i].productName,
                Mfg: $scope.childPO.cpoList[i].mfg,
                stef: $scope.childPO.cpoList[i].stef,
                tabletsCapsule: $scope.childPO.cpoList[i].tabletsCapsule,
                eachStefPrice: $scope.childPO.cpoList[i].eachStefPrice,
                vendorID: $scope.childPO.vendorID,
                masterPOid: $scope.childPO.masterPOid,
                BatchNo: $scope.childPO.cpoList[i].batchNo,
                ExpDate: $scope.childPO.cpoList[i].expDate,
                Remark: $scope.childPO.cpoList[i].remarks
            }
            $scope.Prepurchased.push(model);
        }
        // $scope.Prepurchased = $scope.childPO;
        $scope.purchasedEntryView = true;

    }
    $scope.DeletePurchasedItem = function (val) {

        if (confirm('are you sure want to delete ?? \n medicine : ' + val.productName + "\n Mfg : " + val.mfg)) {
            var childId = val.childPoId;
            $rootScope.isLoadingScreenActive = true;
            $http.post('/Purchased/childPoDelete/' + childId).then(function (res) {

                alert('successfully deleted... \n medicine : ' + val.productName + '\n Mfg : ' + val.mfg);
                $scope.backtoPurchased();
                getPurchasedInbox();
                $rootScope.isLoadingScreenActive = false;
            }, function (error) {
                $rootScope.isLoadingScreenActive = false;
            })


        }
    }

    $scope.childDetails = function (val) {
        $scope.childPO = val;
        $scope.childPoView = !$scope.childPoView;
    }

    // purchased Entry 
    $scope.onSubmitforCreatePurchased = function () {
        var obj = $scope.Prepurchased;
        $scope.isSuccessPOEntry = false;
        $rootScope.isLoadingScreenActive = true;
        $http({
            method: 'post',
            url: "/Purchased/EntryCreatePurchase",
            data: JSON.stringify(obj),
            dataType: "json"
        }).then(function (response) {
            $scope.POmsg = "Purchased Entry has been successfully created..!";
            $scope.isSuccessPOEntry = true;
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $scope.isSuccessPOEntry = false;
            $scope.POmsg = "Something went wrong.. please try again..";
            $rootScope.isLoadingScreenActive = false;
        })

    }
    $scope.inboxfiler = "Open";
    $scope.onInboxFiler = function (val) {
        if (val == "Open") {
            $scope.inboxfiler = "Open";
            $('#chk1').attr('checked', true);
            $('#chk2').attr('checked', false);
            $('#chk3').attr('checked', false);
        }
        else if (val == "Closed") {
            $scope.inboxfiler = "Closed";
            $('#chk1').attr('checked', false);
            $('#chk2').attr('checked', true);
            $('#chk3').attr('checked', false);
        }
        else {
            $scope.inboxfiler = '';
            $('#chk1').attr('checked', false);
            $('#chk2').attr('checked', false);
            $('#chk3').attr('checked', true);
        }
    }
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser){
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }
    function setuser() {
        
        
    }
    //-----------------------------------XX----------------------------------
});

app.controller('PurchasedDirectEntryController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    loadvndor.getvndr();
    loadvndor.getprdct();
    $scope.isVendorSelected = false;
    $scope.medicineName = 'Name';
    $scope.vendorID;
    $scope.getSelectedVendor = function () {
        loadvndor.getVendorbyID($scope.vendorID);
        $scope.isVendorSelected = !$scope.isVendorSelected;
    }
    $scope.child = {
        Id: '',
        Name: '',
        Mfg: '',
        BatchNo: '',
        ExpDt: '',
        Remarks: '',
        stef: '',
        tabletsCapsule: '',
        eachStefPrice: ''

    }
    $scope.cartlists = [];
    $scope.AddChildInvc = function () {
        var qty = $scope.Qty
        if ($scope.child.stef && $scope.child.Mfg && $scope.child.BatchNo &&
            $scope.child.eachStefPrice && $scope.child.tabletsCapsule && $scope.child.ExpDt && $scope.child.Mrp) {
            $scope.err = "";
            $scope.cartlists.push({
                'PrdId': $scope.PrdId,
                'Name': $scope.child.Name,
                'stef': $scope.child.stef,
                'tabletsCapsule': $scope.child.tabletsCapsule,
                'eachStefPrice': $scope.child.eachStefPrice,
                'ExpDt': $scope.child.ExpDt,
                'Mfg': $scope.child.Mfg,
                'BatchNo': $scope.child.BatchNo,
                'Remarks': $scope.child.Remarks
            });
            $scope.child = [];

        }
        else {
            $scope.err = "please provide required fields (*)";

        }
        $scope.medicineName = '';
    }
    $scope.medicineselect = function (item) {
        $scope.PrdId = item.id;
        var id = $scope.PrdId;
        $http.get('/Product/GetMedicnById/?id=' + id).then(function (res) {
            $scope.medicineName = res.data.name;
            $scope.child = {
                Name: res.data.name,
                Mfg: res.data.companyName,
                Mrp: res.data.mrp,
                BatchNo: res.data.batchNo,
                ExpDt: res.data.expDate,
                Amount: res.data.mrp,
                AvlStock: res.data.openingStock,
                stef: res.data.stef,
                tabletsCapsule: res.data.tabletsCapsule,
                eachStefPrice: res.data.eachStefPrice
            };
        }, function (error) {
        })
    }

    $scope.DelCrtIitem = function (index) {
        $scope.cartlists.splice(index, 1);
    }

    $scope.isPreview = false;
    $scope.goBackfrmPreview = function () {
        $scope.isPreview = !$scope.isPreview;
    }
    $scope.onSubmit = function () {
        // TODO API Call
        $rootScope.isLoadingScreenActive = true;
        $scope.Prepurchased = [];
        for (var i = 0; i < $scope.cartlists.length; i++) {
            var model = {
                ProductID: $scope.cartlists[i].PrdId,
                Name: $scope.cartlists[i].Name,
                Mfg: $scope.cartlists[i].Mfg,
                vendorID: $scope.vendorID,
                //masterPOid: $scope.childPO.masterPOid,
                stef: $scope.cartlists[i].stef,
                tabletsCapsule: $scope.cartlists[i].tabletsCapsule,
                eachStefPrice: $scope.cartlists[i].eachStefPrice,
                BatchNo: $scope.cartlists[i].BatchNo,
                ExpDate: $scope.cartlists[i].ExpDt,
                Remark: $scope.cartlists[i].Remarks
            }
            $scope.Prepurchased.push(model);
        }

        var obj = $scope.Prepurchased;
        $scope.isSuccessPOEntry = false;
        $http({
            method: 'post',
            url: "/Purchased/EntryCreatePurchase",
            data: JSON.stringify(obj),
            dataType: "json"
        }).then(function (response) {
            $scope.POmsg = "Purchased Entry has been successfully created..!";
            $scope.isSuccessPOEntry = true;
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $scope.isSuccessPOEntry = false;
            $scope.POmsg = "Something went wrong.. please try again..";
            $rootScope.isLoadingScreenActive = false;

        })


    }

    $scope.Preview = function () {

        if ($scope.cartlists.length == 0) {
            $scope.vendorErrMsg = "please add at least one item in cart";
        }
        else {
            $scope.vendorErrMsg = "";            // TODO
            $scope.isPreview = !$scope.isPreview;
            $scope.isVendorSelected = !$scope.isVendorSelected;


        }
    }

});

app.controller('InvoiceInboxController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();

    $scope.InvList = [];
    $scope.isPreview = false;
    $scope.frmDate = '';
    $scope.toDate = '';
    $scope.filerErr = ''
    $scope.filtered = false;
    $scope.getSubtotal = function () {
        var subtotal = 0
        for (var i = 0; i < $scope.InvList.length; i++) {
            if ($scope.InvList[i].billingAmount) {
                subtotal += parseFloat($scope.InvList[i].billingAmount);
            }
        }
        return parseFloat(subtotal);
    }
    $scope.getdiscount = function () {
        var discount = 0
        for (var i = 0; i < $scope.InvList.length; i++) {
            if ($scope.InvList[i].discount) {
                discount += parseFloat($scope.InvList[i].discount);
            }
        }
        return parseFloat(discount);
    }

    // $rootScope.isLoadingScreenActive = true;
    $scope.onFilterInbox = function () {
        if ($scope.frmDate && $scope.toDate) {
            $rootScope.isLoadingScreenActive = true;
            var invoiceURL = "/Sales/GetAllInvoice?fromDate=" + $scope.frmDate + "&toDate=" + $scope.toDate;
            $http.get(invoiceURL).then(function (res) {
                $scope.InvList = res.data;
                $rootScope.isLoadingScreenActive = false;
                $scope.filtered = true;
            }, function (error) {
                $rootScope.isLoadingScreenActive = false;
                $scope.filtered = true;
            })
            $scope.filerErr = '';
        }
        else {
            $scope.filerErr = "please provide date";
        }
    }
    $scope.fnInbox = function () {
        $scope.isPreview = !$scope.isPreview
    }
    $scope.childDetails = function (val) {

        $scope.Mastinv = val;
        $scope.isPreview = true;
        $scope.isSucessDB = false;
    }
    $scope.onReturn = function () {

        if ($scope.ReturnInv.length >= 1) {
            $scope.returnPreview = true;
        }
        else {
            $scope.returnPreview = false;
            $scope.returnItemRequired = "please select at least one item.";
        }
    }
    $scope.isSucessDB = false;
    $scope.ngSubmitReturn = function () {
        var obj = $scope.ReturnInv;
        let hasMagenicVendor = obj.some(vendor => vendor['soldQty'] === 0)
        if (hasMagenicVendor == true) {
            alert("Some of your product is no more left to return.!!");
            return false
        }
        else {
            $rootScope.isLoadingScreenActive = true;
            $http({
                method: 'post',
                url: "/Sales/ReturnMedicine",
                data: JSON.stringify(obj),
                dataType: "json"
            }).then(function (res) {
                $scope.isSucessDB = true;
                $rootScope.isLoadingScreenActive = false;
            }, function (error) {
                $scope.isSucessDB = false;
                $rootScope.isLoadingScreenActive = false;
            })
        }
    }
    $scope.ReturnInv = [];
    $scope.returnPreview = false;
    $scope.toggleItemIndex = function ($event, index) {
        $event.stopPropagation();
        if ($event.target.checked) {
            var temp = $scope.Mastinv.childInv[index];
            var CreateObj = {
                mastInv: $scope.Mastinv.invId,
                parentIndex: index,
                batchNo: temp.batchNo,
                expDate: temp.expDate,
                mfg: temp.mfg,
                mrp: temp.mrp,
                id: temp.id,
                prodID: temp.prodID,
                soldQty: temp.qty,
                qty: 0,
                name: temp.name,
                remarks: ''
            }
            $scope.ReturnInv.push(CreateObj);

        }
        else {
            for (var i = $scope.ReturnInv.length - 1; i >= 0; i--) {
                if ($scope.ReturnInv[i].parentIndex == index) {
                    $scope.ReturnInv.splice(i, 1);
                }
            }
        }

    }
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser) {
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }
    function setuser() {
       
       
    }
    //-----------------------------------XX----------------------------------    
});

app.controller('DashboardController', function ($rootScope) {
    $rootScope.isLoadingScreenActive = false;
    sessionStorage.clear();
});

app.controller('SalesResportController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    //$scope.leader = '';
    $scope.InvList = [];
    $scope.GridData = []
    $scope.FilterType = '';
    $scope.onInboxFiler = function (val) {
        $scope.GridData = [];
        if (val == 'Monthly') {
            $scope.GridData = $scope.InvList.MonthlyResult;
            $scope.FilterType = 'm';
            $('#chk2').attr('checked', true);
            $('#chk1').attr('checked', false);
            $('#chk3').attr('checked', false);
        }
        else if (val == 'Yearly') {
            $scope.GridData = $scope.InvList.YearlyResult;
            $scope.FilterType = 'y';
            $('#chk3').attr('checked', true);
            $('#chk2').attr('checked', false);
            $('#chk1').attr('checked', false);
        }
        else {
            $scope.GridData = $scope.InvList.DailyResult;
            $scope.FilterType = 'd';
            $('#chk1').attr('checked', true);
            $('#chk2').attr('checked', false);
            $('#chk3').attr('checked', false);
        }

    }
    $scope.getTotal = function () {
        var total = 0;
        var discount = 0;
        for (var i = 0; i < $scope.GridData.length; i++) {
            var amount = parseFloat($scope.GridData[i].amount);
            total += amount;

            // discount
            var dis = $scope.GridData[i].discount;
            if (dis) {
                discount += parseFloat(dis);
            }
        }
        return (total - discount);
    }
    $scope.getTotalInv = function () {
        var grosstotalInv = 0;
        for (var i = 0; i < $scope.GridData.length; i++) {
            var totalInv = $scope.GridData[i].totalInv;
            grosstotalInv += totalInv;
        }
        return grosstotalInv;
    }
    $rootScope.isLoadingScreenActive = true
    $http.get('/Sales/GridRecords').then(function (res) {
        $scope.InvList = res.data;
        for (var i = 0; i < $scope.InvList.DailyResult.length; i++) {
            if ($scope.InvList.DailyResult[i].inv_date == $scope.InvList.DailyDiscount[i].inv_date) {
                $scope.InvList.DailyResult[i].discount = $scope.InvList.DailyDiscount[i].discount;
            }
        }
        for (var i = 0; i < $scope.InvList.MonthlyResult.length; i++) {
            if ($scope.InvList.MonthlyResult[i].inv_date == $scope.InvList.MonthlyDiscount[i].inv_date) {
                $scope.InvList.MonthlyResult[i].discount = $scope.InvList.MonthlyDiscount[i].discount;
            }
        }
        for (var i = 0; i < $scope.InvList.YearlyResult.length; i++) {
            if ($scope.InvList.YearlyResult[i].inv_date == $scope.InvList.YearlyDisCount[i].inv_date) {
                $scope.InvList.YearlyResult[i].discount = $scope.InvList.YearlyDisCount[i].discount;
            }
        }

        $scope.onInboxFiler('');
        $rootScope.isLoadingScreenActive = false;
    }, function (error) {
        $rootScope.isLoadingScreenActive = false;
    })
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser){
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }
    function setuser() {
       
        
    }
    //-----------------------------------XX----------------------------------
});

app.controller('TotalExpMedicineController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    $rootScope.isLoadingScreenActive = true
    $http.get('/Home/getTotalExpMedicine').then(function (res) {
        $scope.InvList = res.data;

        $rootScope.isLoadingScreenActive = false;
    }, function (error) {
        $rootScope.isLoadingScreenActive = false;
    })
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser){
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }
    function setuser() {
        
        
    }
    //-----------------------------------XX----------------------------------
});

app.controller('TopSellingMedicineController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    $rootScope.isLoadingScreenActive = true
    $http.get('/Home/getSellingMedicineReport').then(function (res) {
        $scope.InvList = res.data;
        $rootScope.isLoadingScreenActive = false;
    }, function (error) {
        $rootScope.isLoadingScreenActive = false;
    })
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser) {
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }

    function setuser() {
        
        
    }
    //-----------------------------------XX----------------------------------
});


app.controller('OutOfStockMedicineController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    $rootScope.isLoadingScreenActive = true
    $http.get('/Home/getOutOfStockMedicine').then(function (res) {
        $scope.InvList = res.data;

        $rootScope.isLoadingScreenActive = false;
    }, function (error) {
        $rootScope.isLoadingScreenActive = false;
    })
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title: '',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    loadvndor.currentUser();
    if ($rootScope.currentUser) {
        var user = $rootScope.currentUser;
        $scope.curuser.clinicname = user.name;
        $scope.curuser.Title = user.subTitle;
        $scope.curuser.email = user.email;
        $scope.curuser.dlno = user.dlNo;
        $scope.curuser.mobile = user.mobile;
        $scope.curuser.address = user.address1 + ' ' + user.address2;
    }

    function setuser() {
       
       
    }
    //-----------------------------------XX----------------------------------
});