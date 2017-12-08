const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const autoprefixer = require('autoprefixer');
const webpackHtml = require('html-webpack-plugin');

module.exports = (outputPath, srcPath, publicPath) => {

    const plugins = [];

    const config = {
        devtool: '#eval-source-map',
        entry: {},
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