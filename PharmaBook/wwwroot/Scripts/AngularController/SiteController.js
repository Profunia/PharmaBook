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

    fac.getVendorbyID = function (id) {
        $http.get('/Vendor/GetVendorByID/' + id).then(function (res) {
            $rootScope.selectedVendor = res.data;

        }, function (error) {
        }
        )
    };
    return fac;
}])
app.controller('MyController', function ($scope, $http, loadvndor, $rootScope) {
    loadvndor.getvndr();
    $scope.divhide1 = false;
    $scope.divhide2 = true;
    //$scope.savebtn = false;
    //$scope.cClass = true;
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
        $rootScope.isLoadingScreenActive = true
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
            $scope.isPreview = true;
            $rootScope.isLoadingScreenActive = false;
        }, function (error) {
            $scope.isPreview = false;
            $rootScope.isLoadingScreenActive = false;
        })
    }
    var counter = 0;
    $scope.showdata = function (item, inc) {
        //$scope.savebtn = true;
        //$scope.editbtn = false;
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
})
app.controller('ProductController', function ($scope, $http, $location, $rootScope,loadvndor, $window) {
    $scope.isStefActive = false;
    $scope.stef = '';
    $scope.tablets = '';
    $scope.StefPrice = '';
    $scope.getStockMRP = function () {
        if ($scope.StefPrice) {
            if ($scope.stef && $scope.tablets) {
                $scope.MediProdct.openingStock = $scope.stef * $scope.tablets;
                var unitPrice = $scope.StefPrice * $scope.stef;
                console.log(unitPrice)
                $scope.MediProdct.MRP = unitPrice / $scope.MediProdct.openingStock;
                $scope.isStefActive = true;
                $scope.MediProdct.Remarks = "Stef " + $scope.stef + " x tablets/capsule " + $scope.tablets + " & each stef Price " + $scope.StefPrice;  
            }
        }
        else {
            $scope.isStefActive = false;
            $scope.stef = '';
            $scope.tablets = '';
            $scope.StefPrice = '';
            $scope.MediProdct.Remarks = '';
            $scope.MediProdct.MRP = '';
            $scope.MediProdct.openingStock = '';
        }
    }
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
    $scope.AddMedcin = function () {

        if ($scope.MediProdct.MedicineName && $scope.MediProdct.batchNo
            && $scope.MediProdct.openingStock && $scope.MediProdct.CompanyName
            && $scope.MediProdct.ExpDt && $scope.MediProdct.MRP) {
            $rootScope.isLoadingScreenActive = true;
            var obj = {
                'name': $scope.MediProdct.MedicineName,
                'batchNo': $scope.MediProdct.batchNo,
                'openingStock': $scope.MediProdct.openingStock,
                'expDate': $scope.MediProdct.ExpDt,
                'companyName': $scope.MediProdct.CompanyName,
                'MRP': $scope.MediProdct.MRP,
                'vendorID': $scope.MediProdct.vendorID,
                'Remarks': $scope.MediProdct.Remarks
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
            'MRP': item.mrp,
            'openingStock': item.openingStock,
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
})

app.controller('SalesController', function ($scope, $http, $rootScope,loadvndor) {
    var final = 0;
    var total = 0;
    function initialSetup() {
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
            Amount: ''
        }
    }
    initialSetup();
    $scope.isCreatedInvoice = false;
    $scope.switchToInvoicePage = function () {
        initialSetup();
        $scope.isCreatedInvoice = false;
    }
   
    $scope.AddChildInvc = function () {
        var qty = $scope.Qty
        $scope.ErrMsg = '';
        if ($scope.PrdId && $scope.Qty  && $scope.child.Description) {
            var price = $scope.child.Amount
            total = (qty * price);
            final += total;
            $scope.unitprice = price;
            $scope.totalprice = final;
            $scope.cartlists.push({
                'PrdId': $scope.PrdId,
                'Qty': $scope.Qty,
                'ExpDt': $scope.child.ExpDt,
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
    $scope.medicineselect = function () {
        var id = $scope.PrdId;
        $http.get('/Product/GetMedicnById/?id=' + id).then(function (res) {
            $scope.child = { Description: res.data.companyName, BatchNo: res.data.batchNo, ExpDt: res.data.expDate, Amount: res.data.mrp };
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
        window.location="/Sales/Invoice";
    }
})

app.controller('StockController', function ($scope, $http, loadvndor, $rootScope) {

    loadvndor.getprdct();

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
   
    $scope.PreCreatePO = [];
    $scope.toggleItemIndex = function ($event, index) {
        $event.stopPropagation();
        var ProdList = $filter('orderBy')($rootScope.ProductList, 'openingStock');       
        
        if ($event.target.checked) {
            
            var temp = ProdList[index];            
            var CreateObj = {
                productIndex: index,
                ProdID: temp.id,
                AvlStock: temp.openingStock,
                MedicineName: temp.name,
                Mfg: temp.companyName,
                Qty: '',
                VendorID: $scope.vendorID,
                Remarks: ''
            }
            $scope.PreCreatePO.push(CreateObj)
        }
        else {
            for (var i = $scope.PreCreatePO.length - 1; i >= 0; i--) {
                if ($scope.PreCreatePO[i].productIndex == index) {
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
            if ($scope.PreCreatePO[i].Qty)
                $scope.isReadyToCallAPI = true;
            else {
                $scope.errEntry = "please provide Qty.";
                $scope.isReadyToCallAPI = false;
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
    $scope.masterPo = [];
    $scope.childPoView = false;
    function getPurchasedInbox()
    {
        $http.get('/Purchased/InboxPO').then(function (res) {
        
            $scope.masterPo = res.data.masterPo;

        }, function (error) {

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
                Qty: $scope.childPO.cpoList[i].qty,
                vendorID: $scope.childPO.vendorID,
                masterPOid: $scope.childPO.masterPOid,
                BatchNo: $scope.childPO.cpoList[i].batchNo,
                MRP: $scope.childPO.cpoList[i].mrp,
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
        Mrp: '',
        Qty: '',
        Remarks:''
    }
    $scope.cartlists = [];
    $scope.AddChildInvc = function () {        
        var qty = $scope.Qty
        if ($scope.child.Qty && $scope.child.Mfg && $scope.child.BatchNo && $scope.child.ExpDt && $scope.child.Mrp) {
            $scope.err = "";
            $scope.cartlists.push({
                'PrdId': $scope.PrdId,
                'Name': $scope.child.Name,
                'Qty': $scope.child.Qty,
                'ExpDt': $scope.child.ExpDt,
                'Mrp': $scope.child.Mrp,
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
          
            $scope.child = {
                Name:res.data.name,
                Mfg: res.data.companyName,
                Mrp: res.data.mrp,
                BatchNo: res.data.batchNo,
                ExpDt: res.data.expDate,
                Amount: res.data.mrp,
                AvlStock: res.data.openingStock
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
                Qty: $scope.cartlists[i].Qty,
                vendorID: $scope.vendorID,
                //masterPOid: $scope.childPO.masterPOid,
                BatchNo: $scope.cartlists[i].BatchNo,
                MRP: $scope.cartlists[i].Mrp,
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

    
    $scope.InvList = [];
    $scope.isPreview = false;
    
    $http.get('/Sales/GetAllInvoice').then(function (res) {
        $scope.InvList = res.data;
       
    }, function (error) {
     
    })

    $scope.childDetails = function (val) {
   
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
});

app.controller('DashboardController', function ($rootScope) {
    $rootScope.isLoadingScreenActive = false;
});