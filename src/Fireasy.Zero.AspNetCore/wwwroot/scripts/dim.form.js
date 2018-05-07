var currDim;
var copyRow = null;

function dim(dg, fields) {
    var editIndex = -1; //当前编辑的行索引号
    var editFields = [];
    var editCellIndex = -1;
    //var datasource = {};
    var txtFields = {};
    var selectedIndex = -1;
    this.dg = dg;
    this.options = {};
    this.fields = fields;
    this.count = 10; //初始数据行
    this.validations = {};
    this.validateField = null;
    this.checkRepeatFields = []; //检查重复的字段
    this.editing = true;
    this.contextMenu = true;
    this.initEditor = function () { };
    this.valueChange = function () { };
    var _dim = this;

    //初始化grid
    this.init = function (url) {
        var fields = [];
        $.each(this.fields, function (i, r) {
            r.halign = 'center';
            if (r.editor != undefined) {
                //如果是下拉框，需要进行格式化，使用_text文本来显示
                if (r.editor.type == 'combobox' || r.editor.type == 'combotree' || r.editor.type == 'combogrid') {
                    if (r.txtField != undefined) {
                        txtFields[r.field] = r.txtField;
                    }
                    else {
                        txtFields[r.field] = r.field + "_text";
                    }

                    if (r.formatter == undefined) {
                        r.formatter = function (value, row) {
                            if (row[txtFields[r.field]] != undefined) {
                                return row[txtFields[r.field]];
                            }
                        }
                    }
                }

                //验证
                if (_dim.validations != undefined && _dim.validations[r.field] != undefined) {
                    if (r.editor.options == undefined) {
                        r.editor.options = {};
                    }

                    $.each(_dim.validations[r.field], function (j, v) {
                        if (v == "required") {
                            r.editor.options.required = true;
                        }
                        else {
                            if (r.editor.options.validType == undefined) {
                                r.editor.options.validType = [];
                            }

                            r.editor.options.validType.push(v);
                        }
                    })
                }
            }

            fields.push(r);
        });

        var options = $.extend({
            fit: true,
            url: url,
            columns: [fields],
            singleSelect: true,
            rownumbers: true,
            onDblClickCell: function (rowIndex, field, value) {
                _dim.beginEdit(rowIndex, field);
            },
            onClickCell: function (rowIndex, field, value) {
                if (selectedIndex == rowIndex) {
                    _dim.beginEdit(rowIndex, field);
                }
                else {
                    _dim.endEdit();
                }
            },
            onClickRow: function (rowIndex) {
                selectedIndex = rowIndex;
            },
            onLoadSuccess: function () {
                if (_dim.editing && _dim.count > 0) {
                    _dim.beginEdit(0, fields[0].field);
                }
            },
            onRowContextMenu: function (e, rowIndex, rowData) {
                _dim.showContextMenu(e, rowIndex, rowData);
                currDim = _dim;
            }
        }, this.options);

        $(this.dg).datagrid(options);

        editIndex = -1;
        selectedIndex = -1;

        if (url == undefined) {
            $(this.dg).datagrid('loadData', initData(this.count, this.fields));
        }

        initContextMenu();
    }

    //获取最终数据
    this.getData = function () {
        this.endEdit();
        var rows = $(this.dg).datagrid('getRows');
        var result = [];

        var vf = this.validateField == null ? this.fields[0].field : this.validateField;

        for (var i = 0; i < rows.length; i++) {
            var r = rows[i];
            if (r[vf] == '' || r[vf] == null) {
                continue;
            }

            if (!_dim.checkRepeat(rows, r, i)) {
                return [];
            }

            result.push(rows[i]);
        }

        if (result.length == 0) {
            common.alert('你还没有录入数据!');
        }

        return result;
    }

    this.checkRepeat = function (rows, r, index) {
        if (this.checkRepeatFields.length > 0) {
            var rdata = getRepeatData(r);
            var rs = [];

            for (var i = 0; i < rows.length; i++) {
                if (i == index) {
                    continue;
                }

                if (getRepeatData(rows[i]) == rdata) {
                    if (rs.length == 0) {
                        rs.push(index + 1);
                    }
                    rs.push(i + 1);
                }
            }

            if (rs.length > 0) {
                this.markRepeatRows(rs);
                common.alert('第' + rs + '行数据中' + getRepeatFileName() + '有重复。');
                return false;
            }
        }

        return true;
    }

    this.markRepeatRows = function (rows) {
        for (var i = 0; i < rows.length; i++) {
            $(_dim.dg).prev().find('tr[datagrid-row-index="' + (rows[i] - 1) + '"] td').css('background', '#f1bfb2');
        }
    }

    this.getEditor = function (field) {
        var editor = $(_dim.dg).datagrid('getEditor', { index: editIndex, field: field });
        if (editor != null) {
            return $(editor.target);
        }

        return null;
    }

    this.getEditIndex = function () {
        return editIndex;
    }

    this.getTxtField = function (index) {
        return txtFields[_dim.fields[index].field];
    }

    this.changeEditorType = function (field, type) {
        var editor = $(_dim.dg).datagrid('getEditor', { index: editIndex, field: field });
        if (editor != null) {
            editor.type = type;
        }
    }

    this.clear = function () {
        this.cancelEdit();

        editIndex = -1;
        selectedIndex = -1;

        $(this.dg).datagrid('loadData', initData(this.count, this.fields));
    }

    //取消编辑
    this.cancelEdit = function () {
        if (editIndex != -1) {
            $(this.dg).datagrid('cancelEdit', editIndex);
            editIndex = -1;
        }
    }

    this.newRow = function () {
        var row = new Object();
        $.each(fields, function (j, r) {
            row[r.field] = r.initValue == undefined ? '' : r.initValue;
        });
        return row;
    }

    //初始化数据
    initData = function (count, fields) {
        var data = [];

        for (var i = 0; i < count; i++) {
            data.push(_dim.newRow());
        }

        return data;
    }

    //开始单元格编辑
    this.beginEdit = function (index, field) {
        if (editIndex != index) {
            if (!$(this.dg).datagrid('validateRow', editIndex))
                return;
            $(this.dg).datagrid('endEdit', editIndex);

            $(this.dg).datagrid("beginEdit", index);
            editIndex = index;

            $(this.dg).datagrid('selectRow', editIndex);

            bindGridEnter(field);
        }
    }

    //关闭单元格编辑
    this.endEdit = function () {
        if (editIndex != -1) {
            if (!$(this.dg).datagrid('validateRow', editIndex))
                return false;

            $(this.dg).datagrid('endEdit', editIndex);
            editIndex = -1;
        }

        return true;
    }

    var getRepeatData = function (row) {
        var str = '';
        for (var i = 0; i < _dim.checkRepeatFields.length; i++) {
            str += row[_dim.checkRepeatFields[i]] + ':';
        }

        return str;
    }

    var getRepeatFileName = function () {
        var str = '';
        for (var i = 0; i < _dim.checkRepeatFields.length; i++) {
            for (var j = 0; j < _dim.fields.length; j++) {
                if (_dim.checkRepeatFields[i] == _dim.fields[j].field) {
                    if (str.length > 0) {
                        str += '、';
                    }
                    str += _dim.fields[j].title;
                }
            }
        }

        return str;
    }

    var initEditFields = function () {
        if (editFields.length == 0) {
            for (var i = 0; i < _dim.fields.length; i++) {
                var editor = $(_dim.dg).datagrid('getEditor', { index: editIndex, field: _dim.fields[i].field })
                editFields.push({ field: _dim.fields[i].field, editor: editor != null });
            }
        }
    }

    //通过字段找到其索引
    var findFieldIndex = function (field) {
        for (var i = 0; i < editFields.length; i++) {
            if (editFields[i].field == field) {
                return i;
            }
        }

        return -1;
    }

    //通过编辑器索引找到字段名称(编辑器的个数有可能少于字段的个数，因此两个索引不会一一对应)
    var findEditorField = function (index) {
        var j = 0;
        for (var i = 0; i < editFields.length; i++) {
            if (editFields[i].editor == false) {
                continue;
            }

            if (index == j) {
                return editFields[i].field;
            }

            j++;
        }
    }

    //绑定表格事件
    var bindGridEnter = function (field) {
        initEditFields();
        editCellIndex = findFieldIndex(field);

        if (editCellIndex == -1) {
            return;
        }

        var editors = $(_dim.dg).datagrid('getEditors', editIndex);
        var row = $(_dim.dg).datagrid('getRows')[editIndex];

        for (var i = 0; i < editors.length; i++) {
            var target = $(editors[i].target);

            var efield = findEditorField(i)

            if (_dim.initEditor != undefined) {
                _dim.initEditor(row, efield, target);
            }

            if (editors[i].type == 'combotree') {
                target.data('field', efield);
                target.combotree('tree').data('field', efield);
                target.combotree('tree').data('parent', target);
                processCombotree(target, efield);
            }
            else if (editors[i].type == 'combobox') {
                target.data('field', efield);
                processCombobox(target, efield);
            }
            else if (editors[i].type == 'combogrid') {
                target.data('field', efield);
                target.combogrid('grid').data('field', efield);
                target.combogrid('grid').data('parent', target);
                processCombogrid(target);
            }
            else if (editors[i].type == 'textbox' ||
                editors[i].type == 'numberbox' ||
                editors[i].type == 'datebox') {
                var textbox = target.textbox('textbox');
                textbox.keyup(function (event) {
                    if (event.keyCode == 13) {
                        gotoNextEditor();
                    }
                });
            }
        }

        //当前编辑器选中
        //var current = editors[editCellIndex];

        var current = $(_dim.dg).datagrid('getEditor', { index: editIndex, field: field });
        if (current != null) {
            if (current.type == 'combobox') {
                $(current.target).combobox('showPanel');
            } else if (current.type == 'combotree') {
                $(current.target).combotree('showPanel');
            } else {
                $(current.target).textbox('textbox').focus().select();
            }
        }
    }

    var processCombotree = function (target, efield) {
        var onLoadSuccess = target.combotree('options').onLoadSuccess;
        var onSelect = target.combotree('options').onSelect;

        target.combotree({
            onSelect: function (record) {
                var row = $(_dim.dg).datagrid('getRows')[editIndex];

                var field = $(this).data('field');

                //row[field] = record.id;
                row[txtFields[field]] = record.text;

                attachField(field, row, record);

                if (_dim.valueChange != undefined) {
                    _dim.valueChange(record.id, row, field);
                }

                if (onSelect) onSelect.call(this, record);
                gotoNextEditor();
            },
            onLoadSuccess: function (node, data) {
                //加载完成后，选中
                var row = $(_dim.dg).datagrid('getRows')[editIndex];
                var field = $(this).data('field');
                var parent = $(this).data('parent');
                var url = parent.combotree('options').url;

                if (row[field] != undefined && row[field] != null && row[field] != '') {
                    parent.combotree('setValue', row[field]);
                    row[txtFields[field]] = parent.combotree('getText');
                }
                if (onLoadSuccess) onLoadSuccess.call(this, node, data);
            }
        });
    }

    var processCombobox = function (target, efield) {
        var onLoadSuccess = target.combobox('options').onLoadSuccess;
        var onSelect = target.combobox('options').onSelect;

        target.combobox({
            onSelect: function (record) {
                var options = $(this).combobox('options');
                var text = record[options.textField];
                var value = record[options.valueField];

                var row = $(_dim.dg).datagrid('getRows')[editIndex];
                var field = $(this).data('field');
                //row[field] = value;
                row[txtFields[field]] = text;

                attachField(field, row, record);

                if (_dim.valueChange != undefined) {
                    _dim.valueChange(value, row, field);
                }

                if (onSelect) onSelect.call(this, record);
                gotoNextEditor();
            },
            onLoadSuccess: function (data) {
                //加载完成后，选中
                var row = $(_dim.dg).datagrid('getRows')[editIndex];
                var field = $(this).data('field');
                var options = $(this).combobox('options');
                var url = options.url;

                if (row[field] != undefined && row[field] != null && row[field] != '') {
                    if (!options.multiple) {
                        $(this).combobox('setValue', row[field]);
                    }
                    else {
                        $(this).combobox('setValues', row[field]);
                    }
                    row[txtFields[field]] = $(this).combobox('getText');
                }
                if (onLoadSuccess) onLoadSuccess.call(this, data);
            }
        });
    }

    var processCombogrid = function (target) {
        var onLoadSuccess = target.combogrid('options').onLoadSuccess;
        var onSelect = target.combogrid('options').onSelect;

        target.combogrid({
            onSelect: function (index, row) {
                var parent = $(this).data('parent');
                var options = parent.combogrid('options');
                var text = row[options.textField];
                var value = row[options.idField];

                var dgRow = $(_dim.dg).datagrid('getRows')[editIndex];
                var field = $(this).data('field');
                //dgRow[field] = value;
                dgRow[txtFields[field]] = text;

                attachField(field, dgRow, row);

                if (_dim.valueChange != undefined) {
                    _dim.valueChange(value, row, field);
                }

                if (onSelect) onSelect.call(this, index, row);
                gotoNextEditor();
            },
            onLoadSuccess: function (data) {
                //加载完成后，选中
                var row = $(_dim.dg).datagrid('getRows')[editIndex];
                var field = $(this).data('field');

                if (row[field] != undefined && row[field] != null && row[field] != '') {
                    $(this).combogrid('setValue', row[field]);
                    $(this).combogrid('setText', row[txtFields[field]]);
                }

                if (onLoadSuccess) onLoadSuccess.call(this, data);
            }
        })
    }

    var attachField = function (field, target, source) {
        for (var i = 0; i < _dim.fields.length; i++) {
            if (_dim.fields[i].from == field) {
                var editor = $(_dim.dg).datagrid('getEditor', { index: editIndex, field: _dim.fields[i].field });

                target[_dim.fields[i].field] = source[_dim.fields[i].refField];

                if (editor != null) {
                    $(editor.target).textbox('setValue', source[_dim.fields[i].refField]);
                }
                else {
                    var r = $(_dim.dg).prev().find('tr[datagrid-row-index=' + editIndex + '] td[field="' + _dim.fields[i].field + '"] div');
                    if (r.length > 0) {
                        r.text(source[_dim.fields[i].refField]);
                    }
                }
            }
        }
    }

    var gotoNextEditor = function () {
        return;
        editCellIndex++;
        var editors = $(_dim.dg).datagrid('getEditors', editIndex);
        if (editCellIndex >= editors.length) {
            $(_dim.dg).datagrid('endEdit', editIndex);

            editIndex++;
            $(_dim.dg).datagrid("beginEdit", editIndex);

            bindGridEnter(field);
        }
        else {
            var nextEditor = editors[editCellIndex];

            if (nextEditor.type == 'combotree') {

            }
            else {
            }

            bindGridEnter()
        }
    }
    //显示右键菜单
    this.showContextMenu = function (e, rowIndex, rowData) {
        if (rowIndex == -1 || _dim.contextMenu == false) {
            return;
        }
        $(_dim.dg).datagrid('selectRow', rowIndex);
        e.preventDefault();
        $('#menu').menu('show', { left: e.pageX, top: e.pageY });
    }
}

