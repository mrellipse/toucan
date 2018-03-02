const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const autoprefixer = require('autoprefixer');
const webpackBase = require('./webpack-base');
const webpackHtml = require('html-webpack-plugin');
const webpackExtractTextPlugin = require('extract-text-webpack-plugin');

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

    // emit css to an external file
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

    config.plugins.push(
        new webpackExtractTextPlugin('mount.css')
    );

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

    // emit all css to an external file
    const extractTextRule = {
        exclude: /node_modules/,
        test: /\.scss$/,
        use: webpackExtractTextPlugin.extract({
            fallback: 'style-loader',
            use: ['css-loader', 'sass-loader']
        })
    };

    config.module.rules.splice(0, 0, extractTextRule);

    config.plugins.push(
        new webpackExtractTextPlugin('[name].css')
    );

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

    // add entry points to be built
    config.entry = {
        vendor: [
            'vue', 'vue-router', 'vuex', 'vuelidate', 'vue-i18n', 'bootstrap'
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

    const providePlugin =  new webpack.ProvidePlugin({
        $: 'jquery',
        jQuery: 'jquery',
        bootstrap: ['bootstrap/js/dist', 'default'],
        Popper: ['popper.js', 'default']
    })

    const definePlugin = new webpack.DefinePlugin({
        'process.env.NODE_ENV': '"development"'
    });

    config.plugins.splice(0, 0, providePlugin);
    config.plugins.splice(0, 0, definePlugin);

    const tsLoader = {
        test: /\.ts$/,
        include: /app/,
        use: ['babel-loader', 'vue-hot-typescript-loader', 'ts-loader']
    };

    config.module.rules.splice(1, 0, tsLoader);
}