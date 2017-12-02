var appA = angular.module("ProductModule", []);
var app = angular.module("MyModuleA", []);

app.factory('loadvndor', ['$http', '$rootScope', function ($http, $rootScope) {
    $rootScope.UserName = '';
    var fac = {};
    fac.getvndr = function () {
        $rootScope.isLoadingScreenActive = true;
        $http.get('/Vendor/GetAllVendor').then(function (res) {
            $rootScope.VendorList = res.data;    
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;
        }
        )
    }
    fac.getprdct = function () {
        $rootScope.isLoadingScreenActive = true;
        $http.get('/Product/GetAllMedicine').then(function (res) {
            $rootScope.ProductList = res.data; 
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;
        }       
        )
    }

    fac.getVendorbyID = function (id) {
        $rootScope.isLoadingScreenActive = true;
        $http.get('/Vendor/GetVendorByID/' + id).then(function (res) {
            $rootScope.selectedVendor = res.data;   
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $rootScope.isLoadingScreenActive = false;
        }
        )
    };   
    return fac;
}])
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
        UserName: ''
    }
    debugger
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
    setuser();   
    $scope.getSelectedVendor = function () {
        if ($scope.vendorDDL !='') {
            $scope.searchbox = $scope.vendorDDL;
        }
        else {
            $scope.searchbox = '';            
        }
        console.log($scope.vendorDD);
        console.log($scope.searchbox);
    }

    $scope.ItemNameMedicine = '';
    $scope.medicineselect = function () {
        console.log($scope.ItemNameMedicine);
        if ($scope.ItemNameMedicine != '') {
            $scope.searchbox = $scope.ItemNameMedicine;
        }
        else {
            $scope.searchbox = '';           
        }
        console.log($scope.ItemNameMedicine);
        console.log($scope.searchbox);
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
        Remarks:''
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
            console.log(obj);
            $http({
                method: 'post',
                url: "/Product/Create",
                data: JSON.stringify(obj),
                dataType: "json"
                // headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            }).then(function (response) {
                $window.location.href = "/Product/Create";
                $rootScope.isLoadingScreenActive = false;
            }, function (error) {
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
        console.log(obj);
        $http({
            method: 'post',
            url: "/Product/UpdateMedicn",
            data: obj,
            datatype: "json",
        }).then(function (response) {
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
        Title:'',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    function setuser() {
        $http.get('/Home/CurUser/').then(function (res) {
            var user = res.data;
            $scope.curuser.clinicname = user.name;
            $scope.curuser.Title = user.subTitle;
            $scope.curuser.email = user.email;
            $scope.curuser.dlno = user.dlNo;
            $scope.curuser.mobile = user.mobile;
            $scope.curuser.address = user.address1 + ' ' + user.address2;
        }, function (error) {
        }
        )
    }
//-----------------------------------XX----------------------------------
})

app.controller('SalesController', function ($scope, $http, $rootScope,loadvndor) {
    var total = 0;
    function initialSetup() {
        $scope.medicineName = 'Name';
        $scope.isPreview=false;
        final = 0;
        total = 0;
        $scope.unitprice = '';
        $scope.totalprice = '';
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
            RegNo: ''
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
        if ($scope.child.Qty  && $scope.child.Description) {
            var price = $scope.child.Amount;
            var qty = $scope.child.Qty;
          //  console.log("amount" + $scope.child.Amount);
          //  console.log(qty + price);
          // var  total = ($scope.child.Qty * $scope.child.Amount);
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
           
        }
        else {
            $scope.ErrMsg = "please fill required fields";
        }
        
    }
    $scope.DelCrtIitem = function (index) {
        total = $scope.child.Amount;
        final -= total;
        $scope.totalprice = final;
        $scope.cartlists.splice(index, 1);
    }
    $scope.medicineselect = function (item) {       
        $http.get('/Product/GetMedicnById/?id=' + item.id).then(function (res) {
            console.log(res.data);
            $scope.PrdId = item.id;
            $scope.medicineName = res.data.name;
            $scope.child = {
                Mfg: res.data.companyName,
                Description: res.data.name,
                BatchNo: res.data.batchNo,
                ExpDt: res.data.expDate,
                Amount: res.data.mrp
            };
            console.log($scope.child);
        }, function (error) {
            })
        
    }

    $scope.SaveInvc = function () {
        $rootScope.isLoadingScreenActive = true;
        if ($scope.master.PatientName == '' || $scope.master.PatientAdres == '' || $scope.master.DrName == '' || $scope.master.RegNo == '') {
            $scope.master.PatientName = 'Guest',
                $scope.master.PatientAdres = '',
                $scope.master.DrName = '',
                $scope.master.RegNo = ''
        }
        var obj1 = {
            'PatientName': $scope.master.PatientName,
            'PatientAdres': $scope.master.PatientAdres,
            'DrName': $scope.master.DrName,
            'RegNo': $scope.master.RegNo
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
})

app.controller('StockController', function ($scope, $http, loadvndor, $rootScope) {
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
    function setuser() {
        $http.get('/Home/CurUser/').then(function (res) {
            var user = res.data;
            $scope.curuser.clinicname = user.name;
            $scope.curuser.Title = user.subTitle;
            $scope.curuser.email = user.email;
            $scope.curuser.dlno = user.dlNo;
            $scope.curuser.mobile = user.mobile;
            $scope.curuser.address = user.address1 + ' ' + user.address2;
        }, function (error) {
        }
        )
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
            $scope.poErrMsg ="- please select at least one medicine";
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
            console.log(item);
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
            if ($scope.PreCreatePO[i].stef)
            {
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
    
    function getPurchasedInbox()
    {
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
        console.log($scope.Prepurchased);

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
       var obj= $scope.Prepurchased;
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
        }
        else if (val == "Closed")
        {
            $scope.inboxfiler = "Closed";
        }
        else {
            $scope.inboxfiler = '';
        }
    }
 //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title:'',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    function setuser() {
        $http.get('/Home/CurUser/').then(function (res) {
            var user = res.data;
            $scope.curuser.clinicname = user.name;
            $scope.curuser.Title = user.subTitle;
            $scope.curuser.email = user.email;
            $scope.curuser.dlno = user.dlNo;
            $scope.curuser.mobile = user.mobile;
            $scope.curuser.address = user.address1 + ' ' + user.address2;
        }, function (error) {
        }
        )
    }
//-----------------------------------XX----------------------------------
});

app.controller('PurchasedDirectEntryController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    loadvndor.getvndr();
    loadvndor.getprdct();
    $scope.isVendorSelected = false;
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
        eachStefPrice:''

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
       
    }
    $scope.medicineselect = function () {
        var id = $scope.PrdId;
        $http.get('/Product/GetMedicnById/?id=' + id).then(function (res) {
            console.log(res.data);
            $scope.child = {
                Name:res.data.name,
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
        if (!$scope.vendorID) {
            $scope.vendorErrMsg = "please select vendor";
        }
        else {
            $scope.vendorErrMsg = "";
            // TODO
            $scope.isPreview = !$scope.isPreview;

        }
    }

});

app.controller('InvoiceInboxController', function ($scope, $http, loadvndor, $rootScope, $filter) {    
    setuser();
    $scope.InvList = [];
    $scope.isPreview = false;
    $rootScope.isLoadingScreenActive = true;
    $http.get('/Sales/GetAllInvoice').then(function (res) {
        $scope.InvList = res.data;
        $rootScope.isLoadingScreenActive = false;
    }, function (error) {
        $rootScope.isLoadingScreenActive = false;
    })

    $scope.childDetails = function (val) {
        console.log(val);
        $scope.Mastinv = val;
        $scope.isPreview = true;
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
    $scope.ReturnInv = [];
    $scope.returnPreview = false;
    $scope.toggleItemIndex = function ($event, index) {
        $event.stopPropagation(); 
        if ($event.target.checked) {
            var temp = $scope.Mastinv.childInv[index];
            var CreateObj = {
                mastInv: $scope.Mastinv.invId,                
                parentIndex:index,
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
        Title:'',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    function setuser() {
        $http.get('/Home/CurUser/').then(function (res) {
            var user = res.data;
            $scope.curuser.clinicname = user.name;
            $scope.curuser.Title = user.subTitle;
            $scope.curuser.email = user.email;
            $scope.curuser.dlno = user.dlNo;
            $scope.curuser.mobile = user.mobile;
            $scope.curuser.address = user.address1 + ' ' + user.address2;
        }, function (error) {
        }
        )
    }
//-----------------------------------XX----------------------------------
});

app.controller('DashboardController', function ($rootScope) {
    $rootScope.isLoadingScreenActive = false;
});

app.controller('SalesResportController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    $scope.InvList = [];
    $scope.GridData = []
    $scope.FilterType = '';
    $scope.onInboxFiler = function (val) {
        $scope.GridData = [];
        if (val == 'Monthly') {
            $scope.GridData = $scope.InvList.MonthlyResult;
            $scope.FilterType = 'm';
        }
        else if (val == 'Yearly') {
            $scope.GridData = $scope.InvList.YearlyResult;
            $scope.FilterType = 'y';
        }
        else {
            $scope.GridData = $scope.InvList.DailyResult;
            $scope.FilterType = 'd';
        }
        
    }
    $scope.getTotal = function () {
        var total = 0;
        for (var i = 0; i < $scope.GridData.length; i++) {
            var amount = $scope.GridData[i].amount;
            total += amount;
        }
        return total;
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
        console.log($scope.InvList)
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
    function setuser() {
        $http.get('/Home/CurUser/').then(function (res) {
            var user = res.data;
            $scope.curuser.clinicname = user.name;
            $scope.curuser.Title = user.subTitle;
            $scope.curuser.email = user.email;
            $scope.curuser.dlno = user.dlNo;
            $scope.curuser.mobile = user.mobile;
            $scope.curuser.address = user.address1 + ' ' + user.address2;
        }, function (error) {
        }
        )
    }
//-----------------------------------XX----------------------------------
});

app.controller('TotalExpMedicineController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    $rootScope.isLoadingScreenActive = true
    $http.get('/Home/getTotalExpMedicine').then(function (res) {
        $scope.InvList = res.data;
        console.log($scope.InvList)
     
        $rootScope.isLoadingScreenActive = false;
    }, function (error) {
        $rootScope.isLoadingScreenActive = false;
    })
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title:'',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    function setuser() {
        $http.get('/Home/CurUser/').then(function (res) {
            var user = res.data;
            $scope.curuser.clinicname = user.name;
            $scope.curuser.Title = user.subTitle
            $scope.curuser.email = user.email;
            $scope.curuser.dlno = user.dlNo;
            $scope.curuser.mobile = user.mobile;
            $scope.curuser.address = user.address1 + ' ' + user.address2;
        }, function (error) {
        }
        )
    }
//-----------------------------------XX----------------------------------
});
app.controller('OutOfStockMedicineController', function ($scope, $http, loadvndor, $rootScope, $filter) {
    setuser();
    $rootScope.isLoadingScreenActive = true
    $http.get('/Home/getOutOfStockMedicine').then(function (res) {
        $scope.InvList = res.data;
        console.log($scope.InvList)
        
        $rootScope.isLoadingScreenActive = false;
    }, function (error) {
        $rootScope.isLoadingScreenActive = false;
    })
    //------------------Set Vendor For Report Printing----------------------
    $scope.curuser = {
        clinicname: '',
        Title:'',
        email: '',
        dlno: '',
        mobile: '',
        address: ''
    }
    function setuser() {
        $http.get('/Home/CurUser/').then(function (res) {
            var user = res.data;
            $scope.curuser.clinicname = user.name;
            $scope.curuser.Title = user.subTitle;
            $scope.curuser.email = user.email;
            $scope.curuser.dlno = user.dlNo;
            $scope.curuser.mobile = user.mobile;
            $scope.curuser.address = user.address1 + ' ' + user.address2;
        }, function (error) {
        }
        )
    }
//-----------------------------------XX----------------------------------
});