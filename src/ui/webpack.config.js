const path = require('path');
const webpack = require('webpack');
const webpackBase = require('./webpack-base');
const miniCssExtractPlugin = require("mini-css-extract-plugin")

const outputPath = path.join(__dirname, '../server/wwwroot');
const srcPath = path.resolve(__dirname, './app');

module.exports = () => {

    let configs = [
        createMount(),
        createAreas()
    ];

    configs.forEach(o => extendConfig(o));

    return configs;
}

// emits a small webpack file for bootstrapping the application (displays a loading animation until required resources are loaded)
function createMount() {

    // set publicPath to '/mount/' so that .net core webpack middleware can proxy incoming requests to node.js middleware
    const config = webpackBase(outputPath, srcPath, '/mount/');

    // remove source maps
    delete config.devtool;

    // clear plugins
    config.plugins.length = 0;

    // emit css to an external file
    var cssExtractPlugin = new miniCssExtractPlugin({
        filename: "mount.css"
    })

    config.plugins.push(cssExtractPlugin);

    // add entry points to be built
    config.entry = {
        mount: [
            path.resolve(srcPath, './mount/mount.ts')
        ]
    };

    return config;
}

// emits the root and admin area applications
function createAreas() {
    // set publicPath to '/dist/' so that .net core webpack middleware can proxy incoming requests to node.js middleware
    const config = webpackBase(outputPath, srcPath, '/dist/');

    // emit css to an external file
    var cssExtractPlugin = new miniCssExtractPlugin({
        filename: "[name].css"
    })

    config.plugins.push(cssExtractPlugin);

    return config;
}

function extendConfig(config) {

    config.mode = 'development';

    const definePlugin = new webpack.DefinePlugin({
        'process.env.NODE_ENV': '"development"'
    });

    const noErrorsPlugin = new webpack.NoEmitOnErrorsPlugin()

    config.plugins.splice(0, 0, definePlugin, noErrorsPlugin);

    const tsLoader = {
        test: /\.ts$/,
        include: /app/,
        use: ['babel-loader', 'vue-hot-typescript-loader', 'ts-loader']
    };

    const cssExtractRule = {
        test: /\.css$/,
        use: [
            miniCssExtractPlugin.loader,
            'css-loader'
        ]
    }

    const scssExtractRule = {
        test: /\.scss$/,
        use: [
            miniCssExtractPlugin.loader,
            'css-loader',
            'sass-loader'
        ]
    }

    config.module.rules.splice(1, 0, tsLoader, cssExtractRule, scssExtractRule);
}