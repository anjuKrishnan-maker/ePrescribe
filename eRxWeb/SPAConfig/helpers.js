var path = require('path');

var _root = path.resolve(__dirname, '..');

function root(args) {
    //console.log(_root + ': '+args);
    args = Array.prototype.slice.call(arguments, 0);
    var x = path.join.apply(path, [_root].concat(args));
    //console.log(x);
    return x;
}

exports.root = root;