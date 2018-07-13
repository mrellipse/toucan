const path = require('path');
const webpack = require('webpack');
const webpackBase = require('./webpack-base');
const miniCssExtractPlugin = require("mini-css-extract-plugin")

const outputPath = path.join(__dirname, '../../dist/wwwroot');
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

    const config = webpackBase(outputPath, srcPath, '/');

    // remove source maps
    delete config.devtool;

    // clear plugins
    config.plugins.length = 0;

    // emit css to an external file
    var cssExtractPlugin = new miniCssExtractPlugin({
        filename: "mount.css"
    })

    config.plugins.push(cssExtractPlugin);

    // switch entry point to be built
    config.entry = {
        mount: [
            path.resolve(srcPath, './mount/mount.ts')
        ]
    };

    return config;
}

function createAreas() {

    const config = webpackBase(outputPath, srcPath, '/');

    // remove source maps
    delete config.devtool;

    // emit css to an external file
    var cssExtractPlugin = new miniCssExtractPlugin({
        filename: "[name].css"
    })

    config.plugins.push(cssExtractPlugin);
    
    config.optimization = {
        splitChunks: {
            chunks: 'async',
            minSize: 30000,
            minChunks: 1,
            maxAsyncRequests: 5,
            maxInitialRequests: 5,
            automaticNameDelimiter: '~',
            name: true,
            cacheGroups: {
                vendors: {
                    name: 'vendor',
                    test: /[\\/]node_modules[\\/]/,
                    priority: -10
                },
                styles: {
                    name: 'styles',
                    test: /\.css$/,
                    chunks: 'all',
                    enforce: true
                },
                default: {
                    name: 'common',
                    chunks: 'initial',
                    minChunks: 2
                }
            }
        }
    }

    return config;
}

function extendConfig(config) {

    config.mode = 'production';

    // remove source maps
    delete config.devtool;

    const definePlugin = new webpack.DefinePlugin({
        'process.env.NODE_ENV': '"production"'
    });

    const noErrorsPlugin = new webpack.NoEmitOnErrorsPlugin()

    config.plugins.splice(0, 0, definePlugin, noErrorsPlugin);

    const tsLoader = {
        test: /\.ts$/,
        include: /app/,
        use: ['babel-loader', 'ts-loader']
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