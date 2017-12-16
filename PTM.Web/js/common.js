var Node = function(type = -1, key = null, data = null) {
	this.Type = type;
	this.Key = key;
	this.Data = data;
};
$.extend(Node.prototype,{
	toJson: function() {
		return JSON.stringify(this);
	},
	toObject: function(obj) {
		var temp = JSON.parse(obj);
		temp.fn = Node.fn;
		return temp;
	}
});
var ins = (function(obj) {
	$(obj.onLoad);	
	return obj;
})({
	page_move: true,
	onLoad: function() {
		var ws = new WebSocket("ws://localhost:9999/menu");
		ws.onopen = function() {
			if(sub !== undefined && sub.init !== undefined && typeof sub.init === "function") {
				sub.init.call(this);
			}
		};
		ws.onclose = function() {
			ins.errorPopup("Connection closed.");
		};
		ws.onerror = function() {
			ins.errorPopup("Socket error.");
		};
		ws.onmessage = function(message) {
			ins.message(ws, message);
		};
		ins.send = function(message) {
			ws.send(message);
		};
		$(window).on("beforeunload", function(e){
			if(!ins.page_move) {
				var e = e || window.event;
				console.log(e);
				var msg = "Are you sure you want to navigate away from this page?\n\nYou have started writing or editing a post.\n\nPress OK to continue or Cancel to stay on the current page.";
				if (e) {
					e.returnValue = msg;
				}
				return msg;
			}
		});
	},
	message: function(ws, message) {
		var temp = JSON.parse (message.data);
		
		if(sub !== undefined && sub[temp.Key] !== undefined && typeof sub[temp.Key] === "function") {
			sub[temp.Key].call(this, temp);
		}
	},
	loadPage: function(name, param) {
		if(!ins.page_move) {
			if(!confirm("Are you sure you want to navigate away from this page?\n\nYou have started writing or editing a post.\n\nPress OK to continue or Cancel to stay on the current page.")) {
				return false;
			}
		}
		var node = new Node(1, name, param);
		ins.send(node.toJson());
	},
	loadContents: function(data) {
		ins.clearContents();
		$("#contents").append(data);
	},
	clearContents: function() {
		$("#contents").html("");
	},
	errorPopup: function(msg) {
		//https://blackrockdigital.github.io/startbootstrap-sb-admin-2/pages/panels-wells.html
		ins.popup(msg, "panel-danger");
	},
	successPopup: function(msg) {
		ins.popup(msg, "panel-success");
	},
	popup: function(msg,clz) {
		var zone = $(".message-zone");
		var panel = $("<div></div>");
		panel.addClass("panel");
		panel.addClass(clz);
		panel.append($("<div></div>").addClass("panel-heading").append(msg));
		zone.append(panel);
		setTimeout(function(d) {
			d.slideUp("slow", function() {
				d.remove();
			});
		},3000,panel);
	}
});