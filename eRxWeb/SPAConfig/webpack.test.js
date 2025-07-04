var helpers = require('./helpers');
var webpack = require('webpack');

module.exports = () => {
    return {
        entry: {
            main: './SPA/boot.ts'
        },
        output: {
            path: './dist',
            filename: '[name].bundle.js'
        },
        resolve: {
            extensions: ['.js', '.ts', '.html']
        },
        module: {
            rules: [
                {
                    test: /\.ts$/,
                    loaders: [
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
                }

            ]
        },
        devtool: 'inline-source-map',
        plugins: [

            new webpack.ProvidePlugin({
                $: 'jquery',
                jQuery: 'jquery',
                'window.jQuery': 'jquery',
                Popper: ['popper.js', 'default']
            }),
            new webpack.ContextReplacementPlugin(
                // The (\\|\/) piece accounts for path separators in *nix and Windows
                /angular(\\|\/)core(\\|\/)@angular/,
                helpers.root(''), // location of your src
                {} // a map of your routes
            )
        ]
    };
};