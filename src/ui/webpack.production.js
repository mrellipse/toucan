const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const autoprefixer = require('autoprefixer');
const webpackHtml = require('html-webpack-plugin');
const webpackBase = require('./webpack-base');

module.exports = (env) => {

    const outputPath = path.resolve(__dirname, env.outputPath || '../../dist/wwwroot');

    const srcPath = path.resolve(__dirname, env.srcPath || './app');

    const api = "'" + env.api + "'";

    const config = webpackBase(outputPath, srcPath);

    const definePlugin = new webpack.DefinePlugin({
        'process.env.NODE_ENV': "'production'"
    });

    config.plugins.splice(0, 0, definePlugin);
    
    const uglifyJsPlugin = new webpack.optimize.UglifyJsPlugin({
        compressor: {
            warnings: false
        },
        sourceMap: true
    })
    config.plugins.splice(config.plugins.length - 1, 0, uglifyJsPlugin);

    const tsLoader = {
        test: /\.ts$/,
        include: /app/,
        use: ['babel-loader', 'ts-loader']
    };

    config.module.rules.splice(1, 0, tsLoader);

    return [config];
}