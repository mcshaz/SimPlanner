"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var webpack = require("webpack");
var ExtractTextPlugin = require("extract-text-webpack-plugin");
var ScriptExtHtmlWebpackPlugin = require("script-ext-html-webpack-plugin");
var HtmlWebpackPlugin = require("html-webpack-plugin");
var UglifyJsPlugin = require("uglifyjs-webpack-plugin");
var path = require("path");
var isDevelopment = true, srcPath = path.join(__dirname, '/app'), distPath = path.join(__dirname, '/wwwroot');
var config = {
    cache: true,
    devtool: 'source-map',
    context: srcPath,
    entry: {
        'app': [
            './entry.ts',
            './styles.ts',
        ],
        'update-progress': [
            './loading/update-progress.js',
            './loading/notify-update-browser.js'
        ]
    },
    output: {
        publicPath: "/",
        path: distPath,
        filename: '[name].bundle.js',
    },
    resolve: {
        extensions: ['.ts', '.js', '.json'],
        modules: ["node_modules"],
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                loader: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.css?$/,
                use: ExtractTextPlugin.extract({
                    loader: 'css-loader',
                    options: {
                        outputPath: 'css/',
                        minimize: !isDevelopment
                    },
                })
            },
            {
                test: /\.(png|jpg|eot|ttf|svg|woff|woff2|gif)$/,
                use: [
                    {
                        loader: "file-loader",
                        options: {
                            outputPath: 'assets/',
                            name: '[name].[ext]',
                        }
                    }
                ]
            }
        ]
    },
    plugins: [
        new ExtractTextPlugin('css/[name].' + (isDevelopment ? 'dev' : 'min') + '.css'),
        new HtmlWebpackPlugin({
            template: '../index.html'
        }),
        new ScriptExtHtmlWebpackPlugin({
            inline: 'update-progress',
            defaultAttribute: 'async'
        })
    ].concat((isDevelopment ? [] : [
        new UglifyJsPlugin({
            sourceMap: true,
            uglifyOptions: {
                output: {
                    ascii_only: true,
                }
            }
        }),
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: '"production"'
            }
        })
    ]))
};
exports.default = config;
//# sourceMappingURL=webpack.config.js.map