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
	onLoad: function() {
		var ws = new WebSocket("ws://localhost:9999/menu");
		ws.onopen = function() {
			console.log("ws open");
			if(sub !== undefined && sub.init !== undefined && typeof sub.init === "function") {
				sub.init.call(this);
			}
		};
		ws.onclose = function() {
			console.log("ws close");
		};
		ws.onerror = function() {
			console.log("ws error");
		};
		ws.onmessage = function(message) {
			ins.message(ws, message);
		};
		ins.send = function(message) {
			ws.send(message);
		};
	},
	message: function(ws, message) {
		console.log(message);
		var temp = JSON.parse (message.data);
		
		if(temp.Type === 1) {
			if(sub !== undefined && sub[temp.Key] !== undefined && typeof sub[temp.Key] === "function") {
				sub[temp.Key].call(this, temp.Data);
			}
		}
	},
	loadPage: function(name, param) {
		var node = new Node(1, name, param);
		ins.send(node.toJson());
	},
	loadContents: function(data) {
		ins.clearContents();
		$("#contents").append(data);
	},
	clearContents: function(){
		$("#contents").html("");
	}
});