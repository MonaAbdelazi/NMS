function getdata() {

    var coverage = getCoveragedata();
    
    var network = new Array();
    var relation = new Array();
    var NOS = new Array();


    var chlidrn = document.getElementById("NOSVal").getElementsByTagName('input');

    for (i = 0; i < chlidrn.length; i++) {
        val = chlidrn[i].getAttribute('name');
        networkId = val.substring(8, 9);
        relationId = val.substring(11, 13);
        numOFsub = chlidrn[i].value;
        NOS.push(numOFsub);
        network.push(networkId);
        relation.push(relationId);

    }

    AjaxRequest(NOS, relation, network, coverage);

}


function getCoveragedata() {

    var coverageType = document.getElementById("CoverageType").children;
    var coverage = new Array();

    for (w = 0; w < coverageType.length; w++) {

        id = coverageType[w].childNodes[1];
        result = id.checked;
        if (!result) {
            continue;
        }
        val = id.getAttribute("qwer");
        nam = id.getAttribute("name");
        coverage.push(val);
    }
    return coverage;

}







//    AjaxRequest(NOS, relation, network, coverage);

function AjaxRequest(NOS, relation, network, coverage)
{
    var CustomerOfferId = document.getElementById("CustomerOfferId").value;
    var CurrencyId = document.getElementById("CurrencyId").value;

    var obj = {
        "relation" : relation,
        "numberofSub" : NOS,
        "network" : network,
        "coverageType" : coverage,
        "customerOfferId": CustomerOfferId,
        "CurrencyId" : CurrencyId
    }
    

    
    var postresult = JSON.stringify(obj);



    $.ajax({
        url: '/priceShow/SaveOfferNumbers',
        type: "POST",
        data: postresult,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            alert(data);
            //window.location.href = 'result/' + CustomerOfferId;

        },
        error: function () {
            alert("Error");
        }
    });
}

