Error.stackTraceLimit = Infinity;

require('core-js/es6');
require('core-js/es7/reflect');

require('zone.js/dist/zone');
require('zone.js/dist/long-stack-trace-zone');
require('zone.js/dist/proxy');
require('zone.js/dist/sync-test');
require('zone.js/dist/jasmine-patch');
require('zone.js/dist/async-test');
require('zone.js/dist/fake-async-test');

let importAll = (r) => {
    r.keys().forEach(r);
};

importAll(require.context('../SPA', true, /\.spec\.ts/));
importAll(require.context('../SPARegistration', true, /\.spec\.ts/));

