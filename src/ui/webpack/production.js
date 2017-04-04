var path = require('path');
var webpack = require('webpack');
var merge = require('webpack-merge');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var WebpackCleanupPlugin = require('webpack-cleanup-plugin');

var webpackBase = require('./webpack.base.js');

module.exports = merge(webpackBase, {

  output: {
    path: path.resolve(__dirname, '../../../dist/wwwroot'),
    publicPath: '/',
    filename: '[name].[hash].min.js',
    sourceMapFilename: '[name].[hash].min.js.map',
    chunkFilename: '[id].chunk.min.js',
  },

  plugins: [
    new webpack.DefinePlugin({
      'process.env.NODE_ENV': '"production"'
    }),

    new HtmlWebpackPlugin({
      chunksSortMode: 'dependency',
      excludeChunks: ['app'],
      filename: 'admin.html',
      inject: 'body',
      template: path.resolve(__dirname, '../src/admin/admin.html')
    }),

    new HtmlWebpackPlugin({
      chunksSortMode: 'dependency',
      excludeChunks: ['admin'],
      filename: 'index.html',
      inject: 'body',
      template: path.resolve(__dirname, '../src/root/root.html')
    }),

    new WebpackCleanupPlugin(),

    new webpack.optimize.OccurenceOrderPlugin(),

    new webpack.optimize.UglifyJsPlugin({
      compressor: {
        warnings: false
      }
    }),
  ]
});
