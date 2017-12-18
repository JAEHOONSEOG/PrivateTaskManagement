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
        var ws = new WebSocket("ws://localhost:9999/menu");
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
        };
        ins.send = function (message) {
            ws.send(message);
        };
        $(window).on("beforeunload", function (e) {
            if (!ins.page_move) {
                var e = e || window.event;
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
        if (temp.Type == 1 && temp.ResponseKey !== undefined && temp.ResponseKey !== null) {
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
        $(document).on("click", "#CalendarPanel,#CalendarMenu", function () {
            ins.loadPage("cardmenu");
        });
        $(document).on("click", "#TaskPanel,#TaskMenu", function () {
            ins.loadPage("cardmenu");
        });
        $(document).on("click", "#MemoPanel,#MemoMenu", function () {
            ins.loadPage("memolist");
        });
        $(document).on("click", "#SettingsPanel,#SettingsMenu", function () {
            ins.loadPage("cardmenu");
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
    get_memo_list: function (node) {
        var list = Node.prototype.toObject(node.Data);
        //<button class="list-group-item memo-list-item" value="1">First item<span class="badge">2015/01/01</span></button>
        for (var i = 0; i < list.length; i++) {
            var button = $("<button></button>").addClass("list-group-item").addClass("memo-list-item").val(list[i].Idx);
            button.append(list[i].Title);
            button.append($("<span></span>").addClass("badge").append(list[i].RecentlyDate))
            $(".memo-list#list").append(button);
        }
    },
    set_memo_insert: function (node) {
        console.log(node);
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
        $(".memo-insert#summernote").summernote("code",decodeURIComponent(temp.Contents));
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
            ins.errorPopup("test");
        });
        $(document).on("click", ".memo-insert#btn_delete", function () {
            ins.errorPopup("test");
        });
    }
});
