var Node = function (type = -1, key = null, data = null) {
    this.Type = type;
    this.Key = key;
    this.Data = data;
};
$.extend(Node.prototype, {
    toJson: function () {
        return JSON.stringify(this);
    },
    toObject: function (obj) {
        var temp = JSON.parse(obj);
        temp.fn = Node.fn;
        return temp;
    }
});
/**
 * common
 */
var ins = (function (obj) {
    $(obj.onLoad);
    return obj;
})({
    page_move: true,
    beforeunload_msg: "Are you sure you want to navigate away from this page?\n"
    + "You have started writing or editing a post.\n"
    + "Press OK to continue or Cancel to stay on the current page.",
    onLoad: function () {
        var ws = new WebSocket(wsurl);
        ws.onopen = function () {
            ins.callMethod(navi, "init");
        };
        ws.onclose = function () {
            ins.errorPopup("Connection closed.");
        };
        ws.onerror = function () {
            ins.errorPopup("Socket error.");
        };
        ws.onmessage = function (message) {
            ins.message(ws, message);
            $(".loading").hide();
        };
        ins.send = function (message) {
            ws.send(message);
            $(".loading").show();
        };
        $(window).on("beforeunload", function (e) {
            if (!ins.page_move) {
                e = e || window.event;
                if (e) {
                    e.returnValue = ins.beforeunload_msg;
                }
                return msg;
            }
        });
    },
    message: function (ws, message) {
        var temp = JSON.parse(message.data);
        ins.callMethod(navi, temp.Key, temp);
        if (temp.Type === 1 && temp.ResponseKey !== undefined && temp.ResponseKey !== null) {
            var node = new Node(2, temp.ResponseKey, temp.ResponseData);
            ins.send(node.toJson());
        }
    },
    loadPage: function (name, param) {
        if (!ins.page_move) {
            if (!confirm(ins.beforeunload_msg)) {
                return false;
            }
            ins.page_move = true;
        }
        var node = new Node(1, name, param);
        ins.send(node.toJson());
    },
    loadContents: function (data) {
        ins.clearContents();
        $("#contents").append(data);
    },
    clearContents: function () {
        $("#contents").html("");
    },
    errorPopup: function (msg) {
        //https://blackrockdigital.github.io/startbootstrap-sb-admin-2/pages/panels-wells.html
        ins.popup(msg, "panel-danger");
    },
    successPopup: function (msg) {
        ins.popup(msg, "panel-success");
    },
    popup: function (msg, clz) {
        var zone = $(".message-zone");
        var panel = $("<div></div>");
        panel.addClass("panel");
        panel.addClass(clz);
        panel.append($("<div></div>").addClass("panel-heading").append(msg));
        zone.append(panel);
        setTimeout(function (d) {
            d.slideUp("slow", function () {
                d.remove();
            });
        }, 3000, panel);
    },
    callMethod: function (obj, name, p1, p2, p3, p4, p5, p6, p7, p8, p9) {
        if (obj !== undefined && obj[name] !== undefined && typeof obj[name] === "function") {
            obj[name].call(this, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
    }
});

var navi = {
    init: function () {
        $(document).on("click", "#Home", function () {
            ins.loadPage("cardmenu");
        });
        $(document).on("click", "#CalendarPanel,#CalendarMenu", function () {
            ins.loadPage("cardmenu");
        });
        $(document).on("click", "#TaskPanel,#TaskMenu", function () {
            ins.loadPage("task");
        });
        $(document).on("click", "#MemoPanel,#MemoMenu", function () {
            ins.loadPage("memolist");
        });
        $(document).on("click", "#SettingsPanel,#SettingsMenu", function () {
            ins.loadPage("setting");
        });
        ins.loadPage("cardmenu");
    },
    cardmenu: function (node) {
        ins.loadContents(node.Data);
    },
    memolist: function (node) {
        ins.loadContents(node.Data);
        var temp = new Node(2, "get_memo_list");
        ins.send(temp.toJson());
    },
    memoinsert: function (node) {
        ins.loadContents(node.Data);
        $(".memo-insert#btn_save").show();
        $(".memo-insert#btn_modify").hide();
        $(".memo-insert#btn_delete").hide();
        $(".memo-insert#summernote").summernote({
            height: $(document).height() - 350,
            callbacks: {
                onKeydown: function (e) {
                    ins.page_move = false;
                }
            }
        });
    },
    setting: function (node) {
        ins.loadContents(node.Data);
        var temp = new Node(2, "get_setting");
        ins.send(temp.toJson());
    },
    task: function (node) {
        ins.loadContents(node.Data);
        var date = new Date();
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        $("#task_date").html(year + " / " + month + " / " + day);
        $("#taskdate").val(year + " / " + month + " / " + day);

        var node = new Node(2, "get_task_list");
        ins.send(node.toJson());
    },
    get_memo_list: function (node) {
        var list = Node.prototype.toObject(node.Data);
        //<button class="list-group-item memo-list-item" value="1">First item<span class="badge">2015/01/01</span></button>
        for (var i = 0; i < list.length; i++) {
            var button = $("<button></button>").addClass("list-group-item").addClass("memo-list-item").val(list[i].Idx);
            button.append(list[i].Title);
            button.append($("<span></span>").addClass("badge").append(list[i].RecentlyDate));
            $(".memo-list#list").append(button);
        }
    },
    set_memo_insert: function (node) {
        $(".memo-insert#memo_idx").val(node.Data);
        $(".memo-insert#btn_save").hide();
        $(".memo-insert#btn_modify").show();
        $(".memo-insert#btn_delete").show();
        ins.page_move = true;
        ins.successPopup("Saved.");
    },
    get_memo_item: function (node) {
        var temp = JSON.parse(node.Data);
        $(".memo-insert#memo_idx").val(temp.Idx);
        $(".memo-insert#memo_title").val(temp.Title);
        $(".memo-insert#btn_save").hide();
        $(".memo-insert#btn_modify").show();
        $(".memo-insert#btn_delete").show();
        $(".memo-insert#summernote").summernote("code", decodeURIComponent(temp.Contents));
    },
    excute_memo_delete: function (node) {
        ins.successPopup("Deleted.");
        ins.loadPage("memolist");
    },
    set_memo_modify: function (node) {
        ins.page_move = true;
        ins.successPopup("Modified.");
    },
    get_setting: function (node) {
        var obj = Node.prototype.toObject(node.Data);
        $(".setting#port").val(obj.Port);
        $(".setting#size").val(obj.Size);
        if (obj.Start === "on") {
            $(".setting#start")[0].checked = true;
        } else {
            $(".setting#start")[1].checked = true;
        }
    },
    set_setting: function (node) {
        ins.successPopup("Saved.");
    },
    set_task_insert: function (node) {
        ins.successPopup("Saved.");
        //TODO: insert panel modify, too.
        task.insertTask(JSON.parse(node.Data));
    },
    get_task_list: function (node) {
        var list = JSON.parse(node.Data);
        for (var i = 0; i < list.length; i++) {
            task.insertTask(list[i]);
        }
    },
    error: function (node) {
        ins.errorPopup(node.Data);
    }
};

var menu_list = (function (obj) {
    $(obj.onLoad);
    return obj;
})({
    onLoad: function () {
        $(document).on("click", ".memo-list#btn_new_memo", function () {
            ins.loadPage("memoinsert");
        });
        $(document).on("click", ".memo-list-item", function () {
            //modify
            var key = $(this).val();
            ins.loadPage("memoinsert", key);
        });
    }
});

var menu_insert = (function (obj) {
    $(obj.onLoad);
    return obj;
})({
    onLoad: function () {
        $(document).on("keydown", "input.memo-insert, textarea.memo-insert", function () {
            ins.page_move = false;
        });
        $(document).on("click", ".memo-insert#btn_save", function () {
            $("#memo-contents").val($("#summernote").summernote("code"));
            var data = $("#formdata").serialize();
            var node = new Node(2, "set_memo_insert", data);
            ins.send(node.toJson());
        });
        $(document).on("click", ".memo-insert#btn_menu", function () {
            ins.loadPage("memolist");
        });
        $(document).on("click", ".memo-insert#btn_modify", function () {
            $("#memo-contents").val($("#summernote").summernote("code"));
            var data = $("#formdata").serialize();
            var node = new Node(2, "set_memo_modify", data);
            ins.send(node.toJson());
        });
        $(document).on("click", ".memo-insert#btn_delete", function () {
            var node = new Node(2, "excute_memo_delete", $("#memo_idx").val());
            ins.send(node.toJson());
        });
    }
});

var setting = (function (obj) {
    $(obj.onLoad);
    return obj;
})({
    onLoad: function () {
        $(document).on("click", ".setting#btn_save", function () {

            var data = {
                Port: $(".setting#port").val(),
                Size: $(".setting#size").val(),
                Start: $(".setting#start")[0].checked ? $(".setting#start")[0].value : $(".setting#start")[1].value
            }
            var json = JSON.stringify(data);
            var node = new Node(2, "set_setting", json);
            ins.send(node.toJson());
        });
    }
});

var task = (function (obj) {
    $(obj.onLoad);
    return obj;
})({
    onLoad: function () {
        //$(".task-item").draggable(task.dragevent);
        $(document).on("click", ".task-item", function () {
            $(".task-item.task-active").removeClass("task-active");
            $(this).addClass("task-active");
        });
        $(document).on("click", ".task-menu#btn_save", function () {
            var data = $("#form-data").serialize();
            var node = new Node(2, "set_task_insert", data);
            ins.send(node.toJson());
        });
    },
    insertTask: function (node) {
        var div = $("<div></div>").addClass("task-item-panel");
        if (node.Importance === 1) {
            div.addClass("task-high");
        } else if (node.Importance === 2) {
            div.addClass("task-middle");
        } else if (node.Importance === 3) {
            div.addClass("task-low");
        }
        div.append("<input type='hidden' id='idx' name='idx' value='" + node.Idx + "'>");
        div.append("<input type='hidden' id='title' name='title' value='" + node.Title + "'>");
        div.append("<textarea style='display:none;'>" + node.Contents + "</textarea>");
        div.append("<input type='hidden' id='importance' name='importance' value='" + node.Importance + "'>");
        div.append("<input type='hidden' id='tasktype' name='tasktype' value='" + node.Tasktype + "'>");
        div.append("<input type='hidden' id='taskdate' name='taskdate' value='" + node.Taskdate + "'>");
        div.append(node.Title);
        var item = $("<li></li>").addClass("task-item").append(div);
        $(".task-list").each(function () {
            var data = $(this).attr("data");
            if (parseInt(data) === node.Tasktype) {
                $(this).append(item);
                setTimeout(function () {
                    $(item).draggable(task.dragevent);
                }, 1);
            }
        });
    },
    calculation: function (pos, target) {
        if (!(target.offset().left <= pos.x)) {
            return false;
        }
        if (!(pos.x <= (target.offset().left + target.width()))) {
            return false;
        }
        if (!(target.offset().top <= pos.y)) {
            return false;
        }
        if (!(pos.y <= (target.offset().top + target.height()))) {
            return false;
        }
        return true;
    },
    dragevent: {
        start: function () {
            $(this).css("z-index", "100");
            $(".task-item.task-active").removeClass("task-active");
            $(this).addClass("task-active");
        },
        drag: function (e) {
            var pos = {};
            pos.x = $(this).offset().left + ($(this).width() / 2);
            pos.y = $(this).offset().top + ($(this).height() / 2);
            $(".task-list").each(function () {
                $(this).parent().css("border-color", "");
                $(this).parent().children("div").css("background-color", "");
                if (task.calculation(pos, $(this))) {
                    $(this).parent().css("border-color", "#46b8da");
                    $(this).parent().children("div").css("background-color", "#5bc0de");
                }
            });
        },
        stop: function () {
            $(this).css("z-index", "");
            var isMove = false;
            var target = $(this);
            var pos = {};
            pos.x = $(this).offset().left + ($(this).width() / 2);
            pos.y = $(this).offset().top + ($(this).height() / 2);
            var maxsize = 0;
            $(".task-list").each(function () {
                $(this).parent().css("border-color", "");
                $(this).parent().children("div").css("background-color", "");
                if (task.calculation(pos, $(this))) {
                    //console.log($(this).attr("data"));
                    target.remove();
                    var temp = target[0];
                    $(this).append($(temp));
                    $(temp).css("left", "");
                    $(temp).css("top", "");
                    setTimeout(function () {
                        $(temp).draggable(task.dragevent);
                    }, 1);
                    isMove = true;
                    //console.log($(temp).find("input").val());
                }
                $(this).css("height", "");
                if (maxsize <= $(this).height()) {
                    maxsize = $(this).height();
                }
            });
            if (!isMove) {
                target.css("left", "");
                target.css("top", "")
            }
            //sort
            $(".task-list").each(function () {
                if ($(this).height() < maxsize) {
                    $(this).height(maxsize);
                }
            });
        }
    }
});