
module.exports = function (config) {
    var _config = {
        basePath: '',

        frameworks: ['jasmine'],

        plugins: [
            'karma-jasmine',
            'karma-webpack',
            'karma-chrome-launcher',
            'karma-junit-reporter',
            'karma-phantomjs-launcher'
        ],

        files: [
            { pattern: 'karma-test-shim.js', watched: false },
            // { pattern: './SPA/Test/*.spec.js', watched: false },
            { pattern: '../Test/*.spec.ts', included: false }
            //'../**/*.html'
        ],

        preprocessors: {
            'karma-test-shim.js': ['webpack'],
            // './SPA/Test/*.spec.js': ['webpack'],
            '../Test/*.spec.ts': ['webpack']
            // '../**/*.html': 'redirect'

        },

        webpack: require('./webpack.test')({ env: 'test' }),

        webpackMiddleware: {
            stats: 'errors-only'
        },

        webpackServer: {
            noInfo: true
        },
        mime: { 'text/x-typescript': ['ts', 'tsx'] },
        reporters: ['progress','junit'],
        junitReporter: {
            outputDir: 'testResults',
            outputFile: 'test-results.xml'
        },
        port: 9876,
        colors: true,
        logLevel: config.LOG_INFO,
        autoWatch: true,
        browsers: ['PhantomJS'],
        //browsers: ['ChromeHeadless'],
        //customLaunchers: {
        //    ChromeHeadless: {
        //        base: 'Chrome',
        //        flags: [
        //            '--headless',
        //            '--disable-gpu',
        //            // Without a remote debugging port, Google Chrome exits immediately.
        //            '--remote-debugging-port=9222',
        //        ],
        //    }
        //},
        singleRun: true
    };

    config.set(_config);
};
