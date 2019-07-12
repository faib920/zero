(function ($) {
    $.fn.upload = function (options) {
        options = $.extend({
            name: 'file',
            action: '',
            enctype: 'multipart/form-data',
            multiple: false,
            extFilter: null,
            extExclude: 'bat;exe;msi;html;htm;js',
            autoSubmit: true,
            onMakeAction: function () { },
            onSubmit: function (index, element) { },
            onError: function (result, index, element) { },
            onComplete: function (result, index, element) { },
            onSelect: function (index, element) { return true; },
            onFileExtError: function (file, ext, index, element) { },
            useHttpModule: true,
            readPercentUrl: 'sys.ajax/ReadUploadStatus',
            sizeLimit: 40960 //大小限制 kb
        }, options);

        return new $._upload(this, options);
    },

	$._upload = function (element, options) {
	    var self = this;

	    element.each(function (i, e) {
	        var id = Math.random();
	        e = $(e);
	        if (e.attr('sid')) {
	            return;
	        }

	        var input = $('<input id="file' + id + '" name="file' + id + '" type="file" style="cursor:pointer"' +
                (options.multiple ? ' multiple="true"' : '') + ' />');
	        input.bind('change', function () {
	            if ($(this).val() == '') {
	                return;
	            }
	            if (options.autoSubmit && options.onSelect(i, e)) {
	                self.submit(i, e);
	            }
	        });

	        var iframe = $('<iframe sid="' + id + '" id="iframe' + id + '" name="iframe' + id + '"></iframe>')
                .css({ display: 'none' });

	        var loadcallback = function () {
	            var myFrame = document.getElementById('iframe' + id);
	            if (myFrame) {
	                input.val('');
	                var response = null;
	                
	                if (myFrame.contentWindow && myFrame.contentWindow.document.body) {
	                    //response = myFrame.contentWindow.document.body.innerText;   delete by yzk
	                    response = myFrame.contentWindow.document.body.textContent;
	                } else if (myFrame.contentDocument && myFrame.contentDocument.document.body) {
	                    //response = myFrame.contentDocument.document.body.innerText; delete by yzk
	                    response = myFrame.contentWindow.document.body.textContent;
	                }
	                if (response != '') {
	                    if (options.useHttpModule) {
	                        self.hideProgress(i, e);
	                    }
	                    try {
	                        var result = JSON.parse(response);
	                        if ($.isArray(result)) {
	                            options.onComplete(result, i, e);
	                        }
	                        else if (result.succeed && result.succeed == true) {
	                            options.onComplete(result.data, i, e);
	                        }
	                        else {
	                            options.onError(result, i, e);
	                        }
	                    }
	                    catch (e1) {
	                        options.onError(e1, i, e);
	                    }
	                }
	            }
	        }

	        if (window.attachEvent) {
	            iframe[0].attachEvent('onload', loadcallback);
	        }
	        else {
	            iframe[0].addEventListener('load', loadcallback);
	        }

	        var form = $(
				'<form ' +
                    'sid="' + id + '" ' +
					'method="post" ' +
					'enctype="' + options.enctype + '" ' +
					'action="' + options.action + '" ' +
					'target="iframe' + id + '" ' +
					'style="position:absolute;top:0px;left:0px;opacity:0;filter:alpha(opacity=0);"' +
				'></form>'
			).css({
			    margin: 0,
			    padding: 0
			}).append(input);

	        form.append(input);
	        input.prop('title', '点击上传').css({ 'overflow': 'hidden', 'width': e.width() + 'px', height: e.height() + 'px' });
	        e.attr('sid', id).css({ 'position': 'relative' })
				.append(form)
				.after(iframe);
	    });

	    $.extend(this, {
	        timers: [],
	        submit: function (index, element) {
	            if (!this.checkExt(index, element)) {
	                return;
	            }

	            var form = $('form', element);

	            var action = options.onMakeAction(index, element);
	            if (action != undefined) {
	                form.attr('action', action);
	            }

	            if (options.useHttpModule) {
	                action = form.attr('action');
	                if (action.indexOf('_sid=') == -1) {
	                    form.attr('action', action + (action.indexOf('?') == -1 ? '?' : '&') + '_sid=' + form.attr('sid') + '&_size=' + options.sizeLimit);
	                }
	            }

	            options.onSubmit(index, element);

	            if (options.useHttpModule) {
	                this.showProgress(index, element);
	            }

	            form[0].submit();
	        },
	        checkExt: function (index, element) {
	            var file = $('input[type="file"]', element).val();
	            var ext = file.toLowerCase().split('.').pop();

	            if (options.extExclude != null) {
	                var extList = options.extExclude.toLowerCase().split(';');
	                if ($.inArray(ext, extList) >= 0) {
	                    options.onFileExtError(1, file, options.extExclude, index, element);
	                    return false;
	                }
	            }

	            if (options.extFilter == null) {
	                return true;
	            }

	            if (options.extFilter != '') {
	                var extList = options.extFilter.toLowerCase().split(';');

	                if ($.inArray(ext, extList) < 0) {
	                    options.onFileExtError(0, file, options.extFilter, index, element);
	                    return false;
	                }
	            }

	            return true;
	        },
	        showProgress: function (index, element) {
	            var p = $('#pg_' + index);
	            var sid = element.attr('sid');
	            if (p.length == 0) {
	                p = $('<div id="pg_' + index + '" style="z-index:10001;position:absolute;border:1px solid #888888;height:6px;background:#ffffff;width:100px;display:none"><div style="width:0%;background:#0099ff;height:6px;" /></div>')
	                p.css({ 'left': element.width() + 5, 'top': (element.height() - p.height()) / 2 });
	                element.parent().append(p);
	            }

	            p.attr('title', '0%');
	            $('#pg_' + index + ' div').width('0%');
	            p.show();
	            var __self = this;
	            $('form', element).hide();
	            this.timers[index] = setInterval(function () {
	                $.getJSON(options.readPercentUrl + '?sessionId=' + sid, function (pre) {
	                    if (pre) {
	                        $('#pg_' + index + ' div').width(pre + '%');

	                        if (pre >= 100) {
	                            __self.hideProgress(index, element);
	                        }
	                    }
	                });
	            }, 200);
	        },
	        hideProgress: function (index, element) {
	            var p = $('#pg_' + index);
	            clearInterval(this.timers[index]);
	            p.hide();
	            $('form', element).show();
	        }
	    });
	}
})(jQuery);