const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const babelMinifyPlugin = require("babel-minify-webpack-plugin");
const autoprefixer = require('autoprefixer');
const webpackBase = require('./webpack-base');
const webpackHtml = require('html-webpack-plugin');
const webpackExtractTextPlugin = require('extract-text-webpack-plugin');

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

    // emit all css to an external file
    const extractTextRule = {
        exclude: /node_modules/,
        test: /mount\.scss$/,
        use: webpackExtractTextPlugin.extract({
            fallback: 'style-loader',
            use: ['css-loader', 'sass-loader']
        })
    };

    config.module.rules.splice(1, 0, extractTextRule);

    // clear plugins
    config.plugins.length = 0;

    config.plugins.push(new webpackExtractTextPlugin('mount.css'));

    // add entry points to be built
    config.entry = {
        mount: [
            path.resolve(srcPath, './mount/mount.ts')
        ]
    };

    return config;
}

function createAreas() {

    const config = webpackBase(outputPath, srcPath, '/');

    // emit site css to an external file
    const extractTextRuleSite = {
        exclude: /node_modules/,
        test: /\.scss$/,
        use: webpackExtractTextPlugin.extract({
            fallback: 'style-loader',
            use: ['css-loader', 'sass-loader']
        })
    };

    config.module.rules.splice(1, 0, extractTextRuleSite);

    config.plugins.push(new webpackExtractTextPlugin('[name].css'));

    config.plugins.push(new webpackHtml({
        chunksSortMode: 'dependency',
        excludeChunks: ['admin'],
        favicon: path.resolve(srcPath, './root/favicon.ico'),
        filename: 'index.html',
        inject: false,
        template: path.resolve(srcPath, './root/root.ejs')
    }));

    config.plugins.push(new webpackHtml({
        chunksSortMode: 'dependency',
        excludeChunks: ['app'],
        favicon: path.resolve(srcPath, './admin/favicon.ico'),
        filename: 'admin.html',
        inject: false,
        template: path.resolve(srcPath, './admin/admin.ejs')
    }));

    config.plugins.push(new webpack.optimize.CommonsChunkPlugin({
        name: 'vendor'
    }));

    config.plugins.push(new webpack.optimize.CommonsChunkPlugin({
        name: 'common',
        minChunks: 2
    }));

    config.entry = {
        vendor: [
            'vue', 'vue-router', 'vuex', 'vuelidate', 'vue-i18n'
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

function extendConfig(config) {

    // remove source maps
    delete config.devtool;

    const providePlugin =  new webpack.ProvidePlugin({
        Popper: ['popper.js', 'default']
    })

    const definePlugin = new webpack.DefinePlugin({
        'process.env.NODE_ENV': '"production"'
    });

    config.plugins.splice(0, 0, providePlugin);
    config.plugins.splice(0, 0, definePlugin);

    config.plugins.push(new babelMinifyPlugin());

    const tsLoader = {
        test: /\.ts$/,
        include: /app/,
        use: ['babel-loader', 'ts-loader']
    };

    config.module.rules.splice(1, 0, tsLoader);
}