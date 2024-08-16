
function MapVendor(packageid) {
    
    var _billerid = $("#hdbillerid").val();
    var _data = {
        packageID: packageid,
        interswitchmap: interswitchmap,
        cwgmap: cwgmap,
        primemap: primemap,
        creditSwitch: creditSwitchmap,
        clickatell: clickatellmap
    }

    console.log(_data);
    var newUrl = "/ManageBiller/MapPackage";
    log(newUrl);
    $.ajax({
        url: baseUrl + newUrl,
        data: _data,
        type: "POST",
        dataType: "json",
        crossDomain: true,
        traditional: true,
        success: function (data) {
            console.log(data);
            if (data == false) {

                alert("Unable to map packages");
                return;
            }
            else {
                window.location.href = baseUrl + '/ManageBiller/Index?Billerid=' + _billerid;

            }

        },
        error: function () {

        },
        complete: function () {


        }

    });

}


function Register(id) {
    var newUrl = "/ManageBiller/MapPackage";
    $.ajax({
        url: baseUrl + newUrl,
        data: _data,
        type: "POST",
        dataType: "json",
        crossDomain: true,
        traditional: true,
        success: function (data) {
            console.log(data);
            if (data.id == -1) {

                alert("Package Name cannot be Duplicated");
                return;
            }
            else {
                window.location.href = baseUrl + '/ManageBiller/Index?Billerid=' + _billerid;

            }

        },
        error: function () {

        },
        complete: function () {


        }

    });
}

function Register() {
    if ($("#packName").val() == "") {
        alert("Please enter a valid Package Name");
        return;
    }
    if ($("#packplan").val() == "") {
        alert("Please enter a valid Data plan Name");
        return;
    }
    if ($("#packamount").val() == "") {
        alert("Please enter a valid Amount ");
        return;
    }
    if ($("#packvalidity").val() == "") {
        alert("Please enter a valid Validity Period");
        return;
    }
    if ($("#packDescription").val() == "") {
        alert("Please enter a valid Description");
        return;
    }
    var _IsActive;
    if ($("#chkdataactive").is(":checked") == true) {
        _IsActive = true;
    }
    else {
        _IsActive = false;
    }
    var _packageName = $("#packName").val();
    var _Dataplan = $("#packplan").val();
    var _amount = $("#packamount").val();
    var _Validity = $("#packvalidity").val();
    var _description = $("#packDescription").val();
    var Packageid = $("#hdpackageid").val();
    var billerid = $("#hdbillerid").val();
    var _data = {
        billerid: billerid,
        packageName: _packageName,
        Dataplan: _Dataplan,
        amount: _amount,
        Validity: _Validity,
        description: _description,
        packageid: Packageid,
        IsActive: _IsActive
    }
    console.log(_data);
    var newUrl = "/ManageBiller/SavePackage";
    log(newUrl);
    $.ajax({
        url: baseUrl + newUrl,
        data: _data,
        type: "POST",
        dataType: "json",
        crossDomain: true,
        traditional: true,
        success: function (data) {
            console.log(data);
            if (data.id == -1) {

                alert("Package Name cannot be Duplicated");
                return;
            }
            else {
                window.location.href = baseUrl + '/ManageBiller/Index?Billerid=' + billerid;

            }

        },
        error: function () {

        }

    });
}