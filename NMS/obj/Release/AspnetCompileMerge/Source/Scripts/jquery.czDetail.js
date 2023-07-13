//Title: Cozeit Detail plugin by Yasir Atabani
//Documentation: na
//Author: Yasir O. Atabani
//Website: http://www.cozeit.com
//Twitter: @yatabani
//
//Version 1.0.0 Jun 22nd 2013 -First Release.
(function ($, undefined) {
    $.fn.czDetail = function (options) {

        //Set defauls for the control
        var defaults = {
            max: 5,
            min: 0,
            url: null,
            onLoad: null,
            onAdd: null,
            arg: null
        };
        //Update unset options with defaults if needed
        var options = $.extend(defaults, options);
        $(this).bind("onAdd", function (event, data) {
            options.onAdd.call(event, data);
        });
        $(this).bind("onLoad", function (event, data) {
            options.onLoad.call(event, data);
        });
        //Executing functionality on all selected elements
        return this.each(function () {
            //table object
            var obj = $(this).children();;
            //number of records
            var i = obj.children("tr").size() - 1;
            var divExpand = '<div id="btnExpand" />';
            var divContract = '<div id="btnContract" />';
            var count = '<input id="czDetail_txtCount" name="czDetail_txtCount" type="hidden" size="5" />';
            var detailTd = '<td id="last"></td>';
            obj.children("tr").each(function (index, element) {
                $(element).append(detailTd);
                $(element).children("#last").html(divExpand);
                var columns = $(element).children("td").size();
                var tr = '<tr id="details" style="display:none;"><td colspan="' + columns + '"></td></tr>';
                $(element).after(tr);

                var _recordId = $(element).attr('data-recordId');
                if (_recordId != null && _recordId != "") {
                    var btnExpand = $(element).children("#last").children("#btnExpand");

                    btnExpand.css({
                        'float': 'right',
                        'border': '0px',
                        'background-image': 'url("/Content/Images/icon-arrow-down.png")',
                        'background-position': 'top left',
                        'background-repeat': 'no-repeat',
                        'height': '26px',
                        'width': '26px',
                        'cursor': 'pointer'
                    });

                    btnExpand.hover(
                        function () {
                            btnExpand.css({
                                'background-position': 'bottom left'
                            });
                        },
                       function () {
                           btnExpand.css({
                               'background-position': 'top left'
                           });
                       });

                    btnExpand.click(function () {
                        var _id = $(element).attr('data-id');
                        var _url = options.url;
                        var _arg = options.arg;
                        if (_id == 1) {

                            btnExpand.css({
                                'background-image': 'url("/Content/Images/icon-arrow-down.png")',
                                'background-position': 'top left'
                            });
                            $(element).next("#details").hide();
                            $(element).attr('data-id', '0');
                        }
                        else if (_id == 0) {

                            btnExpand.css({
                                'background-image': 'url("/Content/Images/icon-arrow-up.png")',
                                'background-position': 'top left'
                            });
                            $(element).next("#details").show();
                            $(element).attr('data-id', '1');
                        } else {
                            var input = { recordId: _recordId, arg: _arg };
                            $.ajax({
                                url: _url,
                                type: "POST",
                                data: JSON.stringify(input),
                                dataType: "json",
                                contentType: "application/json; charset=utf-8",
                                success: function (data) {
                                    //alert(data);

                                    btnExpand.css({
                                        'background-image': 'url("/Content/Images/icon-arrow-up.png")',
                                        'background-position': 'top left'
                                    });
                                    $(element).next("#details").show();
                                    $(element).attr('data-id', '1');
                                    $(element).next("#details").children("td").append(data);
                                    return;
                                },
                                error: function () {
                                    $(element).next("#details").children("td").append("there was an error retreiving the data.");
                                }
                            });
                        }
                    });
                }
            });
            //obj.before(count);
            //var set = recordset.children(".recordset").children().first();
            //set.before(divContract);


            //if (recordset.length) {
            //    obj.siblings("#btnExpand").click(function () {
            //        var item = recordset.html();
            //        item = item.replace(/\[0\]/g, "[" + i + "]");
            //        item = item.replace(/\_0\_/g, "_" + i + "_");
            //        //item = $(item).children().first();
            //        //item = $(item).parent();

            //        var btnMinus = set.siblings("#btnMinus");
            //        btnMinus.css({
            //            'float': 'right',
            //            'border': '0px',
            //            'background-image': 'url("/Content/Images/remove.png")',
            //            'background-position': 'center center',
            //            'background-repeat': 'no-repeat',
            //            'height': '25px',
            //            'width': '25px',
            //            'cursor': 'pointer'
            //        });

            //        set.siblings("#btnMinus").click(function () {
            //            set.parent().remove();
            //        });

            //    obj.append(item);
            //    if (options.onAdd != null) {
            //        obj.trigger("onAdd", i);
            //    }
            //    obj.siblings("#czDetail_txtCount").val(i);
            //    i++;
            //    return false;
            //});
            //recordset.remove();

            //if (options.onLoad != null) {
            //    obj.trigger("onLoad", i);
            //}
            //obj.bind("onAdd", function (event, data) {
            //If you had passed anything in your trigger function, you can grab it using the second parameter in the callback function.
            //});
            //}
        });
    };
})(jQuery);