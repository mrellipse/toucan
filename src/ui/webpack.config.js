const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const autoprefixer = require('autoprefixer');
const webpackHtml = require('html-webpack-plugin');
const webpackBase = require('./webpack-base');

module.exports = () => {

    const outputPath = path.join(__dirname, '../server/wwwroot');

    const srcPath = path.resolve(__dirname, './app');

    // set publicPath to '/dist/' is so that .net core webpack middleware can proxy incoming requests to node.js middleware
    const config = webpackBase(outputPath, srcPath, '/dist/');

    const definePlugin = new webpack.DefinePlugin({
        'process.env.NODE_ENV': '"development"'
    });

    config.plugins.splice(0, 0, definePlugin);

    const tsLoader = {
        test: /\.ts$/,
        include: /app/,
        use: ['babel-loader', 'vue-hot-typescript-loader', 'ts-loader']
    };

    config.module.rules.splice(1, 0, tsLoader);

    return [config];
}