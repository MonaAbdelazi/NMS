AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};  

function beginInsert(){
    
    NumberOfRows = document.getElementById("RowCount").value;
    alert("Rows :" + NumberOfRows);
    for (var i = 0; i < NumberOfRows; i++) {
        network = "NetworkId " + i.toString();
        coverage = "CoverageTypeId " + i.toString();
        annual = "AnnualPremium " + i.toString();
        limit = "LimitAmount " + i.toString();
        Text = "CoPaymentText " + i.toString();
        

        NetworkId = document.getElementById(network).value;
        CoverageTypeId = document.getElementById(coverage).value;
        AnnualPremium = document.getElementById(annual).value;
        LimitAmount = document.getElementById(limit).value;
        CoPaymentText = document.getElementById(Text).value;
        
        
    

        val = insert(NetworkId, CoverageTypeId, AnnualPremium, LimitAmount, CoPaymentText);

    }

            if (val == 1)
        {
            alert("Insert Successful");
        }
        if (val = -1)
        {
            alert("Error : -1");
        }else
        {
            alert("Error : -2");
        }
}


function insert( net , type, annual , limit , pay)
{
    id = 0;
    tarig = document.getElementById("EffectiveDate").value;
    In = document.getElementById("InsuranceCompanyId").value;
    Currency = document.getElementById("CurrencyId").value;
    CustomerType = document.getElementById("CustomerType").value;
    url = "http://localhost:1883/StandardCoverage/Create";

    postdata = { "EffectiveDate": tarig, "InsuranceCompanyId": In, "CurrencyId": Currency, "NetworkId": net, "CoverageTypeId": type, "AnnualPremium": annual, "LimitAmount": limit, "CoPaymentText": pay, "CustomerType": CustomerType };
    postresult = JSON.stringify(postdata);
    $.ajax({
        url: url,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: postresult,
        success: function (data) {
            if (data == "yes") {
                id = 1;

            } else
                id = -1;
            },
        error: function () { id = -2;}
    });

    return id;
}