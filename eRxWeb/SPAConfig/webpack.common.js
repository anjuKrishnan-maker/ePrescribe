var webpack = require('webpack');

var MiniCssExtractPlugin = require('mini-css-extract-plugin');
var helpers = require('./helpers');
var config = {
        resolve: {
        extensions: ['.ts', '.js']
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
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
                    helpers.root('SPA', 'not_exist_path')

                ],
                loader: MiniCssExtractPlugin.loader
            },
            {
                test: /\.css$/,
                include: helpers.root('src', 'app'),
                loader: 'raw-loader'
            }
        ]
    },

    plugins: [
        // Workaround for angular/angular#11580
        new webpack.ContextReplacementPlugin(
            // The (\\|\/) piece accounts for path separators in *nix and Windows
            /angular(\\|\/)core(\\|\/)@angular/,
            helpers.root(''), // location of your src
            {} // a map of your routes
        )

    ]
};

var mainAppConfig = Object.assign({}, config, {
    entry: {
        'polyfills': './SPA/polyfills.ts',
        'vendor': './SPA/vendor.ts',
        'app': './SPA/boot.ts'
    },
    output: {
        path: helpers.root('dist'),
        publicPath: '/SPA/dist/',
        filename: '[name].[chunkhash].js'
    }
});

var rootAppConfig = Object.assign({}, config, {
    entry: {
        'polyfills': './SPA/polyfills.ts',
        'vendor': './SPA/vendor.ts',
        'rootapp': ['./SPARegistration/src/main.ts']
    },
    output: {
        path: helpers.root('dist'),
        publicPath: '/SPARegistration/dist/',
        filename: '[name].[chunkhash].js'
    }
});

module.exports = [mainAppConfig, rootAppConfig];