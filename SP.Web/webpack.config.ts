import * as webpack from 'webpack';
import * as ExtractTextPlugin from 'extract-text-webpack-plugin';
import * as ScriptExtHtmlWebpackPlugin from 'script-ext-html-webpack-plugin';
import * as HtmlWebpackPlugin from 'html-webpack-plugin';
import * as UglifyJsPlugin from 'uglifyjs-webpack-plugin';
//import * as CommonsChunkPlugin from 'webpack/lib/optimize/CommonsChunkPlugin';
//import * as HardSourceWebpackPlugin from 'hard-source-webpack-plugin';
//import * as fs from 'fs';
//import * as glob from 'glob'; 
import * as path from 'path';



//const srcPath = path.join(__dirname, '/Scripts'),
//    distPath = path.join(__dirname, '/wwwroot');

const isDevelopment = true,
    srcPath = path.join(__dirname, '/app'),
    distPath = path.join(__dirname, '/wwwroot');


const config: webpack.Configuration = {
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
        extensions: ['.ts', '.js','.json'], //default is .js & .json
        modules: ["node_modules"],
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                loader:'ts-loader',
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
        //new HardSourceWebpackPlugin(),
        //new webpack.NoEmitOnErrorsPlugin(),
        ///It moves all the required *.css modules in entry chunks into a separate CSS file. So your styles are no longer inlined into the JS bundle, but in a separate CSS file (styles.css). If your total stylesheet volume is big, it will be faster because the CSS bundle is loaded in parallel to the JS bundle.
        new ExtractTextPlugin('css/[name].' + (isDevelopment ? 'dev' : 'min') + '.css'),
        /* ...Object.keys(modules).map(function(x){
            return new CommonsChunkPlugin({
                name: x,
                chunks: Object.keys(modules[x]),
                minChunks: 2
            });
        }), ,
        */
        new HtmlWebpackPlugin({
            template: '../index.html'
        }),
        new ScriptExtHtmlWebpackPlugin({
            inline: 'update-progress',
            defaultAttribute: 'async'
        }),
        ...(isDevelopment ? [] : [
            new UglifyJsPlugin({
                sourceMap: true,
                // Needed for TinyMCE, see https://www.tinymce.com/docs/advanced/usage-with-module-loaders/#minificationwithuglifyjs2
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
        ])
    ]
};

export default config;