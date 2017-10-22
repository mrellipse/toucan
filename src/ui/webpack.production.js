const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const autoprefixer = require('autoprefixer');
const webpackHtml = require('html-webpack-plugin');
const webpackBase = require('./webpack-base');

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

function createAreas() {

    const config = webpackBase(outputPath, srcPath, '/');

    config.plugins.push(new webpackHtml({
        chunksSortMode: 'dependency',
        excludeChunks: ['admin'],
        favicon: path.resolve(srcPath, './root/favicon.ico'),
        filename: 'index.html',
        inject: 'body',
        template: path.resolve(srcPath, './root/root.html')
    }));

    config.plugins.push(new webpackHtml({
        chunksSortMode: 'dependency',
        excludeChunks: ['app'],
        favicon: path.resolve(srcPath, './admin/favicon.ico'),
        filename: 'admin.html',
        inject: 'body',
        template: path.resolve(srcPath, './admin/admin.html')
    }));

    config.entry = {
        vendor: [
            path.resolve(__dirname, './node_modules/popper.js/dist/esm/popper.min.js'),
            path.resolve(__dirname, './node_modules/jquery/dist/jquery.js'),
            path.resolve(__dirname, './node_modules/bootstrap/dist/js/bootstrap.js'),
            path.resolve(__dirname, './node_modules/jwt-decode/lib/index.js')
        ],
        admin: [
            path.resolve(srcPath, './admin/admin.ts')
        ],
        app: [
            path.resolve(srcPath, './root/root.ts')
        ]
    };

    return config;
}

// emits a small webpack file for bootstrapping the application (displays a loading animation until required resources are loaded)
function createMount() {

    const config = webpackBase(outputPath, srcPath, '/');

    // clear plugins
    config.plugins.length = 0;

    // add entry points to be built
    config.entry = {
        mount: [
            path.resolve(srcPath, 'mount.ts')
        ]
    };

    return config;
}

function extendConfig(config) {

    // remove source maps
    delete config.devtool;

    const definePlugin = new webpack.DefinePlugin({
        'process.env.NODE_ENV': '"production"'
    });

    config.plugins.splice(0, 0, definePlugin);

    const tsLoader = {
        test: /\.ts$/,
        include: /app/,
        use: ['babel-loader', 'ts-loader']
    };

    config.module.rules.splice(1, 0, tsLoader);
}