$(function () {
    initContextMenu();
})

//初始化右键菜单
var initContextMenu = function () {
    $('body').append('<div id="dialog" style="padding:20px">行数: <input id="txtNum" type="input" style="width:80px" value="1" />&nbsp;&nbsp;<a id="btnOk" onclick="pasterMulti()">确定</a></div>')
        .append('<div id="menu">' +
        '<div data-options="name:\'copy\'">复制整行数据</div>' +
        '<div class="menu-sep"></div>' +
        '<div data-options="name:\'paster\'">粘贴一行数据</div>' +
        '<div data-options="name:\'paster_multi\'">粘贴多行数据</div>' +
        '<div class="menu-sep"></div>' +
        '<div data-options="name:\'insert\'">新增10行</div>' +
        '<div data-options="name:\'clear\'">清除整行数据</div>' +
        '<div data-options="name:\'deleteRow\'">删除当前行</div>' +
        '<div data-options="name:\'cancelEdit\'">取消编辑 (ESC)</div>' +
        '</div>');
    $('#dialog').window({
        title: '输入行数',
        width: 300,
        height: 150,
        closed: true,
        cache: false,
        modal: true,
        minimizable: false,
        maximizable: false,
        collapsible: false
    });
    $('#txtNum').numberbox({});
    $('#btnOk').linkbutton({});
    $('#menu').menu({
        hideOnUnhover: false,
        onClick: function (item) {
            switch (item.name) {
                case 'copy':
                    if (!$(currDim.dg).datagrid('validateRow', currDim.getEditIndex())) {
                        common.alert('数据没有填写完整。');
                        return;
                    }
                    $(currDim.dg).datagrid('endEdit', currDim.getEditIndex());
                    copyRow = $(currDim.dg).datagrid('getSelected');
                    break;
                case 'paster':
                    if (copyRow == null) {
                        common.alert('没有复制数据行。');
                        return;
                    }
                    var row = $(currDim.dg).datagrid('getSelected');
                    var index = $(currDim.dg).datagrid('getRowIndex', row);
                    currDim.cancelEdit();
                    var pasterRow = createPasterRow(null)
                    $(currDim.dg).datagrid('updateRow', { index: index, row: pasterRow });
                    break;
                case 'paster_multi':
                    if (copyRow == null) {
                        common.alert('没有复制数据行。');
                        return;
                    }
                    $('#dialog').window('move', { left: ($('body').width() - 300) / 2, top: ($('body').height - 150) / 2 });
                    $('#dialog').window('open');
                    break;
                case 'insert':
                    for (var i = 0; i < 10; i++) {
                        var obj = {};
                        for (var j = 0; j < currDim.fields.length; j++) {
                            obj[currDim.fields[j].field] = '';
                        }

                        obj._random = Math.random();

                        $(currDim.dg).datagrid('appendRow', obj);
                    }
                    break;
                case 'clear':
                    var row = $(currDim.dg).datagrid('getSelected');
                    var index = $(currDim.dg).datagrid('getRowIndex', row);
                    if (index == currDim.getEditIndex()) {
                        currDim.cancelEdit();
                    }
                    for (var i = 0; i < currDim.fields.length; i++) {
                        row[currDim.fields[i].field] = '';
                        var txtField = currDim.getTxtField(i);
                        if (txtField != undefined) {
                            row[txtField] = '';
                        }
                    }
                    $(currDim.dg).datagrid('updateRow', { index: index, row: row });
                    break;
                case 'deleteRow':
                    var row = $(currDim.dg).datagrid('getSelected');
                    var index = $(currDim.dg).datagrid('getRowIndex', row);
                    if (index == currDim.getEditIndex()) {
                        currDim.cancelEdit();
                    }
                    $(currDim.dg).datagrid('deleteRow', index);
                    break;
                case 'cancelEdit':
                    currDim.cancelEdit();
                    break;
            }
        }
    });
}

var createPasterRow = function (row) {
    var pasterRow = {};
    var source = row != null ? row : copyRow;
    for (var i = 0; i < currDim.fields.length; i++) {
        pasterRow[currDim.fields[i].field] = source[currDim.fields[i].field];
        var txtField = currDim.getTxtField(i);

        if (txtField != undefined && source[txtField] != undefined) {
            pasterRow[txtField] = source[txtField];
        }
    }

    pasterRow._random = Math.random();

    return pasterRow;
}

var pasterMulti = function () {
    var num = parseInt($('#txtNum').numberbox('getValue'));
    $('#dialog').window('close');
    var rows = $(currDim.dg).datagrid('getRows');
    var row = $(currDim.dg).datagrid('getSelected');
    var index = $(currDim.dg).datagrid('getRowIndex', row);
    currDim.cancelEdit();
    for (var i = 0; i < num; i++) {
        if (index + i <= rows.length - 1) {
            $(currDim.dg).datagrid('updateRow', { index: index + i, row: createPasterRow(null) });
        }
        else {
            $(currDim.dg).datagrid('appendRow', createPasterRow(null));
        }
    }
}

