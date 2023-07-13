///////here you can find common function
///////wirte by sharif moawia


//Show Feedback Massage Call from Layout Screen
function ShowFeedback(message, type, messageStyle) {
     var massage = "<div class='alert alert-" + messageStyle.toString().toLowerCase() + "'> <a href='#' class='close' data-dismiss='alert'>&times;</a> <strong>" + type + "! </strong> " + message + ". </div>";
     $('#FeedbackMassage').html(massage);
}

//**ajax call for fill dropdownlist 
//filterInput Parameter
//Url Post url
//targetId Target element Id
function GetDropDownData(input, url, targetId, defaultValue, withdefaultValue) {
     var target = $('#' + targetId);
     $.ajax({
          url: url,
          type: "POST",
          traditional: true,
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               target.empty();
               if (withdefaultValue)
                    target.append("<option value='" + 0 + "'>" + defaultValue + " </option>");
               $(data).each(function () {
                    $(document.createElement('option'))
		    .attr('value', this.Value)
		    .text(this.Text)
		    .appendTo(target);
               });
               return;
          },
          error: function () {
               return;
          }
     });
}

//Get data for Chosen dropdownlist
function GetChosenDropDownData(input, url, targetId, defaultValue, withdefaultValue) {
     var target = $('#' + targetId);
     $.ajax({
          url: url,
          type: "POST",
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               target.empty();
               if (withdefaultValue)
                    target.append("<option value='" + 0 + "'>" + defaultValue + " </option>");
               target.trigger("liszt:updated");
               $(data).each(function () {
                    target.append("<option value='" + this.Value + "'>" + this.Text + "</option>");
                    target.trigger("liszt:updated");
               });
               return;
          },
          error: function () {
               return;
          }
     });
}

//Get checkbox data by ajax
function GetCheckBoxData(input, url, targetId) {
     var target = $('#' + targetId);
     $.ajax({
          url: url,
          type: "POST",
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               target.prop('checked', data);
               return;
          },
          error: function () {
               return;
          }
     });
}

//Get partial view and show in target by targetId
function GetPartialView(input, url, targetId) {
     var target = $('#' + targetId);
     $.ajax({
          url: url,
          type: "POST",
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               target.html(data);
               $("[Chosen = 'Chosen']").chosen({ rtl: true });
               $(".alert alert-info").alert();
               return;
          },
          error: function () {
               return;
          }
     });
}

//Get partial view add show as dialog
function GetPartialViewDialog(input, url, targetId, dialogId) {
     var target = $('#' + targetId);
     $.ajax({
          url: url,
          type: "POST",
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               target.html(data);
               $('#' + dialogId).modal('show');
               return;
          },
          error: function () {
               return;
          }
     });
}



//Ajax Call and return to callbackfunction
function AjaxCall(input, url, callbackfunction, extraPrem) {
     $.ajax({
          url: url,
          type: "POST",
          traditional: true,
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               window.myFunction = window[callbackfunction](data, extraPrem);
               return;
          },
          error: function () {
               return;
          }
     });
}
//Get Data For Input by ajax
function GetDataForInput(input, url, targetId) {
     var target = $('#' + targetId);
     $.ajax({
          url: url,
          type: "POST",
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               target.val(data);
               return data;
          },
          error: function () {
               return;
          }
     });
}
///get Datetime by ajax
function GetDateForInput(input, url, targetId) {
     var target = $('#' + targetId);
     $.ajax({
          url: url,
          type: "POST",
          data: JSON.stringify(input),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data) {
               var value = new Date(parseInt(data.replace("/Date(", "").replace(")/", ""), 10));
               target.val($.datepicker.formatDate('dd/mm/yy', new Date(value)));
               return;
          },
          error: function () {
               return;
          }
     });
}


///Renew Record
function ReNewRecord(postbackurl,input, url, lan, dialogId) {
   
    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(input),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data) {
               // $('#' + dialogId).dialog('close');
                //$('#' + dialogId).modal('hide');
            
                window.location.reload(postbackurl);
                $('.modal.in').modal('hide');
           }
        },
        error: function () {
            return;
        }
    });
}

//Ban Record
function BanRecord(postbackurl, input, url, lan, dialogId) {

    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(input),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data) {
                window.location.reload(postbackurl);
                $('#' + dialogId).modal('hide');
               
            }
        },
        error: function () {
            return;
        }
    });
}





///Show Report in
function ShowReport(input, url, report, lan,progress) {
     $.ajax({
          url: url,
          data: JSON.stringify(input),
          type: 'POST',
          contentType: 'application/json;',
          dataType: 'json',
          success: function (data) {
              
               if (data == true) {
                    window.open("/ReportView/ViewReport?strReportName=" + report, 'mywindow', 'fullscreen=yes, scrollbars=auto');
               } else {
                    if (lan == "ar") {
                         ShowFeedback("لا توجد بيانات لعرضها", "خطأ", "Error");
                    } else {
                         ShowFeedback("No Data To show", "Error", "Error");
                    }
               }
            //   progress.progressTimer('complete');
          }
     });
}
///Show Report in 1111
function ShowReport(input, url, report, lan) {
    $.ajax({
        url: url,
        data: JSON.stringify(input),
        type: 'POST',
        contentType: 'application/json;',
        dataType: 'json',
        success: function (data) {

            if (data == true) {
                window.open("/ReportView/ViewReport?strReportName=" + report, 'mywindow', 'fullscreen=yes, scrollbars=auto');
            } else {
                if (lan == "ar") {
                    ShowFeedback("لا توجد بيانات لعرضها", "خطأ", "Error");
                } else {
                    ShowFeedback("No Data To show", "Error", "Error");
                }
            }

        }
    });
}
//show report by id 
function ShowReportByIdWithAction(input, url, report, id) {
     $.ajax({
          url: url,
          data: JSON.stringify(input),
          type: 'POST',
          contentType: 'application/json;',
          dataType: 'json',
          success: function (data) {
               if (data == true) {
                    window.open("/ReportView/ViewReport?strReportName=" + report + "&&id=" + id, 'mywindow', 'fullscreen=yes, scrollbars=auto');
               }
          }
     });
}
function ShowReportById(report, id) {
     window.open("/ReportView/ViewReport?strReportName=" + report + "&&id=" + id, 'mywindow', 'fullscreen=yes, scrollbars=auto');
}

//Loading bar in ajax call
//$(document).ready(function () {
//    $.ajaxSetup({
//        'beforeSend': function () {
//            $("#ProgressBar").progressbar({
//                value: false,
//            });
//        },
//        'complete': function () {
//            $("#ProgressBar").progressbar({
//                value: false,
//            });
//        }
//    });
//});