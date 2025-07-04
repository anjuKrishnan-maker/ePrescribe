var webpack = require('webpack');
var helpers = require('./helpers');
var HardSourceWebpackPlugin = require('hard-source-webpack-plugin');

var config = {
    entry: {
        'polyfills': './SPA/polyfills.ts',
        'vendor': './SPA/vendor.ts',
        'app': ['./SPA/boot.ts'],
        'rootapp': ['./SPARegistration/src/main.ts']
    },
    devtool: "eval",
    resolve: {
        extensions: ['.ts', '.js']
    },
    module: {
        rules: [
            {
                test: /\.ts$/, exclude: [/\.(spec|e2e)\.ts$/],
                loaders: [
                    {
                        loader: 'thread-loader',

                        options: {
                            // there should be 1 cpu for the fork-ts-checker-webpack-plugin
                            // workers: require('os').cpus().length - 1,
                            workers: 3
                        }
                    },
                    {
                        loader: 'ts-loader',
                        options: {
                            // disable type checker - we will use it in fork plugin
                            transpileOnly: true,
                            happyPackMode: true
                        }
                    }, 'angular2-template-loader'
                ]
            },
            /* Embed files. */
            {
                test: /\.(html|css)$/,
                loader: 'raw-loader',
                exclude: /\.async\.(html|css)$/,
                options: {
                    minimize: true,
                    removeAttributeQuotes: false,
                    caseSensitive: true,
                    customAttrSurround: [[/#/, /(?:)/], [/\*/, /(?:)/], [/\[?\(?/, /(?:)/]],
                    customAttrAssign: [/\)?\]?=/]
                }
            },
            {
                test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
                loader: 'file-loader?name=assets/[name].[hash].[ext]'
            },
            {
                test: /\.css$/,
                exclude: helpers.root('..', 'Style'),
                include: [
                    helpers.root('./SPA/dist', '/'),
                    helpers.root('./SPARegistration/dist', '/')
                ],
                use: ['css-loader', 'sass-loader']
            },
            {
                test: /\.css$/,
                include: helpers.root('src', 'app'),
                loader: 'raw-loader'
            }
        ]
    },
    devServer: {
        hot: true,
        headers: { "Access-Control-Allow-Origin": "*" },
        stats: { colors: true }
    },
    plugins: [
        new HardSourceWebpackPlugin(),
        new webpack.HotModuleReplacementPlugin(),
        new webpack.NoEmitOnErrorsPlugin()
    ]
};

var mainAppConfig = Object.assign({}, config, {
    entry: {
        'polyfills': './SPA/polyfills.ts',
        'vendor': './SPA/vendor.ts',
        'app': ['./SPA/boot.ts']
    },
    output: {
        path: helpers.root('./SPA/dist'),
        publicPath: 'http://localhost:3000/static/',
        hotUpdateChunkFilename: 'hot/hot-update.js',
        hotUpdateMainFilename: 'hot/hot-update.json'
    }
});
var rootAppConfig = Object.assign({}, config, {
    entry: {
        'polyfills': './SPA/polyfills.ts',
        'vendor': './SPA/vendor.ts',
        'rootapp': ['./SPARegistration/src/main.ts']
    },
    output: {
        path: helpers.root('./SPARegistration/dist'),
        publicPath: 'http://localhost:3000/static/',
        hotUpdateChunkFilename: 'hot/hot-update.js',
        hotUpdateMainFilename: 'hot/hot-update.json'
    }
});

module.exports = [mainAppConfig, rootAppConfig];