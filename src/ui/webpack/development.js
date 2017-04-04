var path = require('path');
var webpack = require('webpack');
var merge = require('webpack-merge');
var HtmlWebpackPlugin = require('html-webpack-plugin');

var config = require('../package.json').config;
var webpackBase = require('./webpack.base.js');
var WebpackCleanupPlugin = require('webpack-cleanup-plugin');

module.exports = merge(webpackBase, {
  
  devtool: '#eval-source-map',

  output: {
    path: path.resolve(__dirname, '../../server/wwwroot'),
    publicPath: '/',
    filename: '[name].[hash].js',
    sourceMapFilename: '[name].[hash].js.map',
    chunkFilename: '[id].chunk.js',
  },

  plugins: [

    new webpack.DefinePlugin({
      'process.env.NODE_ENV': '"development"'
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

    new webpack.HotModuleReplacementPlugin(),

    new webpack.NoErrorsPlugin()
  ]
});
