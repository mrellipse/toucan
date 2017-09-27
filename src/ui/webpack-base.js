const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const autoprefixer = require('autoprefixer');
const webpackHtml = require('html-webpack-plugin');

module.exports = (outputPath, srcPath, publicPath) => {

    const plugins = [

        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            'window.jQuery': 'jquery',
            Popper: ['popper.js', 'default']
        }),

        new webpack.LoaderOptionsPlugin({
            options: { postcss: [autoprefixer] }
        }),

        new webpack.optimize.CommonsChunkPlugin({
            name: 'common',
            minChunks: 2
        }),

        new webpackHtml({
            chunksSortMode: 'dependency',
            excludeChunks: ['admin'],
            favicon: path.resolve(srcPath, './root/favicon.ico'),
            filename: 'index.html',
            inject: 'body',
            template: path.resolve(srcPath, './root/root.html')
        }),

        new webpackHtml({
            chunksSortMode: 'dependency',
            excludeChunks: ['app'],
            favicon: path.resolve(srcPath, './admin/favicon.ico'),
            filename: 'admin.html',
            inject: 'body',
            template: path.resolve(srcPath, './admin/admin.html')
        })
    ];

    const config = {
        devtool: '#eval-source-map',
        entry: {
            vendor: [
                path.resolve(__dirname, './node_modules/popper.js/dist/popper.js'),
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
        },
        module: {
            rules: [
                {
                    test: /\.html$/,
                    use: { loader: 'vue-html-loader' }
                },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'url-loader',
                        options: {
                            prefix: 'img',
                            limit: 10000
                        }
                    }
                },
                {
                    test: /\.scss$/,
                    exclude: /node_modules/,
                    use: [
                        { loader: "style-loader" },
                        { loader: "css-loader" },
                        { loader: "sass-loader" }
                    ]
                },
                {
                    test: /\.eot(\?v=\d+\.\d+\.\d+)?$/,
                    use: {
                        loader: "file-loader",
                        options: {
                            name: '[name].[ext]',
                            publicPath: '/assets/fonts'
                        }
                    }
                },
                {
                    test: /\.(woff|woff2)$/,
                    use: {
                        loader: 'url-loader',
                        options: {
                            prefix: 'font',
                            limit: 10000
                        }
                    }
                },
                {
                    test: /\.ttf(\?v=\d+\.\d+\.\d+)?$/,
                    use: {
                        loader: 'url-loader',
                        options: {
                            prefix: 'font',
                            limit: 10000,
                            mimetype: 'application/octet-stream'
                        }
                    }
                },
                {
                    test: /\.svg(\?v=\d+\.\d+\.\d+)?$/,
                    use: {
                        loader: 'url-loader',
                        options: {
                            limit: 10000,
                            mimetype: 'image/svg+xml'
                        }
                    }
                }
            ]
        },
        output: {
            path: outputPath,
            publicPath: publicPath,
            filename: '[name].js'
        },
        plugins: plugins,
        resolve: {
            alias: {
                'src': path.resolve(srcPath, './src'),
                'assets': path.resolve(srcPath, './assets'),
                'components': path.resolve(srcPath, './components'),
                'state': path.resolve(srcPath, './state'),
                'vue$': path.resolve(__dirname, './node_modules/vue/dist/vue.js')
            },
            extensions: ['.js', '.ts', '.html'],
            enforceExtension: false,
            modules: [path.resolve(__dirname, './node_modules')]
        },
        target: 'web'
    };

    return config;
}