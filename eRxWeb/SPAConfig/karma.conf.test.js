
module.exports = function (config) {
    console.log("Executing karma.conf.test.js. Hold on I am loading. Give me a minute or two :" + (new Date).toLocaleTimeString());
    var _config = {
        basePath: '',

        frameworks: ['jasmine'],

        plugins: [
            'karma-jasmine',
            'karma-webpack',
            'karma-chrome-launcher',
            'karma-junit-reporter',
            'karma-jasmine-html-reporter'
        ],
        files: [
            { pattern: 'karma-test-shim.js', watched: false },            
        ],

        preprocessors: {
            'karma-test-shim.js': ['webpack'],
        },
        webpack: require('./webpack.test')({ env: 'test' }),

        webpackMiddleware: {
            stats: 'errors-only'
        },
        webpackServer: {
            noInfo: true
        },
        mime: { 'text/x-typescript': ['ts', 'tsx'] },
        reporters: ['junit', 'kjhtml'],
        junitReporter: {
            outputDir: 'testResults',
            outputFile: 'test-results.xml'
        },
        port: 9876,
        colors: true,
        logLevel: config.LOG_LOG,
        autoWatch: true,
        browsers: ['Chrome'],
        singleRun: false
    };

    config.set(_config);
};
