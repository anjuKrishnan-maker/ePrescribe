
import '../node_modules/core-js/es6/typed';
import '../node_modules/core-js/fn/typed';
import '../node_modules/core-js/fn/typed/array-buffer';
import '../node_modules/core-js/es6/symbol';
import '../node_modules/core-js/es6/object';
import '../node_modules/core-js/es6/function';
import '../node_modules/core-js/es6/parse-int';
import '../node_modules/core-js/es6/parse-float';
import '../node_modules/core-js/es6/number';
import '../node_modules/core-js/es6/math';
import '../node_modules/core-js/es6/string';
import '../node_modules/core-js/es6/date';
import '../node_modules/core-js/es6/array';
import '../node_modules/core-js/es6/regexp';
import '../node_modules/core-js/es6/map';
import '../node_modules/core-js/es6/weak-map';
import '../node_modules/core-js/es6/set';
import '../SPA/Script/e6-custom-polyfill';

import '../node_modules/core-js/es6/reflect';
import '../node_modules/classList.js';
import '../node_modules/core-js/es7/object';
import '../node_modules/core-js/es7/array';
import '../node_modules/core-js/es7/reflect';

if (!Element.prototype.matches) {
    Element.prototype.matches = (<any>Element.prototype).msMatchesSelector ||
        Element.prototype.webkitMatchesSelector;
}

import '../node_modules/zone.js/dist/zone';

//if (process.env.ENV === 'production') {
//    // Production
//} else {
//    // Development and test
//    Error['stackTraceLimit'] = Infinity;
//    require('../node_modules/zone.js/dist/long-stack-trace-zone');
//}