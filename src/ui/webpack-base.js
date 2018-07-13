const path = require('path');
const webpack = require('webpack');
const webpackHtml = require('html-webpack-plugin');

module.exports = (outputPath, srcPath, publicPath) => {

    const config = {
        mode: null,
        devtool: '#eval-source-map',
        entry: {
            vendor: [
                'vue', 'vue-router', 'vuex', 'vuelidate', 'vue-i18n', 'bootstrap'
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
                    test: /\.ts$/,
                    loader: 'lodash-ts-imports-loader',
                    exclude: /node_modules/,
                    enforce: "pre"
                },
                {
                    test: /\.ejs$/,
                    use: {
                        loader: 'ejs-loader'
                    }
                },
                {
                    test: /\.html$/,
                    use: { loader: 'vue-html-loader' }
                },
                {
                    test: /\.css$/,
                    use: ['css-loader']
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
                    test: /\.svg(\?v=\d+\.\d+\.\d+)?$/,
                    use: {
                        loader: 'url-loader',
                        options: {
                            limit: 10000,
                            mimetype: 'image/svg+xml'
                        }
                    }
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
                    test: /\.(woff|woff2)$/,
                    use: {
                        loader: 'url-loader',
                        options: {
                            prefix: 'font',
                            limit: 50000
                        }
                    }
                }
            ]
        },
        optimization: {},
        output: {
            path: outputPath,
            publicPath: publicPath,
            filename: '[name].js'
        },
        plugins: [
            new webpack.ProvidePlugin({
                $: 'jquery',
                jQuery: 'jquery',
                bootstrap: ['bootstrap/js/dist', 'default'],
                Popper: ['popper.js', 'default']
            }),
            new webpackHtml({
                chunksSortMode: 'dependency',
                excludeChunks: ['admin'],
                favicon: path.resolve(srcPath, './root/favicon.ico'),
                filename: 'index.html',
                inject: false,
                template: path.resolve(srcPath, './root/root.ejs')
            }),
            new webpackHtml({
                chunksSortMode: 'dependency',
                excludeChunks: ['app'],
                favicon: path.resolve(srcPath, './admin/favicon.ico'),
                filename: 'admin.html',
                inject: false,
                template: path.resolve(srcPath, './admin/admin.ejs')
            })
        ],
        resolve: {
            alias: {
                'assets': path.resolve(srcPath, './assets'),
                'vue$': path.resolve(__dirname, './node_modules/vue/dist/vue.esm.js')
            },
            extensions: ['.js', '.ts', '.html'],
            enforceExtension: false,
            modules: [path.resolve(__dirname, './node_modules')]
        },
        target: 'web'
    };

    return config;
}